using System.Reflection;
using BomComparer.Attributes;
using BomComparer.Enums;
using BomComparer.Exceptions;
using BomComparer.Models;
using BomWriter.Styles;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

namespace BomWriter.ExcelWriter
{
    public class NpoiWriter : IExcelWriter
    {
        private XSSFWorkbook? _workbook;

        public void Write(string path, BomComparisonResult data)
        {
            _workbook = new XSSFWorkbook();
            var sheet = _workbook.CreateSheet("Sheet1");

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
                    CreateModifiedBomRows(sheet, rowData, data.SourceFileName, data.TargetFileName, properties);
                else
                    CreateBomRow(sheet, rowData, data.SourceFileName, data.TargetFileName, properties, rowData.Status);
            }

            for (var i = 0; i < properties.Count; i++)
                sheet.AutoSizeColumn(i + 1);

            using var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
            _workbook.Write(fileStream);
        }

        private void CreateHeader(ISheet sheet, List<PropertyInfo> properties)
        {
            var headerRow = sheet.CreateRow(0);
            var cellStyle = CellStyleProvider.HeaderCellStyle(sheet.Workbook);

            var dataSourceCell = headerRow.CreateCell(0);
            dataSourceCell.SetCellValue("Data Source");
            dataSourceCell.CellStyle = cellStyle;

            for (var i = 0; i < properties.Count; i++)
            {
                var columnNameAttribute = properties[i].GetCustomAttribute<ColumnNameAttribute>();

                if (columnNameAttribute == null)
                    throw new MissingColumnNameException(properties[i].Name);

                var cell = headerRow.CreateCell(i + 1);
                cell.SetCellValue(columnNameAttribute.Name);
                cell.CellStyle = cellStyle;
            }

            sheet.SetAutoFilter(new CellRangeAddress(0, 0, 0, properties.Count));
        }

        private void CreateModifiedBomRows(ISheet sheet, BomComparisonResultEntry rowData, string sourceName,
            string targetName, List<PropertyInfo> properties)
        {
            var sourceRow = sheet.CreateRow(sheet.LastRowNum + 1);
            var targetRow = sheet.CreateRow(sheet.LastRowNum + 1);

            sourceRow.CreateCell(0).SetCellValue(sourceName);
            targetRow.CreateCell(0).SetCellValue(targetName);

            for (var i = 0; i < properties.Count; i++)
            {
                var value = properties[i].GetValue(rowData);
                var sourceCell = sourceRow.CreateCell(i + 1);
                var targetCell = targetRow.CreateCell(i + 1);

                switch (value)
                {
                    case ComparedValues<int> intCompVal:
                        sourceCell.SetCellValue(intCompVal.SourceValue);
                        targetCell.SetCellValue(intCompVal.TargetValue);

                        switch (intCompVal.Status)
                        {
                            case ComparisonResult.Added:
                                targetCell.CellStyle = CellStyleProvider.AddedCellStyle(sheet.Workbook);
                                break;
                            case ComparisonResult.Removed:
                                sourceCell.CellStyle = CellStyleProvider.RemovedCellStyle(sheet.Workbook);
                                break;
                            case ComparisonResult.Modified:
                                targetCell.CellStyle = CellStyleProvider.AddedCellStyle(sheet.Workbook);
                                sourceCell.CellStyle = CellStyleProvider.RemovedCellStyle(sheet.Workbook);
                                break;
                        }
                        break;
                    case ComparedValues<string> strCompVal:
                        sourceCell.SetCellValue(strCompVal.SourceValue);
                        targetCell.SetCellValue(strCompVal.TargetValue);

                        switch (strCompVal.Status)
                        {
                            case ComparisonResult.Added:
                                targetCell.CellStyle = CellStyleProvider.AddedCellStyle(sheet.Workbook);
                                break;
                            case ComparisonResult.Removed:
                                sourceCell.CellStyle = CellStyleProvider.RemovedCellStyle(sheet.Workbook);
                                break;
                            case ComparisonResult.Modified:
                                targetCell.CellStyle = CellStyleProvider.AddedCellStyle(sheet.Workbook);
                                sourceCell.CellStyle = CellStyleProvider.RemovedCellStyle(sheet.Workbook);
                                break;
                        }
                        break;
                    case ComparisonResult status:
                        sourceCell.SetCellValue(status.ToString());
                        targetCell.SetCellValue(status.ToString());
                        break;
                    case string str:
                        sourceCell.SetCellValue(str);
                        targetCell.SetCellValue(str);
                        break;
                    case List<DesignatorComparisonResultEntry> list:
                        var designatorString = ConstructDesignatorString(sheet.Workbook, list);
                        sourceCell.SetCellValue(designatorString.Item1);
                        targetCell.SetCellValue(designatorString.Item2);
                        break;
                }
            }
        }

        private void CreateBomRow(ISheet sheet, BomComparisonResultEntry rowData, string sourceName,
            string targetName, List<PropertyInfo> properties, ComparisonResult compResult)
        {
            var cellStyle = compResult switch
            {
                ComparisonResult.Added => CellStyleProvider.AddedCellStyle(sheet.Workbook),
                ComparisonResult.Removed => CellStyleProvider.RemovedCellStyle(sheet.Workbook),
                _ => CellStyleProvider.DefaultCellStyle(sheet.Workbook)
            };

            var row = sheet.CreateRow(sheet.LastRowNum + 1);

            var dataSourceCell = row.CreateCell(0);
            dataSourceCell.CellStyle = cellStyle;

            for (var i = 0; i < properties.Count; i++)
            {
                var value = properties[i].GetValue(rowData);
                var cell = row.CreateCell(i + 1);
                cell.CellStyle = cellStyle;

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
                        dataSourceCell.SetCellValue(status == ComparisonResult.Removed ? sourceName : targetName);
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

        private (XSSFRichTextString, XSSFRichTextString) ConstructDesignatorString(IWorkbook workbook, 
            List<DesignatorComparisonResultEntry> designators)
        {
            var source = new XSSFRichTextString();
            var target = new XSSFRichTextString();

            var addedFont = (XSSFFont)FontStyleProvider.AddedFontStyle(workbook);
            var removedFont = (XSSFFont)FontStyleProvider.RemovedFontStyle(workbook);

            foreach (var designator in designators)
            {
                switch (designator.Status)
                {
                    case DesignatorComparisonResult.Added:
                        source.Append(
                            designator != designators.Last() ? $"{designator.Value}, " : $"{designator.Value}",
                            addedFont);
                        break;
                    case DesignatorComparisonResult.Removed:
                        source.Append(
                            designator != designators.Last() ? $"{designator.Value}, " : $"{designator.Value}",
                            removedFont);
                        break;
                    case DesignatorComparisonResult.Unchanged:
                        source.Append(
                            designator != designators.Last() 
                                ? $"{designator.Value}, " 
                                : $"{designator.Value}");
                        target.Append(designator != designators.Last() 
                            ? $"{designator.Value}, " 
                            : $"{designator.Value}");
                        break;
                }
            }

            return (source, target);
        }
    }
}
