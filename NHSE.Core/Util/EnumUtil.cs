using System;
using System.Collections.Generic;

namespace NHSE.Core
{
    /// <summary>
    /// 枚举工具类，用于处理枚举类型的操作
    /// </summary>
    public static class EnumUtil
    {
        /// <summary>
        /// 获取指定枚举类型的名称和值列表
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <returns>包含枚举名称和值数组的键值对</returns>
        public static KeyValuePair<string, string[]> GetEnumList<T>() where T : Enum
        {
            var type = typeof(T);
            var name = type.Name;
            var values = GetTypeValues<T>(type);
            return new KeyValuePair<string, string[]>(name, values);
        }

        /// <summary>
        /// 获取指定枚举类型的值数组
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="type">枚举类型的Type对象</param>
        /// <returns>枚举值的字符串数组</returns>
        private static string[] GetTypeValues<T>(Type type) where T : Enum
        {
            var arr = (T[])Enum.GetValues(type);
            var result = new string[arr.Length];
            for (int i = 0; i < arr.Length; i++)
                result[i] = GetSummary(arr[i]);
            return result;
        }

        /// <summary>
        /// 获取枚举值的字符串表示（包含枚举名称和整数值）
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="z">枚举值</param>
        /// <returns>格式化的枚举值字符串</returns>
        private static string GetSummary<T>(T z) where T : Enum
        {
            int x = Convert.ToInt32(z);
            return $"{z} = {x}";
        }
    }
}
