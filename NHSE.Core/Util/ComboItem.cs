using System;
using System.Collections.Generic;

namespace NHSE.Core
{
    /// <summary>
    /// 键值对，包含显示文本和底层整数值
    /// </summary>
    /// <param name="Text">显示文本</param>
    /// <param name="Value">底层整数值</param>
    public record ComboItem(string Text, int Value);

    /// <summary>
    /// ComboItem工具类
    /// </summary>
    public static class ComboItemUtil
    {
        /// <summary>
        /// 从字符串数组创建ComboItem列表
        /// </summary>
        /// <param name="items">字符串数组</param>
        /// <returns>ComboItem列表</returns>
        public static List<ComboItem> GetArray(string[] items)
        {
            var result = new List<ComboItem>(items.Length / 2);
            for (int i = 0; i < items.Length; i++)
            {
                var text = items[i];
                if (text.Length == 0)
                    continue;
                var item = new ComboItem(text, i);
                result.Add(item);
            }

            return result;
        }

        /// <summary>
        /// 从枚举类型创建ComboItem列表
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="t">枚举类型</param>
        /// <returns>ComboItem列表</returns>
        public static List<ComboItem> GetArray<T>(Type t) where T : struct, IFormattable
        {
            var names = Enum.GetNames(t);
            var values = (T[])Enum.GetValues(t);

            var acres = new List<ComboItem>(names.Length);
            for (int i = 0; i < names.Length; i++)
                acres.Add(new ComboItem($"{names[i]} - {values[i]:X}", (ushort)(object)values[i]));
            acres.SortByText();
            return acres;
        }

        /// <summary>
        /// 从值列表和名称数组创建ComboItem列表
        /// </summary>
        /// <param name="values">值列表</param>
        /// <param name="names">名称数组</param>
        /// <returns>ComboItem列表</returns>
        public static List<ComboItem> GetArray(IReadOnlyList<ushort> values, string[] names)
        {
            var result = new List<ComboItem>(values.Count);
            foreach (var value in values)
            {
                var text = names[value];
                var item = new ComboItem(text, value);
                result.Add(item);
            }

            return result;
        }

        /// <summary>
        /// 向ComboItem列表添加命名值
        /// </summary>
        /// <param name="storage">ComboItem列表</param>
        /// <param name="tuples">命名值列表</param>
        /// <param name="translate">翻译字典</param>
        public static void Add(this List<ComboItem> storage, IReadOnlyList<INamedValue> tuples, Dictionary<string, string> translate)
        {
            int initial = storage.Count;
            storage.Capacity = storage.Count + tuples.Count;
            foreach (var kvp in tuples)
            {
                var translated = translate.TryGetValue(kvp.Name, out var t) ? t : kvp.Name;
                var item = new ComboItem(translated, kvp.Index);
                storage.Add(item);
            }
            storage.Sort(initial, storage.Count - initial, Comparer);
        }

        /// <summary>
        /// 将ComboItem列表转换为字符串列表
        /// </summary>
        /// <param name="arr">ComboItem列表</param>
        /// <param name="includeValues">是否包含值</param>
        /// <returns>字符串列表</returns>
        public static string ToStringList(this List<ComboItem> arr, bool includeValues)
        {
            string format = string.Empty;
            foreach (var ci in arr)
                format += includeValues ? $"{ci.Text} ({ci.Value:X})\n" : $"{ci.Text}\n";
            return format;
        }

        /// <summary>
        /// 按文本排序ComboItem列表
        /// </summary>
        /// <param name="arr">ComboItem列表</param>
        public static void SortByText(this List<ComboItem> arr) => arr.Sort(Comparer);

        /// <summary>
        /// 文本比较器
        /// </summary>
        private static readonly FunctorComparer<ComboItem> Comparer = new((a, b) => string.CompareOrdinal(a.Text, b.Text));

        /// <summary>
        /// 函数式比较器
        /// </summary>
        /// <typeparam name="T">比较类型</typeparam>
        private sealed class FunctorComparer<T> : IComparer<T>
        {
            /// <summary>
            /// 比较委托
            /// </summary>
            private readonly Comparison<T> Comparison;

            /// <summary>
            /// 初始化FunctorComparer实例
            /// </summary>
            /// <param name="comparison">比较委托</param>
            public FunctorComparer(Comparison<T> comparison) => Comparison = comparison;

            /// <summary>
            /// 比较两个对象
            /// </summary>
            /// <param name="x">第一个对象</param>
            /// <param name="y">第二个对象</param>
            /// <returns>比较结果</returns>
            public int Compare(T x, T y) => Comparison(x, y);
        }
    }
}
