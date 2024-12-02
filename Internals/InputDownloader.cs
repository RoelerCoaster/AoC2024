using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using Spectre.Console;

namespace RoelerCoaster.AdventOfCode.Year2024.Internals;
internal class InputDownloader
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;

    public InputDownloader(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
    }

    public async Task DownloadFile(int year, int day, string filePath)
    {
        var token = GetToken();

        if (token == null)
        {
            AnsiConsole.WriteLine("[yellow]AOC session token not found. Not downloading input [/]");
            return;
        }

        var request = new HttpRequestMessage(HttpMethod.Get, $"https://adventofcode.com/{year}/day/{day}/input")
        {
            Headers =
            {
                {HeaderNames.Cookie, $"session={token}" }
            },

        };

        request.Headers.TryAddWithoutValidation(HeaderNames.UserAgent, "github.com/RoelerCoaster");

        using var client = _httpClientFactory.CreateClient();
        var response = await client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var input = await response.Content.ReadAsStringAsync();

        await File.WriteAllTextAsync(filePath, input);
    }

    private string? GetToken()
    {
        return _configuration.GetSection("AOC")?.GetValue<string>("Token");
    }
}
