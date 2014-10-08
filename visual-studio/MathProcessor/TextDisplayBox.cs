using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace MathProcessor
{
    public enum DisplayBoxType { Default, Error, Input }

    class TextDisplayBox
    {        
        static readonly FormattedText blackPointer = TextManager.CreateFormattedText(">>", Brushes.Black);
        static readonly FormattedText bluePointer = TextManager.CreateFormattedText(">>", Brushes.Blue);
        static readonly FormattedText redPointer = TextManager.CreateFormattedText(">>", Brushes.Red);
        static readonly FormattedText placeHolder = TextManager.CreateFormattedText("M");

        public FormattedText Pointer
        {
            get
            {
                switch (BoxType)
                {
                    case DisplayBoxType.Input:
                        return bluePointer;
                    case DisplayBoxType.Error:
                        return redPointer;
                    default:
                        return blackPointer;
                }
            }
        }

        double width;
        double height = placeHolder.Height;
        public bool Selected { get; set; }

        List<FormattedText> formattedTextList = new List<FormattedText>();
        List<Point> locations = new List<Point>();
        public Point Location
        {
            get { return locations[0]; }
        }

        public void AdjustLocations(Point value)
        {
            locations[0] = value;
            for (int i = 1; i < locations.Count; i++)
            {
                locations[i] = new Point(value.X, locations[i - 1].Y + (formattedTextList[i-1].Height > 0 ? formattedTextList[i-1].Height : placeHolder.Height));
            }
        }
        public double Width { get { return width; } }
        public double Height { get { return height; } }
        public double Left { get { return Location.X; } }
        public double Top { get { return Location.Y; } }
        public double Bottom { get { return Location.Y + height; } }
        public double Right { get { return Location.X + width; } }
        public Rect Bounds { get { return new Rect(Location, new Point(Right, Bottom)); } }
        public DisplayBoxType BoxType { get; set; }
        public string Text { get; private set; }
        public TextDisplayBox (DisplayBoxType boxType, Point location)
        {
            BoxType = boxType;
            locations.Add(location);
            formattedTextList.Add(TextManager.CreateFormattedText("", Brushes.Black));
            Text = "";
        }

        public void SetText(string text)
        {
            Text = text;
            Brush brush = Brushes.Black;
            switch (BoxType)
            {
                case DisplayBoxType.Input:
                    brush = Brushes.Blue;
                    break;
                case DisplayBoxType.Error:
                    brush = Brushes.Red;
                    break;
            }
            string[] lines = Regex.Split(text, @"(?<=[\n])");
            formattedTextList.Clear();
            Point firstPoint = locations[0];
            locations.Clear();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < lines.Length; i++)
            {
                sb.Append(lines[i]);
                if ((i + 1) % 100 == 0)
                {
                    formattedTextList.Add(TextManager.CreateFormattedText(sb.ToString(), brush));
                    locations.Add(new Point());
                    sb.Clear();
                }
            }
            if (sb.Length > 0 || text.Length == 0)
            {
                formattedTextList.Add(TextManager.CreateFormattedText(sb.ToString(), brush));
                locations.Add(new Point());
            }
            height = 0;
            width = 0;
            foreach (var v in formattedTextList)
            {
                height += v.Height > 0 ? v.Height : placeHolder.Height;
                width = Math.Max(width, v.WidthIncludingTrailingWhitespace);
            }
            AdjustLocations(firstPoint);
        }

        public void DrawTextDisplayBox (DrawingContext dc, double top, double bottom, double right)
        {
            //System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            //watch.Start();
            if (Selected)
            {
                dc.DrawRectangle(Brushes.LightGray, null, new Rect(Location.X, Location.Y, right, Height+1));
            }
            for (int i = 0; i < locations.Count; i++)
            {
                if (locations[i].Y >= bottom)
                {
                    break;
                }
                if (locations[i].Y + formattedTextList[i].Height >= top)
                {                    
                    dc.DrawText(formattedTextList[i], locations[i]);                    
                }
            }

            //int first = 0;
            //int max = locations.Count;
            //while (max != 0)
            //{
            //    if (locations[first].Y + formattedTextList[first].Height >= top)
            //    {
            //        max = (max + 1) / 2;
            //        if (locations[first].Y <= top)
            //            break;
            //        else
            //            first -= max;
            //        if (first < 0)
            //        {
            //            first = 0;
            //            break;
            //        }
            //    }
            //    else
            //    {
            //        max = max / 2;
            //        first += max;
            //        if (first > locations.Count - 1)
            //        {
            //            first = locations.Count - 1;
            //            break;
            //        }
            //    }
            //}
            //for (int i = first; i < formattedTextList.Count; i++)
            //{
            //    if (locations[i].Y >= bottom)
            //        break;
            //    dc.DrawText(formattedTextList[i], locations[i]);
            //}
            //watch.Stop();
        }
    }
}
