using System;
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace CnSharp.IO
{
	public class GZip
	{
		#region Public Methods

		/// <summary>
		/// Compress
		/// </summary>
		/// <param name="lpSourceFolder">The location of the files to include in the zip file, all files including files in subfolders will be included.</param>
		/// <param name="lpDestFolder">Folder to write the zip file into</param>
		/// <param name="zipFileName">Name of the zip file to write</param>
		public static GZipResult Compress(string lpSourceFolder, string lpDestFolder, string zipFileName)
		{
			return Compress(lpSourceFolder, "*.*", SearchOption.AllDirectories, lpDestFolder, zipFileName, true);
		}

		/// <summary>
		/// Compress
		/// </summary>
		/// <param name="lpSourceFolder">The location of the files to include in the zip file</param>
		/// <param name="searchPattern">Search pattern (ie "*.*" or "*.txt" or "*.gif") to idendify what files in lpSourceFolder to include in the zip file</param>
		/// <param name="searchOption">Only files in lpSourceFolder or include files in subfolders also</param>
		/// <param name="lpDestFolder">Folder to write the zip file into</param>
		/// <param name="zipFileName">Name of the zip file to write</param>
		/// <param name="deleteTempFile">Boolean, true deleted the intermediate temp file, false leaves the temp file in lpDestFolder (for debugging)</param>
		public static GZipResult Compress(
			string lpSourceFolder,
			string searchPattern,
			SearchOption searchOption,
			string lpDestFolder,
			string zipFileName,
			bool deleteTempFile)
		{
			var di = new DirectoryInfo(lpSourceFolder);
			FileInfo[] files = di.GetFiles("*.*", searchOption);
			return Compress(files, lpSourceFolder, lpDestFolder, zipFileName, deleteTempFile);
		}

		/// <summary>
		/// Compress
		/// </summary>
		/// <param name="files">Array of FileInfo objects to be included in the zip file</param>
		/// <param name="lpBaseFolder">Base folder to use when creating relative paths for the files 
		/// stored in the zip file. For example, if lpBaseFolder is 'C:\zipTest\Files\', and there is a file 
		/// 'C:\zipTest\Files\folder1\sample.txt' in the 'files' array, the relative path for sample.txt 
		/// will be 'folder1/sample.txt'</param>
		/// <param name="lpDestFolder">Folder to write the zip file into</param>
		/// <param name="zipFileName">Name of the zip file to write</param>
		public static GZipResult Compress(FileInfo[] files, string lpBaseFolder, string lpDestFolder, string zipFileName)
		{
			return Compress(files, lpBaseFolder, lpDestFolder, zipFileName, true);
		}

		/// <summary>
		/// Compress
		/// </summary>
		/// <param name="files">Array of FileInfo objects to be included in the zip file</param>
		/// <param name="lpBaseFolder">Base folder to use when creating relative paths for the files 
		/// stored in the zip file. For example, if lpBaseFolder is 'C:\zipTest\Files\', and there is a file 
		/// 'C:\zipTest\Files\folder1\sample.txt' in the 'files' array, the relative path for sample.txt 
		/// will be 'folder1/sample.txt'</param>
		/// <param name="lpDestFolder">Folder to write the zip file into</param>
		/// <param name="zipFileName">Name of the zip file to write</param>
		/// <param name="deleteTempFile">Boolean, true deleted the intermediate temp file, false leaves the temp file in lpDestFolder (for debugging)</param>
		public static GZipResult Compress(
			FileInfo[] files, string lpBaseFolder, string lpDestFolder, string zipFileName, bool deleteTempFile)
		{
			var result = new GZipResult();

			if (!lpDestFolder.EndsWith("\\"))
			{
				lpDestFolder += "\\";
			}

			string lpTempFile = lpDestFolder + zipFileName + ".tmp";
			string lpZipFile = lpDestFolder + zipFileName;

			result.TempFile = lpTempFile;
			result.ZipFile = lpZipFile;

			int fileCount = 0;

			if (files != null && files.Length > 0)
			{
				CreateTempFile(files, lpBaseFolder, lpTempFile, result);

				if (result.FileCount > 0)
				{
					CreateZipFile(lpTempFile, lpZipFile, result);
				}

				// delete the temp file
				try
				{
					if (deleteTempFile)
					{
						File.Delete(lpTempFile);
						result.TempFileDeleted = true;
					}
				}
				catch (Exception ex4)
				{
					// handle or display the error 
					throw ex4;
				}
			}
			return result;
		}

		//public static GZipResult Decompress(string lpSourceFolder, string lpDestFolder, string zipFileName)
		//{
		//    return Decompress(lpSourceFolder, lpDestFolder, zipFileName, true);
		//}

		//public static GZipResult Decompress(string lpSrcFolder, string lpDestFolder, string zipFileName, bool deleteTempFile)
		//{

		//}

		public static GZipResult Decompress(string lpZipFile, string lpDestFolder)
		{
			return Decompress(lpZipFile, lpDestFolder, true);
		}

		public static GZipResult Decompress(string lpZipFile, string lpDestFolder, bool deleteTempFile)
		{
			var result = new GZipResult();

			if (!lpDestFolder.EndsWith("\\"))
			{
				lpDestFolder += "\\";
			}
			if (!Directory.Exists(lpDestFolder))
			{
				Directory.CreateDirectory(lpDestFolder);
			}

			string lpTempFile = lpDestFolder + Path.GetFileName(lpZipFile) + ".tmp";

			result.TempFile = lpTempFile;
			result.ZipFile = lpZipFile;

			string line = null;
			string lpFilePath = null;
			GZippedFile gzf = null;
			FileStream fsTemp = null;
			var gzfs = new ArrayList();

			// extract the files from the temp file
			try
			{
				fsTemp = UnzipToTempFile(lpZipFile, lpTempFile, result);
				if (fsTemp != null)
				{
					while (fsTemp.Position != fsTemp.Length)
					{
						line = null;
						while (string.IsNullOrEmpty(line) && fsTemp.Position != fsTemp.Length)
						{
							line = ReadLine(fsTemp);
						}

						if (!string.IsNullOrEmpty(line))
						{
							gzf = GZippedFile.GetGZippedFile(line);
							if (gzf != null && gzf.Length > 0)
							{
								gzfs.Add(gzf);
								lpFilePath = lpDestFolder + gzf.RelativePath;
								gzf.LocalPath = lpFilePath;
								WriteFile(fsTemp, gzf.Length, lpFilePath);
								gzf.Restored = true;
							}
						}
					}
				}
			}
				//catch (Exception ex3)
				//{
				//    // handle or display the error 
				//    throw ex3;
				//}
			finally
			{
				if (fsTemp != null)
				{
					fsTemp.Close();
					fsTemp = null;
				}
			}

			// delete the temp file
			try
			{
				if (deleteTempFile)
				{
					File.Delete(lpTempFile);
					result.TempFileDeleted = true;
				}
			}
			catch (Exception ex4)
			{
				// handle or display the error 
				throw ex4;
			}

			result.FileCount = gzfs.Count;
			gzfs.CopyTo(result.Files);
			return result;
		}

		#endregion

		#region Methods

		private static void CreateTempFile(FileInfo[] files, string lpBaseFolder, string lpTempFile, GZipResult result)
		{
			byte[] buffer;
			int count = 0;
			byte[] header;
			string fileHeader = null;
			string fileModDate = null;
			string lpFolder = null;
			int fileIndex = 0;
			string lpSourceFile = null;
			string vpSourceFile = null;
			GZippedFile gzf = null;
			FileStream fsOut = null;
			FileStream fsIn = null;

			if (files != null && files.Length > 0)
			{
				try
				{
					result.Files = new GZippedFile[files.Length];

					// open the temp file for writing
					fsOut = new FileStream(lpTempFile, FileMode.Create, FileAccess.Write, FileShare.None);

					foreach (FileInfo fi in files)
					{
						lpFolder = fi.DirectoryName + "\\";
						try
						{
							gzf = new GZippedFile();
							gzf.Index = fileIndex;

							// read the source file, get its virtual path within the source folder
							lpSourceFile = fi.FullName;
							gzf.LocalPath = lpSourceFile;
							vpSourceFile = lpSourceFile.Replace(lpBaseFolder, string.Empty);
							vpSourceFile = vpSourceFile.Replace("\\", "/");
							gzf.RelativePath = vpSourceFile;

							fsIn = new FileStream(lpSourceFile, FileMode.Open, FileAccess.Read, FileShare.Read);
							buffer = new byte[fsIn.Length];
							count = fsIn.Read(buffer, 0, buffer.Length);
							fsIn.Close();
							fsIn = null;

							fileModDate = fi.LastWriteTimeUtc.ToString();
							gzf.ModifiedDate = fi.LastWriteTimeUtc;
							gzf.Length = buffer.Length;

							fileHeader = fileIndex.ToString() + "," + vpSourceFile + "," + fileModDate + "," + buffer.Length.ToString()
							             + "\n";
							header = Encoding.Default.GetBytes(fileHeader);

							fsOut.Write(header, 0, header.Length);
							fsOut.Write(buffer, 0, buffer.Length);
							fsOut.WriteByte(10); // linefeed

							gzf.AddedToTempFile = true;

							// update the result object
							result.Files[fileIndex] = gzf;

							// increment the fileIndex
							fileIndex++;
						}
						catch (Exception ex1)
						{
							// handle or display the error 
							throw ex1;
						}
						finally
						{
							if (fsIn != null)
							{
								fsIn.Close();
								fsIn = null;
							}
						}
						if (fsOut != null)
						{
							result.TempFileSize = fsOut.Length;
						}
					}
				}
				catch (Exception ex2)
				{
					// handle or display the error 
					throw ex2;
				}
				finally
				{
					if (fsOut != null)
					{
						fsOut.Close();
						fsOut = null;
					}
				}
			}

			result.FileCount = fileIndex;
		}

		private static void CreateZipFile(string lpSourceFile, string lpZipFile, GZipResult result)
		{
			byte[] buffer;
			int count = 0;
			FileStream fsOut = null;
			FileStream fsIn = null;
			GZipStream gzip = null;

			// compress the file into the zip file
			try
			{
				fsOut = new FileStream(lpZipFile, FileMode.Create, FileAccess.Write, FileShare.None);
				gzip = new GZipStream(fsOut, CompressionMode.Compress, true);

				fsIn = new FileStream(lpSourceFile, FileMode.Open, FileAccess.Read, FileShare.Read);
				buffer = new byte[fsIn.Length];
				count = fsIn.Read(buffer, 0, buffer.Length);
				fsIn.Close();
				fsIn = null;

				// compress to the zip file
				gzip.Write(buffer, 0, buffer.Length);

				result.ZipFileSize = fsOut.Length;
				result.CompressionPercent = GetCompressionPercent(result.TempFileSize, result.ZipFileSize);
			}
			catch (Exception ex1)
			{
				// handle or display the error 
				throw ex1;
			}
			finally
			{
				if (gzip != null)
				{
					gzip.Close();
					gzip = null;
				}
				if (fsOut != null)
				{
					fsOut.Close();
					fsOut = null;
				}
				if (fsIn != null)
				{
					fsIn.Close();
					fsIn = null;
				}
			}
		}

		private static int GetCompressionPercent(long tempLen, long zipLen)
		{
			double tmp = tempLen;
			double zip = zipLen;
			double hundred = 100;

			double ratio = (tmp - zip) / tmp;
			double pcnt = ratio * hundred;

			return (int)pcnt;
		}

		private static string GetFolder(string filename)
		{
			string vpFolder = filename;
			int index = filename.LastIndexOf("/");
			if (index != -1)
			{
				vpFolder = filename.Substring(0, index + 1);
			}
			return vpFolder;
		}

		private static string ReadLine(FileStream fs)
		{
			string line = string.Empty;

			const int bufferSize = 4096;
			var buffer = new byte[bufferSize];
			byte b = 0;
			byte lf = 10;
			int i = 0;

			while (b != lf)
			{
				b = (byte)fs.ReadByte();
				buffer[i] = b;
				i++;
			}

			line = Encoding.Default.GetString(buffer, 0, i - 1);

			return line;
		}

		private static FileStream UnzipToTempFile(string lpZipFile, string lpTempFile, GZipResult result)
		{
			FileStream fsIn = null;
			GZipStream gzip = null;
			FileStream fsOut = null;
			FileStream fsTemp = null;

			const int bufferSize = 4096;
			var buffer = new byte[bufferSize];
			int count = 0;

			try
			{
				fsIn = new FileStream(lpZipFile, FileMode.Open, FileAccess.Read, FileShare.Read);
				result.ZipFileSize = fsIn.Length;

				fsOut = new FileStream(lpTempFile, FileMode.Create, FileAccess.Write, FileShare.None);
				gzip = new GZipStream(fsIn, CompressionMode.Decompress, true);
				while (true)
				{
					count = gzip.Read(buffer, 0, bufferSize);
					if (count != 0)
					{
						fsOut.Write(buffer, 0, count);
					}
					if (count != bufferSize)
					{
						break;
					}
				}
			}
				//catch (Exception ex1)
				//{
				//    // handle or display the error 
				//    throw ex1;
				//}
			finally
			{
				if (gzip != null)
				{
					gzip.Close();
					gzip = null;
				}
				if (fsOut != null)
				{
					fsOut.Close();
					fsOut = null;
				}
				if (fsIn != null)
				{
					fsIn.Close();
					fsIn = null;
				}
			}

			fsTemp = new FileStream(lpTempFile, FileMode.Open, FileAccess.Read, FileShare.None);
			if (fsTemp != null)
			{
				result.TempFileSize = fsTemp.Length;
			}
			return fsTemp;
		}

		private static void WriteFile(FileStream fs, int fileLength, string lpFile)
		{
			FileStream fsFile = null;

			try
			{
				string lpFolder = GetFolder(lpFile);
				if (!string.IsNullOrEmpty(lpFolder) && lpFolder != lpFile && !Directory.Exists(lpFolder))
				{
					Directory.CreateDirectory(lpFolder);
				}

				var buffer = new byte[fileLength];
				int count = fs.Read(buffer, 0, fileLength);
				fsFile = new FileStream(lpFile, FileMode.Create, FileAccess.Write, FileShare.None);
				fsFile.Write(buffer, 0, buffer.Length);
				fsFile.Write(buffer, 0, count);
			}
			catch (Exception ex2)
			{
				// handle or display the error 
				throw ex2;
			}
			finally
			{
				if (fsFile != null)
				{
					fsFile.Flush();
					fsFile.Close();
					fsFile = null;
				}
			}
		}

		#endregion
	}
}