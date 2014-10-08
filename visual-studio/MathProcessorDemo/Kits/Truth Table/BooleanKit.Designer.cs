namespace MathProcessorDemo
{
    partial class BooleanKit
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BooleanKit));
            this.NumVars = new System.Windows.Forms.Label();
            this.setNumVarButton = new System.Windows.Forms.Button();
            this.numVarUpDown = new System.Windows.Forms.NumericUpDown();
            this.addColButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.expressionBox = new System.Windows.Forms.TextBox();
            this.createTableButton = new System.Windows.Forms.Button();
            this.exprHead = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.helpLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numVarUpDown)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // NumVars
            // 
            this.NumVars.AutoSize = true;
            this.NumVars.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NumVars.Location = new System.Drawing.Point(9, 39);
            this.NumVars.Name = "NumVars";
            this.NumVars.Size = new System.Drawing.Size(104, 16);
            this.NumVars.TabIndex = 0;
            this.NumVars.Text = "No. of Variables";
            // 
            // setNumVarButton
            // 
            this.setNumVarButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.setNumVarButton.Location = new System.Drawing.Point(191, 35);
            this.setNumVarButton.Name = "setNumVarButton";
            this.setNumVarButton.Size = new System.Drawing.Size(104, 23);
            this.setNumVarButton.TabIndex = 1;
            this.setNumVarButton.Text = "Set";
            this.setNumVarButton.UseVisualStyleBackColor = true;
            this.setNumVarButton.Click += new System.EventHandler(this.SetNumVar_Click);
            // 
            // numVarUpDown
            // 
            this.numVarUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numVarUpDown.Location = new System.Drawing.Point(119, 35);
            this.numVarUpDown.Maximum = new decimal(new int[] {
            14,
            0,
            0,
            0});
            this.numVarUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numVarUpDown.Name = "numVarUpDown";
            this.numVarUpDown.Size = new System.Drawing.Size(67, 22);
            this.numVarUpDown.TabIndex = 2;
            this.numVarUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // addColButton
            // 
            this.addColButton.Enabled = false;
            this.addColButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addColButton.Location = new System.Drawing.Point(85, 93);
            this.addColButton.Name = "addColButton";
            this.addColButton.Size = new System.Drawing.Size(128, 23);
            this.addColButton.TabIndex = 4;
            this.addColButton.Text = "Add Column";
            this.addColButton.UseVisualStyleBackColor = true;
            this.addColButton.Click += new System.EventHandler(this.AddColumn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(4, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 16);
            this.label1.TabIndex = 3;
            this.label1.Text = "Expression";
            // 
            // expressionBox
            // 
            this.expressionBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.expressionBox.Location = new System.Drawing.Point(85, 62);
            this.expressionBox.Name = "expressionBox";
            this.expressionBox.Size = new System.Drawing.Size(210, 22);
            this.expressionBox.TabIndex = 5;
            this.expressionBox.WordWrap = false;
            // 
            // createTableButton
            // 
            this.createTableButton.Enabled = false;
            this.createTableButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.createTableButton.Location = new System.Drawing.Point(59, 227);
            this.createTableButton.Name = "createTableButton";
            this.createTableButton.Size = new System.Drawing.Size(227, 23);
            this.createTableButton.TabIndex = 6;
            this.createTableButton.Text = "Create Truth Table";
            this.createTableButton.UseVisualStyleBackColor = true;
            this.createTableButton.Click += new System.EventHandler(this.CreateTable_Click);
            // 
            // exprHead
            // 
            this.exprHead.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exprHead.Location = new System.Drawing.Point(85, 31);
            this.exprHead.Name = "exprHead";
            this.exprHead.Size = new System.Drawing.Size(210, 22);
            this.exprHead.TabIndex = 7;
            this.exprHead.WordWrap = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(19, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 16);
            this.label2.TabIndex = 8;
            this.label2.Text = "Heading";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.addColButton);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.exprHead);
            this.groupBox1.Controls.Add(this.expressionBox);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(12, 82);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(316, 128);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Add more Columns to theTruth Table";
            // 
            // helpLabel
            // 
            this.helpLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpLabel.Location = new System.Drawing.Point(6, 282);
            this.helpLabel.Name = "helpLabel";
            this.helpLabel.Size = new System.Drawing.Size(377, 181);
            this.helpLabel.TabIndex = 10;
            this.helpLabel.Text = resources.GetString("helpLabel.Text");
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(6, 60);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(134, 16);
            this.label4.TabIndex = 12;
            this.label4.Text = "How to use the Kit:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(6, 259);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 16);
            this.label5.TabIndex = 13;
            this.label5.Text = "Example";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.Red;
            this.label7.Location = new System.Drawing.Point(12, 264);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(84, 18);
            this.label7.TabIndex = 15;
            this.label7.Text = "Important:";
            // 
            // label8
            // 
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.Black;
            this.label8.Location = new System.Drawing.Point(16, 291);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(331, 130);
            this.label8.TabIndex = 16;
            this.label8.Text = resources.GetString("label8.Text");
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(6, 28);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(357, 30);
            this.label6.TabIndex = 17;
            this.label6.Text = "This Kit can be used to export truth tables to html files";
            // 
            // label9
            // 
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(6, 83);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(394, 172);
            this.label9.TabIndex = 18;
            this.label9.Text = resources.GetString("label9.Text");
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.helpLabel);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(353, 29);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(406, 486);
            this.groupBox2.TabIndex = 19;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Help";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(16, 470);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(192, 13);
            this.linkLabel1.TabIndex = 20;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "http://mathiversity.com/MathProcessor";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel1_LinkClicked);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(16, 451);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(243, 16);
            this.label3.TabIndex = 21;
            this.label3.Text = "For more information and tutorials:";
            // 
            // BooleanKit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(774, 522);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.createTableButton);
            this.Controls.Add(this.numVarUpDown);
            this.Controls.Add(this.setNumVarButton);
            this.Controls.Add(this.NumVars);
            this.MaximumSize = new System.Drawing.Size(790, 560);
            this.Name = "BooleanKit";
            this.Text = "Math Processor Truth Table Kit";
            ((System.ComponentModel.ISupportInitialize)(this.numVarUpDown)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label NumVars;
        private System.Windows.Forms.Button setNumVarButton;
        private System.Windows.Forms.NumericUpDown numVarUpDown;
        private System.Windows.Forms.Button addColButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox expressionBox;
        private System.Windows.Forms.Button createTableButton;
        private System.Windows.Forms.TextBox exprHead;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label helpLabel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label label3;
    }
}