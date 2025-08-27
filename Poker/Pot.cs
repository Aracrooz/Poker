namespace Poker;

public class Pot : Table
{
    private int Value {get; set;}
    public Player Winner { get; set; }

    public Pot(int value)
    {
        Value = value;
    }
}