using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023;

internal class Day7 : IDay
{
    private const int FiveOfAKind = 1;
    private const int FourOfAKind = 2;
    private const int FullHouse = 3;
    private const int ThreeOfAKind = 4;
    private const int TwoPair = 5;
    private const int OnePair = 6;
    private const int HighCard = 7;

    private static readonly Dictionary<char, int> CardStrength = new Dictionary<char, int>
    {
        ['A'] = 1,
        ['K'] = 2,
        ['Q'] = 3,
        ['J'] = 4,
        ['T'] = 5,
        ['9'] = 6,
        ['8'] = 7,
        ['7'] = 8,
        ['6'] = 9,
        ['5'] = 10,
        ['4'] = 11,
        ['3'] = 12,
        ['2'] = 13,
    };

    private static readonly Dictionary<char, int> CardStrength2 = new Dictionary<char, int>
    {
        ['A'] = 1,
        ['K'] = 2,
        ['Q'] = 3,
        ['T'] = 4,
        ['9'] = 5,
        ['8'] = 6,
        ['7'] = 7,
        ['6'] = 8,
        ['5'] = 9,
        ['4'] = 10,
        ['3'] = 11,
        ['2'] = 12,
        ['J'] = 13,
    };

    private record struct HandInfo(string Hand, int Bid, int HandType) : IComparable<HandInfo>
    {
        public int CompareTo(HandInfo other)
        {
            if (this.HandType != other.HandType)
            {
                return this.HandType.CompareTo(other.HandType);
            }

            var cards = Hand.ToCharArray();
            var otherCards = other.Hand.ToCharArray();
            for (int cardIndex = 0 ; cardIndex < cards.Length; cardIndex++) 
            {
                var cardStrength = CardStrength[cards[cardIndex]];
                var otherCardStrength = CardStrength[otherCards[cardIndex]];
                if (cardStrength != otherCardStrength)
                {
                    return cardStrength.CompareTo(otherCardStrength);
                }
            }

            return 0;
        }
    }

    private record struct HandInfo2(string Hand, int Bid, int HandType) : IComparable<HandInfo2>
    {
        public int CompareTo(HandInfo2 other)
        {
            if (this.HandType != other.HandType)
            {
                return this.HandType.CompareTo(other.HandType);
            }

            var cards = Hand.ToCharArray();
            var otherCards = other.Hand.ToCharArray();
            for (int cardIndex = 0; cardIndex < cards.Length; cardIndex++)
            {
                var cardStrength = CardStrength2[cards[cardIndex]];
                var otherCardStrength = CardStrength2[otherCards[cardIndex]];
                if (cardStrength != otherCardStrength)
                {
                    return cardStrength.CompareTo(otherCardStrength);
                }
            }

            return 0;
        }
    }

    public void Solve(IList<string?> inputLines)
    {
        var handInfos = new List<HandInfo>();
        var handInfos2 = new List<HandInfo2>();
        foreach (var line in inputLines) 
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var hand = InputReader.ProcessStringLine(line);
            var handType = GetHandType(hand[0]);
            handInfos.Add(new HandInfo(hand[0], Convert.ToInt32(hand[1]), handType));

            var handType2 = GetHandType2(hand[0]);
            handInfos2.Add(new HandInfo2(hand[0], Convert.ToInt32(hand[1]), handType2));
        }

        // sort descending
        handInfos.Sort((x, y) => y.CompareTo(x));
        var sum = 0;
        var rank = 0;
        foreach (var handInfo in handInfos) 
        {
            rank++;
            sum += handInfo.Bid * rank;
        }

        Console.WriteLine($"{this.GetType().Name} result part 1: {sum}");

        // sort descending
        handInfos2.Sort((x, y) => y.CompareTo(x));
        sum = 0;
        rank = 0;
        foreach (var handInfo in handInfos2)
        {
            rank++;
            sum += handInfo.Bid * rank;
        }
        Console.WriteLine($"{this.GetType().Name} result part 2: {sum}");
    }

    private int GetHandType(string hand)
    {
        var handChars = hand.ToCharArray();
        var distinctChars = handChars.Distinct();
        var dict = new Dictionary<char, int>();
        foreach (var character in distinctChars)
        {
            dict[character] = handChars.Count(hc => hc == character);
        }

        if (dict.Count == 1)
        {
            return FiveOfAKind;
        }

        if (dict.Count == 2 && dict.Any(d => d.Value == 4))
        {
            return FourOfAKind;
        }

        if (dict.Count == 2 && dict.Any(d => d.Value == 3))
        {
            return FullHouse;
        }

        if (dict.Count == 3 && dict.Any(d => d.Value == 3))
        {
            return ThreeOfAKind;
        }

        if (dict.Count == 3 && dict.Any(d => d.Value == 2))
        {
            return TwoPair;
        }

        if (dict.Count == 4 && dict.Any(d => d.Value == 2))
        {
            return OnePair;
        }

        return HighCard;
    }

    private int GetHandType2(string hand)
    {
        var handChars = hand.ToCharArray();
        var distinctChars = handChars.Distinct();
        var dict = new Dictionary<char, int>();
        foreach (var character in distinctChars)
        {
            dict[character] = handChars.Count(hc => hc == character);
        }

        if (dict.Count == 1)
        {
            return FiveOfAKind;
        }

        if (dict.Count == 2 && dict.Any(d => d.Value == 4) && !dict.ContainsKey('J'))
        {
            return FourOfAKind;
        }
        else if (dict.Count == 2 && dict.Any(d => d.Value == 4) && dict.ContainsKey('J'))
        {
            return FiveOfAKind;
        }
        
        if (dict.Count == 2 && dict.Any(d => d.Value == 3) && !dict.ContainsKey('J'))
        {
            return FullHouse;
        }
        else if (dict.Count == 2 && dict.Any(d => d.Value == 3) && dict.ContainsKey('J'))
        {
            return FiveOfAKind;
        }

        if (dict.Count == 3 && dict.Any(d => d.Value == 3) && !dict.ContainsKey('J'))
        {
            return ThreeOfAKind;
        }
        else if (dict.Count == 3 && dict.Any(d => d.Value == 3) && dict.ContainsKey('J'))
        {
            return FourOfAKind;
        }

        if (dict.Count == 3 && dict.Any(d => d.Value == 2) && !dict.ContainsKey('J'))
        {
            return TwoPair;
        }
        else if (dict.Count == 3 && dict.Any(d => d.Value == 2) && dict.ContainsKey('J'))
        {
            if (dict['J'] == 2)
            {
                return FourOfAKind;
            }
            else
            {
                return FullHouse;
            }
        }

        if (dict.Count == 4 && dict.Any(d => d.Value == 2) && !dict.ContainsKey('J'))
        {
            return OnePair;
        }
        else if (dict.Count == 4 && dict.Any(d => d.Value == 2) && dict.ContainsKey('J'))
        {
            return ThreeOfAKind;
        }

        if (!dict.ContainsKey('J'))
        {
            return HighCard;
        }
        else
        {
            return OnePair;
        }
    }
}
