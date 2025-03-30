using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using CnSharp.Serialization;

namespace CnSharp.Extensions
{
    /// <summary>
    /// 枚举类型的扩展
    /// </summary>
    [DebuggerStepThrough]
    public static class EnumExtensions
    {
        private static readonly IDictionary<Tuple<Enum, Type>, object[]> Cache = new Dictionary<Tuple<Enum, Type>, object[]>();
        private static readonly ReaderWriterLockSlim LockCache = new ReaderWriterLockSlim();
        private static readonly Dictionary<Type, Dictionary<int, string>> DictKeyDescription = new Dictionary<Type, Dictionary<int, string>>();
        
        /// <summary>
        /// 获得当前枚举实例的 <see cref="System.ComponentModel.DescriptionAttribute"/> 属性值.
        /// </summary>
        /// <param name="value">当前枚举实例.</param>
        /// <returns>当前枚举实例的 <see cref="System.ComponentModel.DescriptionAttribute"/> 属性值.</returns>
        public static string GetDescription(this Enum value)
        {
            var item = GetAttribute<DescriptionAttribute>(value).FirstOrDefault();
            if (item != null)
                return item.Description;
            return string.Empty;
        }

        /// <summary>
        /// 获得枚举字段关联的指定特性(Attribute)的列表。
        /// </summary>
        /// <typeparam name="TAttribute">扩展特性类型</typeparam>
        /// <param name="value">当前枚举实例。</param>
        /// <returns>枚举字段的特性列表</returns>
        public static TAttribute[] GetAttribute<TAttribute>(this Enum value) where TAttribute : Attribute
        {
            var key = Tuple.Create(value, typeof(TAttribute));
            object[] data = null;

            try
            {
                LockCache.EnterUpgradeableReadLock();
                if (!Cache.TryGetValue(key, out data))
                {
                    try
                    {
                        LockCache.EnterWriteLock();

                        FieldInfo field = value.GetType().GetField(value.ToString());
                        data = field.GetCustomAttributes(typeof(TAttribute), false);

                        Cache.Add(key, data);
                    }
                    finally
                    {
                        LockCache.ExitWriteLock(); 
                    }
                }
            }
            finally
            {
                LockCache.ExitUpgradeableReadLock();
            }
            return data as TAttribute[];
        }

        // private static readonly Dictionary<Type, Dictionary<string, string>> DictNameDescription = new Dictionary<Type, Dictionary<string, string>>();
        ///// <summary>
        ///// 枚举类型的枚举项与描述转为NameValue List（Name为描述 Value为枚举名）
        ///// </summary>
        ///// <param name="enumType"></param>
        ///// <returns></returns>
        // public static Dictionary<string, string> ToDictionary(this Type enumType)
        //{
        //    if (DictNameDescription.ContainsKey(enumType))
        //        return DictNameDescription[enumType].Copy();
        //    var dict = new Dictionary<string, string>();
        //    foreach (MemberInfo memberInfo in enumType.GetMembers())
        //    {
        //        foreach (Attribute attr in Attribute.GetCustomAttributes(memberInfo))
        //        {
        //            if (attr.GetType() == typeof(DescriptionAttribute))
        //            {
        //                dict.Add(Enum.Parse(enumType, memberInfo.Name).ToString(),  ((DescriptionAttribute)attr).Description);

        //                break;
        //            }
        //        }
        //    }
        //    DictNameDescription.Add(enumType, dict);
        //    return dict.Copy();
        //}

        
         /// <summary>
         /// 枚举类型的枚举项与描述转为NameValue List（Name为描述 Value为枚举名）
         /// </summary>
         /// <param name="enumType"></param>
         /// <returns></returns>
         public static Dictionary<int, string> ToDictionary(this Type enumType)
         {
             if (DictKeyDescription.ContainsKey(enumType))
                 return DictKeyDescription[enumType].Copy();
             var dict = new Dictionary<int, string>();
             foreach (MemberInfo memberInfo in enumType.GetMembers())
             {
                 foreach (Attribute attr in Attribute.GetCustomAttributes(memberInfo))
                 {
                     if (attr.GetType() == typeof(DescriptionAttribute))
                     {
                         dict.Add(Enum.Parse(enumType, memberInfo.Name).GetHashCode(), ((DescriptionAttribute)attr).Description);

                         break;
                     }
                 }
             }
             DictKeyDescription.Add(enumType, dict);
             return dict.Copy();
         }
    }
}