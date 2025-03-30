using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace CnSharp.Serialization
{
	/// <summary>
	///  serialize/deserialize object by xml formatter
	/// </summary>
	public static class XmlSerializationHelper
	{
		/// <summary> 
		/// serialize object to xml string
		/// </summary> 
		/// <param name="obj">object</param> 
		/// <returns>XML string</returns> 
		public static string SerializeToXml<T>(T obj)
		{
			using (var stream = new MemoryStream())
			{
				using (var writer = new XmlTextWriter(stream, Encoding.UTF8))
				{
					writer.Indentation = 3;
					writer.IndentChar = ' ';
					writer.Formatting = Formatting.Indented;
					new XmlSerializer(obj.GetType()).Serialize(writer, obj);
					var xml = Encoding.UTF8.GetString(stream.ToArray());
					var index = xml.IndexOf("?>");
					if (index > 0)
					{
						xml = xml.Substring(index + 2);
					}
					return xml.Trim();
				}
			}
		}

		/// <summary> 
		/// serialize object to xml file
		/// </summary> 
		/// <param name="obj">object</param>
		/// <param name="filePath">file name</param>
		public static void SerializeToXmlFile<T>(T obj, string filePath)
		{
			using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
			{
				var serializer = new XmlSerializer(obj.GetType());
				serializer.Serialize(writer, obj);
			}
		}

		/// <summary>
		/// Deserialize object from xml string
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="xml">xml string</param>
		/// <returns></returns>
		public static T DeserializeFromXmlString<T>(string xml)
		{
			var serializer = new XmlSerializer(typeof (T));
			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
			{
				return (T) serializer.Deserialize(stream);
			}
		}

		/// <summary>
		/// Deserialize object from xml file
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="filePath">xml file name</param>
		/// <returns></returns>
		public static T DeserializeFromXmlFile<T>(string filePath)
		{
			using (var reader = new StreamReader(filePath, Encoding.UTF8))
			{
				var serializer = new XmlSerializer(typeof (T));
				return (T) serializer.Deserialize(reader);
			}
		}

	}
}