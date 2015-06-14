using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Randocode.Grammar
{
    public abstract class Command
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        public abstract void Execute(Grammar currentGrammar, Generator.GenerationResult result);


    }

    /// <summary>
    /// Attributes used to register a command with a unique key.
    /// </summary>
    public class CommandAttribute : Attribute
    {
        public string CommandKey { get; set; }
        public CommandAttribute(string commandkey)
        {
            CommandKey = commandkey;
        }
    }

    [Command("symdecl")]
    public class SymdeclCommand : Command
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        public override void Execute(Grammar currentGrammar, Generator.GenerationResult result)
        {
            foreach(var symbol in result.Symbols)
            {
                GrammarRule newRule = new GrammarRule(symbol.Key, new ConstGenerator(symbol.Value, ""));
                currentGrammar.AddRule(newRule);
            }
        }
    }
}
