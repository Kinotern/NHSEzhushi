using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace NHSE.Core
{
    /// <summary>
    /// 反射工具类，提供类型反射相关的操作方法
    /// </summary>
    public static class ReflectUtil
    {
        /// <summary>
        /// 检查属性值是否与指定值相等
        /// </summary>
        /// <param name="pi">属性信息</param>
        /// <param name="obj">对象实例</param>
        /// <param name="value">要比较的值</param>
        /// <returns>值是否相等</returns>
        public static bool IsValueEqual(this PropertyInfo pi, object obj, object value)
        {
            var v = pi.GetValue(obj, null);
            var c = ConvertValue(value, pi.PropertyType);
            return v.Equals(c);
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="pi">属性信息</param>
        /// <param name="obj">对象实例</param>
        /// <param name="value">要设置的值</param>
        public static void SetValue(PropertyInfo pi, object obj, object value)
        {
            var c = ConvertValue(value, pi.PropertyType);
            pi.SetValue(obj, c, null);
        }

        /// <summary>
        /// 获取对象的指定属性值
        /// </summary>
        /// <param name="obj">对象实例</param>
        /// <param name="name">属性名称</param>
        /// <returns>属性值，若属性不存在则返回null</returns>
        public static object? GetValue(object obj, string name) => GetPropertyInfo(obj.GetType().GetTypeInfo(), name)?.GetValue(obj);

        /// <summary>
        /// 设置对象的指定属性值
        /// </summary>
        /// <param name="obj">对象实例</param>
        /// <param name="name">属性名称</param>
        /// <param name="value">要设置的值</param>
        public static void SetValue(object obj, string name, object value) => GetPropertyInfo(obj.GetType().GetTypeInfo(), name)?.SetValue(obj, value, null);

        /// <summary>
        /// 获取静态类的指定属性值
        /// </summary>
        /// <param name="t">类型</param>
        /// <param name="propertyName">属性名称</param>
        /// <returns>属性值</returns>
        public static object GetValue(Type t, string propertyName) => t.GetTypeInfo().GetDeclaredProperty(propertyName).GetValue(null);

        /// <summary>
        /// 设置静态类的指定属性值
        /// </summary>
        /// <param name="t">类型</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="value">要设置的值</param>
        public static void SetValue(Type t, string propertyName, object value) => t.GetTypeInfo().GetDeclaredProperty(propertyName).SetValue(null, value);

        /// <summary>
        /// 获取以指定前缀开头的属性名称列表
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="prefix">前缀</param>
        /// <returns>属性名称列表</returns>
        public static IEnumerable<string> GetPropertiesStartWithPrefix(Type type, string prefix)
        {
            return type.GetTypeInfo().GetAllTypeInfo().SelectMany(GetAllProperties)
                    .Where(p => p.Name.StartsWith(prefix, StringComparison.Ordinal))
                    .Select(p => p.Name)
                    .Distinct()
                ;
        }

        /// <summary>
        /// 获取可公开写入的属性名称列表
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>属性名称列表</returns>
        public static IEnumerable<string> GetPropertiesCanWritePublic(Type type)
        {
            return GetAllPropertyInfoCanWritePublic(type).Select(p => p.Name)
                    .Distinct()
                ;
        }

        /// <summary>
        /// 获取可公开写入的属性信息列表
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>属性信息列表</returns>
        public static IEnumerable<PropertyInfo> GetAllPropertyInfoCanWritePublic(Type type)
        {
            return type.GetTypeInfo().GetAllTypeInfo().SelectMany(GetAllProperties)
                .Where(p => p.CanWrite && p.SetMethod.IsPublic);
        }

        /// <summary>
        /// 获取可公开访问的属性信息列表
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>属性信息列表</returns>
        public static IEnumerable<PropertyInfo> GetAllPropertyInfoPublic(Type type)
        {
            return type.GetTypeInfo().GetAllTypeInfo().SelectMany(GetAllProperties)
                .Where(p => (p.CanRead && p.GetMethod.IsPublic) || (p.CanWrite && p.SetMethod.IsPublic));
        }

        /// <summary>
        /// 获取可公开访问的属性名称列表
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>属性名称列表</returns>
        public static IEnumerable<string> GetPropertiesPublic(Type type)
        {
            return GetAllPropertyInfoPublic(type).Select(p => p.Name)
                    .Distinct()
                ;
        }

        /// <summary>
        /// 获取声明的可公开写入的属性名称列表
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>属性名称列表</returns>
        public static IEnumerable<string> GetPropertiesCanWritePublicDeclared(Type type)
        {
            return type.GetTypeInfo().GetAllProperties()
                    .Where(p => p.CanWrite && p.SetMethod.IsPublic)
                    .Select(p => p.Name)
                    .Distinct()
                ;
        }

        /// <summary>
        /// 转换值为指定类型
        /// </summary>
        /// <param name="value">要转换的值</param>
        /// <param name="type">目标类型</param>
        /// <returns>转换后的值</returns>
        private static object? ConvertValue(object value, Type type)
        {
            if (type == typeof(DateTime?)) // Used for PKM.MetDate and other similar properties
            {
                return DateTime.TryParseExact(value.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateValue)
                    ? new DateTime?(dateValue)
                    : null;
            }

            // Convert.ChangeType is suitable for most things
            return Convert.ChangeType(value, type);
        }

        /// <summary>
        /// 获取所有构造函数信息
        /// </summary>
        /// <param name="typeInfo">类型信息</param>
        /// <returns>构造函数信息列表</returns>
        public static IEnumerable<ConstructorInfo> GetAllConstructors(this TypeInfo typeInfo)
            => GetAll(typeInfo, ti => ti.DeclaredConstructors);

        /// <summary>
        /// 获取所有事件信息
        /// </summary>
        /// <param name="typeInfo">类型信息</param>
        /// <returns>事件信息列表</returns>
        public static IEnumerable<EventInfo> GetAllEvents(this TypeInfo typeInfo)
            => GetAll(typeInfo, ti => ti.DeclaredEvents);

        /// <summary>
        /// 获取所有字段信息
        /// </summary>
        /// <param name="typeInfo">类型信息</param>
        /// <returns>字段信息列表</returns>
        public static IEnumerable<FieldInfo> GetAllFields(this TypeInfo typeInfo)
            => GetAll(typeInfo, ti => ti.DeclaredFields);

        /// <summary>
        /// 获取所有成员信息
        /// </summary>
        /// <param name="typeInfo">类型信息</param>
        /// <returns>成员信息列表</returns>
        public static IEnumerable<MemberInfo> GetAllMembers(this TypeInfo typeInfo)
            => GetAll(typeInfo, ti => ti.DeclaredMembers);

        /// <summary>
        /// 获取所有方法信息
        /// </summary>
        /// <param name="typeInfo">类型信息</param>
        /// <returns>方法信息列表</returns>
        public static IEnumerable<MethodInfo> GetAllMethods(this TypeInfo typeInfo)
            => GetAll(typeInfo, ti => ti.DeclaredMethods);

        /// <summary>
        /// 获取所有嵌套类型信息
        /// </summary>
        /// <param name="typeInfo">类型信息</param>
        /// <returns>嵌套类型信息列表</returns>
        public static IEnumerable<TypeInfo> GetAllNestedTypes(this TypeInfo typeInfo)
            => GetAll(typeInfo, ti => ti.DeclaredNestedTypes);

        /// <summary>
        /// 获取所有属性信息
        /// </summary>
        /// <param name="typeInfo">类型信息</param>
        /// <returns>属性信息列表</returns>
        public static IEnumerable<PropertyInfo> GetAllProperties(this TypeInfo typeInfo)
            => GetAll(typeInfo, ti => ti.DeclaredProperties);

        /// <summary>
        /// 获取所有类型信息（包括基类）
        /// </summary>
        /// <param name="typeInfo">类型信息</param>
        /// <returns>类型信息列表</returns>
        public static IEnumerable<TypeInfo> GetAllTypeInfo(this TypeInfo? typeInfo)
        {
            while (typeInfo != null)
            {
                yield return typeInfo;
                typeInfo = typeInfo.BaseType?.GetTypeInfo();
            }
        }

        /// <summary>
        /// 检查对象是否具有指定名称的属性
        /// </summary>
        /// <param name="obj">对象实例</param>
        /// <param name="name">属性名称</param>
        /// <param name="pi">属性信息</param>
        /// <returns>是否具有该属性</returns>
        public static bool HasProperty(object obj, string name, [NotNullWhen(true)] out PropertyInfo? pi) => (pi = GetPropertyInfo(obj.GetType().GetTypeInfo(), name)) != null;

        /// <summary>
        /// 获取类型的指定属性信息
        /// </summary>
        /// <param name="typeInfo">类型信息</param>
        /// <param name="name">属性名称</param>
        /// <returns>属性信息，若属性不存在则返回null</returns>
        public static PropertyInfo? GetPropertyInfo(this TypeInfo typeInfo, string name)
        {
            return typeInfo.GetAllTypeInfo().Select(t => t.GetDeclaredProperty(name)).FirstOrDefault(pi => pi != null);
        }

        /// <summary>
        /// 获取类型的所有指定成员信息
        /// </summary>
        /// <typeparam name="T">成员类型</typeparam>
        /// <param name="typeInfo">类型信息</param>
        /// <param name="accessor">成员访问器</param>
        /// <returns>成员信息列表</returns>
        private static IEnumerable<T> GetAll<T>(this TypeInfo typeInfo, Func<TypeInfo, IEnumerable<T>> accessor)
        {
            return GetAllTypeInfo(typeInfo).SelectMany(_ => accessor(typeInfo));
        }

        /// <summary>
        /// 获取类型中指定类型的所有常量
        /// </summary>
        /// <typeparam name="T">常量类型</typeparam>
        /// <param name="type">类型</param>
        /// <returns>常量值与名称的字典</returns>
        public static Dictionary<T, string> GetAllConstantsOfType<T>(this Type type) where T : struct
        {
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            var consts = fields.Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(T));
            return consts.ToDictionary(x => (T)x.GetRawConstantValue(), z => z.Name);
        }

        /// <summary>
        /// 获取对象中指定类型的所有属性
        /// </summary>
        /// <typeparam name="T">属性类型</typeparam>
        /// <param name="type">类型</param>
        /// <param name="obj">对象实例</param>
        /// <returns>属性值与名称的字典</returns>
        public static Dictionary<T, string> GetAllPropertiesOfType<T>(this Type type, object obj) where T : class
        {
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            var ofType = props.Where(fi => typeof(T).IsAssignableFrom(fi.PropertyType));
            return ofType.ToDictionary(x => (T)x.GetValue(obj), z => z.Name);
        }
    }
}