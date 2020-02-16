using System.IO;
using System.Text;

namespace CnSharp.IO
{
	/// <summary>
	/// File I/O
	/// </summary>
	public static class FileIO
	{
		#region Public Methods

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
		/// 读文本
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string ReadText(string path)
		{
			return ReadText(path, Encoding.UTF8);
		}

		/// <summary>
		/// 读文本
		/// </summary>
		/// <param name="path"></param>
		/// <param name="coding"></param>
		/// <returns></returns>
		public static string ReadText(string path, Encoding coding)
		{
			using (var sr = new StreamReader(path, coding)) return sr.ReadToEnd();
		}

		/// <summary>
		/// 写文件
		/// </summary>
		/// <param name="path"></param>
		/// <param name="content"></param>
		public static void WriteText(string path, string content)
		{
			WriteText(path, content, Encoding.UTF8);
		}

		/// <summary>
		/// 写文本
		/// </summary>
		/// <param name="path"></param>
		/// <param name="content"></param>
		/// <param name="coding"></param>
		public static void WriteText(string path, string content, Encoding coding)
		{
			using (var sw = new StreamWriter(path, false, coding)) sw.Write(content);
		}

		#endregion
	}
}