namespace AdventOfCode2023;

public interface IHand : IComparable<IHand>
{
    int Bid { get; set; }
    char[] Cards { get; set; }
    int HandType { get; set; }
}