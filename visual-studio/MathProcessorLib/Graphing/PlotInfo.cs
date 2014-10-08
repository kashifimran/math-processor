using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace MathProcessorLib
{
    public class PlotInfo
    {
        public float[] X { get; set; }
        public float[] Y { get; set; }
        public Color Color { get; set; }
        public Brush Brush { get; set; }
        public Pen Pen { get; set; }
        public float Rotation { get; set; }
        public float Thickness { get; set; }
        public float XTranslate { get; set; }
        public float YTranslate { get; set; }
        public PointF[] Points { get; set; }        
    }
}
