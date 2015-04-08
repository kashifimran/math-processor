using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace MathProcessorLib
{
    public delegate void IntermediateResult(Token result);

    public static class Calculator
    {
        public static event IntermediateResult IntermediateResultProduced = x => { };
        static Variables variables = Variables.GetVariables();
        static int loopDepth = 0;
        public static String DefaultFormatString { get; set; }


        public static void SaveXML(XElement root)
        {
            XElement element = new XElement("mplib");
            element.Add(new XElement("fs", DefaultFormatString));
            variables.SaveXML(element);
            FunctionDefiner.SaveXML(element);
            root.Add(element);
        }

        public static void LoadXML(XElement xe)
        {            
            XElement element = xe.Element("mplib");
            DefaultFormatString = element.Element("fs").Value;
            variables.LoadXML(element);
            FunctionDefiner.LoadXML(element);
        }

        public static void SendIntermediateResult(Token token)
        {
            IntermediateResultProduced(token);
        }

        static Calculator()
        {
            DefaultFormatString = "F04";
        }

        static Token VerifyFormatString(List<Token> tokens)
        {
            if (tokens.Count < 3)
            {
                return Token.Error("Invalid command");
            }
            String formatStr = tokens[1].TokenName;
            double testValue = 0;
            try
            {
                String retValue = testValue.ToString(formatStr);
                if (retValue == tokens[1].TokenName)
                {
                    return Token.Error("Given format string is invalid [ " + retValue + " ]");
                }
            }
            catch (Exception)
            {
                return Token.Error("Given format string is invalid [ " + formatStr + " ]");
            }
            return Token.Void;
        }

        internal static bool GetTrimmedExpression(string inExpression, out string outExpression)
        {
            outExpression = inExpression.Trim();
            bool suppressOutput = false;            
            if (outExpression.Length > 0 && outExpression[outExpression.Length - 1] == ';')
            {
                int i = outExpression.Length - 1;
                int count = 1;
                while (--i >= 0 && (outExpression[i] == ';' || Char.IsWhiteSpace(outExpression[i]))) { count++; }
                outExpression = outExpression.Remove(outExpression.Length - count);
                suppressOutput = true;
            }
            return suppressOutput;
        }

        private static string BlockCommentStrip(string text)
        {
            int i = 0;
            while (i < text.Length - 3) //at least 4 characters for a minimum comment block
            {
                if (text[i] == '"')
                {
                    i++;
                    while (i < text.Length)
                    {
                        if (text[i] == '"' && text[i - 1] != '\\')
                        {
                            break; // skipped a string!
                        }
                        i++;
                    }
                }
                if (i < text.Length - 3 && text[i] == '/' && text[i + 1] == '*')
                {
                    int start = i;
                    i += 2;
                    while (i < text.Length - 1)
                    {
                        if (text[i] == '*' && text[i + 1] == '/')
                        {
                            break;
                        }
                        i++;
                    }
                    if (i < text.Length - 1 && text[i] == '*' && text[i + 1] == '/')
                    {
                        int count = i + 2 - start;
                        text = text.Remove(start, count);
                        i -= count - 1;
                    }
                }
                i++;
            }
            return text;
        }

        static string oldCommentStrip(string commentStart, string commentEnd, string text)
        {
            while (text.IndexOf(commentStart) > -1 && text.IndexOf(commentEnd, text.IndexOf(commentStart) + commentStart.Length) > -1)
            {
                int start = text.IndexOf(commentStart);
                int end = text.IndexOf(commentEnd, start + commentStart.Length);
                text = text.Remove(
                    start,
                    (end + commentEnd.Length) - start
                    );
            }
            return text;
        }

        internal static Token ProcessCommand(string expression, bool isIntermedite)
        {            
            Token resultToken = Token.Void;
            expression = BlockCommentStrip(expression);
            bool suppressOutput = GetTrimmedExpression(expression, out expression);            
            if (expression.Length == 0)
            {
                return resultToken;
            }            
            List<Token> tokens;
            resultToken = Tokenizer.TokenizeString(expression, out tokens);
            if (resultToken.TokenType == TokenType.Error)
            {
                return resultToken;
            }

            if (tokens.First().TokenType == TokenType.Directive)
            {
                resultToken = ExecuteDirective(tokens);
            }
            else
            {
                List<Token> postfixTokens = new List<Token>();
                resultToken = InfixToPostfix(tokens, postfixTokens);
                if (resultToken.TokenType != TokenType.Error)
                {
                    resultToken = CalculateAndSaveAns(postfixTokens);
                    if (resultToken.TokenType != TokenType.Error && isIntermedite && !suppressOutput)
                    {
                        IntermediateResultProduced(resultToken);
                    }
                }
            }
            if (!suppressOutput || resultToken.TokenType == TokenType.Error)
            {
                return resultToken;
            }
            else
            {
                return Token.Void;
            }
        }

        internal static Token ExecuteDirective(List<Token> tokens)
        {
            string name = tokens.First().TokenName;
            tokens.RemoveAt(0);
            return Function.InvokeFunction(name, tokens);
        }

        internal static Token CalculateAndSaveAns (List<Token> postfixTokens)
        {
            Token resultToken = Calculate(postfixTokens);
            if (resultToken.TokenType != TokenType.Error)
            {
                if (resultToken.TokenType == TokenType.Vector || resultToken.TokenType == TokenType.Bool ||
                    resultToken.TokenType == TokenType.Matrix || resultToken.TokenType == TokenType.Text)
                {
                    Token ans = resultToken.Clone();
                    ans.TokenName = "ans";
                    variables.SetToken(ans);
                }
            }
            return resultToken;
        }

        public static Token ProcessCommand(String expression)
        {
            return ProcessCommand(expression, false);
        }

        static public Token InfixToPostfix(List<Token> infixTokens, List<Token> postfixTokens)
        {
            Stack<Token> operatorStack = new Stack<Token>();
            int i = 0;
            int bracketStart = 0;
            bool assignmentValid = false;

            for (; i < infixTokens.Count; i++)
            {
                if (infixTokens[i].TokenType == TokenType.Vector || infixTokens[i].TokenType == TokenType.Text ||
                    infixTokens[i].TokenType == TokenType.Bool || infixTokens[i].TokenType == TokenType.Block ||
                    infixTokens[i].TokenType == TokenType.Break || infixTokens[i].TokenType == TokenType.Matrix ||
                    infixTokens[i].TokenType == TokenType.Custom)
                {
                    if (infixTokens[i].TokenType == TokenType.Break && loopDepth == 0)
                        return Token.Error("break statement can only be inside a loop.");
                    if ((i == 0 || infixTokens[i - 1].TokenName == "="))
                        assignmentValid = true;
                    postfixTokens.Add(infixTokens[i]);
                }
                else if (infixTokens[i].TokenType == TokenType.Operator)
                {
                    if (infixTokens[i].TokenName == "(")
                    {
                        bracketStart++;
                    }
                    else if (infixTokens[i].TokenName == ")")
                    {
                        bracketStart--;
                    }
                    if (infixTokens[i].TokenName != "=")
                        assignmentValid = false;
                    if (infixTokens[i].TokenName == "=")
                    {
                        if (assignmentValid)
                            operatorStack.Push(infixTokens[i]);
                        else
                            return Token.Error("Wrong use of '=' operator.");
                    }
                    else if (operatorStack.Count == 0 || infixTokens[i].TokenName == "(")
                    {
                        operatorStack.Push(infixTokens[i]);
                    }
                    else if (infixTokens[i].TokenName == ")")
                    {
                        while (operatorStack.Count > 0 && operatorStack.Peek().TokenName != "(")
                        {
                            postfixTokens.Add(operatorStack.Pop());
                        }
                        if (operatorStack.Count > 0)
                        {
                            operatorStack.Pop();
                        }
                        else
                        {
                            return Token.Error("Round bracket [()] mismatch.");
                        }
                    }
                    else if (OperatorPriority(operatorStack.Peek().TokenName) >= OperatorPriority(infixTokens[i].TokenName))
                    {
                        if (!infixTokens[i].TokenName.StartsWith("u"))
                        {
                            while (operatorStack.Count > 0 && OperatorPriority(operatorStack.Peek().TokenName) >= OperatorPriority(infixTokens[i].TokenName))
                            {
                                if (operatorStack.Peek().TokenName == "(")
                                    break;
                                postfixTokens.Add(operatorStack.Pop());
                            }
                        }
                        operatorStack.Push(infixTokens[i]);
                    }
                    else
                    {
                        operatorStack.Push(infixTokens[i]);
                    }
                }
                else if (infixTokens[i].TokenType == TokenType.Function || infixTokens[i].TokenType == TokenType.Loop ||
                         infixTokens[i].TokenType == TokenType.Condition ||
                         infixTokens[i].TokenType == TokenType.UserFunction || infixTokens[i].TokenType == TokenType.FunctionDefiner)
                {
                    assignmentValid = false;
                    operatorStack.Push(infixTokens[i++]);
                    if (infixTokens.Count - i < 2)
                        return Token.Error("Expression not valid");
                    if (infixTokens[i++].TokenName != "(")
                    {
                        return Token.Error("Wrong use of [ " + infixTokens[i - 2].TokenName + " ]");
                    }
                    List<Token> subTokenList = new List<Token>();
                    int openCount = 1;
                    int paramCount = 0;
                    bool commaBegin = false;
                    while (i < infixTokens.Count)
                    {
                        if (infixTokens[i].TokenName == ")")
                        {
                            if (openCount != 1)
                                subTokenList.Add(infixTokens[i]);
                            openCount--;
                            if (openCount == 0)
                            {
                                Token token = InfixToPostfix(subTokenList, postfixTokens);
                                if (token.TokenType == TokenType.Error)
                                    return token;
                                subTokenList.Clear();
                                break;
                            }
                        }
                        else if (infixTokens[i].TokenName == "(")
                        {
                            subTokenList.Add(infixTokens[i]);
                            openCount++;
                        }
                        else if (openCount == 1 && infixTokens[i].TokenName == ",")
                        {
                            commaBegin = true;
                            paramCount++;
                            Token token = InfixToPostfix(subTokenList, postfixTokens);
                            if (token.TokenType == TokenType.Error)
                                return token;
                            subTokenList.Clear();
                        }
                        else
                        {
                            commaBegin = false;
                            subTokenList.Add(infixTokens[i]);
                            if (paramCount == 0)
                                paramCount = 1;
                        }
                        i++;
                    }
                    if (openCount != 0 || commaBegin)
                        return Token.Error("Bracket mismatch or bad comma");
                    Token p = operatorStack.Peek();
                    if (p.TokenType == TokenType.Loop || p.TokenType == TokenType.Condition || p.TokenType == TokenType.FunctionDefiner)
                    {
                        i++;
                        if (i < infixTokens.Count && infixTokens[i].TokenType == TokenType.Block)
                        {
                            paramCount++;
                            postfixTokens.Add(infixTokens[i]);
                        }
                        else
                            return Token.Error("Expecting  block of commands inside curly brackets - { }");
                    }
                    //if (p.TokenName == "if" || p.TokenName == "elseif")
                    //{
                    //    if (i + 1 < infixTokens.Count)
                    //    {
                    //        if (infixTokens[i + 1].TokenName == "else")
                    //        {
                    //            i++;
                    //            if (i + 1 == infixTokens.Count)
                    //                return Token.Error("Wrong use of 'else'");
                    //            i++;
                    //            if (infixTokens[i].TokenType != TokenType.Block)
                    //                return Token.Error("'else' should be followed by commands inside curly brackets - { }");
                    //            postfixTokens.Add(infixTokens[i]);
                    //            paramCount++;
                    //        }
                    //    }
                    //}
                    postfixTokens.Add(new Token(TokenType.Vector, paramCount));
                    postfixTokens.Add(operatorStack.Pop());
                }
                else
                {
                    return Token.Error("Un-recognized token");
                }
            }
            if (bracketStart != 0)
                return Token.Error("Bracket mismatch");
            int temp = operatorStack.Count;
            for (i = 0; i < temp; i++)
            {
                postfixTokens.Add(operatorStack.Pop());
            }
            return Token.Void;
        }

        static int OperatorPriority(string operatorStr)
        {
            int priority = 0;
            switch (operatorStr)
            {
                case "(":
                case ")":
                    priority = 35;
                    break;

                case "u+":
                case "u-":
                case "u~":
                    priority = 25;
                    break;

                case "^":
                    priority = 22;
                    break;

                case "*":
                case "/":
                case "%":
                    priority = 20;
                    break;

                case "+":
                case "-":
                case "?":
                case "??":
                    priority = 15;
                    break;

                case "<":
                case ">":
                case "<=":
                case ">=":
                    priority = 12;
                    break;

                case "==":
                case "!=":
                    priority = 10;
                    break;

                case "=":
                    priority = 5;
                    break;

                case ",":
                    priority = 1;
                    break;

                default:
                    throw new ArgumentException("Not an operator");
            }
            return priority;
        }

        public static Token Calculate(List<Token> postfixTokens)
        {
            Stack<Token> operands = new Stack<Token>();
            Token result = Token.Void;
            try
            {
                for (int index = 0; index < postfixTokens.Count; index++)
                {
                    Token t = postfixTokens[index];
                    if (t.TokenType == TokenType.Vector || t.TokenType == TokenType.Text ||
                        t.TokenType == TokenType.Bool || t.TokenType == TokenType.Block ||
                        t.TokenType == TokenType.Break || t.TokenType == TokenType.Matrix ||
                        t.TokenType == TokenType.Custom)
                    {
                        operands.Push(t);
                    }
                    else if (t.TokenType == TokenType.Function || t.TokenType == TokenType.Loop || t.TokenType == TokenType.Condition ||
                             t.TokenType == TokenType.UserFunction || t.TokenType == TokenType.FunctionDefiner)
                    {
                        int paramCount = (int)(operands.Pop().FirstValue);
                        List<Token> parameters = new List<Token>();
                        for (int i = 0; i < paramCount; i++)
                        {
                            parameters.Add(operands.Pop());
                        }
                        parameters.Reverse();
                        Token resultToken;
                        if (t.TokenType == TokenType.Function)
                        {
                            resultToken = Function.InvokeFunction(t.TokenName, parameters);
                            if (resultToken.TokenType == TokenType.Error)
                                return resultToken;
                        }
                        else if (t.TokenType == TokenType.UserFunction)
                        {
                            resultToken = FunctionDefiner.ExecuteUserFunction(t.TokenName, parameters);
                            if (resultToken.TokenType == TokenType.Error)
                                return resultToken;
                        }
                        else if (t.TokenType == TokenType.Loop)
                        {
                            loopDepth++;
                            resultToken = BlockCommands.ExecuteLoop(t.TokenName, parameters);
                            if (resultToken.TokenType == TokenType.Error)
                            {
                                loopDepth = 0;
                                return resultToken;
                            }
                            loopDepth--;
                        }
                        else if (t.TokenType == TokenType.Condition)
                        {
                            Token param = Token.Void;
                            if (t.TokenName == "elseif" || t.TokenName == "else")
                            {
                                param = operands.Pop();
                            }
                            resultToken = BlockCommands.ExecuteCondition(t.TokenName, param, parameters);
                            if (resultToken.TokenType == TokenType.Error)
                                return resultToken;
                        }
                        else
                        {
                            if (operands.Count == 0)
                            {
                                return Token.Error("User function must be assigned to a name");
                            }
                            resultToken = FunctionDefiner.CreateUserFunction(parameters);
                            if (resultToken.TokenType == TokenType.Error)
                                return resultToken;
                        }
                        operands.Push(resultToken);
                    }
                    else if (operands.Count >= 1 && t.TokenName.StartsWith("u"))
                    {
                        if (operands.Peek().TokenType != TokenType.Vector && operands.Peek().TokenType != TokenType.Bool &&
                            operands.Peek().TokenType != TokenType.Matrix)
                            return Token.Error("Operand not valid for [ " + t.TokenName + " ]");
                        Token firstParam = operands.Pop();
                        Token temp = null;
                        if (!DoCalculateUnary(t.TokenName, firstParam, out temp))
                            return Token.Error("Operand not valid for [ " + t.TokenName + " ]");
                        operands.Push(temp);
                    }
                    else if (operands.Count > 1)
                    {
                        Token temp = null;
                        Token firstParam = operands.Pop();
                        Token secondParam = operands.Pop();
                        switch (t.TokenName)
                        {
                            case "*":
                            case "/":
                            case "%":
                            case "+":
                            case "-":
                            case "^":
                            case "==":
                            case "!=":
                            case "?":
                            case "??":
                            case "<":
                            case ">":
                            case ">=":
                            case "<=":
                                if (firstParam.TokenType == TokenType.Text && secondParam.TokenType == TokenType.Text)
                                {
                                    temp = new Token(TokenType.Bool, 1, firstParam.StrData == secondParam.StrData ? 1 : 0);
                                }
                                else if (firstParam.TokenType == TokenType.Custom && secondParam.TokenType == TokenType.Custom &&
                                         firstParam.IsOfType(typeof(StringsToken)) && secondParam.IsOfType(typeof(StringsToken)))
                                {
                                    List<string> first = (List<string>)firstParam.CustomData;
                                    List<string> second = (List<string>)secondParam.CustomData;
                                    if (first.Count == second.Count)
                                    {
                                        for (var i = 0; i < first.Count; i++)
                                        {
                                            if (first[i] != second[i])
                                            {
                                                temp = new Token(TokenType.Bool, 1, 0);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        temp = new Token(TokenType.Bool, 1, 0);
                                    }
                                    temp = new Token(TokenType.Bool, 1, 1);

                                }
                                else if ((firstParam.TokenType != TokenType.Vector && firstParam.TokenType != TokenType.Bool && firstParam.TokenType != TokenType.Matrix) ||
                                    (secondParam.TokenType != TokenType.Vector && secondParam.TokenType != TokenType.Bool && secondParam.TokenType != TokenType.Matrix)
                                   )
                                {
                                    return Token.Error("Operands not valid for [ " + t.TokenName + " ]. Both must be the same type.");
                                }
                                else
                                {
                                    temp = DoCalculate(t.TokenName, secondParam, firstParam);
                                }
                                if (temp.TokenType == TokenType.Error) //yep stack is upside down!
                                    return temp;
                                break;

                            case "=":
                                if (firstParam.TokenType == TokenType.Void || string.IsNullOrEmpty(secondParam.TokenName))
                                {
                                    return Token.Error("Assignment not valid.");
                                }
                                secondParam.ResetValues(firstParam);
                                if (variables.Contains(secondParam.TokenName))
                                {
                                    temp = variables.SetTokenVerify(secondParam);
                                }
                                else
                                {
                                    temp = variables.AddTokenVerify(secondParam);
                                }
                                if (temp.TokenType == TokenType.Error)
                                {
                                    return temp;
                                }
                                else
                                {
                                    temp = secondParam;
                                }
                                if (secondParam.TokenType == TokenType.UserFunction)
                                {
                                    if (operands.Count == 0)
                                    {
                                        return new Token(TokenType.Text, "", "User function named [ " + temp.TokenName + " ] added");
                                    }
                                    else
                                    {
                                        return Token.Error("Bad definition of User function");
                                    }
                                }
                                break;
                            default:
                                return Token.Error("Error in expression");
                        }
                        operands.Push(temp);
                    }
                    else
                    {
                        return Token.Error("Error in expression");
                    }
                }
                if (operands.Count != 1)
                    return Token.Error("Error in expression");
                result = operands.Pop();
                if (result.TokenType == TokenType.Block)
                {
                    result = BlockCommands.ExecuteBlock(result);
                    if (result.TokenType != TokenType.Error)
                        return Token.Void;
                }
                else if (result.TokenType == TokenType.Operator || result.TokenType == TokenType.Loop || result.TokenType == TokenType.Condition)
                    return Token.Error("Error in expression");
            }
            catch (Exception)
            {
                return Token.Error("Error in expression");
            }
            if (result.TokenType == TokenType.ConditionExecuted)
            {
                return Token.Void;
            }
            return result;
        }

        static bool DoCalculateUnary(string operation, Token firstParam, out Token result)
        {
            result = null;
            if (firstParam.TokenType == TokenType.Bool)
            {
                if (operation == "u~")
                {
                    double[] temp = firstParam.VectorArray;
                    for (int i = 0; i < firstParam.Count; i++)
                    {
                        if (temp[i] == 0)
                            temp[i] = 1;
                        else
                            temp[i] = 0;
                    }
                    result = new Token(TokenType.Bool, temp);
                }
                else
                {
                    return false;
                }
            }
            else if (firstParam.TokenType == TokenType.Vector)
            {
                result = new Token(TokenType.Vector, firstParam.VectorArray);
                if (operation == "u-")
                {
                    double[] vector = firstParam.VectorArray;
                    for (int i = 0; i < vector.Count(); i++)
                    {
                        vector[i] = -vector[i];
                    }
                    result.VectorArray = vector;
                }
                else if (operation == "u~")
                {
                    return false;
                }
            }
            else
            {
                result = new Token(TokenType.Matrix, firstParam.Extra, firstParam.VectorArray);
                if (operation == "u-")
                {
                    double[] vector = firstParam.VectorArray;
                    for (int i = 0; i < vector.Count(); i++)
                    {
                        vector[i] = -vector[i];
                    }
                    result.VectorArray = vector;
                }
            }
            return true;
        }

        static Token DoCalculate(string operation, Token firstParam, Token secondParam)
        {
            if (firstParam.TokenType == TokenType.Matrix || secondParam.TokenType == TokenType.Matrix)
            {
                return DoCalculateMatrix(operation, firstParam, secondParam);
            }
            if (firstParam.TokenType != secondParam.TokenType)
            {
                return Token.Error("Operation not valid. Operands have Different Types");
            }
            if (operation == "==" || operation == "!=" || operation == "<" || operation == ">" ||
                operation == "<=" || operation == ">="
               )
            {
                return CompareParams(operation, firstParam, secondParam);
            }
            else if (firstParam.TokenType == TokenType.Vector)
            {
                return DoArithmetic(operation, firstParam, secondParam);
            }
            else
            {
                return DoBoolean(operation, firstParam, secondParam);
            }
        }

        static Token CompareParams(string operation, Token firstParam, Token secondParam)
        {
            if (firstParam.Count < 1 || firstParam.Count != secondParam.Count)
                return Token.Error("Either first parameter does not have a numeric value or count of operands not same.");
            if (firstParam.Count > 1)
                return CompareParamArrays(operation, firstParam, secondParam);
            else
                return CompareParamValues(operation, firstParam, secondParam);
        }

        static Token CompareParamArrays(string operation, Token firstParam, Token secondParam)
        {
            int value = 0;  //0 means false
            int i = 0;
            switch (operation)
            {
                case "==":
                    value = 1;
                    for (; i < firstParam.Count; i++)
                    {
                        if (firstParam[i] != secondParam[i])
                        {
                            value = 0;
                            break;
                        }
                    }
                    break;
                case "!=":
                    for (; i < firstParam.Count; i++)
                    {
                        if (firstParam[i] != secondParam[i])
                        {
                            value = 1;
                            break;
                        }
                    }
                    break;
                case "<":
                    for (; i < firstParam.Count; i++)
                    {
                        if (firstParam[i] < secondParam[i])
                        {
                            value = 1;
                            break;
                        }
                    }
                    break;
                case ">":
                    for (; i < firstParam.Count; i++)
                    {
                        if (firstParam[i] > secondParam[i])
                        {
                            value = 1;
                            break;
                        }
                    }
                    break;
                case "<=":
                    value = 1;
                    for (; i < firstParam.Count; i++)
                    {
                        if (firstParam[i] > secondParam[i])
                        {
                            value = 0;
                            break;
                        }
                    }
                    break;
                case ">=":
                    value = 1;
                    for (; i < firstParam.Count; i++)
                    {
                        if (firstParam[i] < secondParam[i])
                        {
                            value = 0;
                            break;
                        }
                    }
                    break;
                default:
                    return Token.Error("Operation not valid");
            }
            return new Token(TokenType.Bool, value);
        }

        static Token CompareParamValues(string operation, Token firstParam, Token secondParam)
        {
            int result = 0;
            switch (operation)
            {
                case "==":
                    if (firstParam.FirstValue == secondParam.FirstValue)
                        result = 1;
                    break;
                case "!=":
                    if (firstParam.FirstValue != secondParam.FirstValue)
                        result = 1;
                    break;
                case "<":
                    if (firstParam.FirstValue < secondParam.FirstValue)
                        result = 1;
                    break;
                case ">":
                    if (firstParam.FirstValue > secondParam.FirstValue)
                        result = 1;
                    break;
                case "<=":
                    if (firstParam.FirstValue <= secondParam.FirstValue)
                        result = 1;
                    break;
                case ">=":
                    if (firstParam.FirstValue >= secondParam.FirstValue)
                        result = 1;
                    break;
                default:
                    return Token.Error("Operation not valid");
            }
            return new Token(TokenType.Bool, 1, result);
        }

        static double PerformBoolOperation(string operation, double firstNumber, double secondNumber)
        {
            bool firstValue = (firstNumber == 0) ? false : true;
            bool secondValue = (secondNumber == 0) ? false : true;

            switch (operation)
            {
                case "+":
                    firstValue = firstValue | secondValue;
                    break;

                case "*":
                    firstValue = firstValue & secondValue;
                    break;

                case "^":
                    firstValue = firstValue ^ secondValue;
                    break;

                case "?":
                    firstValue = (!firstValue) | secondValue;
                    break;

                case "??":
                    firstValue = ((!firstValue) | secondValue) & ((!secondValue) | firstValue);
                    break;

                default:
                    throw new ArgumentException("Operation no supported");
            }
            return firstValue ? 1 : 0;
        }

        static Token DoBoolean(string operation, Token firstParam, Token secondParam)
        {
            int firstCount = firstParam.Count;
            int secondCount = secondParam.Count;

            if (firstCount == 0 || secondCount == 0)
                return Token.Error("Either first parameter does not have a boolean value or count of operands not same.");

            double[] bools = new double[Math.Max(firstCount, secondCount)];

            if (firstCount >= 1 && secondCount == 1)
            {
                for (int i = 0; i < firstCount; i++)
                {
                    bools[i] = PerformBoolOperation(operation, firstParam[i], secondParam.FirstValue);
                }
            }
            else if (firstCount == 1 && secondCount >= 1)
            {
                for (int i = 0; i < secondCount; i++)
                {
                    bools[i] = PerformBoolOperation(operation, firstParam.FirstValue, secondParam[i]);
                }
            }
            else
            {
                if (firstCount != secondCount)
                    return Token.Error("First and second parameters do not have equal number of elements.");

                for (int i = 0; i < secondCount; i++)
                {
                    bools[i] = PerformBoolOperation(operation, firstParam[i], secondParam[i]);
                }
            }
            return new Token(TokenType.Bool, bools);
        }

        static Token DoArithmetic(string operation, Token firstParam, Token secondParam)
        {
            int firstCount = firstParam.Count;
            int secondCount = secondParam.Count;

            if (firstCount == 0 || secondCount == 0)
                return Token.Error("Either first or second or both parameter(s) have 0 elements.");

            double[] numbers = new double[Math.Max(firstCount, secondCount)];

            if (firstCount >= 1 && secondCount == 1)
            {
                for (int i = 0; i < firstCount; i++)
                {
                    numbers[i] = PerformArithmeticOperation(operation, firstParam[i], secondParam.FirstValue);
                }
            }
            else if (firstCount == 1 && secondCount >= 1)
            {
                for (int i = 0; i < secondCount; i++)
                {
                    numbers[i] = PerformArithmeticOperation(operation, firstParam.FirstValue, secondParam[i]);
                }
            }
            else
            {
                if (firstCount != secondCount)
                    return Token.Error("First and second parameters do not have equal number of elements.");

                for (int i = 0; i < secondCount; i++)
                {
                    numbers[i] = PerformArithmeticOperation(operation, firstParam[i], secondParam[i]);
                }
            }
            return new Token(TokenType.Vector, numbers);
        }

        static double PerformArithmeticOperation(string operation, double firstNumber, double secondNumber)
        {
            switch (operation)
            {
                case "+":
                    return firstNumber + secondNumber;
                case "-":
                    return firstNumber - secondNumber;
                case "*":
                    return firstNumber * secondNumber;
                case "/":
                    return firstNumber / secondNumber;
                case "^":
                    return Math.Pow(firstNumber, secondNumber);
                case "%":
                    return firstNumber % secondNumber;
                default:
                    throw new ArgumentException("Operation no supported");
            }
        }

        static Token DoCalculateMatrix(string operation, Token firstParam, Token secondParam)
        {
            /* Scalar operations*/
            Token result = null;
            if (firstParam.TokenType == TokenType.Vector ||
                (firstParam.TokenType == TokenType.Matrix && firstParam.Count == 1))
            {
                if (firstParam.Count != 1)
                    return Token.Error("First parameter is an array. Scalar operation not possible. Pass single value");
                double[] temp = new double[secondParam.Count];
                for (int i = 0; i < secondParam.Count; i++)
                {
                    temp[i] = PerformArithmeticOperation(operation, firstParam.FirstValue, secondParam[i]);
                }
                double extra = firstParam.TokenType == TokenType.Matrix ? firstParam.Extra : secondParam.Extra;
                result = new Token(TokenType.Matrix, extra, temp);
            }
            else if (secondParam.TokenType == TokenType.Vector ||
                     (secondParam.TokenType == TokenType.Matrix && secondParam.Count == 1))
            {
                if (secondParam.Count != 1)
                    return Token.Error("Second parameter is an array. Scalar operation not possible. Pass single value");
                double[] temp = new double[firstParam.Count];
                for (int i = 0; i < firstParam.Count; i++)
                {
                    temp[i] = PerformArithmeticOperation(operation, firstParam[i], secondParam.FirstValue);
                }
                double extra = firstParam.TokenType == TokenType.Matrix ? firstParam.Extra : secondParam.Extra;
                result = new Token(TokenType.Matrix, extra, temp);
            }
            else /* Both operands are Matrix*/
            {
                /*        /, % and ^ return error!           */
                if (operation == "^" || operation == "%" || operation == "/")
                    return Token.Error("'" + operation + "' not defined for Matrices");

                /* "<" ,  ">" , "<=" and ">=" not used for matrices
                 */
                if (operation == "==" || operation == "!=")
                {
                    if (firstParam.Count != secondParam.Count ||
                        firstParam.Extra != secondParam.Extra)
                    {
                        return Token.Error("Argument matrices are not compatible for comparison.");
                    }
                    return CompareParams(operation, firstParam, secondParam);
                }

                double[] temp = new double[firstParam.Count];
                if (operation == "+" || operation == "-")
                {
                    if (firstParam.Count < 1 || secondParam.Count != firstParam.Count ||
                        firstParam.Extra != secondParam.Extra)
                        return Token.Error("Argument(s) not valid for the specified operation");

                    for (int i = 0; i < firstParam.Count; i++)
                    {
                        temp[i] = PerformArithmeticOperation(operation, firstParam[i], secondParam[i]);
                    }
                    result = new Token(TokenType.Matrix, firstParam.Extra, temp);
                }
                else if (operation == "*")
                {
                    int firstRows = (int)firstParam.Extra;
                    int firstCols = (int)((firstParam.Count) / firstParam.Extra);
                    int secondCols = (int)((secondParam.Count) / secondParam.Extra);

                    if (firstCols != secondParam.Extra)
                        return Token.Error("Columns of first matrix do not equal rows of second matrix");

                    double[] data = new double[firstRows * secondCols];
                    double value = 0;
                    for (int i = 0; i < firstRows; i++)
                    {
                        for (int j = 0; j < secondCols; j++)
                        {
                            value = 0;
                            for (int k = 0; k < firstCols; k++)
                            {
                                value += firstParam[i * firstCols + k] * secondParam[j + secondCols * k];
                            }
                            data[i * secondCols + j] = value;
                        }
                    }
                    result = new Token(TokenType.Matrix, firstRows, data);
                }
            }
            return result;
        }
    }
}
