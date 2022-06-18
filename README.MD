# FlashCards

A CRUD console application for creating, managing and studying decks of flashcards.
Written in C# using Visual Studio 2022, making use of SQL Server localdb database.


# Features

- [x] Creation and deletion of "Stacks" of cards. 
- [x] Deleting stack will cascade and delete any cards and study scores associated with it.
- [x] Creation of deletion flashcards. Flashcards have "Front" and "Back" text, PK Unique ID, FK stackname, and ID in stack.
- [x] Stack/Card IDs displayed in the menus will always be consecutive; deletion of any items will cause IDs to be re-indexed.
- [x] "Study" mode - Select a deck to study, Front text of the cards will be displayed in a random order, you must enter the correct Back text.
- [x] Date and Score for each Study session are logged into the database.
- [x] Reporting mode - Reports to show all sessions of a given stack in date order, with number of correct answers and percentage result.
- [x] Further reporting of total sessions and average results, broken down per month.


# Lessons learnt

- Database queries returned as DTOs to the business logic
- Use of Linq queries to filter down lists of objects to the desired outputs
- Use of Dapper to facilite succint database queries.
- Dapper contrib to insert DTO directly into database. 
- Reporting options make use of SQL pivot statement.
