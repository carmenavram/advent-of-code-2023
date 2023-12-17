namespace AdventOfCode2023;

internal class Day11 : IDay
{
    private enum Direction
    {
        Up,
        Down,
        Left,
        Right
    };

    private static readonly IDictionary<Direction, Point> Diffs = new Dictionary<Direction, Point>()
    {
        [Direction.Up] = new Point(0, -1),
        [Direction.Down] = new Point(0, 1),
        [Direction.Left] = new Point(-1, 0),
        [Direction.Right] = new Point(1, 0),
    };

    private const char EmptySpace = '.';
    private const char Galaxy = '#';

    private record struct Point(int X, int Y) : IComparable<Point>
    {
        public int CompareTo(Point other)
        {
            if (X != other.X)
            {
                return X.CompareTo(other.X);
            }
            if (Y != other.Y)
            {
                return Y.CompareTo(other.Y);
            }
            return 0;
        }
    }

    private record struct Tile(char Type, int GalaxyIndex, Dictionary<Direction, int> weightsPerDirection);

    public void Solve(IList<string?> inputLines)
    {
        SolvePart(inputLines, 1);
        SolvePart(inputLines, 2);
    }

    private void SolvePart(IList<string?> inputLines, int part)
    {
        var expand = part == 1 ? 2 : 1000000;
        var map = ReadMap(inputLines, expand);
        var yLength = map.Max(m => m.Key.Y);
        var xLength = map.Max(m => m.Key.X);
        var galaxyPoints = map.Where(m => m.Value.GalaxyIndex > 0).ToDictionary(m => m.Value.GalaxyIndex, m => m.Key);

        long sum = 0;
        for (int i = 1; i <= galaxyPoints.Count; i++)
        {
            var paths = ShortestPaths(galaxyPoints[i], map);
            for (int j = i + 1; j <= galaxyPoints.Count; j++)
            {
                sum += paths[galaxyPoints[j]].Sum(p => p.Weight);
            }
        }

        Console.WriteLine($"{this.GetType().Name} result part {part}: {sum}");
    }

    private Dictionary<Point, List<(Point Coord, long Weight)>> ShortestPaths(Point start, Dictionary<Point, Tile> map)
    {
        var heap = new SortedSet<(long Weight, Point Coord)>();
        var distances = new Dictionary<Point, long>();
        var paths = new Dictionary<Point, List<(Point Coord, long Weight)>>();

        heap.Add((0, start));
        distances[start] = 0;
        paths[start] = new List<(Point Coord, long Weight)>();

        while (heap.Count > 0)
        {
            var node = heap.Min;
            heap.Remove(node!);

            Point currentNode = node.Coord;
            foreach (var neighbor in GetNeighbours(currentNode, map))
            {
                long oldWeight;
                distances.TryGetValue(neighbor.Coord, out oldWeight);
                var newWeight = node.Weight + neighbor.Weight;

                if (!distances.ContainsKey(neighbor.Coord) || newWeight < oldWeight)
                {
                    if (distances.ContainsKey(neighbor.Coord))
                    {
                        heap.Remove((oldWeight, neighbor.Coord));
                    }

                    distances[neighbor.Coord] = newWeight;
                    heap.Add((newWeight, neighbor.Coord));

                    var path = new List<(Point Coord, long Weight)>(paths[currentNode]);
                    path.Add(neighbor);
                    paths[neighbor.Coord] = path;
                }
            }
        }

        return paths;
    }

    private List<(Point Coord, long Weight)> GetNeighbours(Point currentPoint, Dictionary<Point, Tile> map)
    {
        var neighbours = new List<(Point Coord, long Weight)>();
        foreach (var diff in Diffs)
        {
            var neigbourCoord = new Point(currentPoint.X + diff.Value.X, currentPoint.Y + diff.Value.Y);
            if (map.ContainsKey(neigbourCoord))
            {
                neighbours.Add((neigbourCoord, map[neigbourCoord].weightsPerDirection[ReverseDirection(diff.Key)]));
            }
        }

        return neighbours;
    }

    private static Direction ReverseDirection(Direction direction)
    {
        return direction switch
        {
            Direction.Up => Direction.Down,
            Direction.Down => Direction.Up,
            Direction.Left => Direction.Right,
            Direction.Right => Direction.Left,
            _ => throw new NotImplementedException(direction.ToString())
        };
    }

    private static Dictionary<Point, Tile> ReadMap(IList<string?> inputLines, int expand)
    {
        Dictionary<Point, Tile> map = new();
        int x = 0;
        int y = 0;
        int galaxyIndex = 1;
        var previousLineIsEmpty = false;
        var defaultWeightsPerDirection = new Dictionary<Direction, int>();
        foreach (var direction in Diffs.Keys)
        {
            defaultWeightsPerDirection[direction] = 1;
        }
        foreach (var line in inputLines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            x = 0;
            var tiles = line.ToCharArray();
            var lineIsEmpty = !tiles.Contains(Galaxy);
            foreach (var tile in tiles)
            {
                var coord = new Point(x, y);
                var weightsPerDirection = new Dictionary<Direction, int>(defaultWeightsPerDirection);
                if (lineIsEmpty)
                {
                    weightsPerDirection[Direction.Down] = expand;
                }
                if (previousLineIsEmpty)
                {
                    weightsPerDirection[Direction.Up] = expand;
                }
                map.Add(coord, new Tile(tile, tile == Galaxy ? galaxyIndex++ : 0, weightsPerDirection));
                x++;
            }
            previousLineIsEmpty = lineIsEmpty;
            y++;
        }

        var previousColumnIsEmpty = false;
        for (int i = 0; i < x; i++)
        {
            var isEmptyColumn = !map.Any(m => m.Key.X == i && m.Value.Type == Galaxy);
            if (isEmptyColumn)
            {
                for (int j = 0; j < y; j++)
                {
                    var coord = new Point(i, j);
                    var weightsPerDirection = map[coord].weightsPerDirection;
                    weightsPerDirection[Direction.Right] = expand;
                    map[coord] = new Tile(map[coord].Type, map[coord].GalaxyIndex, weightsPerDirection);
                }
            }
            if (previousColumnIsEmpty)
            {
                for (int j = 0; j < y; j++)
                {
                    var coord = new Point(i, j);
                    var weightsPerDirection = map[coord].weightsPerDirection;
                    weightsPerDirection[Direction.Left] = expand;
                    map[coord] = new Tile(map[coord].Type, map[coord].GalaxyIndex, weightsPerDirection);
                }
            }
            previousColumnIsEmpty = isEmptyColumn;
        }

        return map;
    }
}
