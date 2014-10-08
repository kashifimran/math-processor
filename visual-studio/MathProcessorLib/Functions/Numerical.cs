using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathProcessorLib
{
    public static class Numerical
    {        
        public static void CreateFunctions()
        {
            Function.AddFunction("abs",      FindAbs);
            Function.AddFunction("ceil",     FindCeil);            
            Function.AddFunction("truncate", DoTruncate);
            Function.AddFunction("floor",    FindFloor);
            Function.AddFunction("round",    DoRound);
            //Function.AddFunction("isprime0", IsPrimeNew);
            Function.AddFunction("isprime",  IsPrime);
            Function.AddFunction("prime",    FindPrimeAt);
            Function.AddFunction("primes",   CreatePrimesArray);
            Function.AddFunction("primesto", PrimesTo);
            Function.AddFunction("fibs",     CreateFibArray);
            Function.AddFunction("fib",      FindFibAt);
            Function.AddFunction("randlist", RandList);
        }

        public static Token RandList(string operation, List<Token> arguments)
        {
            Random rand = null;
            if (arguments.Count == 1 && arguments[0].Count == 1)
            {
                rand = new Random();
            }
            else if (arguments.Count == 2 && arguments[0].Count == 1 && arguments[1].Count == 1)
            {
                rand = new Random((int)arguments[1].FirstValue);
            }
            else
            {
                return Token.Error("Parameters not valid");
            }
            double[] randNumbers = new double[(int)arguments[0].FirstValue];
            for (int i = 0; i < randNumbers.Count(); i++)
            {
                if (arguments[0].Count != 1)
                    return Token.Error("Parameter(s) not valid");
                randNumbers[i] = rand.NextDouble();
            }
            return new Token(TokenType.Vector, randNumbers);

        }

        /* CreateFibArray will return a Token of TokenType.Vector containing first 'n' fibbonaci numbers*/
        public static Token CreateFibArray(string operation, List<Token> arguments)
        {
            if (arguments.Count != 1 || (arguments[0].TokenType != TokenType.Vector && arguments[0].TokenType != TokenType.Matrix) || arguments[0].Count != 1 ||
                arguments[0].FirstValue < 1 || arguments[0].FirstValue != (int)arguments[0].FirstValue)
            {
                return Token.Error("Invalid argument to function. Please provide a positive integer");
            }

            int numFibs = (int)arguments[0].FirstValue;
            double[] fibs = new double[numFibs];

            // seed Fib(0)
            fibs[0] = 0;

            if (numFibs > 1)
            {
                // seed Fib(1)
                fibs[1] = 1;

                for (int i = 2; i < numFibs; i++)
                {
                    // fibonacci expression
                    fibs[i] = fibs[i - 1] + fibs[i - 2];
                }
            }

            return new Token(TokenType.Vector, fibs);
        }

        /* FindFibAt will return a Token of TokenType.Vector containing just one number i.e. 
         * fibbonaci number at the given index*/
        public static Token FindFibAt(string operation, List<Token> arguments)
        {
            if (arguments.Count != 1 || (arguments[0].TokenType != TokenType.Vector && arguments[0].TokenType != TokenType.Matrix) || arguments[0].Count != 1 ||
                arguments[0].FirstValue < 1 || arguments[0].FirstValue != (int)arguments[0].FirstValue)
            {
                return Token.Error("Invalid argument to function. Please provide a positive integer");
            }

            // fibonacci sequence starts from 0 for n = 1
            // Fib(1) = 0, Fib(2) = 1, Fib(3) = 1 etc.
            int n = (int)arguments[0].FirstValue;
            n--;

            // phi is the well-known 'golden ratio' used to calculate Fib(n)
            double phi = (1 + Math.Sqrt(5)) / 2;

            // closed form expression to calculate fibonacci number given an index
            double fibN = (Math.Pow(phi, n) - Math.Pow(1 - phi, n)) / Math.Sqrt(5);
            fibN = Math.Floor(fibN);    // correct for rounding errors

            return new Token(TokenType.Vector, fibN);
        }

        // CreatePrimeArray will return a Token of TokenType.Vector containing first 'n' primes
        // implemented by c0dejunkie, refined by Kashif Imran
        public static Token CreatePrimesArray(string operation, List<Token> arguments)
        {
            if (arguments.Count != 1 || arguments[0].TokenType != TokenType.Vector || arguments[0].Count != 1 ||
                arguments[0].FirstValue < 1 || arguments[0].FirstValue != (int)arguments[0].FirstValue)
            {
                return Token.Error("Invalid argument to function. Please provide a positive integer");
            }            

            int numPrimes = (int)arguments[0].FirstValue;
            if (numPrimes == 1)
                return new Token(TokenType.Vector, 2);

            double[] primes = new double[numPrimes];
            primes[0] = 2;

            int count = 1;
            long number = 3;
            while (count < numPrimes)
            {
                if (testPrime(number))
                {
                    primes[count] = number;
                    count++;
                }
                number += 2;
            }

            return new Token(TokenType.Vector, primes);
        }
        

        // FindPrimeAt will return a Token of TokenType.Vector which will indicate the prime at a given index
        // Originally implemented by c0dejunkie, refined by KashifImran      
        public static Token FindPrimeAt(string operation, List<Token> arguments)
        {
            if (arguments.Count != 1 || arguments[0].TokenType != TokenType.Vector || arguments[0].Count != 1 ||
                arguments[0].FirstValue < 1 || arguments[0].FirstValue != (int)arguments[0].FirstValue)
            {
                return Token.Error("Invalid argument to function. Please provide a positive integer");
            }

            // an index of 1 will return 2 as the prime number
            int primeIndex = (int)arguments[0].FirstValue;
            int primeCount = 0;
            long primeAt = 1;

            if (primeIndex == 1)
            {
                primeAt = 2;
            }
            else
            {
                primeCount = 1;
                while (primeCount < primeIndex)
                {
                    primeAt += 2;                    
                    if (testPrime(primeAt))
                    {
                        primeCount++;
                    }
                }
            }
            return new Token(TokenType.Vector, primeAt);
        }
        
        //Another IsPrime(), which did not perform well!!!
        /*
        public static Token IsPrimeNew(string operation, List<Token> arguments)
        {
            if (arguments.Count != 1 || arguments[0].TokenType != TokenType.Vector || arguments[0].Count != 1 ||
                arguments[0].FirstValue < 1 || arguments[0].FirstValue != (long)arguments[0].FirstValue)
            {
                return Token.Error("Invalid argument to function. Please provide a positive integer");
            }
            //long number = (long)arguments[0].FirstValue;  
            //return new Token(TokenType.Bool, 1, testPrime(number) ? 1 : 0);
            

            ulong limit = (ulong)arguments[0].FirstValue;
            List<double> primes = new List<double>();
            bool[] isPrime = new bool[limit + 1];
            double sqrt = Math.Sqrt(limit);

            for (ulong x = 1; x <= sqrt; x++)
            {
                for (ulong y = 1; y <= sqrt; y++)
                {
                    var n = 4 * x * x + y * y;
                    if (n <= limit && (n % 12 == 1 || n % 12 == 5))
                        isPrime[n] ^= true;

                    n = 3 * x * x + y * y;
                    if (n <= limit && n % 12 == 7)
                        isPrime[n] ^= true;

                    n = 3 * x * x - y * y;
                    if (x > y && n <= limit && n % 12 == 11)
                        isPrime[n] ^= true;
                }
            }
            for (ulong n = 5; n <= sqrt; n++)
            {
                if (isPrime[n])
                {
                    var s = n * n;
                    for (ulong k = s; k <= limit; k += s)
                    {
                        isPrime[k] = false;
                    }
                }
            }
            primes.Add(2);
            primes.Add(3);
            for (ulong n = 5; n <= limit; n += 2)
            {
                if (isPrime[n])
                {
                    primes.Add(n);
                }
            }
            return new Token(TokenType.Bool, 1, isPrime[limit] ? 1: 0);
        }
        */
        // IsPrime will return a Token of TokenType.Bool which indicates whether the argument is prime
        public static Token IsPrime(string operation, List<Token> arguments)
        {
            if (arguments.Count != 1 || arguments[0].TokenType != TokenType.Vector || arguments[0].Count != 1 ||
                arguments[0].FirstValue < 1 || arguments[0].FirstValue != (long)arguments[0].FirstValue)
            {
                return Token.Error("Invalid argument to function. Please provide a positive integer");
            }
            long number = (long)arguments[0].FirstValue;
            return new Token(TokenType.Bool, 1, testPrime(number) ? 1 : 0);
        }

        //testPrime will test the argument number for primality        
        static bool testPrime(long number)
        {
            bool isPrime = true;
            bool exitFlag = false;
            long i = 1;
            long primTest;

            // avoid calculating square root each time in loop
            double sqrtNum = Math.Sqrt((double)number);

            // are 2 or 3 factors?
            if (!(number % 3 == 0 || number % 2 == 0 || number == 1))
            {
                while (exitFlag == false)
                {
                    primTest = 6 * i;
                    if (primTest + 1 > sqrtNum)
                    {
                        exitFlag = true;
                    }
                    // we have found a prime factor
                    if (number % (primTest + 1) == 0 || number % (primTest - 1) == 0)
                    {
                        // make sure that the prime factor is not the prime number itself
                        if (!(number == (primTest + 1) || number == (primTest - 1)))
                        {
                            isPrime = false;
                        }
                        exitFlag = true;
                    }
                    else
                    {
                        i++;
                    }
                }
            }
            else
            {
                if (number == 2 || number == 3)
                {
                    isPrime = true;
                }
                else
                {
                    isPrime = false;
                }
            }
            return isPrime;
        }

        public static Token PrimesTo (string operation, List<Token> arguments)
        {
            if (arguments.Count != 1 || arguments[0].TokenType != TokenType.Vector || arguments[0].Count != 1 ||
                arguments[0].FirstValue < 1 || arguments[0].FirstValue != (ulong)arguments[0].FirstValue)
            {
                return Token.Error("Invalid argument to function. Please provide a positive integer");
            }

            ulong limit = (ulong)arguments[0].FirstValue;
            List<double> primes = new List<double>();
            bool[] isPrime = new bool[limit + 1];
            double sqrt = Math.Sqrt(limit);

            for (ulong x = 1; x <= sqrt; x++)
            {
                for (ulong y = 1; y <= sqrt; y++)
                {
                    var n = 4 * x * x + y * y;
                    if (n <= limit && (n % 12 == 1 || n % 12 == 5))
                        isPrime[n] ^= true;

                    n = 3 * x * x + y * y;
                    if (n <= limit && n % 12 == 7)
                        isPrime[n] ^= true;

                    n = 3 * x * x - y * y;
                    if (x > y && n <= limit && n % 12 == 11)
                        isPrime[n] ^= true;
                }
            }
            for (ulong n = 5; n <= sqrt; n++)
            {
                if (isPrime[n])
                {
                    var s = n * n;
                    for (ulong k = s; k <= limit; k += s)
                    {
                        isPrime[k] = false;
                    }
                }
            }
            primes.Add(2);
            primes.Add(3);
            for (ulong n = 5; n <= limit; n += 2)
            {
                if (isPrime[n])
                {
                    primes.Add(n);
                }
            }
            return new Token(TokenType.Vector, primes);
        }

        public static Token DoRound(string operation, List<Token> arguments)
        {
            if (arguments.Count < 1)
                return Token.Error ( "No arguments given");            
            
            if (arguments[0].TokenType != TokenType.Vector && arguments[0].TokenType != TokenType.Matrix)
            {   
                return Token.Error("Only arrays and matrices can be passed to the function round");
            }
            
            Token result = arguments[0].Clone();
            if (arguments.Count == 1)
            {                
                for (int i = 0; i < result.Count; i++)
                {
                    result[i] = Math.Round(arguments[0][i]);
                }
            }
            else if (arguments.Count == 2)
            {               
                if (arguments[1].Count == 1)
                {                    
                    for (int i = 0; i < result.Count; i++)
                    {                        
                        result[i] = Math.Round(arguments[0][i], (int)arguments[1].FirstValue);
                    }
                }                
                else 
                {
                    return Token.Error("Second argument not valid");
                }
            }
            else
            {
                return Token.Error ( "Argument(s) no valid");
            }
            return result;
        }

        public static Token DoTruncate(string operation, List<Token> arguments)
        {
            if (arguments.Count < 1)
                return Token.Error("No arguments given");           
            
            if (arguments[0].TokenType != TokenType.Vector && arguments[0].TokenType != TokenType.Matrix)
            {
                return Token.Error("Only arrays and matrices can be passed to the function round");
            }
            Token result = arguments[0].Clone();

            if (arguments.Count == 1)
            {                
                for (int i = 0; i < result.Count; i++)
                {
                    result[i] = Math.Truncate(arguments[0][i]);
                }
            }
            else
            {
                result = new Token(TokenType.Vector, new double[arguments.Count]);
                for (int i = 0; i < arguments.Count; i++)
                {
                    if (arguments[i].Count != 1)
                        return Token.Error ( "Argument(s) no valid");

                    result[i] = Math.Truncate(arguments[i].FirstValue);
                }
            }
            return result;
        }

        public static Token FindFloor(string operation, List<Token> arguments)
        {
            if (arguments.Count < 1)
                return Token.Error ( "No argument(s) given");

            if (arguments[0].TokenType != TokenType.Vector && arguments[0].TokenType != TokenType.Matrix)
            {
                return Token.Error("Only arrays and matrices can be passed to the function round");
            }
            Token result = arguments[0].Clone();

            if (arguments.Count == 1)
            {                
                for (int i = 0; i < result.Count; i++)
                {
                    result[i] = Math.Floor(arguments[0][i]);
                }
            }
            else
            {
                result = new Token(TokenType.Vector, new double[arguments.Count]);
                for (int i = 0; i < arguments.Count; i++)
                {
                    if (arguments[i].Count != 1)
                        return Token.Error ( "Argument(s) no valid");
                    result[i] = Math.Floor(arguments[i].FirstValue);
                }
            }
            return result;
        }

        public static Token FindAbs(string operation, List<Token> arguments)
        {
            if (arguments.Count < 1)
                return Token.Error ( "No argument given");
            
            if (arguments[0].TokenType != TokenType.Vector && arguments[0].TokenType != TokenType.Matrix)
            {
                return Token.Error("Only arrays and matrices can be passed to the function round");
            }
            Token result = arguments[0].Clone();

            if (arguments.Count == 1)
            {               
                for (int i= 0; i <result.Count; i++)
                {
                    result[i] = Math.Abs(arguments[0][i]);
                }                
            }
            else
            {
                result = new Token(TokenType.Vector, new double[arguments.Count]);
                for (int i = 0; i < arguments.Count; i++)
                {
                    if (arguments[i].Count != 1)
                        return Token.Error ( "Argument(s) no valid");
                    result[i] = Math.Abs(arguments[i].FirstValue);
                }
            }
            return result;
        }

        public static Token FindCeil(string operation, List<Token> arguments)
        {
            if (arguments.Count < 1)
                return Token.Error ( "Argument(s) no valid");

            if (arguments[0].TokenType != TokenType.Vector && arguments[0].TokenType != TokenType.Matrix)
            {
                return Token.Error("Only arrays and matrices can be passed to the function round");
            }
            Token result = arguments[0].Clone();

            if (arguments.Count == 1)
            {               
                for (int i = 0; i < result.Count; i++)
                {
                    result[i] = Math.Ceiling(arguments[0][i]);
                }
            }
            else
            {
                result = new Token(TokenType.Vector, new double[arguments.Count]);
                for (int i = 0; i < arguments.Count; i++)
                {
                    if (arguments[i].Count != 1)
                        return Token.Error ( "Argument(s) no valid");
                    result[i] = Math.Ceiling(arguments[i].FirstValue);
                }
            }
            return result;
        }
    }
}
