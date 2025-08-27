namespace Poker;

public class Player
{
    public string name;
    public List<Card> cards;
    public int balance;

    public Player(string name)
    {
        this.name = name;
        this.cards = new List<Card>();
    }

    public void SeeCards()
    {
        Console.WriteLine(name);
        Console.WriteLine(cards[0].Value+" "+cards[0].Suit+"\n"+cards[1].Value+" "+cards[1].Suit);
    }
    
}