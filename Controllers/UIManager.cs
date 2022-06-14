using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flashcards.Models;

namespace Flashcards.Controllers
{

    internal class UIManager
    {
        public DatabaseManager db;
        public UIManager(DatabaseManager db)
        {
            this.db = db;
        }
        public void MenuLoop()
        {
            int UserInput = 1;
            while (UserInput != 0)
            {
                UserInput = GetUserInput(menuChoices, MainMenu);
                MainMenuFunctionSelect(UserInput);
            }
        }

        private string[] menuChoices = { "0", "1", "2", "3", "4" };
        private string[] stackMenuChoices = { "0", "1", "2" };

        private void MainMenu()
        {
            Console.WriteLine("---------------------------");
            Console.WriteLine("         MAIN MENU         ");
            Console.WriteLine("---------------------------\n");
            Console.WriteLine("0 - Exit Application");
            Console.WriteLine("1 - Manage Stacks");
            Console.WriteLine("2 - Manage Flashcards");
            Console.WriteLine("3 - Study");
            Console.WriteLine("4 - View Study Scores");
            Console.Write("\nEnter option number: ");
        }

        private void ManageStacksMenu()
        {
            Console.WriteLine("---------------------------");
            Console.WriteLine("       Manage Stacks       ");
            Console.WriteLine("---------------------------\n");
            Console.WriteLine("0 - Back to Main Menu");
            Console.WriteLine("1 - Add Stack");
            Console.WriteLine("2 - Delete Stack");
            Console.Write("\nEnter option number: ");
        }

        private void MainMenuFunctionSelect(int intUserInput)
        {
            switch (intUserInput)
            {
                case 0:
                    // Exit Program
                    Console.WriteLine("Goodbye!");
                    break;
                case 1:
                    ManageStacks();
                    break;
                case 2:
                    ManageCards();
                    break;
                case 3:
                    Study();
                    break;
                case 4:
                    ViewScores();
                    break;
            }
        }

        private void ManageStacksFunctionSelect(int intUserInput)
        {
            switch (intUserInput)
            {
                case 0:
                    // Exit Program
                    break;
                case 1:
                    AddStack();
                    break;
                case 2:
                    DeleteStack();
                    break;
            }
        }

        private void DeleteStack()
        {
            List<CardStack> stacks = db.GetStackList();
            Report.DisplayAllRecords(stacks);
            int input = GetUserInput(Helpers.GetArrayOfIds(stacks), DisplayStacks);
            if (input != 0)
            {
                db.DeleteStack(input);
            }

        }

        private void DisplayStacks()
        {
            Console.Write("Enter ID of Stack to delete, Or 0 to go back: ");
        }


        private void AddStack()
        {
            Console.Write("\nEnter name of the stack to add: ");
            string? stackName = Console.ReadLine();
            if (stackName.Length > 50)
            {
                Console.WriteLine("\nStack name cannot be longer than 50 characters, please enter shorter name.");
                AddStack();
            }
            try
            {
                db.CreateStack(stackName);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE KEY"))
                {
                    Console.WriteLine($"\nStack name - '{stackName}' - already exists.");
                    Console.WriteLine($"Please enter new name.");
                    AddStack();
                }
                else
                {
                    Console.WriteLine("ERROR");
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void ManageStacks()
        {
            int input = GetUserInput(stackMenuChoices, ManageStacksMenu);
            ManageStacksFunctionSelect(input);
        }


        private void ManageCards()
        {
            throw new NotImplementedException();
        }

        private void Study()
        {
            throw new NotImplementedException();
        }

        private void ViewScores()
        {
            throw new NotImplementedException();
        }

        private int GetUserInput(string[] choices, Action MenuToPrint)
        {
            string? userInput = string.Empty;
            while (!choices.Contains(userInput))
            {
                MenuToPrint();
                userInput = Console.ReadLine();
            }
            int intUserInput = int.Parse(userInput);
            return intUserInput;
        }
    }
}
