using System.Text;

namespace AdventOfCode2023;

internal class Day3 : IDay
{
    private const char Dot = '.';

    private record struct Point(int X, int Y);
    private record struct Number(int Value, int XLeft, int XRight, int Y);

    public void Solve(IList<string?> inputLines)
    {
        var sum = 0;
        var symbols = new Dictionary<Point, char>();
        var numbers = new List<Number>();
        int lineIndex = 0;
        foreach (var line in inputLines)
        {
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            lineIndex++;
            int columnIndex = 0;
            var sb = new StringBuilder(string.Empty);
            var leftColumnIndex = line.Length;
            foreach (var character in line)
            {
                columnIndex++;
                if (char.IsDigit(character))
                {
                    sb.Append(character);
                    if (columnIndex < leftColumnIndex)
                    {
                        leftColumnIndex = columnIndex;
                    }
                }
                else if (character == Dot && sb.Length > 0)
                {
                    leftColumnIndex = AddNumber(numbers, lineIndex, line, columnIndex, sb, leftColumnIndex);
                }
                else if (character != Dot)
                {
                    symbols.Add(new Point(columnIndex, lineIndex), character);
                    if (sb.Length > 0)
                    {
                        leftColumnIndex = AddNumber(numbers, lineIndex, line, columnIndex, sb, leftColumnIndex);
                    }
                }
            }
        }

        foreach (var number in numbers)
        {
            var adjacentPointsToCheck = new List<Point>()
            {
                new Point(number.XLeft - 1, number.Y),
                new Point(number.XLeft - 1, number.Y - 1),
                new Point(number.XLeft - 1, number.Y + 1),
                new Point(number.XRight + 1, number.Y),
                new Point(number.XRight + 1, number.Y - 1),
                new Point(number.XRight + 1, number.Y + 1),
            };

            for (int i = number.XLeft; i <= number.XRight; i++)
            {
                adjacentPointsToCheck.Add(new Point(i, number.Y - 1));
                adjacentPointsToCheck.Add(new Point(i, number.Y + 1));
            }

            var containsAdjacentSymbol = adjacentPointsToCheck.Any(p => symbols.ContainsKey(p));
            if (containsAdjacentSymbol)
            {
                sum += number.Value;
            }
        }

        Console.WriteLine($"{this.GetType().Name} result part 1: {sum}");
        Console.WriteLine($"{this.GetType().Name} result part 2: {null}");

        static int AddNumber(List<Number> numbers, int lineIndex, string? line, int columnIndex, StringBuilder sb, int leftColumnIndex)
        {
            numbers.Add(new Number(Convert.ToInt32(sb.ToString()), leftColumnIndex, columnIndex - 1, lineIndex));
            sb.Clear();
            leftColumnIndex = line.Length;
            return leftColumnIndex;
        }
    }
}
