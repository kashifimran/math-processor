using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathProcessorLib
{
    public static class Statistics
    {
        
        public static void CreateFunctions()
        {
            Function.AddFunction("avg",     FindAvg);
            Function.AddFunction("mode",    Mode);
            Function.AddFunction("median",  Median);
            Function.AddFunction("pnr",     Pnr);
            Function.AddFunction("cnr",     Cnr);            
        }

        public static Token Pnr(string operation, List<Token> arguments)
        {
            if (arguments.Count != 2)
                return Token.Error ( "Function expects exactly 2 arguments");

            if (arguments[0].Count < 1                                  || 
                arguments[1].Count < 1                                  ||
                arguments[0].Count != arguments[1].Count                ||
                arguments[0].FirstValue != (int)arguments[0].FirstValue ||
                arguments[1].FirstValue != (int)arguments[1].FirstValue ||
                arguments[0].FirstValue < 0                             ||
                arguments[1].FirstValue < 0                             ||
                arguments[0].FirstValue < arguments[1].FirstValue                
                )
                return Token.Error ( "Argument(s) not valid");

            List<Token> factList = new List<Token>();
            factList.Add(arguments[0]);
            factList.Add(new Token(TokenType.Vector, arguments[0].FirstValue-arguments[1].FirstValue));
            Token result = BasicCalculations.FindFactorial("fact", factList);
            return new Token(TokenType.Vector, result[0] / result[1]);            
        }

        public static Token Cnr(string operation, List<Token> arguments)
        {
            if (arguments.Count != 2)
                return Token.Error ( "Function expects exactly 2 arguments");
            if ( arguments[0].Count < 1                                  ||
                 arguments[1].Count < 1                                  ||
                 arguments[0].Count != arguments[1].Count                ||
                 arguments[0].FirstValue != (int)arguments[0].FirstValue ||
                 arguments[1].FirstValue != (int)arguments[1].FirstValue ||
                 arguments[0].FirstValue < 0                             ||
                 arguments[1].FirstValue < 0                             ||
                 arguments[0].FirstValue < arguments[1].FirstValue
                 )
                return Token.Error ( "Argument(s) not valid");

            List<Token> factList = new List<Token>();
            factList.Add(arguments[0]);
            factList.Add(arguments[1]);
            factList.Add(new Token(TokenType.Vector, arguments[0].FirstValue - arguments[1].FirstValue));
            Token result = BasicCalculations.FindFactorial("fact", factList);
            return new Token(TokenType.Vector, result[0] / (result[1]*result[2]));            
        }

        public static Token FindAvg(string operation, List<Token> arguments)
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
                    numberArray[i] = arguments[i].Average;
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
                result = new Token(TokenType.Vector, number / arguments.Count);
            }
            return result;
        }

        public static Token CalculateMode(double[] data)
        {
            Dictionary<double, int> occurCount = new Dictionary<double, int>();
            foreach (double d in data)
            {
                if (!occurCount.Keys.Contains(d))
                {
                    occurCount.Add(d, 0);
                }
                else
                {
                    occurCount[d] = occurCount[d] + 1;
                }
            }
            List<double> modeList = new List<double>();
            int max = occurCount.Values.Max();
            if (max > 0)
            {
                foreach (KeyValuePair<double, int> kvp in occurCount)
                {
                    if (kvp.Value == max)
                        modeList.Add(kvp.Key);
                }
            }
            else
            {
                return new Token(TokenType.Vector, double.NaN);
            }
            return new Token(TokenType.Vector, modeList.ToArray());

        }


        public static Token Mode(string operation, List<Token> arguments)
        {
            if (arguments.Count < 1)
            {
                return Token.Error ( "No argument");
            }
            double [] data = null;            
            if (arguments.Count ==1)
            {
                data = arguments[0].VectorArray;
            }            
            else
            {
                data = new double[arguments.Count];
                for (int i = 0; i < arguments.Count; i++ )
                {
                    if (arguments[i].Count != 1)
                        return Token.Error ( "In valid argument(s)");
                    data[i] = arguments[i].FirstValue;
                }                
            }
            return CalculateMode(data);            
        }

        public static Token CalculateMedian(double[] data)
        {
            int even = data.Count() % 2;
            List<double> temp = new List<double>();
            temp.AddRange(data);
            temp.Sort();
            double median = 0;
            if (data.Count() > 1)
            {
                if (even == 0)
                {
                    median = (temp[data.Count() / 2 - 1] + temp[data.Count() / 2]) / 2.0;
                }
                else
                {
                    median = temp[data.Count() / 2];
                }
            }
            else
            {
                median = temp[0];
            }            
            return new Token(TokenType.Vector, median);
        }

        public static Token Median(string operation, List<Token> arguments)
        {
            if (arguments.Count < 1)
            {
                return Token.Error ( "No argument");
            }
            double[] data = null;
            if (arguments.Count == 1)
            {
                data = arguments[0].VectorArray;
            }
            else
            {
                data = new double[arguments.Count];
                for (int i = 0; i < arguments.Count; i++)
                {
                    if (arguments[i].Count != 1)
                        return Token.Error ( "Argument(s) not valid");
                    data[i] = arguments[i].FirstValue;
                }
            }
            return CalculateMedian(data);            
        }
    }
}
