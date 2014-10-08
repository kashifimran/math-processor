namespace MathProcessor
{
    partial class BasicKit
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
            this.operationCombo = new System.Windows.Forms.ComboBox();
            this.operationButton = new System.Windows.Forms.Button();
            this.inputView = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.varFlowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.inputNameButton = new System.Windows.Forms.Button();
            this.nameBox = new System.Windows.Forms.TextBox();
            this.outputNameButton = new System.Windows.Forms.Button();
            this.inLabel = new System.Windows.Forms.Label();
            this.outLabel = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.outName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.updateButton = new System.Windows.Forms.Button();
            this.outputView = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.inputView)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.outputView)).BeginInit();
            this.SuspendLayout();
            // 
            // operationCombo
            // 
            this.operationCombo.FormattingEnabled = true;
            this.operationCombo.Items.AddRange(new object[] {
            "abs",
            "acos",
            "asin",
            "atan",
            "avg",
            "ceil",
            "cos",
            "cosh",
            "count",
            "exp",
            "fact",
            "floor",
            "gcd",
            "lcm",
            "lg",
            "log",
            "max",
            "median",
            "min",
            "mode",
            "prod",
            "reverse",
            "round",
            "sin",
            "sinh",
            "sort",
            "sortd",
            "sqrt",
            "sum",
            "tan",
            "tanh",
            "truncate"});
            this.operationCombo.Location = new System.Drawing.Point(23, 32);
            this.operationCombo.MaxDropDownItems = 10;
            this.operationCombo.Name = "operationCombo";
            this.operationCombo.Size = new System.Drawing.Size(118, 21);
            this.operationCombo.Sorted = true;
            this.operationCombo.TabIndex = 1;
            this.operationCombo.Text = "abs";
            // 
            // operationButton
            // 
            this.operationButton.Location = new System.Drawing.Point(270, 31);
            this.operationButton.Name = "operationButton";
            this.operationButton.Size = new System.Drawing.Size(52, 23);
            this.operationButton.TabIndex = 2;
            this.operationButton.Text = "Go";
            this.operationButton.UseVisualStyleBackColor = true;
            this.operationButton.Click += new System.EventHandler(this.operationButton_Click);
            // 
            // inputView
            // 
            this.inputView.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.inputView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.inputView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1});
            this.inputView.Location = new System.Drawing.Point(12, 33);
            this.inputView.Name = "inputView";
            this.inputView.Size = new System.Drawing.Size(186, 471);
            this.inputView.TabIndex = 2;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Values";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.dataGridViewTextBoxColumn1.Width = 140;
            // 
            // varFlowPanel
            // 
            this.varFlowPanel.AutoScroll = true;
            this.varFlowPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.varFlowPanel.Location = new System.Drawing.Point(215, 226);
            this.varFlowPanel.Name = "varFlowPanel";
            this.varFlowPanel.Size = new System.Drawing.Size(349, 215);
            this.varFlowPanel.TabIndex = 5;
            // 
            // inputNameButton
            // 
            this.inputNameButton.Location = new System.Drawing.Point(148, 22);
            this.inputNameButton.Name = "inputNameButton";
            this.inputNameButton.Size = new System.Drawing.Size(83, 23);
            this.inputNameButton.TabIndex = 4;
            this.inputNameButton.Text = "Save Input";
            this.inputNameButton.UseVisualStyleBackColor = true;
            this.inputNameButton.Click += new System.EventHandler(this.inputNameButton_Click);
            // 
            // nameBox
            // 
            this.nameBox.Location = new System.Drawing.Point(24, 24);
            this.nameBox.Name = "nameBox";
            this.nameBox.Size = new System.Drawing.Size(118, 20);
            this.nameBox.TabIndex = 3;
            // 
            // outputNameButton
            // 
            this.outputNameButton.Location = new System.Drawing.Point(237, 22);
            this.outputNameButton.Name = "outputNameButton";
            this.outputNameButton.Size = new System.Drawing.Size(88, 23);
            this.outputNameButton.TabIndex = 11;
            this.outputNameButton.Text = "Save Output";
            this.outputNameButton.UseVisualStyleBackColor = true;
            this.outputNameButton.Click += new System.EventHandler(this.outputNameButton_Click);
            // 
            // inLabel
            // 
            this.inLabel.AutoSize = true;
            this.inLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.inLabel.Location = new System.Drawing.Point(11, 12);
            this.inLabel.Name = "inLabel";
            this.inLabel.Size = new System.Drawing.Size(44, 18);
            this.inLabel.TabIndex = 12;
            this.inLabel.Text = "Input";
            // 
            // outLabel
            // 
            this.outLabel.AutoSize = true;
            this.outLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.outLabel.Location = new System.Drawing.Point(578, 12);
            this.outLabel.Name = "outLabel";
            this.outLabel.Size = new System.Drawing.Size(58, 18);
            this.outLabel.TabIndex = 13;
            this.outLabel.Text = "Output";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.inputNameButton);
            this.groupBox1.Controls.Add(this.nameBox);
            this.groupBox1.Controls.Add(this.outputNameButton);
            this.groupBox1.Location = new System.Drawing.Point(215, 117);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(349, 58);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Save variable as";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.outName);
            this.groupBox2.Controls.Add(this.operationCombo);
            this.groupBox2.Controls.Add(this.operationButton);
            this.groupBox2.Location = new System.Drawing.Point(215, 34);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(349, 60);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Operation";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(145, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(117, 15);
            this.label3.TabIndex = 21;
            this.label3.Text = "Output name (if any)";
            // 
            // outName
            // 
            this.outName.Location = new System.Drawing.Point(147, 32);
            this.outName.Name = "outName";
            this.outName.Size = new System.Drawing.Size(117, 20);
            this.outName.TabIndex = 16;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(208, 206);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 16);
            this.label1.TabIndex = 16;
            this.label1.Text = "Variables";
            // 
            // updateButton
            // 
            this.updateButton.Location = new System.Drawing.Point(481, 201);
            this.updateButton.Name = "updateButton";
            this.updateButton.Size = new System.Drawing.Size(81, 23);
            this.updateButton.TabIndex = 12;
            this.updateButton.Text = "Refresh";
            this.updateButton.UseVisualStyleBackColor = true;
            this.updateButton.Click += new System.EventHandler(this.updateButton_Click);
            // 
            // outputView
            // 
            this.outputView.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.outputView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.outputView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn2});
            this.outputView.Location = new System.Drawing.Point(582, 33);
            this.outputView.Name = "outputView";
            this.outputView.Size = new System.Drawing.Size(186, 471);
            this.outputView.TabIndex = 17;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Values";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.dataGridViewTextBoxColumn2.Width = 140;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(213, 462);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(148, 16);
            this.label2.TabIndex = 18;
            this.label2.Text = "Execute Expression:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(277, 205);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(206, 16);
            this.label4.TabIndex = 20;
            this.label4.Text = " (click a variable to use it as input)";
            // 
            // BasicKit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(774, 522);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.outputView);
            this.Controls.Add(this.updateButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.outLabel);
            this.Controls.Add(this.inLabel);
            this.Controls.Add(this.inputView);
            this.Controls.Add(this.varFlowPanel);
            this.Controls.Add(this.label4);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(790, 560);
            this.Name = "BasicKit";
            this.Text = "Basic Math and Stats Kit";
            ((System.ComponentModel.ISupportInitialize)(this.inputView)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.outputView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox operationCombo;
        private System.Windows.Forms.Button operationButton;
        private System.Windows.Forms.FlowLayoutPanel varFlowPanel;
        private System.Windows.Forms.Button inputNameButton;
        private System.Windows.Forms.TextBox nameBox;
        private System.Windows.Forms.DataGridView inputView;
        private System.Windows.Forms.Button outputNameButton;
        private System.Windows.Forms.Label inLabel;
        private System.Windows.Forms.Label outLabel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox outName;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button updateButton;
        private System.Windows.Forms.DataGridView outputView;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;

    }
}