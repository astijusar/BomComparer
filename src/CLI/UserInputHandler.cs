using Spectre.Console;

namespace CLI
{
    public static class UserInputHandler
    {
        private static readonly string[] AllowedFileExtensions = new[] { ".xls", ".xlsx" };

        /*public static string GetFilePath(string label)
        {
            var error = false;

            while (true)
            {
                AnsiConsole.Clear();

                if (error)
                    AnsiConsole.Markup("[red]File with given path does not exist! Please try again.[/] \n");

                var filePath = AnsiConsole.Ask<string>(label);

                if (File.Exists(filePath)) return filePath;

                error = true;
            }
        }*/

        public static string GetFilePath(string label)
        {
            AnsiConsole.Clear();

            return AnsiConsole.Prompt(
                new TextPrompt<string>(label)
                    .Validate(path =>
                    {
                        if (!File.Exists(path))
                            return ValidationResult.Error("[red]File with given path does not exist! Please try again.[/]");

                        if (!AllowedFileExtensions.Contains(Path.GetExtension(path)))
                            return ValidationResult.Error("[red]Only .xls and .xlsx files are allowed!.[/]");

                        return ValidationResult.Success();
                    }));
        }

        public static string GetDirectory(string label)
        {
            AnsiConsole.Clear();

            return AnsiConsole.Prompt(
                new TextPrompt<string>(label)
                    .Validate(path =>
                    {
                        if (string.IsNullOrEmpty(path)) 
                            return ValidationResult.Success();

                        if (!Directory.Exists(path))
                            return ValidationResult.Error("[red]Directory with given path does not exist! Please try again.[/]");

                        return ValidationResult.Success();
                    })
                    .AllowEmpty());
        }
    }
}
