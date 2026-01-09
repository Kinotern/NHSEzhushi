using System.Collections.Generic;

namespace NHSE.Core
{
    /// <summary>
    /// 包含文件哈希区域信息的类
    /// </summary>
    public sealed class FileHashDetails
    {
        /// <summary>
        /// 应用这些哈希区域的文件名
        /// </summary>
        public readonly string FileName;

        /// <summary>
        /// 文件名对应的预期文件大小
        /// </summary>
        public readonly uint FileSize;

        /// <summary>
        /// 此文件中包含的哈希区域规格
        /// </summary>
        public readonly IReadOnlyList<FileHashRegion> HashRegions;

        /// <summary>
        /// 初始化 FileHashDetails 实例
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="regions">哈希区域列表</param>
        public FileHashDetails(string fileName, uint fileSize, IReadOnlyList<FileHashRegion> regions)
        {
            FileName = fileName;
            FileSize = fileSize;
            HashRegions = regions;
        }
    }
}