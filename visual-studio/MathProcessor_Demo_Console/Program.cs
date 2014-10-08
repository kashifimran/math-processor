using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathProcessorLib;

namespace MathProcessor_Demo_Console
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Function.AddDirective("paste", Paste);
            Function.AddReplaceFunction("plot", CreatePlot);
            Calculator.IntermediateResultProduced += new IntermediateResult(IntermediatResultProduced);
            string input = "";
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Enter MP expressions to process. Type 'paste' to use text from the Clipboard.\r\nType 'exit' to quit.");
            Console.ForegroundColor = ConsoleColor.Gray;
            while (true)
            {
                Console.Write(">> ");
                input = Console.ReadLine();
                if (input.Trim().ToLower() == "exit")
                {
                    break;
                }
                Token result = Calculator.ProcessCommand(input);
                DisplayResult(result);
            }
        }

        static void IntermediatResultProduced(Token result)
        {
            DisplayResult(result);
        }

        static private void DisplayResult(Token result)
        {
            if (result.TokenType == TokenType.Error)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(">> " + result.GetString());
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            else if (result.TokenType == TokenType.Matrix)
            {
                int rows = (int)result.Extra;
                if (rows > 0)
                {
                    List<double> data = result.VectorArray.ToList();
                    Token temp;
                    temp = new Token(TokenType.Vector, data.GetRange(0, result.Count / rows).ToArray());
                    Console.WriteLine(">> " + temp.GetString());
                    for (int i = result.Count / rows; i < result.Count; i += result.Count / rows)
                    {
                        temp = new Token(TokenType.Vector, data.GetRange(i, result.Count / rows).ToArray());
                        Console.WriteLine("   " + temp.GetString());
                    }
                }
            }
            else if (result.TokenType != TokenType.Void)
            {
                Console.WriteLine(">> " + result.GetString());
            }
        }

        public static Token Paste(string operation, List<Token> arguments)
        {
            try
            {
                string text = System.Windows.Forms.Clipboard.GetText();
                return Calculator.ProcessCommand(text);
            }
            catch (Exception)
            {
                return new Token(TokenType.Error, "Could not paste text from Clipboard.");
            }
        }

        public static Token CreatePlot(string operation, List<Token> arguments)
        {
            if (arguments.Count < 2)
                return Token.Error("Function requires more than one parameter");

            GraphForm gf = new GraphForm();
            for (int i = 0; i < arguments.Count; )
            {
                if (arguments.Count - i > 2 && arguments[i + 2].TokenType == TokenType.Text)
                {
                    if (!gf.AddCurve(arguments[i].VectorArray, arguments[i + 1].VectorArray, arguments[i + 2].TokenName))
                        return Token.Error("Parameters not valid");
                    i += 3;
                }
                else if (arguments.Count - i >= 2)
                {
                    if (!gf.AddCurve(arguments[i].VectorArray, arguments[i + 1].VectorArray))
                        return Token.Error("Data not valid");
                    i += 2;
                }
                else
                    return Token.Error("Parameters not valid");
            }
            gf.ShowDialog();
            return Token.Void;
        }
    }
}
