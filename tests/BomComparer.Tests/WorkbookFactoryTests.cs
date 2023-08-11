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
        [Fact]
        public void CreateWorkbook_UnsupportedFileFormat_ThrowsNotSupportedException()
        {
            var filePath = "TestData/test.txt";

            Assert.Throws<NotSupportedException>(() => WorkbookFactory.CreateWorkbook(filePath));
        }

        [Theory]
        [InlineData(".xls")]
        [InlineData(".xlsx")]
        public void CreateWorkbook_ValidFileFormat_ReturnsWorkbook(string fileExtension)
        {
            var filePath = "TestData/test" + fileExtension;
            
            var result = WorkbookFactory.CreateWorkbook(filePath);

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