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
            catch (IOException ex)
            {
                AnsiConsole.Clear();
                AnsiConsole.WriteLine("Comparison failed! Check if the source, target or result files are closed.");
                return -1;
            }
            catch (Exception ex)
            {
                AnsiConsole.Clear();
                AnsiConsole.WriteLine("Unexpected error occurred!");
                return -1;
            }
        }
    }
}