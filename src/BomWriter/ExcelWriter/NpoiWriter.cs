using System.Reflection;
using BomComparer.Attributes;
using BomComparer.Enums;
using BomComparer.Models;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

namespace BomWriter.ExcelWriter
{
    public class NpoiWriter : IExcelWriter
    {
        public void Write(string path, BomComparisonResult data)
        {
            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet("Sheet1");

            var properties = typeof(BomComparisonResultEntry).GetProperties()
                .Select(prop => new
                {
                    Property = prop,
                    Attribute = prop.GetCustomAttribute<ColumnOrderAttribute>()
                })
                .OrderBy(x => x.Attribute?.Order ?? int.MaxValue)
                .Select(x => x.Property)
                .ToList();

            CreateHeader(sheet, properties);

            for (var i = 1; i <= data.ResultEntries.Count; i++)
            {
                var rowData = data.ResultEntries[i - 1];

                if (rowData.Status == ComparisonResult.Modified)
                    CreateModifiedBomRows(sheet, rowData, properties);
                else
                    CreateBomRow(sheet, rowData, properties);
            }

            for (var i = 0; i < properties.Count; i++)
                sheet.AutoSizeColumn(i);

            using var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
            workbook.Write(fileStream);
        }

        private void CreateHeader(ISheet sheet, List<PropertyInfo> properties)
        {
            var headerRow = sheet.CreateRow(0);

            for (var i = 0; i < properties.Count; i++)
            {
                headerRow.CreateCell(i)
                    .SetCellValue(properties[i].Name);
            }

            sheet.SetAutoFilter(new CellRangeAddress(0, 0, 0, properties.Count - 1));
        }

        private void CreateModifiedBomRows(ISheet sheet, BomComparisonResultEntry rowData,
            List<PropertyInfo> properties)
        {

        }

        private void CreateBomRow(ISheet sheet, BomComparisonResultEntry rowData, List<PropertyInfo> properties)
        {
            var row = sheet.CreateRow(sheet.LastRowNum + 1);

            for (var i = 0; i < properties.Count; i++)
            {
                var value = properties[i].GetValue(rowData);
                var cell = row.CreateCell(i);

                switch (value)
                {
                    case ComparedValues<int> intCompVal:
                        cell.SetCellValue(intCompVal.Status == ComparisonResult.Added
                            ? intCompVal.TargetValue
                            : intCompVal.SourceValue);
                        break;
                    case ComparedValues<string> strCompVal:
                        cell.SetCellValue(strCompVal.Status == ComparisonResult.Added
                            ? strCompVal.TargetValue
                            : strCompVal.SourceValue);
                        break;
                    case ComparisonResult status:
                        cell.SetCellValue(status.ToString());
                        break;
                    case string str:
                        cell.SetCellValue(str);
                        break;
                    case List<DesignatorComparisonResultEntry> list:
                        cell.SetCellValue(string.Join(", ", list.Select(l => l.Value)));
                        break;
                }
            }
        }
    }
}
