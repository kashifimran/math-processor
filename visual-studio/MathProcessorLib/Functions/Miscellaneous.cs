using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing;


namespace MathProcessorLib
{
    public static class Miscellaneous
    {
        static Stack<Token> stack = new Stack<Token>();

        public static void CreateFunctions()
        {
            Function.AddFunction("echo", EchoString);
            Function.AddFunction("format", Format);
            Function.AddFunction("unit", Unit);            
            Function.AddFunction("ticks", GetTicks);
            Function.AddFunction("rand", GetRandom);
            Function.AddFunction("getstring", GetString);
            Function.AddFunction("constants", GetNames);
            Function.AddFunction("vars", GetNames);
            Function.AddFunction("savefile", SaveFile);
            Function.AddFunction("getusernum", GetNumberFromUser);
            Function.AddFunction("getuserstr", GetStringFromUser);
            Function.AddFunction("messagebox", ShowMessageBox);
            Function.AddFunction("guid", GetGuid);
            Function.AddFunction("typeof", GetTokenType);
            Function.AddFunction("push", Push);
            Function.AddFunction("pop", Pop);
        }

        public static Token Push(string operation, List<Token> arguments)
        {
            if (arguments.Count != 1)
            {
                return Token.Error("push(): Exactly one parameter expected.");
            }
            stack.Push(arguments[0]);
            return Token.Void;
        }

        public static Token Pop(string operation, List<Token> arguments)
        {
            if (stack.Count > 0)
            {
                return stack.Pop();
            }
            return Token.Error("pop(): stack is empty.");
        }

        public static Token GetTokenType(string operation, List<Token> arguments)
        {
            if (arguments.Count != 1)
            {
                return Token.Error("Error: Exactly one argument expected.");
            }
            if (arguments[0].TokenType == TokenType.Custom)
            {
                return new Token(TokenType.Text, "", arguments[0].StrData);
            }
            else
            {
                return new Token(TokenType.Text, "", arguments[0].TokenType.ToString());
            }
        }

        public static Token GetGuid(string operation, List<Token> arguments)
        {
            if (arguments.Count > 0)
            {
                return Token.Error("Function expects no arguments");
            }
            return new Token(TokenType.Text, "", Guid.NewGuid().ToString("N"));
        }

        public static Token ShowMessageBox(string operation, List<Token> arguments)
        {
            if (arguments.Count != 2 || arguments[0].TokenType != TokenType.Text || arguments[1].TokenType != TokenType.Text)
            {
                return Token.Error("Two strings must be passed as caption and message");
            }
            MessageBox.Show(arguments[1].GetString(), arguments[0].GetString());
            return Token.Void;
        }

        public static Token GetNumberFromUser(string operation, List<Token> arguments)
        {
            if (arguments.Count == 0)
            {
                return getUserInput("", true);
            }
            else if (arguments.Count == 1)
            {
                return getUserInput(arguments[0].StrData, true);
            }
            else
            {
                return Token.Error("Function expects 0 or 1 arguments");
            }
        }

        public static Token GetStringFromUser(string operation, List<Token> arguments)
        {
            if (arguments.Count == 0)
            {
                return getUserInput("", false);
            }
            else if (arguments.Count == 1)
            {
                return getUserInput(arguments[0].StrData, false);
            }
            else
            {
                return Token.Error("Function expects 0 or 1 arguments");
            }
        }

        static Token getUserInput(string message, bool isNumber)
        {
            Form inputForm = new Form();            
            inputForm.Text = "User Input";
            Label messageLabel = new Label();
            messageLabel.Width = 390;
            messageLabel.Font = new Font("arial", 11);
            messageLabel.Text = message;
            inputForm.Controls.Add(messageLabel);
            messageLabel.Location = new Point(20, 15);
            inputForm.Size = new Size(400, 180);
            inputForm.MaximizeBox = false;
            inputForm.MinimizeBox = false;
            inputForm.MaximumSize = inputForm.Size;
            inputForm.MinimumSize = inputForm.Size;
            inputForm.BackColor = Color.WhiteSmoke;
            Button OKButton = new Button();            
            OKButton.Text = "OK";
            OKButton.Click += (x, y) => { inputForm.DialogResult = DialogResult.OK; };
            OKButton.Size = new Size(80, 25);
            inputForm.Controls.Add(OKButton);
            OKButton.Location = new Point(160, 105);
            TextBox text = new TextBox();
            text.Location = new Point(20, 50);
            text.Size = new Size(355, 30);
            text.Font = new Font("Sans Serif", 12);
            text.TabIndex = 0;
            inputForm.Controls.Add(text);
            //text.KeyUp += (x, y) => { if (y.KeyCode == Keys.Return) { inputForm.DialogResult = DialogResult.OK; } };
            //inputForm.Shown += (x, y) => { text.Focus(); };
            inputForm.StartPosition = FormStartPosition.CenterParent;
            inputForm.AcceptButton = OKButton;
            DialogResult result = inputForm.ShowDialog();
            inputForm.Focus();
            Token temp = new Token(TokenType.Vector, double.NaN);
            if (result == DialogResult.OK)
            {
                if (isNumber)
                {
                    double num = double.NaN;
                    try
                    {
                        num = double.Parse(text.Text);
                    }
                    catch (Exception) { }
                    temp = new Token(TokenType.Vector, num);
                }
                else
                {
                    if (text.Text.Length > 0)
                        temp = new Token(TokenType.Text, "", text.Text);
                }
            }
            inputForm.Dispose();
            return temp;
        }

