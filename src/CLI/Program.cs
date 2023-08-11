using BomComparer;
using BomComparer.Models;

namespace CLI
{
    public class Program
    {
        static void Main()
        {
            const string filePath = "BOM_A.xls";

            var excelReader = new ExcelReader();

            var file = excelReader.ReadData(filePath);

            Console.ReadKey();
        }
    }
}