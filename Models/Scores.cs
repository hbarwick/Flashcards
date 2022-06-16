using Dapper.Contrib.Extensions;

namespace Flashcards.Models
{
    // Dapper contrib requires table to be declared in the model
    [Table("Scores")]
    internal class Scores
    {
        public int Id { get; set; }
        public string? StackName { get; set; }
        public DateTime Date { get; set; }
        public int Score { get; set; }
        public int StackSize { get; set; }
    }
}
