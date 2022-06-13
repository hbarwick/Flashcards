// See https://aka.ms/new-console-template for more information
using Flashcards.Controllers;
using Flashcards.Models;

Console.WriteLine("Hello, World!");

DatabaseManager db = new();

var dbExists = db.CheckDatabase();
if (!dbExists) { db.CreateDatabase(); }
db.CheckTables();

//Console.WriteLine($"Checkdatabase result {result}");

//db.CreateStack("A");
//db.CreateStack("B");

//db.CreateCard("A", "Dog", "Bone");
//db.CreateCard("A", "Cat", "Mouse");
//db.CreateCard("A", "Chip", "Potato");
//db.CreateCard("B", "DD", "22");
//db.CreateCard("B", "FF", "44");
//db.CreateCard("B", "GG", "65");

List<Card> cards = new List<Card>();

cards = db.GetCardList();

//foreach (var card in cards)
//{
//    Console.WriteLine(card.FrontText);
//    Console.WriteLine(card.BackText);
//    Console.WriteLine(card.StackName);
//}

var sqlcards = from card in cards
               where card.StackName == "A"
               select card;


UIManager ui = new(db);

ui.MenuLoop();