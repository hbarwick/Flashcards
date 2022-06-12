// See https://aka.ms/new-console-template for more information
using Flashcards.Controllers;
using Flashcards.Models;

Console.WriteLine("Hello, World!");

DatabaseManager db = new();

var dbExists = db.CheckDatabase();
if (!dbExists) { db.CreateDatabase(); }
db.CheckTables();

//Console.WriteLine($"Checkdatabase result {result}");

// db.CreateStack("SQL Queries");

// CardStack st = db.QueryStack("SQL Queries");

db.CreateCard("SQL Queries", "Hello", "World");
db.CreateCard("SQL Queries", "Hal", "Katy");
db.CreateCard("SQL Queries", "This", "That");


// linq
//var lowNums = from num in nums
//              where num < 4
//              select num;

