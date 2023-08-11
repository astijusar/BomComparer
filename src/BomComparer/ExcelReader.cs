using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BomComparer.Attributes;
using BomComparer.Models;
using NPOI.SS.UserModel;
using WorkbookFactory = BomComparer.Factories.WorkbookFactory;

namespace BomComparer
{
    public class ExcelReader
    {
        public static List<T> ReadData<T>(string filePath) where T : class, new()
        {
            var data = new List<T>();

            var workbook = WorkbookFactory.CreateWorkbook(filePath);

            var sheet = workbook.GetSheetAt(0);
            var headerRow = sheet.GetRow(0);
            var propertyIndexes = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            for (var i = 0; i < headerRow.LastCellNum; i++)
            {
                var cellValue = headerRow.GetCell(i)?.StringCellValue?.Trim();
                if (cellValue != null) propertyIndexes[cellValue] = i;
            }

            for (var i = 1; i <= sheet.LastRowNum; i++)
            {
                var dataRow = sheet.GetRow(i);

                if (dataRow == null) continue;

                var dataRowModel = new T();
                var properties = typeof(BomDataRow).GetProperties();

                foreach (var property in properties)
                {
                    var columnNameAttribute = property.GetCustomAttribute<ColumnNameAttribute>();
                    var columnName = columnNameAttribute?.Name != null ? columnNameAttribute.Name : property.Name;

                    if (!propertyIndexes.TryGetValue(columnName, out var columnIndex)) continue;

                    var propertyType = property.PropertyType;

                    if (propertyType == typeof(string))
                        property.SetValue(dataRowModel, dataRow.GetCell(columnIndex)?.StringCellValue?.Trim());
                    else if (propertyType == typeof(int))
                        property.SetValue(dataRowModel, (int)dataRow.GetCell(columnIndex).NumericCellValue);
                    else if (propertyType == typeof(List<string>))
                        property.SetValue(dataRowModel, dataRow.GetCell(columnIndex).StringCellValue.Split(", ").ToList());
                    else
                        throw new NotSupportedException($"Type {propertyType.Name} is not supported.");
                }

                data.Add(dataRowModel);
            }

            return data;
        }
    }
}
