using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Randocode.Grammar
{
    public class Grammar : List<GrammarRule>
    {
        #region Variables
        /// <summary>
        /// Represents additional rules.
        /// </summary>
        Dictionary<string, List<GrammarRule>> m_additionalRules;
        /// <summary>
        /// DNA of the program.
        /// </summary>
        public DNA DNA { get; set; }
        /// <summary>
        /// Indicates the current depth of the execution engine. 
        /// (must be put somewhere else in the future ;D)
        /// </summary>
        public int CurrentDepth { get; set; }
        #endregion

        /// <summary>
        /// Adds a new rule in the grammar.
        /// This can also be done during the rules' execution.
        /// </summary>
        /// <param name="rule"></param>
        public void AddRule(GrammarRule rule)
        {
            if(!m_additionalRules.ContainsKey(rule.RuleName))
            {
                m_additionalRules.Add(rule.RuleName, new List<GrammarRule>());
            }
            m_additionalRules[rule.RuleName].Add(rule);
        }

        public Grammar()
        {
            m_additionalRules = new Dictionary<string, List<GrammarRule>>();
            DNA = new DNA();
            CurrentDepth = 0;
        }



        /// <summary>
        /// Picks a random rule with the given rule name.
        /// </summary>
        public GrammarRule PickRandom(string ruleName)
        {
            int lowest = int.MaxValue;
            GrammarRule lowestRule = null;
            var rules = this.Where(new Func<GrammarRule, bool>((GrammarRule rule) =>
            {
                if (rule.RuleName == ruleName)
                {
                    int subgens = rule.Gen.GetSubgenCount();
                    if (subgens < lowest)
                    {
                        lowest = subgens;
                        lowestRule = rule;
                    }

                    return rule.Gen.GetSubgenCount() <= 1 || CurrentDepth <= 8;
                }
                else
                    return false;
            })).ToList();

            if (lowestRule != null)
                rules.Add(lowestRule);
            // Adds additional rules to the rule set.
            bool canUseAdditional = (rules.Count == 0 | DNA.Next(rules.Count) == 0);
            if(canUseAdditional && m_additionalRules.ContainsKey(ruleName))
            {
                rules.AddRange(m_additionalRules[ruleName]);
            }

            if (rules.Count == 0)
                return new GrammarRule("<unknown_rule>", new ConstGenerator("<unknown_rule:" + ruleName +  ">", ""));

            return rules[DNA.Next(rules.Count)];
        }
    }
}
