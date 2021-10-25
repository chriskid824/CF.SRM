using Convience.Filestorage.Abstraction;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Convience.Service.SRM
{
    public class Utility
    {
        public class UploadFile
        {
            public FileInfo GetFileInfoAsync(string path)
            {
                var fileInfo = new FileInfo(path);

                if (fileInfo.Exists)
                {
                    return fileInfo;
                }
                return null;
            }
            public string CreateFileFromStreamAsync(string path, Stream inputStream, bool overwrite = false)
            {
                if (!overwrite && System.IO.File.Exists(path))
                {
                    throw new FileStoreException($"Cannot create file '{path}' because it already exists.");
                }

                if (Directory.Exists(path))
                {
                    throw new FileStoreException($"Cannot create file '{path}' because it already exists as a directory.");
                }

                // Create directory path if it doesn't exist.
                var physicalDirectoryPath = Path.GetDirectoryName(path);
                Directory.CreateDirectory(physicalDirectoryPath);

                var fileInfo = new FileInfo(path);
                using (var outputStream = fileInfo.Create())
                {
                    inputStream.CopyTo(outputStream);
                }
                return path;
            }
        }

        public DataTable ReadExcel(string path, int sheetNum)
        {
            IWorkbook workbook;
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                workbook = new XSSFWorkbook(stream);
            }

            ISheet sheet = workbook.GetSheetAt(sheetNum); // zero-based index of your target sheet
            DataTable dt = new DataTable(sheet.SheetName);

            // write header row
            IRow headerRow = sheet.GetRow(0);
            for (int i = 0; i < headerRow.Cells.Count; i++)
            {
                headerRow.GetCell(i).SetCellType(CellType.String);
                dt.Columns.Add(headerRow.GetCell(i).StringCellValue);
            }
            Dictionary<string, int> dtHeader = new Dictionary<string, int>();
            foreach (DataColumn col in dt.Columns)
            {
                string header = col.ColumnName;
                dtHeader.Add(header, dt.Columns.IndexOf(header));
            }

            int rowIndex = 0;
            foreach (IRow row in sheet)
            {
                if (rowIndex == 0) { rowIndex++; continue; }
                DataFormatter formatter = new DataFormatter();
                DataRow dataRow = dt.NewRow();
                foreach (var h in dtHeader)
                {
                    if (row.GetCell(h.Value) != null)
                    {
                        row.GetCell(h.Value).SetCellType(CellType.String);
                        dataRow[h.Key] = row.GetCell(h.Value).StringCellValue;
                    }
                }
                //dataRow["IsExists"] = _context.SrmVendors.Any(r => r.SrmVendor1.Equals(dataRow["供應商編號"]) && user.Werks.Contains(r.Ekorg.Value));//2021/10/19問過LEO 同名字不同廠可能存在多筆
                dt.Rows.Add(dataRow);
                rowIndex++;
            }
            return dt;
        }
    }
}
