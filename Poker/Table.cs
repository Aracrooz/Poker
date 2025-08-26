using System.ComponentModel.Design;

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

        var x = IsRoyalFlush(hand);
        if (x!=0)
        {
            return "Royal flush";
        }
        x = IsStraightFlush(hand);
        if (x!=0) 
        {
            return "Straight flush";
        }
        x = IsFourOfAKind(hand);
        if (x!=0)
        {
            return "Four of a kind";
        }
        x = IsFullHouse(hand);
        if (x!=0)
        {
            return "Full house";
        }
        x = IsFlush(hand);
        if (x!=0)
        {
            return "Flush";
        }
        x = IsStraight(hand);
        if (x!=0)
        {
            return "Straight";
        }
        x = IsThreeOfAKind(hand);
        if (x!=0)
        {
            return "Three of a kind";
        }
        x = IsTwoPair(hand);
        if (x!=0)
        {
            return "Two pair";
        }
        x = IsPair(hand);
        if (x!=0)
        {
            return "Pair";
        }
        x=IsHighCard(hand);
        if (x != 0)
        {
            return "High card";
        }
        return "Error";
    }
    
    public int IsRoyalFlush(List<Card> hand)
    {
        var royalValues = new HashSet<int> { 10, 11, 12, 13, 14 };
        var groups = hand.GroupBy(c => c.Suit);
        foreach (var group in groups)
        {
            var values = new HashSet<int>(group.Select(c => c.Value));
            if (royalValues.IsSubsetOf(values))
            {
                return 10000000;
            }
        }
        return 0;
    }
    
    public int IsStraightFlush(List<Card> hand)
    {
        var groups = hand.GroupBy(c => c.Suit);
        foreach (var group in groups)
        {
            var x = IsStraight(group.ToList());
            if (x!=0) return 4000000 + x;
        }
        return 0;
    }
    
    public int IsFourOfAKind(List<Card> hand)
    {
        var groups = hand.GroupBy(c => c.Value);
        var four = groups.FirstOrDefault(g => g.Count() == 4);
        if (four == null)
            return 0;
        var fourValue = four.Key;
        var kicker = groups.Where(g => g.Count() != 4).Max(g => g.Key);
        return 8000000 + 50*fourValue + kicker;
    }
    
    public int IsFullHouse(List<Card> hand)
    {
        var groups = hand.GroupBy(c => c.Value);
        var threeValue = 0;
        var pairValue = 0;
        foreach (var group in groups)
        {
            if (group.Count() == 3)
            {
                if(group.Key>threeValue) 
                    threeValue=group.Key;
            }
            if (group.Count() >= 2)
            {
                if(group.Key>pairValue && group.Key!=threeValue) 
                    pairValue=group.Key;
            }
        }
        if (threeValue==0 || pairValue==0) 
            return 0;
        return 7000000 + 50*threeValue + pairValue;
    }

    public int IsFlush(List<Card> hand)
    {
        var flush = hand.GroupBy(c => c.Suit)
            .OrderByDescending(g => g.Count())
            .FirstOrDefault(g => g.Count() > 4);
        if (flush == null) return 0;
        return 6000000+IsHighCard(flush.ToList());
    }
    
    public int IsStraight(List<Card> hand)
    {
        var values = new SortedSet<int>(hand.Select(c => c.Value)).ToList();
        if (values.Count < 5)
            return 0;
        if (new[] { 14, 2, 3, 4, 5 }.All(values.Contains)) 
            return 5005;
        var inRow = 0;
        var highest = 0;
        for (var i = 1; i < values.Count; i++)
        {
            if (values[i - 1] + 1 == values[i])
                inRow++;
            else
                inRow = 0;
            
            if (inRow >= 4)
                highest= values[i];
        }
        if (highest > 0)
            return 5000000+highest;
        return 0;
    }
    
    public int IsThreeOfAKind(List<Card> hand)
    {
        var groups = hand.GroupBy(c => c.Value);
        var threeValue = 0;
        foreach (var group in groups)
        {
            if (group.Count() == 3 && group.Key>threeValue) threeValue = group.Key;
        }
        if (threeValue == 0)
            return 0;
        var kickers = hand.Where(g => g.Value != threeValue)
            .OrderByDescending(c => c.Value)
            .Take(2)
            .ToList();
        return 4000000+10000*threeValue+IsHighCard(kickers);
    }

    public int IsTwoPair(List<Card> hand)
    {
        var groups = hand.GroupBy(c => c.Value);
        groups = groups.OrderByDescending(g => g.Key).ToList();
        var pairHigh = 0;
        var pairLow = 0;
        foreach (var group in groups)
        {
            if (group.Count() == 2 && pairHigh == 0) pairHigh = group.Key;
            else if (group.Count() == 2) pairLow = group.Key;
        }
        if (pairHigh == 0 || pairLow == 0) return 0;
        return 3000000 + 10000 * pairHigh + 1000 * pairLow + IsHighCard(
            hand.Where(g => g.Value != pairHigh && g.Value != pairLow)
                .OrderByDescending(c => c.Value)
                .Take(1)
                .ToList()
        );
    }
    
    public int IsPair(List<Card> hand)
    {
        var groups = hand.GroupBy(c => c.Value);
        groups = groups.OrderByDescending(g => g.Key).ToList();
        var pairHigh = groups.FirstOrDefault(g => g.Count() == 2)?.Key ?? 0;
        if (pairHigh == 0) return 0;
        return 2000000 + 10000 * pairHigh + IsHighCard(
            hand.Where(g => g.Value != pairHigh)
                .OrderByDescending(c => c.Value)
                .Take(3)
                .ToList()
        );
    }
    
    public int IsHighCard(List<Card> hand)
    {
        var score = 0;
        var multiplier = 1;
        var orderedHand = hand.OrderByDescending(c => c.Value).Take(5);
        orderedHand = orderedHand.OrderBy(c => c.Value);
        foreach (var card in orderedHand)
        {
            score += card.Value * multiplier;
            multiplier *= 10;
        }
        return score;
    }
}