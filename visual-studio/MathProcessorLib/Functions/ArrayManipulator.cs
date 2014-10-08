using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathProcessorLib
{
    public static class ArrayManipulator
    {
        static Variables vars = Variables.GetVariables();
        
        public static void CreateFunctions()
        {
            Function.AddFunction("array",    CreateArray);
            Function.AddFunction("vector",   CreateVector);
            Function.AddFunction("vectorin", CreateVector);
            Function.AddFunction("count",    FindCount);
            Function.AddFunction("first",    FindFirst);
            Function.AddFunction("last",     FindLast);
            Function.AddFunction("max",      FindMax);
            Function.AddFunction("min",      FindMin);
            Function.AddFunction("reverse",  Reverse);
            Function.AddFunction("extract",  Extract);
            Function.AddFunction("concat",   Concat);
            Function.AddFunction("part",     Part);            
            Function.AddFunction("sort",     Sorta);
            Function.AddFunction("sortd",    Sortd);
            Function.AddFunction("append",   Append);
            Function.AddFunction("contains", Contains);
            Function.AddFunction("itemat",   ItemAt);
        }

        public static Token ItemAt(string operation, List<Token> arguments)
        {
            if (arguments.Count != 2)
                return Token.Error("Exactly two arguments expected");

            if ((arguments[0].TokenType != TokenType.Matrix && arguments[0].TokenType != TokenType.Vector && arguments[0].TokenType != TokenType.Bool) ||
                (arguments[1].TokenType != TokenType.Matrix && arguments[1].TokenType != TokenType.Vector) ||
                 arguments[1].Count != 1)
                return Token.Error("First argument should be an array or  matrix and second a value");

            try
            {
                if (arguments[0].TokenType == TokenType.Bool)
                    return new Token(TokenType.Bool, arguments[0][(int)arguments[1].FirstValue]);
                else
                    return new Token(TokenType.Vector, arguments[0][(int)arguments[1].FirstValue]);
            }
            catch (Exception)
            {
                return new Token(TokenType.Error, "Index out of bounds");
            }
        }

        public static Token Contains(string operation, List<Token> arguments)
        {
            if (arguments.Count != 2)
                return Token.Error("Exactly two arguments expected");

            if ((arguments[0].TokenType != TokenType.Matrix && arguments[0].TokenType != TokenType.Vector) ||
                (arguments[1].TokenType != TokenType.Matrix && arguments[1].TokenType != TokenType.Vector) ||
                 arguments[1].Count != 1)
                return Token.Error("First argument should be an array or matrix and second a single numeric value");

            if (arguments[0].VectorArray.Contains(arguments[1].FirstValue))
                return new Token(TokenType.Bool, 1, 1);
            else 
                return new Token(TokenType.Bool, 1, 0);
        }

        public static Token Append(string operation, List<Token> arguments)
        {
            if ( arguments.Count < 2)
            {
                return Token.Error ( "At least two parameters required.");
            }
            if (!vars.Contains(arguments[0].TokenName))
                return Token.Error ( "First parameter should be an existing variable (Boolean or numeric). Did you mean to use function concat()?");

            if (arguments[0].TokenType == TokenType.Bool)
            {
                List<double> numbers = new List<double>();
                for (int i = 1; i < arguments.Count; i++)
                {
                    foreach (double d in arguments[i].VectorArray)
                    {
                        numbers.Add(d == 0 ? 0 : 1);
                    }
                }
                vars.AddArrayToToken(arguments[0].TokenName, numbers.ToArray());
            }
            else if (arguments[0].TokenType == TokenType.Vector)
            {
                for (int i = 1; i < arguments.Count; i++)
                {
                    vars.AddArrayToToken(arguments[0].TokenName, arguments[i].VectorArray);
                }
            }
            else
                return Token.Error ( "First parameter can only be of type Boolean or Vector");

            return vars.GetToken(arguments[0].TokenName);
        }

        public static Token Sortd(string operation, List<Token> arguments)
        {
            Token result = Token.Void;
            if (arguments.Count == 1)
            {
                result = new Token(TokenType.Vector, arguments[0].Sort("sortd"));
            }
            else if (arguments.Count > 1)
            {
                List<double> tempList = new List<double>();
                for (int i = 0, j = arguments.Count - 1; i < arguments.Count; i++, j--)
                {
                    if (arguments[i].Count != 1)
                        return Token.Error ( "For more than argument to sortd(), each must contain exactly one value");
                    tempList.Add(arguments[i].FirstValue);
                }
                tempList.Sort();
                tempList.Reverse();
                result = new Token(TokenType.Vector, tempList.ToArray());
            }
            else
            {
                return Token.Error ( "No argument provided");
            }
            return result;
        }

        public static Token Sorta(string operation, List<Token> arguments)
        {
            Token result = null;
            if (arguments.Count == 1)
            {                
                result = new Token(TokenType.Vector, arguments[0].Sort("sorta"));
            }
            else if (arguments.Count > 1)
            {
                List<double> tempList = new List<double>();
                for (int i = 0; i < arguments.Count; i++)
                {
                    if (arguments[i].Count > 1)
                        return Token.Error("For more than argument to sortd(), each must contain exactly one value");
                    tempList.Add(arguments[i].FirstValue);
                }
                tempList.Sort();
                result = new Token(TokenType.Vector, tempList.ToArray());
            }
            else
            {
                return Token.Error("No argument provided");
            }
            return result;
        }

        public static Token FindMax(string operation, List<Token> arguments)
        {
            if (arguments.Count < 1)
                return Token.Error("No argument provided");
            Token result = null;                
            bool modeArray = false;
            foreach (Token t in arguments)
            {
                if (t.TokenType != TokenType.Vector && t.TokenType != TokenType.Matrix)
                    return Token.Error("Only vector or matrix supported by max()");
                if (t.Count > 1)
                {
                    modeArray = true;                    
                }
            }
            if (modeArray)
            {
                double[] numberArray = new double[arguments.Count];
                for (int i = 0; i < arguments.Count; i++)
                {
                    numberArray[i] = arguments[i].Max;
                }
                result = new Token(TokenType.Vector, numberArray);
            }
            else
            {
                double number = double.MinValue;
                foreach (Token t in arguments)
                {
                    if (t.FirstValue > number)
                        number = t.FirstValue;
                }
                result = new Token(TokenType.Vector, number);
            }
            return result;
        }

        public static Token FindMin(string operation, List<Token> arguments)
        {
            if (arguments.Count < 1)
                return Token.Error("No argument provided");
            Token result = null;
            bool modeArray = false;
            foreach (Token t in arguments)
            {
                if (t.TokenType != TokenType.Vector && t.TokenType != TokenType.Matrix)
                    return Token.Error ( "All arguments should be of type vector");
                if (t.Count > 1)
                    modeArray = true;
            }
            if (modeArray)
            {
                double[] numberArray = new double[arguments.Count];
                for (int i = 0; i < arguments.Count; i++)
                {
                    numberArray[i] = arguments[i].Min;
                }
                result = new Token(TokenType.Vector, numberArray);
            }
            else
            {
                double number = Double.MaxValue;
                foreach (Token t in arguments)
                {
                    if (t.FirstValue < number)
                        number = t.FirstValue;
                }
                result = new Token(TokenType.Vector, number);
            }
            return result;
        }

        public static Token Concat(string operation, List<Token> arguments)
        {
            if (arguments.Count < 1)
                return Token.Error("No argument provided");

            Token result = null;
            TokenType tt = arguments[0].TokenType;
            foreach (Token t in arguments)
            {
                if (t.TokenType != tt)
                    return Token.Error("Type mismatch in arguments");
            }
            switch (tt)
            {
                case TokenType.Vector:
                    result = ConcatVector(arguments);
                    break;
                case TokenType.Bool:
                    result = ConcatBool(arguments);
                    break;
                case TokenType.Text:
                    result = ConcatString(arguments);
                    break;
                default:
                    return Token.Error("Argument type not supported");
            }
            return result;
        }

        public static Token ConcatVector(List<Token> arguments)
        {            
            List<double> resultVector = new List<double>();            
            foreach (Token t in arguments)
            {                
                resultVector.AddRange(t.VectorArray);
            }
            return new Token(TokenType.Vector, resultVector.ToArray());            
        }

        public static Token ConcatBool(List<Token> arguments)
        {
            List<double> resultVector = new List<double>();
            foreach (Token t in arguments)
            {
                resultVector.AddRange(t.VectorArray);
            }
            return new Token(TokenType.Bool, resultVector.ToArray());            
        }

        public static Token ConcatString(List<Token> arguments)
        {          
            StringBuilder resultStr = new StringBuilder();            
            foreach (Token t in arguments)
            {                
                resultStr.Append(t.StrData);
            }
            return new Token(TokenType.Text, resultStr.ToString());           
        }

        //extracts valid double values (i.e. excludes NaN & infinity) from argument[0] and puts into result's vector
        public static Token Extract(string operation, List<Token> arguments)
        {
            if (arguments.Count != 1 || arguments[0].TokenType != TokenType.Vector)
                return Token.Error("Function expects exactly one parameter of type array");

            List<double> resultVector = new List<double>();
            foreach (double d in arguments[0].VectorArray)
            {
                if (d >= double.MinValue && d <= double.MaxValue)
                {
                    resultVector.Add(d);
                }
            }
            return new Token(TokenType.Vector, resultVector.ToArray());            
        }

        public static Token Reverse(string operation, List<Token> arguments)
        {
            Token result = null;
            if (arguments.Count == 1)
            {
                result = new Token(TokenType.Vector, arguments[0].VectorArray.Reverse().ToArray());
            }
            else if (arguments.Count > 1)
            {
                double[] resultArray = new double[arguments.Count];
                for (int i = 0, j = arguments.Count - 1; i < arguments.Count; i++, j--)
                {
                    if (arguments[i].Count > 1)
                        return Token.Error("One of the arguments is array");
                    resultArray[j] = arguments[i].FirstValue;
                }
                result = new Token(TokenType.Vector, resultArray);
            }
            else
            {
                return Token.Error ( "No parameter provided");
            }
            return result;
        }

        public static Token Part(string operation, List<Token> arguments)
        {
            if (arguments.Count != 2 && arguments.Count != 3)
                return Token.Error("Function expects exactly 2 or 3 parameters");

            TokenType tt = arguments[1].TokenType;
            Token result = null;
            if (tt != TokenType.Vector)
                return Token.Error("Function expects second parameter to be of type vector");

            if (arguments[0].TokenType != TokenType.Vector && arguments[0].TokenType != TokenType.Bool)
                return Token.Error("Function requires first parameter to be of type vector or boolean");

            if (arguments.Count == 3)
            {
                if (arguments[2].TokenType != TokenType.Vector)
                    return Token.Error("Third parameter passed should be of type vector");
            }

            if (arguments.Count == 2)
            {
                if (arguments[1].Count != 1 || arguments[1].FirstValue < 1 || arguments[1].FirstValue > arguments[0].Count - 1)
                    return Token.Error("Offset of range not valid");

                IEnumerable<double> temp = arguments[0].VectorArray.Skip((int)arguments[1].FirstValue);
                if (arguments[0].TokenType == TokenType.Vector)
                    result = new Token(TokenType.Vector, temp.ToArray());
                else
                    result = new Token(TokenType.Bool, temp.ToArray());
            }
            else if (arguments.Count == 3)
            {
                if (arguments[1].Count != 1 || arguments[1].FirstValue < 0 ||
                    arguments[2].Count > 1 || arguments[2].FirstValue < 1 ||
                    arguments[0].Count < arguments[1].FirstValue + arguments[2].FirstValue)
                {
                    return Token.Error ( "Parameter(s) not valid");
                }

                IEnumerable<double> temp = arguments[0].VectorArray.Skip((int)arguments[1].FirstValue);
                if (arguments[0].TokenType == TokenType.Vector)
                    result = new Token(TokenType.Vector, temp.Take((int)arguments[2].FirstValue).ToArray());
                else
                    result = new Token(TokenType.Bool, temp.Take((int)arguments[2].FirstValue).ToArray());
            }           
            return result;
        }

        public static Token CreateVector(string operation, List<Token> arguments)
        {
            if (arguments.Count != 3)
                return Token.Error ( "Function expects exactly 3 parameters");

            for (int i = 0; i < arguments.Count; i++)
            {
                if (arguments[i].TokenType != TokenType.Vector || arguments[i].Count > 1 || arguments[1].FirstValue == 0)
                    return Token.Error ( "Parameter(s) not valid");
            }

            if (arguments[1].FirstValue < 0)
            {
                if (arguments[0].FirstValue < arguments[2].FirstValue)
                    return Token.Error ( "Parameter(s) not valid");
            }
            else
            {
                if (arguments[0].FirstValue > arguments[2].FirstValue)
                    return Token.Error ( "Parameter(s) not valid");
            }

            List<double> doubleList = new List<double>();
            double first = arguments[0].FirstValue;
            double interval = arguments[1].FirstValue;
            double last = arguments[2].FirstValue;
            if (interval < 0)
            {
                do
                {
                    doubleList.Add(first);
                    first += interval;
                } while (first >= last);                
            }
            else
            {
                do
                {
                    doubleList.Add(first);
                    first += interval;
                } while (first <= last);
            }
            if (operation == "vectorin" && doubleList.Last() != last)
            {
                doubleList.Add(last);
            }
            return new Token(TokenType.Vector, doubleList.ToArray());            
        }

        /**********************************************************************************
         * function createArray. Used to create array of double values
         * Parameters:
         *     string operation:        Not used.
         *     List<Token> arguments:   Should have more than 0 Tokens of type TokenType.Vector
         *                              each containing exactly one double value in its vector
         *             On success          * 
         *                              On failure remains untouched
         * 
         * Return value: Success --> TokenType.Vector
         *               Failure --> TokenType.Error
         * ********************************************************************************/
        public static Token CreateArray(string operation, List<Token> arguments)
        {
            //if (arguments.Count < 1)
            //    return Token.Error ( "No parameter provided");

            double[] array = new double[arguments.Count];
            for (int i = 0; i < arguments.Count; i++)
            {
                if (arguments[i].TokenType == TokenType.Vector && arguments[i].Count == 1)
                    array[i] = arguments[i].FirstValue;
                else
                    return Token.Error ( "Parameter(s) not valid");

            }
            return new Token(TokenType.Vector, array);            
        }

        public static Token FindFirst(string operation, List<Token> arguments)
        {
            if (arguments.Count < 1)
                return Token.Error ( "No parameter provided");

            bool modeArray = false;
            TokenType tt = arguments[0].TokenType;

            if (tt != TokenType.Vector && tt != TokenType.Bool)
                return Token.Error ( "Parameter should be either vector or boolean");

            Token result = null;
            foreach (Token t in arguments)
            {
                if (t.TokenType != tt)
                    return Token.Error ( "Parameter type mismatch");
                if (t.Count > 1)
                    modeArray = true;
            }
            if (modeArray)
            {
                double[] numberArray = new double[arguments.Count];
                for (int i = 0; i < arguments.Count; i++)
                {
                    numberArray[i] = arguments[i].FirstValue;
                }
                result = new Token(tt, numberArray);
            }
            else
            {
                result = new Token(tt, arguments[0].FirstValue);
            }
            return result;
        }

        public static Token FindLast(string operation, List<Token> arguments)
        {
            if (arguments.Count < 1)
                return Token.Error ( "No parameter provided");
            bool modeArray = false;
            TokenType tt = arguments[0].TokenType;
            if (tt != TokenType.Vector && tt != TokenType.Bool)
                return Token.Error ( "Function supports only type Boolean or vector");
            Token result = null;
            foreach (Token t in arguments)
            {
                if (t.TokenType != tt)
                    return Token.Error ( "Argument type mismatch");
                if (t.Count > 1)
                    modeArray = true;
            }
            if (modeArray)
            {
                double[] numberArray = new double[arguments.Count];
                for (int i = 0; i < arguments.Count; i++)
                {
                    numberArray[i] = arguments[i].LastValue;
                }
                result = new Token(tt, numberArray);
            }
            else
            {
                result = new Token(tt, arguments.Last().FirstValue);
            }
            return result;
        }

        public static Token FindCount(string operation, List<Token> arguments)
        {
            if (arguments.Count < 1)
                return Token.Error ( "No parameter provided");

            bool modeArray = false;
            foreach (Token t in arguments)
            {
                if (t.TokenType != TokenType.Vector && t.TokenType != TokenType.Bool)
                    return Token.Error ( "Function supports only type Boolean or vector");
                if (t.Count > 1)
                    modeArray = true;
            }
            Token result = null;
            if (modeArray)
            {
                double[] countArray = new double[arguments.Count];
                for (int i = 0; i < arguments.Count; i++)
                {
                    countArray[i] = arguments[i].Count;
                }
                result = new Token(TokenType.Vector, countArray);
            }
            else if (arguments.Count == 1)
            {
                result = new Token(TokenType.Vector, arguments[0].Count);
            }
            else
            {
                result = new Token(TokenType.Vector, arguments.Count);
            }

            return result;
        }
    }
}
