using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace UpgradeHelpers.Gui
{
    /// <summary>
    /// Control that displays a label followed by a line until the end of the control size.
    /// </summary>
    public class LineHeader : Label
    {
        private int _spaceBetweenTextAndLine;
        private Border3DStyle _lineBorderStyle;

        /// <summary>
        /// Creates a new LineHeader control.
        /// </summary>
        public LineHeader()
        {
            AutoSize = false;
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
        }

        /// <summary>
        /// Sealed property of AutoSize
        /// </summary>
        public override sealed bool AutoSize
        {
            get { return base.AutoSize; }
            set { base.AutoSize = value; }
        }

        /// <summary>
        /// Separation between the label and the header line.
        /// </summary>
        [Description("The separation between the text and the divider line."), Category("Appearance")]
        public int SpaceBetweenTextAndLine
        {
            get { return _spaceBetweenTextAndLine; }
            set
            {
                if (value != _spaceBetweenTextAndLine)
                {
                    _spaceBetweenTextAndLine = value;
                    Invalidate(); // Mark that the control require redraw.
                }
            }
        }

        /// <summary>
        /// Style of the header line.
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
        /// Paints the control on the screen
        /// </summary>
        /// <param name="e">The context to paint</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Font f = Font;
            Brush b = new SolidBrush(ForeColor);
            StringFormat sf = StringFormat.GenericTypographic;
            new RectangleF(0, 0, Width, Height);
            SizeF textSize = g.MeasureString(Text, f, Width);
            g.DrawString(Text, f, b, 0, 0, sf);
            if (textSize.Width + SpaceBetweenTextAndLine < Width)
            {
                Point startingPoint = new Point((int)textSize.Width + SpaceBetweenTextAndLine,
                                                (int)textSize.Height / 2);
                ControlPaint.DrawBorder3D(g, startingPoint.X,
                                          startingPoint.Y,
                                          Width - startingPoint.X,
                                          5, LineBorderStyle, Border3DSide.Top);
            }
        }
    }
}
