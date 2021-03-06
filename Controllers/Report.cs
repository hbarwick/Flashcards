using Flashcards.Models;
using ConsoleTableExt;

namespace Flashcards.Controllers
{
    internal class Report
    {
        /// <summary>
        /// Prints out the Id, StartTime, EndTime & Duration of the supplied list of sessions.
        /// Formats into table using ConsoleTableExt for prettier display.
        /// </summary>
        /// <param name="sessionList">List of Session objects.</param>
        public static void DisplayAllStacks(List<CardStack> stacks)
        {
            var tableData = new List<List<object>>();

            foreach (CardStack stack in stacks)
            {
                tableData.Add(new List<object> { stack.TempId, stack.StackName, stack.StackSize });
            }
            ConsoleTableExt.ConsoleTableBuilder
                .From(tableData)
                .WithColumn("Id", "Stack Name", "Cards in stack")
                .ExportAndWriteLine();
        }

        /// <summary>
        /// Prints out the Id, Front text and Back text of the supplied list of cards.
        /// Formats into table using ConsoleTableExt for prettier display.
        /// </summary>
        /// <param name="cards">List of Card objects.</param>
        public static void DisplayCards(List<Card> cards)
        {
            var tableData = new List<List<object>>();

            foreach (Card card in cards)
            {
                tableData.Add(new List<object> { card.StackId, card.FrontText, card.BackText });
            }
            ConsoleTableExt.ConsoleTableBuilder
                .From(tableData)
                .WithColumn("Id", "Front Text", "Back Text")
                .ExportAndWriteLine();
        }

        /// <summary>
        /// Prints out the Stack Name, StackId, Front text and Back text of the supplied list of cards.
        /// Used on method call to display all cards in the database.
        /// Formats into table using ConsoleTableExt for prettier display.
        /// </summary>
        /// <param name="cards">List of Card objects.</param>
        public static void DisplayAllCards(List<Card> cards)
        {
            var tableData = new List<List<object>>();

            foreach (Card card in cards)
            {
                tableData.Add(new List<object> { card.StackName, card.StackId, card.FrontText, card.BackText });
            }
            ConsoleTableExt.ConsoleTableBuilder
                .From(tableData)
                .WithColumn("Stack Name", "Id", "Front Text", "Back Text")
                .ExportAndWriteLine();
        }

        /// <summary>
        /// Prints out row for each stack, with a value split over each month of the year.
        /// </summary>
        /// <param name="reportLines">List of MonthReport objects corresponding to each stack name</param>
        public static void DisplayMonthReport(List<MonthReport> reportLines)
        {
            var tableData = new List<List<object>>();

            foreach (MonthReport line in reportLines)
            {
                tableData.Add(new List<object> { line.StackName, 
                    line.January, line.February, line.March, line.April, 
                    line.May, line.June, line.July, line.August, line.September, 
                    line.October, line.November, line.December });
            }
            ConsoleTableExt.ConsoleTableBuilder
                .From(tableData)
                .WithColumn("Stack Name", "Jan", "Feb", 
                "Mar", "Apr", "May", "Jun", "Jul", "Aug", 
                "Sep", "Oct", "Nov", "Dec")
                .ExportAndWriteLine();
        }

        /// <summary>
        /// Prints out the data from a list of Score objects.
        /// Calculates the percentage correct from the Score and stacksize.
        /// </summary>
        /// <param name="scores">List of Score objects</param>
        internal static void DisplayStackScores(List<Scores> scores)
        {
            var tableData = new List<List<object>>();

            foreach (Scores score in scores)
            {
                float fpercent = (float)score.Score / (float)score.StackSize * 100;
                string percent = $"{fpercent.ToString("0.00")}%";
                tableData.Add(new List<object> { score.Date, score.Score, score.StackSize, percent});
            }
            ConsoleTableExt.ConsoleTableBuilder
                .From(tableData)
                .WithColumn("Date", "Score", "Stack Size", "Percent Correct")
                .ExportAndWriteLine();
        }
    }
}
