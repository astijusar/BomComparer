using FluentAssertions;
using Moq;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using WorkbookFactory = BomComparer.Factories.WorkbookFactory;

namespace BomComparer.Tests
{
    public class WorkbookFactoryTests
    {
        private const string TestFileDirectory = "TestData";

        [Fact]
        public void CreateWorkbook_UnsupportedFileFormat_ThrowsNotSupportedException()
        {
            var filePath = $"{TestFileDirectory}/test.txt";

            Assert.Throws<NotSupportedException>(() => WorkbookFactory.CreateWorkbook(filePath));
        }

        [Theory]
        [InlineData(TestFileDirectory + "/empty.xls")]
        [InlineData(TestFileDirectory + "/test.xlsx")]
        public void CreateWorkbook_ValidFileFormat_ReturnsWorkbook(string filePath)
        {
            var result = WorkbookFactory.CreateWorkbook(filePath);

            var fileExtension = Path.GetExtension(filePath);

            if (fileExtension.Equals(".xls", StringComparison.OrdinalIgnoreCase))
            {
                result.Should().BeOfType<HSSFWorkbook>();
            }
            else if (fileExtension.Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                result.Should().BeOfType<XSSFWorkbook>();
            }
        }
    }
}