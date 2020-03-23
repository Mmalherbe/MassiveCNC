using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace UpgradeHelpers.Gui
{
    /// <summary>
    /// Control that shows a group of lines along the size of the control.
    /// </summary>
    public sealed class BarsNet : Control
    {
        private Border3DStyle _lineBorderStyle;
        private int _spaceBetweenLines;

        /// <summary>
        /// Creates a new Bars control.
        /// </summary>
        public BarsNet()
        {
            AutoSize = false;
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
        }

        /// <summary>
        /// The lines style.
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

        /// <summary>
        /// Vertical separation space between the lines.
        /// </summary>
        [Description("The Style for the divider line."), Category("Appearance")]
        public int SpaceBetweenLines
        {
            get
            {
                if (_spaceBetweenLines == 0)
                    _spaceBetweenLines = 4;
                return _spaceBetweenLines;
            }
            set
            {
                if (value != _spaceBetweenLines)
                {
                    _spaceBetweenLines = value;
                    Invalidate(); // Mark that the control require redraw.
                }
            }
        }

        /// <summary>
        /// Paints the control on the screen
        /// </summary>
        /// <param name="e">The context to paint</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            new SolidBrush(this.ForeColor);

            Point startingPoint = new Point(5, 0);
            DrawLineAtGivenPoint(g, startingPoint);

            int linesToBeDrawn = Height - 5 / SpaceBetweenLines;
            for (int i = 0; i < linesToBeDrawn; i++)
            {
                startingPoint.Y += SpaceBetweenLines;
                DrawLineAtGivenPoint(g, startingPoint);
            }
        }

        private void DrawLineAtGivenPoint(Graphics graphContext, Point startingPoint)
        {
            ControlPaint.DrawBorder3D(graphContext, startingPoint.X,
                              startingPoint.Y,
                              Width - startingPoint.X,
                              5, LineBorderStyle, Border3DSide.Top);
        }
    }
}
