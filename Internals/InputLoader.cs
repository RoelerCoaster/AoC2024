using Spectre.Console;

namespace RoelerCoaster.AdventOfCode.Year2024.Internals;

internal class InputLoader
{
    private const string INPUT_DIR = "./Inputs";

    private const string TEST_DIR_NAME = "Test";
    private const string ACTUAL_DIR_NAME = "Actual";

    private readonly InputDownloader _inputDownloader;

    public InputLoader(InputDownloader inputDownloader)
    {
        _inputDownloader = inputDownloader;
    }

    public async Task<string> GetInputForDay(int year, int day, bool useTestInput)
    {
        string inputDirName;
        if (useTestInput)
        {
            AnsiConsole.MarkupLine("[yellow]Warning: Using test input. The reported solutions will not be correct for the real input![/]");
            inputDirName = TEST_DIR_NAME;
        }
        else
        {
            inputDirName = ACTUAL_DIR_NAME;
        }

        var path = Path.Combine(INPUT_DIR, inputDirName, $"{day:D2}.txt");

        if (!Path.Exists(path) && !useTestInput)
        {
            // Try downloading file
            AnsiConsole.MarkupLine("Input file not found. Trying to download it.");

            await _inputDownloader.DownloadFile(year, day, path);
        }

        if (!Path.Exists(path))
        {
            AnsiConsole.MarkupLine("[red]Input file not found. Please create the file at:[/]");
            AnsiConsole.Write(new TextPath(Path.GetFullPath(path)));
            AnsiConsole.WriteLine();

            throw new FileNotFoundException();
        }

        return (await File.ReadAllTextAsync(path)).Trim().ReplaceLineEndings();
    }
}
