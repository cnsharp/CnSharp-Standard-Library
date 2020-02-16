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
		#region Public Methods

	
		/// <summary> 
		/// serialize object to xml string
		/// </summary> 
		/// <param name="obj">object</param> 
		/// <returns>XML string</returns> 
		public static string ToXml<T>(this T obj)
		{
			using (var ms = new MemoryStream())
			using (var writer = new XmlTextWriter(ms, Encoding.UTF8))
			{
				writer.Indentation = 3;
				writer.IndentChar = ' ';
				writer.Formatting = Formatting.Indented;
				var sc = new XmlSerializer(obj.GetType());
				sc.Serialize(writer, obj);
				string xml = Encoding.UTF8.GetString(ms.ToArray());
				int index = xml.IndexOf("?>");
				if (index > 0)
				{
					xml = xml.Substring(index + 2);
				}
				return xml.Trim();
			}
		}

		/// <summary>
		/// Deserialize object from xml file
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="filename">xml file name</param>
		/// <returns></returns>
		public static T LoadObjectFromXmlFile<T>(string filename)
		{
			var doc = new XmlDocument();
			doc.Load(filename);
            return LoadObjectFromXml<T>(doc.InnerXml);
		}

		/// <summary>
		/// Deserialize object from xml string
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="xml">xml string</param>
		/// <returns></returns>
		public static T LoadObjectFromXml<T>(string xml)
		{
			var serializer = new XmlSerializer(typeof(T));
			using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
			{
				return (T)serializer.Deserialize(ms);
			}
		}

		#endregion
	}
}