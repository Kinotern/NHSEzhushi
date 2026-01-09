namespace NHSE.Core
{
    /// <summary>
    /// 加密文件结构体，包含加密/解密所需的数据、密钥和计数器
    /// </summary>
    internal readonly ref struct CryptoFile
    {
        /// <summary>
        /// 要加密或解密的数据
        /// </summary>
        public readonly byte[] Data;
        /// <summary>
        /// 加密或解密使用的密钥
        /// </summary>
        public readonly byte[] Key;
        /// <summary>
        /// 加密或解密使用的计数器值
        /// </summary>
        public readonly byte[] Ctr;

        /// <summary>
        /// 初始化 CryptoFile 实例
        /// </summary>
        /// <param name="data">要加密或解密的数据</param>
        /// <param name="key">加密或解密使用的密钥</param>
        /// <param name="ctr">加密或解密使用的计数器值</param>
        public CryptoFile(byte[] data, byte[] key, byte[] ctr)
        {
            Data = data;
            Key = key;
            Ctr = ctr;
        }
    }
}
