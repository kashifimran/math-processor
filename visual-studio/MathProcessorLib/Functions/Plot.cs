using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace MathProcessorLib
{
    class PlotToken : IExtendedToken
    {
        string name = "Plot";
        public string GetName()
        {
            return name;
        }
        public Token CopyToken(Token token)
        {
            var t = Token.CustomToken(this.GetType(), new List<PlotInfo>());
            List<PlotInfo> oldList = (List<PlotInfo>)token.CustomData;
            foreach (PlotInfo pi in oldList)
            {
                PlotInfo p = new PlotInfo
                {
                    X = pi.X,
                    Y = pi.Y,
                    Color = pi.Color,
                    Pen = pi.Pen,
                    Rotation = pi.Rotation,
                    Thickness = pi.Thickness,
                    XTranslate = pi.XTranslate,
                    YTranslate = pi.YTranslate,
                    Brush = pi.Brush
                };
                ((List<PlotInfo>)t.CustomData).Add(p);
            }
            return t;
        }
    }

    class BrushToken : IExtendedToken
    {
        string name = "Brush";
        public string GetName()
        {
            return name;
        }

        public Token CopyToken(Token token)
        {
            return Token.CustomToken(this.GetType(), token.CustomData);
        }
    }

    static class Plot
    {
        static List<Color> defaultColors = new List<Color>();
        static int nextColorIndex = 0;

        static Plot()
        {
            defaultColors.AddRange(new Color[] { Color.DarkBlue, Color.DarkGreen, Color.Crimson, Color.Brown,
                                                 Color.Magenta,  Color.DarkOrange,
                                                 Color.DarkRed, Color.Gold, Color.Cyan, Color.Wheat, Color.Violet, Color.Black});
        }

        public static void CreateFunctions()
        {
            Token.RegisterCustomType(typeof(PlotToken));
            Token.RegisterCustomType(typeof(BrushToken));

            Function.AddFunction("plot", CreatePlot);//the old classical method, imported from Miscellaneous file

            //new methods!
            Function.AddFunction("createplot", CreatePlot_New);
            Function.AddFunction("addplot", AddPlot);
            Function.AddFunction("showplot", Show);
            Function.AddFunction("removeplot", RemovePlot);
            Function.AddFunction("copyplot", CopyPlot);
            Function.AddFunction("lgbrush", CreateLGBrush); // LinearGradientBrush 
            Function.AddFunction("pgbrush", CreatePGBrush); // PathGradientBrush 
        }

        // LinearGradientBrush 
        public static Token CreateLGBrush(string operation, List<Token> arguments)
        {
            if (arguments.Count < 4 || arguments.Count > 7)
            {
                return Token.Error("lgbrush() error: Between 4 and 7 parameters expected.");
            }
            if (arguments[0].Count != 4)
            {
                return Token.Error("lgbrush() error: First parameters must provide 4 numeric values (e.g. use array() function).");
            }
            if (arguments[1].TokenType != TokenType.Bool)
            {
                return Token.Error("lgbrush() error: 2nd parameter must be a Boolean value to specify whether to apply Gamma correction to the brush.");
            }
            if (arguments[2].Count != 1)
            {
                return Token.Error("lgbrush() error: 3rd parameter must be a number to specify the orientation angle of the brush.");
            }
            if (!arguments[3].IsOfType(typeof(StringsToken)))
            {
                return Token.Error("lgbrush() error: 3rd parameters must be an array of color names or (A)RGB values in valid format.");
            }
            List<string> colors = (List<string>)arguments[3].CustomData;
            if (arguments.Count == 4 && colors.Count != 2)
            {
                return Token.Error("lgbrush() error: 4th parameters should have 2 color values in this call.");
            }
            if (arguments.Count > 4 && (arguments[4].Count < 2 || arguments[4].Count != colors.Count))
            {
                return Token.Error("lgbrush() error: 4th and 5th parameters should have equal number of items.");
            }
            if (arguments.Count > 5 && arguments[5].Count != colors.Count)
            {
                return Token.Error("lgbrush() error: 4th, 5th and 6th parameters should have equal number of items.");
            }
            Color firstColor = Color.Black;
            Color secondColor = Color.Black;
            if (arguments.Count == 4)
            {
                try
                {
                    firstColor = createColor(colors[0]);
                    secondColor = createColor(colors[1]);
                }
                catch
                {
                    return Token.Error("3rd parameters must be an array of valid color names or (A)RGB values (use strings() function).");
                }
            }
            float[] arr = arguments[0].FloatArray;
            LinearGradientBrush lgBrush = new LinearGradientBrush(new RectangleF(arr[0], arr[1], arr[2], arr[3]), firstColor, secondColor, (float)arguments[2].FirstValue);
            if (arguments.Count > 4)
            {
                try
                {
                    if (colors.Count == 2)
                    {
                        Blend myBlend = new Blend();
                        myBlend.Positions = arguments[4].FloatArray;
                        if (arguments.Count == 6)
                        {
                            myBlend.Factors = arguments[5].FloatArray;
                        }
                        lgBrush.Blend = myBlend;
                    }
                    else
                    {
                        ColorBlend myBlend = new ColorBlend();
                        myBlend.Positions = arguments[4].FloatArray;
                        Color[] c = new Color[colors.Count];
                        for (var i = 0; i < colors.Count; i++)
                        {
                            c[i] = createColor(colors[i]);
                        }
                        myBlend.Colors = c;
                        lgBrush.InterpolationColors = myBlend;
                    }
                }
                catch
                {
                    return Token.Error("Erorr: The brush could not be created. Please check input parameters");
                }
            }
            return Token.CustomToken(typeof(BrushToken), lgBrush);
        }


        //PathGradientBrush 
        public static Token CreatePGBrush(string operation, List<Token> arguments)
        {
            return Token.Error("Not yet implemented.");
        }

        //Old classical plot routine
        public static Token CreatePlot(string operation, List<Token> arguments)
        {
            if (arguments.Count < 2)
                return Token.Error("Function requires more than one parameter");

            GraphForm gf = new GraphForm();
            for (int i = 0; i < arguments.Count; )
            {
                if (arguments.Count - i > 2 && arguments[i + 2].TokenType == TokenType.Text)
                {
                    if (!gf.AddCurve(arguments[i].VectorArray, arguments[i + 1].VectorArray, arguments[i + 2].StrData))
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
            gf.Show();
            return Token.Void;
        }

        //new thing!
        public static Token CreatePlot_New(string operation, List<Token> arguments)
        {
            if (arguments.Count > 0)
                return Token.Error("Function does not expect any parameters.");
            return Token.CustomToken(typeof(PlotToken), new List<PlotInfo>());
        }

        public static Token AddPlot(string operation, List<Token> arguments)
        {
            if (arguments.Count < 3 || arguments.Count > 9)
                return Token.Error("Wrong number of arguments passed to the function");
            var plot = arguments[0];
            Color color = defaultColors[nextColorIndex];
            double rotation = 0;
            float xTranslate = 0;
            float yTranslate = 0;
            float thickness = 1;
            Brush brush = null;

            if (!plot.IsOfType(typeof(PlotToken)))
            {
                return Token.Error("1st parameter must be a plot created through a call to createplot() function");
            }
            if (arguments[1].TokenType != TokenType.Vector)
            {
                return Token.Error("2nd parameter must be a numeric array");
            }
            if (arguments[2].TokenType != TokenType.Vector)
            {
                return Token.Error("3rd parameter must be a numeric array");
            }
            if (arguments[1].Count != arguments[2].Count)
            {
                return Token.Error("2nd & 3rd parameters must be of equal length");
            }
            if (arguments.Count > 3)
            {
                if (arguments[3].TokenType != TokenType.Vector && arguments[3].Count != 1)
                {
                    return Token.Error("4th parameter, when provided, must be a number (rotation).");
                }
                rotation = arguments[3].FirstValue;
            }
            if (arguments.Count > 4)
            {
                if (arguments[4].TokenType != TokenType.Vector && arguments[4].Count != 1)
                {
                    return Token.Error("5th parameter must be a number (line width). If not provided, default value of 1 will be used.");
                }
                thickness = (float)arguments[4].FirstValue;

            }
            if (arguments.Count > 5)
            {
                try
                {
                    if (arguments[5].TokenType != TokenType.Text)
                    {
                        throw new Exception();
                    }
                    string str = arguments[5].StrData.Trim();
                    if (str.Length > 0)
                    {
                        color = createColor(str);
                    }
                    else
                    {
                        nextColorIndex++;
                        nextColorIndex %= defaultColors.Count;
                    }
                }
                catch
                {
                    return Token.Error("6th parameter must be a valid color name. Leave out to get a random color.");
                }
            }
            else
            {
                nextColorIndex++;
                nextColorIndex %= defaultColors.Count;
            }
            if (arguments.Count > 6)
            {
                if (arguments[6].TokenType != TokenType.Vector && arguments[6].Count != 1)
                {
                    return Token.Error("7th parameter, when provided, must be a number (x translation).");
                }
                xTranslate = (float)arguments[6].FirstValue;
            }
            if (arguments.Count > 7)
            {
                if (arguments[7].TokenType != TokenType.Vector && arguments[7].Count != 1)
                {
                    return Token.Error("8th parameter, when provided, must be a number (y translation).");
                }
                yTranslate = (float)arguments[7].FirstValue;
            }
            if (arguments.Count > 8)
            {
                if (arguments[8].TokenType == TokenType.Text)
                {
                    try
                    {
                        brush = new SolidBrush(createColor(arguments[8].StrData));
                    }
                    catch
                    {
                        return Token.Error("9th parameter is not a valid color value (colro name or (A)RGB value).");
                    }
                }
                else if (arguments[8].IsOfType(typeof(BrushToken)))
                {
                    brush = (Brush)arguments[8].CustomData;
                }
                else
                {
                    return Token.Error("9th parameter, when provided, must either be a Brush (use createbrush()) or a color name or (A)RGB value.");
                }
            }

            Pen pen = new Pen(color, (float)Math.Max(0.5, Math.Min(100, thickness)));
            List<PlotInfo> list = (List<PlotInfo>)plot.CustomData;
            var pi = new PlotInfo
            {
                X = arguments[1].FloatArray,
                Y = arguments[2].FloatArray,
                Color = color,
                Rotation = (float)rotation,
                Thickness = thickness,
                Pen = pen,
                XTranslate = xTranslate,
                YTranslate = yTranslate,
                Brush = brush,
            };
            list.Add(pi);
            return Token.Void;
        }

        private static Color createColor(string str)
        {
            Color color;
            if (str[0] == '#' && (str.Length == 7 || str.Length == 9))
            {
                str = str.Substring(1, str.Length - 1);
                if (str.Length == 6)
                {
                    str = "FF" + str;
                }
                string temp = "";
                for (var i = 0; i < 8; i += 2)
                {
                    temp += Convert.ToInt32(str.Substring(i, 2), 16) + ",";
                }
                str = temp.Trim(',');
            }
            if (char.IsDigit(str[0])) //System.Text.RegularExpressions.Regex.IsMatch(str, @"^\d"))
            {
                int alpha = 255;
                int red = 255;
                int green = 255;
                int blue = 255;
                string[] values = str.Split(',');
                if (values.Length == 3)
                {
                    red = int.Parse(values[0]);
                    green = int.Parse(values[1]);
                    blue = int.Parse(values[2]);
                }
                else if (values.Length == 4)
                {
                    alpha = int.Parse(values[0]);
                    red = int.Parse(values[1]);
                    green = int.Parse(values[2]);
                    blue = int.Parse(values[3]);
                }
                color = Color.FromArgb(alpha, red, green, blue);
            }
            else
            {
                color = Color.FromName(str);
                if (color.A == 0 && color.B == 0 && color.R == 0 && color.G == 0)
                {
                    throw new Exception();
                }
            }
            return color;
        }

        public static Token Show(string operation, List<Token> arguments)
        {
            float scaleX = 1, scaleY = 1;
            float rotation = 0;
            if (arguments.Count < 1 || !arguments[0].IsOfType(typeof(PlotToken)))
            {
                return Token.Error("Error: First parameter must be of type 'Plot' created using the createplot() function");
            }
            if (arguments.Count > 4)
            {
                return Token.Error("Error: Too many parameters.");
            }
            for (var i = 1; i < arguments.Count; i++)
            {
                if (arguments[i].TokenType != TokenType.Vector || arguments[i].Count != 1)
                {
                    return Token.Error("Error: Parameter " + (i + 1) + "must be a single numeric value.");
                }
            }
            if (arguments.Count == 2)
            {
                rotation = (float)arguments[1].FirstValue;
            }
            else if (arguments.Count == 3)
            {
                scaleX = (float)arguments[1].FirstValue;
                scaleY = (float)arguments[2].FirstValue;
            }
            else if (arguments.Count == 4)
            {
                rotation = (float)arguments[1].FirstValue;
                scaleX = (float)arguments[2].FirstValue;
                scaleY = (float)arguments[3].FirstValue;
            }

            List<PlotInfo> list = (List<PlotInfo>)arguments[0].CopyToken().CustomData;
            if (list.Count == 0)
            {
                return Token.Error("The plot is empty. You can fill the plot using addplot() function");
            }
            GraphForm_New gf = new GraphForm_New(list, rotation, scaleX, scaleY);
            gf.Show();
            return Token.Void;
        }

        public static Token CopyPlot(string operation, List<Token> arguments)
        {
            if (arguments.Count != 1 && !arguments[0].IsOfType(typeof(PlotToken)))
            {
                return Token.Error("Error: Function expects exactly one parameter of type plot. Use createplot to create a new plot.");
            }
            return arguments[0].CopyToken();
        }

        public static Token RemovePlot(string operation, List<Token> arguments)
        {
            if (arguments.Count < 2 || !arguments[0].IsOfType(typeof(PlotToken)))
            {
                return Token.Error("At least two parameters expected. The first must be of type plot.");
            }
            List<int> indexes = new List<int>();
            List<PlotInfo> list = (List<PlotInfo>)arguments[0].CustomData;
            for (var i = 1; i < arguments.Count; i++)
            {
                if (arguments[i].TokenType != TokenType.Vector || arguments[i].Count != 1)
                {
                    return Token.Error("Error: parameter " + (i + 1) + " must be a single numeric value");
                }
                if ((int)arguments[i].FirstValue != arguments[i].FirstValue)
                {
                    return Token.Error("Error: parameter " + (i + 1) + " must be an integer");
                }
                if (arguments[i].FirstValue > list.Count - 1)
                {
                    return Token.Error("Error: parameter " + (i + 1) + " out of bounds. The Plot does not have anything at this index.");
                }
                if (!indexes.Contains((int)arguments[i].FirstValue))
                {
                    indexes.Add((int)arguments[i].FirstValue);
                }
            }
            indexes.Sort();
            for (var i = indexes.Count - 1; i >= 0; i--)
            {
                list.RemoveAt(i);
            }
            return new Token(TokenType.Text, "", "Specified indexes removed. Remaining items in the Plot: " + list.Count);
        }
    }
}
