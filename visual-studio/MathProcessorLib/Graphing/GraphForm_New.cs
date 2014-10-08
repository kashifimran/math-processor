/*
 * GraphForm.cs
 * Plots functions
 * Rev: 20110403
 */

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Drawing.Drawing2D;


namespace MathProcessorLib
{
    public class GraphForm_New : Form
    {
        List<PlotInfo> plots;
        float rotation = 0;

        bool printCheck;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem exportImageToolStripMenuItem;
        private ToolStripMenuItem printToolStripMenuItem;

        String tempPath = Environment.GetEnvironmentVariable("APPDATA") + @"\MathProcessor\";

        public GraphForm_New(List<PlotInfo> plots, float rotation, float scaleX, float scaleY)
        {
            this.rotation = rotation;
            InitializeComponent();
            Text = "Math Processor";
            ClientSize = new Size(550, 580);
            BackColor = Color.White;
            this.MinimumSize = new Size(MinimumSize.Width, 90);
            this.plots = plots;
            foreach (var plot in plots)
            {
                plot.Points = new PointF[plot.X.Length];
                for (var i = 0; i < plot.Points.Length; i++)
                {
                    var x = plot.X[i] * scaleX;
                    var y = plot.Y[i] * scaleY;
                    plot.Points[i] = new PointF(x, y);
                }
            }
        }

        private void PrintPage(object sender, PrintPageEventArgs e)
        {
            String tempImgPath = tempPath + "print.bmp";

            float x = e.MarginBounds.Left;
            float y = e.MarginBounds.Top;

            if (!Directory.Exists(tempPath))
                Directory.CreateDirectory(tempPath);

            // Revised from exportImage_Click
            Image bitmap = new Bitmap(ClientRectangle.Width, ClientRectangle.Height);
            Graphics g = Graphics.FromImage(bitmap);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.FillRectangle(Brushes.White, ClientRectangle);
            PaintCurves(g);
            try
            {
                bitmap.Save(tempImgPath);
                e.Graphics.DrawImage(Image.FromFile(tempImgPath), x, y);
            }
            catch (Exception exc)
            {
                MessageBox.Show("Unable to write temporary file to " + tempImgPath + "\n" +
                    exc);
                printCheck = false;
            }
        }


        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            PaintCurves(e.Graphics);
        }

        void PaintCurves(Graphics g)
        {
            try
            {
                g.TranslateTransform(ClientRectangle.Width / 2, ClientRectangle.Height / 2);
                g.RotateTransform(rotation);
                foreach (var pi in plots)
                {
                    var s = g.Save();
                    if (pi.XTranslate != 0 || pi.YTranslate != 0)
                    {
                        g.TranslateTransform(pi.XTranslate, pi.YTranslate);
                    }
                    g.RotateTransform(pi.Rotation);
                    if (pi.Color != Color.Transparent)
                    {
                        g.DrawLines(pi.Pen, pi.Points);
                    }
                    if (pi.Brush != null)
                    {
                        GraphicsPath graphPath = new GraphicsPath();
                        graphPath.AddLines(pi.Points);
                        //Brush b = new LinearGradientBrush(new Point(0, 0), new Point(100, 100), Color.Red, Color.Green);
                        //PathGradientBrush pthGrBrush = new PathGradientBrush(graphPath);

                        // Set the color at the center of the path to blue.
                        //pthGrBrush.CenterColor = Color.FromArgb(255, 0, 0, 255);

                        // Set the color along the entire boundary  
                        // of the path to aqua.
                        //Color[] colors = { Color.FromArgb(255, 0, 255, 255) };
                        //pthGrBrush.SurroundColors = colors;
                        g.FillPath(pi.Brush, graphPath);
                    }
                    g.Restore(s);
                }

            }
            catch (Exception)
            {
                MessageBox.Show("The given values are not all valid");
            }
        }

        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(542, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportImageToolStripMenuItem,
            this.printToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // exportImageToolStripMenuItem
            // 
            this.exportImageToolStripMenuItem.Name = "exportImageToolStripMenuItem";
            this.exportImageToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exportImageToolStripMenuItem.Text = "&Export Image";
            this.exportImageToolStripMenuItem.Click += new System.EventHandler(this.exportImageToolStripMenuItem_Click);
            // 
            // printToolStripMenuItem
            // 
            this.printToolStripMenuItem.Name = "printToolStripMenuItem";
            this.printToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.printToolStripMenuItem.Text = "&Print";
            this.printToolStripMenuItem.Click += new System.EventHandler(this.printToolStripMenuItem_Click);
            // 
            // GraphForm
            //             
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "GraphForm";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void exportImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Image File(*.bmp)|*.bmp";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Image bitmap = new Bitmap(ClientRectangle.Width, ClientRectangle.Height);
                Graphics g = Graphics.FromImage(bitmap);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.FillRectangle(Brushes.White, ClientRectangle);
                PaintCurves(g);
                bitmap.Save(sfd.FileName);
            }
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            printCheck = true;
            PrintDocument doc = new PrintDocument();
            doc.PrintPage += this.PrintPage;

            if (printCheck)
            {
                PrintDialog dlgSettings = new PrintDialog();
                dlgSettings.Document = doc;

                if (dlgSettings.ShowDialog() == DialogResult.OK)
                    doc.Print();
            }
        }
    }
}