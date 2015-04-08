using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;


namespace MathProcessorLib
{
    public enum TokenType
    {
        Void, Error, Operator, Function, UserFunction, Directive, Vector, Text, Block,
        Loop, Condition, Bool, FunctionDefiner, Break, Matrix, Custom, ConditionExecuted
    }

    public interface IExtendedToken
    {
        Token CopyToken(Token t);
        string GetName();
    }


    public class Token
    {
        static Dictionary<Type, IExtendedToken> extendedTokens = new Dictionary<Type, IExtendedToken>();
        static Dictionary<string, Type> typeMap = new Dictionary<string, Type>();

        public static void RegisterCustomType(Type type)
        {
            if (!extendedTokens.ContainsKey(type))
            {
                var extendedToken = Activator.CreateInstance(type);
                if (type.GetInterface("IExtendedToken") != null)
                {
                    extendedTokens.Add(type, (IExtendedToken)extendedToken);
                    typeMap.Add(((IExtendedToken)extendedToken).GetName(), type);
                }
            }
        }
        
        //We dont' need this for now
        //public static void RegisterNewType(IExtendedToken extendedToken)
        //{
        //    extendedTokens.Add(extendedToken.GetType(), extendedToken);
        //    typeMap.Add(extendedToken.GetName(), extendedToken.GetType());
        //}

        public static Token CustomToken(IExtendedToken creator, object customData)
        {
            return CustomToken(creator.GetType(), customData);
        }

        public static Token CustomToken(Type type, object customData)
        {
            if (extendedTokens.ContainsKey(type))
            {
                return new Token(TokenType.Custom, "", extendedTokens[type].GetName()) { CustomData = customData };
            }
            else
            {
                throw new ApplicationException("Wrong type");
            }
        }

        public bool IsOfType(Type extendedType)
        {
            try
            {
                return extendedType == typeMap[data];
            }
            catch
            {
                return false;
            }
        }

        TokenType type = TokenType.Void;
        string name = "";
        string data = "";
        List<double> vector = new List<double>();
        double extra = 0;

        public object CustomData { get; set; }

        public List<double> Vector { get { return vector; } }

        public Token() { }

        public void SaveXML(XElement root)
        {
            XElement element = new XElement("token");
            element.Add(new XElement("type", type));
            element.Add(new XElement("name", name));
            element.Add(new XElement("data", data));
            element.Add(new XElement("extra", extra));
            StringBuilder sb = new StringBuilder();
            foreach (double d in vector)
            {
                sb.Append(d + ",");
            }
            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            element.Add(new XElement("vector", sb.ToString()));
            root.Add(element);
        }

        public void LoadXML(XElement element)
        {
            type = (TokenType)Enum.Parse(typeof(TokenType), element.Element("type").Value);
            name = element.Element("name").Value;
            data = element.Element("data").Value;
            extra = double.Parse(element.Element("extra").Value);
            string numberData = element.Element("vector").Value;
            if (numberData.Length > 0)
            {
                string[] values = numberData.Split(',');
                foreach (string s in values)
                {
                    vector.Add(double.Parse(s));
                }
            }
        }

        public void ResetValues(Token t)
        {
            this.type = t.type;
            this.data = t.data;
            this.vector = t.vector;
            this.extra = t.extra;
            this.CustomData = t.CustomData;
        }

        public static readonly Token Void = new Token(TokenType.Void);

        public static Token Error(string strData)
        {
            return new Token(TokenType.Error, "", strData);
        }

        public Token CopyToken()
        {
            if (type != TokenType.Custom)
            {
                throw new ApplicationException("Not allowed");
            }
            var item = extendedTokens[typeMap[data]];
            return item.CopyToken(this);
        }

        public Token Clone()
        {
            /* 
               Token temp = new Token(type, name, data);
               temp.extra = this.extra;
               temp.vector.AddRange(this.Vector.ToArray());
            */
            return Clone(type);
        }

        public Token Clone(TokenType newType)
        {
            Token temp = new Token(newType, name, data);
            temp.CustomData = this.CustomData;
            temp.extra = this.extra;
            temp.vector.AddRange(this.VectorArray.ToArray());
            return temp;
        }

        public Token(TokenType type)
        {
            this.type = type;
        }

        public double Extra
        {
            get { return extra; }
            set { extra = value; }
        }

        public Token(TokenType type, string tokenName)
        {
            this.type = type;
            this.name = tokenName;
            this.data = tokenName;
        }

        public Token(TokenType type, string tokenName, string strData)
        {
            this.type = type;
            this.name = tokenName;
            this.data = strData;
        }

        public Token(TokenType type, double value)
        {
            this.type = type;
            this.vector.Add(value);
        }

        public Token(TokenType type, double[] vector)
            : this(type, "", vector)
        {
        }

        public Token(TokenType type, double extra, double value)
            : this(type, value)
        {
            this.extra = extra;
        }


        public Token(TokenType type, double extra, double[] vector)
            : this(type, "", vector)
        {
            this.extra = extra;
        }

        public Token(TokenType type, double extra, List<double> values)
            : this(type, "", values)
        {
            this.extra = extra;
        }


        public Token(TokenType type, string tokenName, double[] vector)
        {
            this.type = type;
            this.name = tokenName;
            this.vector.AddRange(vector);
        }

