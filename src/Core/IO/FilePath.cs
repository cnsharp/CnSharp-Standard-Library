using System.IO;
using System.Text.RegularExpressions;

namespace CnSharp.IO
{
	public static class FilePath
	{
		#region Constants and Fields

		public const string BlackChars = @"/ \ < > * ？";

		#endregion

		#region Public Methods

		/// <summary>
		/// 计算绝对路径
		/// </summary>
		/// <param name="absoluteDir">绝对目录</param>
		/// <param name="relativeFile">相对文件</param>
		/// <returns></returns>
		/// <example>
		/// @"D:\Windows\regedit.exe" = GetAbsolutePath(@"D:\Windows\Web\Wallpaper\", @"..\..\regedit.exe" );
		/// </example>
		public static string GetAbsolutePath(string absoluteDir, string relativeFile)
		{
			absoluteDir = FixPath(absoluteDir);
			relativeFile = FixPath(relativeFile);
			bool isNotOver = true;
			int intStart = 0;
			while (isNotOver)
			{
				if (relativeFile.StartsWith(@"..\"))
				{
					relativeFile = relativeFile.Remove(0, 3);
					intStart++;
				}
				else
				{
					isNotOver = false;
				}
			}

			if (intStart > 0)
			{
				if (absoluteDir.EndsWith("\\"))
				{
					absoluteDir = absoluteDir.Remove(absoluteDir.Length - 1);
				}

				for (int i = 0; i < intStart; i++)
				{
					absoluteDir = absoluteDir.Remove(absoluteDir.LastIndexOf("\\"));
				}
			}
			return Path.Combine(absoluteDir, relativeFile);
		}

		public static string GetDefaultFileName(string folder, string defaultName, string extension)
		{
			int i = 1;
			if (!folder.EndsWith("\\") && !folder.EndsWith("/"))
			{
				folder += "\\";
			}
			if (!extension.StartsWith("."))
			{
				extension = "." + extension;
			}
			string fileName = string.Empty;
			while (true)
			{
				fileName = string.Format("{0}{1}{2}{3}", folder, defaultName, i, extension);
				if (!File.Exists(fileName))
				{
					break;
				}
				i++;
			}
			return fileName;
		}

		public static string GetDefaultFolderName(string folder, string defaultName)
		{
			int i = 1;
			if (!folder.EndsWith("\\") && !folder.EndsWith("/"))
			{
				folder += "\\";
			}
			string folderName = string.Empty;
			while (true)
			{
				folderName = string.Format("{0}{1}{2}\\", folder, defaultName, i);
				if (!Directory.Exists(folderName))
				{
					break;
				}
				i++;
			}
			return folderName;
		}

		/// <summary>
		/// 计算相对路径
		/// 后者相对前者的路径。
		/// </summary>
		/// <param name="mainDir">主目录</param>
		/// <param name="fullFilePath">文件的绝对路径</param>
		/// <returns>fullFilePath相对于mainDir的路径</returns>
		/// <example>
		/// @"..\..\regedit.exe" = GetRelativePath(@"D:\Windows\Web\Wallpaper\", @"D:\Windows\regedit.exe" );
		/// </example>
		public static string GetRelativePath(string mainDir, string fullFilePath)
		{
			mainDir = FixPath(mainDir);
			fullFilePath = FixPath(fullFilePath);

			if (!mainDir.EndsWith("\\"))
			{
				mainDir += "\\";
			}

			int intIndex = -1, intPos = mainDir.IndexOf('\\');

			while (intPos >= 0)
			{
				intPos++;
				if (string.Compare(mainDir, 0, fullFilePath, 0, intPos, true) != 0)
				{
					break;
				}
				intIndex = intPos;
				intPos = mainDir.IndexOf('\\', intPos);
			}

			if (intIndex >= 0)
			{
				fullFilePath = fullFilePath.Substring(intIndex);
				intPos = mainDir.IndexOf("\\", intIndex);
				while (intPos >= 0)
				{
					fullFilePath = "..\\" + fullFilePath;
					intPos = mainDir.IndexOf("\\", intPos + 1);
				}
			}

			return fullFilePath;
		}

		/// <summary>
		/// 检查是否合法文件名
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public static bool IsValidFileName(string fileName)
		{
			foreach (char c in BlackChars)
			{
				if (fileName.IndexOf(c) >= 0)
				{
					return false;
				}
			}
			return true;
		}

		#endregion

		#region Methods

		private static string FixPath(string path)
		{
			path = path.Replace("/", "\\");
			return Regex.Replace(path, "\\\\{2,}", "\\", RegexOptions.Compiled);
		}

		#endregion
	}
}