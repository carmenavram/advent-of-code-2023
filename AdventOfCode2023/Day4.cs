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
        var totalCards = 0;
        int cardIndex = 0;
        Dictionary<int, LineInfo> linesInfo = new();
        Dictionary<int, int> numberOfCards = new();
        foreach (var line in inputLines)
        {
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            cardIndex++;
            var lineWithoutCard = Regex.Replace(line, pattern, string.Empty);
            var numbers = InputReader.ProcessStringLine(lineWithoutCard, '|');
            var winningNumbers = numbers[0].Split(" ").Where(n => !string.IsNullOrWhiteSpace(n)).Select(n => Convert.ToInt32(n.Trim())).ToHashSet();
            var numbersYouHave = numbers[1].Split(" ").Where(n => !string.IsNullOrWhiteSpace(n)).Select(n => Convert.ToInt32(n.Trim())).ToList();
            linesInfo[cardIndex] = new LineInfo(winningNumbers, numbersYouHave);
        }

        foreach (var item in linesInfo) 
        {
            var lineInfo = item.Value;
            var cardNumber = item.Key;
            var matchingNumbers = new List<int>();
            foreach (var number in lineInfo.NumbersYouHave) 
            {
                if (lineInfo.WinningNumbers.Contains(number)) 
                {
                    matchingNumbers.Add(number);
                }
            }
            sum += CalculateCardPoints(matchingNumbers);
            AddCard(numberOfCards, cardNumber);
            for (int i = cardNumber + 1; i <= Math.Min(cardNumber + matchingNumbers.Count, cardIndex); i++)
            {
                for (var cardCopy = 0; cardCopy < numberOfCards[cardNumber]; cardCopy++)
                {
                    AddCard(numberOfCards, i);
                }
            }
        }

        foreach (var card in numberOfCards.Keys)
        {
            totalCards += numberOfCards[card];
        }

        Console.WriteLine($"{this.GetType().Name} result part 1: {sum}");
        Console.WriteLine($"{this.GetType().Name} result part 2: {totalCards}");

        static int CalculateCardPoints(List<int> matchingNumbers)
        {
            int sum = 0;
            if (matchingNumbers.Any())
            {
                sum = 1;
                if (matchingNumbers.Count > 1) 
                {
                    for (int i = 1; i < matchingNumbers.Count; i++)
                    {
                        sum *= 2;
                    }
                }
            }

            return sum;
        }

        static void AddCard(Dictionary<int, int> numberOfCards, int cardIndex)
        {
            if (numberOfCards.ContainsKey(cardIndex))
            {
                numberOfCards[cardIndex] += 1;
            }
            else
            {
                numberOfCards[cardIndex] = 1;
            }
        }
    }
}
