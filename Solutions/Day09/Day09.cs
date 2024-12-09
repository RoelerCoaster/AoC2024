using RoelerCoaster.AdventOfCode.Year2024.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2024.Util;

namespace RoelerCoaster.AdventOfCode.Year2024.Solutions.Day09;

internal class Day09 : DayBase
{
    public override int Day => 9;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Both;

    protected override async Task<string> SolvePart1(string input)
    {
        var diskmap = input.Digits();

        var checksum = 0L;
        var i = 0;
        var j = diskmap.Length - 1;
        var block = 0;

        while (i <= j)
        {
            if (i % 2 == 0)
            {
                // even index from the left. This space contains a file. Add file to checksum
                var fileId = i / 2;
                var fileSize = diskmap[i];
                for (var k = 0; k < fileSize; k++)
                {
                    checksum += (block * fileId);
                    block++;
                }
            }
            else
            {
                // Odd index from the left. We are in free space. Move blocks from the end to the start.
                var emptySpaceSize = diskmap[i];

                while (emptySpaceSize > 0)
                {
                    while (j % 2 == 1 || diskmap[j] == 0)
                    {
                        // j currently points to free space, or we already have moved the file at j completely.
                        j--;
                    }

                    if (j <= i)
                    {
                        break;
                    }

                    var fileId = j / 2;
                    while (emptySpaceSize > 0 && diskmap[j] > 0)
                    {
                        checksum += (block * fileId);
                        block++;

                        diskmap[j]--;
                        emptySpaceSize--;
                    }
                }
            }


            i++;

        }

        return checksum.ToString();

    }

    protected override async Task<string> SolvePart2(string input)
    {
        var diskmap = input.Digits()
            .Select((size, index) => (
                Files: index % 2 == 0 ? new List<(int Id, int Size)> {
                     (index / 2, size)
                } : new List<(int Id, int Size)>(),
                TotalSize: size
            ))
            .ToArray();


        for (var j = diskmap.Length - 1; j >= 0; j--)
        {
            if (j % 2 != 0)
            {
                continue;
            }

            var files = diskmap[j].Files;
            var fileToMove = files.Single(f => f.Id == j / 2);
            // find spot to move file to:
            for (var i = 1; i < j; i += 2)
            {
                var emptySize = diskmap[i].TotalSize - (diskmap[i].Files.Sum(f => f.Size));

                if (emptySize >= fileToMove.Size)
                {
                    diskmap[i].Files.Add(fileToMove);
                    files.Remove(fileToMove);
                    break;
                }
            }
        }

        var checksum = 0L;
        var block = 0;

        foreach (var entry in diskmap)
        {
            var empty = entry.TotalSize;
            foreach (var file in entry.Files)
            {
                for (var k = 0; k < file.Size; k++)
                {
                    checksum += file.Id * block;
                    block++;
                }

                empty -= file.Size;
            }
            block += empty;
        }

        return checksum.ToString();
    }

}
