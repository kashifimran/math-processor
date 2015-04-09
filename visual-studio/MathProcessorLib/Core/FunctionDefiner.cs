using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace MathProcessorLib
{
    static class FunctionDefiner
    {
        static Variables variables = Variables.GetVariables();
        static Dictionary<string, UserFunction> userFunctions = new Dictionary<string, UserFunction>();
        static int nextIdentifier = 0;
        static ulong functionCallNumber = 0;
        static int callDepth = -1;

        public static void SaveXML(XElement root)
        {
            XElement element = new XElement("functions");
            foreach (UserFunction uf in userFunctions.Values)
            {
                uf.SaveXML(element);
            }
            root.Add(element);
        }

        public static void LoadXML(XElement xe)
        {
            userFunctions.Clear();
            nextIdentifier = 0;
            XElement element = xe.Element("functions");
            foreach (var v in element.Elements())
            {
                var uf = new UserFunction();
                uf.LoadXML(v);
                userFunctions.Add(nextIdentifier.ToString(), uf);
                nextIdentifier++;
            }
        }


        public static void RemoveFromList(string uFuncName)
        {
            if (userFunctions.ContainsKey(variables.GetStringData(uFuncName)))
                userFunctions.Remove(variables.GetStringData(uFuncName));
        }

        public static Token CreateUserFunction(List<Token> arguments)
        {
            if (arguments.Count < 1)
                return Token.Error("Arguments not valid");
            if (arguments.Last().TokenType != TokenType.Block)
                return Token.Error("Last argument should be of type 'Block' (i.e. inside {} ");
            Token block = arguments.Last();
            List<string> signatureStrings = new List<string>();
            int i = 0;
            bool returns = false;
            if (arguments.Count > 1 && arguments[0].TokenName == "return")
            {
                i = 1;
                returns = true;
            }
            for (; i < arguments.Count - 1; i++)
            {
                signatureStrings.Add(arguments[i].TokenName);
            }
            userFunctions.Add(nextIdentifier.ToString(), new UserFunction(signatureStrings, arguments.Last().StrData, returns));
            Token temp = new Token(TokenType.UserFunction, nextIdentifier.ToString());
            nextIdentifier++;
            return temp;
        }

        public static Token ExecuteUserFunction(string funcName, List<Token> arguments)
        {
            functionCallNumber++;
            Token result = Token.Error("Invalid call to user function");
            if (!userFunctions.ContainsKey(variables.GetStringData(funcName)))
                return result;
            UserFunction userFunc = userFunctions[variables.GetStringData(funcName)];
            Dictionary<string, string> names = new Dictionary<string, string>();
            for (int i = 0; i < arguments.Count; i++)
            {
                //var tempName = "_" + Guid.NewGuid().ToString("N");
                var tempName = GenerateRandomName(); //This is much faster than Guid due to smaller string length
                names.Add(tempName, arguments[i].TokenName);
                arguments[i].TokenName = tempName;
                variables.AddToken(arguments[i]);
            }
            string executbleCode = ReplaceNames(arguments, userFunc);
            Token temp = new Token(TokenType.Block, "", executbleCode);
            callDepth++;
            if (callDepth == 0)
            {
                variables.RemoveReservedWord("return");
            }
            try
            {
                temp = BlockCommands.ExecuteBlock(temp);
            }
            catch (Exception)
            {
                temp = Token.Error("An error occured while performaing the operation");
            }
            callDepth--;
            if (callDepth < 0)
            {
                variables.AddReservedWord("return");
            }
            foreach (var pair in names)
            {
                var item = variables.GetToken(pair.Key);
                item.TokenName = pair.Value;
                variables.Remove(pair.Key);
            }
            if (userFunc.returns && temp.TokenType != TokenType.Error)
            {
                try
                {
                    temp = variables.GetToken("return");
                }
                catch (Exception)
                {
                    temp = Token.Error("Return value not valid in function definition");
                }
            }
            return temp;
        }

        static StringBuilder executbleCode = new StringBuilder();
        private static string ReplaceNames(List<Token> arguments, UserFunction userFunc)
        {
            executbleCode.Clear();
            string str = userFunc.data;
            for (int j = 0; j < str.Length; )
            {
                int k = BlockCommands.SkipString(j, str);
                if (k > j)
                {
                    executbleCode.Append(str.Substring(j, k - j));
                    j = k;
                    continue;
                }
                while (j < str.Length && Char.IsWhiteSpace(str[j]))
                {
                    j++;
                }
                if (j < str.Length)
                {
                    k = j;
                    string currentStr = null;
                    var isOp = Tokenizer.IsOperator(str[j].ToString());
                    if (isOp != OpEnum.No)
                    {
                        if (isOp == OpEnum.YesKeepGo && j < str.Length - 1)
                        {
                            isOp = Tokenizer.IsOperator(str.Substring(j, 2));
                            if (isOp == OpEnum.Yes)
                            {
                                j++;
                            }
                        }
                        j++;
                        currentStr = str.Substring(k, j - k);
                    }
                    else
                    {
                        //executbleCode.Append(" ");
                        while (j < str.Length && !Char.IsWhiteSpace(str[j]) &&
                               str[j] != ';' && str[j] != ':' && str[j] != '{' && str[j] != '}' && Tokenizer.IsOperator(str[j].ToString()) == OpEnum.No)
                        {
                            j++;
                        }
                        if (k == j)
                        {
                            j++;
                        }
                        currentStr = str.Substring(k, j - k);
                        for (int i = 0; i < arguments.Count; i++)
                        {
                            string oldStr = userFunc.signatureList[i];
                            if (currentStr == oldStr)
                            {
                                currentStr = arguments[i].TokenName;
                                break;
                            }
                        }           
                    }
                    executbleCode.Append(currentStr);
                }
            }
            return executbleCode.ToString();
        }

        static StringBuilder sb = new StringBuilder();
        static int length = 4;
        static int clashCount = 0;
        //static HashSet<string> nameSet = new HashSet<string>();
        static Random rand = new Random();
        static string domain = "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
                               "abcdefghijklmnopqrstuvwxyz";// + "0123456789";

        static string domainN = "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "0123456789";

        static string GenerateRandomName()
        {
            sb.Clear();
            sb.Append(domain[rand.Next(domain.Length)]);
            for (var i = 0; i < length - 1; i++)
            {
                sb.Append(domainN[rand.Next(domain.Length)]);
            }
            string str = sb.ToString();
            if (variables.Contains(str))
            {
                clashCount++;
                if (clashCount > 10)
                {
                    length++;
                    clashCount = 0;
                }
                return GenerateRandomName();
            }
            else
            {
                return str;
            }
            //string str = sb.ToString();
            //if (nameSet.Contains(str))
            //{
            //    return GenerateRandomName();
            //}
            //else
            //{
            //    nameSet.Add(str);
            //    return str;
            //}
        }

        internal class UserFunction
        {
            internal List<string> signatureList;
            internal string data;
            internal bool returns;

            internal UserFunction() { }

            internal UserFunction(List<string> argumentNames, string data, bool returns)
            {
                this.signatureList = argumentNames;
                this.returns = returns;
                this.data = data.Trim();
            }
            
            public void SaveXML(XElement root)
            {
                XElement element = new XElement("userfunction");
                element.Add(new XElement("data", data));
                element.Add(new XElement("returns", returns));
                XElement signatures = new XElement("signatures");
                foreach (string s in signatureList)
                {
                    signatures.Add(new XElement("s", s));
                }
                element.Add(signatures);
                root.Add(element);
            }

            public void LoadXML(XElement element)
            {
                data = element.Element("data").Value;
                returns = bool.Parse(element.Element("returns").Value);
                signatureList = new List<string>();
                XElement signatures = element.Element("signatures");
                foreach (var v in signatures.Elements())
                {
                    signatureList.Add(v.Value);
                }
            }             
        }
    }
}
