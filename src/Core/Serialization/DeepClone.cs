using System.IO;

namespace CnSharp.Serialization
{
    /// <summary>
    /// Class for deep cloning of objects
    /// </summary>
    public static class DeepClone
    {
        /// <summary>
        /// Deep clones an object instance.
        /// </summary>
        /// <typeparam name="TObject">The target class to be serialized</typeparam>
        /// <param name="obj">An instance of the target class.</param>
        /// <param name="serializationAction">Method for deep cloning the object: <c>null</c> = binary cloning (must be marked with Serializable attribute or implement ISerializable interface).</param>
        /// <returns>A new instance of the target class obtained through deep cloning.</returns>
        public static TObject Copy<TObject>(this TObject obj, IObjectSerialization serializationAction = null) where TObject : class
        {
            if (ReferenceEquals(obj, null))
                return obj;

            if (ReferenceEquals(serializationAction, null))
                serializationAction = new BinarySerialization { ObjectType = typeof(TObject) };
            else
                serializationAction.ObjectType = typeof(TObject);

            using (var stream = serializationAction.Serialize(obj))
            {
                stream.Seek(0, SeekOrigin.Begin);
                return (TObject)serializationAction.Deserialize(stream);
            }
        }

        /// <summary>
        /// Gets an instance of binary serialization method.
        /// </summary>
        public static IObjectSerialization Binary { get { return new BinarySerialization(); } }

        /// <summary>
        /// Gets an instance of XML serialization method.
        /// </summary>
        public static IObjectSerialization Xml { get { return new XmlSerialization(); } }
    }
}