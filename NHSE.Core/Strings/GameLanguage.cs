using System;

namespace NHSE.Core
{
    /// <summary>
    /// 处理语言代码的元数据
    /// </summary>
    public static class GameLanguage
    {
        /// <summary>
        /// 默认语言（英语）
        /// </summary>
        public const string DefaultLanguage = "en"; // English
        /// <summary>
        /// 获取默认语言的索引
        /// </summary>
        public static int DefaultLanguageIndex => Array.IndexOf(LanguageCodes, DefaultLanguage);
        /// <summary>
        /// 将语言索引转换为2字符语言代码
        /// </summary>
        /// <param name="lang">语言索引</param>
        /// <returns>2字符语言代码</returns>
        public static string Language2Char(int lang) => lang > LanguageCodes.Length ? DefaultLanguage : LanguageCodes[lang];

        /// <summary>
        /// 获取支持的语言数量
        /// </summary>
        public static int LanguageCount => LanguageCodes.Length;

        /// <summary>
        /// 获取指定语言代码的索引
        /// </summary>
        /// <param name="lang">2字符语言代码</param>
        /// <returns>语言索引</returns>
        public static int GetLanguageIndex(string lang)
        {
            int l = Array.IndexOf(LanguageCodes, lang);
            return l < 0 ? DefaultLanguageIndex : l;
        }

        /// <summary>
        /// 支持加载字符串资源的语言代码
        /// </summary>
        private static readonly string[] LanguageCodes = { "en", "jp", "de", "es", "fr", "it", "ko", "zhs", "zht" };

        /// <summary>
        /// 获取指定标识、语言和类型的字符串数组
        /// </summary>
        /// <param name="ident">字符串资源标识</param>
        /// <param name="lang">2字符语言代码</param>
        /// <param name="type">资源类型（默认为"text"）</param>
        /// <returns>字符串数组</returns>
        public static string[] GetStrings(string ident, string lang, string type = "text")
        {
            string[] data = ResourceUtil.GetStringList(ident, lang, type);
            if (data.Length == 0)
                data = ResourceUtil.GetStringList(ident, DefaultLanguage, type);

            return data;
        }
    }
}