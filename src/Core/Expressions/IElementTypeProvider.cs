using System;

namespace CnSharp.Expressions
{
    /// <summary>
    /// Provides an interface for obtaining the original element type.
    /// </summary>
    internal interface IElementTypeProvider
    {
        /// <summary>
        /// Gets or sets the original element type.
        /// </summary>
        Type OriginalElementType { get; set; }
    }
}
