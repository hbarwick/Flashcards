using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;

namespace Flashcards.Controllers
{
    internal class DatabaseManager
    {
        string? initialConnectionString = ConfigurationManager.AppSettings.Get("initialConnectionString");
        string? connectionString = ConfigurationManager.AppSettings.Get("connectionString");

        public bool CheckDatabase()
        {
            Console.WriteLine(connectionString);
            using (var connection = new SqlConnection(connectionString))
            {
                using (var tableCommand = connection.CreateCommand())
                {
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
            }
        }

        public bool CheckTables()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                using (var tableCommand = connection.CreateCommand())
                { 
                    // Select statement to test whether tables exist
                    tableCommand.CommandText = "SELECT 1 FROM Cards WHERE 1=0";
                    try
                    {
                        tableCommand.ExecuteNonQuery();
                        return true;
                    }
                    catch
                    {
                        CreateTables();
                        return false;
                    }
                }
            }
        }

        public void CreateDatabase()
        {
            using (var initconnection = new SqlConnection(initialConnectionString))
            {
                using (var dbCommand = initconnection.CreateCommand())
                {
                    initconnection.Open();
                    dbCommand.CommandText =
                        $@"CREATE DATABASE Flashcards";
                    dbCommand.ExecuteNonQuery();
                }
            }
        }

        public void CreateTables()
        {
            Console.WriteLine(connectionString);
            using (var connection = new SqlConnection(connectionString))
            {
                using (var tableCommand = connection.CreateCommand())
                {
                    connection.Open();
                    tableCommand.CommandText =
                        $@"USE Flashcards;
                           CREATE TABLE Stacks(
                           StackName VARCHAR(50) PRIMARY KEY);
                            
                           CREATE TABLE Cards(
                           Id INT  IDENTITY(1,1) PRIMARY KEY,
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
            }
        }

        public void CreateStack(string stackName)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                using (var tableCommand = connection.CreateCommand())
                {
                    connection.Open();
                    tableCommand.CommandText = "INSERT INTO Stacks (StackName)" +
                        "                       VALUES (@stackName)";
                    tableCommand.Parameters.AddWithValue("stackName", stackName);
                }
            }
        }
    }
}
