using System.Reflection;
using BomComparer.Attributes;
using BomComparer.Exceptions;
using BomComparer.Models;
using NPOI.SS.UserModel;
using WorkbookFactory = BomComparer.Factories.WorkbookFactory;

namespace BomComparer.ExcelReaders
{
    public class NpoiReader : IExcelReader
    {
        public BomFile ReadData(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

            var file = new BomFile
            {
                Name = Path.GetFileName(filePath)
            };

            var workbook = WorkbookFactory.CreateWorkbook(filePath);

            var sheet = workbook.GetSheetAt(0);
            var headerRow = sheet.GetRow(0);

            if (headerRow == null)
                throw new InvalidFileFormatException($"Header in file '{file.Name}' needs to be on the first row.");

            var headerColumns = GetHeaderColumns(headerRow);
            var columnPropertyMap = GetColumnPropertyMap();

            for (var i = 1; i <= sheet.LastRowNum; i++)
            {
                var dataRow = sheet.GetRow(i);

                if (dataRow == null) continue;

                var rowDataModel = GetRowData(dataRow, headerColumns, columnPropertyMap);
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

                if (cellValue == null)
                    throw new InvalidFileFormatException($"Empty cell at index '{i}' in the header.");
                    
                headerColumns[cellValue] = i;
            }

            return headerColumns;
        }

        private Dictionary<string, PropertyInfo> GetColumnPropertyMap()
        {
            var columnPropertyMap = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);
            var properties = typeof(BomDataRow).GetProperties();

            foreach (var property in properties)
            {
                var columnNameAttribute = property.GetCustomAttribute<ColumnNameAttribute>();

                if (columnNameAttribute == null) 
                    throw new MissingColumnNameException(property.Name);

                var columnName = columnNameAttribute.Name;
                columnPropertyMap[columnName] = property;
            }

            return columnPropertyMap;
        }

        private BomDataRow GetRowData(IRow dataRow, Dictionary<string, int> headerColumns,
            Dictionary<string, PropertyInfo> columnPropertyMap)
        {
            var rowDataModel = new BomDataRow();

            foreach (var (columnName, columnIndex) in headerColumns)
            {
                if (!columnPropertyMap.TryGetValue(columnName, out var property)) continue;

                var propertyType = property.PropertyType;
                var cell = dataRow.GetCell(columnIndex);

                object value = propertyType switch
                {
                    { } t when t == typeof(string) => cell.StringCellValue.Trim(),
                    { } t when t == typeof(int) => (int)cell.NumericCellValue,
                    { } t when t == typeof(List<string>) => cell.StringCellValue
                        .Split(",", StringSplitOptions.TrimEntries).ToList(),
                    _ => throw new NotSupportedException($"Type {propertyType.Name} is not supported.")
                };

                property.SetValue(rowDataModel, value);
            }

            return rowDataModel;
        }
    }
}
