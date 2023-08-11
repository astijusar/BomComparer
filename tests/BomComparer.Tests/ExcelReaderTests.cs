using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BomComparer.Models;
using FluentAssertions;
using Moq;
using NPOI.SS.UserModel;
using WorkbookFactory = BomComparer.Factories.WorkbookFactory;

namespace BomComparer.Tests
{
    public class ExcelReaderTests
    {
        private readonly ExcelReader _excelReader = new();

        [Fact]
        public void ReadData_ValidExcelFile_ReturnsData()
        {
            var filePath = "TestData/test.xlsx";
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
                        SMD = "Yes",
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
            var filePath = "TestData/test.txt";

            _excelReader.Invoking(e => e.ReadData(filePath))
                .Should()
                .Throw<NotSupportedException>();
        }

        [Fact]
        public void ReadData_EmptyFile_ReturnsEmptyList()
        {
            var filePath = "TestData/test.xls";
            var expectedResult = new BomFile
            {
                Name = "test.xls",
                Data = new List<BomDataRow>()
            };

            var result = _excelReader.ReadData(filePath);

            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
