using System;

namespace NHSE.Core
{
    /// <summary>
    /// 加密的32位整数类，提供加密、解密、读取和验证功能
    /// </summary>
    public sealed class EncryptedInt32
    {
        /// <summary>
        /// 用于加密整数的常量值
        /// </summary>
        private const uint ENCRYPTION_CONSTANT = 0x80E32B11;
        /// <summary>
        /// 加密中使用的基础移位计数
        /// </summary>
        private const byte SHIFT_BASE = 3;

        /// <summary>
        /// 原始加密值
        /// </summary>
        public readonly uint OriginalEncrypted;
        /// <summary>
        /// 调整值
        /// </summary>
        public readonly ushort Adjust;
        /// <summary>
        /// 移位值
        /// </summary>
        public readonly byte Shift;
        /// <summary>
        /// 校验和
        /// </summary>
        public readonly byte Checksum;

        /// <summary>
        /// 解密后的值
        /// </summary>
        public uint Value;

        /// <summary>
        /// 将值转换为字符串
        /// </summary>
        /// <returns>解密后值的字符串表示</returns>
        public override string ToString() => Value.ToString();

        /// <summary>
        /// 初始化 EncryptedInt32 实例
        /// </summary>
        /// <param name="encryptedValue">加密的值</param>
        /// <param name="adjust">调整值，默认为0</param>
        /// <param name="shift">移位值，默认为0</param>
        /// <param name="checksum">校验和，默认为0</param>
        public EncryptedInt32(uint encryptedValue, ushort adjust = 0, byte shift = 0, byte checksum = 0)
        {
            OriginalEncrypted = encryptedValue;
            Adjust = adjust;
            Shift = shift;
            Checksum = checksum;
            Value = Decrypt(encryptedValue, shift, adjust);
        }

        /// <summary>
        /// 将加密值写入数据数组
        /// </summary>
        /// <param name="data">目标数据数组</param>
        /// <param name="offset">写入偏移量</param>
        public void Write(byte[] data, int offset) => Write(this, data, offset);

        /// <summary>
        /// 计算给定加密值的校验和
        /// </summary>
        /// <param name="value">加密值</param>
        /// <returns>计算得到的校验和</returns>
        public static byte CalculateChecksum(uint value)
        {
            var byteSum = value + (value >> 16) + (value >> 24) + (value >> 8);
            return (byte)(byteSum - 0x2D);
        }

        /// <summary>
        /// 解密加密的整数
        /// </summary>
        /// <param name="encrypted">加密的值</param>
        /// <param name="shift">移位值</param>
        /// <param name="adjust">调整值</param>
        /// <returns>解密后的值</returns>
        public static uint Decrypt(uint encrypted, byte shift, ushort adjust)
        {
            ulong val = ((ulong) encrypted) << ((32 - SHIFT_BASE - shift) & 0x3F);
            val += val >> 32;
            return ENCRYPTION_CONSTANT - adjust + (uint)val;
        }

        /// <summary>
        /// 加密整数
        /// </summary>
        /// <param name="value">要加密的值</param>
        /// <param name="shift">移位值</param>
        /// <param name="adjust">调整值</param>
        /// <returns>加密后的值</returns>
        public static uint Encrypt(uint value, byte shift, ushort adjust)
        {
            ulong val = (ulong) (value + (adjust - ENCRYPTION_CONSTANT)) << (shift + SHIFT_BASE);
            return (uint) ((val >> 32) + val);
        }

        /// <summary>
        /// 从数据数组读取并验证 EncryptedInt32
        /// </summary>
        /// <param name="data">源数据数组</param>
        /// <param name="offset">读取偏移量</param>
        /// <returns>验证后的 EncryptedInt32 实例</returns>
        /// <exception cref="ArgumentException">当验证失败时抛出</exception>
        public static EncryptedInt32 ReadVerify(byte[] data, int offset)
        {
            var val = Read(data, offset);
            if (val.Checksum != CalculateChecksum(val.OriginalEncrypted))
                throw new ArgumentException($"Failed to verify the {nameof(EncryptedInt32)} at {nameof(offset)}");
            return val;
        }

        /// <summary>
        /// 从数据数组读取 EncryptedInt32
        /// </summary>
        /// <param name="data">源数据数组</param>
        /// <param name="offset">读取偏移量</param>
        /// <returns>EncryptedInt32 实例</returns>
        public static EncryptedInt32 Read(byte[] data, int offset)
        {
            var enc = BitConverter.ToUInt32(data, offset + 0);
            var adjust = BitConverter.ToUInt16(data, offset + 4);
            var shift = data[offset + 6];
            var chk = data[offset + 7];
            return new EncryptedInt32(enc, adjust, shift, chk);
        }

        /// <summary>
        /// 将 EncryptedInt32 写入数据数组
        /// </summary>
        /// <param name="value">要写入的 EncryptedInt32 实例</param>
        /// <param name="data">目标数据数组</param>
        /// <param name="offset">写入偏移量</param>
        public static void Write(EncryptedInt32 value, byte[] data, int offset)
        {
            uint enc = Encrypt(value.Value, value.Shift, value.Adjust);
            byte chk = CalculateChecksum(enc);
            BitConverter.GetBytes(enc).CopyTo(data, offset + 0);
            BitConverter.GetBytes(value.Adjust).CopyTo(data, offset + 4);
            data[offset + 6] = value.Shift;
            data[offset + 7] = chk;
        }
    }
}
