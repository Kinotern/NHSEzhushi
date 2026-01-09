using System;
using System.Collections.Generic;

namespace NHSE.Core
{
    /// <summary>
    /// 数组操作工具类
    /// </summary>
    public static class ArrayUtil
    {
        /// <summary>
        /// 从字节数组中截取指定长度的子数组
        /// </summary>
        /// <param name="src">源字节数组</param>
        /// <param name="offset">起始偏移量</param>
        /// <param name="length">截取长度</param>
        /// <returns>截取的子数组</returns>
        public static byte[] Slice(this byte[] src, int offset, int length)
        {
            byte[] data = new byte[length];
            Buffer.BlockCopy(src, offset, data, 0, data.Length);
            return data;
        }

        /// <summary>
        /// 从字节数组的指定偏移量开始截取到末尾
        /// </summary>
        /// <param name="src">源字节数组</param>
        /// <param name="offset">起始偏移量</param>
        /// <returns>截取的子数组</returns>
        public static byte[] SliceEnd(this byte[] src, int offset)
        {
            int length = src.Length - offset;
            byte[] data = new byte[length];
            Buffer.BlockCopy(src, offset, data, 0, data.Length);
            return data;
        }

        /// <summary>
        /// 从泛型数组中截取指定长度的子数组
        /// </summary>
        /// <typeparam name="T">数组元素类型</typeparam>
        /// <param name="src">源泛型数组</param>
        /// <param name="offset">起始偏移量</param>
        /// <param name="length">截取长度</param>
        /// <returns>截取的子数组</returns>
        public static T[] Slice<T>(this T[] src, int offset, int length)
        {
            var data = new T[length];
            Array.Copy(src, offset, data, 0, data.Length);
            return data;
        }

        /// <summary>
        /// 从泛型数组的指定偏移量开始截取到末尾
        /// </summary>
        /// <typeparam name="T">数组元素类型</typeparam>
        /// <param name="src">源泛型数组</param>
        /// <param name="offset">起始偏移量</param>
        /// <returns>截取的子数组</returns>
        public static T[] SliceEnd<T>(this T[] src, int offset)
        {
            int length = src.Length - offset;
            var data = new T[length];
            Array.Copy(src, offset, data, 0, data.Length);
            return data;
        }

        /// <summary>
        /// 检查值是否在指定范围内
        /// </summary>
        /// <param name="value">要检查的值</param>
        /// <param name="min">最小值（包含）</param>
        /// <param name="max">最大值（不包含）</param>
        /// <returns>值是否在范围内</returns>
        public static bool WithinRange(int value, int min, int max) => min <= value && value < max;

        /// <summary>
        /// 将泛型数组按指定大小分割成二维数组
        /// </summary>
        /// <typeparam name="T">数组元素类型</typeparam>
        /// <param name="data">源泛型数组</param>
        /// <param name="size">分割大小</param>
        /// <returns>分割后的二维数组</returns>
        public static T[][] Split<T>(this T[] data, int size)
        {
            var result = new T[data.Length / size][];
            for (int i = 0; i < data.Length; i += size)
                result[i / size] = data.Slice(i, size);
            return result;
        }

        /// <summary>
        /// 枚举分割泛型数组
        /// </summary>
        /// <typeparam name="T">数组元素类型</typeparam>
        /// <param name="bin">源泛型数组</param>
        /// <param name="size">分割大小</param>
        /// <param name="start">起始偏移量</param>
        /// <returns>分割后的子数组枚举</returns>
        public static IEnumerable<T[]> EnumerateSplit<T>(T[] bin, int size, int start = 0)
        {
            for (int i = start; i < bin.Length; i += size)
                yield return bin.Slice(i, size);
        }

        /// <summary>
        /// 枚举分割泛型数组（指定范围）
        /// </summary>
        /// <typeparam name="T">数组元素类型</typeparam>
        /// <param name="bin">源泛型数组</param>
        /// <param name="size">分割大小</param>
        /// <param name="start">起始偏移量</param>
        /// <param name="end">结束偏移量（-1表示数组末尾）</param>
        /// <returns>分割后的子数组枚举</returns>
        public static IEnumerable<T[]> EnumerateSplit<T>(T[] bin, int size, int start, int end)
        {
            if (end < 0)
                end = bin.Length;
            for (int i = start; i < end; i += size)
                yield return bin.Slice(i, size);
        }

        /// <summary>
        /// 从字节数组中获取位标志数组
        /// </summary>
        /// <param name="data">源字节数组</param>
        /// <param name="offset">起始偏移量</param>
        /// <param name="count">位标志数量</param>
        /// <returns>位标志数组</returns>
        public static bool[] GitBitFlagArray(byte[] data, int offset, int count)
        {
            bool[] result = new bool[count];
            for (int i = 0; i < result.Length; i++)
                result[i] = (data[offset + (i >> 3)] >> (i & 7) & 0x1) == 1;
            return result;
        }

