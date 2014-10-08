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
using MathProcessorDemo;
using System.Collections.ObjectModel;
using MathProcessorLib;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace MathProcessorDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<string> commandHistory = new ObservableCollection<string>();
        public MainWindow()
        {
            InitializeComponent();
            historyBox.ItemsSource = commandHistory;
            resultText.Inlines.Add(new Run(">> Welcome to Math Processor Demo. Click 'Execute' or simply press 'Enter' to execute your commands." + Environment.NewLine) { Foreground = Brushes.DarkGreen });
            Calculator.IntermediateResultProduced += new IntermediateResult(Calculator_IntermediatResultProduced);
            System.Windows.DataObject.AddPastingHandler(commandBox, new DataObjectPastingEventHandler(OnPaste));
            commandBox.Focus();
        }

        void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            System.Windows.DataObject d = new System.Windows.DataObject();
            d.SetData(System.Windows.DataFormats.Text, e.DataObject.GetData(typeof(string)).ToString().Replace(Environment.NewLine, " "));
            e.DataObject = d;
        }

        void Calculator_IntermediatResultProduced(Token result)
        {
            DisplayResult(result);
        }

        private void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            string command = commandBox.Text.Trim();
            if (command.Length > 0)
            {
                commandHistory.Insert(0, command);
                historySelectionInCode = true;
                historyBox.SelectedIndex = 0;
                if (commandHistory.Count > 20)
                {
                    commandHistory.RemoveAt(20);
                }
                historySelectionInCode = false;
                commandBox.SelectAll();

                resultText.Inlines.Add(new Run(">> " + command + Environment.NewLine) { Foreground = Brushes.Blue });
                Token result = Calculator.ProcessCommand(command);
                DisplayResult(result);
                mainScrollViewer.UpdateLayout();
                mainScrollViewer.ScrollToBottom();
            }
        }

        private void DisplayResult(Token result)
        {
            if (result.TokenType == TokenType.Error)
            {
                resultText.Inlines.Add(new Run(">> " + result.StrData + Environment.NewLine) { Foreground = Brushes.Red });
            }
            else if (result.TokenType == TokenType.Text || result.TokenType == TokenType.Vector || result.TokenType == TokenType.Bool)
            {
                string output = "";
                if (result.TokenType == TokenType.Vector)
                {
                    output = result.GetVectorString(Calculator.DefaultFormatString);
                }
                else
                {
                    output = result.GetString();
                }
                resultText.Inlines.Add(">> " + output + Environment.NewLine);
            }
            else if (result.TokenType == TokenType.Matrix)
            {
                PrintMatrix(result);
            }
        }

        void PrintMatrix(Token matrix)
        {
            int rows = (int)matrix.Extra;
            if (rows > 0)
            {
                List<double> data = matrix.VectorArray.ToList();
                Token temp;
                temp = new Token(TokenType.Vector, data.GetRange(0, matrix.Count / rows).ToArray());
                resultText.Inlines.Add(">> " + temp.GetVectorString(Calculator.DefaultFormatString, "   ") + Environment.NewLine);

                for (int i = matrix.Count / rows; i < matrix.Count; i += matrix.Count / rows)
                {
                    temp = new Token(TokenType.Vector, data.GetRange(i, matrix.Count / rows).ToArray());
                    resultText.Inlines.Add("   " + temp.GetVectorString(Calculator.DefaultFormatString, "   ") + Environment.NewLine);
                }
            }
        }

        bool historySelectionInCode = false;

        private void historyBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!historySelectionInCode)
            {
                commandBox.Text = historyBox.SelectedValue as string;
            }
        }

        private void KitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem item = sender as System.Windows.Controls.MenuItem;
            System.Windows.Forms.Form form = null;
            if (item != null)
            {
                switch (item.Header.ToString())
                {
                    case "_Basic Math":
                        form = new BasicKit(this);
                        break;
                    case "_Matrices":
                        form = new MatrixKit(this);
                        break;
                    case "_Truth Tables":
                        form = new BooleanKit(this);
                        break;
                }
                form.Show();
            }
        }

        private void DocumentationMenu_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://MathiVersity.com/MathProcessor/Documentation");
        }

        private void TutorialsMenu_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://MathiVersity.com/MathProcessor/Tutorials");
        }

        private void RunFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase), "Examples");
            ofd.Title = "Open comamnd file to execute";
            ofd.Filter = "File (.txt;*.*)|*.txt;*.*";
            ofd.InitialDirectory = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Examples");
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FileStream textFile = File.Open(ofd.FileName, FileMode.Open, FileAccess.Read);
                StreamReader reader = new StreamReader(textFile);
                String data = reader.ReadToEnd();
                reader.Close();
                Token result = Calculator.ProcessCommand(data);
                DisplayResult(result);
            }
        }
    }
}
