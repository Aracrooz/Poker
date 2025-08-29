namespace Poker;

public class Table
{
    private enum PlayerRole
    {
        None,
        Dealer,
        SmallBlind,
        BigBlind
    }

    private enum PlayerStatuses
    {
        Folded,
        Raised,
        ToCall,
        Checked,
        Called,
        AllIn
    }
    
    public List<Player> players = new(5);
    private Deck deck;
    public List<Card> cards = new(5);
    public int buyIn;
    public int smallBlind;
    public int dealerIndex;
    public int smallBlindIndex;
    public int bigBlindIndex;
    public int minRaise;
    public int toCall;
    Dictionary<Player, int> bets = new();
    Dictionary<Player, PlayerRole> playerRoles = new();
    Dictionary<Player, PlayerStatuses> playerStatuses = new();
    public Table(int buyIn)
    {
        deck = new Deck();
        this.buyIn = buyIn;
        smallBlind = buyIn/100/2;
        minRaise = smallBlind*2;
        toCall = smallBlind*2;
    }

    public void PlayRound()
    {
        ResetTable();
        if (players.Count < 2)
            return;
        Deal();
        ShowTableCards();
        Console.ReadLine();
        Flop();
        ShowTableCards();
        Console.ReadLine();
        Turn();
        ShowTableCards();
        Console.ReadLine();
        River();
        ShowTableCards();
        Console.ReadLine();
        
    }

    private void BettingRound()
    {
        for (var i = 0; i < players.Count; i++)
        {
            var player = players[i];
            if (playerStatuses[player] == PlayerStatuses.Folded || playerStatuses[player] == PlayerStatuses.AllIn)
                 continue;
            
            Console.WriteLine(player.name + ": ");
            var decision = Console.ReadLine();
            
            switch (decision)
            {
                case "check":
                    continue;
                case "fold":
                    playerStatuses[player] = PlayerStatuses.Folded;
                    continue;
                case "call":
                    if (player.tableBalance <= toCall)
                    {
                        bets[player] += player.tableBalance;
                        player.tableBalance = 0;
                        playerStatuses[player] = PlayerStatuses.AllIn;
                    }
                    else
                    {
                        bets[player] += toCall;
                        player.tableBalance -= toCall;
                        playerStatuses[player] = PlayerStatuses.Called;
                    }
                    continue;
                case "raise":
                    Console.WriteLine("Amount: ");
                    var input = Convert.ToInt32(Console.ReadLine());
                    
                    if (input > player.tableBalance)
                        continue;
                    var amount = input >= minRaise ? input : minRaise;
                    
                    bets[player] += amount;
                    player.tableBalance -= amount;
                    toCall = bets[player];
                    
                    if(player.tableBalance == 0)
                        playerStatuses[player] = PlayerStatuses.AllIn;
                    else
                        playerStatuses[player] = PlayerStatuses.Raised;
                    foreach (var p in players)
                    {
                        if (p != player && playerStatuses[p] != PlayerStatuses.Folded)
                        {
                            playerStatuses[p] = PlayerStatuses.ToCall;
                        }
                    }
                    continue;
                default:
                    continue;
            }
        }
    }

    public void Reindex()
    {
        dealerIndex = NextIndex(dealerIndex);
        if (players.Count == 2)
        {
            smallBlindIndex = dealerIndex;
            bigBlindIndex = NextIndex(dealerIndex);
        }
        else if (players.Count >= 3)
        {
            smallBlindIndex = NextIndex(dealerIndex);
            bigBlindIndex = NextIndex(smallBlindIndex);
        }
    }
    
    private int NextIndex(int currentIndex)
    {
        return (currentIndex + 1) % players.Count;
    }

    public void ResetTable()
    {
        Reindex();
        bets.Clear();
        playerRoles.Clear();
        foreach (var player in players)
        {
            bets.Add(player, 0);
        }
        for (var i = 0; i < players.Count; i++)
        {
            var player = players[i];
            playerRoles[player] =
                i == dealerIndex ? PlayerRole.Dealer :
                i == smallBlindIndex ? PlayerRole.SmallBlind :
                i == bigBlindIndex ? PlayerRole.BigBlind :
                PlayerRole.None;
        }
    }
    
    public void ShowTableCards()
    {
        Console.Clear();
        foreach (var player in players)
        {
            player.SeeCards();
            var score = CheckScore(player); 
            Console.WriteLine(WhatHand(score) + "\n" + score + "\n");
        }
        foreach (var card in cards)
        {
            Console.WriteLine(card.Value+" "+card.Suit);
        }
    }
    

    public void AddPlayer(Player player)
    {
        if (players.Count > 4)
            return;
        players.Add(player);
        bets.Add(player, 0);
    }
    
    public void Deal()
    {
        cards.Clear();
        deck.Shuffle();
        foreach (var player in players)
        {
            player.cards.Clear();
        }
        for (var i = 0; i < 2; i++)
        {
            foreach (var p in players)
            {
                p.cards.Add(deck.Draw());
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

    public string WhatHand(int x)
    {
        if (x >= 10000000)
            return "Royal flush";
        if (x >= 9000000)
            return "Straight flush";
        if (x >= 8000000)
            return "Four of a kind";
        if (x >= 7000000)
            return "Full house";
        if (x >= 6000000)
            return "Flush";
        if (x >= 5000000)
            return "Straight";
        if (x >= 4000000)
            return "Three of a kind";
        if (x >= 3000000)
            return "Two pair";
        if (x >= 2000000)
            return "Pair";
        return "High card";
    }
    
    public int CheckScore(Player player)
    {
        var hand = this.cards.Concat(player.cards).ToList();

        var x = IsRoyalFlush(hand);
        if (x!=0)
        {
            return x;
        }
        x = IsStraightFlush(hand);
        if (x!=0) 
        {
            return x;
        }
        x = IsFourOfAKind(hand);
        if (x!=0)
        {
            return x;
        }
        x = IsFullHouse(hand);
        if (x!=0)
        {
            return x;
        }
        x = IsFlush(hand);
        if (x!=0)
        {
            return x;
        }
        x = IsStraight(hand);
        if (x!=0)
        {
            return x;
        }
        x = IsThreeOfAKind(hand);
        if (x!=0)
        {
            return x;
        }
        x = IsTwoPair(hand);
        if (x!=0)
        {
            return x;
        }
        x = IsPair(hand);
        if (x!=0)
        {
            return x;
        }
        x=IsHighCard(hand);
        return x;
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
            return 5000005;
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