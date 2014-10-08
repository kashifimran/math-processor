using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;
using System.Globalization;
using System.Xml.Linq;

namespace MathProcessor
{
    static class TextManager
    {
        static Typeface typeFace = new Typeface("Courier New");
        static double fontSize = 16;

        public static FormattedText CreateFormattedText(string text)
        {
            return CreateFormattedText(text, Brushes.Black);
        }
        
        public static FormattedText CreateFormattedText(string text, Brush brush)
        {
            return new FormattedText(text, CultureInfo.InvariantCulture, System.Windows.FlowDirection.LeftToRight, typeFace, fontSize, brush);
        }

        public static double GetTextWidth(string text, int count)
        {
            FormattedText formattedText = new FormattedText(text.Substring(0, count),
                                                            CultureInfo.InvariantCulture,
                                                            FlowDirection.LeftToRight,
                                                            typeFace,
                                                            fontSize,
                                                            Brushes.Black);
            return formattedText.WidthIncludingTrailingWhitespace;
        }        
    }
}
