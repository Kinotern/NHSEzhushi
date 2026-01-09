using System;

namespace NHSE.Core
{
    /// <summary>
    /// 提供加密和解密功能的静态类
    /// </summary>
    public static class Encryption
    {
        /// <summary>
        /// 从数据中获取加密参数（密钥或计数器）
        /// </summary>
        /// <param name="data">输入数据数组</param>
        /// <param name="index">参数索引</param>
        /// <returns>生成的参数字节数组（16字节）</returns>
        private static byte[] GetParam(uint[] data, in int index)
        {
            var rand = new XorShift128(data[data[index] & 0x7F]);
            var prms = data[data[index + 1] & 0x7F] & 0x7F;

            var rndRollCount = (prms & 0xF) + 1;
            for (var i = 0; i < rndRollCount; i++)
                rand.GetU64();

            var result = new byte[0x10];
            for (var i = 0; i < result.Length; i++)
                result[i] = (byte)(rand.GetU32() >> 24);

            return result;
        }

        /// <summary>
        /// 使用头部数据解密加密数据（原地解密）
        /// </summary>
        /// <param name="headerData">头部数据</param>
        /// <param name="encData">加密的保存数据</param>
        public static void Decrypt(byte[] headerData, byte[] encData)
        {
            // First 256 bytes go unused
            var importantData = new uint[0x80];
            Buffer.BlockCopy(headerData, 0x100, importantData, 0, 0x200);

            // Set up Key
            var key = GetParam(importantData, 0);

            // Set up counter
            var counter = GetParam(importantData, 2);

            // Do the AES
            using var aesCtr = new Aes128CounterMode(counter);
            var transform = aesCtr.CreateDecryptor(key, counter);

            transform.TransformBlock(encData, 0, encData.Length, encData, 0);
        }

        /// <summary>
        /// 生成头部文件，包含版本数据和加密参数
        /// </summary>
        /// <param name="seed">随机种子</param>
        /// <param name="versionData">版本数据</param>
        /// <returns>包含头部数据、密钥和计数器的 CryptoFile</returns>
        private static CryptoFile GenerateHeaderFile(uint seed, byte[] versionData)
        {
            // Generate 128 Random uints which will be used for params
            var random = new XorShift128(seed);
            var encryptData = new uint[128];
            for (var i = 0; i < encryptData.Length; i++)
                encryptData[i] = random.GetU32();

            var headerData = new byte[0x300];
            Buffer.BlockCopy(versionData, 0, headerData, 0, 0x100);
            Buffer.BlockCopy(encryptData, 0, headerData, 0x100, 0x200);
            return new CryptoFile(headerData, GetParam(encryptData, 0), GetParam(encryptData, 2));
        }

        /// <summary>
        /// 使用提供的种子加密保存数据
        /// </summary>
        /// <param name="data">要加密的保存数据</param>
        /// <param name="seed">加密使用的种子</param>
        /// <param name="versionData">加密使用的版本数据</param>
        /// <returns>包含加密数据和关联头部数据的 EncryptedSaveFile</returns>
        public static EncryptedSaveFile Encrypt(byte[] data, uint seed, byte[] versionData)
        {
            // Generate header file and get key and counter
            var header = GenerateHeaderFile(seed, versionData);

            // Encrypt file
            using var aesCtr = new Aes128CounterMode(header.Ctr);
            var transform = aesCtr.CreateEncryptor(header.Key, header.Ctr);
            var encData = new byte[data.Length];
            transform.TransformBlock(data, 0, data.Length, encData, 0);

            return new EncryptedSaveFile(encData, header.Data);
        }
    }
}
