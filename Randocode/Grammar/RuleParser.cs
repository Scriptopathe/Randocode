using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
namespace Randocode.Grammar
{
    /// <summary>
    /// Class used to create rules based on strings.
    /// </summary>
    public class RuleParser
    {
        static Regex s_main = new Regex(@"([a-z]*):(.*)" , RegexOptions.Singleline);
        static Regex s_genRegex = new Regex(@"@([a-z]*)(?:\[([^\(]*)\])?\((.*)\)", RegexOptions.Singleline);
        /// <summary>
        /// Parses one rule on a single line.
        /// </summary>
        public static GrammarRule ParseRule(string str)
        {
            Match match = s_main.Match(str);
            if(match.Success)
            {
                string key = match.Groups[1].Value;
                string value = match.Groups[2].Value;
                Generator gen = CreateGeneratorByString(value);
                if(gen != null)
                {
                    // use given generator.
                    return new GrammarRule(key, gen);
                }
                else
                {
                    // Use default generator.
                    return new GrammarRule(key, new ConstGenerator(value, ""));
                }
            }
            return null;
        }

        /// <summary>
        /// Parses a grammar in a line separated list of rules.
        /// Newlines can be escaped with a backslash in the string.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Grammar ParseGrammar(string str)
        {
            // Split the string into lines (takes escaping characters into account)
            str = str.Replace("\r\n", "\n");
            List<string> lines = new List<string>();
            List<char> currentLine = new List<char>();
            int i;
            for(i = 0; i< str.Count() - 1; i++)
            {

                if (str[i] == '\\' && str[i + 1] == '\n')
                {
                    currentLine.Add('\n');
                    i++;
                }
                else if (str[i] == '\n')
                {
                    if (currentLine.Count != 0)
                    {
                        lines.Add(new String(currentLine.ToArray()));
                        currentLine.Clear();
                    }
                }
                else
                {
                    currentLine.Add(str[i]);
                }
            }
            currentLine.Add(str[i]);
            if(currentLine.Count != 0)
                lines.Add(new String(currentLine.ToArray()));

            // Creates the grammar
            Grammar g = new Grammar();
            foreach(string line in lines)
            {
                var rule = ParseRule(line.Replace("\\n", "\n"));
                if(rule != null)
                    g.Add(rule);
            }
            return g;
        }

        /// <summary>
        /// Creates a generator from a string with the given syntax :
        /// @gentype[options](parameters)
        /// </summary>
        public static Generator CreateGeneratorByString(string name)
        {
            Regex reg2 = new Regex(@"@([a-z]*)(?:\[([^\(]*)\])?\((.*)\)", RegexOptions.Singleline);
            Match match = reg2.Match(name);

            if(match.Success)
            {
                string genName = match.Groups[1].Value;
                string genParameter = match.Groups[3].Value;
                string genOptions = match.Groups[2].Value;
                Type t = GetGeneratorByName(genName);
                return (Generator)Activator.CreateInstance(t, new object[] { genParameter, genOptions });
            }

            return null;
        }

        public static string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }
        /// <summary>
        /// Gets the generator registered with the given name.
        /// Registration is made by using the GeneratorAttribute.
        /// </summary>
        public static Type GetGeneratorByName(string name)
        {
            System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            Type[] types = a.GetTypes();
            foreach(Type t in types)
            {

                object[] attributes = t.GetCustomAttributes(false).Where(new Func<object,bool>((object o) => {
                    return o is GeneratorAttribute;
                })).ToArray();
                if(attributes.Count() != 0)
                {
                    GeneratorAttribute attr = (GeneratorAttribute)attributes.First();
                    if(attr.GeneratorKey == name)
                    {
                        return t;
                    }
                }
            }
            return null;
        }
    }
}
