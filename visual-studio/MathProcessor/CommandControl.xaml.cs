using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MathProcessorLib;
using System.Xml.Linq;

namespace MathProcessor
{
    /// <summary>
    /// Interaction logic for CommandControl.xaml
    /// </summary>
    public partial class CommandControl : UserControl
    {
        Caret caret = new Caret(false);
        CommandCashe commandCache = new CommandCashe();
        List<TextDisplayBox> displayBoxes = new List<TextDisplayBox>();
        List<int> selectedBoxesIndexes = new List<int>();
        TextDisplayBox currentBox = null;
        StringBuilder currentCommand = new StringBuilder();
        int caretIndex = 0;
        int gap = 4;
        int padding = 10;
        bool currentIsMultiLine = false;
        static double oneLineHeight = TextManager.CreateFormattedText("M").Height;
        bool isFirstIntermediateResult = false;
        FormattedText pointer = TextManager.CreateFormattedText(">>", Brushes.Black);
        

        public CommandControl()
        {
            InitializeComponent();
            mainGrid.Children.Add(caret);
            caret.Location = new Point(pointer.Width + gap + padding, padding);
            currentBox = new TextDisplayBox(DisplayBoxType.Input, caret.Location);
            caret.CaretLength = pointer.Height;
            displayBoxes.Add(currentBox);
            this.MinHeight = 800;
            this.MinWidth = 1200;
            Calculator.IntermediateResultProduced += new IntermediateResult(Calculator_IntermediateResultProduced);
            this.LostFocus += new RoutedEventHandler(CommandControl_LostFocus);
            this.GotFocus += new RoutedEventHandler(CommandControl_GotFocus);
        }
        
        void CommandControl_GotFocus(object sender, RoutedEventArgs e)
        {
            caret.StartBlinking();
        }

        void CommandControl_LostFocus(object sender, RoutedEventArgs e)
        {
            caret.StopBlinking();
        }

        void Calculator_IntermediateResultProduced(Token result)
        {
            if (result.TokenType != TokenType.Void)
            {
                if (isFirstIntermediateResult)
                {
                    currentCommand.Clear();
                    string text = result.GetString();
                    currentCommand.Append(text);
                    AddNewBox(DisplayBoxType.Default, text);
                    isFirstIntermediateResult = false;
                }
                else
                {
                    currentCommand.Append(Environment.NewLine);
                    currentCommand.Append(result.GetString());                    
                }
            }            
        }

        private void CommandControl_Loaded(object sender, RoutedEventArgs e)
        {
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            ScrollViewer scrollViewer = Parent as ScrollViewer;
            double top = scrollViewer.VerticalOffset - padding;
            double bottom = scrollViewer.VerticalOffset + scrollViewer.ViewportHeight - padding;
            for (int i = 0; i < displayBoxes.Count; i++)
            {
                if (displayBoxes[i].Top >= bottom)
                {
                    break;
                }
                if (displayBoxes[i].Bottom >= top)
                {
                    dc.DrawText(displayBoxes[i].Pointer, new Point(padding, displayBoxes[i].Location.Y));
                    displayBoxes[i].DrawTextDisplayBox(dc, top, bottom, this.ActualWidth);
                }
            }
            //int first = 0;
            //int max = displayBoxes.Count;
            //while (max != 0)
            //{
            //    if (displayBoxes[first].Bottom >= top)
            //    {
            //        max = (max + 1) / 2;
            //        if (displayBoxes[first].Top <= top)
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
            //        if (first > displayBoxes.Count - 1)
            //        {
            //            first = displayBoxes.Count - 1;
            //            break;
            //        }
            //    }
            //}
            //for (int i=first; i < displayBoxes.Count; i++)
            //{
            //    if (displayBoxes[i].Top >= bottom)
            //        break;
            //    dc.DrawText(displayBoxes[i].Pointer, new Point(0, displayBoxes[i].Location.Y));
            //    displayBoxes[i].DrawText(dc, top, bottom);
            //}
        }

