using System;

namespace NHSE.Core
{
    /// <summary>
    /// 动物森友会：新 horizons 使用的 MurmurHash 实现
    /// </summary>
    public static class Murmur3
    {
        /// <summary>
        /// Murmur32 哈希算法的 scrambling 函数
        /// </summary>
        /// <param name="k">要 scrambling 的值</param>
        /// <returns>Scrambled 值</returns>
        private static uint Murmur32_Scramble(uint k)
        {
            k = (k * 0x16A88000) | ((k * 0xCC9E2D51) >> 17);
            k *= 0x1B873593;
            return k;
        }

        /// <summary>
        /// 使用输入参数计算指定偏移量处的哈希值
        /// </summary>
        /// <param name="data">要哈希的数据</param>
        /// <param name="offset">要哈希的数据的起始位置</param>
        /// <param name="size">要哈希的数据量</param>
        /// <param name="seed">初始 Murmur 种子（可选）</param>
        /// <returns>计算得到的哈希值</returns>
        public static uint GetMurmur3Hash(byte[] data, int offset, uint size, uint seed = 0)
        {
            uint checksum = seed;
            if (size > 3)
            {
                for (var i = 0; i < (size / sizeof(uint)); i++)
                {
                    var val = BitConverter.ToUInt32(data, offset);
                    checksum ^= Murmur32_Scramble(val);
                    checksum = (checksum >> 19) | (checksum << 13);
                    checksum = (checksum * 5) + 0xE6546B64;
                    offset += 4;
                }
            }

            var remainder = size % sizeof(uint);
            if (remainder != 0)
            {
                uint val = BitConverter.ToUInt32(data, (int)((offset + size) - remainder));
                for (var i = 0; i < (sizeof(uint) - remainder); i++)
                    val >>= 8;
                checksum ^= Murmur32_Scramble(val);
            }

            checksum ^= size;
            checksum ^= checksum >> 16;
            checksum *= 0x85EBCA6B;
            checksum ^= checksum >> 13;
            checksum *= 0xC2B2AE35;
            checksum ^= checksum >> 16;
            return checksum;
        }

        /// <summary>
        /// 使用输入参数更新指定偏移量处的哈希值
        /// </summary>
        /// <param name="data">要哈希的数据</param>
        /// <param name="hashOffset">写入哈希值的偏移量</param>
        /// <param name="readOffset">要哈希的数据的起始位置</param>
        /// <param name="readSize">要哈希的数据量</param>
        /// <returns>写入数据的计算哈希值</returns>
        public static uint UpdateMurmur32(byte[] data, int hashOffset, int readOffset, uint readSize)
        {
            var newHash = GetMurmur3Hash(data, readOffset, readSize);
            var hashBytes = BitConverter.GetBytes(newHash);
            hashBytes.CopyTo(data, hashOffset);
            return newHash;
        }

        /// <summary>
        /// 检查指定偏移量处的哈希值，看存储的值是否与计算的值匹配
        /// </summary>
        /// <param name="data">要哈希的数据</param>
        /// <param name="hashOffset">哈希值的偏移量</param>
        /// <param name="readOffset">要哈希的数据的起始位置</param>
        /// <param name="readSize">要哈希的数据量</param>
        /// <returns>计算的哈希值是否与当前存储的哈希值匹配</returns>
        public static bool VerifyMurmur32(byte[] data, int hashOffset, int readOffset, uint readSize)
            => BitConverter.ToUInt32(data, hashOffset) == GetMurmur3Hash(data, readOffset, readSize);
    }
}
