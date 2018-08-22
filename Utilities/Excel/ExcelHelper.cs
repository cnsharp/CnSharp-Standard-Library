using System.Data;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace CnSharp.Utilities.Excel
{
    /// <summary>
    /// excel helper support both 2003 and 2007 version
    /// </summary>
    /// <remarks>
    /// Reference:http://blog.csdn.net/wyii26/article/details/21385905
    /// </remarks>
    public static class ExcelHelper
    {
        public static Stream RenderDataSetToExcel(DataSet ds, ExcelVersion version = ExcelVersion.V2007)
        {
            using (var ms = new MemoryStream())
            {
                foreach (DataTable table in ds.Tables)
                {
                    OuputDataTableToStream(table, ms);
                }
                ms.Flush();
                return ms;
            }
        }

        public static Stream RenderDataTableToExcel(DataTable table, ExcelVersion version = ExcelVersion.V2007)
        {
            using (var ms = new MemoryStream())
            {
                OuputDataTableToStream(table, ms);
                ms.Flush();
                return ms;
            }
        }

        public static void OuputDataSetToStream(DataSet ds, Stream stream, ExcelVersion version = ExcelVersion.V2007)
        {
            var workbook = GetWorkbookInstance(version);
            var maxRowNumber = GetMaxRows(version);

            foreach (DataTable table in ds.Tables)
            {
                OuputDataTableToStream(table, stream, workbook,maxRowNumber);
            }
            workbook.Write(stream);
        }

        public static void OuputDataTableToStream(DataTable table, Stream stream,
            ExcelVersion version = ExcelVersion.V2007)
        {
            var workbook = GetWorkbookInstance(version);
            var maxRowNumber = GetMaxRows(version);
            OuputDataTableToStream(table, stream, workbook, maxRowNumber);
            workbook.Write(stream);
        }
        
        private static void OuputDataTableToStream(DataTable sourceTable, Stream stream,IWorkbook workbook,int maxRowNumber)
        {
           
            var sheetName = sourceTable.TableName;
            if (string.IsNullOrWhiteSpace(sheetName))
                sheetName = "Sheet";
            var sheet = workbook.CreateSheet(sheetName);
            var headerRow = sheet.CreateRow(0);
            foreach (DataColumn column in sourceTable.Columns)
                headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
            var rowIndex = 1;
            var currentSheet = sheet;
        
            foreach (DataRow row in sourceTable.Rows)
            {
                if (rowIndex % maxRowNumber == 0)
                {
                    var sheetNo = rowIndex/maxRowNumber + 1;
                    sheetName = sourceTable.TableName + sheetNo;
                    currentSheet = workbook.CreateSheet(sheetName);
                }
                var dataRow = currentSheet.CreateRow(rowIndex);
                foreach (DataColumn column in sourceTable.Columns)
                    dataRow.CreateCell(column.Ordinal).SetCellValue(row[column].ToString());
                ++rowIndex;
            }
            //列宽自适应，只对英文和数字有效
            for (var i = 0; i <= sourceTable.Columns.Count; ++i)
                sheet.AutoSizeColumn(i);
          
        }

        private static IWorkbook GetWorkbookInstance(ExcelVersion version,FileStream fileStream = null)
        {
            if (version == ExcelVersion.V2007)
                return fileStream == null ? new XSSFWorkbook() : new XSSFWorkbook(fileStream);
            return fileStream == null ? new HSSFWorkbook() : new HSSFWorkbook(fileStream);
        }

        private static int GetMaxRows(ExcelVersion version)
        {
            return version == ExcelVersion.V2007 ? 1048576 : 65536;
        }

        private static ExcelVersion GetExcelVersionByFileName(string fileName)
        {
            var version = ExcelVersion.V2003;
            if (fileName.ToLower().EndsWith(".xlsx"))
                version = ExcelVersion.V2007;
            return version;
        }

        public static void SaveDataSetToExcel(DataSet ds, string fileName)
        {
            var version = GetExcelVersionByFileName(fileName);
            using (var sw = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                foreach (DataTable table in ds.Tables)
                {
                    OuputDataTableToStream(table, sw, version);
                }
            }
        }


        public static void SaveDataTableToExcel(DataTable table, string fileName)
        {
            var version = GetExcelVersionByFileName(fileName);
            using (var sw = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                    OuputDataTableToStream(table, sw, version);
            }
        }

        public static DataSet ImportDataSetFromExcel(string fileName, string sheetName = null)
        {
            var ds = new DataSet();
            using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                var version = GetExcelVersionByFileName(fileName);
                var workbook = GetWorkbookInstance(version, fs);
                for (var sheetIndex = 0; sheetIndex < workbook.NumberOfSheets; sheetIndex++)
                {
                    DataTable table = new DataTable();
                    var sheet = sheetName != null ? workbook.GetSheet(sheetName) : workbook.GetSheetAt(sheetIndex);
                    if (sheet == null)
                    {
                        continue;
                    }
                    IRow firstRow = sheet.GetRow(0);
                    if (firstRow == null)
                        continue;
                    int cellCount = firstRow.LastCellNum;


                    for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                    {
                        DataColumn column = new DataColumn(firstRow.GetCell(i).StringCellValue);
                        table.Columns.Add(column);
                    }
                    var startRow = sheet.FirstRowNum + 1;

                    int rowCount = sheet.LastRowNum;
                    for (int i = startRow; i <= rowCount; ++i)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue;

                        DataRow dataRow = table.NewRow();
                        for (int j = row.FirstCellNum; j < cellCount; ++j)
                        {
                            if (row.GetCell(j) != null)
                                dataRow[j] = row.GetCell(j).ToString();
                        }
                        table.Rows.Add(dataRow);
                    }

                    table.TableName = sheet.SheetName;
                    ds.Tables.Add(table);
                    if (sheetName != null)
                        break;
                }

                return ds;
            }
        }
    }

    public enum ExcelVersion
    {
        V2003,
        V2007
    }
}