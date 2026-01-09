namespace NHSE.Core
{
    /// <summary>
    /// 加密保存文件结构体，包含加密数据和文件头
    /// </summary>
    public readonly ref struct EncryptedSaveFile
    {
        /// <summary>
        /// 加密的保存数据
        /// </summary>
        public readonly byte[] Data;
        /// <summary>
        /// 文件头数据
        /// </summary>
        public readonly byte[] Header;

        /// <summary>
        /// 初始化 EncryptedSaveFile 实例
        /// </summary>
        /// <param name="data">加密的保存数据</param>
        /// <param name="header">文件头数据</param>
        public EncryptedSaveFile(byte[] data, byte[] header)
        {
            Data = data;
            Header = header;
        }

        #region Equality Comparison
        /// <summary>
        /// 确定当前对象是否等于另一个对象
        /// </summary>
        /// <param name="obj">要比较的对象</param>
        /// <returns>总是返回 false，因为引用类型比较不适用</returns>
        public override bool Equals(object obj) => false;
        /// <summary>
        /// 获取当前对象的哈希码
        /// </summary>
        /// <returns>基于 Data 字段的哈希码</returns>
        public override int GetHashCode() => Data.GetHashCode();
        /// <summary>
        /// 比较两个 EncryptedSaveFile 实例是否不相等
        /// </summary>
        /// <param name="left">左侧实例</param>
        /// <param name="right">右侧实例</param>
        /// <returns>如果不相等则返回 true，否则返回 false</returns>
        public static bool operator !=(EncryptedSaveFile left, EncryptedSaveFile right) => !(left == right);
        /// <summary>
        /// 比较两个 EncryptedSaveFile 实例是否相等
        /// </summary>
        /// <param name="left">左侧实例</param>
        /// <param name="right">右侧实例</param>
        /// <returns>如果相等则返回 true，否则返回 false</returns>
        public static bool operator ==(EncryptedSaveFile left, EncryptedSaveFile right) => left.Data == right.Data && left.Header == right.Header;
        #endregion
    }
}
