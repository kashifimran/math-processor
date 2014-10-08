using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MathProcessorLib;

namespace MathProcessor
{
    public partial class BasicKit : KitsBase
    {
        Variables vars = Variables.GetVariables();

        public BasicKit(MainWindow parent)
            : base(parent, true)
        {
            InitializeComponent();
            commandBox.Size = new Size(300, 24);
            SetCommandBoxLocation(new Point(215, 480));
            PopulateVariables();
            inputView.CellValidating += new DataGridViewCellValidatingEventHandler(inputView_CellValidating);
        }
        
        void inputView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            inputView.Rows[e.RowIndex].ErrorText = "";
            double value;
            if (inputView.Rows[e.RowIndex].IsNewRow)
            {
                return; //Nothing to validate...
            }
            if (e.FormattedValue.ToString().Length > 0)
            {
                if (!double.TryParse(e.FormattedValue.ToString(), out value))
                {
                    e.Cancel = true;
                    inputView.Rows[e.RowIndex].ErrorText = "Value not numeric!";
                }
            }
        }

        void PopulateVariables()
        {
            varFlowPanel.Controls.Clear();
            List<string> varNames = vars.GetVariableNames();
            varNames.Remove("ans");
            varNames.Remove("maxtime");
            foreach (string s in varNames)
            {
                if (vars.GetTokenType(s) == TokenType.Vector)
                {
                    AddButtonToVarFlowPanel(s);
                }
            }
        }

        void AddButtonToVarFlowPanel(string str)
        {
            Button button = new Button();
            button.FlatStyle = FlatStyle.Flat;
            button.Text = str;
            button.Size = new Size(20, 15);
            button.AutoSize = true;            
            varFlowPanel.Controls.Add(button);
            button.Click += new EventHandler(button_Click);
            button.BackColor = Color.WhiteSmoke;
        }

        void button_Click(object sender, EventArgs dataGridView)
        {
            if (vars.Contains(((Button)sender).Text))
            {
                Token token = vars.GetToken(((Button)sender).Text);
                inputView.RowCount = token.Count + 1;
                for (int i = 0; i < token.Count; i++)
                {
                    inputView.Rows[i].Cells[0].Value = token[i];
                }
            }
            else
            {
                MessageBox.Show("Variable does not exist any longer.");
                varFlowPanel.Controls.Remove((Button)sender);

            }
        }


        private void operationButton_Click(object sender, EventArgs e)
        {
            if (inputView.Rows.Count < 2)
                return;
            outputView.RowCount = 1;
            outputView.Rows[0].Cells[0].Value = "Error";
            double[] data = new double[inputView.RowCount - 1];
            for (int i = 0; i < data.Count(); i++)
            {
                data[i] = Double.Parse(inputView.Rows[i].Cells[0].Value.ToString());
            }
            List<Token> temp = new List<Token>();
            temp.Add(new Token(TokenType.Vector, data));
            Token result = Function.InvokeFunction(operationCombo.Text, temp);
            if (result.TokenType == TokenType.Error)
            {
                MessageBox.Show(result.StrData);
                return;
            }
            outputView.RowCount = result.Count;
            for (int i = 0; i < result.Count; i++)
            {
                outputView.Rows[i].Cells[0].Value = result[i].ToString("G");
            }
            if (outName.Text.Length > 0)
            {
                if (!(Char.IsLetter(outName.Text[0]) || outName.Text[0] == '_'))
                {
                    MessageBox.Show("Invalid name specified for result. Variable not stored.", "Error");
                    return;
                }
                AddTokenToVarFlowPanel(result, outName.Text);
                outName.Text = "";
            }
        }

        void AddTokenToVarFlowPanel(Token token, string name)
        {
            Token result = vars.IsValidVarName(name);
            if (result.TokenType == TokenType.Error)
            {
                MessageBox.Show(result.StrData, "Error");
                return;
            }
            token.TokenName = name;            
            if (vars.Contains(name))
            {
                result = vars.SetTokenVerify(token);
            }
            else
            {
                result = vars.AddTokenVerify(token);
            }
            if (result.TokenType == TokenType.Error)
            {
                MessageBox.Show(result.StrData);
                return;
            }
            bool contains = false;
            foreach (Control c in varFlowPanel.Controls)
            {
                if (c.Text == name)
                {
                    contains = true;
                    break;
                }
            }
            if (!contains)
            {
                AddButtonToVarFlowPanel(name);
            }
        }

        private void inputNameButton_Click(object sender, EventArgs e)
        {
            if (nameBox.Text.Length > 0)
            {
                if (!(Char.IsLetter(nameBox.Text[0]) || nameBox.Text[0] == '_'))
                {
                    MessageBox.Show("Invalid name. Variable not stored.", "Error");
                    return;
                }
                double[] data = new double[inputView.RowCount - 1];
                for (int i = 0; i < data.Count(); i++)
                {
                    data[i] = Double.Parse(inputView.Rows[i].Cells[0].Value.ToString());
                }
                Token temp = new Token(TokenType.Vector, data);
                AddTokenToVarFlowPanel(temp, nameBox.Text);
            }            
        }

        private void outputNameButton_Click(object sender, EventArgs e)
        {
            if (nameBox.Text.Length > 0)
            {
                if (!(Char.IsLetter(nameBox.Text[0]) || nameBox.Text[0] == '_'))
                {
                    MessageBox.Show("Invalid name. Variable not stored.", "Error");
                    return;
                }
                double[] data = new double[outputView.RowCount];
                try
                {
                    for (int i = 0; i < data.Count(); i++)
                    {
                        data[i] = Double.Parse(outputView.Rows[i].Cells[0].Value.ToString());
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Output could not be converted into a vector. Data not numeric", "Error");
                    return;
                }
                Token temp = new Token(TokenType.Vector, data);
                AddTokenToVarFlowPanel(temp, nameBox.Text);
            }
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            PopulateVariables();
        }
    }
}
