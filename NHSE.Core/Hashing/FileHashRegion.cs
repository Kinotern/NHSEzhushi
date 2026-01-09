namespace NHSE.Core
{
    /// <summary>
    /// 指定验证哈希计算的区域
    /// </summary>
    public readonly struct FileHashRegion
    {
        /// <summary>
        /// 计算哈希值的偏移量
        /// </summary>
        public readonly int HashOffset;

        /// <summary>
        /// 哈希数据的长度
        /// </summary>
        public readonly int Size;

        /// <summary>
        /// 要哈希的数据开始的偏移量（计算得出）
        /// </summary>
        public int BeginOffset => HashOffset + 4;

        /// <summary>
        /// 要哈希的数据结束的偏移量（计算得出）
        /// </summary>
        public int EndOffset => BeginOffset + Size;

        /// <summary>
        /// 返回当前实例的字符串表示形式
        /// </summary>
        /// <returns>包含哈希区域信息的字符串</returns>
        public override string ToString() => $"0x{HashOffset:X}: (0x{BeginOffset:X}-0x{EndOffset:X})";

        /// <summary>
        /// 初始化 FileHashRegion 实例
        /// </summary>
        /// <param name="hashOfs">哈希值的偏移量</param>
        /// <param name="size">哈希数据的长度</param>
        public FileHashRegion(int hashOfs, int size)
        {
            HashOffset = hashOfs;
            Size = size;
        }

        #region Equality Comparison
        /// <summary>
        /// 确定指定对象是否等于当前实例
        /// </summary>
        /// <param name="obj">要与当前实例比较的对象</param>
        /// <returns>如果指定对象等于当前实例，则为 true；否则为 false</returns>
        public override bool Equals(object obj) => obj is FileHashRegion r && r == this;
        // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
        /// <summary>
        /// 返回当前实例的哈希代码
        /// </summary>
        /// <returns>当前实例的哈希代码</returns>
        public override int GetHashCode() => BeginOffset.GetHashCode();

        /// <summary>
        /// 确定两个 FileHashRegion 实例是否不相等
        /// </summary>
        /// <param name="left">左侧的 FileHashRegion 实例</param>
        /// <param name="right">右侧的 FileHashRegion 实例</param>
        /// <returns>如果两个实例不相等，则为 true；否则为 false</returns>
        public static bool operator !=(FileHashRegion left, FileHashRegion right) => !(left == right);

        /// <summary>
        /// 确定两个 FileHashRegion 实例是否相等
        /// </summary>
        /// <param name="left">左侧的 FileHashRegion 实例</param>
        /// <param name="right">右侧的 FileHashRegion 实例</param>
        /// <returns>如果两个实例相等，则为 true；否则为 false</returns>
        public static bool operator ==(FileHashRegion left, FileHashRegion right)
        {
            if (left.HashOffset != right.HashOffset)
                return false;
            if (left.BeginOffset != right.BeginOffset)
                return false;
            if (left.Size != right.Size)
                return false;
            return true;
        }
        #endregion
    }
}