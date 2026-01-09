namespace NHSE.Core
{
    /// <summary>
    /// 游戏字符串的全局存储库；初始化为指定的语言
    /// </summary>
    public static class GameInfo
    {
        /// <summary>
        /// 存储不同语言的GameStrings实例数组
        /// </summary>
        private static readonly GameStrings?[] Languages = new GameStrings[GameLanguage.LanguageCount];

        /// <summary>
        /// 获取当前使用的语言
        /// </summary>
        public static string CurrentLanguage { get; private set; } = GameLanguage.DefaultLanguage;
        /// <summary>
        /// 获取当前语言的游戏字符串
        /// </summary>
        public static GameStrings Strings { get; private set; } = GetStrings(CurrentLanguage);

        /// <summary>
        /// 获取指定语言的游戏字符串
        /// </summary>
        /// <param name="lang">2字符语言ID</param>
        /// <returns>指定语言的GameStrings实例</returns>
        public static GameStrings GetStrings(string lang)
        {
            int index = GameLanguage.GetLanguageIndex(lang);
            return GetStrings(index);
        }

        /// <summary>
        /// 通过语言索引获取游戏字符串
        /// </summary>
        /// <param name="index">语言索引</param>
        /// <returns>对应语言的GameStrings实例</returns>
        public static GameStrings GetStrings(int index)
        {
            return Languages[index] ??= new GameStrings(GameLanguage.Language2Char(index));
        }

        /// <summary>
        /// 设置当前语言
        /// </summary>
        /// <param name="index">语言索引</param>
        /// <returns>设置的2字符语言ID</returns>
        public static string SetLanguage2Char(int index)
        {
            var lang = GameLanguage.Language2Char(index);
            CurrentLanguage = lang;
            Strings = GetStrings(lang);
            return lang;
        }
    }
}
