
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
    private record struct Tile(Point Coord, char Type, bool IsEnergized);

    private Dictionary<Point, Tile> map = new Dictionary<Point, Tile>();
    private Dictionary<int, List<Point>> beams = new Dictionary<int, List<Point>>();
    private int beamIndex = 0;

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
        beams = new Dictionary<int, List<Point>>();
        beamIndex = 1;
        beams[beamIndex] = new List<Point>();
       
        MoveThroughTiles(startPoint, direction);

        HashSet<Point> points = new HashSet<Point>();
        for (var i = 1; i <= beams.Count; i++)
        {
            foreach (var beam in beams[i])
            {
                points.Add(beam);
            }
        }

        Console.WriteLine();
        var startPointText = $"StartPoint: {startPoint.X} {startPoint.Y}";
        Console.WriteLine($"{startPointText} Distinct Beams count: {points.Count}");
        Console.WriteLine($"{startPointText} Energized tiles: {map.Count(m => m.Value.IsEnergized)}");
        Console.WriteLine($"{startPointText} Energized Points not in the beam {string.Join(", ", map.Where(m => m.Value.IsEnergized && !points.Contains(m.Key)))}");

        if (maxTiles < points.Count)
        {
            maxTiles = points.Count;
        }

        return maxTiles;
    }

    private void MoveThroughTiles(Point startPoint, Direction direction)
    {
        var nextPoint = GetNextPoint(startPoint, direction);
        if (map.ContainsKey(nextPoint) && !BeamContains(startPoint, direction)) 
        {
            SetIsEnergized(map, nextPoint, true);
            beams[beamIndex].Add(nextPoint);
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
                        beams[++beamIndex] = new List<Point>() { nextPoint };
                        MoveThroughTiles(nextPoint, Direction.Up);
                        beams[++beamIndex] = new List<Point>() { nextPoint };
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
                        beams[++beamIndex] = new List<Point>() { nextPoint };
                        MoveThroughTiles(nextPoint, Direction.Left);
                        beams[++beamIndex] = new List<Point>() { nextPoint };
                        MoveThroughTiles(nextPoint, Direction.Right);
                    }
                    break;
            }
        }
    }

    private bool BeamContains(Point point, Direction direction)
    {
        var nextPoint = GetNextPoint(point, direction);
        foreach (var beam in beams) 
        {
            var index = beam.Value.IndexOf(point);
            if (index > 0 && index + 1 < beam.Value.Count)
            {
                var nextPointInBeam = beam.Value[index + 1];
                if (nextPoint == nextPointInBeam)
                {
                    return true;
                }
            }
        }

        return false;
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

    private static void SetIsEnergized(Dictionary<Point, Tile> map, Point point, bool isEnergized)
    {
        var tile = map[point];
        map[point] = new Tile(point, tile.Type, isEnergized);
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
                map.Add(coord, new Tile(coord, tile, false));
                x++;
            }
            y++;
        }

        return map;
    }
}
