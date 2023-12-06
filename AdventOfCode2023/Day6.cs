namespace AdventOfCode2023;

internal class Day6 : IDay
{
    private record struct Race(long time, long distance);

    public void Solve(IList<string?> inputLines)
    {
        var timesInput = InputReader.GetNumbersFromLineWithStartText("Time:", inputLines[0]!);
        var distancesInput = InputReader.GetNumbersFromLineWithStartText("Distance:", inputLines[1]!);

        var races = new List<Race>();
        for (int i = 0; i < timesInput.Length; i++)
        {
            races.Add(new Race(timesInput[i], distancesInput[i]));
        }

        var result = 1;
        foreach (var race in races)
        {
            var numberOfRecordsBeaten = GetNumberOfRecordsBeaten(race);
            result = numberOfRecordsBeaten > 0 ? result * numberOfRecordsBeaten : result;
        }

        Console.WriteLine($"{this.GetType().Name} result part 1: {result}");

        // part 2
        var raceTime = Convert.ToInt64(string.Join(string.Empty, timesInput));
        var raceDistance = Convert.ToInt64(string.Join(string.Empty, distancesInput));
        var race2 = new Race(raceTime, raceDistance);
        var numberOfRecordsBeaten2 = GetNumberOfRecordsBeaten(race2);

        Console.WriteLine($"{this.GetType().Name} result part 2: {numberOfRecordsBeaten2}");

        static long CalculateDistance(long timeButtonPress, long totalTime) => (totalTime - timeButtonPress) * timeButtonPress;

        static int GetNumberOfRecordsBeaten(Race race)
        {
            var numberOfRecordsBeaten = 0;
            for (int i = 1; i < race.time - 1; i++)
            {
                var calculatedDistance = CalculateDistance(i, race.time);
                if (calculatedDistance > race.distance)
                {
                    numberOfRecordsBeaten++;
                }
            }

            return numberOfRecordsBeaten;
        }
    }
}