        private void CommandControl_TextInput(object sender, TextCompositionEventArgs e)
        {
            ConsumeTextCommand(e.Text);
        }

        public void RunFileCommand(string data)
        {
            //data = data.Replace(Environment.NewLine, " ");
            currentCommand.Clear();
            caretIndex = 0;
            ConsumeTextCommand(data);
            ExecuteCommand(data, false);
            AdjustCaret();
        }
        
        void ConsumeTextCommand(string command)
        {
            Deselect();
            currentCommand.Insert(caretIndex, command);
            currentBox.SetText(currentCommand.ToString());
            caretIndex += command.Length;
            this.MinWidth = Math.Max(this.MinWidth, currentBox.Width + 100);
            AdjustCaret();
        }

        private void AdjustCaret()
        {
            Point caretPoint = currentBox.Location;            
            if (currentIsMultiLine)
            {                   
                string text = currentCommand.ToString(0, caretIndex);
                string [] lines = text.Split('\n');
                caretPoint.Y = currentBox.Location.Y + TextManager.CreateFormattedText(text + "M").Height - oneLineHeight;
                caretPoint.X = TextManager.CreateFormattedText(lines.Last()).WidthIncludingTrailingWhitespace + currentBox.Location.X;                
            }
            else
            {
                if (caretIndex < currentCommand.Length)
                {
                    caretPoint.X = TextManager.CreateFormattedText(currentCommand.ToString(0, caretIndex)).WidthIncludingTrailingWhitespace + currentBox.Location.X;
                }
                else
                {
                    caretPoint.X = currentBox.Width + currentBox.Location.X;
                }
                caretPoint.Y = currentBox.Location.Y;
            }
            double height =0;
            foreach (var v in displayBoxes)
            {
                height += v.Height;
            }
            this.MinHeight = Math.Max(1200, height + 200);
            caret.Location = caretPoint;
            AdjustScrollViewer();
        }

        void AddNewBox(DisplayBoxType dbt, string text)
        {
            TextDisplayBox newBox = new TextDisplayBox(dbt, new Point(currentBox.Location.X, currentBox.Bottom));
            displayBoxes.Add(newBox);
            currentBox = newBox;
            this.MinHeight += currentBox.Height;
        }

        private void CommandControl_KeyDown(object sender, KeyEventArgs e)
        {
            if ((new[] { Key.Enter, Key.Left, Key.Right, Key.Up, Key.Down, Key.Back, Key.Home, Key.End }).Contains(e.Key))
            {
                e.Handled = true;
                Deselect();
            }
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (e.Key == Key.Enter)
                {
                    currentCommand.Insert(caretIndex, Environment.NewLine);
                    caretIndex += Environment.NewLine.Length;
                    currentIsMultiLine = true;
                    this.MinHeight += oneLineHeight;
                }
                else if (e.Key == Key.Home)
                {
                    (Parent as ScrollViewer).ScrollToHome();
                    (Parent as ScrollViewer).ScrollToLeftEnd();
                }
                else if (e.Key == Key.End)
                {
                    (Parent as ScrollViewer).ScrollToEnd();
                    (Parent as ScrollViewer).ScrollToRightEnd();
                    caretIndex = currentCommand.Length;
                }
            }
            else if (e.Key == Key.Enter)
            {
                ExecuteCommand(currentCommand.ToString(), true);
                currentIsMultiLine = false;
            }
            else if (e.Key == Key.Left && caretIndex > 0)
            {
                caretIndex--;
                if (currentCommand[caretIndex] == '\n')
                {
                    caretIndex -= 2;
                }
            }
            else if (e.Key == Key.Right && caretIndex < currentCommand.Length)
            {
                if (currentCommand[caretIndex] == '\r')
                {
                    caretIndex += 2;
                }
                else
                {
                    caretIndex++;
                }
            }
            else if (e.Key == Key.Up)
            {
                //currentCommand.Clear();
                //string command = commandCache.Previous();
                //currentCommand.Append(command);
                //currentBox.SetText(command);
                //caretIndex = 0;             
            }
            else if (e.Key == Key.Down)
            {
                //currentCommand.Clear();
                //string command = commandCache.Next();
                //currentCommand.Append(command);
                //currentBox.SetText(command);
                //caretIndex = 0;
            }
            else if (e.Key == Key.Back)
            {
                if (caretIndex > 0)
                {
                    currentCommand.Remove(caretIndex - 1, 1);
                    caretIndex--;
                    if (caretIndex > 0 && currentCommand[caretIndex - 1] == '\r')
                    {
                        currentCommand.Remove(caretIndex - 1, 1);
                        caretIndex--;
                    }
                    currentBox.SetText(currentCommand.ToString());
                }
            }
            else if (e.Key == Key.Home)
            {
                caretIndex = 0;
            }
            else if (e.Key == Key.End)
            {
                caretIndex = currentCommand.Length;
            }
            else if (e.Key == Key.Delete)
            {
                if (caretIndex < currentCommand.Length)
                {
                    currentCommand.Remove(caretIndex, 1);
                    currentBox.SetText(currentCommand.ToString());
                }
            }
            AdjustCaret();
        }

