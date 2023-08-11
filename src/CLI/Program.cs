using BomComparer;
using BomComparer.Models;

namespace CLI
{
    public class Program
    {
        static void Main()
        {
            const string filePath = "BOM_A.xls";

            var data = ExcelReader.ReadData<BomDataRow>(filePath);

            Console.ReadKey();
        }
    }
}