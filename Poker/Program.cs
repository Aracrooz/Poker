// See https://aka.ms/new-console-template for more information

using Poker;

Table table = new Table();
table.AddPlayer(new Player("Marek"));
table.AddPlayer(new Player("Szparek"));
table.PlayRound();
