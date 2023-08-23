using System.IO;
using System.Reflection;
using BomComparer.Attributes;
using BomComparer.Comparer;
using BomComparer.ExcelReaders;
using BomComparer.Models;
using BomWriter.ExcelWriter;
using FluentAssertions;
using NPOI.SS.UserModel;
using WorkbookFactory = BomComparer.Factories.WorkbookFactory;

namespace BomWriter.Tests
{
    public class NpoiWriterTests
    {
        [Fact]
        public void Write_ShouldCreateCorrectExcelFile()
        {
            var sourceFile = "TestData/source.xlsx";
            var targetFile = "TestData/target.xlsx";

            var reader = new NpoiReader();
            var sourceData = reader.ReadData(sourceFile);
            var targetData = reader.ReadData(targetFile);

            var comparer = new BomCompare();
            var results = comparer.Compare(sourceData, targetData);

            var writer = new NpoiWriter();
            var resultPath = Path.Combine(Directory.GetCurrentDirectory(),
                Path.GetFileName(sourceFile) + Path.GetFileName(targetFile));

            writer.Write(resultPath, results);

            var workbook = WorkbookFactory.CreateWorkbook(resultPath);
            var sheet = workbook.GetSheetAt(0);
            var header = sheet.GetRow(0);

            var properties = typeof(BomComparisonResultEntry).GetProperties()
                .Select(prop => new
                {
                    Property = prop,
                    Attribute = prop.GetCustomAttribute<ColumnOrderAttribute>()
                })
                .OrderBy(x => x.Attribute?.Order ?? int.MaxValue)
                .Select(x => x.Property)
                .ToList();

            header.GetCell(0).StringCellValue.Should().Be("Data Source");
            for (var i = 0; i < properties.Count; i++)
            {
                var columnNameAttribute = properties[i].GetCustomAttribute<ColumnNameAttribute>();
                header.GetCell(i + 1).StringCellValue.Should().Be(columnNameAttribute!.Name);
            }

            var row1 = sheet.GetRow(1);
            row1.GetCell(0).StringCellValue.Should().Be("target.xlsx");
            row1.GetCell(1).StringCellValue.Should().Be("Unchanged");
            row1.GetCell(4).StringCellValue.Should().Be("A1200");

            var row2 = sheet.GetRow(2);
            row2.GetCell(0).StringCellValue.Should().Be("source.xlsx");
            row2.GetCell(1).StringCellValue.Should().Be("Removed");
            IsCorrectStyle(row2.GetCell(1), IndexedColors.Red.Index, false, true);

            var row3 = sheet.GetRow(3);
            row3.GetCell(0).StringCellValue.Should().Be("source.xlsx");
            IsCorrectStyle(row3.GetCell(9), IndexedColors.Red.Index, true, true);
            IsCorrectDistributorStyle(row3.GetCell(4), 1, IndexedColors.Red.Index, true, true);

            var row4 = sheet.GetRow(4);
            row4.GetCell(0).StringCellValue.Should().Be("target.xlsx");
            IsCorrectStyle(row4.GetCell(9), IndexedColors.Green.Index, true, false);

            var row5 = sheet.GetRow(5);
            row5.GetCell(0).StringCellValue.Should().Be("target.xlsx");
            row5.GetCell(1).StringCellValue.Should().Be("Added");
            IsCorrectStyle(row5.GetCell(1), IndexedColors.Green.Index, false, false);

            File.Delete(resultPath);
        }

        [Fact]
        public void BomComparisonResultEntry_AllPropertiesShouldHaveColumnNameAttribute()
        {
            var properties = typeof(BomComparisonResultEntry).GetProperties();

            foreach (var property in properties)
            {
                var columnNameAttribute = property.GetCustomAttribute<ColumnNameAttribute>();

                columnNameAttribute.Should().NotBeNull();
            }
        }

        private void IsCorrectDistributorStyle(ICell cell, int index, short color, bool isBold, bool isStrikeout)
        {
            var richText = cell.RichStringCellValue;
            var fontIndex = richText.GetIndexOfFormattingRun(index);
            var font = cell.Row.Sheet.Workbook.GetFontAt((short)fontIndex);

            font.Color.Should().Be(color);
            font.IsBold.Should().Be(isBold);
            font.IsStrikeout.Should().Be(isStrikeout);

        }

        private void IsCorrectStyle(ICell cell, short color, bool isBold, bool isStrikeout)
        {
            var cellStyle = cell.CellStyle;
            var font = cellStyle.GetFont(cell.Row.Sheet.Workbook);

            font.Color.Should().Be(color);
            font.IsBold.Should().Be(isBold);
            font.IsStrikeout.Should().Be(isStrikeout);
        }
    }
}