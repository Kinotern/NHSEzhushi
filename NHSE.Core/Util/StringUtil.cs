using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace NHSE.Core
{
    /// <summary>
    /// 字符串操作工具类
    /// </summary>
    public static class StringUtil
    {
        /// <summary>
        /// 从字符串的第一个0x0000终止符处截取字符串
        /// </summary>
        /// <param name="input">要截取的字符串</param>
        /// <returns>截取后的字符串</returns>
        public static string TrimFromZero(string input) => TrimFromFirst(input, '\0');

        /// <summary>
        /// 从字符串的第一个指定字符处截取字符串
        /// </summary>
        /// <param name="input">要截取的字符串</param>
        /// <param name="c">指定的截取字符</param>
        /// <returns>截取后的字符串</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string TrimFromFirst(string input, char c)
        {
            int index = input.IndexOf(c);
            return index < 0 ? input : input.Substring(0, index);
        }

        /// <summary>
        /// 从字节数组中获取Unicode字符串
        /// </summary>
        /// <param name="data">源字节数组</param>
        /// <param name="offset">起始偏移量</param>
        /// <param name="maxLength">最大字符串长度</param>
        /// <returns>获取的字符串</returns>
        public static string GetString(byte[] data, int offset, int maxLength)
        {
            var str = Encoding.Unicode.GetString(data, offset, maxLength * 2);
            return TrimFromZero(str);
        }

        /// <summary>
        /// 将字符串转换为Unicode字节数组
        /// </summary>
        /// <param name="value">源字符串</param>
        /// <param name="maxLength">最大字符串长度</param>
        /// <returns>转换后的字节数组</returns>
        public static byte[] GetBytes(string value, int maxLength)
        {
            if (value.Length > maxLength)
                value = value.Substring(0, maxLength);
            else if (value.Length < maxLength)
                value = value.PadRight(maxLength, '\0');
            return Encoding.Unicode.GetBytes(value);
        }

        /// <summary>
        /// 清理文件名中的无效字符
        /// </summary>
        /// <param name="fileName">原始文件名</param>
        /// <returns>清理后的文件名</returns>
        public static string CleanFileName(string fileName)
        {
            return string.Concat(fileName.Split(Path.GetInvalidFileNameChars()));
        }

        /// <summary>
        /// 将十六进制字符串解析为uint，跳过所有非有效数字的字符
        /// </summary>
        /// <param name="value">要解析的十六进制字符串</param>
        /// <returns>解析后的值</returns>
        public static uint GetHexValue(string value)
        {
            uint result = 0;
            if (string.IsNullOrEmpty(value))
                return result;

            foreach (var c in value)
            {
                if (IsNum(c))
                {
                    result <<= 4;
                    result += (uint)(c - '0');
                }
                else if (IsHexUpper(c))
                {
                    result <<= 4;
                    result += (uint)(c - 'A' + 10);
                }
                else if (IsHexLower(c))
                {
                    result <<= 4;
                    result += (uint)(c - 'a' + 10);
                }
            }
            return result;
        }

        /// <summary>
        /// 检查字符是否为数字
        /// </summary>
        /// <param name="c">要检查的字符</param>
        /// <returns>是否为数字</returns>
        private static bool IsNum(char c) => (uint)(c - '0') <= 9;

        /// <summary>
        /// 检查字符是否为大写十六进制字符
        /// </summary>
        /// <param name="c">要检查的字符</param>
        /// <returns>是否为大写十六进制字符</returns>
        private static bool IsHexUpper(char c) => (uint)(c - 'A') <= 5;

        /// <summary>
        /// 检查字符是否为小写十六进制字符
        /// </summary>
        /// <param name="c">要检查的字符</param>
        /// <returns>是否为小写十六进制字符</returns>
        private static bool IsHexLower(char c) => (uint)(c - 'a') <= 5;
    }
}