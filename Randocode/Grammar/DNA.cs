using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Randocode.Grammar
{
    /// <summary>
    /// Represents the 'DNA' of each program.
    /// </summary>
    public class DNA
    {
        Random m_rand = new Random();
        public int Next(int minValue, int maxValue)
        {
            return m_rand.Next(minValue, maxValue);
        }

        public int Next(int maxValue)
        {
            return m_rand.Next(maxValue);
        }
    }
}
