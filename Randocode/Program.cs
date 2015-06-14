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
            Grammar.Grammar g = Grammar.RuleParser.ParseGrammar(System.IO.File.ReadAllText("prog.txt"));
            string str = g.PickRandom("instructionlist").Execute(g).Content;

            g.Add(Grammar.RuleParser.ParseRule("boolexpr:true"));
            g.Add(Grammar.RuleParser.ParseRule("boolexpr:false"));
            g.Add(Grammar.RuleParser.ParseRule("intexpr:@int[0, 255]()"));
            g.Add(Grammar.RuleParser.ParseRule("boolexpr:${{intexpr}} < ${{intexpr}}"));
            g.Add(Grammar.RuleParser.ParseRule("boolexpr:${{boolexpr}} || ${{boolexpr}}"));
            g.Add(Grammar.RuleParser.ParseRule("intvarname:@name(intvar)"));
            g.Add(Grammar.RuleParser.ParseRule("boolvarname:@name(boolvar)"));
            g.Add(Grammar.RuleParser.ParseRule("boolvardecl:${{boolvarname:boolexpr}} = ${{boolexpr}};\n"));
            g.Add(Grammar.RuleParser.ParseRule("instruction:${{boolvardecl}}"));
            g.Add(Grammar.RuleParser.ParseRule("instructionlist:@many[50, 150](${{instruction}})"));

            str = g.PickRandom("instructionlist").Execute(g).Content;
            int lol = 0;
#if false
            Grammar.Grammar grammar = new Grammar.Grammar();
            grammar.Add(new Grammar.GrammarRule() {
                RuleName = "boolexpr",
                Cmd = null,
                Gen = new Grammar.ConstGenerator("true")
            });
            grammar.Add(new Grammar.GrammarRule()
            {
                RuleName = "boolexpr",
                Cmd = null,
                Gen = new Grammar.ConstGenerator("false")
            });
            grammar.Add(new Grammar.GrammarRule()
            {
                RuleName = "intexpr",
                Cmd = null,
                Gen = new Grammar.ConstGenerator("5")
            });
            grammar.Add(new Grammar.GrammarRule()
            {
                RuleName = "boolexpr",
                Cmd = null,
                Gen = new Grammar.ConstGenerator("${{intexpr}} < ${{intexpr}}")
            });

            grammar.Add(new Grammar.GrammarRule()
            {
                RuleName = "intvarname",
                Cmd = null,
                Gen = new Grammar.NameGenerator("intvar")
            }); 
            
            grammar.Add(new Grammar.GrammarRule()
            {
                RuleName = "boolvarname",
                Cmd = null,
                Gen = new Grammar.NameGenerator("boolvar")
            }); 

            grammar.Add(new Grammar.GrammarRule()
            {
                RuleName = "vardecl",
                Cmd = new Grammar.SymdeclCommand(),
                Gen = new Grammar.ConstGenerator("${{intvarname:intexpr}} = ${{intexpr}};\n")
            });

            grammar.Add(new Grammar.GrammarRule()
            {
                RuleName = "vardecl",
                Cmd = new Grammar.SymdeclCommand(),
                Gen = new Grammar.ConstGenerator("${{boolvarname:boolexpr}} = ${{boolexpr}};\n")
            });

            StringBuilder sb = new StringBuilder();
            for(int i = 0; i< 50; i++)
            {
                sb.Append(grammar.PickRandom("vardecl").Execute(grammar).Content);
            }

            string str = sb.ToString();
#endif
            Regex reg2 = new Regex(@"@([a-z]*)(?:\[([^\(]*)\])?\((.*)\)", RegexOptions.Singleline);// new Regex(@"([a-z]*):(.*)" , RegexOptions.Singleline);
            Match match2 = reg2.Match("@mdrrr(lololol)");
            var grp5 = match2.Groups[0];
            var grp6 = match2.Groups[1];
            var grp7 = match2.Groups[2];
            var grp8 = match2.Groups[3];

            // -------------------------------

            Regex reg = new Regex(@"\${{([a-z]+)(:[a-z]+)?}}");
            Match match = reg.Match("oktamer ${{hellobiatch:oktamer}} ${{hahaha}}");
            var grp = match.Groups[0];
            var grp2 = match.Groups[1];
            var grp3 = match.Groups[2];
            string cap = "$$";
            string cap2 = "$$";
            if(match.Success)
            {
                cap = match.Value;
            }
            match = match.NextMatch();
            if(match.Success)
            {
                cap2 = match.Value;
            }

        }
    }
}
