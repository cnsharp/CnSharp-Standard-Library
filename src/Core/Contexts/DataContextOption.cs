namespace CnSharp.Contexts
{
    /// <summary>
    /// Shared data context configuration information
    /// </summary>
    public enum DataContextOption
    {
        /// <summary>
        /// On the same thread, the outer DataContext is directly used by the inner layer
        /// </summary>
        Required,

        /// <summary>
        /// On the same thread, a new DataContext is created each time for the inner layer
        /// </summary>
        RequiresNew,

        /// <summary>
        /// On the same thread, the outer DataContextOption is suppressed in the inner layer, and the current DataContextOption does not exist in the inner layer
        /// </summary>
        Suppress
    }
}
