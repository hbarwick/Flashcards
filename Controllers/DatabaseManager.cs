﻿using System.Data.SqlClient;
using System.Configuration;
using Flashcards.Models;
using Dapper;

namespace Flashcards.Controllers
{
    internal class DatabaseManager
    {
        string? initialConnectionString = ConfigurationManager.AppSettings.Get("initialConnectionString");
        string? connectionString = ConfigurationManager.AppSettings.Get("connectionString");

        public bool CheckDatabase()
        {
            Console.WriteLine(connectionString);
            using var connection = new SqlConnection(connectionString);
            using var tableCommand = connection.CreateCommand();
            try
            {
                // Try to connect with the connection string with database name specified
                connection.Open();
                return true;
            }
            catch (Exception ex)
            {
                // If no database with that name, it will fail so catch will Create the database
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public bool CheckTables()
        {
            using var connection = new SqlConnection(connectionString);
            using var tableCommand = connection.CreateCommand();
            // Select statement to test whether tables exist
            tableCommand.CommandText = "SELECT 1 FROM Cards WHERE 1=0";
            try
            {
                connection.Open();
                tableCommand.ExecuteNonQuery();
                return true;
            }
            catch
            {
                CreateTables();
                return false;
            }
        }

        public void CreateDatabase()
        {
            using var initconnection = new SqlConnection(initialConnectionString);
            using var dbCommand = initconnection.CreateCommand();
            initconnection.Open();
            dbCommand.CommandText =
                $@"CREATE DATABASE Flashcards";
            dbCommand.ExecuteNonQuery();
        }

        public void CreateTables()
        {
            Console.WriteLine(connectionString);
            using var connection = new SqlConnection(connectionString);
            using var tableCommand = connection.CreateCommand();
            connection.Open();
            tableCommand.CommandText =
                $@"USE Flashcards;
                           CREATE TABLE Stacks(
                           Id INT IDENTITY(1,1) PRIMARY KEY,
                           StackName VARCHAR(50) UNIQUE);
                            
                           CREATE TABLE Cards(
                           Id INT  IDENTITY(1,1) PRIMARY KEY,
                           StackId INT,
                           StackName VARCHAR(50) FOREIGN KEY REFERENCES Stacks(StackName) ON DELETE CASCADE,
                           FrontText VARCHAR(150),
                           BackText VARCHAR(150));
                            
                           CREATE TABLE Scores(
                           Id INT IDENTITY(1,1) PRIMARY KEY,
                           Stack VARCHAR(50) FOREIGN KEY REFERENCES Stacks(StackName) ON DELETE CASCADE,
                           Date DATETIME,
                           Score INT);";
            tableCommand.ExecuteNonQuery();
        }

        public void CreateStack(string stackName)
        {
            using var connection = new SqlConnection(connectionString);
            using var tableCommand = connection.CreateCommand();
            connection.Open();
            tableCommand.CommandText = "INSERT INTO Stacks (StackName)" +
                "                       VALUES (@stackName)";
            tableCommand.Parameters.AddWithValue("stackName", stackName);
            tableCommand.ExecuteNonQuery();
        }

        public void CreateCard(string stackName, string front, string back)
        {
            CardStack stack = QueryStack(stackName);
            using var connection = new SqlConnection(connectionString);
            using var tableCommand = connection.CreateCommand();
            connection.Open();
            tableCommand.CommandText = "INSERT INTO Cards (StackId, StackName, FrontText, BackText)" +
                                       "VALUES (@StackId, @stackName, @FrontText, @BackText)";
            tableCommand.Parameters.AddWithValue("@StackId", stack.StackSize + 1);
            tableCommand.Parameters.AddWithValue("stackName", stack.StackName);
            tableCommand.Parameters.AddWithValue("FrontText", front);
            tableCommand.Parameters.AddWithValue("BackText", back);
            tableCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Returns a CardStack object, with name from the Stack table of the database
        /// Stacksize comes from the number of cards with that Stack name
        /// </summary>
        /// <param name="stackName">String name of stack</param>
        /// <returns>CardStack object</returns>
        public CardStack QueryStack(string stackName)
        {
            CardStack newStack = new();
            int stackSize = 0;
            using var connection = new SqlConnection(connectionString);
            using var tableCommand = connection.CreateCommand();
            connection.Open();
            tableCommand.CommandText = "SELECT COUNT(*) FROM Cards WHERE StackName = @StackName";
            tableCommand.Parameters.AddWithValue("@StackName", stackName);
            stackSize = (Int32)tableCommand.ExecuteScalar();

            newStack.StackName = stackName;
            newStack.StackSize = stackSize;

            return newStack;
        }

        /// <summary>
        /// To reorder the StackId of cards in given stackName, removing any gaps in Id.
        /// Filters down list of cards to those assigned to given stackName,
        /// Loops through list updating database with new StackId
        /// </summary>
        /// <param name="stackName">Name of the stack to reindex</param>
        public void ReIndexCards(string stackName)
        {
            var cards = GetCardList();
            var stackCards = from card in cards
                             where card.StackName == stackName
                             select card;

            int index = 1;
            using var connection = new SqlConnection(connectionString);
            using var tableCommand = connection.CreateCommand();
            connection.Open();
            foreach (Card card in stackCards)
            {
                tableCommand.CommandText = "UPDATE CARDS SET StackId = @StackId WHERE Id = @Id";
                tableCommand.Parameters.AddWithValue("@StackId", index++);
                tableCommand.Parameters.AddWithValue("@Id", card.Id);
                tableCommand.ExecuteNonQuery();
                tableCommand.Parameters.Clear();
            }
        }

        public void ReIndexStacks()
        {
            using var connection = new SqlConnection(connectionString);
            using var tableCommand = connection.CreateCommand();
            connection.Open();
            tableCommand.CommandText = @"SELECT StackName INTO #TempTable FROM Stacks;
                                         DELETE FROM stacks;
                                         DBCC CHECKIDENT('Stacks', RESEED, 0);
                                         INSERT INTO stacks SELECT StackName FROM #TempTable;
                                         DROP TABLE #TempTable;";
            tableCommand.ExecuteNonQuery();
        }


        /// <summary>
        /// Uses dappers Query method to return a list of Card from the Cards table.
        /// </summary>
        /// <returns>List of Card</returns>
        public List<Card> GetCardList()
        {
            List<Card> cards = new List<Card>();
            using var connection = new SqlConnection(connectionString);

            cards = connection.Query<Card>("SELECT * FROM Cards").ToList();

            return cards;
        }

        /// <summary>
        /// Uses dappers Query method to return a list of Stacks from the Stacks table.
        /// </summary>
        /// <returns>List of CardStack</returns>
        public List<CardStack> GetStackList()
        {
            List<CardStack> stacks = new List<CardStack>();
            using var connection = new SqlConnection(connectionString);

            stacks = connection.Query<CardStack>("SELECT * FROM Stacks ORDER BY Id").ToList();

            return stacks;
        }

        public void DeleteStack(int Id)
        {
            using var connection = new SqlConnection(connectionString);
            using var tableCommand = connection.CreateCommand();
            connection.Open();
            tableCommand.CommandText = "DELETE FROM Stacks WHERE Id = @Id";
            tableCommand.Parameters.AddWithValue("@Id", Id);
            tableCommand.ExecuteNonQuery();
            ReIndexStacks();
        }
    }
}
