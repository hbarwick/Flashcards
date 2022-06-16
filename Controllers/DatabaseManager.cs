using System.Data.SqlClient;
using System.Configuration;
using Flashcards.Models;
using Dapper;
using Dapper.Contrib;
using Dapper.Contrib.Extensions;

namespace Flashcards.Controllers
{
    internal class DatabaseManager
    {
        string? initialConnectionString = ConfigurationManager.AppSettings.Get("initialConnectionString");

        static string? staticConnectionString = ConfigurationManager.AppSettings.Get("connectionString");
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
                           StackName VARCHAR(50) FOREIGN KEY REFERENCES Stacks(StackName) ON DELETE CASCADE,
                           Date DATETIME,
                           Score INT,
                           StackSize INT);";
            tableCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Called by the CardStack DTO to determine the size of the stack.
        /// </summary>
        /// <param name="stackName">The stack name to Query</param>
        /// <returns>Int Stacksize</returns>
        internal static int GetStackSize(string? stackName)
        {
            using var connection = new SqlConnection(staticConnectionString);
            using var tableCommand = connection.CreateCommand();
            connection.Open();
            tableCommand.CommandText = "SELECT COUNT(*) FROM Cards WHERE StackName = @StackName";
            tableCommand.Parameters.AddWithValue("@StackName", stackName);
            var stackSize = (Int32)tableCommand.ExecuteScalar();
            return stackSize;
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
            using var connection = new SqlConnection(connectionString);
            using var tableCommand = connection.CreateCommand();
            var sql = "SELECT * FROM Stacks WHERE StackName = @StackName";
            var parameters = new { StackName = stackName };
            var stacks = connection.Query<CardStack>(sql, parameters).ToList();
            var stack = stacks.FirstOrDefault();

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

            // Adds the TempId value to each Stack for purposes of display/selection
            for (var i = 0; i < stacks.Count; i++)
            {
                stacks[i].TempId = i + 1;
            }

            return stacks;
        }

        public void DeleteStack(string stackName)
        {
            using var connection = new SqlConnection(connectionString);
            using var tableCommand = connection.CreateCommand();
            connection.Open();
            tableCommand.CommandText = "DELETE FROM Stacks WHERE StackName = @stackName";
            tableCommand.Parameters.AddWithValue("@StackName", stackName);
            tableCommand.ExecuteNonQuery();
        }

        public void DeleteCard(int Id)
        {
            using var connection = new SqlConnection(connectionString);
            using var tableCommand = connection.CreateCommand();
            connection.Open();
            tableCommand.CommandText = "DELETE FROM Cards WHERE Id = @Id";
            tableCommand.Parameters.AddWithValue("@Id", Id);
            tableCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Uses Dapper Contrib to write the scores DTO direct to the scores table
        /// </summary>
        /// <param name="scoreCard">Populates Scores object</param>
        internal void WriteScore(Scores scoreCard)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Insert(scoreCard);
        }
    }
}
