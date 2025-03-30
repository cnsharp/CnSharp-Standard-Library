using System;
using System.Collections.Generic;
using System.Threading;

namespace CnSharp.Contexts
{
    /// <summary>
    /// Data context mirror
    /// </summary>
    [Serializable]
    public class MirrorContext : DataContext
    {
        #region Business Properties

        /// <summary>
        /// Gets the thread information of the system when saving the mirror
        /// </summary>
        public Thread OriginalThread { get; private set; }

        #endregion Business Properties

        #region Entrance

        /// <summary>
        /// Initializes a new instance of the <see cref="MirrorContext"/> class
        /// </summary>
        /// <param name="context">An instance of a shared data context</param>
        public MirrorContext(DataContext context)
        {
            OriginalThread = Thread.CurrentThread;
            this.Items = new Dictionary<string, object>(context.Items);
        }

        #endregion Entrance
    }
}