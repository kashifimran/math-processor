using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathProcessorLib
{
    static class Testbed
    {
        public static void CreateFunctions()
        {
            Function.AddFunction("iscong", IsCongruent);
            //Function.AddFunction("primesto", CreatePrimesArray4);
            //Function.AddFunction("primes3",  CreatePrimesArray3);
            //Function.AddFunction("primes2",  CreatePrimesArray2);
            //Function.AddFunction("primeat2", FindPrimeAt2);
        }

        // FindPrimeAt will return a Token of TokenType.Vector which will indicate the prime at a given index
        // Originally implemented by c0dejunkie, refined by KashifImran  
        public static Token FindPrimeAt2(string operation, List<Token> arguments)
        {
            if (arguments.Count != 1 || arguments[0].TokenType != TokenType.Vector || arguments[0].Count != 1 ||
                arguments[0].FirstValue < 1 || arguments[0].FirstValue != (int)arguments[0].FirstValue)
            {
                return Token.Error("Invalid argument to function. Please provide a positive integer");
            }

            // an index of 1 will return 2 as the prime number
            int primeIndex = (int)arguments[0].FirstValue;
            int primeCount = 0;
            double primeAt = 1;

            if (primeIndex == 1)
            {
                primeAt = 2;
            }
            else
            {
                primeCount = 1;
                List<Token> testL = new List<Token>();
                Token primeTest = new Token(TokenType.Vector, 0);
                Token primeResult = new Token(TokenType.Bool, 0);
                testL.Add(primeTest);
                while (primeCount < primeIndex)
                {
                    primeAt += 2;
                    primeTest[0] = primeAt;
                    if (Numerical.IsPrime("isprime", testL)[0] == 1)
                    {
                        primeCount++;
                    }
                }
            }
            return new Token(TokenType.Vector, primeAt);
        }

        // CreatePrimeArray will return a Token of TokenType.Vector containing first 'n' primes
        // implemented by c0dejunkie, refined by Kashif Imran
        public static Token CreatePrimesArray3(string operation, List<Token> arguments)
        {
            if (arguments.Count != 1 || arguments[0].TokenType != TokenType.Vector || arguments[0].Count != 1 ||
                arguments[0].FirstValue < 1 || arguments[0].FirstValue != (int)arguments[0].FirstValue)
            {
                return Token.Error("Invalid argument to function. Please provide a positive integer");
            }

            int numPrimes = (int)arguments[0].FirstValue;
            double[] primes = new double[numPrimes];

            int count = 0;
            long i = 2;
            List<Token> args = new List<Token>();
            Token number = new Token(TokenType.Vector, i);
            args.Add(number);
            while (count < numPrimes)
            {
                if (Numerical.IsPrime("", args).FirstValue != 0)
                {
                    primes[count] = i;
                    count++;
                }
                number[0] = ++i;
            }

            return new Token(TokenType.Vector, primes);
        }

        public static Token CreatePrimesArray2(string operation, List<Token> arguments)
        {
            if (arguments.Count != 1 || arguments[0].TokenType != TokenType.Vector || arguments[0].Count != 1 ||
                arguments[0].FirstValue < 1 || arguments[0].FirstValue != (int)arguments[0].FirstValue)
            {
                return Token.Error("Invalid argument to function. Please provide a positive integer");
            }

            int numPrimes = (int)arguments[0].FirstValue;
            double[] primes = new double[numPrimes];

            Token primeIndxN = new Token(TokenType.Vector, 0);
            List<Token> primeL = new List<Token>();
            primeL.Add(primeIndxN);

            for (int i = 0; i < numPrimes; i++)
            {
                primeIndxN[0] = i + 1;
                primes[i] = Numerical.FindPrimeAt("", primeL)[0];
            }

            return new Token(TokenType.Vector, primes);
        }

        

        /* IsCongruent will return a Token of TokenType.Bool to tell whehter 
         * the first given number is congurment to second number modulu third number*/
        public static Token IsCongruent(string operation, List<Token> arguments)
        {
            if (arguments.Count != 3)
            {
                return Token.Error("Function expects three integers");
            }
            for (int i = 0; i < arguments.Count; i++)
            {
                if (arguments[i].TokenType != TokenType.Vector || arguments[i].Count != 1 ||
                    arguments[i].FirstValue != (int)arguments[i].FirstValue)
                {
                    return Token.Error("Function expects three integers");
                }
            }

            int firstNumber = (int)arguments[0].FirstValue;
            int secondNumber = (int)arguments[1].FirstValue;
            int thirdNumber = (int)arguments[2].FirstValue;

            //this needs to be calculated
            //bool isCongruent = false;

            //delete this line after providing proper implemenation 
            return new Token(TokenType.Text, "", "Not yet implemented. Would you like to work on it?");

            //un-comment the following line after providing proper implemenation 

            //return new Token(TokenType.Bool, 1, isCongruent? 1:0);
        }
    }
}
