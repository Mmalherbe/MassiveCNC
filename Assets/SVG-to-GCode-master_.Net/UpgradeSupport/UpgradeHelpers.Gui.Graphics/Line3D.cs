using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace UpgradeHelpers.Gui
{
    /// <summary>
    /// Print a 3D line on the center of the longest part of the control
    /// </summary>
    public sealed class Line3D : Control
    {
        private Border3DStyle _lineBorderStyle;

        /// <summary>
        /// Creates a new Line3D
        /// </summary>
        public Line3D()
        {
            AutoSize = false;
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
        }

        /// <summary>
        /// The line style.
        /// </summary>
        [Description("The Style for the divider line."), Category("Appearance")]
        public Border3DStyle LineBorderStyle
        {
            get
            {
                if (_lineBorderStyle == 0)
                    _lineBorderStyle = Border3DStyle.Etched; // default style.
                return _lineBorderStyle;
            }
            set
            {
                if (value != _lineBorderStyle)
                {
                    _lineBorderStyle = value;
                    Invalidate(); // Mark that the control require redraw.
                }
            }
        }

        private bool DrawHorizontalLine
        {
            get
            {
                return Width >= Height;
            }
        }

        /// <summary>
        /// Paints the control on the screen
        /// </summary>
        /// <param name="e">The context to paint</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Brush b = new SolidBrush(ForeColor);

            Point startingPoint = new Point(0, 0);
            Point endPoint = new Point(0, 0);
            Border3DSide side = Border3DSide.Top;
            if (DrawHorizontalLine)
            {
                startingPoint.Y = Height / 2;
                endPoint.Y = startingPoint.Y;
                endPoint.X = Width - startingPoint.X;
            }
            else
            {
                startingPoint.X = Width / 2;
                endPoint.X = startingPoint.X;
                endPoint.Y = Height - startingPoint.Y;
                side = Border3DSide.Left;
            }

            ControlPaint.DrawBorder3D(g, startingPoint.X,
                  startingPoint.Y,
                  endPoint.X,
                  endPoint.Y, LineBorderStyle, side);
        }


        private void DrawLineAtGivenPoint(Graphics graphContext, Point startingPoint)
        {
            ControlPaint.DrawBorder3D(graphContext, startingPoint.X,
                              startingPoint.Y,
                              this.Width - startingPoint.X,
                              5, LineBorderStyle, Border3DSide.Top);
        }
    }
}
