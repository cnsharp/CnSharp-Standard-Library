using System;
using System.Threading;

namespace CnSharp.Contexts
{
    /// <summary>
    /// Shared data context scope
    /// </summary>
    public class DataContextScope : IDisposable
    {
        #region Private Fields

        private DataContext _originalContext = DataContext.Current;

        #endregion Private Fields

        #region Entrance

        /// <summary>
        /// Initializes a new instance of the <see cref="DataContextScope"/> class
        /// </summary>
        /// <param name="contextOption">Shared data context configuration information</param>
        public DataContextScope(DataContextOption contextOption = DataContextOption.Required)
        {
            switch (contextOption)
            {
                case DataContextOption.RequiresNew:
                    DataContext.Current = new DataContext();
                    break;

                case DataContextOption.Required:
                    DataContext.Current = _originalContext ?? new DataContext();
                    break;

                case DataContextOption.Suppress:
                    DataContext.Current = null;
                    break;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataContextScope"/> class
        /// </summary>
        /// <param name="mirrorContext">A snapshot of a shared data context at a certain point in time</param>
        public DataContextScope(MirrorContext mirrorContext)
        {
            if (mirrorContext.OriginalThread == Thread.CurrentThread)
            {
                throw new InvalidOperationException("The DependentContextScope cannot be created in the thread in which the DependentContext is created.");
            }
            DataContext.Current = mirrorContext;
        }

        #endregion Entrance

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources
        /// </summary>
        public void Dispose()
        {
            DataContext.Current = _originalContext;
        }

        #endregion IDisposable Members
    }
}
