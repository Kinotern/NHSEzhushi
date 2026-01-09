using System.Collections.Generic;
using System.Linq;

namespace NHSE.Core
{
    /// <summary>
    /// 文件哈希信息类，管理不同文件大小的哈希详情
    /// </summary>
    public sealed class FileHashInfo
    {
        /// <summary>
        /// 存储文件大小到FileHashDetails的映射字典
        /// </summary>
        private readonly IReadOnlyDictionary<uint, FileHashDetails> List;

        /// <summary>
        /// 复制构造函数，从现有实例创建新实例
        /// </summary>
        /// <param name="dupe">要复制的现有实例</param>
        public FileHashInfo(FileHashInfo dupe) : this(dupe.List.Values) { }

        /// <summary>
        /// 从哈希集合创建 FileHashInfo 实例
        /// </summary>
        /// <param name="hashSets">哈希详情集合</param>
        public FileHashInfo(IEnumerable<FileHashDetails> hashSets)
        {
            var list = new Dictionary<uint, FileHashDetails>();
            foreach (var hashSet in hashSets)
                list[hashSet.FileSize] = hashSet;
            List = list;
        }

        /// <summary>
        /// 根据文件名获取文件哈希详情
        /// </summary>
        /// <param name="nameData">文件名</param>
        /// <returns>匹配的 FileHashDetails 实例，如果未找到则返回 null</returns>
        public FileHashDetails? GetFile(string nameData)
        {
            return List.Values.FirstOrDefault(z => z.FileName == nameData);
        }
    }
}
