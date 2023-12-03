using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023;

internal class Day1 : IDay
{
    private const string One = "one";
    private const string Two = "two";
    private const string Three = "three";
    private const string Four = "four";
    private const string Five = "five";
    private const string Six = "six";
    private const string Seven = "seven";
    private const string Eight = "eight";
    private const string Nine = "nine";

    private const string DigitOne = "1";
    private const string DigitTwo = "2";
    private const string DigitThree = "3";
    private const string DigitFour = "4";
    private const string DigitFive = "5";
    private const string DigitSix = "6";
    private const string DigitSeven = "7";
    private const string DigitEight = "8";
    private const string DigitNine = "9";

    private static readonly Dictionary<string, int> Numbers = new Dictionary<string, int>
    {
        [One] = 1,
        [Two] = 2,
        [Three] = 3,
        [Four] = 4,
        [Five] = 5,
        [Six] = 6,
        [Seven] = 7,
        [Eight] = 8,
        [Nine] = 9,
        [DigitOne] = 1,
        [DigitTwo] = 2,
        [DigitThree] = 3,
        [DigitFour] = 4,
        [DigitFive] = 5,
        [DigitSix] = 6,
        [DigitSeven] = 7,
        [DigitEight] = 8,
        [DigitNine] = 9,
    };

    public void Solve(IList<string?> inputLines)
    {
        long sum = 0;
        long sumNamed = 0;
        foreach (var line in inputLines) 
        {
            if (line is not null)
            {
                int firstDigit = line.First(c => char.IsDigit(c)) - '0';
                int lastDigit = line.Last(c => char.IsDigit(c)) - '0';
                sum += GetTwoDigitNumber(firstDigit, lastDigit);
                (var firstNamedDigit, var lastNamedDigit) = GetFirstAndLastNamedDigits(line);
                sumNamed += GetTwoDigitNumber(firstNamedDigit, lastNamedDigit);
            }
        }

        Console.WriteLine($"{this.GetType().Name} result part 1: {sum}");
        Console.WriteLine($"{this.GetType().Name} result part 2: {sumNamed}");

        static (int, int) GetFirstAndLastNamedDigits(string line)
        {
            int firstDigit = 0;
            int lastDigit = 0;
            int firstIndex = line.Length;
            int lastIndex = - 1;
            foreach (var namedDigit in Numbers.Keys) 
            {
                var namedDigitIndexes = AllIndexesOf(line, namedDigit);
                foreach (var namedDigitIndex in namedDigitIndexes)
                {
                    if (namedDigitIndex >= 0 && namedDigitIndex < firstIndex)
                    {
                        firstIndex = namedDigitIndex;
                        firstDigit = Numbers[namedDigit];
                    }
                    if (namedDigitIndex >= 0 && namedDigitIndex > lastIndex)
                    {
                        lastIndex = namedDigitIndex;
                        lastDigit = Numbers[namedDigit];
                    }
                }
            }

            return (firstDigit, lastDigit);

            static List<int> AllIndexesOf(string line, string value)
            {
                List<int> indexes = new List<int>();
                for (int index = 0; ; index += value.Length)
                {
                    index = line.IndexOf(value, index, StringComparison.OrdinalIgnoreCase);
                    if (index == -1)
                    {
                        return indexes;
                    }
                    indexes.Add(index);
                }
            }
        }

        static int GetTwoDigitNumber(int firstDigit, int lastDigit) => firstDigit * 10 + lastDigit;
    } 
}
