using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathProcessorLib
{
    public static class Booleans
    {
        static Variables vars = Variables.GetVariables();
        
        
        public static void CreateFunctions()
        {
            Function.AddFunction("bool", CreateBoolArray);
            Function.AddFunction("fillbool", FillBool);
        }

        public static Token FillBool(string operation, List<Token> arguments)
        {
            if (arguments.Count < 1 || arguments.Count > 30)
                return Token.Error ( "Count of arguments must be between 1 and 30");
            
            foreach (Token t in arguments)
            {
                if (t.TokenName.Length < 1 || (vars.Contains(t.TokenName) && vars.IsConstant(t.TokenName)) || vars.IsReservedWord(t.TokenName))
                {
                    return Token.Error ( "[ " + t.TokenName + " ] is not a valid name or is constant/ reserved word.");
                }                
            }
            double[] temp;            
            for (int i = 0; i < arguments.Count; i++)
            {
                temp = new double[(int)Math.Pow(2, arguments.Count)];
                for (int j = 0; j < temp.Count(); j++)
                {                    
                    temp[j] = ((j / (int)Math.Pow(2, arguments.Count-i-1))+1) % 2; 
                }
                
                if (!vars.Contains(arguments[i].TokenName))
                {
                    try
                    {
                        vars.AddToken(new Token(TokenType.Bool, arguments[i].TokenName, temp));
                    }
                    catch (ArgumentException e)
                    {
                        return Token.Error ( e.Message);
                    }
                }
                else 
                {   
                    vars.ChangeTokenType(arguments[i].TokenName, TokenType.Bool);
                    vars.SetVector(arguments[i].TokenName, temp);
                }
            }
            return Token.Void;            
        }

        public static Token CreateBoolArray(string operation, List<Token> arguments)
        {
            if (arguments.Count < 1)
                return Token.Error ( "Function expects non-zeor number of arguments");
            Token result = Token.Void;

            if (arguments.Count > 1)
            {                
                foreach (Token t in arguments)
                {
                    if (t.Count != 1)
                        return Token.Error ( "Function expects non-array arguments");
                }
            }
            else if (arguments[0].Count < 1)
            {
                return Token.Error ( "Argument does not contain data");
            }
            if (arguments.Count == 1)
            {
                double[] temp = arguments[0].VectorArray;
                for (int i = 0; i < temp.Count(); i++)
                {
                    if (temp[i] != 0)
                        temp[i] = 1;
                }
                result = new Token(TokenType.Bool, temp);
            }
            else
            {
                double[] temp = new double[arguments.Count];
                for (int i = 0; i < temp.Count(); i++)
                {
                    if (arguments[i].FirstValue == 0)
                        temp[i] = 0;
                    else
                        temp[i] = 1;
                }
                result = new Token(TokenType.Bool, temp);
            }
            return result;
        }
    }
}
