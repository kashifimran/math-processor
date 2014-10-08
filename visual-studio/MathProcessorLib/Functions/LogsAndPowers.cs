using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathProcessorLib
{
    public static class LogsAndPowers
    {        
        public static void CreateFunctions()
        {
            Function.AddFunction("lg",  FindLogExponent);
            Function.AddFunction("ln",  FindLogExponent);
            Function.AddFunction("exp", FindLogExponent);
            Function.AddFunction("pow", CalculatePowLog);
            Function.AddFunction("log", CalculatePowLog);
        }
     
        static Token FindLogExponent (string operation, List<Token> arguments)
        {
            if (arguments.Count != 1)
                return Token.Error ( "Function expects single parameter");
            Token result = Token.Error ( "Argument not numeric");
            if (arguments[0].TokenType == TokenType.Vector || arguments[0].TokenType == TokenType.Matrix)
            {
                double[] resultVector = new double[arguments[0].Count];
                switch (operation)
                {
                    case "lg":
                        for (int i = 0; i < resultVector.Count(); i++)
                        {
                            resultVector[i] = Math.Log(arguments[0].VectorArray[i], 2);
                        }
                        break;

                    case "ln":
                        for (int i = 0; i < resultVector.Count(); i++)
                        {
                            resultVector[i] = Math.Log(arguments[0].VectorArray[i]);
                        }
                        break;

                    case "exp":
                        for (int i = 0; i < resultVector.Count(); i++)
                        {
                            resultVector[i] = Math.Exp(arguments[0].VectorArray[i]);
                        }
                        break;
                }                
                result = new Token(arguments[0].TokenType,arguments[0].Extra, resultVector);

            }
            return result;
        }

        static Token CalculatePowLog(string operation, List<Token> arguments)
        {
            if (arguments.Count < 1)
                return Token.Error ( "No arguments passed.");

            Token result = Token.Error ( "Argument not numeric");

            if (arguments.Count == 1 && operation == "log")
            {
                double[] values = arguments[0].VectorArray;
                for (int i = 0; i < values.Count(); i++)
                {
                    values[i] = Math.Log10(values[i]);
                }
                result = new Token(arguments[0].TokenType, arguments[0].Extra, values);
            }
            else if (arguments[0].Count >= 1 && arguments[1].Count == 1)
            {
                double[] values = arguments[0].VectorArray;
                for (int i = 0; i < values.Count(); i++)
                {
                    if (operation == "log")
                    {
                        values[i] = Math.Log(values[i], arguments[1].FirstValue);
                    }
                    else
                    {
                        values[i] = Math.Pow(values[i], arguments[1].FirstValue);
                    }
                }
                result = new Token(arguments[0].TokenType, arguments[0].Extra, values);
            }

            else if (arguments[0].Count == 1 && arguments[1].Count >= 1)
            {
                double[] values = arguments[1].VectorArray;
                for (int i = 0; i < values.Count(); i++)
                {
                    if (operation == "log")
                    {
                        values[i] = Math.Log(arguments[0].FirstValue, values[i]);
                    }
                    else
                    {
                        values[i] = Math.Pow(arguments[0].FirstValue, values[i]);
                    }
                }
                result = new Token(arguments[1].TokenType, arguments[1].Extra, values);
            }

            else if (arguments[0].Count > 1 && arguments[1].Count > 1)
            {
                double[] resultVector = arguments[0].VectorArray;
                double[] inputVector = arguments[1].VectorArray;
                if (resultVector.Count() != inputVector.Count())
                    Token.Error ( "Argument mismatch");

                for (int i = 0; i < resultVector.Count(); i++)
                {
                    if (operation == "log")
                    {
                        resultVector[i] = Math.Log(resultVector[i], inputVector[i]);
                    }
                    else
                    {
                        resultVector[i] = Math.Pow(resultVector[i], inputVector[i]);
                    }
                }
                result = new Token(arguments[0].TokenType, arguments[0].Extra, resultVector);
            }
            return result;
        }        
    }
}
