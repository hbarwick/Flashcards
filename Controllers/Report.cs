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
        public static void DisplayAllRecords(List<CardStack> stacks)
        {
            var tableData = new List<List<object>>();

            foreach (CardStack stack in stacks)
            {
                tableData.Add(new List<object> { stack.Id, stack.StackName, stack.StackSize });
            }
            ConsoleTableExt.ConsoleTableBuilder
                .From(tableData)
                .WithColumn("Id", "Stack Name", "Cards in stack")
                .ExportAndWriteLine();
        }
    }
}
