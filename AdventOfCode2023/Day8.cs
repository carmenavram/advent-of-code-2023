namespace AdventOfCode2023;

internal class Day8 : IDay
{
    private const char Left = 'L';
    private const char Right = 'R';
    private record struct LeftRight(string Left, string Right);

    public void Solve(IList<string?> inputLines)
    {
        SolvePart1(inputLines);
        SolvePart2(inputLines);
    }

    private void SolvePart1(IList<string?> inputLines)
    {
        (var instructions, var maps) = ReadInput(inputLines);

        var steps = 0;
        var source = "AAA";
        var destination = string.Empty;
        var instructionIndex = 0;
        while (destination != "ZZZ")
        {
            steps++;
            if (instructionIndex == instructions.Length)
            {
                instructionIndex = 0;
            }

            var instruction = instructions[instructionIndex++];
            destination = (instruction == Left) ? maps[source].Left : maps[source].Right;
            source = destination;
        }

        Console.WriteLine($"{this.GetType().Name} result part 1: {steps}");
    }

    private void SolvePart2(IList<string?> inputLines)
    {
        (var instructions, var maps) = ReadInput(inputLines);

        var sources = maps.Keys.Where(k => k.EndsWith('A')).ToList();
        var stepsPerSource = new List<long>();
        foreach (var source in sources)
        {
            long stepIndex = 0;
            var instructionIndex = 0;
            var sourceKey = source;
            var destination = string.Empty;

            while (!destination.EndsWith('Z'))
            {
                stepIndex++;
                if (instructionIndex == instructions.Length)
                {
                    instructionIndex = 0;
                }

                var instruction = instructions[instructionIndex++];
                destination = (instruction == Left) ? maps[sourceKey].Left : maps[sourceKey].Right;
                sourceKey = destination;
            }
            stepsPerSource.Add(stepIndex);
        }

        long steps = stepsPerSource[0];
        for (int i = 1; i < stepsPerSource.Count; i++)
        {
            steps = LeastCommonMultiple(steps, stepsPerSource[i]);
        }

        Console.WriteLine($"{this.GetType().Name} result part 2: {steps}");
    }

    private static (char[] Instructions, Dictionary<string, LeftRight> Maps) ReadInput(IList<string?> inputLines)
    {
        Dictionary<string, LeftRight> maps;
        var instructions = inputLines[0]!.ToCharArray();
        maps = new();
        foreach (var line in inputLines.Skip(2))
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var strings = InputReader.ProcessStringLineString(line, " = ");
            var sourceString = strings[0];
            var pairs = InputReader.ProcessStringLineString(strings[1], ", ");
            maps.Add(sourceString, new LeftRight(pairs[0].Trim('('), pairs[1].Trim(')')));
        }

        return (instructions, maps);
    }

    private static long LeastCommonMultiple(long a, long b)
    {
        var num1 = Math.Max(a, b);
        var num2 = Math.Min(a, b);
        for (long i = 1; i <= num2; i++)
        {
            if ((num1 * i) % num2 == 0)
            {
                return i * num1;
            }
        }

        return num2;
    }
}
