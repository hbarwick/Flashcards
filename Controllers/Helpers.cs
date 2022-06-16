using Flashcards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flashcards.Controllers
{
    internal static class Helpers
    {
        public static string[] GetArrayOfIds(List<CardStack> stackList)
        {
            var choices = from stack in stackList
                          select stack.TempId.ToString();
            choices = choices.Append("0");
            return choices.ToArray();
        }

        public static IEnumerable<T> Randomize<T>(this IEnumerable<T> source)
        {
            Random random = new Random();
            return source.OrderBy<T, int>((item) => random.Next());
        }
    }
}
