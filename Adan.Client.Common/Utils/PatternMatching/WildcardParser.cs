// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WildcardParser.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the WildcardParser type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Utils.PatternMatching
{
    using System.Text;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using Model;

    /// <summary>
    /// A class to parse wildcards.
    /// </summary>
    public static class WildcardParser
    {
        /// <summary>
        /// Parses the wild card string.
        /// </summary>
        /// <param name="target">The string to parse.</param>
        /// <param name="rootModel">The root model.</param>
        /// <returns>
        /// A reference to root <see cref="WildcardToken"/>.
        /// </returns>
        [NotNull]
        public static PatternToken ParseWildcardString([NotNull] string target, [NotNull]RootModel rootModel)
        {
            Assert.ArgumentNotNullOrWhiteSpace(target, "target");
            Assert.ArgumentNotNull(rootModel, "rootModel");

            int position = 0;
            bool anchored = false;
            PatternToken rootToken = null;
            PatternToken previousToken = null;
            var sb = new StringBuilder();

            if (target[0] == '^')
            {
                position++;
                anchored = true;
            }

            while (position < target.Length)
            {
                if (position < target.Length - 1)
                {
                    if (target[position] == '%' && char.IsDigit(target[position + 1]) && target[position + 1] != '0')
                    {
                        var token = new WildcardToken(target[position + 1] - '0');
                        if (sb.Length != 0)
                        {
                            var constantToken = new ConstantStringToken(previousToken, anchored, sb.ToString());
                            sb.Clear();
                            if (previousToken != null)
                            {
                                previousToken.SetNextToken(constantToken);
                            }

                            previousToken = constantToken;
                            if (rootToken == null)
                            {
                                rootToken = constantToken;
                            }
                        }

                        position += 2;
                        anchored = false;

                        if (previousToken != null)
                        {
                            previousToken.SetNextToken(token);
                        }

                        previousToken = token;
                        if (rootToken == null)
                        {
                            rootToken = token;
                        }

                        continue;
                    }

                    if (target[position] == '$')
                    {
                        var endPosition = target.IndexOf(' ', position);
                        string variableName = endPosition >= position
                                                  ? target.Substring(position + 1, endPosition - position - 1)
                                                  : target.Substring(position + 1);

                        if (sb.Length != 0)
                        {
                            var constantToken = new ConstantStringToken(previousToken, anchored, sb.ToString());
                            sb.Clear();
                            if (previousToken != null)
                            {
                                previousToken.SetNextToken(constantToken);
                            }

                            previousToken = constantToken;
                            if (rootToken == null)
                            {
                                rootToken = constantToken;
                            }

                            anchored = true;
                        }

                        var token = new VariableReferenceToken(rootModel, previousToken, anchored, variableName);
                        position += variableName.Length + 1;

                        if (previousToken != null)
                        {
                            previousToken.SetNextToken(token);
                        }

                        previousToken = token;
                        if (rootToken == null)
                        {
                            rootToken = token;
                        }

                        anchored = true;

                        continue;
                    }
                }

                sb.Append(target[position]);
                position++;
            }

            if (sb.Length != 0)
            {
                var constantToken = new ConstantStringToken(previousToken, anchored, sb.ToString());
                sb.Clear();
                if (previousToken != null)
                {
                    previousToken.SetNextToken(constantToken);
                }

                if (rootToken == null)
                {
                    rootToken = constantToken;
                }
            }

            return rootToken ?? new FaultToken();
        }
    }
}
