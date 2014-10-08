using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace MathProcessorLib
{
    public enum AngleUnit { Radian, Degree }
    public delegate Token FunctionDelegate(string operation, List<Token> arguments);

    public static class Function
    {
        static Dictionary<string, FunctionDelegate> functions = new Dictionary<string, FunctionDelegate>();
        static List<string> directiveNames = new List<string>();
        static List<string> functionNames = new List<string>();
        static AngleUnit aunit = AngleUnit.Radian;
        static Dictionary<string, List<string>> funcGroup = new Dictionary<string, List<string>>();

        static Function()
        {
            try
            {
                ArrayManipulator.CreateFunctions();
                BasicCalculations.CreateFunctions();
                LogsAndPowers.CreateFunctions();
                Numerical.CreateFunctions();
                Statistics.CreateFunctions();
                Trigonometry.CreateFunctions();
                Booleans.CreateFunctions();
                Miscellaneous.CreateFunctions();
                Matrix.CreateFunctions();                
                Testbed.CreateFunctions();
                Directive.CreateFunctions();
                Plot.CreateFunctions();
                Text.CreateFunctions();
            }
            catch //(TypeInitializationException)
            {
                //MessageBox.Show("Initialization error. Please restart the application \r\n" + e.Message);
            }
        }

        public static void AddFunction(string name, FunctionDelegate fd)
        {
            functions.Add(name, fd);
            functionNames.Add(name);
        }

        public static void AddReplaceFunction(string name, FunctionDelegate fd)
        {
            if (functionNames.Contains(name))
            {
                functions[name] = fd;
            }
            else
            {
                functions.Add(name, fd);
                functionNames.Add(name);
            }
        }

        public static void AddDirective(string name, FunctionDelegate fd)
        {
            functions.Add(name, fd);
            directiveNames.Add(name);
        }

        public static AngleUnit Unit
        {
            get { return aunit; }
            set { aunit = value; }
        }
        
        public static bool IsFuction(string funcName)
        {
            if (functionNames.Contains(funcName))
                return true;
            else
                return false;
        }

        public static bool IsDirective(string name)
        {
            if (directiveNames.Contains(name))
                return true;
            else
                return false;
        }

        public static Token InvokeFunction(string funcName, List<Token> arguments)
        {
            try
            {
                return functions[funcName](funcName, arguments);
            }
            catch (OutOfMemoryException)
            {
                return Token.Error("Not enough memory to perform the requested operation.");
            }
            catch
            {
                return Token.Error("Error in executing requested operation. Make sure the command exists.");
            }
        }
    }
}
