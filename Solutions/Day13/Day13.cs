using RoelerCoaster.AdventOfCode.Year2024.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2024.Util;
using Spectre.Console;
using System.Text.RegularExpressions;

namespace RoelerCoaster.AdventOfCode.Year2024.Solutions.Day13;

internal class Day13 : DayBase
{
    public override int Day => 13;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Both;

    /*
     * Reasoning:
     * 
     * Let c_i denote the number of times we press button i
     * 
     * For the x coordinate we have that 
     * 
     * c_A * X_A + c_B * X_B = X 
     * ->
     * c_b = (X - c_A * X_A) / X_B
     * 
     * Both c_A and c_b have to be integer, so we know we have to find c_A such that (X - c_A * X_A) % X_B = 0
     * 
     * The same holds for the y-coordinate.
     * 
     * Since we know c_A + c_B <= 100, we can simply try all possibilities
     * 
     */
    protected override async Task<string> SolvePart1(string input)
    {
        var clawGames = ParseInput(input);

        var tokens = clawGames.Select(game => GetNumberOfTokens(game, 101)).ToList();

        return tokens.Select(t => t.GetValueOrDefault()).Sum().ToString();
    }

    /*
     * Reasoning:
     * 
     * As it turns out, working out the equations a bit more helps. 
     * 
     * We have 
     * 
     * c_b = (X - c_A * X_A) / X_B
     * c_b = (Y - c_A * Y_A) / Y_B
     * 
     * -> 
     * (X - c_A * X_A) / X_B = (Y - c_A * Y_A) / Y_B
     * 
     * ->
     * c_A = (Y_B * X - X_B * Y) / (Y_B * X_A - X_B * Y_A)
     * 
     * So, unless the above denominator is 0, we have just a single solution which gives
     * the minimum number of tokens.
     * 
     * It appears that the input is crafted in such a way that (Y_B * X_A - X_B * Y_A) is never 0,
     * so we will ignore that case.
     * 
     * So what we simply have to chec now is that both
     * c_A = (Y_B * X - X_B * Y) / (Y_B * X_A - X_B * Y_A)
     * c_b = (Y - c_A * Y_A) / Y_B
     * 
     * are integer.
     * 
     */
    protected override async Task<string> SolvePart2(string input)
    {
        var bignumber = 10000000000000;
        var clawGames = ParseInput(input)
            .Select(g => g with { Price = new Coordinate(g.Price.X + bignumber, g.Price.Y + bignumber) });


        var tokens = clawGames.Select(game => GetNumberOfTokensV2(game)).ToList();

        return tokens.Select(t => t.GetValueOrDefault()).Sum().ToString();
    }


    private long? GetNumberOfTokens(ClawGame game, long maxCount)
    {
        var validResults = new List<(long CA, long CB)>();

        for (var cA = 0; cA < maxCount; cA++)
        {
            if (cA * game.A.X > game.Price.X || cA * game.A.Y > game.Price.Y)
            {
                // We have already overshot the price, so no need to try any further
                break;
            }

            var (cBx, remx) = Math.DivRem(game.Price.X - cA * game.A.X, game.B.X);
            var (cBy, remy) = Math.DivRem(game.Price.Y - cA * game.A.Y, game.B.Y);

            if (remx is 0 && remy is 0 && cBx == cBy)
            {
                validResults.Add((cA, cBx));
            }
        }

        return validResults.Min(r => (long?)3 * r.CA + r.CB);
    }

    private long? GetNumberOfTokensV2(ClawGame game)
    {
        var nom = game.B.Y * game.Price.X - game.B.X * game.Price.Y;
        var den = game.B.Y * game.A.X - game.B.X * game.A.Y;

        if (den == 0)
        {
            throw new DivideByZeroException();
        }

        var (cA, remX) = Math.DivRem(nom, den);
        if (remX != 0)
        {
            // cA is not integer
            return null;
        }

        var (cB, remY) = Math.DivRem(game.Price.X - cA * game.A.X, game.B.X);

        if (remY != 0)
        {
            // cB is not integer
            return null;
        }

        return 3 * cA + cB;
    }


    private static List<ClawGame> ParseInput(string input)
    {
        return input.Sections()
            .Select(section =>
            {

                var coords = section.Lines()
                    .Select(line =>
                    {
                        var numbers = Regex.Matches(line, @"\d+").Select(m => m.Value.ToNumber<long>()).ToArray();
                        return new Coordinate(numbers[0], numbers[1]);
                    })
                    .ToArray();

                return new ClawGame(coords[0], coords[1], coords[2]);
            })
            .ToList();
    }

    private record struct Coordinate(long X, long Y);
    private record struct ClawGame(Coordinate A, Coordinate B, Coordinate Price);
}
