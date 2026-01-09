using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace NHSE.Core
{
    /// <summary>
    /// 结构体转换工具类，用于在原始数据和类/结构体之间进行转换
    /// </summary>
    public static class StructConverter
    {
        /// <summary>
        /// 将字节数组转换为结构体
        /// </summary>
        /// <typeparam name="T">结构体类型</typeparam>
        /// <param name="bytes">源字节数组</param>
        /// <returns>转换后的结构体</returns>
        public static T ToStructure<T>(this byte[] bytes) where T : struct
        {
            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try { return (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T)); }
            finally { handle.Free(); }
        }

        /// <summary>
        /// 将字节数组转换为类实例
        /// </summary>
        /// <typeparam name="T">类类型</typeparam>
        /// <param name="bytes">源字节数组</param>
        /// <returns>转换后的类实例</returns>
        public static T ToClass<T>(this byte[] bytes) where T : class
        {
            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try { return (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T)); }
            finally { handle.Free(); }
        }

        /// <summary>
        /// 从字节数组的指定偏移量和长度处转换为结构体
        /// </summary>
        /// <typeparam name="T">结构体类型</typeparam>
        /// <param name="bytes">源字节数组</param>
        /// <param name="offset">起始偏移量</param>
        /// <param name="length">转换长度</param>
        /// <returns>转换后的结构体</returns>
        public static T ToStructure<T>(this byte[] bytes, int offset, int length) where T : struct
        {
            var slice = bytes.Slice(offset, length);
            return slice.ToStructure<T>();
        }

        /// <summary>
        /// 从字节数组的指定偏移量和长度处转换为类实例
        /// </summary>
        /// <typeparam name="T">类类型</typeparam>
        /// <param name="bytes">源字节数组</param>
        /// <param name="offset">起始偏移量</param>
        /// <param name="length">转换长度</param>
        /// <returns>转换后的类实例</returns>
        public static T ToClass<T>(this byte[] bytes, int offset, int length) where T : class
        {
            var slice = bytes.Slice(offset, length);
            return slice.ToClass<T>();
        }

        /// <summary>
        /// 将类实例转换为字节数组
        /// </summary>
        /// <typeparam name="T">类类型</typeparam>
        /// <param name="obj">源类实例</param>
        /// <returns>转换后的字节数组</returns>
        public static byte[] ToBytesClass<T>(this T obj) where T : class
        {
            int size = Marshal.SizeOf(obj);
            byte[] arr = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(obj, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        /// <summary>
        /// 将结构体转换为字节数组
        /// </summary>
        /// <typeparam name="T">结构体类型</typeparam>
        /// <param name="obj">源结构体</param>
        /// <returns>转换后的字节数组</returns>
        public static byte[] ToBytes<T>(this T obj) where T : struct
        {
            int size = Marshal.SizeOf(obj);
            byte[] arr = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(obj, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        /// <summary>
        /// 从字节数组中获取类实例数组
        /// </summary>
        /// <typeparam name="T">类类型</typeparam>
        /// <param name="data">源字节数组</param>
        /// <param name="size">每个实例的大小</param>
        /// <returns>类实例数组</returns>
        public static T[] GetArray<T>(this byte[] data, int size) where T : class
        {
            var result = new T[data.Length / size];
            for (int i = 0; i < result.Length; i++)
                result[i] = data.Slice(i * size, size).ToClass<T>();
            return result;
        }

        /// <summary>
        /// 将类实例列表转换为字节数组
        /// </summary>
        /// <typeparam name="T">类类型</typeparam>
        /// <param name="data">源类实例列表</param>
        /// <param name="size">每个实例的大小</param>
        /// <returns>转换后的字节数组</returns>
        public static byte[] SetArray<T>(this IReadOnlyList<T> data, int size) where T : class
        {
            var result = new byte[data.Count * size];
            for (int i = 0; i < data.Count; i++)
                data[i].ToBytesClass().CopyTo(result, i * size);
            return result;
        }

        /// <summary>
        /// 从字节数组中获取结构体数组
        /// </summary>
        /// <typeparam name="T">结构体类型</typeparam>
        /// <param name="data">源字节数组</param>
        /// <param name="size">每个结构体的大小</param>
        /// <returns>结构体数组</returns>
        public static T[] GetArrayStructure<T>(this byte[] data, int size) where T : struct
        {
            var result = new T[data.Length / size];
            for (int i = 0; i < result.Length; i++)
                result[i] = data.Slice(i * size, size).ToStructure<T>();
            return result;
        }

        /// <summary>
        /// 将结构体列表转换为字节数组
        /// </summary>
        /// <typeparam name="T">结构体类型</typeparam>
        /// <param name="data">源结构体列表</param>
        /// <param name="size">每个结构体的大小</param>
        /// <returns>转换后的字节数组</returns>
        public static byte[] SetArrayStructure<T>(this IReadOnlyList<T> data, int size) where T : struct
        {
            var result = new byte[data.Count * size];
            for (int i = 0; i < data.Count; i++)
                data[i].ToBytes().CopyTo(result, i * size);
            return result;
        }
    }
}
