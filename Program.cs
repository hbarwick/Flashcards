// See https://aka.ms/new-console-template for more information
using Flashcards.Controllers;

Console.WriteLine("Hello, World!");

DatabaseManager db = new();

var dbExists = db.CheckDatabase();
if (!dbExists) { db.CreateDatabase(); }
db.CheckTables();

//Console.WriteLine($"Checkdatabase result {result}");

db.CreateStack("SQL Queries");



// linq
//var lowNums = from num in nums
//              where num < 4
//              select num;

