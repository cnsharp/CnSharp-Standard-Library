using System.IO;
using System.Text;

namespace CnSharp.IO
{
	/// <summary>
	/// File I/O
	/// </summary>
	public static class FileIO
	{
		public static void CopyFiles(string sourceDirectory, string targetDirectory)
		{
			if (!Directory.Exists(sourceDirectory))
			{
				return;
			}
			if (!Directory.Exists(targetDirectory))
			{
				Directory.CreateDirectory(targetDirectory);
			}

			string[] directories = Directory.GetDirectories(sourceDirectory);
			if (directories.Length > 0)
			{
				foreach (string d in directories)
				{
					CopyFiles(d, targetDirectory + d.Substring(d.LastIndexOf("\\")));
				}
			}

			string[] files = Directory.GetFiles(sourceDirectory);
			if (files.Length > 0)
			{
				foreach (string s in files)
				{
					File.Copy(s, targetDirectory + s.Substring(s.LastIndexOf("\\")), true);
				}
			}
		}

		/// <summary>
		 /// Read text
		 /// </summary>
		 /// <param name="path"></param>
		 /// <returns></returns>
		public static string ReadText(string path)
		{
			return ReadText(path, Encoding.UTF8);
		}

		/// <summary>
		 /// Read text
		 /// </summary>
		 /// <param name="path"></param>
		 /// <param name="encoding"></param>
		 /// <returns></returns>
		public static string ReadText(string path, Encoding encoding)
		{
			using (var sr = new StreamReader(path, encoding)) return sr.ReadToEnd();
		}

		/// <summary>
		 /// Write file
		 /// </summary>
		 /// <param name="path"></param>
		 /// <param name="content"></param>
		public static void WriteText(string path, string content)
		{
			WriteText(path, content, Encoding.UTF8);
		}

		/// <summary>
		 /// Write text
		 /// </summary>
		 /// <param name="path"></param>
		 /// <param name="content"></param>
		 /// <param name="encoding"></param>
		public static void WriteText(string path, string content, Encoding encoding)
		{
			using (var sw = new StreamWriter(path, false, encoding)) sw.Write(content);
		}

	}
}
