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
                if (arguments[i].TokenName.Length < 3 ||
                    (!arguments[i].TokenName.StartsWith("v_") && !arguments[i].TokenName.StartsWith("b_") &&
                     !arguments[i].TokenName.StartsWith("s_") && !arguments[i].TokenName.StartsWith("m_")) &&
                     !arguments[i].TokenName.StartsWith("o_")
                   )
                {
                    return Token.Error("Invalid parameter name. Param No. " + (i + 1) + ": " + arguments[i].TokenName);
                }
                foreach (string s in signatureStrings)
                {
                    if (arguments[i].TokenName.StartsWith(s))
                    {
                        return Token.Error(arguments[i].TokenName + " starts with " + s + ", which is the FULL name of another parameter. This is not allowed.");
                    }
                }
                signatureStrings.Add(arguments[i].TokenName);
            }
            userFunctions.Add(nextIdentifier.ToString(), new UserFunction(signatureStrings, arguments.Last().StrData, returns));
            Token temp = new Token(TokenType.UserFunction, nextIdentifier.ToString());
            nextIdentifier++;
            return temp;
        }

        public static Token ExecuteUserFunction (string funcName, List<Token> arguments)
        {
            functionCallNumber++;            
            List<string> namelessParams = new List<string>();
            Token result = Token.Error("Invalid call to user function");
            if (!userFunctions.ContainsKey(variables.GetStringData(funcName)))
                return result;
            UserFunction userFunc = userFunctions[variables.GetStringData(funcName)];
            if ((result = VerifySignature(userFunc, arguments)).TokenType == TokenType.Error)
            {
                return result;
            }

            string executbleCode = ReplaceNames(arguments, namelessParams, userFunc);           
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
            foreach (string s in namelessParams)
            {
                try
                {                    
                    variables.Remove(s);                    
                }
                catch (Exception) { }                
            }
            if (userFunc.returns && temp.TokenType != TokenType.Error)
            {
                try 
                {
                    temp = variables.GetToken("return");                    
                }
                    catch (Exception)
                {
                    temp= Token.Error("Return value not valid in function definition");
                }
            }
            return temp;
        }

        private static string ReplaceNames(List<Token> arguments, List<string> namelessParams, UserFunction userFunc)
        {
            StringBuilder executbleCode = new StringBuilder(userFunc.data);
            for (int i = 0; i < arguments.Count; i++)
            {
                if (arguments[i].TokenType == TokenType.Text)
                {
                    //continue;
                }
                if (arguments[i].TokenName.Length == 0 || arguments[i].TokenType == TokenType.Text)
                {
                    arguments[i].TokenName = "_" + Guid.NewGuid().ToString("N");
                    variables.AddToken(arguments[i]);
                    namelessParams.Add(arguments[i].TokenName);
                }
                bool isString = false;
                for (int j = 0; j < executbleCode.Length; j++)
                {
                    if (executbleCode[j] == '"')
                    {
                        if (j > 0)
                        {
                            if (executbleCode[j - 1] != '\"')
                            {
                                isString = !isString;
                            }
                        }
                        else
                        {
                            isString = !isString;
                        }
                    }
                    if (!isString)
                    {
                        int end = j + 1;
                        int length = userFunc.signatureList[i].Length;
                        int start = end - length;
                        string oldStr = userFunc.signatureList[i];
                        string newStr = arguments[i].TokenName;
                        if (end > length)
                        {
                            string currentStr = executbleCode.ToString(start, length);
                            if (currentStr == oldStr)
                            {
                                executbleCode.Replace(oldStr, newStr, start, length);
                            }
                        }
                    }
                }
            }
            return executbleCode.ToString();
        }

        static Token VerifySignature(UserFunction uFunc, List<Token>arguments)
        {
            if (uFunc.signatureList.Count != arguments.Count)
            {
                return Token.Error("Wrong number of arguments passed to function. Function expects " + uFunc.signatureList.Count + " argument(s).");                
            }
            for (int i = 0; i < arguments.Count; i++)
            {
                switch (uFunc.signatureList[i].Substring(0, 2))
                {
                    case "v_":
                        if (arguments[i].TokenType != TokenType.Vector)
                        {
                            return Token.Error("Argument No. " + (i + 1) + " is not of type vector as required in function signature");
                        }
                        break;
                    case "m_":
                        if (arguments[i].TokenType != TokenType.Matrix)
                        {
                            return Token.Error("Argument No. " + (i + 1) + " is not of type matrix as required in function signature");
                        }
                        break;
                    case "b_":
                        if (arguments[i].TokenType != TokenType.Bool)
                        {
                            return Token.Error("Argument No. " + (i + 1) + " is not of type Bool as required in function signature");
                        }
                        break;
                    case "s_":
                        if (arguments[i].TokenType != TokenType.Text)
                        {
                            return Token.Error("Argument No. " + (i + 1) + " is not of type String as required in function signature");
                        }
                        break;
                    //case "p_":
                    //    if (!arguments[i].IsOfType(typeof(PlotToken)))
                    //    {
                    //        return Token.Error("Argument No. " + (i + 1) + " is not of type plot as required in function signature");
                    //    }
                    //    break;
                    case "o_":
                        if (arguments[i].TokenType != TokenType.Custom)
                        {
                            return Token.Error("Argument No. " + (i + 1) + " is not of type 'object' as required in function signature");
                        }
                        break;
                    default:
                        return Token.Error("Bad function definition");
                }
            }
            return Token.Void;
        }
        
        internal class UserFunction
        {
            internal List<string> signatureList;
            internal string data;
            internal bool returns;

            internal UserFunction() { }

            internal UserFunction(List<string> signature, string data, bool returns)
            {
                this.signatureList = signature;
                this.data = data;
                this.returns = returns;
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