        public static Token EchoString(string operation, List<Token> arguments)
        {
            StringBuilder message = new StringBuilder();
            foreach (Token t in arguments)
            {
                message.Append(t.GetString());
            }
            Calculator.SendIntermediateResult(new Token(TokenType.Text, "", message.ToString()));
            return Token.Void;
        }

        static Token Format(string operation, List<Token> tokenList)
        {
            if (tokenList.Count == 1)
            {
                double testValue = 0;
                try
                {
                    string formatStr = tokenList[0].StrData.Length != 0 ? tokenList[0].StrData : tokenList[0].TokenName;
                    String retValue = testValue.ToString(formatStr);
                    if (retValue == tokenList[0].StrData || formatStr.Length == 0)
                    {
                        return Token.Error("Bad number format");
                    }
                }
                catch (Exception)
                {
                    return Token.Error("Bad argument passed to format() function");
                }
                string oldFormat = Calculator.DefaultFormatString;
                Calculator.DefaultFormatString = tokenList[0].StrData.ToUpper();
                return new Token(TokenType.Text, "", oldFormat);
            }
            else if (tokenList.Count > 0)
            {
                return Token.Error("Invalid number of arguments passed to format directive");
            }
            return new Token(TokenType.Text, Calculator.DefaultFormatString);
        }

        static Token Unit(string operation, List<Token> tokenList)
        {
            if (tokenList.Count == 1)
            {
                if (tokenList[0].StrData.ToLower() == "radian" || tokenList[0].StrData.ToLower() == "rad")
                    Function.Unit = AngleUnit.Radian;
                else if (tokenList[0].StrData.ToLower() == "degree" || tokenList[0].StrData.ToLower() == "deg")
                    Function.Unit = AngleUnit.Degree;
                else
                {
                    return Token.Error("Only rad/radian OR deg/degree can be set as angle-unit");
                }
            }
            else if (tokenList.Count > 0)
            {
                return Token.Error("Wrong number of arguments to the directive 'unit'");
            }
            return new Token(TokenType.Text, "", Function.Unit.ToString());
        }
        
        public static Token GetNames(string operation, List<Token> arguments)
        {
            if (arguments.Count > 0)
            {
                return Token.Error("Function does not expect any parameters.");
            }
            List<string> names = null;
            if (operation == "constants")
            {
                names = Variables.GetVariables().GetConstantNames();
            }
            else if (operation == "vars")
            {
                names = Variables.GetVariables().GetVariableNames();
            }
            else
            {
                return Token.Error("Bad input");
            }

            StringBuilder strBuilder = new StringBuilder();
            foreach (string s in names)
            {
                strBuilder.Append(s + "  ");
            }
            return new Token(TokenType.Text, "", strBuilder.ToString());
        }

        public static Token GetString(string operation, List<Token> arguments)
        {
            if (arguments.Count != 1 && arguments.Count != 2)
                return Token.Error("Count of arguments must be 1 or 2");

            Token result = Token.Void;
            if (arguments.Count == 1)
                result = new Token(TokenType.Text, arguments[0].GetString());
            if (arguments.Count == 2)
            {
                if (arguments[0].TokenType != TokenType.Vector || arguments[1].TokenType != TokenType.Text)
                    return Token.Error("First argument must be a vector and second must be a string");

                result = new Token(TokenType.Text, arguments[0].GetVectorString(arguments[1].StrData, 10));
            }
            return result;
        }

        public static Token GetRandom(string operation, List<Token> arguments)
        {
            Random rand;
            if (arguments.Count == 0)
            {
                rand = new Random();
            }
            else if (arguments.Count == 1 && arguments[0].Count == 1)
            {
                rand = new Random((int)arguments[0].FirstValue);
            }
            else
            {
                return Token.Error("Parameter(s) invalid.");
            }
            return new Token(TokenType.Vector, rand.NextDouble());
        }


        public static Token GetTicks(string operation, List<Token> arguments)
        {
            if (arguments.Count != 0)
                return Token.Error("No parameters provided.");

            return new Token(TokenType.Vector, Environment.TickCount);
        }
        
        public static Token SaveFile(string operation, List<Token> arguments)
        {
            string file = null;
            if (arguments.Count == 1)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    file = sfd.FileName;
                }
                else
                {
                    return Token.Error("Operation cancelled by the user");
                }
            }
            else if (arguments.Count == 2 && !string.IsNullOrEmpty(arguments[1].StrData))
            {
                file = arguments[1].StrData;
            }
            else
            {
                return Token.Error("Incorrect paramter(s) passed to function savetextfile().");
            }
            try
            {
                using (FileStream textFile = File.Open(file, FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(textFile))
                    {
                        sw.Write(arguments[0].GetString());
                    }
                }
                return new Token(TokenType.Text, "", "Data written to " + arguments[0].StrData);
            }
            catch
            {
                return Token.Error("An error occured while saving " + file + ". Make sure the path is correct.");
            }
        }
    }
}
