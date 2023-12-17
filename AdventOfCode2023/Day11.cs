namespace AdventOfCode2023;

internal class Day11 : IDay
{
    private const char Galaxy = '#';

    private record struct Point(int X, int Y);
    
    public void Solve(IList<string?> inputLines)
    {
        SolvePart(1, inputLines);
        SolvePart(2, inputLines);
    }
   
    private void SolvePart(int part, IList<string?> inputLines)
    {
        var expand = part == 1 ? 2 : 1000000;
        int x = 0;
        int y = 0;
        Dictionary<int, Point> galaxies = new();
        var galaxyIndex = 0;
        foreach (var line in inputLines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            x = 0;
            var tiles = line.ToCharArray();
            var lineIsEmpty = !tiles.Contains(Galaxy);
            if (lineIsEmpty)
            {
                y += expand;
            }
            else
            {
                foreach (var tile in tiles)
                {
                    if (tile == Galaxy)
                    {
                        galaxies[++galaxyIndex] = new Point(x, y);
                    }
                    x++;
                }
                y++;
            }
        }

        var originalGalaxies = galaxies.ToDictionary(g => g.Key, g => g.Value);
        for (int i = x - 1; i >= 0; i--)
        {
            var isEmptyColumn = !galaxies.Any(g => g.Value.X == i);
            if (isEmptyColumn)
            {
                for (int g = 1; g <= galaxyIndex; g++)
                {
                    if (originalGalaxies[g].X > i)
                    {
                        galaxies[g] = new Point(galaxies[g].X + expand - 1, galaxies[g].Y);
                    }
                }
            }
        }

        long sum = 0;
        for (int i = 1; i <= galaxies.Count; i++)
        {
            for (int j = i + 1; j <= galaxies.Count; j++)
            {
                sum += Math.Abs(galaxies[i].X - galaxies[j].X) + Math.Abs(galaxies[i].Y - galaxies[j].Y);
            }
        }

        Console.WriteLine($"{this.GetType().Name} result part {part}: {sum}");
    }
}
