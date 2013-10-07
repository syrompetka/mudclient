using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Adan.Client.Common.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public static class FakeXmlParser
    {
        private static Regex _regexXml = new Regex(@"<.*(\/)?.*>", RegexOptions.Compiled);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Parse(string input)
        {
            int nest = 0;
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '<')
                {
                    nest++;
                }
                else if (input[i] == '>')
                {
                    if (nest != 0)
                    {
                        nest--;
                    }
                    else if (nest == 0)
                        sb.Append(input[i]);
                }
                else
                    sb.Append(input[i]);
            }

            return sb.ToString();
        }
    }
}
