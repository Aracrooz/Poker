// See https://aka.ms/new-console-template for more information

using Poker;

Table table = new Table();
table.AddPlayer(new Player("Marek"));
table.AddPlayer(new Player("Szparek"));
table.Deal();
Console.WriteLine(table.players[0].name);
Console.WriteLine(table.players[0].cards[0].Value+" "+table.players[0].cards[0].Suit);
Console.WriteLine(table.players[0].cards[1].Value+" "+table.players[0].cards[1].Suit);
Console.WriteLine(table.players[1].name);
Console.WriteLine(table.players[1].cards[0].Value+" "+table.players[1].cards[0].Suit);
Console.WriteLine(table.players[1].cards[1].Value+" "+table.players[1].cards[1].Suit);
Console.WriteLine(table.CheckHand(table.players[0]));
Console.WriteLine(table.CheckHand(table.players[1]));
Console.WriteLine("Table");
table.Flop();
for (int i = 0; i < 3; i++)
{
    Console.WriteLine(table.cards[i].Value+" "+table.cards[i].Suit);
}
Console.WriteLine(table.CheckHand(table.players[0]));
Console.WriteLine(table.CheckHand(table.players[1]));
table.Turn();
Console.WriteLine(table.cards[3].Value+" "+table.cards[3].Suit);
Console.WriteLine(table.CheckHand(table.players[0]));
Console.WriteLine(table.CheckHand(table.players[1]));
table.River();
Console.WriteLine(table.cards[4].Value+" "+table.cards[4].Suit);
Console.WriteLine(table.CheckHand(table.players[0]));
Console.WriteLine(table.CheckHand(table.players[1]));