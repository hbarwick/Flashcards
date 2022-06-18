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
        private string[] stackMenuChoices = { "0", "1", "2", "3" };

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
            Console.WriteLine("3 - View Stacks");
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
            Console.WriteLine("3 - View All Cards");
            Console.Write("\nEnter option number: ");
        }

        private void ViewScoresMenu()
        {
            Console.WriteLine("\n---------------------------");
            Console.WriteLine("         View Scores       ");
            Console.WriteLine("---------------------------\n");
            Console.WriteLine("0 - Back to Main Menu");
            Console.WriteLine("1 - View sessions for stack");
            Console.WriteLine("2 - View Total Sessions per month");
            Console.WriteLine("3 - View Average Score per month");
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
                case 3:
                    List<CardStack> stacks = db.GetStackList();
                    Report.DisplayAllStacks(stacks);
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
                case 3:
                    var cards = db.GetCardList();
                    Report.DisplayAllCards(cards);
                    break;
            }
        }
        private void ViewScoresFunctionSelect(int intUserInput)
        {
            switch (intUserInput)
            {
                case 0:
                    // Exit Program
                    break;
                case 1:
                    ViewScoresByStack();
                    break;
                case 2:
                    var totalsReport = db.GetMonthlyTotalsReport();
                    Console.WriteLine("\nTotal number of sessions per month...\n");
                    Report.DisplayMonthReport(totalsReport);
                    break;
                case 3:
                    var averageReport = db.GetMonthlyAverageReport();
                    Console.WriteLine("\nAverage score per month...\n");
                    Report.DisplayMonthReport(averageReport);
                    break;
            }
        }

        private void ViewScoresByStack()
        {
            Console.WriteLine("Choose stack to view study sessions");
            var stack = DisplayAndChooseStack();
            var allscores = db.GetScoresList();
            var scores = allscores.Where(x => x.StackName == stack).OrderBy(x => x.Date).ToList();
            Console.WriteLine($"\n\nDisplaying all scores for stack '{stack}'.\n");
            Report.DisplayStackScores(scores);
            Console.WriteLine();
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

        private void ViewScores()
        {
            int input = GetUserInput(stackMenuChoices, ViewScoresMenu);
            ViewScoresFunctionSelect(input);
        }

        private void DeleteCard()
        {
            Console.WriteLine("\nSelect stack to delete card from... ");
            var stackname = DisplayAndChooseStack();
            List<Card> allCards = db.GetCardList();
            var cards = allCards.Where(a => a.StackName == stackname).ToList();
            var card = DisplayAndChooseCard(cards);
            db.DeleteCard(card);
            db.ReIndexCards(stackname);
        }

        private int DisplayAndChooseCard(List<Card> cards)
        {
            Report.DisplayCards(cards);
            var ids = cards.Select(a => a.StackId.ToString()).ToList();
            ids.Add("0");
            var idarray = ids.ToArray();
            int input = GetUserInput(idarray, DisplayCards);
            var card = cards.Where(a => a.StackId == input).ToList();

            try
            {
                var cardId = card.First().Id;
                return cardId;
            }
            catch
            {
                return 0;
            }
        }


        private void AddCard()
        {
            Console.WriteLine("\nSelect stack to add a card to.\n");
            List<CardStack> stacks = db.GetStackList();
            Report.DisplayAllStacks(stacks);
            int input = GetUserInput(Helpers.GetArrayOfIds(stacks), DisplayStacks);
            var stack = stacks.Where(a => a.TempId == input).ToList();
            var stackname = stack.First().StackName;
            var keepGoing = string.Empty;
            while (keepGoing != "0")
            {
                string? frontText = GetString(150, "\nEnter front text of flashcard: ");
                string? backText = GetString(150, "\nEnter back text of flashcard: ");
                db.CreateCard(stackname, frontText, backText);
                Console.Write("Add another, or 0 to go back?\n");
                keepGoing = Console.ReadLine();
            }
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
            var stackName = DisplayAndChooseStack();

            if (stackName != null)
            {
                string message = $"\nAre you SURE you want to delete stack '{stackName}'?" +
                    $"\nALL cards AND study scores for this stack will also be deleted - y/n: ";
                string answer = GetYN(message);
                if (answer == "y")
                {
                    db.DeleteStack(stackName);
                    Console.WriteLine("\nStack Deleted.\n");
                }
            }
        }

        private string DisplayAndChooseStack()
        {
            List<CardStack> stacks = db.GetStackList();
            Report.DisplayAllStacks(stacks);
            var ids = stacks.Select(a => a.TempId.ToString()).ToList();
            ids.Add("0");
            var idarray = ids.ToArray();
            int input = GetUserInput(idarray, DisplayStacks);
            var stacklist = stacks.Where(a => a.TempId == input).ToList();
            try
            {
                var stackname = stacklist.First().StackName;
                return stackname;
            }
            catch
            {
                return null;
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

        private void DisplayCards()
        {
            Console.Write("\nEnter ID of Card, Or 0 to go back: ");
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
        private void Study()
        {
            Console.WriteLine("Choose stack to study...");
            var stackName = DisplayAndChooseStack();
            var cards = db.GetCardList();
            var studyCards = cards.Where(a => a.StackName == stackName).ToList();
            var randomized = Helpers.Randomize(studyCards);
            Scores scoreCard = new();
            scoreCard.Date = DateTime.Now;
            scoreCard.StackName = stackName;
            scoreCard.StackSize = studyCards.Count;
            scoreCard.Score = 0;
            Console.WriteLine($"\nStarting test on stack: {stackName}\n");
            foreach(var card in randomized)
            {
                Console.WriteLine($"\nQ: {card.FrontText}");
                Console.Write($"A: ");
                var answer = Console.ReadLine();
                if (answer == card.BackText)
                {
                    Console.WriteLine("Correct!");
                    scoreCard.Score++;
                }
                else
                {
                    Console.WriteLine($"Incorrect. The answer was {card.BackText}");
                }
            }
            Console.WriteLine($"\nYou scored {scoreCard.Score} out of {scoreCard.StackSize}.\n");
            db.WriteScore(scoreCard);
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
