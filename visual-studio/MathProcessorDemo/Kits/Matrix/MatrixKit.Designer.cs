namespace MathProcessorDemo
{
    partial class MatrixKit
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
            this.matrixTab = new System.Windows.Forms.TabControl();
            this.addMatrixButton = new System.Windows.Forms.Button();
            this.colsUpDown = new System.Windows.Forms.NumericUpDown();
            this.rowsUpDown = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.rankLabel = new System.Windows.Forms.Label();
            this.detLabel = new System.Windows.Forms.Label();
            this.isIdenLabel = new System.Windows.Forms.Label();
            this.isDiagLabel = new System.Windows.Forms.Label();
            this.orderLabel = new System.Windows.Forms.Label();
            this.refreshInfoButton = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.createNewCheckBox = new System.Windows.Forms.CheckBox();
            this.inverseButton = new System.Windows.Forms.Button();
            this.minorsButton = new System.Windows.Forms.Button();
            this.cofactsButton = new System.Windows.Forms.Button();
            this.refButton = new System.Windows.Forms.Button();
            this.rrefButton = new System.Windows.Forms.Button();
            this.transButton = new System.Windows.Forms.Button();
            this.newMatrixTextBox = new System.Windows.Forms.TextBox();
            this.newFromExprButton = new System.Windows.Forms.Button();
            this.randCheckBox = new System.Windows.Forms.CheckBox();
            this.removeButton = new System.Windows.Forms.Button();
            this.reloadButton = new System.Windows.Forms.Button();
            this.ReloadAllButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.colsUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rowsUpDown)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // matrixTab
            // 
            this.matrixTab.Location = new System.Drawing.Point(15, 39);
            this.matrixTab.Name = "matrixTab";
            this.matrixTab.SelectedIndex = 0;
            this.matrixTab.Size = new System.Drawing.Size(521, 35);
            this.matrixTab.TabIndex = 1;
            // 
            // addMatrixButton
            // 
            this.addMatrixButton.Location = new System.Drawing.Point(231, 8);
            this.addMatrixButton.Name = "addMatrixButton";
            this.addMatrixButton.Size = new System.Drawing.Size(55, 23);
            this.addMatrixButton.TabIndex = 2;
            this.addMatrixButton.Text = "Create";
            this.addMatrixButton.UseVisualStyleBackColor = true;
            this.addMatrixButton.Click += new System.EventHandler(this.addMatrixButton_Click);
            // 
            // colsUpDown
            // 
            this.colsUpDown.Location = new System.Drawing.Point(126, 9);
            this.colsUpDown.Maximum = new decimal(new int[] {
            655,
            0,
            0,
            0});
            this.colsUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.colsUpDown.Name = "colsUpDown";
            this.colsUpDown.Size = new System.Drawing.Size(43, 20);
            this.colsUpDown.TabIndex = 3;
            this.colsUpDown.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // rowsUpDown
            // 
            this.rowsUpDown.Location = new System.Drawing.Point(47, 9);
            this.rowsUpDown.Maximum = new decimal(new int[] {
            655,
            0,
            0,
            0});
            this.rowsUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.rowsUpDown.Name = "rowsUpDown";
            this.rowsUpDown.Size = new System.Drawing.Size(43, 20);
            this.rowsUpDown.TabIndex = 4;
            this.rowsUpDown.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Rows";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(95, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Cols";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.rankLabel);
            this.groupBox1.Controls.Add(this.detLabel);
            this.groupBox1.Controls.Add(this.isIdenLabel);
            this.groupBox1.Controls.Add(this.isDiagLabel);
            this.groupBox1.Controls.Add(this.orderLabel);
            this.groupBox1.Controls.Add(this.refreshInfoButton);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(548, 246);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 149);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Info";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(173, 100);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(0, 13);
            this.label4.TabIndex = 21;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(173, 81);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(0, 13);
            this.label9.TabIndex = 20;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(173, 61);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(0, 13);
            this.label10.TabIndex = 19;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(173, 41);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(0, 13);
            this.label11.TabIndex = 18;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(173, 21);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(0, 13);
            this.label12.TabIndex = 17;
            // 
            // rankLabel
            // 
            this.rankLabel.AutoSize = true;
            this.rankLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rankLabel.Location = new System.Drawing.Point(88, 100);
            this.rankLabel.Name = "rankLabel";
            this.rankLabel.Size = new System.Drawing.Size(10, 13);
            this.rankLabel.TabIndex = 16;
            this.rankLabel.Text = "-";
            // 
            // detLabel
            // 
            this.detLabel.AutoSize = true;
            this.detLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.detLabel.Location = new System.Drawing.Point(89, 81);
            this.detLabel.Name = "detLabel";
            this.detLabel.Size = new System.Drawing.Size(10, 13);
            this.detLabel.TabIndex = 15;
            this.detLabel.Text = "-";
            // 
            // isIdenLabel
            // 
            this.isIdenLabel.AutoSize = true;
            this.isIdenLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.isIdenLabel.Location = new System.Drawing.Point(89, 61);
            this.isIdenLabel.Name = "isIdenLabel";
            this.isIdenLabel.Size = new System.Drawing.Size(10, 13);
            this.isIdenLabel.TabIndex = 14;
            this.isIdenLabel.Text = "-";
            // 
            // isDiagLabel
            // 
            this.isDiagLabel.AutoSize = true;
            this.isDiagLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.isDiagLabel.Location = new System.Drawing.Point(89, 41);
            this.isDiagLabel.Name = "isDiagLabel";
            this.isDiagLabel.Size = new System.Drawing.Size(10, 13);
            this.isDiagLabel.TabIndex = 13;
            this.isDiagLabel.Text = "-";
            // 
            // orderLabel
            // 
            this.orderLabel.AutoSize = true;
            this.orderLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.orderLabel.Location = new System.Drawing.Point(89, 21);
            this.orderLabel.Name = "orderLabel";
            this.orderLabel.Size = new System.Drawing.Size(10, 13);
            this.orderLabel.TabIndex = 12;
            this.orderLabel.Text = "-";
            // 
            // refreshInfoButton
            // 
            this.refreshInfoButton.Location = new System.Drawing.Point(60, 121);
            this.refreshInfoButton.Name = "refreshInfoButton";
            this.refreshInfoButton.Size = new System.Drawing.Size(75, 23);
            this.refreshInfoButton.TabIndex = 6;
            this.refreshInfoButton.Text = "Refresh";
            this.refreshInfoButton.UseVisualStyleBackColor = true;
            this.refreshInfoButton.Click += new System.EventHandler(this.refreshInfoButton_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(5, 100);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(85, 13);
            this.label8.TabIndex = 5;
            this.label8.Text = "Rank           :";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(6, 81);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(83, 13);
            this.label7.TabIndex = 4;
            this.label7.Text = "Determinant :";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(6, 61);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Is Identity    :";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(6, 41);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(83, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Is Diagonal  :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(6, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Order          :";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.createNewCheckBox);
            this.groupBox2.Controls.Add(this.inverseButton);
            this.groupBox2.Controls.Add(this.minorsButton);
            this.groupBox2.Controls.Add(this.cofactsButton);
            this.groupBox2.Controls.Add(this.refButton);
            this.groupBox2.Controls.Add(this.rrefButton);
            this.groupBox2.Controls.Add(this.transButton);
            this.groupBox2.Location = new System.Drawing.Point(548, 95);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 139);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Actions";
            // 
            // createNewCheckBox
            // 
            this.createNewCheckBox.AutoSize = true;
            this.createNewCheckBox.Location = new System.Drawing.Point(60, 18);
            this.createNewCheckBox.Name = "createNewCheckBox";
            this.createNewCheckBox.Size = new System.Drawing.Size(82, 17);
            this.createNewCheckBox.TabIndex = 6;
            this.createNewCheckBox.Text = "Create New";
            this.createNewCheckBox.UseVisualStyleBackColor = true;
            // 
            // inverseButton
            // 
            this.inverseButton.Location = new System.Drawing.Point(106, 45);
            this.inverseButton.Name = "inverseButton";
            this.inverseButton.Size = new System.Drawing.Size(88, 23);
            this.inverseButton.TabIndex = 5;
            this.inverseButton.Text = "Inverse";
            this.inverseButton.UseVisualStyleBackColor = true;
            this.inverseButton.Click += new System.EventHandler(this.inverseButton_Click);
            // 
            // minorsButton
            // 
            this.minorsButton.Location = new System.Drawing.Point(12, 103);
            this.minorsButton.Name = "minorsButton";
            this.minorsButton.Size = new System.Drawing.Size(88, 23);
            this.minorsButton.TabIndex = 4;
            this.minorsButton.Text = "Minors Matrix";
            this.minorsButton.UseVisualStyleBackColor = true;
            this.minorsButton.Click += new System.EventHandler(this.minorsButton_Click);
            // 
            // cofactsButton
            // 
            this.cofactsButton.Location = new System.Drawing.Point(106, 103);
            this.cofactsButton.Name = "cofactsButton";
            this.cofactsButton.Size = new System.Drawing.Size(88, 23);
            this.cofactsButton.TabIndex = 3;
            this.cofactsButton.Text = "Cofacts Matrix";
            this.cofactsButton.UseVisualStyleBackColor = true;
            this.cofactsButton.Click += new System.EventHandler(this.cofactsButton_Click);
            // 
            // refButton
            // 
            this.refButton.Location = new System.Drawing.Point(12, 74);
            this.refButton.Name = "refButton";
            this.refButton.Size = new System.Drawing.Size(88, 23);
            this.refButton.TabIndex = 2;
            this.refButton.Text = "Row Ech.";
            this.refButton.UseVisualStyleBackColor = true;
            this.refButton.Click += new System.EventHandler(this.refButton_Click);
            // 
            // rrefButton
            // 
            this.rrefButton.Location = new System.Drawing.Point(106, 74);
            this.rrefButton.Name = "rrefButton";
            this.rrefButton.Size = new System.Drawing.Size(88, 23);
            this.rrefButton.TabIndex = 1;
            this.rrefButton.Text = "R. Row Ech.";
            this.rrefButton.UseVisualStyleBackColor = true;
            this.rrefButton.Click += new System.EventHandler(this.rrefButton_Click);
            // 
            // transButton
            // 
            this.transButton.Location = new System.Drawing.Point(12, 45);
            this.transButton.Name = "transButton";
            this.transButton.Size = new System.Drawing.Size(88, 23);
            this.transButton.TabIndex = 0;
            this.transButton.Text = "Transpose";
            this.transButton.UseVisualStyleBackColor = true;
            this.transButton.Click += new System.EventHandler(this.transButton_Click);
            // 
            // newMatrixTextBox
            // 
            this.newMatrixTextBox.Location = new System.Drawing.Point(290, 9);
            this.newMatrixTextBox.Name = "newMatrixTextBox";
            this.newMatrixTextBox.Size = new System.Drawing.Size(92, 20);
            this.newMatrixTextBox.TabIndex = 10;
            // 
            // newFromExprButton
            // 
            this.newFromExprButton.Location = new System.Drawing.Point(386, 8);
            this.newFromExprButton.Name = "newFromExprButton";
            this.newFromExprButton.Size = new System.Drawing.Size(118, 23);
            this.newFromExprButton.TabIndex = 11;
            this.newFromExprButton.Text = "New from Expression";
            this.newFromExprButton.UseVisualStyleBackColor = true;
            this.newFromExprButton.Click += new System.EventHandler(this.newFromExprButton_Click);
            // 
            // randCheckBox
            // 
            this.randCheckBox.AutoSize = true;
            this.randCheckBox.Location = new System.Drawing.Point(177, 11);
            this.randCheckBox.Name = "randCheckBox";
            this.randCheckBox.Size = new System.Drawing.Size(55, 17);
            this.randCheckBox.TabIndex = 12;
            this.randCheckBox.Text = "Rand.";
            this.randCheckBox.UseVisualStyleBackColor = true;
            // 
            // removeButton
            // 
            this.removeButton.Location = new System.Drawing.Point(510, 8);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(105, 23);
            this.removeButton.TabIndex = 13;
            this.removeButton.Text = "Remove Current";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // reloadButton
            // 
            this.reloadButton.Location = new System.Drawing.Point(449, 528);
            this.reloadButton.Name = "reloadButton";
            this.reloadButton.Size = new System.Drawing.Size(87, 23);
            this.reloadButton.TabIndex = 16;
            this.reloadButton.Text = "Reload Matrix";
            this.reloadButton.UseVisualStyleBackColor = true;
            this.reloadButton.Click += new System.EventHandler(this.reloadButton_Click);
            // 
            // ReloadAllButton
            // 
            this.ReloadAllButton.Location = new System.Drawing.Point(621, 9);
            this.ReloadAllButton.Name = "ReloadAllButton";
            this.ReloadAllButton.Size = new System.Drawing.Size(100, 23);
            this.ReloadAllButton.TabIndex = 17;
            this.ReloadAllButton.Text = "Reload All";
            this.ReloadAllButton.UseVisualStyleBackColor = true;
            this.ReloadAllButton.Click += new System.EventHandler(this.ReloadAllButton_Click);
            // 
            // MatrixKit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(757, 566);
            this.Controls.Add(this.ReloadAllButton);
            this.Controls.Add(this.reloadButton);
            this.Controls.Add(this.removeButton);
            this.Controls.Add(this.randCheckBox);
            this.Controls.Add(this.newFromExprButton);
            this.Controls.Add(this.newMatrixTextBox);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rowsUpDown);
            this.Controls.Add(this.colsUpDown);
            this.Controls.Add(this.addMatrixButton);
            this.Controls.Add(this.matrixTab);
            this.MaximumSize = new System.Drawing.Size(765, 600);
            this.Name = "MatrixKit";
            this.Text = "Math Processor Matrix Kit";
            ((System.ComponentModel.ISupportInitialize)(this.colsUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rowsUpDown)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl matrixTab;
        private System.Windows.Forms.Button addMatrixButton;
        private System.Windows.Forms.NumericUpDown colsUpDown;
        private System.Windows.Forms.NumericUpDown rowsUpDown;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button refreshInfoButton;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button transButton;
        private System.Windows.Forms.Button refButton;
        private System.Windows.Forms.Button rrefButton;
        private System.Windows.Forms.Button minorsButton;
        private System.Windows.Forms.Button cofactsButton;
        private System.Windows.Forms.CheckBox createNewCheckBox;
        private System.Windows.Forms.Button inverseButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label rankLabel;
        private System.Windows.Forms.Label detLabel;
        private System.Windows.Forms.Label isIdenLabel;
        private System.Windows.Forms.Label isDiagLabel;
        private System.Windows.Forms.Label orderLabel;
        private System.Windows.Forms.TextBox newMatrixTextBox;
        private System.Windows.Forms.Button newFromExprButton;
        private System.Windows.Forms.CheckBox randCheckBox;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.Button reloadButton;
        private System.Windows.Forms.Button ReloadAllButton;
    }
}