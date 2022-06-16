using Flashcards.Controllers;

namespace Flashcards.Models
{
    internal class CardStack
    {
        public int Id { get; set; }
        public int TempId { get; set; }
        public string? StackName { get; set; }
        public int StackSize 
        {
            get { return DatabaseManager.GetStackSize(StackName); }
        }
    }
}
