using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flashcards.Models
{
    internal class Scores
    {
        public int Id { get; set; }
        public string? StackName { get; set; }
        public DateTime Date { get; set; }
        public int Score { get; set; }
    }
}
