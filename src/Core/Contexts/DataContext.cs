using System;
using System.Collections.Generic;

namespace CnSharp.Contexts
{
    /// <summary>
    /// Represents a shared data context.
    /// </summary>
    [Serializable]
    public class DataContext
    {
        #region Private Fields

        /// <summary>
        /// Holds the current instance of the DataContext for the current thread.
        /// </summary>
        [ThreadStatic]
        private static DataContext _current;

        #endregion Private Fields

        #region Business Properties

        /// <summary>
        /// Gets or sets the current instance of the shared data context.
        /// </summary>
        public static DataContext Current
        {
            get { return _current; }
            internal set { _current = value; }
        }

        /// <summary>
        /// Gets the collection of data contained in the shared data context.
        /// </summary>
        public IDictionary<string, object> Items { get; internal set; }

        #endregion Business Properties

        #region Entrance

        /// <summary>
        /// Initializes a new instance of the DataContext class.
        /// </summary>
        internal DataContext()
        {
            this.Items = new Dictionary<string, object>();
        }

        #endregion Entrance

        #region Business Methods

        /// <summary>
        /// Creates a deep copy of the current shared data context.
        /// </summary>
        /// <returns>A new instance of MirrorContext that is a deep copy of the current context.</returns>
        public MirrorContext Clone()
        {
            return new MirrorContext(this);
        }

        /// <summary>
        /// Gets the data stored in the context by the specified key.
        /// </summary>
        /// <typeparam name="T">The type of the target data.</typeparam>
        /// <param name="key">The key used to retrieve the target data.</param>
        /// <param name="defaultValue">The value to return if the target data is not found.</param>
        /// <returns>The data stored in the context by the specified key.</returns>
        public T GetValue<T>(string key, T defaultValue = default(T))
        {
            object value;
            if (this.Items.TryGetValue(key, out value))
            {
                return (T)value;
            }
            return defaultValue;
        }

        /// <summary>
        /// Adds or updates the value for the specified key in the shared data context.
        /// </summary>
        /// <param name="key">The key used to set the target data.</param>
        /// <param name="value">The value of the target data.</param>
        public void SetValue(string key, object value)
        {
            this.Items[key] = value;
        }

        #endregion Business Methods
    }
}