using System;
using System.Collections.Generic;
using NPOI.HPSF;

namespace CnSharp.Utilities.Excel
{
    public class ExcelOptions
    {
        private string _sheetNamePattern;
        
        //public string Password { get; set; }

        public string SheetNamePattern
        {
            get { return _sheetNamePattern; }
            set
            {
                if(!string.IsNullOrEmpty(value) && !value.Contains("{0}"))
                    throw new ArgumentException("pattern requires '{0}' placeholder");
                _sheetNamePattern = value;
            }
        }

        public IList<string> SheetNames { get;  set; }

        public string GetSheetName(int i)
        {
            if (SheetNames != null && SheetNames.Count > i)
                return SheetNames[i];
            if (!string.IsNullOrEmpty(_sheetNamePattern))
            {
                return string.Format(_sheetNamePattern, i + 1);
            }
            return null;
        }

        //public DocumentSummaryInformation DocumentSummaryInformation { get; set; }

        public ExcelOptions()
        {
            SheetNames = new List<string>();
        }
    }
}
