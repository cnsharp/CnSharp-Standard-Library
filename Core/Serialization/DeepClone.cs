using System.IO;

namespace CnSharp.Serialization
{
    /// <summary>
    /// 实例对象深层对象拷贝类
    /// </summary>
    public static class DeepClone
    {
        /// <summary>
        /// 深层拷贝一个实例对象。
        /// </summary>
        /// <typeparam name="TObject">带序列化的目标类</typeparam>
        /// <param name="obj">目标类的一个实例对象。</param>
        /// <param name="serializationAction">深层拷贝对象的方式：<c>null</c> = 二进制拷贝方式（必须标记 Serializable 特性，或者实现 ISerializable 接口）。</param>
        /// <returns>深层克隆的一个目标类的实例对象。</returns>
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
        /// 获取二进制序列化对象方式的一个实例对象。
        /// </summary>
        public static IObjectSerialization Binary { get { return new BinarySerialization(); } }

        /// <summary>
        /// 获取 XML 序列化对象方式的一个实例对象。
        /// </summary>
        public static IObjectSerialization Xml { get { return new XmlSerialization(); } }
    }
}