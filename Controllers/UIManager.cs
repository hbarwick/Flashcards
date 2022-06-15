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
            Console.WriteLine("\n---------------------------");
            Console.WriteLine("       Manage Stacks       ");
            Console.WriteLine("---------------------------\n");
            Console.WriteLine("0 - Back to Main Menu");
            Console.WriteLine("1 - Add Stack");
            Console.WriteLine("2 - Delete Stack");
            Console.Write("\nEnter option number: ");
        }

        private void ManageCardsMenu()
        {
            Console.WriteLine("\n---------------------------");
            Console.WriteLine("        Manage Cards       ");
            Console.WriteLine("---------------------------\n");
            Console.WriteLine("0 - Back to Main Menu");
            Console.WriteLine("1 - Add Card");
            Console.WriteLine("2 - Delete Card");
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

        private void ManageCardsFunctionSelect(int intUserInput)
        {
            switch (intUserInput)
            {
                case 0:
                    // Exit Program
                    break;
                case 1:
                    AddCard();
                    break;
                case 2:
                    DeleteCard();
                    break;
            }
        }

        private void DeleteCard()
        {
            throw new NotImplementedException();
        }

        private void AddCard()
        {
            Console.WriteLine("\nSelect stack to add a card to.\n");
            List<CardStack> stacks = db.GetStackList();
            Report.DisplayAllRecords(stacks);
            int input = GetUserInput(Helpers.GetArrayOfIds(stacks), DisplayStacks);
            var stack = stacks.Where(a => a.Id == input).ToList();
            var stackname = stack.First().StackName;
            if (input == 0)
            {
                return;
            }
            string? frontText = GetString(150, "\nEnter front text of flashcard: ");
            string? backText = GetString(150, "\nEnter back text of flashcard: ");

            db.CreateCard(stackname, frontText, backText);

        }
        private string GetString(int length, string message)
        {
            Console.Write(message);
            string? textEntry = Console.ReadLine();
            while(textEntry.Length > length)
            {
                Console.Write($"\nMax length of {length} characters exceeded." +
                    $"\nPlease enter shorter string: ");
                textEntry = Console.ReadLine();
            }
            return textEntry;
        }

        private void DeleteStack()
        {
            List<CardStack> stacks = db.GetStackList();
            Report.DisplayAllRecords(stacks);
            int input = GetUserInput(Helpers.GetArrayOfIds(stacks), DisplayStacks);

            var stackname = stacks.Where(a => a.Id == input).ToList();
            if (input != 0)
            {
                string message = $"\nAre you SURE you want to delete stack '{stackname.First().StackName}'?" +
                    $"\nALL cards AND study scores for this stack will also be deleted - y/n: ";
                string answer = GetYN(message);
                if (answer == "y")
                {
                    db.DeleteStack(input);
                    Console.WriteLine("\nStack Deleted.\n");
                }
            }
        }

        private string GetYN(string message)
        {
            string? userInput = string.Empty;
            string[] choices = { "y", "n" };
            while (!choices.Contains(userInput))
            {
                Console.Write(message);
                userInput = Console.ReadLine().ToLower();
            }
            return userInput;
        }

        private void DisplayStacks()
        {
            Console.Write("\nEnter ID of Stack, Or 0 to go back: ");
        }


        private void AddStack()
        {
            string? stackName = GetString(50, "\nEnter name of the stack to add: ");
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
            int input = GetUserInput(stackMenuChoices, ManageCardsMenu);
            ManageCardsFunctionSelect(input);
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
