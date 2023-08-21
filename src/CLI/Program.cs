using BomComparer.ExcelReaders;
using BomWriter.ExcelWriter;

namespace CLI
{
    public class Program
    {
        static void Main()
        {
            const string sourceFilePath = "BOM_A.xls";
            const string targetFilePath = "BOM_B.xlsx";

            var excelReader = new NpoiReader();

            var sourceFile = excelReader.ReadData(sourceFilePath);
            var targetFile = excelReader.ReadData(targetFilePath);

            var comparer = new BomComparer.BomComparer();
            var results = comparer.Compare(sourceFile, targetFile);

            var path = $"{sourceFilePath} vs {targetFilePath}.xlsx";

            var writer = new NpoiWriter();
            writer.Write(path, results);

            //Console.ReadKey();
        }
    }
}