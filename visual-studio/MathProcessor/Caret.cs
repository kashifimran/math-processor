using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Threading;

namespace MathProcessor
{
    public class Caret : FrameworkElement
    {
        Point location;
        public double CaretLength { get; set; }
        bool isHorizontal = false;
        Pen pen = new Pen(Brushes.Black, 1);

        DispatcherTimer timer;

        public static readonly DependencyProperty VisibleProperty = DependencyProperty.Register("Visible", typeof(bool), typeof(Caret), new FrameworkPropertyMetadata(false /* defaultValue */, FrameworkPropertyMetadataOptions.AffectsRender));

        public Caret(bool isHorizontal)
        {
            this.isHorizontal = isHorizontal;
            CaretLength = 18;
            Visible = true;
            timer = new DispatcherTimer(TimeSpan.FromMilliseconds(600), DispatcherPriority.Normal, TimerElapsed, this.Dispatcher);
        }

        void TimerElapsed(object sender, EventArgs e)
        {
            Visible = !Visible;
        }

        public void StopBlinking()
        {
            timer.IsEnabled = false;
            Visible = false;
        }

        public void StartBlinking()
        {
            timer.IsEnabled = true;
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (Visible)
            {
                dc.DrawLine(pen, location, OtherPoint);
            }
            else if (isHorizontal)
            {
                dc.DrawLine(pen, location, OtherPoint);
            }
        }

        Point OtherPoint
        {
            get
            {
                if (isHorizontal)
                {
                    return new Point(Left + CaretLength, Top);
                }
                else
                {
                    return new Point(Left, VerticalCaretBottom);
                }
            }
        }

        public void ToggleVisibility()
        {
            Dispatcher.Invoke(new Action(() => { Visible = !Visible; }));
        }

        bool Visible
        {
            get
            {
                return (bool)GetValue(VisibleProperty);
            }
            set
            {
                SetValue(VisibleProperty, value);
            }
        }

        public Point Location
        {
            get { return location; }
            set
            {
                location.X = Math.Floor(value.X) + .5;
                location.Y = Math.Floor(value.Y) + .5;
                if (Visible)
                {
                    Visible = false;
                }
            }
        }

        public double Left
        {
            get { return location.X; }
            set
            {
                location.X = Math.Floor(value) + .5;
                if (Visible)
                {
                    Visible = false;
                }
            }
        }

        public double Top
        {
            get { return location.Y; }
            set
            {
                location.Y = Math.Floor(value) + .5;
                if (Visible)
                {
                    Visible = false;
                }
            }
        }

        public double VerticalCaretBottom
        {
            get { return location.Y + CaretLength; }
        }
    }
}
