using System;
using System.IO;

namespace CnSharp.Serialization
{
    /// <summary>
    /// Interface for deep cloning objects
    /// </summary>
    public interface IObjectSerialization
    {
        /// <summary>
        /// Gets or sets the type of the object to be deeply cloned.
        /// </summary>
        Type ObjectType { get; set; }

        /// <summary>
        /// Clones an object and generates a stream.
        /// </summary>
        /// <param name="obj">The original object to be cloned, which cannot be <c>null</c>.</param>
        /// <returns>A stream containing the serialized data of the original object.</returns>
        Stream Serialize(object obj);

        /// <summary>
        /// Reads and generates a new object specified by the <see cref="ObjectType" /> property from a data stream.
        /// </summary>
        /// <param name="stream">The stream containing the serialized data of the original object.</param>
        /// <returns>A new object whose data is obtained from the specified stream.</returns>
        object Deserialize(Stream stream);
    }
}