        /// <summary>
        /// 将位标志数组设置到字节数组中
        /// </summary>
        /// <param name="data">目标字节数组</param>
        /// <param name="offset">起始偏移量</param>
        /// <param name="value">位标志数组</param>
        public static void SetBitFlagArray(byte[] data, int offset, bool[] value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                var ofs = offset + (i >> 3);
                var mask = (1 << (i & 7));
                if (value[i])
                    data[ofs] |= (byte)mask;
                else
                    data[ofs] &= (byte)~mask;
            }
        }

        /// <summary>
        /// 将位标志数组转换为字节数组
        /// </summary>
        /// <param name="value">位标志数组</param>
        /// <returns>转换后的字节数组</returns>
        public static byte[] SetBitFlagArray(bool[] value)
        {
            byte[] data = new byte[value.Length / 8];
            SetBitFlagArray(data, 0, value);
            return data;
        }

        /// <summary>
        /// 将泛型列表复制到目标列表，可选择从指定位置开始复制
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="list">源列表</param>
        /// <param name="dest">目标列表/数组</param>
        /// <param name="skip">跳过槽位的条件</param>
        /// <param name="start">开始复制的位置</param>
        /// <returns>复制的元素数量</returns>
        public static int CopyTo<T>(this IEnumerable<T> list, IList<T> dest, Func<T, bool> skip, int start = 0)
        {
            int ctr = start;
            int skipped = 0;
            foreach (var z in list)
            {
                // seek forward to next open slot
                int next = FindNextValidIndex(dest, skip, ctr);
                if (next == -1)
                    break;
                skipped += next - ctr;
                ctr = next;
                dest[ctr++] = z;
            }
            return ctr - start - skipped;
        }

        /// <summary>
        /// 查找下一个有效的索引
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="dest">目标列表</param>
        /// <param name="skip">跳过槽位的条件</param>
        /// <param name="ctr">当前索引</param>
        /// <returns>下一个有效索引，或-1（如果没有找到）</returns>
        public static int FindNextValidIndex<T>(IList<T> dest, Func<T, bool> skip, int ctr)
        {
            while (true)
            {
                if ((uint)ctr >= dest.Count)
                    return -1;
                var exist = dest[ctr];
                if (exist == null || !skip(exist))
                    return ctr;
                ctr++;
            }
        }

        /// <summary>
        /// 将泛型列表复制到目标列表，可选择从指定位置开始复制
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="list">源列表</param>
        /// <param name="dest">目标列表/数组</param>
        /// <param name="start">开始复制的位置</param>
        /// <returns>复制的元素数量</returns>
        public static int CopyTo<T>(this IEnumerable<T> list, IList<T> dest, int start = 0)
        {
            int ctr = start;
            foreach (var z in list)
            {
                if ((uint)ctr >= dest.Count)
                    break;
                dest[ctr++] = z;
            }
            return ctr - start;
        }

        /// <summary>
        /// 连接多个泛型数组
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="arr">要连接的数组</param>
        /// <returns>连接后的新数组</returns>
        public static T[] ConcatAll<T>(params T[][] arr)
        {
            int len = 0;
            foreach (var a in arr)
                len += a.Length;

            var result = new T[len];

            int ctr = 0;
            foreach (var a in arr)
            {
                a.CopyTo(result, ctr);
                ctr += a.Length;
            }

            return result;
        }

        /// <summary>
        /// 在字节数组中查找指定模式
        /// </summary>
        /// <param name="array">要查找的数组</param>
        /// <param name="pattern">要查找的模式</param>
        /// <param name="startIndex">开始查找的偏移量</param>
        /// <param name="length">要查找的条目数量</param>
        /// <returns>模式出现的索引；如果未找到，返回-1</returns>
        public static int IndexOfBytes(byte[] array, byte[] pattern, int startIndex = 0, int length = -1)
        {
            int len = pattern.Length;
            int endIndex = length > 0
                ? startIndex + length
                : array.Length - len - startIndex;

            endIndex = Math.Min(array.Length - pattern.Length, endIndex);

            int i = startIndex;
            int j = 0;
            while (true)
            {
                if (pattern[j] != array[i + j])
                {
                    if (++i == endIndex)
                        return -1;
                    j = 0;
                }
                else if (++j == len)
                {
                    return i;
                }
            }
        }

        /// <summary>
        /// 替换字节数组中的所有匹配模式
        /// </summary>
        /// <param name="array">源字节数组</param>
        /// <param name="pattern">要查找的模式</param>
        /// <param name="swap">要替换的内容</param>
        /// <returns>替换的次数</returns>
        public static int ReplaceOccurrences(this byte[] array, byte[] pattern, byte[] swap)
        {
            int count = 0;
            while (true)
            {
                int ofs = IndexOfBytes(array, pattern);
                if (ofs == -1)
                    return count;
                swap.CopyTo(array, ofs);
                ++count;
            }
        }
    }
}
