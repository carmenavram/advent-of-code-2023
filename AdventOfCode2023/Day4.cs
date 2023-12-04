using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode2023;

internal class Day4 : IDay
{
    private const string pattern = @"Card\s+\d+:\s*";

    private record class LineInfo(HashSet<int> WinningNumbers, List<int> NumbersYouHave);

    public void Solve(IList<string?> inputLines)
    {
        var sum = 0;
        int cardIndex = 0;
        Dictionary<int, LineInfo> linesInfo = new();
        foreach (var line in inputLines)
        {
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            cardIndex++;
            var lineWithoutCard = Regex.Replace(line, pattern, string.Empty);
            var numbers = InputReader.ProcessStringLine(lineWithoutCard, '|');
            var winningNumbers = numbers[0].Split(" ").Select(n => Convert.ToInt32(n.Trim())).ToHashSet();
            var numbersYouHave = numbers[1].Split(" ").Select(n => Convert.ToInt32(n.Trim())).ToList();
            linesInfo[cardIndex] = new LineInfo(winningNumbers, numbersYouHave);
        }

        foreach (var lineInfo in linesInfo.Values) 
        {
            var matchingNumbers = new List<int>();
            foreach (var number in lineInfo.NumbersYouHave) 
            {
                if (lineInfo.WinningNumbers.Contains(number)) 
                {
                    matchingNumbers.Add(number);
                }
            }
            sum += CalculateCardPoints(matchingNumbers);
        }

        long sumGears = 0;

        Console.WriteLine($"{this.GetType().Name} result part 1: {sum}");
        Console.WriteLine($"{this.GetType().Name} result part 2: {sumGears}");

        static int CalculateCardPoints(List<int> matchingNumbers)
        {
            int sum = 0;
            if (matchingNumbers.Any())
            {
                sum = 1;
                if (matchingNumbers.Count > 1) 
                {
                    sum++;
                    for (int i = 2; i < matchingNumbers.Count; i++)
                    {
                        sum += i * 2;
                    }
                }
            }

            return sum;
        }
    }
}
