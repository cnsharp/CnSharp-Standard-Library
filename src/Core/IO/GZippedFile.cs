using System;

namespace CnSharp.IO
{
    public class GZippedFile
    {
        #region Constants and Fields

        public bool AddedToTempFile { get; set; }

        public string Folder { get; set; }

        public int Index { get; set; }

        public int Length { get; set; }

        public string LocalPath { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string RelativePath { get; set; }

        public bool Restored { get; set; }

        #endregion

        #region Public Methods

        public static GZippedFile GetGZippedFile(string filePath)
        {
            GZippedFile gzf = null;

            if (!string.IsNullOrEmpty(filePath))
            {
                // get the file information
                string[] info = filePath.Split(',');
                if (info.Length == 4)
                {
                    gzf = new GZippedFile();
                    gzf.Index = Convert.ToInt32(info[0]);
                    gzf.RelativePath = info[1].Replace("/", "\\");
                    gzf.ModifiedDate = Convert.ToDateTime(info[2]);
                    gzf.Length = Convert.ToInt32(info[3]);
                }
            }

            return gzf;
        }

        #endregion
    }
}