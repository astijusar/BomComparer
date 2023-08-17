using BomComparer.Models;

namespace BomComparer.ExcelReaders
{
    public interface IExcelReader
    {
        public BomFile ReadData(string filePath);
    }
}
