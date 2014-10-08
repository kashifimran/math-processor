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
    public class KitsBase : Form
    {
        protected MainWindow parent = null;
        protected TextBox commandBox;
        protected Button goButton;

        protected void SetCommandBoxLocation(Point location)
        {
            commandBox.Location = location;
            goButton.Location = new Point(commandBox.Right + 1, commandBox.Top);
        }

        static KitsBase()
        {
            Application.EnableVisualStyles();
        }

        public KitsBase(MainWindow parent, bool createCommandBox)
        {
            if (createCommandBox)
            {
                goButton = new Button();
                goButton.Size = new Size(48, 23);
                goButton.Text = "Go";
                goButton.Click += new EventHandler(goButton_Click);

                commandBox = new TextBox();
                commandBox.Font = new Font("Microsoft Sans Serif", 12);
                commandBox.Size = new Size(380, 26);
                commandBox.Location = new Point(3, 40);
                goButton.Location = new Point(commandBox.Right + 1, commandBox.Top);
                this.Controls.Add(goButton);
                this.Controls.Add(commandBox);
            }
            this.parent = parent;            
            MaximizeBox = false;    
        }
       
        void goButton_Click(object sender, EventArgs e)
        {
            string expr = commandBox.Text.Trim() + ";";
            try
            {
                Token token = Calculator.ProcessCommand(expr);
                if (token.TokenType == TokenType.Error)
                {
                    MessageBox.Show(token.StrData, "Bad Input");
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message, "Error");
            }
        }
    }
}