using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BomComparer.Attributes;
using BomComparer.Models;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using WorkbookFactory = BomComparer.Factories.WorkbookFactory;

namespace BomComparer
{
    public class ExcelReader
    {
        public BomFile ReadData(string filePath)
        {
            var file = new BomFile
            {
                Name = Path.GetFileName(filePath)
            };

            var workbook = WorkbookFactory.CreateWorkbook(filePath);

            var sheet = workbook.GetSheetAt(0);
            var headerRow = sheet.GetRow(0);

            if (headerRow == null) return file;

            var headerColumns = GetHeaderColumns(headerRow);

            for (var i = 1; i <= sheet.LastRowNum; i++)
            {
                var dataRow = sheet.GetRow(i);

                if (dataRow == null) continue;

                var rowDataModel = GetRowData(dataRow, headerColumns);
                file.Data.Add(rowDataModel);
            }

            return file;
        }

        private Dictionary<string, int> GetHeaderColumns(IRow headerRow)
        {
            var headerColumns = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            for (var i = 0; i < headerRow.LastCellNum; i++)
            {
                var cellValue = headerRow.GetCell(i)?.StringCellValue?.Trim();
                if (cellValue != null) headerColumns[cellValue] = i;
            }

            return headerColumns;
        }

        private BomDataRow GetRowData(IRow dataRow, Dictionary<string, int> headerColumns)
        {
            var rowDataModel = new BomDataRow();
            var properties = typeof(BomDataRow).GetProperties();

            foreach (var property in properties)
            {
                var columnNameAttribute = property.GetCustomAttribute<ColumnNameAttribute>();
                var columnName = columnNameAttribute?.Name ?? property.Name;

                if (!headerColumns.TryGetValue(columnName, out var columnIndex)) continue;

                var propertyType = property.PropertyType;
                var cell = dataRow.GetCell(columnIndex);

                object value = propertyType switch
                {
                    { } t when t == typeof(string) => cell.StringCellValue.Trim(),
                    { } t when t == typeof(int) => (int)cell.NumericCellValue,
                    { } t when t == typeof(List<string>) => cell.StringCellValue.Split(", ").ToList(),
                    _ => throw new NotSupportedException($"Type {propertyType.Name} is not supported.")
                };

                property.SetValue(rowDataModel, value);
            }

            return rowDataModel;
        }
    }
}
