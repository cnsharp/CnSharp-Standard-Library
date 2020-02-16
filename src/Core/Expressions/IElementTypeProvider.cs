using System;

namespace CnSharp.Expressions
{
    internal interface IElementTypeProvider
    {
        Type OriginalElementType { get; set; }
    }
}