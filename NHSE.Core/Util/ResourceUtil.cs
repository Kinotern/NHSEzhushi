using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace NHSE.Core
{
    /// <summary>
    /// 资源获取工具类，用于从DLL中检索资源
    /// </summary>
    public static class ResourceUtil
    {
        /// <summary>
        /// 当前程序集
        /// </summary>
        private static readonly Assembly thisAssembly = typeof(ResourceUtil).GetTypeInfo().Assembly;
        /// <summary>
        /// 清单资源名称数组
        /// </summary>
        private static readonly string[] manifestResourceNames = thisAssembly.GetManifestResourceNames();
        /// <summary>
        /// 资源名称映射字典
        /// </summary>
        private static readonly Dictionary<string, string> resourceNameMap = new();
        /// <summary>
        /// 字符串列表缓存字典
        /// </summary>
        private static readonly Dictionary<string, string[]> stringListCache = new();
        /// <summary>
        /// 获取字符串列表的加载锁
        /// </summary>
        private static readonly object getStringListLoadLock = new();

        /// <summary>
        /// 获取字符串列表资源
        /// </summary>
        /// <param name="fileName">资源文件名</param>
        /// <returns>字符串数组</returns>
        public static string[] GetStringList(string fileName)
        {
            if (IsStringListCached(fileName, out var result))
                return result;
            var txt = GetStringResource(fileName); // Fetch File, \n to list.
            return LoadStringList(fileName, txt);
        }

        /// <summary>
        /// 检查字符串列表是否已缓存
        /// </summary>
        /// <param name="fileName">资源文件名</param>
        /// <param name="result">缓存的字符串数组</param>
        /// <returns>是否已缓存</returns>
        public static bool IsStringListCached(string fileName, out string[] result)
        {
            lock (getStringListLoadLock) // Make sure only one thread can read the cache
                return stringListCache.TryGetValue(fileName, out result);
        }

        /// <summary>
        /// 加载字符串列表
        /// </summary>
        /// <param name="file">资源文件名</param>
        /// <param name="txt">文本内容</param>
        /// <returns>字符串数组</returns>
        public static string[] LoadStringList(string file, string? txt)
        {
            if (txt == null)
                return Array.Empty<string>();
            string[] rawlist = txt.TrimEnd('\r', '\n').Split('\n');
            for (int i = 0; i < rawlist.Length; i++)
                rawlist[i] = rawlist[i].TrimEnd('\r');

            lock (getStringListLoadLock) // Make sure only one thread can write to the cache
            {
                if (!stringListCache.ContainsKey(file)) // Check cache again in case of race condition
                    stringListCache.Add(file, rawlist);
            }

            return (string[])rawlist.Clone();
        }

        /// <summary>
        /// 根据文件名、语言代码和类型获取字符串列表
        /// </summary>
        /// <param name="fileName">资源文件名</param>
        /// <param name="lang2char">语言代码（2字符）</param>
        /// <param name="type">资源类型，默认为"text"</param>
        /// <returns>字符串数组</returns>
        public static string[] GetStringList(string fileName, string lang2char, string type = "text") => GetStringList($"{type}_{fileName}_{lang2char}");

        /// <summary>
        /// 获取二进制资源
        /// </summary>
        /// <param name="name">资源名称</param>
        /// <returns>字节数组</returns>
        public static byte[] GetBinaryResource(string name)
        {
            using var resource = thisAssembly.GetManifestResourceStream($"NHSE.Core.Resources.byte.{name}");
            var buffer = new byte[resource.Length];
            resource.Read(buffer, 0, (int)resource.Length);
            return buffer;
        }

        /// <summary>
        /// 获取二进制资源并转换为ushort数组
        /// </summary>
        /// <param name="name">资源名称</param>
        /// <returns>ushort数组</returns>
        public static ushort[] GetBinaryResourceAsUshort(string name)
        {
            var byteBuffer = GetBinaryResource(name);
            var buffer = new ushort[byteBuffer.Length / 2];
            for (int i = 0; i < byteBuffer.Length / 2; i++)
                buffer[i] = BitConverter.ToUInt16(byteBuffer, i*2);
            return buffer;
        }

        /// <summary>
        /// 获取字符串资源
        /// </summary>
        /// <param name="name">资源名称</param>
        /// <returns>字符串内容，若资源不存在则返回null</returns>
        public static string? GetStringResource(string name)
        {
            if (!resourceNameMap.TryGetValue(name, out var resname))
            {
                bool Match(string x) => x.StartsWith("NHSE.Core.Resources.text.") && x.EndsWith($"{name}.txt", StringComparison.OrdinalIgnoreCase);
                resname = Array.Find(manifestResourceNames, Match);
                if (resname == null)
                    return null;
                resourceNameMap.Add(name, resname);
            }

            using var resource = thisAssembly.GetManifestResourceStream(resname);
            if (resource == null)
                return null;
            using var reader = new StreamReader(resource);
            return reader.ReadToEnd();
        }
    }
}