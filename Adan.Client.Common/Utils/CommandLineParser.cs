using System;
using System.Collections.Generic;
using CSLib.Net.Annotations;
using CSLib.Net.Diagnostics;
using Adan.Client.Common.Settings;

namespace Adan.Client.Common.Utils
{
    /// <summary>
    /// Utililty class <see cref="CommandLineParser"/> for parse args from command line.
    /// </summary>
    public static class CommandLineParser
    {
        /// <summary>
        /// Get args from command line
        /// </summary>
        /// <param name="input"> Command line </param>
        /// <returns></returns>
        public static string[] GetArgs([NotNull] string input)
        {
            Assert.ArgumentNotNull(input, "input");

            input = input.Trim();

            List<string> args = new List<string>();

            int nest = 0;
            int i = 0;
            int startIndex = 0;

            while (i < input.Length && i != -1 && input[i] != SettingsHolder.Instance.Settings.CommandDelimiter)
            {
                if (input[i] != '{')
                {
                    startIndex = i;

                    if (args.Count > 0)
                    {
                        i = input.IndexOf('{', i);
                        if (i == -1)
                        {
                            args.Add(input.Substring(startIndex, input.Length - startIndex));
                            break;
                        }
                        else
                        {
                            args.Add(input.Substring(startIndex, i - startIndex));
                        }
                    }
                    else
                    {
                        i = input.IndexOfAny(new char[] { ' ', '{' });
                        if (i == -1)
                        {
                            args.Add(input.Substring(startIndex, input.Length));
                            break;
                        }

                        args.Add(input.Substring(startIndex, i - startIndex));
                        i = GetFirstNoneWhitespace(input, i);
                    }
                }
                else
                {
                    startIndex = ++i;

                    while (i < input.Length && !(input[i] == '}' && nest == 0))
                    {
                        if (input[i] == '{')
                        {
                            nest++;
                        }
                        else if (input[i] == '}')
                        {
                            nest--;
                        }

                        i++;
                    }
                    args.Add(input.Substring(startIndex, i - startIndex));

                    if (i == input.Length)
                        break;

                    i++;
                    i = GetFirstNoneWhitespace(input, i);
                }
            }

            return args.ToArray();
        }

        private static int GetFirstNoneWhitespace([NotNull] string input, int index)
        {
            Assert.ArgumentNotNull(input, "input");

            while (index < input.Length)
            {
                if (!Char.IsWhiteSpace(input[index]))
                    return index;

                ++index;
            }

            return -1;
        }
    }
}
