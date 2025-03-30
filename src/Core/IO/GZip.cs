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
                catch (Exception ignored)
                {
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

            var files = new ArrayList();

            // extract the files from the temp file

            using (var fsTemp = UnzipToTempFile(zipFile, lpTempFile, result))
            {
                while (fsTemp.Position != fsTemp.Length)
                {
                    string line = null;
                    while (string.IsNullOrEmpty(line) && fsTemp.Position != fsTemp.Length) line = ReadLine(fsTemp);

                    if (!string.IsNullOrEmpty(line))
                    {
                        var gzf = GZippedFile.GetGZippedFile(line);
                        if (gzf != null && gzf.Length > 0)
                        {
                            files.Add(gzf);
                            var lpFilePath = destFolder + gzf.RelativePath;
                            gzf.LocalPath = lpFilePath;
                            WriteFile(fsTemp, gzf.Length, lpFilePath);
                            gzf.Restored = true;
                        }
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
                catch (Exception ignored)
                {
                }
                result.FileCount = files.Count;
                files.CopyTo(result.Files);
                return result;
            }
        }

        #endregion

        #region Methods

        private static void CreateTempFile(FileInfo[] files, string lpBaseFolder, string lpTempFile, GZipResult result)
        {
            if (files == null || files.Length <= 0) return;
            result.Files = new GZippedFile[files.Length];
            // open the temp file for writing
            using (var fsOut = new FileStream(lpTempFile, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                var fileIndex = 0;
                foreach (var fi in files)
                {
                    var gzf = new GZippedFile();
                    gzf.Index = fileIndex;

                    // read the source file, get its virtual path within the source folder
                    var lpSourceFile = fi.FullName;
                    gzf.LocalPath = lpSourceFile;
                    var vpSourceFile = lpSourceFile.Replace(lpBaseFolder, string.Empty);
                    vpSourceFile = vpSourceFile.Replace("\\", "/");
                    gzf.RelativePath = vpSourceFile;

                    using (var fsIn = new FileStream(lpSourceFile, FileMode.Open, FileAccess.Read,
                               FileShare.Read))
                    {
                        var buffer = new byte[fsIn.Length];
                        fsIn.Read(buffer, 0, buffer.Length);
                        var fileModDate = fi.LastWriteTimeUtc.ToString();
                        gzf.ModifiedDate = fi.LastWriteTimeUtc;
                        gzf.Length = buffer.Length;

                        var fileHeader = fileIndex + "," + vpSourceFile + "," + fileModDate + "," +
                                         buffer.Length
                                         + "\n";
                        var header = Encoding.Default.GetBytes(fileHeader);

                        fsOut.Write(header, 0, header.Length);
                        fsOut.Write(buffer, 0, buffer.Length);
                        fsOut.WriteByte(10); // linefeed

                        gzf.AddedToTempFile = true;

                        // update the result object
                        result.Files[fileIndex] = gzf;
                    }
                    
                    // increment the fileIndex
                    fileIndex++;

                    result.TempFileSize = fsOut.Length;
                }
                
                result.FileCount = fileIndex;
            }
        }

        private static void CreateZipFile(string lpSourceFile, string lpZipFile, GZipResult result)
        {
            // compress the file into the zip file

            using (var fsOut = new FileStream(lpZipFile, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (var gzip = new GZipStream(fsOut, CompressionMode.Compress, true))
                {
                    using (var fsIn = new FileStream(lpSourceFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        var buffer = new byte[fsIn.Length];
                        fsIn.Read(buffer, 0, buffer.Length);

                        // compress to the zip file
                        gzip.Write(buffer, 0, buffer.Length);

                        result.ZipFileSize = fsOut.Length;
                        result.CompressionPercent = GetCompressionPercent(result.TempFileSize, result.ZipFileSize);
                    }
                }
            }
        }

        private static int GetCompressionPercent(long tempLen, long zipLen)
        {
            double tmp = tempLen;
            double zip = zipLen;
            const double hundred = 100;
            var ratio = (tmp - zip) / tmp;
            var percent = ratio * hundred;
            return (int) percent;
        }


        private static string ReadLine(FileStream fs)
        {
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

            return Encoding.Default.GetString(buffer, 0, i - 1);
        }

        private static FileStream UnzipToTempFile(string lpZipFile, string lpTempFile, GZipResult result)
        {
            using (var fsIn = new FileStream(lpZipFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                result.ZipFileSize = fsIn.Length;

                using (var fsOut = new FileStream(lpTempFile, FileMode.Create, FileAccess.Write, FileShare.None))
                using (var gzip = new GZipStream(fsIn, CompressionMode.Decompress, true))
                {
                    while (true)
                    {
                        const int bufferSize = 4096;
                        var buffer = new byte[bufferSize];
                        var count = gzip.Read(buffer, 0, bufferSize);
                        if (count != 0) fsOut.Write(buffer, 0, count);
                        if (count != bufferSize) break;
                    }
                }

                var fsTemp = new FileStream(lpTempFile, FileMode.Open, FileAccess.Read, FileShare.None);
                result.TempFileSize = fsTemp.Length;
                return fsTemp;
            }
        }

        private static void WriteFile(FileStream fs, int fileLength, string lpFile)
        {
            var lpFolder = Path.GetDirectoryName(lpFile);
            if (!string.IsNullOrEmpty(lpFolder) && lpFolder != lpFile && !Directory.Exists(lpFolder))
                Directory.CreateDirectory(lpFolder);

            var buffer = new byte[fileLength];
            var count = fs.Read(buffer, 0, fileLength);
            using (var fsFile = new FileStream(lpFile, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                fsFile.Write(buffer, 0, buffer.Length);
                fsFile.Write(buffer, 0, count);
                fsFile.Flush();
            }
        }

        #endregion
    }
}