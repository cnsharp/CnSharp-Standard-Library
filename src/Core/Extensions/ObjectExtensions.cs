namespace CnSharp.Extensions
{
    /// <summary>
    /// Provides extension methods for objects.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Determines whether the specified object is a numeric type.
        /// </summary>
        /// <param name="value">The object to check.</param>
        /// <returns><c>true</c> if the object is a numeric type; otherwise, <c>false</c>.</returns>
        public static bool IsNumber(this object value)
        {
            return value is sbyte
                   || value is byte
                   || value is short
                   || value is ushort
                   || value is int
                   || value is uint
                   || value is long
                   || value is ulong
                   || value is float
                   || value is double
                   || value is decimal;
        }
    }
}
