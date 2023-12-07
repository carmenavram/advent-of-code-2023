namespace AdventOfCode2023;

internal class Day7 : IDay
{
    private const int FiveOfAKind = 7;
    private const int FourOfAKind = 6;
    private const int FullHouse = 5;
    private const int ThreeOfAKind = 4;
    private const int TwoPair = 3;
    private const int OnePair = 2;
    private const int HighCard = 1;
    private const char J = 'J';

    private static readonly Dictionary<char, int> CardStrength = new Dictionary<char, int>
    {
        ['A'] = 13,
        ['K'] = 12,
        ['Q'] = 11,
        ['J'] = 10,
        ['T'] = 9,
        ['9'] = 8,
        ['8'] = 7,
        ['7'] = 6,
        ['6'] = 5,
        ['5'] = 4,
        ['4'] = 3,
        ['3'] = 2,
        ['2'] = 1,
    };

    private static readonly Dictionary<char, int> CardStrength2 = new Dictionary<char, int>
    {
        ['A'] = 13,
        ['K'] = 12,
        ['Q'] = 11,
        ['T'] = 10,
        ['9'] = 9,
        ['8'] = 8,
        ['7'] = 7,
        ['6'] = 6,
        ['5'] = 5,
        ['4'] = 4,
        ['3'] = 3,
        ['2'] = 2,
        ['J'] = 1,
    };

    private record struct Hand(char[] Cards, int Bid, int HandType) : IHand
    {
        public readonly int CompareTo(IHand? other) => Compare(this, other, CardStrength);
    }

    private record struct Hand2(char[] Cards, int Bid, int HandType) : IHand
    {
        public readonly int CompareTo(IHand? other) => Compare(this, other, CardStrength2);
    }

    public void Solve(IList<string?> inputLines)
    {
        var hands = new List<IHand>();
        var hands2 = new List<IHand>();
        foreach (var line in inputLines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var hand = InputReader.ProcessStringLine(line);
            var cards = hand[0].ToCharArray();
            hands.Add(new Hand(cards, Convert.ToInt32(hand[1]), GetHandType(cards)));
            hands2.Add(new Hand2(cards, Convert.ToInt32(hand[1]), GetHandType2(cards)));
        }

        Console.WriteLine($"{this.GetType().Name} result part 1: {CalculateSum(hands)}");
        Console.WriteLine($"{this.GetType().Name} result part 2: {CalculateSum(hands2)}");
    }

    private int CalculateSum(List<IHand> hands)
    {
        hands.Sort();
        var sum = 0;
        var rank = 0;
        foreach (var handInfo in hands)
        {
            rank++;
            sum += handInfo.Bid * rank;
        }

        return sum;
    }

    private static int Compare(IHand hand, IHand? other, Dictionary<char, int> cardStrength)
    {
        if (hand.HandType != other!.HandType)
        {
            return hand.HandType.CompareTo(other.HandType);
        }

        for (int cardIndex = 0; cardIndex < hand.Cards.Length; cardIndex++)
        {
            var currentCardStrength = cardStrength[hand.Cards[cardIndex]];
            var otherCardStrength = cardStrength[other.Cards[cardIndex]];
            if (currentCardStrength != otherCardStrength)
            {
                return currentCardStrength.CompareTo(otherCardStrength);
            }
        }

        return 0;
    }

    private int GetHandType(char[] cards)
    {
        var distinctCards = GetCardsDict(cards);
        return distinctCards.Count switch
        {
            1 => FiveOfAKind,
            2 => GetTypeForTwoDistinct(distinctCards),
            3 => GetTypeForThreeDistinct(distinctCards),
            4 => OnePair,
            _ => HighCard
        };
    }

    private static int GetTypeForTwoDistinct(Dictionary<char, int> distinctCards)
    {
        if (distinctCards.Any(d => d.Value == 4))
        {
            return FourOfAKind;
        }

        return FullHouse;
    }

    private static int GetTypeForThreeDistinct(Dictionary<char, int> distinctCards)
    {
        if (distinctCards.Any(d => d.Value == 3))
        {
            return ThreeOfAKind;
        }

        return TwoPair;
    }

    private int GetHandType2(char[] cards)
    {
        var distinctCards = GetCardsDict(cards);

        if (!distinctCards.ContainsKey(J))
        {
            return GetHandType(cards);
        }

        return distinctCards.Count switch
        {
            1 => FiveOfAKind,
            2 => FiveOfAKind,
            3 => GetTypeForThreeDistinctWithJ(distinctCards),
            4 => ThreeOfAKind,
            _ => OnePair
        };
    }

    private int GetTypeForThreeDistinctWithJ(Dictionary<char, int> distinctCards)
    {
        if (distinctCards.Any(d => d.Value == 3))
        {
            return FourOfAKind;
        }

        if (distinctCards.Any(d => d.Value == 2) && distinctCards[J] == 2)
        {
            return FourOfAKind;
        }

        return FullHouse;
    }

    private static Dictionary<char, int> GetCardsDict(char[] cards)
    {
        var distinctCards = cards.Distinct();
        var dict = new Dictionary<char, int>();
        foreach (var card in distinctCards)
        {
            dict[card] = cards.Count(c => c == card);
        }

        return dict;
    }
}
