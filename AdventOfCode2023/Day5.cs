namespace AdventOfCode2023;

internal class Day5 : IDay
{
    private record struct Range(long Start, long End);

    public void Solve(IList<string?> inputLines)
    {
        SolvePart1(inputLines);
        SolvePart2(inputLines);
    }

    private void SolvePart1(IList<string?> inputLines)
    {
        List<Dictionary<long, long>> maps = new();

        var seedsString = inputLines.First()!.Replace("seeds: ", string.Empty);
        var seeds = InputReader.GetLongNumbersFromLine(seedsString);

        var newMap = false;
        Dictionary<long, long> map = new();
        for (int i = 1; i < inputLines.Count; i++)
        {
            if (string.IsNullOrEmpty(inputLines[i]))
            {
                i++;
                newMap = true;
                continue;
            }

            if (newMap)
            {
                map = new();
                maps.Add(map);
                newMap = false;
            }

            var mapRanges = InputReader.GetLongNumbersFromLine(inputLines[i]!);
            var destRangeStart = mapRanges[0];
            var sourceRangeStart = mapRanges[1];
            var rangeLength = mapRanges[2];

            // map only first and last
            map[sourceRangeStart] = destRangeStart;
            map[sourceRangeStart + rangeLength - 1] = destRangeStart + rangeLength - 1;
        }

        var locationMin = long.MaxValue;
        foreach (var seed in seeds)
        {
            var key = seed;
            foreach (var mapDict in maps)
            {
                var mapRanges = mapDict.Keys.ToArray();
                if (mapDict.ContainsKey(key))
                {
                    key = mapDict[key];
                }
                else
                {
                    Array.Sort(mapRanges);
                    for (int i = 0; i < mapRanges.Length; i++)
                    {
                        if (key > mapRanges[i] && i < mapRanges.Length - 1 && key < mapRanges[i + 1])
                        {
                            var diff = key - mapRanges[i];
                            key = mapDict[mapRanges[i]] + diff;
                            break;
                        }
                    }
                }
            }

            if (key < locationMin)
            {
                locationMin = key;
            }
        }

        Console.WriteLine($"{this.GetType().Name} result part 1: {locationMin}");
    }

