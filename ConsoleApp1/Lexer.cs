using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace ConsoleApp1
{
    public class Lexer
    {
        public Token go(String lexerString, int position)
        {
            Regex regexIdentifier = new Regex(@"^\s*[a-zA-Z|_][a-z|A-Z|\d|_]*\s*(?:\s|$)");
            Regex regexUnsignedIntegerBinary = new Regex(@"^\s*\%[0|1]+\s*(?:\s|$)");
            Regex regexUnsignedIntegerOctal = new Regex(@"^\s*\&[0-8]+\s*(?:\s|$)");
            Regex regexUnsignedIntegerHex = new Regex(@"^\s*\$(?:\d|[A-Fa-f])+\s*(?:\s|$)");
            Regex regexUnsignedIntegerDecimal = new Regex(@"^\s*\d+\s*(?:\s|$)");
            Regex regexUnsignedReal = new Regex(@"^\s*\d*\.\d+(?:[E|e][\+|\-]?\d+)?\s*(?:\s|$)");
            Regex regexSignedNumber =
                new Regex(
                    @"(^\s*[\+|\-]\d*\.\d+(?:[E|e][\+|\-]?\d+)?\s*(?:\s|$)|^\s*[\+|\-]\%[0|1]+\s*(?:\s|$)|^\s*[\+|\-]\&[0-8]+\s*(?:\s|$)|^\s*[\+|\-]\$(?:\d|[A-Fa-f])+\s*(?:\s|$)|\s*[\+|\-]\d+\s*(?:\s|$))");
            Regex regexCharacterString =
                new Regex(
                    @"(\s*((\'(?:[^\']|[\']{2})*\')*|((#\%[0|1]+|#\&[0-8]+|#\$(?:\d|[A-Fa-f])+|#\d+))*)*\s*(?:\s|$))");
            Regex[] allRegexes =
            {
                regexIdentifier,
                regexUnsignedIntegerBinary,
                regexUnsignedIntegerOctal,
                regexUnsignedIntegerHex,
                regexUnsignedIntegerDecimal,
                regexUnsignedReal,
                regexSignedNumber,
                regexCharacterString
            };

            String curSubString = lexerString.Substring(position, lexerString.Length - position).Trim();

            if (curSubString.StartsWith("//"))
            {
                String considerString;
                if (curSubString.Contains('\n'))
                {
                    considerString = curSubString.Substring(0, curSubString.IndexOf('\n'));
                }
                else
                {
                    considerString = curSubString;
                } // либо строчку до перевода корректки, либо всю строку до конца

                //осталось проверить является ли она правильно скобочной последовательностью
                Stack<String> rightSequenceBrackets = new Stack<string>();
                for (int i = 0; i < considerString.Length; i++)
                {
                    if (considerString[i] == '{')
                    {
                        rightSequenceBrackets.Push("{");
                    }

                    if (i < considerString.Length - 1 && considerString[i] == '(' && considerString[i + 1] == '*')
                    {
                        rightSequenceBrackets.Push("(*");
                    }

                    if (considerString[i] == '}')
                    {
                        if (rightSequenceBrackets.Count == 0)
                        {
                            return null;
                        }

                        String top = rightSequenceBrackets.Peek();
                        if (String.Compare("{", top) != 0)
                        {
                            return null; //неправильная последовательность
                        }

                        rightSequenceBrackets.Pop();
                    }

                    if (i < considerString.Length - 1 && considerString[i] == '*' && considerString[i + 1] == ')')
                    {
                        if (rightSequenceBrackets.Count == 0)
                        {
                            return null;
                        }

                        String top = rightSequenceBrackets.Peek();
                        if (String.Compare("(*", top) != 0)
                        {
                            return null; //неправильная последовательность
                        }

                        rightSequenceBrackets.Pop();
                    }
                }

                if (rightSequenceBrackets.Count != 0)
                {
                    return null;
                }

                return new Token(considerString.Trim(), TokenType.Comments, considerString.Length);
            }

            if (curSubString.StartsWith("{") || curSubString.StartsWith("(*"))
            {
                Stack<String> checkSeq = new Stack<string>();
                int curIndex;
                StringBuilder returnString = new StringBuilder();
                if (curSubString.StartsWith("{"))
                {
                    checkSeq.Push("{");
                    curIndex = 1;
                    returnString.Append("{");
                }
                else
                {
                    checkSeq.Push("(*");
                    returnString.Append("(*");
                    curIndex = 2;
                }
                while (checkSeq.Count != 0)
                {
                    if (curIndex == curSubString.Length)
                    {
                        break;
                    }

                    if (curSubString[curIndex] == '{')
                    {
                        checkSeq.Push("{");
                    }

                    if (curIndex < curSubString.Length - 1 && curSubString[curIndex] == '(' &&
                        curSubString[curIndex + 1] == '*')
                    {
                        checkSeq.Push("(*");
                    }

                    if (curSubString[curIndex] == '}')
                    {
                        String top = checkSeq.Peek();

                        if (String.Compare("{", top) != 0)
                        {
                            return null;
                        }

                        checkSeq.Pop();
                    }

                    if ( curSubString[curIndex] == ')' &&
                        curSubString[curIndex - 1] == '*')
                    {
                        String top = checkSeq.Peek();

                        if (String.Compare("(*", top) != 0)
                        {
                            return null;
                        }

                        checkSeq.Pop();
                    }

                    returnString.Append(curSubString[curIndex]);
                    curIndex++;
                }

                if (checkSeq.Count != 0)
                {
                    return null;
                }

                return new Token(returnString.ToString().Trim(), TokenType.Comments, returnString.Length);
            }


            for (int i = 0; i < allRegexes.Length; i++)
            {
                //Console.WriteLine(allRegexes[i].Match(curSubString).Value);
                if (allRegexes[i].Match(curSubString).Success &&
                    curSubString.StartsWith(allRegexes[i].Match(curSubString).Value))
                {
                    Console.WriteLine(i);
                    String matchPart = allRegexes[i].Match(curSubString).Value;
                    if (i == 0)
                    {
                        return new Token(matchPart.Trim(), TokenType.Identifiers, matchPart.Length);
                    }

                    if (i > 0 && i <= 6)
                    {
                        return new Token(matchPart.Trim(), TokenType.Numbers, matchPart.Length);
                    }

                    if (i == 7)
                    {
                        return new Token(matchPart.Trim(), TokenType.CharacterString, matchPart.Length);
                    }
                }
            }

            return null;
        }
    }
}