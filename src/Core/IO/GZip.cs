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
        ///     Compress
        /// </summary>
        /// <param name="sourceFolder">
        ///     The location of the files to include in the zip file, all files including files in
        ///     subfolders will be included.
        /// </param>
        /// <param name="destFolder">Folder to write the zip file into</param>
        /// <param name="zipFileName">Name of the zip file to write</param>
        public static GZipResult Compress(string sourceFolder, string destFolder, string zipFileName)
        {
            return Compress(sourceFolder, "*.*", SearchOption.AllDirectories, destFolder, zipFileName, true);
        }

        /// <summary>
        ///     Compress
        /// </summary>
        /// <param name="sourceFolder">The location of the files to include in the zip file</param>
        /// <param name="searchPattern">
        ///     Search pattern (ie "*.*" or "*.txt" or "*.gif") to identify what files in sourceFolder to
        ///     include in the zip file
        /// </param>
        /// <param name="searchOption">Only files in sourceFolder or include files in subfolders also</param>
        /// <param name="lpDestFolder">Folder to write the zip file into</param>
        /// <param name="zipFileName">Name of the zip file to write</param>
        /// <param name="deleteTempFile">
        ///     Boolean, true deleted the intermediate temp file, false leaves the temp file in
        ///     lpDestFolder (for debugging)
        /// </param>
        public static GZipResult Compress(
            string sourceFolder,
            string searchPattern,
            SearchOption searchOption,
            string lpDestFolder,
            string zipFileName,
            bool deleteTempFile)
        {
            var di = new DirectoryInfo(sourceFolder);
            var files = di.GetFiles(searchPattern, searchOption);
            return Compress(files, sourceFolder, lpDestFolder, zipFileName, deleteTempFile);
        }


        /// <summary>
        ///     Compress
        /// </summary>
        /// <param name="files">Array of FileInfo objects to be included in the zip file</param>
        /// <param name="baseFolder">
        ///     Base folder to use when creating relative paths for the files
        ///     stored in the zip file. For example, if lpBaseFolder is 'C:\zipTest\Files\', and there is a file
        ///     'C:\zipTest\Files\folder1\sample.txt' in the 'files' array, the relative path for sample.txt
        ///     will be 'folder1/sample.txt'
        /// </param>
        /// <param name="destFolder">Folder to write the zip file into</param>
        /// <param name="zipFileName">Name of the zip file to write</param>
        /// <param name="deleteTempFile">
        ///     Boolean, true deleted the intermediate temp file, false leaves the temp file in
        ///     lpDestFolder (for debugging)
        /// </param>
        public static GZipResult Compress(
            FileInfo[] files, string baseFolder, string destFolder, string zipFileName, bool deleteTempFile = true)
        {
            var result = new GZipResult();

            if (!destFolder.EndsWith("\\")) destFolder += "\\";

            var lpTempFile = destFolder + zipFileName + ".tmp";
            var lpZipFile = destFolder + zipFileName;

            result.TempFile = lpTempFile;
            result.ZipFile = lpZipFile;

            if (files != null && files.Length > 0)
            {
                CreateTempFile(files, baseFolder, lpTempFile, result);

                if (result.FileCount > 0) CreateZipFile(lpTempFile, lpZipFile, result);

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

        public static GZipResult Decompress(string zipFile, string destFolder, bool deleteTempFile = true)
        {
            var result = new GZipResult();

            if (!destFolder.EndsWith("\\")) destFolder += "\\";
            if (!Directory.Exists(destFolder)) Directory.CreateDirectory(destFolder);

            var lpTempFile = destFolder + Path.GetFileName(zipFile) + ".tmp";

            result.TempFile = lpTempFile;
            result.ZipFile = zipFile;

            string line;
            string lpFilePath;
            GZippedFile gzf;
            FileStream fsTemp = null;
            var gzfs = new ArrayList();

            // extract the files from the temp file
            try
            {
                fsTemp = UnzipToTempFile(zipFile, lpTempFile, result);
                if (fsTemp != null)
                    while (fsTemp.Position != fsTemp.Length)
                    {
                        line = null;
                        while (string.IsNullOrEmpty(line) && fsTemp.Position != fsTemp.Length) line = ReadLine(fsTemp);

                        if (!string.IsNullOrEmpty(line))
                        {
                            gzf = GZippedFile.GetGZippedFile(line);
                            if (gzf != null && gzf.Length > 0)
                            {
                                gzfs.Add(gzf);
                                lpFilePath = destFolder + gzf.RelativePath;
                                gzf.LocalPath = lpFilePath;
                                WriteFile(fsTemp, gzf.Length, lpFilePath);
                                gzf.Restored = true;
                            }
                        }
                    }
            }
            finally
            {
                if (fsTemp != null) fsTemp.Close();
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
            var count = 0;
            byte[] header;
            string fileHeader;
            string fileModDate;
            string lpFolder = null;
            var fileIndex = 0;
            string lpSourceFile;
            string vpSourceFile;
            GZippedFile gzf = null;
            FileStream fsOut = null;
            FileStream fsIn = null;

            if (files != null && files.Length > 0)
                try
                {
                    result.Files = new GZippedFile[files.Length];

                    // open the temp file for writing
                    fsOut = new FileStream(lpTempFile, FileMode.Create, FileAccess.Write, FileShare.None);

                    foreach (var fi in files)
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

                            fileHeader = fileIndex + "," + vpSourceFile + "," + fileModDate + "," + buffer.Length
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

                        if (fsOut != null) result.TempFileSize = fsOut.Length;
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
                    }
                }

            result.FileCount = fileIndex;
        }

        private static void CreateZipFile(string lpSourceFile, string lpZipFile, GZipResult result)
        {
            byte[] buffer;
            var count = 0;
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

            var ratio = (tmp - zip) / tmp;
            var pcnt = ratio * hundred;

            return (int) pcnt;
        }

        private static string GetFolder(string filename)
        {
            var vpFolder = filename;
            var index = filename.LastIndexOf("/");
            if (index != -1) vpFolder = filename.Substring(0, index + 1);
            return vpFolder;
        }

        private static string ReadLine(FileStream fs)
        {
            var line = string.Empty;

            const int bufferSize = 4096;
            var buffer = new byte[bufferSize];
            byte b = 0;
            byte lf = 10;
            var i = 0;

            while (b != lf)
            {
                b = (byte) fs.ReadByte();
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
            var count = 0;

            try
            {
                fsIn = new FileStream(lpZipFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                result.ZipFileSize = fsIn.Length;

                fsOut = new FileStream(lpTempFile, FileMode.Create, FileAccess.Write, FileShare.None);
                gzip = new GZipStream(fsIn, CompressionMode.Decompress, true);
                while (true)
                {
                    count = gzip.Read(buffer, 0, bufferSize);
                    if (count != 0) fsOut.Write(buffer, 0, count);
                    if (count != bufferSize) break;
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
                }

                if (fsOut != null)
                {
                    fsOut.Close();
                }

                if (fsIn != null)
                {
                    fsIn.Close();
                }
            }

            fsTemp = new FileStream(lpTempFile, FileMode.Open, FileAccess.Read, FileShare.None);
            result.TempFileSize = fsTemp.Length;
            return fsTemp;
        }

        private static void WriteFile(FileStream fs, int fileLength, string lpFile)
        {
            FileStream fsFile = null;

            try
            {
                var lpFolder = GetFolder(lpFile);
                if (!string.IsNullOrEmpty(lpFolder) && lpFolder != lpFile && !Directory.Exists(lpFolder))
                    Directory.CreateDirectory(lpFolder);

                var buffer = new byte[fileLength];
                var count = fs.Read(buffer, 0, fileLength);
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
                }
            }
        }

        #endregion
    }
}