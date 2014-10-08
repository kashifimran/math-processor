using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MathProcessorLib;
using System.IO;

namespace MathProcessor
{
    public partial class BooleanKit : KitsBase 
    {
        List<string> varNames = new List<string>();
        Variables vars = Variables.GetVariables();
        public BooleanKit(MainWindow parent)
            : base(parent, false)
        {
            InitializeComponent();
            this.linkLabel1.Links[0].LinkData = "http://mathiversity.com/MathProcessor";            
        }

        private void SetNumVar_Click(object sender, EventArgs e)
        {
            varNames.Clear();
            for (int i = 0; i < numVarUpDown.Value; i++)
            {
                varNames.Add(((char)(i + 0x61)).ToString());
            }
            List<Token> tokens = new List<Token>();
            foreach (string s in varNames)
            {
                tokens.Add(new Token(TokenType.Text, s));
            }
            Token result = Booleans.FillBool("fillbool", tokens);
            if (result.TokenType == TokenType.Error)
            {
                MessageBox.Show("Something's is wrong with your luck! Operation failed. Variables may be corrupt!");
                return;
            }
            this.createTableButton.Enabled = true;
            this.addColButton.Enabled = true;
        }

        private void AddColumn_Click(object sender, EventArgs e)
        {
            Token result;
            string expression = expressionBox.Text.Trim();
            string heading = exprHead.Text.Trim();
            if (expression.Length == 0)
            {
                MessageBox.Show("No expression to evaluate");
                return;
            }
            if (heading.Length == 0)
            {
                MessageBox.Show("Please fill in heading column to name the expression");
                return;
            }
            result = Calculator.ProcessCommand(heading + " = " + expression + ";");
            if (result.TokenType == TokenType.Error)
            {
                MessageBox.Show("Bad Expression");
                return;
            }
            if (result.TokenType != TokenType.Void)
            {
                MessageBox.Show("Somethings is wrong with either 'Heading' or 'Expression'!");
                return;
            }
            varNames.Add(heading);            
        }

        private void CreateTable_Click(object sender, EventArgs e)
        {
            StringBuilder html = new StringBuilder("<html><head><title>Exported truth table</title></head>");
            html.Append("<body><table border=1 width=95% align=center cellspacing=0><tr>");
            foreach (string s in varNames)
            {
                html.Append("<th>" + s + "</th>");
            }
            html.Append("</tr>");
            List<Token> tokens = new List<Token>();

            foreach (string s in varNames)
            {
                tokens.Add(vars.GetToken(s));
            }
            for (int i = 0; i < tokens[0].Count; i++)
            {
                html.Append("<tr>");
                foreach (Token t in tokens)
                {
                    html.Append("<td align=center>" + t[i].ToString() + "</td>");
                }
                html.Append("</tr>");
            }
            html.Append("</table></body></html>");
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "HTML File(*.html)|*.html";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                using (FileStream textFile = File.Open(sfd.FileName, FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(textFile))
                    {
                        sw.Write(html.ToString());
                    }
                }
            }            
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string target = (string)e.Link.LinkData;
            if (null != target)
            {
                System.Diagnostics.Process.Start(target);
            }
        }
    }
}
