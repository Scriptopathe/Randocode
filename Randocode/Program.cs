using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
namespace Randocode
{
    class Program
    {
        static void Main(string[] args)
        {
            /* TODO : 
             * - portée des variables
             * - gestion avancée de la profondeur
             * 
             * */
            Grammar.Grammar g = Grammar.RuleParser.ParseGrammar(System.IO.File.ReadAllText("prog.txt"));
            string str = g.PickRandom("instructionlist").Execute(g).Content;


        }
    }
}
