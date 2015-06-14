using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Randocode.Grammar
{
    /// <summary>
    /// Represents a grammar rule.
    /// A grammar rule is composed of : 
    /// - a name
    /// - an expression generator
    /// - (optional) a command.
    /// 
    /// Syntax:
    /// rulename:expression:command
    /// 
    /// Examples:
    /// varname:@validvarname:@makeunique
    /// booleanexpr:true
    /// booleanexpr:false
    /// booleanexpr:${{intvar}}==${{intvar}}
    /// booleanexpr:@symbol(boolean)
    /// booleanvardecl:${{booleanvarnamei:name}}=${{booleanexpr}}:@simdecl(booleanexpr, name) # attribue au jeton booleanvarname (d'id "id") le symbole boolean
    /// intexpr:@numberliteral[0, 16635]
    /// </summary>
    public class GrammarRule
    {
        /// <summary>
        /// Name of the rule.
        /// </summary>
        public string RuleName { get; set; }
        /// <summary>
        /// Generator of the rule.
        /// </summary>
        public Generator Gen { get; set; }
        /// <summary>
        /// Command of the rule. 
        /// It is executed when the rule is executed.
        /// </summary>
        public Command Cmd { get; set; }

        /// <summary>
        /// Creates a new rule with empty parameters.
        /// </summary>
        public GrammarRule() { }
        /// <summary>
        /// Creates a new rule.
        /// </summary>
        public GrammarRule(string rulename, Generator gen)
        {
            RuleName = rulename;
            Gen = gen;
        }

        /// <summary>
        /// Creates a new rule.
        /// </summary>
        public GrammarRule(string rulename, Generator gen, Command cmd)
        {
            RuleName = rulename;
            Gen = gen;
            Cmd = cmd;
        }

        /// <summary>
        /// Executes the rule to generate a symbol.
        /// </summary>
        public Generator.GenerationResult Execute(Grammar currentGrammar)
        {
            var result =  Gen.Generate(this, currentGrammar);
            
            if(Cmd != null)
                Cmd.Execute(currentGrammar, result);

            return result;
        }
    }
}
