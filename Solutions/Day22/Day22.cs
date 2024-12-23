using MoreLinq;
using RoelerCoaster.AdventOfCode.Year2024.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2024.Util;
using System.Collections.Concurrent;

namespace RoelerCoaster.AdventOfCode.Year2024.Solutions.Day22;

internal class Day22 : DayBase
{
    public override int Day => 22;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Both;

    protected override async Task<string> SolvePart1(string input)
    {
        var generators = input.NumberLines<long>()
            .Select(i => new BuyerPseudorandomGenerator(i))
            .ToList();

        Parallel.ForEach(generators, generator =>
        {
            for (var i = 0; i < 1999; i++)
            {
                generator.Next();
            }
        });

        var numbers2000 = generators.Select(g => g.Next());

        return numbers2000.Sum().ToString();
    }

    protected override async Task<string> SolvePart2(string input)
    {
        var generators = input.NumberLines<long>()
            .Select(i => new BuyerPseudorandomGenerator(i))
            .ToList();

        var sequenceToPriceMapsBag = new ConcurrentBag<Dictionary<(int, int, int, int), int>>();

        Parallel.ForEach(generators, generator =>
        {
            var prices = new List<int>
            {
                (int)(generator.Seed %10)
            };
            for (var i = 0; i < 2000; i++)
            {
                prices.Add((int)(generator.Next() % 10));
            }

            var changes = prices.Pairwise((a, b) => a - b).ToList();

            var sequenceToPriceMap = new Dictionary<(int, int, int, int), int>();
            for (var i = 0; i <= changes.Count - 4; i++)
            {
                var sequence = (changes[i], changes[i + 1], changes[i + 2], changes[i + 3]);
                var price = prices[i + 4];

                if (!sequenceToPriceMap.ContainsKey(sequence))
                {
                    // We only record the first price for each sequence
                    sequenceToPriceMap.Add(sequence, price);
                }
            }

            sequenceToPriceMapsBag.Add(sequenceToPriceMap);
        });

        var sequenceToPriceMaps = sequenceToPriceMapsBag.ToList();

        // Collect all possible sequences
        var allSequences = sequenceToPriceMaps.SelectMany(s => s.Keys).ToHashSet();

        var prices = new ConcurrentBag<int>();

        Parallel.ForEach(allSequences, sequence =>
        {
            var price = sequenceToPriceMaps.Select(map => map.TryGetValue(sequence, out var p) ? p : 0).Sum();
            prices.Add(price);
        });

        return prices.Max().ToString();
    }

}
