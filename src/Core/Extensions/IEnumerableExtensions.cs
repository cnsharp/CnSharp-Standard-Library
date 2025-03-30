using System;
using System.Collections.Generic;

namespace CnSharp.Extensions
{
    /// <summary>
    /// Collection of extension methods for IEnumerable
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Executes the specified action on each element of the data.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the data collection</typeparam>
        /// <param name="data">The collection of elements to iterate over.</param>
        /// <param name="action">The System.Action&lt;T&gt; delegate to perform on each element of the data.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when data is null</exception>
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
        /// Executes the specified action on each element of the data.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the data collection</typeparam>
        /// <param name="data">The collection of elements to iterate over.</param>
        /// <param name="action">The System.Action&lt;T, int&gt; delegate to perform on each element of the data.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when data is null</exception>
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
