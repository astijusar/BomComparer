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
        private readonly ExcelReader _excelReader;

        public ExcelReaderTests()
        {
            _excelReader = new ExcelReader();
        }

        [Fact]
        public void ReadData_ValidFile_ReturnsData()
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
    }
}
