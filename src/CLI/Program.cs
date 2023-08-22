using BomComparer.Comparer;
using BomComparer.ExcelReaders;
using BomWriter.ExcelWriter;
using Spectre.Console;
using Spectre.Console.Cli;

namespace CLI
{
    public class Program
    {
        static int Main(string[] args)
        {
            var app = new CommandApp<BomComparisonCommand>();
            app.Configure(config =>
            {
                config.PropagateExceptions();
            });

            try
            {
                return app.Run(args);
            }
            catch (Exception ex)
            {
                AnsiConsole.Clear();
                AnsiConsole.WriteLine(ex.Message);
                return -1;
            }
        }
    }
}