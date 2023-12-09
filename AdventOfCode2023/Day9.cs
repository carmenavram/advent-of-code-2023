namespace AdventOfCode2023;

internal class Day9 : IDay
{
    public void Solve(IList<string?> inputLines)
    {
        var histories = new List<List<int>>();
        foreach (var line in inputLines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var history = InputReader.GetNumbersFromLine(line).ToList();
            histories.Add(history);
        }

        long sum = 0;
        foreach (var history in histories)
        {
            sum += PredictNextItemInHistory(history);
        }

        Console.WriteLine($"{this.GetType().Name} result part 1: {sum}");

        // Part 2
        sum = 0;
        foreach (var history in histories)
        {
            sum += PredictFirstItemInHistory(history);
        }

        Console.WriteLine($"{this.GetType().Name} result part 2: {sum}");
    }

    private static long PredictNextItemInHistory(List<int> history)
    {
        var lastNumbers = new List<int>();
        var previousNumbers = new List<int>(history);
        lastNumbers.Add(history.Last());
        var newNumbers = new List<int>(history);
        while (newNumbers.Any(n => n != 0))
        {
            newNumbers.Clear();
            for (int i = 0; i < previousNumbers.Count - 1; i++)
            {
                newNumbers.Add(previousNumbers[i + 1] - previousNumbers[i]);
            }
            lastNumbers.Add(newNumbers.Last());
            previousNumbers = new List<int>(newNumbers);
        }

        return lastNumbers.Sum();
    }

    private static long PredictFirstItemInHistory(List<int> history)
    {
        var firstNumbers = new List<int>();
        var previousNumbers = new List<int>(history);
        firstNumbers.Add(history.First());
        var newNumbers = new List<int>(history);
        while (newNumbers.Any(n => n != 0))
        {
            newNumbers.Clear();
            for (int i = 0; i < previousNumbers.Count - 1; i++)
            {
                newNumbers.Add(previousNumbers[i + 1] - previousNumbers[i]);
            }
            firstNumbers.Add(newNumbers.First());
            previousNumbers = new List<int>(newNumbers);
        }

        long predict = 0;
        for (int i = 0; i < firstNumbers.Count; i++)
        {
            predict += i % 2 == 0 ? firstNumbers[i] : -firstNumbers[i];
        }
        return predict;
    }
}
