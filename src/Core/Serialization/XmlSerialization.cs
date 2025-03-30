using System;
using System.IO;
using System.Xml.Serialization;

namespace CnSharp.Serialization
{
    /// <summary>
    /// XML form object cloning class
    /// </summary>
    public class XmlSerialization : IObjectSerialization
    {
        private XmlSerializer _serializer = null;

        /// <summary>
        /// Gets the XML serialization instance object.
        /// </summary>
        public XmlSerializer Serializer
        {
            get { return (_serializer ?? (_serializer = new XmlSerializer(ObjectType))); }
        }

        /// <summary>
        /// Gets or sets the type of the object to be deeply cloned.
        /// </summary>
        public Type ObjectType { get; set; }

        /// <summary>
        /// Clones an object and generates a stream.
        /// </summary>
        /// <param name="obj">The original object to be cloned, which cannot be <c>null</c>.</param>
        /// <returns>A stream containing the serialized data of the original object.</returns>
        public Stream Serialize(object obj)
        {
            Stream stream = new MemoryStream();
            Serializer.Serialize(stream, obj);
            return stream;
        }

        /// <summary>
        /// Reads and generates a new object specified by the <see cref="ObjectType" /> property from a data stream.
        /// </summary>
        /// <param name="stream">The stream containing the serialized data of the original object.</param>
        /// <returns>A new object whose data is obtained from the specified stream.</returns>
        public object Deserialize(Stream stream)
        {
            return Serializer.Deserialize(stream);
        }

    }
}