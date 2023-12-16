
namespace AdventOfCode2023;

internal class Day16 : IDay
{
    private enum Direction
    {
        Up,
        Down,
        Left,
        Right
    };

    private const char EmptySpace = '.';
    private const char MirrorF = '/';
    private const char MirrorB = '\\';
    private const char VerticalSplitter = '|';
    private const char HorizontalSplitter = '-';
    private record struct Point(int X, int Y);
    private record struct Tile(Point Coord, char Type, bool IsEnergized, HashSet<Direction> EnergizedFrom);

    private Dictionary<Point, Tile> map = new Dictionary<Point, Tile>();

    public void Solve(IList<string?> inputLines)
    {
        map = ReadMap(inputLines);
        var startPoint = new Point(-1, 0);
        var direction = Direction.Right;
        var energizedTiles = CalculateEnergizedTiles(startPoint, direction, 0);

        Console.WriteLine($"{this.GetType().Name} result part 1: {energizedTiles}");

        SolvePart2();
    }

    private void SolvePart2()
    {
        var leftX = 0;
        var rightX = map.Keys.Max(p => p.X);
        var topY = 0;
        var bottomY = map.Keys.Max(p => p.Y);
        var maxTiles = 0;

        for (int x = leftX; x <= rightX; x++)
        {
            var startPoint = new Point(x, topY - 1);
            var direction = Direction.Down;
            maxTiles = CalculateEnergizedTiles(startPoint, direction, maxTiles);
        }
        for (int x = leftX; x <= rightX; x++)
        {
            var startPoint = new Point(x, bottomY + 1);
            var direction = Direction.Up;
            maxTiles = CalculateEnergizedTiles(startPoint, direction, maxTiles);
        }
        for (int y = topY; y <= bottomY; y++)
        {
            var startPoint = new Point(leftX - 1, y);
            var direction = Direction.Right;
            maxTiles = CalculateEnergizedTiles(startPoint, direction, maxTiles);
        }
        for (int y = topY; y <= bottomY; y++)
        {
            var startPoint = new Point(rightX + 1, y);
            var direction = Direction.Left;
            maxTiles = CalculateEnergizedTiles(startPoint, direction, maxTiles);
        }

        Console.WriteLine($"{this.GetType().Name} result part 2: {maxTiles}");
    }

    private int CalculateEnergizedTiles(Point startPoint, Direction direction, int maxTiles)
    {
        foreach (var tile in map)
        {
            SetIsEnergized(map, tile.Key, false, null);
        }

        MoveThroughTiles(startPoint, direction);
        var energizedTiles = map.Count(m => m.Value.IsEnergized);

        //Console.WriteLine();
        //Console.WriteLine($"StartPoint: {startPoint.X} {startPoint.Y} Energized tiles: {energizedTiles}");

        if (maxTiles < energizedTiles)
        {
            maxTiles = energizedTiles;
        }

        return maxTiles;
    }

    private void MoveThroughTiles(Point startPoint, Direction direction)
    {
        var nextPoint = GetNextPoint(startPoint, direction);
        if (map.ContainsKey(nextPoint) && !map[nextPoint].EnergizedFrom.Contains(direction))
        {
            SetIsEnergized(map, nextPoint, true, direction);
            switch (map[nextPoint].Type)
            {
                case EmptySpace:
                    MoveThroughTiles(nextPoint, direction);
                    break;
                case MirrorF:
                    MoveThroughTiles(nextPoint, GetDirectionMirrorF(direction));
                    break;
                case MirrorB:
                    MoveThroughTiles(nextPoint, GetDirectionMirrorB(direction));
                    break;
                case VerticalSplitter:
                    if (direction == Direction.Up || direction == Direction.Down)
                    {
                        MoveThroughTiles(nextPoint, direction);
                    }
                    else
                    {
                        MoveThroughTiles(nextPoint, Direction.Up);
                        MoveThroughTiles(nextPoint, Direction.Down);
                    }
                    break;
                case HorizontalSplitter:
                    if (direction == Direction.Left || direction == Direction.Right)
                    {
                        MoveThroughTiles(nextPoint, direction);
                    }
                    else
                    {
                        MoveThroughTiles(nextPoint, Direction.Left);
                        MoveThroughTiles(nextPoint, Direction.Right);
                    }
                    break;
            }
        }
    }

    private static Point GetNextPoint(Point startPoint, Direction direction)
    {
        var diff = direction switch
        {
            Direction.Up => new Point(0, -1),
            Direction.Down => new Point(0, 1),
            Direction.Left => new Point(-1, 0),
            Direction.Right => new Point(1, 0),
            _ => throw new NotImplementedException()
        };

        return new Point(startPoint.X + diff.X, startPoint.Y + diff.Y);
    }

    private static Direction GetDirectionMirrorF(Direction direction)
    {
        return direction switch
        {
            Direction.Up => Direction.Right,
            Direction.Down => Direction.Left,
            Direction.Left => Direction.Down,
            Direction.Right => Direction.Up,
            _ => throw new NotImplementedException()
        };
    }

    private static Direction GetDirectionMirrorB(Direction direction)
    {
        return direction switch
        {
            Direction.Up => Direction.Left,
            Direction.Down => Direction.Right,
            Direction.Left => Direction.Up,
            Direction.Right => Direction.Down,
            _ => throw new NotImplementedException()
        };
    }

    private static void SetIsEnergized(Dictionary<Point, Tile> map, Point point, bool isEnergized, Direction? direction)
    {
        var tile = map[point];
        if (direction.HasValue)
        {
            tile.EnergizedFrom.Add(direction.Value);
        }
        else
        {
            tile.EnergizedFrom = new HashSet<Direction>();
        }
        map[point] = new Tile(point, tile.Type, isEnergized, tile.EnergizedFrom);
    }

    private static Dictionary<Point, Tile> ReadMap(IList<string?> inputLines)
    {
        Dictionary<Point, Tile> map = new();
        int y = 0;
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
                map.Add(coord, new Tile(coord, tile, false, new HashSet<Direction>()));
                x++;
            }
            y++;
        }

        return map;
    }
}
