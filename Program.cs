// See https://aka.ms/new-console-template for more information
using Flashcards.Controllers;
using Flashcards.Models;


DatabaseManager db = new();

var dbExists = db.CheckDatabase();
if (!dbExists) { db.CreateDatabase(); }
db.CheckTables();


UIManager ui = new(db);
ui.MenuLoop();