using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flashcards.Models
{
    internal class Card
    {
        public int Id { get; set; }
        public int StackId { get; set; }
        public string? StackName { get; set; }
        public string? FrontText { get; set; }
        public string? BackText { get; set; }
    }
}