    private void SolvePart2(IList<string?> inputLines)
    {
        var seedsString = inputLines.First()!.Replace("seeds: ", string.Empty);
        var seeds = InputReader.GetLongNumbersFromLine(seedsString);
        var seedsPart2 = new List<Range>();
        var maps = new List<List<(Range Source, Range Destination)>>();
        for (int i = 0; i < seeds.Length - 1; i += 2)
        {
            seedsPart2.Add(new Range(seeds[i], seeds[i] + seeds[i + 1] - 1));
        }
        var newMap = false;
        List<(Range Source, Range Destination)> map2 = new();
        for (int i = 1; i < inputLines.Count; i++)
        {
            if (string.IsNullOrEmpty(inputLines[i]))
            {
                i++;
                newMap = true;
                continue;
            }

            if (newMap)
            {
                map2 = new();
                maps.Add(map2);
                newMap = false;
            }

            var mapRanges = InputReader.GetLongNumbersFromLine(inputLines[i]!);
            var destRangeStart = mapRanges[0];
            var sourceRangeStart = mapRanges[1];
            var rangeLength = mapRanges[2];

            map2.Add((new Range(sourceRangeStart, sourceRangeStart + rangeLength - 1), new Range(destRangeStart, destRangeStart + rangeLength - 1)));
        }

        var initialMap = maps.First();
        var initialSelected = seedsPart2;
        List<(Range Source, Range Destination)> resultMap = new();
        foreach (var map in maps)
        {
            resultMap = GetSelectedMap(initialSelected, map);
            initialSelected = resultMap.Select(r => r.Destination).ToList();
        }

        var locationMin2 = resultMap.Min(r => r.Destination.Start);
        Console.WriteLine($"{this.GetType().Name} result part 2: {locationMin2}");

        static List<(Range Source, Range Destination)> GetSelectedMap(List<Range> selectedSourceRanges, List<(Range Source, Range Destination)> existingMap)
        {
            List<(Range Source, Range Destination)> selectedSeedsToSoilMap = new();
            foreach (var seedRange in selectedSourceRanges.OrderBy(s => s.Start))
            {
                (Range Source, Range Destination) selectedSeedsToSoilMapRange = (seedRange, seedRange);
                var selectedSourceRange = seedRange;
                var found = false;
                foreach (var seedToSoilRange in existingMap.OrderBy(s => s.Source.Start))
                {
                    if (selectedSourceRange.Start >= seedToSoilRange.Source.Start && selectedSourceRange.End <= seedToSoilRange.Source.End)
                    {
                        var destinationRangeStart = seedToSoilRange.Destination.Start + selectedSourceRange.Start - seedToSoilRange.Source.Start;
                        var destinationRangeEnd = seedToSoilRange.Destination.Start + selectedSourceRange.End - seedToSoilRange.Source.Start;
                        selectedSeedsToSoilMapRange = (selectedSourceRange, new Range(destinationRangeStart, destinationRangeEnd));
                        selectedSeedsToSoilMap.Add(selectedSeedsToSoilMapRange);
                        found = true;
                        break;
                    }
                    else if (selectedSourceRange.Start < seedToSoilRange.Source.Start && selectedSourceRange.End >= seedToSoilRange.Source.Start)
                    {
                        // get range before map Range start. In this case source range will be equal to destination range
                        var destinationRangeStart = selectedSourceRange.Start;
                        var destinationRangeEnd = seedToSoilRange.Source.Start - 1;
                        selectedSeedsToSoilMap.Add((new Range(destinationRangeStart, destinationRangeEnd), new Range(destinationRangeStart, destinationRangeEnd)));

                        // get range from map Range start to seedRange end
                        var sourceRangeStart = seedToSoilRange.Source.Start;
                        var sourceRangeEnd = Math.Min(selectedSourceRange.End, seedToSoilRange.Source.End);
                        destinationRangeStart = seedToSoilRange.Destination.Start + sourceRangeStart - seedToSoilRange.Source.Start;
                        destinationRangeEnd = seedToSoilRange.Destination.Start + sourceRangeEnd - seedToSoilRange.Source.Start;
                        selectedSeedsToSoilMapRange = (new Range(sourceRangeStart, sourceRangeEnd), new Range(destinationRangeStart, destinationRangeEnd));
                        selectedSeedsToSoilMap.Add(selectedSeedsToSoilMapRange);
                        found = true;

                        selectedSourceRange = new Range(sourceRangeEnd + 1, selectedSourceRange.End);
                    }
                    else if (selectedSourceRange.Start < seedToSoilRange.Source.End && selectedSourceRange.Start >= seedToSoilRange.Source.Start && selectedSourceRange.End > seedToSoilRange.Source.End)
                    {
                        var destinationRangeStart = seedToSoilRange.Destination.Start + selectedSourceRange.Start - seedToSoilRange.Source.Start;
                        var destinationRangeEnd = seedToSoilRange.Destination.End;
                        selectedSeedsToSoilMapRange = (new Range(selectedSourceRange.Start, seedToSoilRange.Source.End), new Range(destinationRangeStart, destinationRangeEnd));
                        selectedSeedsToSoilMap.Add(selectedSeedsToSoilMapRange);
                        found = true;
                        selectedSourceRange = new Range(seedToSoilRange.Source.End + 1, selectedSourceRange.End);
                    }
                }

                if (!found)
                {
                    selectedSeedsToSoilMap.Add(selectedSeedsToSoilMapRange);
                }
                else if (selectedSeedsToSoilMapRange.Source.End < seedRange.End)
                {
                    var destinationRangeStart = selectedSeedsToSoilMapRange.Source.End + 1;
                    var destinationRangeEnd = seedRange.End;
                    selectedSeedsToSoilMap.Add((new Range(destinationRangeStart, destinationRangeEnd), new Range(destinationRangeStart, destinationRangeEnd)));
                }
            }

            return selectedSeedsToSoilMap;
        }
    }
}