        private void ExecuteCommand(string command, bool addToCache)
        {
            if (addToCache)
            {
                commandCache.AddString(command);
            }
            isFirstIntermediateResult = true;
            Token result = Calculator.ProcessCommand(command);
            string text = result.GetString();
            if (result.TokenType == TokenType.Error)
            {
                AddNewBox(DisplayBoxType.Error, text);
                currentBox.SetText(text);
            }
            else if (result.TokenType != TokenType.Void)
            {
                AddNewBox(DisplayBoxType.Default, text);
                currentBox.SetText(text);
            }
            else
            {
                currentBox.SetText(currentCommand.ToString());
            }            
            this.MinWidth = Math.Max(this.MinWidth, currentBox.Width + 100);
            AddNewBox(DisplayBoxType.Input, "");
            currentCommand.Clear();
            caretIndex = 0;
        }

        private void CommandControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePoint = Mouse.GetPosition(this);
            double left = currentBox.Location.X;
            if (mousePoint.X < left)
            {
                for (int i = 0; i < displayBoxes.Count; i++)
                {
                    if (displayBoxes[i].Top < mousePoint.Y && displayBoxes[i].Bottom > mousePoint.Y)
                    {
                        //if (displayBoxes[i].Text.Length > 0 || displayBoxes[i].Selected)
                        {
                            displayBoxes[i].Selected = !displayBoxes[i].Selected;
                            if (displayBoxes[i].Selected && !selectedBoxesIndexes.Contains(i))
                            {
                                selectedBoxesIndexes.Add(i);                                
                            }
                            else if (selectedBoxesIndexes.Contains(i))
                            {
                                selectedBoxesIndexes.Remove(i);
                            }
                            InvalidateVisual();
                        }
                        break;
                    }
                }   
            }
            else
            {   
                caretIndex = currentCommand.Length;
                for (; caretIndex > 0; caretIndex--)
                {
                    FormattedText lastChar = TextManager.CreateFormattedText(currentCommand.ToString(caretIndex - 1, 1));
                    FormattedText textPart = TextManager.CreateFormattedText(currentCommand.ToString(0, caretIndex));
                    left = textPart.WidthIncludingTrailingWhitespace + currentBox.Pointer.Width + gap;
                    if (left <= mousePoint.X + lastChar.WidthIncludingTrailingWhitespace / 2)
                    {
                        break;
                    }
                }
                caret.Left = left;
                Deselect();
            }
            e.Handled = true;
        }

        private void Deselect()
        {
            if (selectedBoxesIndexes.Count > 0)
            {
                selectedBoxesIndexes.Clear();
                foreach (var v in displayBoxes)
                {
                    v.Selected = false;
                }
                InvalidateVisual();
            }
        }

        private void CommandControl_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void CommandControl_MouseMove(object sender, MouseEventArgs e)
        {

        }

        void AdjustScrollViewer()
        {
            ScrollViewer scrollViewer = Parent as ScrollViewer;
            //Vector offsetPoint = VisualTreeHelper.GetOffset(this);           

            if (scrollViewer != null)
            {
                double left = scrollViewer.HorizontalOffset;
                double top = scrollViewer.VerticalOffset;
                double right = scrollViewer.ViewportWidth + scrollViewer.HorizontalOffset;
                double bottom = scrollViewer.ViewportHeight + scrollViewer.VerticalOffset;
                double hOffset = 0;
                double vOffset = 0;
                bool rightDone = false;
                bool bottomDone = false;
                while (caret.Left > right - 8)
                {
                    hOffset += 8;
                    right += 8;
                    rightDone = true;
                }
                while (caret.VerticalCaretBottom > bottom - 10)
                {
                    vOffset += 10;
                    bottom += 10;
                    bottomDone = true;
                }
                while (caret.Left < left + currentBox.Pointer.Width + gap && !rightDone)
                {
                    hOffset -= 8;
                    left -= 8;
                }
                while (caret.Top < top + 10 && !bottomDone)
                {
                    vOffset -= 10;
                    top -= 10;
                }
                left = scrollViewer.HorizontalOffset;
                top = scrollViewer.VerticalOffset;
                scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset + hOffset);
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + vOffset);
                if (top == scrollViewer.VerticalOffset && left == scrollViewer.HorizontalOffset)
                {
                    InvalidateVisual();
                }
            }
        }

        public void OpenFile(XElement root)
        {   
            XElement data = root.Element("data");
            Clear();
            displayBoxes.Clear();
            currentCommand.Clear();
            foreach (XElement xe in data.Elements())
            {
                DisplayBoxType dbt = (DisplayBoxType)Enum.Parse(typeof(DisplayBoxType), xe.Attribute("type").Value);
                Point location;
                if (displayBoxes.Count > 0)
                {   
                    location = new Point(currentBox.Location.X, currentBox.Bottom);
                    this.MinHeight += currentBox.Height;
                }
                else
                {
                    location = caret.Location;
                }
                TextDisplayBox newBox = new TextDisplayBox(dbt, location);
                displayBoxes.Add(newBox);
                newBox.SetText(xe.Value);                
                currentBox = newBox;
            }
            if (currentBox.BoxType == DisplayBoxType.Input)
            {
                currentCommand.Append(currentBox.Text);
                caretIndex = currentCommand.Length;
            }
            commandCache.LoadXML(root);
            Calculator.LoadXML(root);
            AdjustCaret();
        }

        public void SaveXML(XElement root)
        {            
            XElement data = new XElement("data");
            foreach (var v in displayBoxes)
            {
                XElement element = new XElement("d", v.Text);
                element.Add(new XAttribute("type", v.BoxType.ToString()));
                data.Add(element);
            }
            root.Add(data);
            commandCache.SaveXML(root);
            Calculator.SaveXML(root);
        }

        public void Copy()
        {
            try
            {
                string text = "";
                Clipboard.Clear();
                selectedBoxesIndexes.Sort();
                if (selectedBoxesIndexes.Count > 1)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (int i in selectedBoxesIndexes)
                    {
                        sb.Append(displayBoxes[i].Text);
                        sb.Append(Environment.NewLine);
                    }
                    text = sb.ToString();
                }
                else if (selectedBoxesIndexes.Count == 1)
                {
                    text = displayBoxes[selectedBoxesIndexes[0]].Text;
                }
                if (text.Length > 0)
                {
                    Clipboard.SetText(text);
                }
                else
                {
                    MessageBox.Show("Nothing selected. You can select data by clicking on '>>' signs or the empty space on the left.", "Message");
                }
            }
            catch
            {
                MessageBox.Show("An error occured while trying to access the Windows Clipboad. Please try again.", "Error");
            }
        }

        public void Cut()
        {
            try
            {
                string text = "";
                Clipboard.Clear();
                selectedBoxesIndexes.Sort();
                if (selectedBoxesIndexes.Count > 1)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (int i in selectedBoxesIndexes)
                    {
                        sb.Append(displayBoxes[i].Text);
                        sb.Append(Environment.NewLine);
                    }
                    text = sb.ToString();
                }
                else if (selectedBoxesIndexes.Count == 1)
                {
                    text = displayBoxes[selectedBoxesIndexes[0]].Text;
                }
                if (selectedBoxesIndexes.Count > 0)
                {
                    Clipboard.SetText(text);
                    DeleteSelected();
                }
                else
                {
                    MessageBox.Show("Nothing selected. You can select data by clicking on '>>' signs or the empty space on the left.", "Message");
                }
            }
            catch
            {
                MessageBox.Show("An error occured while trying to access the Windows Clipboad. Please try again.", "Error");
            }
        }

        public void DeleteSelected()
        {
            if (selectedBoxesIndexes.Count > 0)
            {
                selectedBoxesIndexes.Sort();
                bool lastRemoved = selectedBoxesIndexes.Last() == displayBoxes.Count - 1;
                double x = currentBox.Location.X;
                double y = displayBoxes[selectedBoxesIndexes[0]].Top;
                for (int i = selectedBoxesIndexes.Count - 1; i >= 0; i--)
                {
                    displayBoxes.RemoveAt(selectedBoxesIndexes[i]);
                }
                this.MinHeight = (Parent as ScrollViewer).ViewportHeight;
                for (int i = selectedBoxesIndexes[0]; i < displayBoxes.Count; i++)
                {
                    displayBoxes[i].AdjustLocations(new Point(x, y));
                    y += displayBoxes[i].Height;
                    this.MinHeight += displayBoxes[i].Height;
                }
                if (lastRemoved)
                {
                    currentBox = new TextDisplayBox(DisplayBoxType.Input, new Point(x, y));
                    displayBoxes.Add(currentBox);
                    this.MinHeight += currentBox.Height;
                    caret.Location = currentBox.Location;
                    currentCommand.Clear();
                    caretIndex = 0;
                }
                selectedBoxesIndexes.Clear();
                AdjustCaret();
            }
        }

        public void Paste()
        {
            try
            {
                if (Clipboard.ContainsText())
                {
                    string text = Clipboard.GetText();
                    if (text.Contains(Environment.NewLine))
                        currentIsMultiLine = true;
                    ConsumeTextCommand(text);
                }
            }
            catch
            {
                MessageBox.Show("An error occured while trying to access the Windows Clipboad. Please try again.", "Error");
            }
        }

        public void Clear()
        {
            displayBoxes.Clear();
            currentBox = new TextDisplayBox(DisplayBoxType.Input, new Point(pointer.Width + gap + padding, padding));
            displayBoxes.Add(currentBox);
            caret.Location = currentBox.Location;
            this.MinHeight = 800;
            this.MinWidth = 1200;
            ScrollViewer scrollViewer = Parent as ScrollViewer;
            scrollViewer.ScrollToTop();
            scrollViewer.ScrollToLeftEnd();
            //InvalidateVisual();
            GC.Collect();
        }

        public void SelectAll()
        {
            if (displayBoxes.Count > 1 || currentCommand.Length > 0)
            {
                selectedBoxesIndexes.Clear();
                for (int i = 0; i < displayBoxes.Count; i++)
                {
                    displayBoxes[i].Selected = true;
                    selectedBoxesIndexes.Add(i);
                }
                if (currentCommand.Length == 0)
                {
                    selectedBoxesIndexes.RemoveAt(selectedBoxesIndexes.Count - 1);
                    displayBoxes.Last().Selected = false;
                }
                InvalidateVisual();
            }
        }
    }
}
