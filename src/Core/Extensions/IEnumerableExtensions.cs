using System;
using System.Collections.Generic;

namespace CnSharp.Extensions
{
    /// <summary>
    /// IEnumerable 扩展方法集合
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// 对 data 的每个元素执行指定操作。
        /// </summary>
        /// <typeparam name="T">data 集合的元素类型</typeparam>
        /// <param name="data">要进行迭代的元素集合。</param>
        /// <param name="action">要对 data 的每个元素执行的 System.Actionlt;Tgt; 委托。</param>
        /// <exception cref="System.ArgumentNullException">data</exception>
        public static void ForEach<T>(this IEnumerable<T> data, Action<T> action)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            foreach (var item in data)
            {
                action(item);
            }
        }

        /// <summary>
        /// 对 data 的每个元素执行指定操作。
        /// </summary>
        /// <typeparam name="T">data 集合的元素类型</typeparam>
        /// <param name="data">要进行迭代的元素集合。</param>
        /// <param name="action">要对 data 的每个元素执行的 System.Actionlt;T, intgt; 委托。</param>
        /// <exception cref="System.ArgumentNullException">data</exception>
        public static void ForEach<T>(this IEnumerable<T> data, Action<T, int> action)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            int index = 0;
            foreach (var item in data)
            {
                action(item, index++);
            }
        }

    }
}