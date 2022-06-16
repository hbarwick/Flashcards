using Flashcards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    }
}
