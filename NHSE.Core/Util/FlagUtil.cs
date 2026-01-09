namespace NHSE.Core
{
    /// <summary>
    /// 位标志处理工具类，用于操作字节数组中的位标志
    /// </summary>
    public static class FlagUtil
    {
        /// <summary>
        /// 获取字节数组中指定位置的位标志值
        /// </summary>
        /// <param name="arr">源字节数组</param>
        /// <param name="offset">起始偏移量</param>
        /// <param name="bitIndex">位索引</param>
        /// <returns>位标志值</returns>
        public static bool GetFlag(byte[] arr, int offset, int bitIndex)
        {
            var b = arr[offset + (bitIndex >> 3)];
            var mask = 1 << (bitIndex & 7);
            return (b & mask) != 0;
        }

        /// <summary>
        /// 设置字节数组中指定位置的位标志值
        /// </summary>
        /// <param name="arr">目标字节数组</param>
        /// <param name="offset">起始偏移量</param>
        /// <param name="bitIndex">位索引</param>
        /// <param name="value">要设置的位标志值</param>
        public static void SetFlag(byte[] arr, int offset, int bitIndex, bool value)
        {
            offset += (bitIndex >> 3);
            bitIndex &= 7; // ensure bit access is 0-7
            arr[offset] &= (byte)~(1 << bitIndex);
            arr[offset] |= (byte)((value ? 1 : 0) << bitIndex);
        }
    }
}