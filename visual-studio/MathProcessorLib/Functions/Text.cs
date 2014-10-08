using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace MathProcessorLib
{
    class StringsToken : IExtendedToken
    {
        string name = "String Array";
        public string GetName()
        {
            return name;
        }

        public Token CopyToken(Token token)
        {
            var list = new List<string>();
            list.AddRange((List<string>)token.CustomData);
            var t = Token.CustomToken(this.GetType(), list);
            return t;
        }
    }
        
    
    static class Text
    {   
        public static void CreateFunctions()
        {
            Token.RegisterCustomType(typeof(StringsToken));

            Function.AddFunction("strings", StringsArray);
            Function.AddFunction("addstrings", AddStrings);
            Function.AddFunction("insertstrings", InsertStrings);
            Function.AddFunction("stringat", StringAt);
            Function.AddFunction("removestrings", RemoveStrings);
            Function.AddFunction("removestringsat", RemoveStringsAt);
            Function.AddFunction("combinestrings", CombineStrings);
            Function.AddFunction("countstrings", CountStrings);
        }

        public static Token CountStrings(string operation, List<Token> arguments)
        {
            if (arguments.Count == 1 && arguments[0].IsOfType(typeof(StringsToken)))
            {
                return new Token(TokenType.Vector, ((List<string>)arguments[0].CustomData).Count);
            }
            else
            {
                return Token.Error("Error: countstrings() function expects exactly one parameter of type String Array. Use strings() to create one.");
            }
        }

        public static Token StringsArray(string operation, List<Token> arguments)
        {
            List<string> list = new List<string>();
            Token t = Token.CustomToken(typeof(StringsToken), list);
            foreach (var a in arguments)
            {
                list.Add(a.GetString(int.MaxValue));
            }
            t.CustomData = list;
            return t;
        }

        public static Token CombineStrings(string operation, List<Token> arguments)
        {
            if (arguments.Count < 1 || !arguments[0].IsOfType(typeof(StringsToken)))
            {
                return Token.Error("Error: combinestrings() function expects at least one parameter of type 'string array'.");
            }
            if (arguments.Count > 2)
            {
                return Token.Error("Error: combinestrings() expects at most two parameters.");
            }
            if (arguments.Count ==2 && arguments[1].TokenType != TokenType.Text)
            {
                return Token.Error("Error: combinestrings(). Second parameter must be a string to be used a separator.");
            }
            string separator = ",";
            if (arguments.Count == 2)
            {
                separator = arguments[1].StrData;
            }            
            List<string> list = (List<string>)arguments[0].CustomData;
            string str = "";
            for (var i = 0; i < list.Count; i++)
            {
                str += list[i] + (i < list.Count - 1 ? separator : "");
            }
            return new Token(TokenType.Text, "", str);
        }

        public static Token StringAt(string operation, List<Token> arguments)
        {
            if (arguments.Count != 2)
            {
                return Token.Error("Error: stringat() function expects two parameters.");
            }
            if (!arguments[0].IsOfType(typeof(StringsToken)))
            {
                return Token.Error("Error: First parameter to stringat() function must be a string array (use strings() to create one).");
            }
            if (arguments[1].TokenType != TokenType.Vector || arguments[1].Count != 1)
            {
                return Token.Error("Error: Second parameter to stringat() function must be a single numeric value.");
            }
            List<string> list = (List<string>)arguments[0].CustomData;
            int index = (int)arguments[1].FirstValue;
            try
            {
                return new Token(TokenType.Text, "", list[index]);
            }
            catch
            {
                return Token.Error("Error: stringat(). Array index out of bounds. Please make sure you provided correct value for index.");
            }
        }

        public static Token AddStrings(string operation, List<Token> arguments)
        {
            if (arguments.Count < 2)
            {
                return Token.Error("Error: addstrings() function expects at least two parameters.");
            }
            if (!arguments[0].IsOfType(typeof(StringsToken)))
            {
                return Token.Error("Error: First parameter to addstring() function must be a string array (use strings() to create one).");
            }
            List<string> list = (List<string>)arguments[0].CustomData;
            for (var i = 1; i < arguments.Count; i++)
            {
                list.Add(arguments[i].GetString(int.MaxValue));
            }
            return Token.Void;
        }

        public static Token InsertStrings(string operation, List<Token> arguments)
        {
            if (arguments.Count < 3)
            {
                return Token.Error("Error: addstring() function expects at least three parameters.");
            }
            if (!arguments[0].IsOfType(typeof(StringsToken)))
            {
                return Token.Error("Error: First parameter to insertstrings() function must be a string array (use strings() to create one).");
            }
            if (arguments[1].TokenType != TokenType.Vector && arguments[1].Count != 1)
            {
                return Token.Error("Error: Second parameter to insertstrings() function must be a single numeric value.");
            }
            try
            {
                List<string> list = new List<string>();
                int index = (int)arguments[1].FirstValue;
                for (var i = 2; i < arguments.Count; i++)
                {
                    list.Add(arguments[i].GetString(int.MaxValue));
                }
                ((List<string>)arguments[0].CustomData).InsertRange(index, list);
            }
            catch
            {
                return Token.Error("Error: insertstrings(). Make sure you provided correct value for index.");
            }
            return Token.Void;
        }

        public static Token RemoveStrings(string operation, List<Token> arguments)
        {
            if (arguments.Count < 2)
            {
                return Token.Error("Error: removestrings() function expects two or more parameters.");
            }
            if (!arguments[0].IsOfType(typeof(StringsToken)))
            {
                return Token.Error("Error: removestrings(). The first parameter must be an array of strings. Use strings() to create one.");
            }
            List<string> list = (List<string>)arguments[0].CustomData;
            int removed = 0;
            for (var i = 1; i < arguments.Count; i++)
            {
                var s = arguments[i].GetString(int.MaxValue);
                if (list.Remove(s))
                {
                    removed++;
                }
            }
            return new Token(TokenType.Text, "", removed +  (removed == 1 ? " string" : " strings") + " matched & removed from the string array.");
        }

        public static Token RemoveStringsAt(string operation, List<Token> arguments)
        {
            if (arguments.Count != 2)
            {
                return Token.Error("Error: removestringsat() function expects two parameters.");
            }
            if (!arguments[0].IsOfType(typeof(StringsToken)))
            {
                return Token.Error("Error: removestringsat(). The first parameter must be an array of strings. Use strings() to create one.");
            }
            if (arguments[1].TokenType != TokenType.Vector)
            {
                return Token.Error("Error: removestringsat(). The second parameter must be of type vector containing one or more valid indexes.");
            }
            try
            {
                List<int> indexes = arguments[1].Vector.Select(x => (int)x).ToList();
                indexes.Sort();
                List<string> list = (List<string>)arguments[0].CustomData;
                for (var i = indexes.Count - 1; i >= 0; i--)
                {
                    list.RemoveAt(indexes[i]);
                }
            }
            catch
            {
                return Token.Error("Error: removestringsat(). Make sure you provided correct index values.");
            }
            return new Token(TokenType.Text, "", "Specifed strings removed from the string array.");
        }
    }
}
