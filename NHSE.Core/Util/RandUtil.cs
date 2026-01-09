using System;
using System.Collections.Generic;
using System.Threading;

namespace NHSE.Core
{
    /// <summary>
    /// 随机值生成工具类
    /// </summary>
    public static class RandUtil
    {
        /// <summary>
        /// 线程安全的随机数生成器
        /// </summary>
        public static Random Rand => _local.Value;

        /// <summary>
        /// 线程本地存储的随机数生成器实例
        /// </summary>
        private static readonly ThreadLocal<Random> _local = new(() => new Random());

        /// <summary>
        /// 生成32位随机数
        /// </summary>
        /// <returns>32位随机数</returns>
        public static uint Rand32() => Rand32(Rand);

        /// <summary>
        /// 使用指定的随机数生成器生成32位随机数
        /// </summary>
        /// <param name="rnd">随机数生成器</param>
        /// <returns>32位随机数</returns>
        public static uint Rand32(Random rnd) => (uint)rnd.Next(1 << 30) << 2 | (uint)rnd.Next(1 << 2);

        /// <summary>
        /// 打乱集合中元素的顺序
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="items">元素集合</param>
        public static void Shuffle<T>(IList<T> items) => Shuffle(items, 0, items.Count, Rand);

        /// <summary>
        /// 打乱集合中指定范围内元素的顺序
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="items">元素集合</param>
        /// <param name="start">起始位置</param>
        /// <param name="end">结束位置</param>
        /// <param name="rnd">随机数生成器</param>
        public static void Shuffle<T>(IList<T> items, int start, int end, Random rnd)
        {
            for (int i = start; i < end; i++)
            {
                int index = i + rnd.Next(end - i);
                T t = items[index];
                items[index] = items[i];
                items[i] = t;
            }
        }
    }
}