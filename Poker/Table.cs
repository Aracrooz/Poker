namespace Poker;

public class Table
{
    public List<Player> players = new(6);
    public List<Card> cards = new(5);
    public Deck deck;

    public Table()
    {
        deck = new Deck();
    }

    public void AddPlayer(Player player)
    {
        players.Add(player);
    }
    
    public void Deal()
    {
        for (var i = 0; i < 2; i++)
        {
            foreach (var t in players)
            {
                t.cards.Add(deck.Draw());
            }
        }
    }
    
    public void Flop()
    {
        for (var i = 0; i < 3; i++)
            cards.Add(deck.Draw());
    }
    
    public void Turn()
    {
        cards.Add(deck.Draw());
    }
    
    public void River()
    {
        cards.Add(deck.Draw());
    }
    
    public string CheckHand(Player player)
    {
        var hand = this.cards.Concat(player.cards).ToList();
        
        if (IsRoyalFlush(hand))
        {
            return "Royal flush";
        }
        if (IsStraightFlush(hand))
        {
            return "Straight flush";
        }
        if (IsFourOfAKind(hand))
        {
            return "Four of a kind";
        }
        if (IsFullHouse(hand))
        {
            return "Full house";
        }
        if (IsFlush(hand))
        {
            return "Flush";
        }
        if (IsStraight(hand))
        {
            return "Straight";
        }
        if (IsThreeOfAKind(hand))
        {
            return "Three of a kind";
        }
        if (IsTwoPair(hand))
        {
            return "Two pair";
        }
        if (IsPair(hand))
        {
            return "Pair";
        }
        return "High card";
    }
    
    public bool IsRoyalFlush(List<Card> hand)
    {
        var royalValues = new HashSet<int> { 10, 11, 12, 13, 14 };
        var groups = hand.GroupBy(c => c.Suit);
        foreach (var group in groups)
        {
            var values = new HashSet<int>(group.Select(c => c.Value));
            if (royalValues.IsSubsetOf(values))
            {
                return true;
            }
        }
        return false;
    }
    
    public bool IsStraightFlush(List<Card> hand)
    {
        var groups = hand.GroupBy(c => c.Suit);
        foreach (var group in groups)
        {
            if (IsStraight(group.ToList())) return true;
        }
        return false;
    }
    
    public bool IsFourOfAKind(List<Card> hand)
    {
        return hand.GroupBy(c => c.Value).Any(g => g.Count() == 4);
    }
    
    public bool IsFullHouse(List<Card> hand)
    {
        var groups = hand.GroupBy(c => c.Value);
        var threeCount = 0;
        var twoCount = 0;
        foreach (var group in groups)
        {
            if (group.Count() == 3) threeCount++;
            else if (group.Count() == 2) twoCount++;
        }
        return threeCount>0 && threeCount+twoCount>1;
    }
    
    public bool IsFlush(List<Card> hand)
    {
        return hand.GroupBy(c => c.Suit).Any(g => g.Count() > 4);
    }
    
    public bool IsStraight(List<Card> hand)
    {
        var values = new SortedSet<int>(hand.Select(c => c.Value)).ToList();
        if (values.Count < 5)
            return false;
        if (new[] { 14, 2, 3, 4, 5 }.All(values.Contains)) 
            return true;
        var inRow = 0;
        for (var i = 1; i < values.Count; i++)
        {
            if (values[i - 1] + 1 == values[i])
                inRow++;
            else
                inRow = 0;
        }
        return inRow == 4;
    }
    
    public bool IsThreeOfAKind(List<Card> hand)
    {
        var groups = hand.GroupBy(c => c.Value);
        foreach (var group in groups)
        {
            if (group.Count() == 3) return true;
        }
        return false;
    }
    
    public bool IsTwoPair(List<Card> hand)
    {
        var groups = hand.GroupBy(c => c.Value);
        var pairCount = 0;
        foreach (var group in groups)
        {
            if (group.Count() == 2) pairCount++;
        }
        return pairCount > 1;
    }
    
    public bool IsPair(List<Card> hand)
    {
        var groups = hand.GroupBy(c => c.Value);
        foreach (var group in groups)
        {
            if (group.Count() == 2) return true;
        }
        return false;
    }
}