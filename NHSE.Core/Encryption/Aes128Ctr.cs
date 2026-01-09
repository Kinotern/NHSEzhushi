using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace NHSE.Core
{
    // The MIT License (MIT)

    // Copyright (c) 2014 Hans Wolff

    // Permission is hereby granted, free of charge, to any person obtaining a copy
    // of this software and associated documentation files (the "Software"), to deal
    // in the Software without restriction, including without limitation the rights
    // to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    // copies of the Software, and to permit persons to whom the Software is
    // furnished to do so, subject to the following conditions:

    // The above copyright notice and this permission notice shall be included in
    // all copies or substantial portions of the Software.

    // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    // IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    // FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    // AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    // LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    // OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    // THE SOFTWARE.

    /// <summary>
    /// AES-128 计数器模式加密算法实现
    /// </summary>
    public sealed class Aes128CounterMode : SymmetricAlgorithm
    {
        /// <summary>
        /// 计数器值
        /// </summary>
        private readonly byte[] _counter;
        /// <summary>
        /// AES 算法实例，使用 ECB 模式和无填充
        /// </summary>
        private readonly AesManaged _aes = new() {Mode = CipherMode.ECB, Padding = PaddingMode.None};

        /// <summary>
        /// 初始化 Aes128CounterMode 实例
        /// </summary>
        /// <param name="counter">计数器值，必须与块大小相同（16字节）</param>
        /// <exception cref="ArgumentException">当计数器大小与块大小不同时抛出</exception>
        public Aes128CounterMode(byte[] counter)
        {
            const int expect = 0x10;
            if (counter.Length != expect)
                throw new ArgumentException($"Counter size must be same as block size (actual: {counter.Length}, expected: {expect})");
            _counter = counter;
        }

        /// <summary>
        /// 创建加密转换器
        /// </summary>
        /// <param name="rgbKey">加密密钥</param>
        /// <param name="ignoredParameter">忽略的参数（计数器模式不需要）</param>
        /// <returns>加密转换器实例</returns>
        public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] ignoredParameter) => new CounterModeCryptoTransform(_aes, rgbKey, _counter);
        /// <summary>
        /// 创建解密转换器
        /// </summary>
        /// <param name="rgbKey">解密密钥</param>
        /// <param name="ignoredParameter">忽略的参数（计数器模式不需要）</param>
        /// <returns>解密转换器实例</returns>
        public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] ignoredParameter) => new CounterModeCryptoTransform(_aes, rgbKey, _counter);

        /// <summary>
        /// 生成密钥
        /// </summary>
        public override void GenerateKey() => _aes.GenerateKey();
        /// <summary>
        /// 生成初始化向量（计数器模式不需要）
        /// </summary>
        public override void GenerateIV() { /* IV not needed in Counter Mode */ }
        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">是否释放托管资源</param>
        protected override void Dispose(bool disposing) => _aes.Dispose();
    }

    /// <summary>
    /// 计数器模式加密转换器
    /// </summary>
    public sealed class CounterModeCryptoTransform : ICryptoTransform
    {
        /// <summary>
        /// 计数器值
        /// </summary>
        private readonly byte[] _counter;
        /// <summary>
        /// 计数器加密器
        /// </summary>
        private readonly ICryptoTransform _counterEncryptor;
        /// <summary>
        /// XOR 掩码队列
        /// </summary>
        private readonly Queue<byte> _xorMask = new();
        /// <summary>
        /// 对称加密算法实例
        /// </summary>
        private readonly SymmetricAlgorithm _symmetricAlgorithm;

        /// <summary>
        /// 初始化 CounterModeCryptoTransform 实例
        /// </summary>
        /// <param name="symmetricAlgorithm">对称加密算法实例</param>
        /// <param name="key">加密密钥</param>
        /// <param name="counter">计数器值，必须与块大小相同</param>
        /// <exception cref="ArgumentException">当计数器大小与块大小不同时抛出</exception>
        public CounterModeCryptoTransform(SymmetricAlgorithm symmetricAlgorithm, byte[] key, byte[] counter)
        {
            if (counter.Length != symmetricAlgorithm.BlockSize / 8)
                throw new ArgumentException($"Counter size must be same as block size (actual: {counter.Length}, expected: {symmetricAlgorithm.BlockSize / 8})");

            _symmetricAlgorithm = symmetricAlgorithm;
            _encryptOutput = new byte[counter.Length];
            _counter = counter;

            var zeroIv = new byte[counter.Length];
            _counterEncryptor = symmetricAlgorithm.CreateEncryptor(key, zeroIv);
        }

        /// <summary>
        /// 转换最后一个数据块
        /// </summary>
        /// <param name="inputBuffer">输入缓冲区</param>
        /// <param name="inputOffset">输入偏移量</param>
        /// <param name="inputCount">输入数据长度</param>
        /// <returns>转换后的数据</returns>
        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            var output = new byte[inputCount];
            TransformBlock(inputBuffer, inputOffset, inputCount, output, 0);
            return output;
        }

        /// <summary>
        /// 转换数据块
        /// </summary>
        /// <param name="inputBuffer">输入缓冲区</param>
        /// <param name="inputOffset">输入偏移量</param>
        /// <param name="inputCount">输入数据长度</param>
        /// <param name="outputBuffer">输出缓冲区</param>
        /// <param name="outputOffset">输出偏移量</param>
        /// <returns>转换的数据长度</returns>
        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            var xm = _xorMask;
            for (var i = 0; i < inputCount; i++)
            {
                if (xm.Count == 0)
                    EncryptCounterThenIncrement();

                var mask = xm.Dequeue();
                outputBuffer[outputOffset + i] = (byte)(inputBuffer[inputOffset + i] ^ mask);
            }

            return inputCount;
        }

        /// <summary>
        /// 加密输出缓冲区
        /// </summary>
        private readonly byte[] _encryptOutput;

        /// <summary>
        /// 加密计数器并递增
        /// </summary>
        private void EncryptCounterThenIncrement()
        {
            var counterModeBlock = _encryptOutput;

            _counterEncryptor.TransformBlock(_counter, 0, _counter.Length, counterModeBlock, 0);
            IncrementCounter();

            var xm = _xorMask;
            foreach (var b in counterModeBlock)
                xm.Enqueue(b);
        }

        /// <summary>
        /// 递增计数器
        /// </summary>
        private void IncrementCounter()
        {
            var ctr = _counter;
            for (var i = ctr.Length - 1; i >= 0; i--)
            {
                if (++ctr[i] != 0)
                    break;
            }
        }

        /// <summary>
        /// 输入块大小
        /// </summary>
        public int InputBlockSize => _symmetricAlgorithm.BlockSize / 8;
        /// <summary>
        /// 输出块大小
        /// </summary>
        public int OutputBlockSize => _symmetricAlgorithm.BlockSize / 8;
        /// <summary>
        /// 是否可以转换多个块
        /// </summary>
        public bool CanTransformMultipleBlocks => true;
        /// <summary>
        /// 是否可以重用转换
        /// </summary>
        public bool CanReuseTransform => false;

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose() => _counterEncryptor.Dispose();
    }
}
