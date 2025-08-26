namespace Poker;

public class Player
{
    public string name;
    public List<Card> cards = new(2);

    public Player(string name)
    {
        this.name = name;
    }
}