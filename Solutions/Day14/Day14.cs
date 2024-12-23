using RoelerCoaster.AdventOfCode.Year2024.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2024.Util;
using SkiaSharp;
using Spectre.Console;
using System.Text.RegularExpressions;

namespace RoelerCoaster.AdventOfCode.Year2024.Solutions.Day14;

internal class Day14 : DayBase
{
    public override int Day => 14;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Both;

    protected override async Task<string> SolvePart1(string input)
    {
        var robots = ParseInput(input);
        var rounds = 100;
        var width = 101;
        var height = 103;

        var finalPositions = robots.Select(r => GetFinalPosition(r, rounds, width, height)).ToList();

        var numberOfRobots = finalPositions
            .GroupBy(p => GetQuadrant(p, width, height))
            .Where(grp => grp.Key != 0)
            .Select(grp => grp.Count())
            .ToList();

        return numberOfRobots.Product().ToString();
    }

    protected override async Task<string> SolvePart2(string input)
    {
        var robots = ParseInput(input);
        var savePictures = false;

        string? path = null;

        if (savePictures)
        {
            Directory.CreateDirectory("./Outputs");

            path = Path.Join("./Outputs", Day.ToString());

            if (Path.Exists(path))
            {
                Directory.Delete(path, true);
            }

            Directory.CreateDirectory(path);
        }


        var result = Simulate(robots, 10_000, 101, 103, path);

        /*
         * After inspecting the generated images manually and finding the solution that way,
         * it turns out the solution is most likely the file with the lowest filesize.
         * 
         * PNG uses compression, and when the robots are arranged in a christmas tree this
         * appears to compress much better.
         */

        var guess = result.MinBy(r => r.PictureFileSize).Round;

        var report = $"Educated guess: {guess}";

        if (savePictures)
        {
            report += Environment.NewLine + $"Watch the generated images in {Path.GetFullPath(path)}";
        }

        return report;
    }


    private int GetQuadrant(XYCoordinate position, int width, int height)
    {
        var middleX = width / 2;
        var middleY = height / 2;


        if (position.X < middleX && position.Y < middleY)
        {
            return 1;
        }

        if (position.X > middleX && position.Y < middleY)
        {
            return 2;
        }

        if (position.X < middleX && position.Y > middleY)
        {
            return 3;
        }

        if (position.X > middleX && position.Y > middleY)
        {
            return 4;
        }

        return 0;
    }

    private XYCoordinate GetFinalPosition(Robot robot, int rounds, int width, int height)
    {
        var X = robot.Position.X + rounds * robot.Velocity.X;
        var Y = robot.Position.Y + rounds * robot.Velocity.Y;

        return new(MathUtil.Mod(X, width), MathUtil.Mod(Y, height));
    }

    private List<SimulationResult> Simulate(List<Robot> robots, int maxRounds, int width, int height, string? path)
    {
        var result = new List<SimulationResult>();
        for (var i = 1; i <= maxRounds; i++)
        {
            Parallel.ForEach(robots, r =>
            {
                r.Position = new(
                        MathUtil.Mod(r.Position.X + r.Velocity.X, width),
                        MathUtil.Mod(r.Position.Y + r.Velocity.Y, height)
                );
            });

            var fileSize = CreateRobotsPicture(robots, width, height, i, path);

            result.Add(new(i, fileSize));
        }

        return result;
    }


    private long CreateRobotsPicture(List<Robot> robots, int width, int height, int round, string? path)
    {
        var bitmap = new SKBitmap(width, height, SKColorType.Rgba8888, SKAlphaType.Opaque);

        foreach (var robot in robots)
        {
            bitmap.SetPixel(robot.Position.X, robot.Position.Y, SKColors.White);
        }

        using var data = bitmap.Encode(SKEncodedImageFormat.Png, 100);

        if (path != null)
        {
            using var stream = File.OpenWrite(Path.Combine(path, $"{round}.png"));
            // save the data to a stream
            data.SaveTo(stream);
        }

        return data.Size;
    }

    private List<Robot> ParseInput(string input)
    {
        return input.Lines()
            .Select(line =>
            {
                var split = line.Split(" ");

                var positionMatch = Regex.Match(split[0], @"p=(\d+),(\d+)");
                var velocityMatch = Regex.Match(split[1], @"v=(-?\d+),(-?\d+)");

                return new Robot(
                    new(
                        positionMatch.Groups[1].Value.ToNumber<int>(),
                        positionMatch.Groups[2].Value.ToNumber<int>()
                    ),
                    new(
                        velocityMatch.Groups[1].Value.ToNumber<int>(),
                        velocityMatch.Groups[2].Value.ToNumber<int>()
                    )
                );

            })
            .ToList();
    }

    private record struct XYCoordinate(int X, int Y);
    private class Robot(XYCoordinate position, XYCoordinate velocity)
    {
        public XYCoordinate Position { get; set; } = position;
        public XYCoordinate Velocity { get; set; } = velocity;

    }

    private record struct SimulationResult(int Round, long PictureFileSize);
}
