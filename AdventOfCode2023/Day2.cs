using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023;

internal class Day2 : IDay
{
    private const string Blue = "blue";
    private const string Red = "red";
    private const string Green = "green";

    private readonly List<string> colors = new() { Blue, Red, Green };

    private record CubeSet(int RedCubes, int GreenCubes, int BlueCubes);

    private static readonly CubeSet bag = new CubeSet(12, 13, 14);

    public void Solve(IList<string?> inputLines)
    {
        int sum = 0;
        long sumPowers = 0;
        int gameIndex = 0;
        foreach (var line in inputLines) 
        {
            gameIndex++;
            if (line is not null)
            {
                var sets = InputReader.ProcessStringLine(line.Replace($"Game {gameIndex}:", string.Empty), ';');
                var cubeSets = new List<CubeSet>();
                foreach (var set in sets) 
                {
                    var numbersAndColors = set.Trim().Split(", ");
                    int redCubes = 0;
                    int greenCubes = 0;
                    int blueCubes = 0;
                    foreach (var numberAndColor in numbersAndColors)
                    {
                        foreach (var color in colors)
                        {
                            var colorIndex = numberAndColor.IndexOf(color, StringComparison.OrdinalIgnoreCase);
                            if (colorIndex > 0)
                            {
                                var number = Convert.ToInt16(numberAndColor.Substring(0, colorIndex - 1));
                                switch (color)
                                {
                                    case Red:
                                        redCubes += number; 
                                        break;
                                    case Green:
                                        greenCubes += number;
                                        break;
                                    case Blue:
                                        blueCubes += number;
                                        break;
                                }
                            }
                        }
                    }
                    cubeSets.Add(new CubeSet(redCubes, greenCubes, blueCubes));
                }

                if (IsGamePossible(cubeSets)) 
                {
                    sum += gameIndex;
                }

                var minimumSet = GetMinimumNumberOfColoredCubes(cubeSets);
                sumPowers += minimumSet.RedCubes * minimumSet.GreenCubes * minimumSet.BlueCubes;
            }
        }

        Console.WriteLine($"{this.GetType().Name} result part 1: {sum}");
        Console.WriteLine($"{this.GetType().Name} result part 2: {sumPowers}");

        static bool IsGamePossible(List<CubeSet> sets)
        {
            foreach (var set in sets) 
            {
                if (set.RedCubes > bag.RedCubes || set.GreenCubes > bag.GreenCubes || set.BlueCubes > bag.BlueCubes)
                {
                    return false;
                }
            }

            return true;
        }

        static CubeSet GetMinimumNumberOfColoredCubes(List<CubeSet> cubeSets)
        {
            var minRedCubes = 0;
            var minBlueCubes = 0;
            var minGreenCubes = 0;

            foreach (var cubeSet in cubeSets) 
            {
                if (cubeSet.RedCubes > minRedCubes)
                {
                    minRedCubes = cubeSet.RedCubes;
                }
                if (cubeSet.GreenCubes > minGreenCubes)
                {
                    minGreenCubes = cubeSet.GreenCubes;
                }
                if (cubeSet.BlueCubes > minBlueCubes)
                {
                    minBlueCubes = cubeSet.BlueCubes;
                }
            }

            return new CubeSet(minRedCubes, minGreenCubes, minBlueCubes);
        }
    }
}
