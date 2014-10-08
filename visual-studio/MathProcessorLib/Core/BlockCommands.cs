using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace MathProcessorLib
{
    static class BlockCommands
    {
        static Variables variables = Variables.GetVariables();

        static BlockCommands()
        {
            if (!variables.Contains("maxtime"))
                variables.AddToken(new Token(TokenType.Vector, "maxtime", new double[] { 5000 }));
        }

        public static bool IsBlockCommand(string operation)
        {
            if (operation == "if" || operation == "else" || operation == "repeat" || operation == "while")
                return true;
            else
                return false;
        }

        public static Token Execute(string operation, List<Token> arguments)
        {
            if (!(operation == "if" || operation == "repeat" || operation == "while"))
                return Token.Error("Invalid command");

            if (operation == "if")
            {
                return IfCondition(arguments);
            }
            else if (operation == "repeat")
            {
                return RepeatLoop(arguments);
            }
            else //(operation == "while")
            {
                return WhileLoop(arguments);
            }
        }

        public static Token ExecuteBlock(Token block)
        {
            try
            {
                Token result = null;
                block.TrimToken();
                String tokenString = block.StrData;
                char lastChar = tokenString[tokenString.Length - 1];
                if (lastChar != ':' && lastChar != '}')
                {
                    return Token.Error("Missing ':' or '}'");
                }

                List<string> commands = GetCommandsColonEnd(tokenString);
                if (commands == null)
                {
                    return Token.Error("No valid commands provided");
                }

                for (int i = 0; i < commands.Count; i++)
                {
                    result = Calculator.ProcessCommand(commands[i], true);
                    if (result.TokenType == TokenType.Error || result.TokenType == TokenType.Break)
                    {
                        return result;
                    }
                }
                return Token.Void;
            }
            catch (OutOfMemoryException)
            {
                return Token.Error("Not enough memory to perform the requested operation.");
            }
            catch
            {
                return Token.Error("Error in executing requested operation. Make sure the command exists.");
            }
        }

        public static Token IfCondition(List<Token> arguments)
        {
            if (arguments.Count < 2)
                return Token.Error("Syntax error in use of 'if' construct");

            if (arguments.First().TokenType != TokenType.Bool)
                return Token.Error("Condition specified does not evaluate to Boolean value");

            if (arguments.Count == 2)
            {
                if (arguments.Last().TokenType != TokenType.Block)
                    return Token.Error("Syntax error in use of 'if' construct");
                if (arguments.First().FirstValue == 1)
                    return ExecuteBlock(arguments[1]);
            }
            else if (arguments.Count == 3)
            {
                if (arguments[1].TokenType != TokenType.Block || arguments[2].TokenType != TokenType.Block)
                    return Token.Error("Syntax error in use of 'if' construct");
                if (arguments.First().FirstValue == 1)
                    return ExecuteBlock(arguments[1]);
                else
                    return ExecuteBlock(arguments[2]);
            }
            return Token.Void;
        }

        static Token WhileLoop(List<Token> tokenList)
        {   
            if (tokenList.Count != 2 || tokenList[0].TokenType != TokenType.Text || tokenList[1].TokenType != TokenType.Block)
                return Token.Error("Syntax error in use of 'while' construct");
            tokenList[1].TrimToken();

            char lastChar = tokenList[1].StrData[tokenList[1].StrData.Length - 1];
            if (lastChar != ':' && lastChar != '}')
                return Token.Error("'Missing ':' or '}' in 'while' construct");

            List<string> commands = GetCommandsColonEnd(tokenList.Last().StrData);
            if (commands == null)
                return Token.Error("No commands inside { } block.");

            int tickCount = Environment.TickCount; // we should have a time limit!
            int maxTime = 5000;
            try
            {
                maxTime = (int)variables.GetVector("maxtime").First();
                if (maxTime <= 0)
                    maxTime = 5000;                
            }
            catch (Exception) { }

            List<List<Token>> commandTokenList = new List<List<Token>>();
            List<bool> suppressOutput = new List<bool>();
            Token resultToken = null;
            foreach (string s in commands)
            {
                string trimmedExp;
                bool suppress = Calculator.GetTrimmedExpression(s, out trimmedExp);
                List<Token> tokens;
                resultToken = Tokenizer.TokenizeString(trimmedExp, out tokens);
                if (resultToken.TokenType == TokenType.Error)
                {
                    return Token.Error(resultToken.StrData + " (Expression No. " + (commands.IndexOf(s) + 1) + " )");
                }
                if (tokens.First().TokenType == TokenType.Directive)
                {
                    commandTokenList.Add(tokens);
                }
                else
                {
                    List<Token> postfixTokens = new List<Token>();
                    resultToken = Calculator.InfixToPostfix(tokens, postfixTokens);
                    if (resultToken.TokenType == TokenType.Error)
                    {
                        return Token.Error(resultToken.StrData + " (Expression No. " + (commands.IndexOf(s) + 1) + " )");
                    }
                    commandTokenList.Add(postfixTokens);
                }
                suppressOutput.Add(suppress);
            }
            if (commandTokenList.Count > 0)
            {
                while (true)
                {
                    //resultToken = Calculator.ProcessCommand(tokenList[0].StrData);
                    resultToken = Calculator.ProcessCommand(tokenList[0].StrData);
                    if (resultToken.TokenType != TokenType.Bool)
                    {
                        return Token.Error("The condition expression did not evaluate to a Boolean value. Aborting while loop.");
                    }
                    else if (resultToken.FirstValue == 0)
                    {
                        break;
                    }

                    for (int j = 0; j < commandTokenList.Count; j++)
                    {
                        List<Token> nextCommandList = commandTokenList[j];
                        resultToken = ExecuteCommand(nextCommandList);
                        if (resultToken.TokenType == TokenType.Error)
                            return resultToken;
                        if (resultToken.TokenType == TokenType.Break)
                            return Token.Void;
                        if (!suppressOutput[j])
                        {
                            Calculator.SendIntermediateResult(resultToken);
                        }
                        if (Environment.TickCount - tickCount > maxTime)
                        {
                            return Token.Error("The 'while' loop took longer than the maximum allowed time ( " + maxTime + " ms). Operation aborted. Variables my be corrupt. You may change the value of 'maxtime' to incease/decrease the maximum allowed time for the while loop.");
                        }
                    }                    
                }
            }
            return Token.Void;
        }

        private static Token ExecuteCommand(List<Token> nextCommandList)
        {  
            Token resultToken = Token.Void;
            if (nextCommandList.First().TokenType == TokenType.Directive)
            {
                resultToken = Calculator.ExecuteDirective(nextCommandList);
            }
            else
            {
                resultToken = Calculator.CalculateAndSaveAns(nextCommandList);
            }
            return resultToken;
        }

        static Token RepeatLoop(List<Token> tokenList)
        {   
            if (tokenList.Count != 2 || tokenList.Last().TokenType != TokenType.Block)
                return Token.Error("Syntax error in use of 'repeat' construct");
            tokenList[1].TrimToken();
            int count = (int)tokenList[0].FirstValue;
            if (tokenList[0].FirstValue != count)
                return Token.Error("Count for 'repeat' must be an integer");

            char lastChar = tokenList[1].StrData[tokenList[1].StrData.Length - 1];
            if (lastChar != ':' && lastChar != '}')
                return Token.Error("'Missing ':' or '}' in 'repeat' construct");

            List<string> commands = GetCommandsColonEnd(tokenList.Last().StrData);
            if (commands == null)
                return Token.Error("No commands inside { } block.");

            List<List<Token>> commandTokenList = new List<List<Token>>();
            List<bool> suppressOutput = new List<bool>();
            Token resultToken = null;
            foreach (string s in commands)
            {
                string trimmedExp;
                bool suppress = Calculator.GetTrimmedExpression(s, out trimmedExp);
                List<Token> tokens;
                resultToken = Tokenizer.TokenizeString(trimmedExp, out tokens);
                if (resultToken.TokenType == TokenType.Error)
                {
                    return Token.Error(resultToken.StrData + " (Expression No. " + (commands.IndexOf(s) + 1) + " )");
                }                
                if (tokens.First().TokenType == TokenType.Directive)
                {
                    commandTokenList.Add(tokens);
                }
                else
                {
                    List<Token> postfixTokens = new List<Token>();
                    resultToken = Calculator.InfixToPostfix(tokens, postfixTokens);
                    if (resultToken.TokenType == TokenType.Error)
                    {
                        return Token.Error(resultToken.StrData + " (Expression No. " + (commands.IndexOf(s) + 1) + " )");
                    }
                    commandTokenList.Add(postfixTokens);
                }
                suppressOutput.Add(suppress);
            }
            
            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < commandTokenList.Count; j++)
                {
                    List<Token> nextCommandList = commandTokenList[j];
                    resultToken = ExecuteCommand(nextCommandList);
                    if (resultToken.TokenType == TokenType.Error)
                        return resultToken;
                    if (resultToken.TokenType == TokenType.Break)
                        return Token.Void;
                    if (!suppressOutput[j])
                    {
                        Calculator.SendIntermediateResult(resultToken);
                    }
                }
            }
            return Token.Void;
        }

        static List<string> GetCommandsColonEnd(String str)
        {
            List<string> commands = new List<string>();
            int start = 0;
            for (int i = 0; i < str.Length; i++)
            {
                i = SkipString(i, str);
                if (i < 0)
                    return null;
                if (str[i] == '{')
                {
                    int startedCount = 1;
                    i++;
                    while (i < str.Length)
                    {
                        i = SkipString(i, str);
                        if (i < 0)
                            return null;
                        if (str[i] == '}')
                        {
                            startedCount--;
                            if (startedCount == 0)
                                break;
                        }
                        else if (str[i] == '{')
                        {
                            startedCount++;
                        }
                        i++;
                    }
                    if (startedCount != 0)
                    {
                        return null;
                    }
                }
                i = SkipString(i, str);
                if (i < 0)
                    return null;
                if ((str[i] == ':' || str[i] == '}') && i - start > 0)
                {
                    if (str[i] == '}')
                    {
                        commands.Add(str.Substring(start, i - start + 1));
                    }
                    else
                    {
                        commands.Add(str.Substring(start, i - start));
                    }
                    start = i + 1;
                }
            }
            List<string> temp = new List<string>();
            for (int i = 0; i < commands.Count; i++)
            {
                commands[i] = commands[i].Trim();
                if (commands[i].StartsWith("else"))
                {
                    if (i > 0)
                    {
                        commands[i - 1] = commands[i - 1] + commands[i];
                        temp.RemoveAt(temp.Count - 1);
                        temp.Add(commands[i - 1]);
                        continue;
                    }
                }
                if (commands[i].Length > 0)
                {
                    temp.Add(commands[i]);
                }
            }
            return temp;
        }

        static int SkipString(int i, string str)
        {
            if (str[i] == '\"')
            {
                bool escape = false;
                i++;
                while (i < str.Length)
                {
                    if (str[i] == '\\')
                    {
                        escape = !escape;
                        if (escape)
                        {
                            i++;
                            continue;
                        }
                    }
                    else if (str[i] == '"')
                    {
                        if (!escape)
                        {
                            i++;
                            return i;
                        }
                    }
                    i++;
                    escape = false;
                }
                return -1;
            }
            return i; // will never reach here!
        }
    }
}
