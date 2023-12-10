namespace AdventOfCode2023;

internal class Day10 : IDay
{
    private static readonly Point North = new Point(0, -1);
    private static readonly Point South = new Point(0, 1);
    private static readonly Point East = new Point(1, 0);
    private static readonly Point West = new Point(-1, 0);
    private static readonly Point NorthEast = new Point(1, -1);
    private static readonly Point SouthEast = new Point(1, 1);
    private static readonly Point NorthWest = new Point(-1, -1);
    private static readonly Point SouthWest = new Point(-1, 1);
    private const char S = 'S';

    private static readonly List<Point> Diffs = new List<Point> { North, South, East, West, NorthEast, NorthWest, SouthEast, SouthWest };

    private static readonly List<char> Pipes = new() { '|', '7', '-', 'F', 'J', 'L' };

    private static readonly Dictionary<Point, Dictionary<char, List<char>>> ValidMatches = new Dictionary<Point, Dictionary<char, List<char>>>
    {
        [North] = new Dictionary<char, List<char>>
        {
            ['|'] = new List<char> { '|', '7', 'F' },
            ['7'] = new List<char>(),
            ['-'] = new List<char>(),
            ['F'] = new List<char>(),
            ['J'] = new List<char> { '|', '7', 'F' },
            ['L'] = new List<char> { '|', '7', 'F' },
        },
        [South] = new Dictionary<char, List<char>>
        {
            ['|'] = new List<char> { '|', 'J', 'L' },
            ['7'] = new List<char> { '|', 'J', 'L' },
            ['-'] = new List<char>(),
            ['F'] = new List<char> { '|', 'J', 'L' },
            ['J'] = new List<char>(),
            ['L'] = new List<char>(),
        },
        [West] = new Dictionary<char, List<char>>
        {
            ['|'] = new List<char>(),
            ['7'] = new List<char> { 'L', '-', 'F' },
            ['-'] = new List<char> { 'L', '-', 'F' },
            ['F'] = new List<char>(),
            ['J'] = new List<char> { 'L', '-', 'F' },
            ['L'] = new List<char>(),
        },
        [East] = new Dictionary<char, List<char>>
        {
            ['|'] = new List<char>(),
            ['7'] = new List<char>(),
            ['-'] = new List<char> { 'J', '-', '7' },
            ['F'] = new List<char> { 'J', '-', '7' },
            ['J'] = new List<char>(),
            ['L'] = new List<char>() { 'J', '-', '7' },
        }
    };

    private record struct Point(int X, int Y);
    private record class Value(char Type, bool Found);

    public void Solve(IList<string?> inputLines)
    {
        int y = 0;
        var startPoint = new Point();
        Dictionary<Point, Value> loop = new();
        foreach (var line in inputLines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            int x = 0;
            var tiles = line.ToCharArray();
            foreach (var tile in tiles)
            {
                var coord = new Point(x, y);
                var val = new Value(tile, false);
                if (tile == S)
                {
                    startPoint = coord;
                    val = new Value(tile, true);
                }

                loop.Add(coord, val);
                x++;
            }
            y++;
        }

        startPoint = GetNextFromS(startPoint, loop);
        loop[startPoint] = new Value(loop[startPoint].Type, true);
        int distance = 1;
        Value next = loop[startPoint];
        while (distance == 1 || !next.Found)
        {
            foreach (var diff in Diffs)
            {
                var nextCoord = new Point(startPoint.X + diff.X, startPoint.Y + diff.Y);
                if (loop.ContainsKey(nextCoord) && loop[nextCoord].Type != '.')
                {
                    next = loop[nextCoord];
                    if (!next.Found && IsConnectedTile(diff, loop, nextCoord, startPoint))
                    {
                        loop[nextCoord] = new Value(next.Type, true);
                        distance++;
                        startPoint = nextCoord;
                        break;
                    }
                }
            }
        }

        var result = distance % 2 == 0 ? distance / 2 : distance / 2 + 1;
        Console.WriteLine($"{this.GetType().Name} result part 1: {result}");
    }

    private static Point GetNextFromS(Point startPoint, Dictionary<Point, Value> loop)
    {
        foreach (var diff in Diffs)
        {
            var nextCoord = new Point(startPoint.X + diff.X, startPoint.Y + diff.Y);
            if (loop.ContainsKey(nextCoord) && loop[nextCoord].Type != '.')
            {
                foreach (var current in Pipes)
                {
                    loop[startPoint] = new Value(current, true);
                    if (IsConnectedTile(diff, loop, nextCoord, startPoint))
                    {
                        return nextCoord;
                    }
                }
            }
        }

        return startPoint;
    }

    private static bool IsConnectedTile(Point diff, Dictionary<Point, Value> loop, Point nextCoord, Point previousCoord)
    {
        var next = loop[nextCoord];
        var previous = loop[previousCoord];
        if (ValidMatches.ContainsKey(diff))
        {
            var matches = ValidMatches[diff];
            if (matches[previous.Type].Contains(next.Type))
            {
                return true;
            }
        }

        return false;
    }
}
