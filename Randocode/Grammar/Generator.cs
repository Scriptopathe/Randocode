using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
namespace Randocode.Grammar
{
    /// <summary>
    /// Represents a generator.
    /// The generator is the class reponsible for generating code.
    /// It has one string parameter, as well as various options.
    /// 
    /// It is summoned (in the syntax of the RuleParser) like this :
    /// rulename:@genratorname[options string](parameter)
    /// </summary>
    public abstract class Generator
    {
        public class GenerationResult
        {
            /// <summary>
            /// Original rule name.
            /// </summary>
            public string RuleName { get; set; }
            /// <summary>
            /// Generated content.
            /// </summary>
            public string Content { get; set; }
            /// <summary>
            /// Generated symbols symbolkey -> generated name.
            /// </summary>
            public Dictionary<string, string> Symbols { get; set; }

            public GenerationResult() { Symbols = new Dictionary<string, string>(); }
        }

        /// <summary>
        /// Parameter of the generator.
        /// </summary>
        public string Parameter { get; set; }
        /// <summary>
        /// Option string of the generator.
        /// </summary>
        public string Options { get; set; }
        

        public Generator(string parameter, string options)
        {
            Parameter = parameter;
            Options = options;
        }
        /// <summary>
        /// Generates a string given the current grammar.
        /// </summary>
        public abstract GenerationResult Generate(GrammarRule parent, Grammar currentGrammar);
        /// <summary>
        /// Gets the MAXIMUM number of subgeneration task that can be done by this 
        /// </summary>
        /// <returns></returns>
        public abstract int GetSubgenCount();
    }
    /// <summary>
    /// Attributes used to register a generator with a unique key.
    /// </summary>
    public class GeneratorAttribute : Attribute
    {
        public string GeneratorKey { get; set; }
        public GeneratorAttribute(string commandkey)
        {
            GeneratorKey = commandkey;
        }
    }

    /// <summary>
    /// Represents the standard generator which :
    /// - reads the ConstName and parses special entries (embedded generators, like ${{rule:symbolname}}) 
    /// - regenerates special entries if needed.
    /// </summary>
    [Generator("const")]
    public class ConstGenerator : Generator
    {
        Regex m_regex = new Regex(@"\${{([a-z]+)(:[a-z]+)?}}");
        int m_subgens;
        public ConstGenerator(string constname, string options) : base(constname, options) 
        {  
            Match match = m_regex.Match(Parameter);
            while(match.Success)
            {
                m_subgens++;
                match = match.NextMatch();
            }
        }

        public override GenerationResult Generate(GrammarRule parent, Grammar currentGrammar)
        {
            GenerationResult res = new GenerationResult();
            res.Content = Parameter;

            // Execute generators.
            Match match = m_regex.Match(Parameter);
            while(match.Success)
            {
                string ruleName = match.Groups[1].Value;

                currentGrammar.CurrentDepth++;
                string generatedSymbol = currentGrammar.PickRandom(ruleName).Execute(currentGrammar).Content;
                currentGrammar.CurrentDepth--;

                res.RuleName = parent.RuleName;

                res.Content = RuleParser.ReplaceFirst(res.Content, match.Groups[0].Value, generatedSymbol);

                // Symbol
                if(match.Groups[2].Success)
                {
                    res.Symbols.Add(match.Groups[2].Value.TrimStart(':'), generatedSymbol);
                }
                match = match.NextMatch();
            }

            // Register symbols if any
            foreach (var symbol in res.Symbols)
            {
                GrammarRule newRule = new GrammarRule(symbol.Key, new ConstGenerator(symbol.Value, ""));
                currentGrammar.AddRule(newRule);
            }

            return res;
        }

        public override int GetSubgenCount()
        {
            return m_subgens;
        }
    }
    /// <summary>
    /// Generator used to generate many 
    /// </summary>
    [Generator("many")]
    public class MultipleGenerator : Generator
    {
        static int s_count;
        int m_minAmount;
        int m_maxAmount;
        public MultipleGenerator(string baseName, string options)
            : base(baseName, options)
        {
            string[] opts = Options.Split(',');
            m_minAmount = 0;
            m_maxAmount = 25;
            if (opts.Count() == 1)
            {
                m_maxAmount = Int32.Parse(opts[0]);
            }
            else if (opts.Count() == 2)
            {
                m_minAmount = Int32.Parse(opts[0]);
                m_maxAmount = Int32.Parse(opts[1]);
            }
        }

        public override GenerationResult Generate(GrammarRule parent, Grammar currentGrammar)
        {
            int amount = currentGrammar.DNA.Next(m_minAmount, m_maxAmount);

            ConstGenerator gen = new ConstGenerator(Parameter, "");
            GenerationResult result = new GenerationResult();
            result.RuleName = parent.RuleName;
            result.Content = "";
            for(int i = 0; i < amount; i++)
            {
                currentGrammar.CurrentDepth++;
                var res = gen.Generate(parent, currentGrammar);
                currentGrammar.CurrentDepth--;
                foreach (var kvp in res.Symbols) { result.Symbols.Add(kvp.Key, kvp.Value); }
                result.Content += res.Content;
            }

            return result;
        }

        public override int GetSubgenCount()
        {
            return m_maxAmount;
        }
    }
    /// <summary>
    /// Generator used to generate random integer numbers.
    /// Options : 
    /// @int[min, max]
    /// </summary>
    [Generator("int")]
    public class IntGenerator : Generator
    {
        public IntGenerator(string baseName, string options)
            : base(baseName, options)
        {

        }

        public override GenerationResult Generate(GrammarRule parent, Grammar currentGrammar)
        {
            string[] options = Options.Split(','); 
            int minAmount = 0;
            int maxAmount = 25;
            if (options.Count() == 1)
            {
                maxAmount = Int32.Parse(options[0]);
            }
            else if (options.Count() == 2)
            {
                minAmount = Int32.Parse(options[0]);
                maxAmount = Int32.Parse(options[1]);
            }

            int amount = currentGrammar.DNA.Next(minAmount, maxAmount);

            return new GenerationResult() { RuleName = parent.RuleName, Content = amount.ToString(), Symbols = new Dictionary<string, string>() };
        }

        public override int GetSubgenCount()
        {
            return 0;
        }
    }
    /// <summary>
    /// Generator used to generate unique variable names.
    /// </summary>
    [Generator("name")]
    public class NameGenerator : Generator
    {
        static int s_count;

        public NameGenerator(string baseName, string options) : base(baseName, options)
        {

        }

        public override GenerationResult Generate(GrammarRule parent, Grammar currentGrammar)
        {
            return new GenerationResult() { RuleName = parent.RuleName, Content = Parameter + (s_count++).ToString(), Symbols = new Dictionary<string,string>() };
        }

        public override int GetSubgenCount()
        {
            return 0;
        }
    }
}
