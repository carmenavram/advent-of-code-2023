using System.Collections.Generic;
using System.Collections.Immutable;

namespace AdventOfCode2023;

internal class Day5 : IDay
{
    private record struct Range(long Start, long End);

    public void Solve(IList<string?> inputLines)
    {
        var locations = new List<long>();
        var locations2 = new List<long>();
        List<Dictionary<long, long>> maps = new();

        var seedsString = inputLines.First()!.Replace("seeds: ", string.Empty);
        var seeds = InputReader.GetLongNumbersFromLine(seedsString);
        //long totalNumberOfSeeds2 = 0;
        //for (int i = 1; i < seeds.Length; i += 2)
        //{
        //    totalNumberOfSeeds2 += seeds[i];
        //}
        var seedsPart2 = new List<Range>();
        var mapsPart2 = new List<List<(Range Source, Range Destination)>>();
        var seed2Index = 0;
        for (int i = 0; i < seeds.Length - 1; i += 2)
        {
            seedsPart2.Add(new Range(seeds[i], seeds[i] + seeds[i + 1] - 1));
        }
        var newMap = false;
        Dictionary<long, long> map = new();
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
                map = new();
                maps.Add(map);
                map2 = new();
                mapsPart2.Add(map2);
                newMap = false;
            }

            var mapRanges = InputReader.GetLongNumbersFromLine(inputLines[i]);
            var destRangeStart = mapRanges[0];
            var sourceRangeStart = mapRanges[1];
            var rangeLength = mapRanges[2];

            // map only first and last
            map[sourceRangeStart] = destRangeStart;
            map[sourceRangeStart + rangeLength - 1] = destRangeStart + rangeLength - 1;

            map2.Add((new Range(sourceRangeStart, sourceRangeStart + rangeLength - 1), new Range(destRangeStart, destRangeStart + rangeLength - 1)));

            //for (int j = 0; j < rangeLength; j++)
            //{
            //    map[sourceRangeStart + j] = destRangeStart + j;
            //}
        }

        foreach (var seed in seeds)
        {
            var key = seed;
            foreach (var mapDict in maps)
            {
                var mapRanges = mapDict.Keys.ToArray();
                Array.Sort(mapRanges);
                if (mapDict.ContainsKey(key))
                {
                    key = mapDict[key];
                }
                else
                {
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
            locations.Add(key);
        }

        long locationMin2 = long.MaxValue;
        //foreach (var range in mapsPart2.Last())
        //{
        //    if (range.Destination.Start < locationMin2)
        //    {
        //        locationMin2 = range.Destination.Start;
        //    }
        //}

        //Console.WriteLine($"Location min from data: {}");

        foreach (var range in seedsPart2.Skip(3))
        {
        //var range2 = seedsPart2.Skip(3).Take(1).First();
            for (long s = range.Start; s <= range.End; s++)
            {
                var key = s;
                foreach (var map2List in mapsPart2)
                {
                    foreach (var map2Range in map2List)
                    {
                        if (key >= map2Range.Source.Start && key <= map2Range.Source.End)
                        {
                            var diff = key - map2Range.Source.Start;
                            key = map2Range.Destination.Start + diff;
                            break;
                        }
                    }
                }
                //foreach (var mapDict in maps)
                //{
                //    var mapRanges = mapDict.Keys.ToArray();
                //    Array.Sort(mapRanges);
                //    if (mapDict.ContainsKey(key))
                //    {
                //        key = mapDict[key];
                //    }
                //    else
                //    {
                //        for (int i = 0; i < mapRanges.Length; i++)
                //        {
                //            if (key > mapRanges[i] && i < mapRanges.Length - 1 && key < mapRanges[i + 1])
                //            {
                //                var diff = key - mapRanges[i];
                //                key = mapDict[mapRanges[i]] + diff;
                //                break;
                //            }
                //        }
                //    }
                //}

                if (key < locationMin2)
                {
                    locationMin2 = key;
                }
            }
        }

        Console.WriteLine($"{this.GetType().Name} result part 1: {locations.Min()}");
        Console.WriteLine($"{this.GetType().Name} result part 2: {locationMin2}");

        //static Dictionary<Range, long> InitializeMap(List<long> sources)
        //{
        //    Dictionary<long, long> map = new();
        //    foreach (var source in sources)
        //    {
        //        map[source] = source;
        //    }

        //    return map;
        //}
    }
}
