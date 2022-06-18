using Flashcards.Controllers;

DatabaseManager db = new();

var dbExists = db.CheckDatabase();
if (!dbExists) { db.CreateDatabase(); }
db.CheckTables();

UIManager ui = new(db);
ui.MenuLoop();