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
    public partial class MatrixKit : KitsBase
    {
        Variables vars = Variables.GetVariables();
        List<DataGridView> matrixGrids = new List<DataGridView>();
        List<Token> matrixTokens = new List<Token>();
        int currentMatrixIndex = 0;
        static int nextIndex = 1;        

        public MatrixKit(MainWindow parent)
            : base (parent, true)
        {
            InitializeComponent();
            matrixTab.Selected += new TabControlEventHandler(matrixTab_Selected);            
            EnableControls(false);
            commandBox.Location = new Point(15, 525);
            goButton.Location = new Point(398, 528);
        }

        void EnableControls(bool enable)
        {
            refreshInfoButton.Enabled = enable;
            transButton.Enabled = enable;
            inverseButton.Enabled = enable;
            refButton.Enabled = enable;
            rrefButton.Enabled = enable;
            minorsButton.Enabled = enable;
            cofactsButton.Enabled = enable;
            removeButton.Enabled = enable;
            reloadButton.Enabled = enable;
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            matrixTab.Selected -= matrixTab_Selected;
            DataGridViewEventRegistrar(matrixGrids[currentMatrixIndex], false);
            this.Controls.Remove(matrixGrids[currentMatrixIndex]);
            matrixGrids.RemoveAt(currentMatrixIndex);
            matrixTokens.RemoveAt(currentMatrixIndex);
            matrixTab.TabPages.RemoveAt(currentMatrixIndex);            
            if (matrixTokens.Count == 0)
            {
                EnableControls(false);
                currentMatrixIndex = 0;                
            }
            else
            {
                if (currentMatrixIndex > 0)
                    currentMatrixIndex--;
                matrixTab.SelectedIndex = currentMatrixIndex;
                resetInfo();
                matrixGrids[currentMatrixIndex].Show();
            }
            matrixTab.Selected += matrixTab_Selected;            
        }

        void matrixTab_Selected(object sender, TabControlEventArgs e)
        {
            resetInfo();
            matrixGrids[currentMatrixIndex].Hide();
            matrixGrids[matrixTab.SelectedIndex].Show();
            currentMatrixIndex = matrixTab.SelectedIndex;
        }
                
        private void newFromExprButton_Click(object sender, EventArgs e)
        {
            string expr = newMatrixTextBox.Text.Trim() + ";";
            if (expr.Length == 1)
            {
                MessageBox.Show("Expression box is empty!", "No expression");
                return;
            }

            Token token = Calculator.ProcessCommand(expr);
            if (token.TokenType == TokenType.Matrix)
            {                
                addNewMatrix(token);
                if (matrixGrids.Count == 1)
                {
                    EnableControls(true);
                }
            }
            else
            {
                MessageBox.Show("The expression did not return a matrix", "Error");
            }
        }
               
        private void addMatrixButton_Click(object sender, EventArgs e)
        {
            Token token = new Token(TokenType.Matrix, (double)rowsUpDown.Value, new double[(int)rowsUpDown.Value * (int)colsUpDown.Value]);
            if (randCheckBox.Checked)
            {
                Random rand = new Random();
                for (int i = 0; i < token.Count; i++)
                {
                    token[i] = rand.NextDouble();
                }
            }
            addNewMatrix(token);
            if (matrixGrids.Count == 1)
            {
                EnableControls(true);
            }
        }

        void addNewMatrix(Token token)
        {
            string tokenName = "m" + nextIndex;
            while (vars.Contains(tokenName))
            {
                nextIndex++;
                tokenName = "m" + nextIndex;
            }
            DataGridView dataGridView = new DataGridView();
            dataGridView.BackgroundColor = Color.White;            
            dataGridView.AllowUserToAddRows = false;
            matrixGrids.Add(dataGridView);
            matrixTab.TabPages.Add(tokenName);
            token.TokenName = tokenName;
            vars.AddToken(token);
            matrixTokens.Add(token);
            dataGridView.Location = new Point(15, 75);
            dataGridView.Size = new Size(520, 435);
            SetupDataGridView(dataGridView, token);
            this.Controls.Add(dataGridView);
            matrixTab.SelectedIndex = matrixGrids.Count - 1;
            nextIndex++;
            DataGridViewEventRegistrar(dataGridView, true);
        }

        void UpdateDataGridView(DataGridView dataGridView, Token matrix)
        {            
            int rows = (int)matrix.Extra;
            int cols = matrix.Count / rows;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    dataGridView.Rows[i].Cells[j].Value = matrix[i * cols + j];
                }
            }
        }

        void SetupDataGridView(DataGridView dataGridView, Token matrix)
        {
            dataGridView.RowCount = 0;
            dataGridView.ColumnCount = 0;
            int rows = (int)matrix.Extra;
            int cols = matrix.Count / rows;

            for (int i = 0; i < cols; i++)
            {
                dataGridView.Columns.Add("C" + i + 1, (i + 1).ToString());
                dataGridView.Columns[i].SortMode = DataGridViewColumnSortMode.Programmatic;
                dataGridView.Columns[i].Width = 60;                
            }
            dataGridView.Rows.Add(rows);
            UpdateDataGridView(dataGridView, matrix);
        }

        void DataGridViewEventRegistrar(DataGridView dataGridView, bool register)
        {
            if (register)
            {
                dataGridView.MouseDown += new MouseEventHandler(dataGridView_MouseDown);
                dataGridView.CellValidating += new DataGridViewCellValidatingEventHandler(dataGridView_CellValidating);
                dataGridView.RowPostPaint += new DataGridViewRowPostPaintEventHandler(dataGridView_RowPostPaint);
                dataGridView.CellValueChanged += new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
            }
            else
            {
                dataGridView.MouseDown -= new MouseEventHandler(dataGridView_MouseDown);
                dataGridView.CellValidating -= new DataGridViewCellValidatingEventHandler(dataGridView_CellValidating);
                dataGridView.RowPostPaint -= new DataGridViewRowPostPaintEventHandler(dataGridView_RowPostPaint);
                dataGridView.CellValueChanged -= new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
            }
        }

        void dataGridView_MouseDown(object sender, MouseEventArgs e)
        {
            DataGridView.HitTestInfo hti = matrixGrids[currentMatrixIndex].HitTest(e.X, e.Y);

            if (hti.ColumnIndex == -1 && hti.RowIndex >= 0)
            {
                // row header click
                if (matrixGrids[currentMatrixIndex].SelectionMode != DataGridViewSelectionMode.RowHeaderSelect)
                {
                    matrixGrids[currentMatrixIndex].SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;
                }
            }
            else if (hti.RowIndex == -1 && hti.ColumnIndex >= 0)
            {
                // column header click
                if (matrixGrids[currentMatrixIndex].SelectionMode != DataGridViewSelectionMode.ColumnHeaderSelect)
                {
                    matrixGrids[currentMatrixIndex].SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect;
                }
            }
        }

        void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            Token current = matrixTokens[currentMatrixIndex];
            DataGridView dataGridView = (DataGridView)sender; 
            if (dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null)
                dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "0";
            current[e.RowIndex * current.Count / (int)current.Extra + e.ColumnIndex] = double.Parse(dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());  
        }

        void dataGridView_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            string strRowNumber = (e.RowIndex + 1).ToString();
            while (strRowNumber.Length < dgv.RowCount.ToString().Length) 
                strRowNumber = "0" + strRowNumber;
            SizeF size = e.Graphics.MeasureString(strRowNumber, this.Font);
            if (dgv.RowHeadersWidth < (int)(size.Width + 20)) dgv.RowHeadersWidth = (int)(size.Width + 20);
            Brush b = SystemBrushes.ControlText;
            e.Graphics.DrawString(strRowNumber, this.Font, b, e.RowBounds.Location.X + 15, e.RowBounds.Location.Y + ((e.RowBounds.Height - size.Height) / 2));            
        }

        void dataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            matrixGrids[matrixTab.SelectedIndex].Rows[e.RowIndex].ErrorText = "";
            double value;
            if (matrixGrids[matrixTab.SelectedIndex].Rows[e.RowIndex].IsNewRow) 
            { 
                return; //Nothing to validate...
            }
            if (e.FormattedValue.ToString().Length > 0)
            {
                if (!double.TryParse(e.FormattedValue.ToString(), out value))
                {
                    e.Cancel = true;
                    matrixGrids[matrixTab.SelectedIndex].Rows[e.RowIndex].ErrorText = "Value not numeric!";
                }
            }
        }
                
        private void refreshInfoButton_Click(object sender, EventArgs e)
        {
            updateInfo();
        }

        void resetInfo()
        {
            orderLabel.Text = "-";
            detLabel.Text = "-";
            rankLabel.Text = "-";
            isIdenLabel.Text = "-";
            isDiagLabel.Text = "-";
        }

        void updateInfo()
        {
            Token matrix = matrixTokens[currentMatrixIndex];
            List<Token> args = new List<Token>();
            int rows = (int)matrix.Extra;
            int cols = matrix.Count / rows;
            orderLabel.Text = rows + " x " + cols;
            args.Add(matrix);
            Token result = Matrix.Determinant("det", args);
            if (result.TokenType == TokenType.Error)
            {
                detLabel.Text = "Not defined";
            }
            else
            {
                detLabel.Text = result.FirstValue.ToString("G10");
            }
            result = Matrix.Rank("rank", args);
            
            if (result.TokenType == TokenType.Error)
            {
                rankLabel.Text = "Not defined";
            }
            else
            {
                rankLabel.Text = result.FirstValue.ToString();
            }

            result = Matrix.IsDiagonal("isdiag", args);

            if (result.TokenType == TokenType.Error)
            {
                isDiagLabel.Text = "Not defined";
            }
            else
            {
                if (result.FirstValue ==0)
                    isDiagLabel.Text = "No";
                else 
                    isDiagLabel.Text = "Yes";                 
            }

            result = Matrix.IsIdentity("isiden", args);

            if (result.TokenType == TokenType.Error)
            {
                isIdenLabel.Text = "Not defined";
            }
            else
            {
                if (result.FirstValue == 0)
                    isIdenLabel.Text = "No";
                else
                    isIdenLabel.Text = "Yes";
            }
        }

        private void inverseButton_Click(object sender, EventArgs e)
        {
            if (matrixTokens[currentMatrixIndex].Count / matrixTokens[currentMatrixIndex].Extra != matrixTokens[currentMatrixIndex].Extra)
            {
                MessageBox.Show("Matrix is not square");
                return;
            }            
            List<Token> args = new List<Token>();
            args.Add(matrixTokens[currentMatrixIndex]);
            Token result = Matrix.GetInverse("inverse", args);

            if (result.TokenType == TokenType.Error)
            {
                MessageBox.Show("Matrix is not invertible!");
                return;
            }
            if (createNewCheckBox.Checked)
            {
                addNewMatrix(result);
            }
            else
            {
                changeMatrixView(result);
            }              
        }

        private void refButton_Click(object sender, EventArgs e)
        {
            if (createNewCheckBox.Checked)
            {
                Token result = matrixTokens[currentMatrixIndex].Clone();
                Matrix.RowReduce(result);
                addNewMatrix(result);
            }
            else
            {
                Matrix.RowReduce(matrixTokens[currentMatrixIndex]);
            }
            DataGridView dataGridView = matrixGrids[currentMatrixIndex];
            UpdateDataGridView(dataGridView, matrixTokens[currentMatrixIndex]);    
        }

        private void rrefButton_Click(object sender, EventArgs e)
        {   
            List<Token> args = new List<Token>();
            args.Add(matrixTokens[currentMatrixIndex]);
            Token result = Matrix.Rref("rref", args);
            if (result.TokenType == TokenType.Error)
            {
                MessageBox.Show("An error occured. Aborted");
                return;
            }
            if (createNewCheckBox.Checked)
            {
                addNewMatrix(result);
            }
            else
            {
                changeMatrixView(result);
            }          
        }

        private void minorsButton_Click(object sender, EventArgs e)
        {
            if (matrixTokens[currentMatrixIndex].Count / matrixTokens[currentMatrixIndex].Extra != matrixTokens[currentMatrixIndex].Extra)
            {
                MessageBox.Show("Matrix is not square");
                return;
            }
            string name = matrixTokens[currentMatrixIndex].TokenName;
            List<Token> args = new List<Token>();
            args.Add(matrixTokens[currentMatrixIndex]);
            Token result = Matrix.GetMinorMatrix("minormatrix", args);
            if (createNewCheckBox.Checked)
            {
                addNewMatrix(result);
            }
            else
            {
                changeMatrixView(result);
            }
        }

        private void cofactsButton_Click(object sender, EventArgs e)
        {
            if (matrixTokens[currentMatrixIndex].Count / matrixTokens[currentMatrixIndex].Extra != matrixTokens[currentMatrixIndex].Extra)
            {
                MessageBox.Show("Matrix is not square");
                return;
            }
            
            List<Token> args = new List<Token>();
            args.Add(matrixTokens[currentMatrixIndex]);
            Token result = Matrix.GetCofactorMatrix("comatrix", args);
            if (createNewCheckBox.Checked)
            {
                addNewMatrix(result);
            }
            else
            {
                changeMatrixView(result);
            }
        }

        private void transButton_Click(object sender, EventArgs e)
        {
            List<Token> args = new List<Token>();
            DataGridView dataGridView = matrixGrids[currentMatrixIndex];
            args.Add(matrixTokens[currentMatrixIndex]);
            Token result = Matrix.Transpose("trans", args);
            if (createNewCheckBox.Checked)
            {
                addNewMatrix(result);
            }
            else
            {
                string name = matrixTokens[currentMatrixIndex].TokenName;
                matrixTokens[currentMatrixIndex] = result;
                matrixTokens[currentMatrixIndex].TokenName = name;
                vars.SetToken(matrixTokens[currentMatrixIndex]);
                if (matrixTokens[currentMatrixIndex].Extra != (matrixTokens[currentMatrixIndex].Count / matrixTokens[currentMatrixIndex].Extra))
                {
                    SetupDataGridView(dataGridView, matrixTokens[currentMatrixIndex]);
                }
                else
                {
                    UpdateDataGridView(dataGridView, matrixTokens[currentMatrixIndex]);
                }
            }
        }

        void changeMatrixView(Token token)
        {
            string name = matrixTokens[currentMatrixIndex].TokenName;
            matrixTokens[currentMatrixIndex] = token;
            DataGridView dataGridView = matrixGrids[currentMatrixIndex];
            matrixTokens[currentMatrixIndex].TokenName = name;
            vars.SetToken(matrixTokens[currentMatrixIndex]);
            UpdateDataGridView(dataGridView, matrixTokens[currentMatrixIndex]); 
        }
       
        private void reloadButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (vars.GetTokenType(matrixTokens[currentMatrixIndex].TokenName)!= TokenType.Matrix)
                {
                    MessageBox.Show("Type of the matrix has been changed externally. Could not re-load");
                    return;
                }
                matrixGrids[currentMatrixIndex].SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;
                matrixTokens[currentMatrixIndex] = vars.GetToken(matrixTokens[currentMatrixIndex].TokenName);                
                SetupDataGridView(matrixGrids[currentMatrixIndex], matrixTokens[currentMatrixIndex]);
            }
            catch (Exception exp)
            {
                MessageBox.Show("Could not re-load\r\n" + exp.Message);
            }
        }

        private void ReloadAllButton_Click(object sender, EventArgs e)
        {
            int currentIndex = currentMatrixIndex;
            for (int i = 0; i < matrixTokens.Count; i++)
            {
                currentMatrixIndex = i;
                try
                {
                    if (vars.GetTokenType(matrixTokens[i].TokenName) != TokenType.Matrix)
                    {
                        MessageBox.Show("Type of the matrix No: " + i+1 + "has been changed externally. Could not re-load");
                        continue;
                    }
                    matrixGrids[i].SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;
                    matrixTokens[i].VectorArray = vars.GetVector(matrixTokens[i].TokenName);
                    matrixTokens[i].Extra = vars.GetExtra(matrixTokens[i].TokenName);
                    SetupDataGridView(matrixGrids[i], matrixTokens[i]);
                }
                catch (Exception exp)
                {
                    MessageBox.Show("Could not re-load\r\n" + exp.Message);
                }
            }
            currentMatrixIndex = currentIndex;
         }  
    }
}
