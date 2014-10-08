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


namespace MathProcessorLib
{
    public class GraphForm : Form
    {
        List<PointF[]> curveList = new List<PointF[]>();
        List<Pen> curvePen = new List<Pen>();
        List<double[]> xData = new List<double[]>();
        List<double[]> yData = new List<double[]>();
        List<Color> defaultColors = new List<Color>();

        float g_xMin = float.MaxValue;
        float g_xMax = float.MinValue;
        float g_yMin = float.MaxValue;
        float g_yMax = float.MinValue;

        Rectangle drawingRect;
        int nextColorIndex = 0;

        bool printCheck;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem exportImageToolStripMenuItem;
        private ToolStripMenuItem printToolStripMenuItem;

        String tempPath = Environment.GetEnvironmentVariable("APPDATA") + @"\MathProcessor\";

        public GraphForm()
        {
            InitializeComponent();
            Text = "Math Processor";
            ClientSize = new Size(550, 580);
            drawingRect = new Rectangle(ClientRectangle.Left + 8, ClientRectangle.Top + 30, ClientSize.Width - 16, ClientSize.Height - 38);
            BackColor = Color.White;
            this.MinimumSize = new Size(MinimumSize.Width, 90);
            defaultColors.AddRange(new Color[] { Color.DarkBlue, Color.DarkGreen, Color.Crimson, Color.Brown,
                                                 Color.Magenta,  Color.DarkOrange,
                                                 Color.DarkRed, Color.Gold, Color.Cyan});
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
            drawingRect.Size = new Size(ClientSize.Width - 16, ClientSize.Height - 38);
            ReadjustCruves();
            Invalidate();
        }

        void ReadjustCruves()
        {
            curveList.Clear();
            for (int i = 0; i < xData.Count; i++)
            {
                AddCurveToCurveList(i);
            }
        }

        public bool AddCurve(double[] xArr, double[] yArr, String colorStr)
        {
            Color color = Color.Black;
            try
            {
                color = Color.FromName(colorStr);
            }
            catch { }

            return AddCurve(xArr, yArr, color);
        }

        public bool AddCurve(double[] xArr, double[] yArr, Color curveColor)
        {
            if (xArr.Length < 1 || (xArr.Length != yArr.Length))
                return false;
            xData.Add(xArr);
            yData.Add(yArr);
            if (curveColor != Color.Transparent)
                curvePen.Add(new Pen(curveColor));
            else
            {
                curvePen.Add(new Pen(defaultColors[nextColorIndex++]));
                nextColorIndex %= defaultColors.Count;
            }
            AddCurveToCurveList(curveList.Count);
            return true;
        }

        public bool AddCurve(double[] xArr, double[] yArr)
        {
            return AddCurve(xArr, yArr, Color.Transparent);
        }

        void AddCurveToCurveList(int index)
        {
            int numPoints = xData[index].Length;
            PointF[] points = new PointF[numPoints];

            float xMin = float.MaxValue, xMax = float.MinValue;
            float yMin = float.MaxValue, yMax = float.MinValue;

            for (int i = 0; i < numPoints; i++)
            {
                if (xData[index][i] < xMin)
                    xMin = (float)xData[index][i];
                if (xData[index][i] > xMax)
                    xMax = (float)xData[index][i];
                if (yData[index][i] < yMin)
                    yMin = (float)yData[index][i];
                if (yData[index][i] > yMax)
                    yMax = (float)yData[index][i];
            }

            bool minMaxFault = false;
            if (xMin < g_xMin)
            {
                g_xMin = xMin;
                minMaxFault = true;
            }
            if (xMax > g_xMax)
            {
                g_xMax = xMax;
                minMaxFault = true;
            }
            if (yMin < g_yMin)
            {
                g_yMin = yMin;
                minMaxFault = true;
            }
            if (yMax > g_yMax)
            {
                g_yMax = yMax;
                minMaxFault = true;
            }
            if (minMaxFault)
            {
                curveList.Clear();
                for (int i = 0; i < xData.Count - 1; i++)
                {
                    AddCurveToCurveList(i);
                }
            }
            float widthFactor = drawingRect.Width / (g_xMax - g_xMin);
            float heightFactor = 0;
            if (g_yMax != g_yMin)
            {
                heightFactor = drawingRect.Height / (g_yMax - g_yMin);
            }

            for (int i = 0; i < numPoints; i++)
            {
                points[i].X = (float)(xData[index][i] - g_xMin) * widthFactor + drawingRect.Left;
                points[i].Y = (float)(g_yMax - yData[index][i]) * heightFactor + drawingRect.Top;
            }
            curveList.Add(points);
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
                foreach (PointF[] list in curveList)
                {
                    if (list.Length > 1)
                    {
                        g.DrawLines(curvePen[curveList.IndexOf(list)], list);
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("The given values are not all valid");
                this.Hide();
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
            sfd.Filter = "Image File(*.bmp;*.gif;*.png;*.jpeg;*.tiff)|*.gif;*.bmp;*.png;*.jpeg;*.tiff";
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