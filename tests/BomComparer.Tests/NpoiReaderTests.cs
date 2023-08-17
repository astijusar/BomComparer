using System.Reflection;
using BomComparer.Attributes;
using BomComparer.ExcelReaders;
using BomComparer.Exceptions;
using BomComparer.Models;
using FluentAssertions;

namespace BomComparer.Tests
{
    public class NpoiReaderTests
    {
        private readonly NpoiReader _excelReader = new();
        private const string TestFileDirectory = "TestData";

        [Fact]
        public void ReadData_ValidExcelFile_ReturnsData()
        {
            var filePath = $"{TestFileDirectory}/test.xlsx";
            var expectedResult = new BomFile
            {
                Name = "test.xlsx",
                Data = new List<BomDataRow>
                {
                    new()
                    {
                        Quantity = 1,
                        PartNumber = "100",
                        Designators = new List<string> { "A100" },
                        Value = "value",
                        Smd = "Yes",
                        Description = "desc",
                        Manufacturer = "test",
                        ManufacturerPartNumber = "100",
                        Distributor = "",
                        DistributorPartNumber = ""
                    }
                }
            };


            var result = _excelReader.ReadData(filePath);

            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void ReadData_InvalidExcelFile_ThrowsNotSupportedException()
        {
            var filePath = $"{TestFileDirectory}/test.txt";

            _excelReader.Invoking(e => e.ReadData(filePath))
                .Should()
                .Throw<NotSupportedException>();
        }

        [Fact]
        public void ReadData_EmptyFile_ThrowsInvalidFileFormatException()
        {
            var filePath = $"{TestFileDirectory}/empty.xls";

            _excelReader.Invoking(e => e.ReadData(filePath))
                .Should()
                .Throw<InvalidFileFormatException>();
        }

        [Fact]
        public void ReadData_HeaderNotInFirstRow_ThrowsInvalidFileFormatException()
        {
            var filePath = $"{TestFileDirectory}/wrong_header_location.xlsx";

            _excelReader.Invoking(e => e.ReadData(filePath))
                .Should()
                .Throw<InvalidFileFormatException>()
                .WithMessage($"Header in file '{Path.GetFileName(filePath)}' needs to be on the first row.");
        }

        [Fact]
        public void ReadData_EmptyHeaderCell_ThrowsInvalidFileFormatException()
        {
            var filePath = $"{TestFileDirectory}/empty_header_cell.xlsx";

            _excelReader.Invoking(e => e.ReadData(filePath))
                .Should()
                .Throw<InvalidFileFormatException>()
                .WithMessage("Empty cell at index '2' in the header.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("   ")]
        public void ReadData_FilePathIsNullOrWhitespace_ThrowsArgumentException(string filePath)
        {
            _excelReader.Invoking(e => e.ReadData(filePath))
                .Should()
                .Throw<ArgumentException>();
        }

        [Fact]
        public void BomDataRow_AllPropertiesShouldHaveColumnNameAttribute()
        {
            var properties = typeof(BomDataRow).GetProperties();

            foreach (var property in properties)
            {
                var columnNameAttribute = property.GetCustomAttribute<ColumnNameAttribute>();

                columnNameAttribute.Should().NotBeNull();
            }
        }
    }
}
