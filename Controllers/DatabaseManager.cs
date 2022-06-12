using System.Data.SqlClient;
using System.Configuration;
using Flashcards.Models;

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
                           Id INT IDENTITY(1,1) PRIMARY KEY
                           StackName VARCHAR(50) UNIQUE);
                            
                           CREATE TABLE Cards(
                           Id INT  IDENTITY(1,1) PRIMARY KEY,
                           StackId INT,
                           Stack VARCHAR(50) FOREIGN KEY REFERENCES Stacks(StackName),
                           FrontText VARCHAR(150),
                           BackText VARCHAR(150));
                            
                           CREATE TABLE Scores(
                           Id INT IDENTITY(1,1) PRIMARY KEY,
                           Stack VARCHAR(50) FOREIGN KEY REFERENCES Stacks(StackName),
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
            tableCommand.CommandText = "INSERT INTO Cards (StackId, Stack, FrontText, BackText)" +
                                       "VALUES (@StackId, @stackName, @FrontText, @BackText)";
            tableCommand.Parameters.AddWithValue("@StackId", stack.StackSize + 1);
            tableCommand.Parameters.AddWithValue("stackName", stack.StackName);
            tableCommand.Parameters.AddWithValue("FrontText", front);
            tableCommand.Parameters.AddWithValue("BackText", back);
            tableCommand.ExecuteNonQuery();
        }

        public CardStack QueryStack(string StackName)
        {
            CardStack newStack = new();
            int stackSize = 0;
            using var connection = new SqlConnection(connectionString);
            using var tableCommand = connection.CreateCommand();
            connection.Open();
            tableCommand.CommandText = "SELECT COUNT(*) FROM Cards WHERE Stack = @StackName";
            tableCommand.Parameters.AddWithValue("@StackName", StackName);
            stackSize = (Int32)tableCommand.ExecuteScalar();

            newStack.StackName = StackName;
            newStack.StackSize = stackSize;

            return newStack;
        }

    }
}
