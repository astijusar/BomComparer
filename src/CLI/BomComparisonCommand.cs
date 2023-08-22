using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using BomComparer.Comparer;
using BomComparer.ExcelReaders;
using BomWriter.ExcelWriter;
using Spectre.Console;
using Spectre.Console.Cli;

namespace CLI;

internal sealed class BomComparisonCommand : Command<BomComparisonCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [Description("Path to source BOM file.")]
        [CommandArgument(0, "[sourceBomPath]")]
        public string? SourceFilePath { get; set; }

        [Description("Path to target BOM file.")]
        [CommandArgument(0, "[targetBomPath]")]
        public string? TargetFilePath { get; set; }

        [Description("Path to output the result")]
        [CommandOption("--output")]
        public string? OutputPath { get; set; }
    }

    public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
    {
        settings.SourceFilePath ??= UserInputHandler.GetFilePath("Enter source BOM file path: ");
        settings.TargetFilePath ??= UserInputHandler.GetFilePath("Enter target BOM file path: ");
        settings.OutputPath ??= UserInputHandler.GetDirectory("[yellow](Optional)[/] Enter output path: ");

        var resultFileName = Utilities.ConstructFilePath(settings.OutputPath, settings.SourceFilePath, settings.TargetFilePath);

        AnsiConsole.Clear();

        AnsiConsole.Status()
            .Spinner(Spinner.Known.Line)
            .Start("Comparing...", ctx =>
            {
                var reader = new NpoiReader();
                var sourceData = reader.ReadData(settings.SourceFilePath);
                var targetData = reader.ReadData(settings.TargetFilePath);

                var comparer = new BomCompare();
                var results = comparer.Compare(sourceData, targetData);

                ctx.Status("Writing results to file...");

                var writer = new NpoiWriter();
                writer.Write(resultFileName, results);
            });

        AnsiConsole.Markup($"Done! The results are saved here: [yellow]{resultFileName}[/] \n");

        return 0;
    }
}