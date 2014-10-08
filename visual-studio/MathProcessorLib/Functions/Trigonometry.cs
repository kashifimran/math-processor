using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathProcessorLib
{
    public static class Trigonometry
    {        
        public static void CreateFunctions()
        {            
            Function.AddFunction("acos",    TrignometricFunc);
            Function.AddFunction("asin",    TrignometricFunc);
            Function.AddFunction("atan",    TrignometricFunc);
            Function.AddFunction("sin",     TrignometricFunc);
            Function.AddFunction("cos",     TrignometricFunc);
            Function.AddFunction("tan",     TrignometricFunc);
            Function.AddFunction("sinh",    TrignometricFunc);
            Function.AddFunction("cosh",    TrignometricFunc);
            Function.AddFunction("tanh",    TrignometricFunc);
            Function.AddFunction("rad2deg", Convert);
            Function.AddFunction("deg2rad", Convert); 
        }

        public static Token Convert(string operation, List<Token> arguments)
        {
            if (arguments.Count != 1)
                return Token.Error ( "Exactly 1 argument expected");

            Token result = Token.Error("Invalid parameter");
            if (arguments[0].TokenType == TokenType.Vector)
            {
                double[] resultVector = new double[arguments[0].Count];
                for (int i = 0; i < resultVector.Count(); i++)
                {
                    if (operation == "deg2rad")
                    {
                        resultVector[i] = arguments[0][i] * (Math.PI / 180);
                    }
                    else
                    {
                        resultVector[i] = arguments[0][i] / (Math.PI / 180);
                    }
                     
                }
                result = new Token(TokenType.Vector, resultVector);
            }
            return result;
        }

        public static Token TrignometricFunc(string operation, List<Token> arguments)
        {
            if (arguments.Count < 1)
                return Token.Error ( "No argument");

            Token result = null;
            if (arguments.Count == 1)
            {
                if (arguments[0].TokenType != TokenType.Vector && arguments[0].TokenType != TokenType.Matrix)
                    return Token.Error ( "First argument should be a vector (array) or matrix");
                result = CreateTrigToken(operation, arguments[0]);
            }
            else
            {
                double[] resultArray = new double[arguments.Count];
                for (int i = 0; i < arguments.Count; i++)
                {
                    if ((arguments[i].TokenType != TokenType.Vector && arguments[i].TokenType != TokenType.Matrix) || arguments[i].Count != 1)
                        return Token.Error ( "Argument(s) not valid");
                    resultArray[i] = CreateTrigToken(operation, arguments[i]).FirstValue;
                }
                result = new Token(TokenType.Vector, resultArray);
            }
            return result;
        }

        public static Token CreateTrigToken(string funcName, Token input)
        {   
            if (input.TokenType == TokenType.Vector || input.TokenType == TokenType.Matrix)
            {
                Token result = input.Clone();
                for (int i = 0; i < result.Count; i++)
                {
                    result[i] = CalculateTrigValue(funcName, input.VectorArray[i]);
                }
                return result;
            }
            return Token.Error("Invalid command.");
        }

        public static double CalculateTrigValue(string operation, double input)
        {
            if (Function.Unit == AngleUnit.Degree)
                input = input * (Math.PI / 180);

            switch (operation)
            {
                case "sin":
                    input = Math.Sin(input);
                    break;
                case "cos":
                    input = Math.Cos(input);
                    break;
                case "tan":
                    input = Math.Tan(input);
                    break;
                case "sinh":
                    input = Math.Sinh(input);
                    break;
                case "cosh":
                    input = Math.Cosh(input);
                    break;
                case "tanh":
                    input = Math.Tanh(input);
                    break;
                case "asin":
                    input = Math.Asin(input);
                    break;
                case "acos":
                    input = Math.Acos(input);
                    break;
                case "atan":
                    input = Math.Atan(input);
                    break;
                default:
                    throw new ArgumentException("Request invalid");
            }
            return input;
        }       
    }
}
