using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathProcessorLib
{
    public static class BasicCalculations
    {        
        public static void CreateFunctions()
        {
            Function.AddFunction("sum",     FindSum);
            Function.AddFunction("prod",    FindProd);
            Function.AddFunction("fact",    FindFactorial);
            Function.AddFunction("sqrt",    FindSqrt);
            Function.AddFunction("rem",     FindRem);
            Function.AddFunction("IEEERem", FindRem);
            Function.AddFunction("gcd",     GCD);
            Function.AddFunction("lcm",     LCM);            
        }

        public static Token FindRem (string operation, List<Token> arguments)
        {
            if (arguments.Count != 2)
                return Token.Error ( "Function expects exactly two arguments");

            Token input1 = arguments[0];
            Token input2 = arguments[1];
            
            Token result = null;
            if (input1.Count >= 1 && input2.Count == 1)
            {
                double[] values = input1.VectorArray;
                for (int i = 0; i < values.Count(); i++)
                {
                    values[i] = CalculateRem(operation, values[i], input2.FirstValue);
                }
                result = new Token(TokenType.Vector, values);
            }

            else if (input1.Count == 1 && input2.Count >= 1)
            {
                double[] values = input2.VectorArray;
                for (int i = 0; i < values.Count(); i++)
                {
                    values[i] = CalculateRem(operation, input1.FirstValue, values[i]);
                }
                result = new Token(TokenType.Vector, values);
            }

            else if (input1.Count > 1 && input2.Count > 1)
            {
                double[] resultVector = input1.VectorArray;
                double[] inputVector = input2.VectorArray;
                if (resultVector.Count() != inputVector.Count())
                    return Token.Error ( "Argument(s) no valid");

                for (int i = 0; i < resultVector.Count(); i++)
                {
                    resultVector[i] = CalculateRem(operation, resultVector[i], inputVector[i]);
                }
                result = new Token(TokenType.Vector, resultVector);

            }
            return result;
        }

        static double CalculateRem(string operation, double input1, double input2)
        {
            if (operation == "rem")
            {
                input1 = Math.Abs(Math.IEEERemainder(input1, input2));
            }
            else 
            {
                input1 = Math.IEEERemainder(input1, input2);
            }            
            return input1;
        }

        public static Token FindSqrt(string operation, List<Token> arguments)
        {
            if (arguments.Count < 1)
                return Token.Error ( "No argument given");

            double[] temp = null;
            if (arguments.Count == 1)
            {
                temp = new double [arguments[0].Count];
                for (int i = 0; i < temp.Count(); i++)
                {
                    temp[i] = Math.Sqrt(arguments[0][i]);
                }                
            }
            else
            {
                temp = new double[arguments.Count];
                for (int i = 0; i < arguments.Count; i++)
                {
                    if (arguments[i].Count > 1)
                        return Token.Error ( "Argument(s) no valid");
                    temp[i] = Math.Sqrt(arguments[i].FirstValue);
                }                    
            }
            return new Token(TokenType.Vector, temp);           
        }

        public static Token GCD(string operation, List<Token> arguments)
        {
            if (arguments.Count < 1)
                return Token.Error ( "Argument(s) no valid");

            bool modeArray = false;
            foreach (Token t in arguments)
            {
                if (t.TokenType != TokenType.Vector)
                    return Token.Error ( "Argument(s) not valid");
                if (t.Count > 1)
                    modeArray = true;
            }
            List<int> numbers = new List<int>();
            Token result = null;
            if (modeArray)
            {
                double[] input;
                double[] outPut = new double[arguments.Count];
                foreach (Token t in arguments)
                {
                    if (t.Count < 2)
                        return Token.Error ( "Argument(s) not valid");
                    input = t.VectorArray;
                    numbers.Clear();
                    foreach (double d in input)
                    {
                        if ((int)d != d)
                            return Token.Error ( "Argument(s) not valid");
                        if (!numbers.Contains((int)d))
                        {
                            numbers.Add((int)d);
                        }
                    }
                    for (int i = 1; i < numbers.Count; i++)
                    {
                        numbers[i] = DoGCD(numbers[i - 1], numbers[i]);
                    }
                    outPut[arguments.IndexOf(t)] = numbers.Last();
                }
                result = new Token(TokenType.Vector, outPut);
            }
            else
            {
                if (arguments.Count < 2)
                    return Token.Error ( "Argument(s) not valid");
                foreach (Token t in arguments)
                {
                    if ((int)t.FirstValue != t.FirstValue)
                        return Token.Error ( "Argument(s) not valid");
                    if (!numbers.Contains((int)t.FirstValue))
                    {
                        numbers.Add((int)t.FirstValue);
                    }
                }
                for (int i = 1; i < numbers.Count; i++)
                {
                    numbers[i] = DoGCD(numbers[i - 1], numbers[i]);
                }
                result = new Token(TokenType.Vector, numbers.Last());
            }
            return result;
        }

        static int DoGCD(int firstNum, int secondNum)
        {
            if (firstNum == 0 && secondNum == 0)
            {
                return 0;
            }
            if (firstNum < secondNum || secondNum == 0)
            {
                int temp = secondNum;
                secondNum = firstNum;
                firstNum = temp;
            }
            int gcd = firstNum % secondNum;
            while (gcd != 0)
            {
                firstNum = secondNum;
                secondNum = gcd;
                gcd = firstNum % secondNum;
            }
            return Math.Abs(secondNum);
        }

        public static Token LCM(string operation, List<Token> arguments)
        {
            if (arguments.Count < 1)
                return Token.Error ( "No argument");

            bool modeArray = false;
            foreach (Token t in arguments)
            {
                if (t.TokenType != TokenType.Vector)
                    return Token.Error ( "Argument(s) not valid");
                if (t.Count > 1)
                    modeArray = true;
            }
            List<int> numbers = new List<int>();
            Token result = null;
            if (modeArray)
            {
                double[] input;
                double[] outPut = new double[arguments.Count];
                foreach (Token t in arguments)
                {
                    if (t.Count < 2)
                        return Token.Error ( "Argument(s) not valid");
                    input = t.VectorArray;
                    numbers.Clear();
                    bool hasZero = false;
                    foreach (double d in input)
                    {
                        if (d < 0 || (int)d != d)
                            return Token.Error ( "Argument(s) not valid");
                        if (d == 0)
                        {
                            hasZero = true;
                            break;
                        }
                        if (!numbers.Contains((int)d))
                        {
                            numbers.Add((int)d);
                        }
                    }
                    if (hasZero)
                    {
                        outPut[arguments.IndexOf(t)] = 0;
                        continue;
                    }
                    long product = 0;
                    for (int i = 1; i < numbers.Count; i++)
                    {
                        product = numbers[i] * numbers[i - 1];
                        numbers[i] = (int)(product / DoGCD(numbers[i], numbers[i - 1]));
                    }
                    outPut[arguments.IndexOf(t)] = numbers.Last();
                }
                result = new Token(TokenType.Vector, outPut);
            }
            else
            {
                if (arguments.Count < 2)
                    return Token.Error ( "Argument(s) not valid");

                foreach (Token t in arguments)
                {                    
                    if (t.FirstValue < 0 || (int)t.FirstValue != t.FirstValue)
                        return Token.Error ( "Argument(s) not valid");

                    if (t.FirstValue == 0)
                    {
                        return new Token(TokenType.Vector,0);                        
                    }
                }
                for (int i = 0; i < arguments.Count; i++)
                {
                    if (!numbers.Contains((int)arguments[i].FirstValue))
                    {
                        numbers.Add((int)arguments[i].FirstValue);
                    }
                }
                long product = 0;
                for (int i = 1; i < numbers.Count; i++)
                {
                    product = numbers[i] * numbers[i - 1];
                    numbers[i] = (int)(product / DoGCD(numbers[i], numbers[i - 1]));
                }
                result = new Token(TokenType.Vector, numbers.Last());
            }
            return result;
        }

        public static Token FindSum(string operation, List<Token> arguments)
        {
            if (arguments.Count < 1)
                return Token.Error ( "No argument");

            bool modeArray = false;
            Token result = null;
            foreach (Token t in arguments)
            {
                if (t.TokenType != TokenType.Vector)
                    return Token.Error ( "Argument(s) not valid");
                if (t.Count > 1)
                    modeArray = true;
            }
            if (modeArray)
            {
                double[] numberArray = new double[arguments.Count];
                for (int i = 0; i < arguments.Count; i++)
                {
                    numberArray[i] = arguments[i].Sum;
                }
                result = new Token(TokenType.Vector, numberArray);
            }
            else
            {
                double number = 0;
                foreach (Token t in arguments)
                {
                    number += t.FirstValue;
                }
                result = new Token(TokenType.Vector, number);
            }
            return result;
        }

        public static Token FindProd(string operation, List<Token> arguments)
        {
            if (arguments.Count < 1)
                return Token.Error ( "No argument");

            bool modeArray = false;
            Token result = null;
            foreach (Token t in arguments)
            {
                if (t.TokenType != TokenType.Vector)
                    return Token.Error ( "Some argument(s) not vector");
                if (t.Count > 1)
                    modeArray = true;
            }
            if (modeArray)
            {
                double[] numberArray = new double[arguments.Count];
                for (int i = 0; i < arguments.Count; i++)
                {
                    numberArray[i] = arguments[i].Prod;
                }
                result = new Token(TokenType.Vector, numberArray);
            }
            else
            {
                double number = 1;
                foreach (Token t in arguments)
                {
                    number *= t.FirstValue;
                }
                result = new Token(TokenType.Vector, number);
            }
            return result;
        }

        public static Token FindFactorial(string operation, List<Token> arguments)
        {
            if (arguments.Count < 1)
                return Token.Error ( "No argument");

            double[] numberArray = null;            
            if (arguments.Count == 1)
            {
                if (arguments[0].TokenType != TokenType.Vector)
                    return Token.Error ( "Argument not vector");

                numberArray = new double[arguments[0].Count];
                for (int i = 0; i < arguments[0].Count; i++)
                {
                    if (arguments[0][i] == 0 || arguments[0][i] == 1)
                        numberArray[i] = 1;
                    else
                    {
                        double value = 1;
                        for (int j = (int)(arguments[0][i]); j > 1; j--)
                        {
                            try
                            {
                                value *= j;
                            }
                            catch (OverflowException)
                            {
                               return Token.Error ( "Overflow");
                            }
                        }
                        numberArray[i] = value;
                    }
                }
            }
            else
            {
                foreach (Token t in arguments)
                {
                    if (t.TokenType != TokenType.Vector || t.Count > 1 || t.FirstValue < 0)
                        return Token.Error ( "Argument(s) not valid");
                }
                numberArray = new double[arguments.Count];
                double value = 1;
                for (int i = 0; i < arguments.Count; i++)
                {
                    if (arguments[i].FirstValue == 0 || arguments[i].FirstValue == 1)
                        numberArray[i] = 1;
                    else
                    {
                        value = 1;
                        for (int j = (int)(arguments[i].FirstValue); j > 1; j--)
                        {
                            try
                            {
                                value *= j;
                            }
                            catch (OverflowException)
                            {
                                return Token.Error ( "Overflow");
                            }
                        }
                        numberArray[i] = value;
                    }
                }
            }
            return new Token(TokenType.Vector, numberArray);            
        }
    }
}
