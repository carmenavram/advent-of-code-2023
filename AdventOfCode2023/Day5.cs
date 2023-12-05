namespace AdventOfCode2023;

internal class Day5 : IDay
{
    public void Solve(IList<string?> inputLines)
    {
        var locations = new List<long>();
        List<Dictionary<long, long>> maps = new();

        var seedsString = inputLines.First()!.Replace("seeds: ", string.Empty);
        var seeds = InputReader.GetLongNumbersFromLine(seedsString);
        var sources = seeds.ToList();
        var newMap = false;
        Dictionary<long, long> map = new();
        for (int i = 1; i < inputLines.Count; i++)
        {
            if (string.IsNullOrEmpty(inputLines[i]))
            {
                i++;
                newMap = true;
                //sources = maps.LastOrDefault()?.Values?.ToList() ?? sources;
                continue;
            }

            if (newMap)
            {
                map = new();
                maps.Add(map);
                newMap = false;
            }

            var mapRanges = InputReader.GetLongNumbersFromLine(inputLines[i]);
            var destRangeStart = mapRanges[0];
            var sourceRangeStart = mapRanges[1];
            var rangeLength = mapRanges[2];

            for (int j = 0; j < rangeLength; j++)
            {
                map[sourceRangeStart + j] = destRangeStart + j;
            }
        }

        foreach (var seed in seeds)
        {
            var key = seed;
            foreach (var mapDict in maps)
            {
                if (mapDict.ContainsKey(key))
                {
                    key = mapDict[key];
                }
            }
            locations.Add(key);
        }

        Console.WriteLine($"{this.GetType().Name} result part 1: {locations.Min()}");
        Console.WriteLine($"{this.GetType().Name} result part 2: {null}");

        static Dictionary<long, long> InitializeMap(List<long> sources)
        {
            Dictionary<long, long> map = new();
            foreach (var source in sources)
            {
                map[source] = source;
            }

            return map;
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
