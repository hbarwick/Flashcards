using Flashcards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flashcards.Controllers
{
    internal class Helpers
    {
        public static string[] GetArrayOfIds(List<CardStack> stackList)
        {
            var choices = from stack in stackList
                          select stack.Id.ToString();
            choices.Append("0");
            return choices.ToArray();
        }
    }
}