        public Token(TokenType type, List<double> values)
            : this(type, "", values)
        {
        }

        public Token(TokenType type, string tokenName, List<double> values)
        {
            this.type = type;
            this.name = tokenName;
            this.vector = values;
        }

        public string StrData
        {
            get { return data; }
            set
            {
                if (type == TokenType.Text || type == TokenType.UserFunction || type == TokenType.Condition || type == TokenType.Loop || type == TokenType.Block)
                    data = value;
                else
                    throw new ArgumentException("Type of token incompatible");
            }
        }

        public void TrimToken()
        {
            name = name.Trim();
            data = data.Trim();
        }

        public void Append(double num)
        {
            vector.Add(num);
        }

        public void Append(double[] numArray)
        {
            vector.AddRange(numArray);
        }

        public List<double> GetRange(int index, int count)
        {
            return vector.GetRange(index, count);
        }

        public void Insert(int index, double number)
        {
            vector.Insert(index, number);
        }


        public void InsertRange(int index, double[] numArray)
        {
            vector.InsertRange(index, numArray);
        }

        public void RemoveRange(int index, int count)
        {
            vector.RemoveRange(index, count);
        }

        public void RemoveAt(int index)
        {
            vector.RemoveAt(index);
        }


        public String TokenName
        {
            get { return name; }
            set
            {
                name = value;
            }
        }

        public TokenType TokenType
        {
            get { return type; }
            set { type = value; }
        }

        public double FirstValue
        {
            get
            {
                return vector[0];
            }
        }

        public double LastValue
        {
            get
            {
                return vector[vector.Count - 1];
            }
        }

        public double this[int i]
        {
            get
            {
                return vector[i];
            }
            set
            {
                vector[i] = value;
            }
        }

        public double[] VectorArray
        {
            get { return vector.ToArray(); }
            set
            {
                vector.Clear();
                vector.AddRange(value);
            }
        }

        public float[] FloatArray
        {
            get { return vector.Select(x => (float)x).ToArray(); }
        }

        public double[] Sort(string order)
        {
            List<double> temp = vector.ToList();
            temp.Sort();
            if (order == "sortd")
            {
                temp.Reverse();
            }
            return temp.ToArray();
        }

        public int Count
        {
            get { return vector.Count; }
        }

        public double Sum
        {
            get { return vector.Sum(); }
        }

        public double Prod
        {
            get
            {
                double product = 1;
                foreach (double d in vector)
                    product *= d;
                return product;
            }
        }

        public double Average
        {
            get { return vector.Average(); }
        }

        public double Min
        {
            get { return vector.Min(); }
        }

        public double Max
        {
            get { return vector.Max(); }
        }

        public string GetVectorString(string format, int count = 10)
        {
            return GetVectorString(format, " ", count);
        }

        public string GetVectorString(string format, string seperator, int count = 10)
        {
            if (type != TokenType.Vector && type != TokenType.Matrix)
                throw new ArgumentException("Method only valid for vectors and matrices");
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < vector.Count; i++)
            {
                if (i > 0 && i % count == 0)
                {
                    strBuilder.Append(Environment.NewLine);
                }
                strBuilder.Append(vector[i].ToString(format) + seperator);
            }
            return strBuilder.ToString();
        }

        public string GetMatrixString(string format)
        {
            return GetMatrixString(format, " ");
        }

        public string GetMatrixString(string format, string seperator)
        {
            if (type != TokenType.Matrix)
                throw new ArgumentException("Method only valid matrices.");
            StringBuilder strBuilder = new StringBuilder();
            int columns = vector.Count / (int)extra;
            for (int i = 0; i < vector.Count; i++)
            {
                if (i > 0 && i % columns == 0)
                {
                    strBuilder.Append(Environment.NewLine);
                }
                strBuilder.Append(vector[i].ToString(format) + seperator);
            }
            return strBuilder.ToString();
        }


        public string GetString(int count = 10)
        {
            switch (type)
            {
                case TokenType.Vector:
                    return GetVectorString(Calculator.DefaultFormatString, count);
                case TokenType.Matrix:
                    return GetMatrixString(Calculator.DefaultFormatString);
                case TokenType.Bool:
                    return GetBoolString(count);
                case TokenType.Error:
                case TokenType.Text:
                case TokenType.Custom:
                    return data;
                default:
                    return "";
            }
        }

        string GetBoolString(int count)
        {
            StringBuilder strBuilder = new StringBuilder();
            if (extra == 1)
            {
                for (int i = 0; i < vector.Count; i++)
                {
                    if (i > 0 && i % count == 0)
                    {
                        strBuilder.Append(Environment.NewLine);
                    }
                    if (vector[i] == 0) // 0 is false
                        strBuilder.Append("false ");
                    else
                        strBuilder.Append("true ");
                }
            }
            else
            {
                for (int i = 0; i < vector.Count; i++)
                {
                    if (i > 0 && i % count == 0)
                    {
                        strBuilder.Append(Environment.NewLine);
                    }
                    if (vector[i] == 0) // 0 is false
                        strBuilder.Append("0 ");
                    else
                        strBuilder.Append("1 ");
                }
            }
            if (strBuilder.Length > 0 && strBuilder[strBuilder.Length - 1] == ' ')
                strBuilder.Remove(strBuilder.Length - 1, 1);
            return strBuilder.ToString();
        }
    }
}
