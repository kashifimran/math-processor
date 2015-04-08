using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MathProcessorLib
{
    public class Variables
    {
        Dictionary<string, NamedToken> namedTokens = new Dictionary<string, NamedToken>();
        List<string> reservedWords = new List<string>();
        static Variables vars = new Variables();

        void FillReservedList()
        {
            vars.AddReservedWord("ans");
            vars.AddReservedWord("return");
        }

        public bool IsReservedWord(string word)
        {
            return reservedWords.Contains(word);
        }

        public Token IsValidVarName(string name)
        {
            if (name.Length > 0)
            {
                if (Char.IsLetter(name[0]) || name[0] == '_')
                {
                    if (!(Function.IsFuction(name) || IsReservedWord(name)))
                    {
                        if (namedTokens.ContainsKey(name))
                        {
                            if (!namedTokens[name].constant)
                            {
                                TokenType type = GetTokenType(name);
                                if (type == TokenType.Text || type == TokenType.Matrix ||
                                    type == TokenType.Vector || type == TokenType.Bool)
                                {
                                    return Token.Void;
                                }
                            }
                        }
                        else
                            return Token.Void;
                    }
                }
            }
            return Token.Error("Invalid variable name");
        }

        public void SaveXML(XElement root)
        {
            XElement element = new XElement("vars");
            foreach (KeyValuePair<string, NamedToken> kvp in namedTokens)
            {
                if (!kvp.Value.constant)
                {
                    kvp.Value.token.SaveXML(element);
                }
            }
            root.Add(element);
        }

        public void LoadXML(XElement xe)
        {
            XElement element = xe.Element("vars");
            var elements = element.Elements();
            foreach (var v in elements)
            {
                Token t = new Token();
                t.LoadXML(v);
                if (namedTokens.Keys.Contains(t.TokenName))
                {
                    namedTokens[t.TokenName] = new NamedToken(t);
                }
                else
                {
                    namedTokens.Add(t.TokenName, new NamedToken(t));
                }
            }
        }

        public void RemoveReservedWord(string word)
        {
            if (reservedWords.Contains(word))
            {
                reservedWords.Remove(word);
            }
        }

        public void AddReservedWord(string word)
        {
            if (!reservedWords.Contains(word))
            {
                reservedWords.Add(word);
            }
        }

        private Variables() { } //singleton creates itself only once...
        static Variables()
        {
            vars.SetConstants();
            vars.FillReservedList();
            vars.AddToken(new Token(TokenType.Vector, "ans", new double[] { double.NaN }), false);
        }

        public List<string> GetConstantNames()
        {
            List<string> constantNames = new List<string>();
            foreach (string s in namedTokens.Keys)
            {
                if (namedTokens[s].constant)
                    constantNames.Add(s);
            }
            return constantNames;
        }

        public List<string> GetVariableNames()
        {
            List<string> variableNames = new List<string>();
            foreach (string s in namedTokens.Keys)
            {
                if (!namedTokens[s].constant)
                    variableNames.Add(s);
            }
            return variableNames;
        }

        void SetConstants()
        {
            AddToken(new Token(TokenType.Vector, "PI", new double[] { Math.PI }), true);
            AddToken(new Token(TokenType.Vector, "pi", new double[] { Math.PI }), true);
            AddToken(new Token(TokenType.Vector, "NaN", new double[] { double.NaN }), true);
            AddToken(new Token(TokenType.Bool, "true", new double[] { 1 }), true);
            AddToken(new Token(TokenType.Bool, "false", new double[] { 0 }), true);
        }

        public void Reset()
        {
            foreach (NamedToken namedToken in namedTokens.Values)
            {
                namedToken.Reset();
            }
        }

        public void Reset(Token t)
        {
            if (namedTokens.Keys.Contains(t.TokenName))
                namedTokens[t.TokenName].Reset();
        }

        public static Variables GetVariables()
        {
            return vars;
        }

        void AddToken(Token t, bool constant)
        {
            namedTokens.Add(t.TokenName, new NamedToken(t, constant));
        }

        public void AddToken(Token t)
        {
            AddToken(t, false);
        }

        public Token AddTokenVerify(Token t)
        {
            if (IsReservedWord(t.TokenName))
            {
                return Token.Error("[ " + t.TokenName + " ] is a keyword. It cannot be assigned a value.");
            }
            AddToken(t, false);
            return Token.Void;
        }

        public void AddArrayToToken(string name, double[] numArray)
        {
            namedTokens[name].token.Append(numArray);
        }

        public void Remove(String str)
        {
            if (namedTokens.Keys.Contains(str) && !(namedTokens[str].constant || IsReservedWord(str)))
            {
                FunctionDefiner.RemoveFromList(str);
                var t = namedTokens[str];
                namedTokens.Remove(str);
                t.token.TokenName = "";
            }
        }

        public void RemoveAll()
        {
            foreach (string str in namedTokens.Keys)
            {
                if (!(IsConstant(str) || IsReservedWord(str)))
                    FunctionDefiner.RemoveFromList(str);
            }
            foreach (var t in namedTokens)
            {
                t.Value.token.TokenName = "";
            }
            namedTokens.Clear();
            SetConstants();
            AddToken(new Token(TokenType.Vector, "ans", new double[] { double.NaN }), false);
        }

        public bool Contains(string key)
        {
            return namedTokens.ContainsKey(key);
        }

        public bool IsConstant(string key)
        {
            return namedTokens[key].constant;
        }

        public double[] GetVector(string varName)
        {
            return namedTokens[varName].token.VectorArray;
        }

        public void SetVector(string tokenName, double[] vector)
        {
            if (!namedTokens[tokenName].constant && !IsReservedWord(tokenName))
                namedTokens[tokenName].token.VectorArray = vector;
            else
                throw new InvalidOperationException(tokenName + " is a constant or reserved word.");
        }

        public TokenType GetTokenType(string str)
        {
            return namedTokens[str].token.TokenType;
        }

        public string GetStringData(string name)
        {
            return namedTokens[name].token.StrData;
        }

        public double GetExtra(string name)
        {
            return namedTokens[name].token.Extra;
        }

        public void ChangeTokenType(string name, TokenType tt)
        {
            if (namedTokens[name].constant || reservedWords.Contains(name))
            {
                throw new ArgumentException(name + " is a constant or reserved word.");
            }
            namedTokens[name].token.TokenType = tt;
        }

        public Token GetClone(string name)
        {
            return namedTokens[name].token.Clone();
        }

        public Token SetTokenVerify(Token t)
        {
            if (namedTokens[t.TokenName].constant)
            {
                return Token.Error(t.TokenName + " is a constant");
            }
            if (IsReservedWord(t.TokenName))
            {
                return Token.Error("[ " + t.TokenName + " ] is a keyword. It cannot be assigned a value.");
            }
            namedTokens[t.TokenName].token = t;
            return Token.Void;
        }

        //Call this if you are sure the assignment is valid
        public void SetToken(Token t)
        {
            namedTokens[t.TokenName].token = t;
        }

        public Token GetToken(string name)
        {
            var t = namedTokens[name].token;
            if (t.TokenName != name)
            {
                t.TokenName = name;
            }
            return t;
        }

        class NamedToken
        {
            public bool constant = false;
            public Token token;

            public double this[int index]
            {
                get { return token[index]; }
                set { token[index] = value; }
            }

            public NamedToken(Token token)
                : this(token, false)
            {
            }

            public NamedToken(Token token, bool constant)
            {
                if (token.TokenName.Length <= 0)
                    throw new ArgumentException("Named constant must be given a name!");
                this.token = token;
                this.constant = constant;
            }

            public void Reset()
            {
                if (!constant)
                    token.VectorArray = new double[] { double.NaN };
            }
        }
    }
}
