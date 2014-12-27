using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace MathProcessorLib
{
    enum OpEnum { Yes, No, YesKeepGo };

    static class Tokenizer
    {
        static Variables variables = Variables.GetVariables();        

        public static Token TokenizeString(String expression, out List<Token> tokens) //str --> a trimmed string
        {
            tokens = new List<Token>();
            int i = 0;
            String nextTokenStr;
            bool unaryFlag = true;
            while (i < expression.Length)
            {
                nextTokenStr = "";
                while (i < expression.Length && Char.IsWhiteSpace(expression[i])) i++;
                if (i == expression.Length)
                    break;
                nextTokenStr += expression[i++].ToString();
                OpEnum opEnum = IsOperator(nextTokenStr);
                if (opEnum == OpEnum.Yes || opEnum == OpEnum.YesKeepGo)
                {
                    if (opEnum == OpEnum.YesKeepGo)
                    {
                        if (i < expression.Length)
                        {
                            String tempStr = nextTokenStr + expression[i].ToString();
                            if (IsOperator(tempStr) == OpEnum.Yes)
                            {
                                tokens.Add(new Token(TokenType.Operator, tempStr));
                                i++;
                                unaryFlag = true;
                                continue;
                            }
                        }
                        if (nextTokenStr == "!")
                        {
                            nextTokenStr = "~";
                        }
                    }
                    if (((nextTokenStr == "+" || nextTokenStr == "-") && unaryFlag) || nextTokenStr == "~")
                    {
                        tokens.Add(new Token(TokenType.Operator, "u" + nextTokenStr));
                    }
                    else
                    {
                        tokens.Add(new Token(TokenType.Operator, nextTokenStr));
                    }
                    if (expression[i - 1] != ')')
                        unaryFlag = true;
                    else
                        unaryFlag = false;
                    continue;
                }
                else
                {
                    unaryFlag = false;
                }
                if (nextTokenStr == "\"")
                {
                    i = SkipString(i, ref nextTokenStr, expression, false);
                    if (i < 0)
                    {
                        return Token.Error("Bad input. Input not valid: [ " + nextTokenStr + " ]");
                    }
                    tokens.Add(new Token(TokenType.Text, nextTokenStr.Substring(1, nextTokenStr.Length - 2)));
                    continue;
                }
                if (nextTokenStr == "{")
                {
                    int startedCount = 1;
                    while (i < expression.Length)
                    {
                        nextTokenStr += expression[i++].ToString();
                        if (expression[i - 1] == '\"')
                        {
                            i = SkipString(i, ref nextTokenStr, expression, true);
                            if (i < 0)
                            {
                                return Token.Error("Bad input. Input not valid: [ " + nextTokenStr + " ]");
                            }
                            continue;
                        }

                        if (expression[i - 1] == '}')
                        {
                            startedCount--;
                            if (startedCount == 0)
                                break;
                        }
                        else if (expression[i - 1] == '{')
                        {
                            startedCount++;
                        }
                    }
                    if (startedCount != 0)
                    {
                        return Token.Error("Bad input. Bracket mismatch :[ " + nextTokenStr + " ]");
                    }
                    tokens.Add(new Token(TokenType.Block, "", nextTokenStr.Substring(1, nextTokenStr.Length - 2)));
                    continue;
                }
                while (i < expression.Length && !Char.IsWhiteSpace(expression[i]) && (IsOperator(expression[i].ToString()) == OpEnum.No) && expression[i] != '"' && expression[i] != '{')
                {
                    nextTokenStr += expression[i++].ToString();
                }
                if (Char.IsDigit(nextTokenStr[0]) || nextTokenStr[0] == '.')
                {
                    try
                    {
                        Double.Parse(nextTokenStr);
                        tokens.Add(new Token(TokenType.Vector, Double.Parse(nextTokenStr)));
                    }
                    catch (Exception)
                    {
                        return Token.Error("Bad input: [ " + nextTokenStr + " ]");
                    }
                }
                else if (variables.Contains(nextTokenStr))
                {
                    tokens.Add(variables.GetToken(nextTokenStr));
                }
                else
                {
                    if (Function.IsDirective(nextTokenStr))
                        tokens.Add(new Token(TokenType.Directive, nextTokenStr));
                    else if (Function.IsFuction(nextTokenStr))
                        tokens.Add(new Token(TokenType.Function, nextTokenStr));
                    else if (BlockCommands.IsBlockCommand(nextTokenStr))
                    {
                        tokens.Add(new Token(TokenType.LoopOrCondition, nextTokenStr));
                        if (nextTokenStr == "while")
                        {
                            nextTokenStr = "";
                            while (i < expression.Length && Char.IsWhiteSpace(expression[i])) i++;
                            if (i == expression.Length || expression[i++] != '(')
                            {
                                return Token.Error("Wrong use of 'while' statement.");
                            }
                            int closeCount = -1;
                            while (i < expression.Length)
                            {
                                if (expression[i] == ')')
                                {
                                    ++closeCount;
                                }
                                else if (expression[i] == '(')
                                {
                                    --closeCount;
                                }
                                if (closeCount == 0)
                                {
                                    i++;
                                    break;
                                }
                                nextTokenStr += expression[i++].ToString();
                            }
                            if (closeCount == 0)
                            {
                                tokens.Add(new Token(TokenType.Text, "("));
                                tokens.Add(new Token(TokenType.Text, nextTokenStr, nextTokenStr));
                                tokens.Add(new Token(TokenType.Text, ")"));
                            }
                            else
                            {
                                return Token.Error("Wrong use of 'while' statement. () mismatch.");
                            }
                        }
                    }
                    else if (nextTokenStr == "break")
                    {
                        tokens.Add(new Token(TokenType.Break, nextTokenStr));
                    }
                    else if (nextTokenStr == "function")
                    {
                        tokens.Add(new Token(TokenType.FunctionDefiner, nextTokenStr));
                    }
                    else
                    {
                        tokens.Add(new Token(TokenType.Text, nextTokenStr));
                    }
                }
            }
            return Token.Void;
        }

        static int SkipString(int i, ref string nextTokenStr, string str, bool keepEscape)
        {
            bool escape = false;
            while (i < str.Length)
            {
                if (str[i] == '\\')
                {
                    escape = !escape;
                    if (escape)
                    {
                        if (keepEscape)
                        {
                            nextTokenStr += str[i].ToString();
                        }
                        i++;
                        continue;
                    }
                }
                else if (str[i] == '"')
                {
                    if (!escape)
                    {
                        nextTokenStr += str[i++].ToString();
                        return i;
                    }
                }
                nextTokenStr += str[i++].ToString();
                escape = false;
            }
            return -1;
        }

        static string[] operators = { "%", "+", "-", "/", "*", "(", ")", ",", "^", "<=", ">=", "==", "!=", "~", "??" };
        static OpEnum IsOperator(string s)
        {
            if (operators.Contains(s))
            {
                return OpEnum.Yes;
            }
            else if (s == "!" || s == "=" || s == "?" || s == "<" || s == ">")
            {
                return OpEnum.YesKeepGo;
            }
            return OpEnum.No;
        }
    }
}
