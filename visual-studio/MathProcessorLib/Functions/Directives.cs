using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Forms;

namespace MathProcessorLib
{
    public static class Directive
    {
        static Variables variables = Variables.GetVariables();

        public static void CreateFunctions()
        {            
            Function.AddDirective("postfix", Postfix);
            Function.AddDirective("remove", Remove);
            Function.AddDirective("removeall", RemoveAll);
            Function.AddDirective("reset", Reset);
            Function.AddDirective("resetall", ResetAll);
            Function.AddDirective("load", LoadFile);
        }

        public static Token LoadFile(string operation, List<Token> arguments)
        {
            string filePath = null;
            if (arguments.Count == 0)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Title = "Open comamnd file to execute";
                ofd.Filter = "Text File (.txt;*.*)|*.txt;*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    filePath = ofd.FileName;
                }
                else
                {
                    return Token.Void;
                }
            }
            else if (arguments.Count == 1 && arguments[0].TokenType == TokenType.Text)
            {
                filePath = arguments[0].StrData;
            }
            else
            {
                return Token.Error("Function 'load' should either be called without parameters or with one parameter containing path of the file to be loaded.");
            }
            if (!File.Exists(filePath))
            {
                Token.Error("The specified file does not exist or is not accessible");
            }
            using (FileStream textFile = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader reader = new StreamReader(textFile))
                {
                    String data = reader.ReadToEnd();
                    return Calculator.ProcessCommand(data, true);
                }
            }
        }

        static Token RemoveAll(string operation, List<Token> tokenList)
        {
            variables.RemoveAll();
            return Token.Void;
        }

        static Token Postfix(string operation, List<Token> tokenList)
        {
            List<Token> postfixTokens = new List<Token>();
            string message;
            if (Calculator.InfixToPostfix(tokenList, postfixTokens).TokenType != TokenType.Error)
            {
                message = "Postfix:   ";
                foreach (Token t in postfixTokens)
                {
                    switch (t.TokenType)
                    {
                        case TokenType.Bool:
                        case TokenType.Vector:
                            if (t.Count == 1)
                                message += t.FirstValue + "  ";
                            else
                                message += t.TokenType + "  ";
                            break;
                        case TokenType.Break:
                        case TokenType.Directive:
                        case TokenType.Function:
                        case TokenType.UserFunction:
                        case TokenType.Operator:
                        case TokenType.LoopOrCondition:
                            message += t.TokenName + "  ";
                            break;
                        default:
                            message += t.TokenType + "  ";
                            break;
                    }
                }
            }
            else
            {
                return Token.Error("Could not Convert expression to postfix");

            }
            return new Token(TokenType.Text, "", message);
        }

        static Token Remove(string operation, List<Token> tokenList)
        {
            for (int i = 0; i < tokenList.Count; i++)
            {
                variables.Remove(tokenList[i].TokenName);
            }
            return Token.Void;
        }

        static Token Reset(string operation, List<Token> tokenList)
        {
            foreach (Token t in tokenList)
                variables.Reset(t);
            return Token.Void;
        }

        static Token ResetAll(string operation, List<Token> tokenList)
        {
            variables.Reset();
            return Token.Void;
        }
    }
}
