using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MoreLinq;
using RoelerCoaster.AdventOfCode.Year2024.Internals;
using RoelerCoaster.AdventOfCode.Year2024.Solutions;
using Spectre.Console;

int? day = null;

if (args.Length > 0)
{
    day = int.Parse(args[0]);
}

var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var serviceCollection = new ServiceCollection()
    .AddSingleton<InputLoader>()
    .AddSingleton<DayResolver>()
    .AddSingleton<InputDownloader>()
    .AddSingleton<IConfiguration>(configuration)
    .AddHttpClient();

typeof(Program)
    .Assembly
    .GetTypes()
    .Where(t => t.IsAssignableTo(typeof(DayBase)) && !t.IsAbstract)
    .ForEach(t => serviceCollection.AddSingleton(typeof(DayBase), t));

try
{
    await ActivatorUtilities.CreateInstance<Solver>(serviceCollection.BuildServiceProvider(), 2024).Run(day);
}
catch (Exception ex)
{
    AnsiConsole.WriteException(ex);
}

AnsiConsole.WriteLine("Press any key to quit.");
Console.ReadLine();