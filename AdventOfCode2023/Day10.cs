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

    private static readonly List<Point> Diffs = new List<Point> { East, West, South, North };
    private static readonly List<Point> NeighbourDiffs = new List<Point> { NorthWest, West, SouthWest, North, South, NorthEast, East, SouthEast };
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
    private record class ValueInt(int Index, bool IsBorder, int GroupIndex);

    public void Solve(IList<string?> inputLines)
    {
        int xLength = 0;
        int y = 0;
        var startPoint = new Point();
        var sPoint = new Point();
        Dictionary<Point, Value> map = ReadMap(inputLines, ref xLength, ref y, ref startPoint);

        sPoint = startPoint;
        startPoint = GetNextFromS(startPoint, map);
        map[startPoint] = new Value(map[startPoint].Type, true);
        int distance = 1;
        Value next = map[startPoint];
        Dictionary<Point, ValueInt> map2 = new();
        foreach (var coord in map.Keys)
        {
            map2.Add(new Point(coord.X, coord.Y), new ValueInt(0, false, 0));
        }
        map2[sPoint] = new ValueInt(1, false, 0);
        map2[startPoint] = new ValueInt(2, false, 0);
        while (distance == 1 || !next.Found)
        {
            foreach (var diff in Diffs)
            {
                var nextCoord = new Point(startPoint.X + diff.X, startPoint.Y + diff.Y);
                if (map.ContainsKey(nextCoord) && map[nextCoord].Type != '.')
                {
                    next = map[nextCoord];
                    if (!next.Found && IsConnectedTile(diff, map, nextCoord, startPoint))
                    {
                        map[nextCoord] = new Value(next.Type, true);
                        distance++;
                        map2[nextCoord] = new ValueInt(distance + 1, false, 0);
                        startPoint = nextCoord;
                        break;
                    }
                }
            }
        }

        var result = distance % 2 == 0 ? distance / 2 : distance / 2 + 1;
        Console.WriteLine($"{this.GetType().Name} result part 1: {result}");

        // Part 2
        var tilesEnclosed = 0;
        var groups = ExtractGroups(map2);
        var groupsToDiscard = new HashSet<int>();
        var orderedLoop = map2.Where(l => l.Value.Index > 0).OrderBy(l => l.Value.Index).Select(l => l.Key).ToList();
        foreach (var group in groups)
        {
            var first = group.Value.First();
            if (!IsPointInLoop(first, orderedLoop))
            {
                groupsToDiscard.Add(group.Key);
            }
        }

        tilesEnclosed = groups.Where(g => !groupsToDiscard.Contains(g.Key)).ToDictionary().Values.Sum(group => group.Count);

        Console.WriteLine($"{this.GetType().Name} result part 2: {tilesEnclosed}");

        // useful to visualize the map and groups
        // PrintMap(xLength, y, map2, groups);
    }

    private static Dictionary<Point, Value> ReadMap(IList<string?> inputLines, ref int xLength, ref int y, ref Point startPoint)
    {
        Dictionary<Point, Value> map = new();
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

                map.Add(coord, val);
                x++;
                xLength = x;
            }
            y++;
        }

        return map;
    }

    private bool IsPointInLoop(Point point, IList<Point> loopPoints)
    {
        bool isInsideLoop = false;
        for (int i = 0; i < loopPoints.Count; i++)
        {
            var point1 = loopPoints[i];
            var point2 = loopPoints[(i + 1) % loopPoints.Count];
            if (((point1.Y > point.Y) != (point2.Y > point.Y)) &&
                (point.X < (point2.X - point1.X) * (point.Y - point1.Y) / (point2.Y - point1.Y) + point1.X))
            {
                isInsideLoop = !isInsideLoop;
            }
        }

        return isInsideLoop;
    }

    private static Dictionary<int, List<Point>> ExtractGroups(Dictionary<Point, ValueInt> map2)
    {
        Dictionary<int, List<Point>> groups = new();
        var group = new List<Point>();

        // first eliminate 0s with link to borders
        Mark0Borders(map2);
        int groupIndex = 1;
        var maxX = map2.Select(p => p.Key.X).Max();
        var maxY = map2.Select(p => p.Key.Y).Max();

        for (int y = 0; y <= maxY; y++)
        {
            for (int x = 0; x <= maxX; x++)
            {
                var coord = new Point(x, y);
                if (IsBorder0(coord, map2))
                {
                    MarkAsBorder(map2, coord);
                }
                else if (map2[coord].Index == 0)
                {
                    var tileGroupIndex = GetNeighbour0GroupIndex(coord, map2);
                    if (tileGroupIndex == 0)
                    {
                        groups[groupIndex] = new List<Point>();
                        tileGroupIndex = groupIndex++;
                    }
                    SetGroupIndex(map2, coord, tileGroupIndex);
                    groups[tileGroupIndex].Add(coord);
                }
            }
        }

        return groups;
    }

    private static void Mark0Borders(Dictionary<Point, ValueInt> map2)
    {
        var maxX = map2.Select(p => p.Key.X).Max();
        var maxY = map2.Select(p => p.Key.Y).Max();
        int top = 0, bottom = maxY;
        int left = 0, right = maxX;
        while (true)
        {
            if (left > right)
            {
                break;
            }

            // Traverse from left to right.
            for (int i = left; i <= right; i++)
            {
                var point = new Point(i, top);
                if (IsBorder0(point, map2))
                {
                    MarkAsBorder(map2, point);
                }
            }
            top++;

            if (top > bottom)
            {
                break;
            }

            // Traverse from top to bottom.
            for (int i = top; i <= bottom; i++)
            {
                var point = new Point(right, i);
                if (IsBorder0(point, map2))
                {
                    MarkAsBorder(map2, point);
                }
            }
            right--;

            if (left > right)
            {
                break;
            }

            // Traverse from right to left.
            for (int i = right; i >= left; i--)
            {
                var point = new Point(i, bottom);
                if (IsBorder0(point, map2))
                {
                    MarkAsBorder(map2, point);
                }
            }
            bottom--;

            if (top > bottom)
            {
                break;
            }

            // Traverse from bottom to top.
            for (int i = bottom; i >= top; i--)
            {
                var point = new Point(left, i);
                if (IsBorder0(point, map2))
                {
                    MarkAsBorder(map2, point);
                }
            }
            left++;
        }
    }

    private static void MarkAsBorder(Dictionary<Point, ValueInt> map2, Point tileCoord)
    {
        map2[tileCoord] = new ValueInt(map2[tileCoord].Index, true, map2[tileCoord].GroupIndex);
    }

    private static void SetGroupIndex(Dictionary<Point, ValueInt> map2, Point tileCoord, int groupIndex)
    {
        map2[tileCoord] = new ValueInt(map2[tileCoord].Index, map2[tileCoord].IsBorder, groupIndex);
    }

    private static int GetNeighbour0GroupIndex(Point tileCoord, Dictionary<Point, ValueInt> map2)
    {
        if (!map2.ContainsKey(tileCoord) || map2[tileCoord].Index != 0)
        {
            return 0;
        }
        if (map2[tileCoord].GroupIndex > 0)
        {
            return map2[tileCoord].GroupIndex;
        }

        foreach (var diff in NeighbourDiffs)
        {
            var neighbourCoord = new Point(tileCoord.X + diff.X, tileCoord.Y + diff.Y);
            if (map2.ContainsKey(neighbourCoord) && map2[neighbourCoord].Index == 0 && map2[neighbourCoord].GroupIndex > 0)
            {
                return map2[neighbourCoord].GroupIndex;
            }
        }

        return GetNeighbour0GroupIndex(new Point(tileCoord.X + 1, tileCoord.Y), map2);
    }

    private static bool IsBorder0(Point tileCoord, Dictionary<Point, ValueInt> map2)
    {
        if (map2[tileCoord].Index != 0)
        {
            return false;
        }

        if (map2[tileCoord].IsBorder)
        {
            return true;
        }

        foreach (var diff in NeighbourDiffs)
        {
            var neighbourCoord = new Point(tileCoord.X + diff.X, tileCoord.Y + diff.Y);
            if (!map2.ContainsKey(neighbourCoord) || map2[neighbourCoord].IsBorder)
            {
                return true;
            }
        }

        return false;
    }

    private static Point GetNextFromS(Point startPoint, Dictionary<Point, Value> map)
    {
        foreach (var diff in Diffs)
        {
            var nextCoord = new Point(startPoint.X + diff.X, startPoint.Y + diff.Y);
            if (map.ContainsKey(nextCoord) && map[nextCoord].Type != '.')
            {
                foreach (var current in Pipes)
                {
                    map[startPoint] = new Value(current, true);
                    if (IsConnectedTile(diff, map, nextCoord, startPoint))
                    {
                        return nextCoord;
                    }
                }
            }
        }

        return startPoint;
    }

    private static bool IsConnectedTile(Point diff, Dictionary<Point, Value> map, Point nextCoord, Point previousCoord)
    {
        var next = map[nextCoord];
        var previous = map[previousCoord];
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

    private static void PrintMap(int xLength, int y, Dictionary<Point, ValueInt> map2, Dictionary<int, List<Point>> groups)
    {
        Console.WriteLine();
        Console.OutputEncoding = System.Text.Encoding.Unicode;
        for (int i = 0; i < y; i++)
        {
            for (int j = 0; j < xLength; j++)
            {
                var point = new Point(j, i);
                var ind = map2[point].Index;
                var isBorder = map2[point].IsBorder;
                var isInGroup = groups.Values.Any(g => g.Contains(point));
                if (ind == 0 && isInGroup)
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    ind = map2[point].GroupIndex;
                }
                else if (ind == 0 && isBorder)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                }
                else if (ind == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else if (isInGroup)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else if (ind == 1)
                {
                    // It's the S
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }

                Console.Write($"{ind.ToString("D3")} ");
                Console.ResetColor();
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }
}
