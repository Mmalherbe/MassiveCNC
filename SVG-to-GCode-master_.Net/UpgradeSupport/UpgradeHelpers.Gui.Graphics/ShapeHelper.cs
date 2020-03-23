using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;

namespace UpgradeHelpers.Gui
{
    /// <summary>
    /// Helper to support VB6 Shape controls.
    /// </summary>
    public partial class ShapeHelper : UserControl, IDisposable
    {
        /// <summary>
        /// Constructor for the ShapeHelper class.
        /// </summary>
        public ShapeHelper()
        {
            InitializeComponent();

            SetStyle(ControlStyles.UserPaint, true);
            base.BackColor = Color.Transparent;

        }

        /// <summary>
        /// Enumeration for the shapes values.
        /// </summary>
        private enum ShapesEnum
        {
            Rectangle = 0,
            Square = 1,
            Oval = 2,
            Circle = 3,
            RoundRectangle = 4,
            RoundSquare = 5
        }

        /// <summary>
        /// BackStyle enumeration.
        /// </summary>
        private enum BackStyleEnum
        {
            Transparent = 0,
            Opaque = 1
        }

        /// <summary>
        /// Enumeration for FillStyle.
        /// </summary>
        private enum FillStyleEnum
        {
            Solid = 0,
            Transparent = 1,
            HorizontalLine = 2,
            VerticalLine = 3,
            DownwardDiagonal = 4,
            UpwardDiagonal = 5,
            Cross = 6,
            DiagonalCross = 7
        }


        /// <summary>
        /// Stores the BackColor property.
        /// </summary>
        private Color _backColor = SystemColors.Control;
        /// <summary>
        /// Brush used to paint the Shape control.
        /// </summary>
        private HatchBrush _shapeBrush =
            new HatchBrush(HatchStyle.Horizontal, Color.Transparent, Color.Transparent);

        /// <summary>
        /// Background Color to display text and graphics.
        /// </summary>
        [Description("Returns/sets the background color used to display text and graphics in an object."), Category("Appearance")]
        public new Color BackColor
        {
            get
            {
                return _backColor;
            }
            set
            {
                _backColor = value;
                SetBrush();
            }
        }

        /// <summary>
        /// Stores the BackStyle property.
        /// </summary>
        private BackStyleEnum _backStyle = BackStyleEnum.Transparent;
        /// <summary>
        /// Indicates whether a Label or the background of a Shape is transparent or opaque.
        /// </summary>
        [Description("Indicates whether a Label or the background of a Shape is transparent or opaque."), Category("Appearance")]
        public int BackStyle
        {
            get
            {
                return (int)_backStyle;
            }
            set
            {
                _backStyle = value == 0 ? BackStyleEnum.Transparent : BackStyleEnum.Opaque;
                SetBrush();
            }
        }

        /// <summary>
        /// Pen used to paint the Shape Control.
        /// </summary>
        private readonly Pen shapePen = new Pen(SystemColors.WindowText);
        /// <summary>
        /// Stores the BorderColor property.
        /// </summary>
        private Color _borderColor = SystemColors.WindowText;
        /// <summary>
        /// Color of the Shape border.
        /// </summary>
        [Description("Returns/sets the color of an object's border."), Category("Appearance")]
        public Color BorderColor
        {
            get
            {
                return _borderColor;
            }
            set
            {
                _borderColor = value;
                if (BorderStyle != 0)
                    shapePen.Color = value;

                Refresh();
            }
        }

        /// <summary>
        /// Stores the BorderStyle property.
        /// </summary>
        private int _borderStyle = 1;
        /// <summary>
        /// Border style of the Shape control.
        /// </summary>
        [Description("Returns/sets the border style for an object."), Category("Appearance")]
        public new int BorderStyle
        {
            get
            {
                return _borderStyle;
            }
            set
            {
                shapePen.DashOffset = 1500;
                switch (value)
                {
                    case 0:
                        _borderStyle = 0;
                        shapePen.Color = Color.Transparent;
                        break;
                    case 2:
                        _borderStyle = 2;
                        shapePen.Color = BorderColor;
                        shapePen.DashStyle = DashStyle.Dash;
                        break;
                    case 3:
                        _borderStyle = 3;
                        shapePen.Color = BorderColor;
                        shapePen.DashStyle = DashStyle.Dot;
                        break;
                    case 4:
                        _borderStyle = 4;
                        shapePen.Color = BorderColor;
                        shapePen.DashStyle = DashStyle.DashDot;
                        break;
                    case 5:
                        _borderStyle = 5;
                        shapePen.Color = BorderColor;
                        shapePen.DashStyle = DashStyle.DashDotDot;
                        break;
                    case 6:
                        _borderStyle = 6;
                        shapePen.Color = BorderColor;
                        shapePen.DashStyle = DashStyle.Solid;
                        break;
                    default:
                        _borderStyle = 1;
                        shapePen.Color = BorderColor;
                        shapePen.DashStyle = DashStyle.Solid;
                        break;
                }

                Refresh();
            }
        }

        /// <summary>
        /// Stores the BorderWidth property.
        /// </summary>
        private int _borderWidth = 1;
        /// <summary>
        /// Width of the Shape border.
        /// </summary>
        [Description("Returns or sets the width of a control's border."), Category("Appearance")]
        public int BorderWidth
        {
            get
            {
                return _borderWidth;
            }
            set
            {
                _borderWidth = value;
                shapePen.Width = _borderWidth;
                Refresh();
            }
        }

        /// <summary>
        /// Stores FillColor property.
        /// </summary>
        private Color _fillColor = Color.Black;
        /// <summary>
        /// Color to fill in Shape control.
        /// </summary>
        [Description("Returns/sets the color used to fill in shapes, circles, and boxes"), Category("Appearance")]
        public Color FillColor
        {
            get
            {
                return _fillColor;
            }
            set
            {
                _fillColor = value;
                SetBrush();
            }
        }

        /// <summary>
        /// Stores FillStyle property.
        /// </summary>
        private FillStyleEnum _fillStyle = FillStyleEnum.Solid;
        /// <summary>
        /// FillStyle in Shape control.
        /// </summary>
        [Description("Returns/sets the fill style of a shape"), Category("Appearance")]
        public int FillStyle
        {
            get
            {
                return (int)_fillStyle;
            }
            set
            {
                if (value >= 0 && value <= 7)
                {
                    _fillStyle = (FillStyleEnum)value;
                }
                else
                {
                    _fillStyle = FillStyleEnum.Transparent;
                }
                SetBrush();
            }
        }

        private void SetBrush()
        {
            Color foreColor = FillColor;
            Color backColor = BackColor;
            HatchStyle hatchStyle = HatchStyle.Horizontal;

            if (_backStyle == BackStyleEnum.Transparent)
            {
                backColor = Color.Transparent;
            }

            switch (_fillStyle)
            {
                case FillStyleEnum.Solid:
                    hatchStyle = HatchStyle.Horizontal;
                    backColor = FillColor;
                    break;
                case FillStyleEnum.HorizontalLine:
                    hatchStyle = HatchStyle.Horizontal;
                    break;
                case FillStyleEnum.VerticalLine:
                    hatchStyle = HatchStyle.Vertical;
                    break;
                case FillStyleEnum.DownwardDiagonal:
                    hatchStyle = HatchStyle.WideDownwardDiagonal;
                    break;
                case FillStyleEnum.UpwardDiagonal:
                    hatchStyle = HatchStyle.WideUpwardDiagonal;
                    break;
                case FillStyleEnum.Cross:
                    hatchStyle = HatchStyle.Cross;
                    break;
                case FillStyleEnum.DiagonalCross:
                    hatchStyle = HatchStyle.DiagonalCross;
                    break;
                default:
                    hatchStyle = HatchStyle.Horizontal;
                    foreColor = _backStyle == BackStyleEnum.Transparent ? foreColor = Color.Transparent : foreColor = BackColor;
                    break;
            }
            _shapeBrush = new HatchBrush(hatchStyle, foreColor, backColor);
            Refresh();
        }

        /// <summary>
        /// Stores the Shape property.
        /// </summary>
        private ShapesEnum _shape = ShapesEnum.Rectangle;
        /// <summary>
        /// The kind of Shape.
        /// </summary>
        [Description("Returns/sets a value indicating the appearance of a control"), Category("Appearance")]
        public int Shape
        {
            get
            {
                return (int)_shape;
            }
            set
            {
                switch (value)
                {
                    case 1:
                        _shape = ShapesEnum.Square;
                        break;
                    case 2:
                        _shape = ShapesEnum.Oval;
                        break;
                    case 3:
                        _shape = ShapesEnum.Circle;
                        break;
                    case 4:
                        _shape = ShapesEnum.RoundRectangle;
                        break;
                    case 5:
                        _shape = ShapesEnum.RoundSquare;
                        break;
                    default:
                        _shape = ShapesEnum.Rectangle;
                        break;
                }
                Refresh();
            }
        }

        /// <summary>
        /// Stores the RoundPercent property.
        /// </summary>
        private double _roundPercent = 0.15;
        /// <summary>
        /// Adds a property to specify the percent used to 
        /// round the corners in round rectangles and round squares.
        /// </summary>
        [Description("Allows to specify the percent used to round the corners of round rectangles and round squares")]
        public int RoundPercent
        {
            get { return (int)(_roundPercent * 100); }
            set
            {
                if ((value < 1) || (value > 50))
                    throw new InvalidConstraintException("Invalid property value");
                _roundPercent = (double)value / 100;
                Refresh();
            }
        }

        /// <summary>
        /// Manages the paint event of the Shape control.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The Paint event arguments.</param>
        private void ShapeHelper_Paint(object sender, PaintEventArgs e)
        {
            Rectangle clientRectangle = new Rectangle(1, 1, ClientRectangle.Width - 2, ClientRectangle.Height - 2);
            switch (Shape)
            {
                case (int)ShapesEnum.Rectangle:
                    DrawRectangle(clientRectangle, e.Graphics);
                    break;
                case (int)ShapesEnum.Square:
                    DrawSquare(clientRectangle, e.Graphics);
                    break;
                case (int)ShapesEnum.Oval:
                    DrawOval(clientRectangle, e.Graphics);
                    break;
                case (int)ShapesEnum.Circle:
                    DrawCircle(clientRectangle, e.Graphics);
                    break;
                case (int)ShapesEnum.RoundRectangle:
                    DrawRoundRectangle(clientRectangle, e.Graphics);
                    break;
                case (int)ShapesEnum.RoundSquare:
                    DrawRoundSquare(clientRectangle, e.Graphics);
                    break;
            }
        }

        /// <summary>
        /// Manages the Resize event to force the repaint.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void ShapeHelper_Resize(object sender, EventArgs e)
        {
            Refresh();
        }

        /// <summary>
        /// Draws a round square.
        /// </summary>
        /// <param name="clientRectangle">The client rectangle.</param>
        /// <param name="g">The graphics object to use.</param>
        private void DrawRoundSquare(Rectangle clientRectangle, Graphics g)
        {
            int maxDiameter = Math.Min(clientRectangle.Height, clientRectangle.Width);
            Rectangle newClientRectangle = new Rectangle(clientRectangle.Location.X + ((clientRectangle.Width - maxDiameter) / 2),
                clientRectangle.Location.Y + ((clientRectangle.Height - maxDiameter) / 2), maxDiameter, maxDiameter);

            DrawRoundRectangle(newClientRectangle, g);
        }

        /// <summary>
        /// Draws a round rectangle.
        /// </summary>
        /// <param name="clientRectangle">The region where to draw.</param>
        /// <param name="g">The GDI used to draw the rectangle.</param>
        private void DrawRoundRectangle(Rectangle clientRectangle, Graphics g)
        {
            double percentX = clientRectangle.Width * _roundPercent;
            double percentY = clientRectangle.Height * _roundPercent;
            double minPercent = Math.Min(percentX, percentY);
            double halfPercentX = percentX / 2;
            double halfPercentY = percentY / 2;
            double minHalfPercent = Math.Min(halfPercentX, halfPercentY);

            PointF pointUp1 = new PointF((float)(clientRectangle.X + minPercent), clientRectangle.Y);
            PointF pointUp2 = new PointF((float)(clientRectangle.X + clientRectangle.Width - minPercent), clientRectangle.Y);

            PointF pointDown1 = new PointF((float)(clientRectangle.X + clientRectangle.Width - minPercent), clientRectangle.Y + clientRectangle.Height);
            PointF pointDown2 = new PointF((float)(clientRectangle.X + minPercent), clientRectangle.Y + clientRectangle.Height);

            PointF pointLeft1 = new PointF(clientRectangle.X, (float)(clientRectangle.Y + clientRectangle.Height - minPercent));
            PointF pointLeft2 = new PointF(clientRectangle.X, (float)(clientRectangle.Y + minPercent));

            PointF pointRight1 = new PointF(clientRectangle.X + clientRectangle.Width, (float)(clientRectangle.Y + minPercent));
            PointF pointRight2 = new PointF(clientRectangle.X + clientRectangle.Width, (float)(clientRectangle.Y + clientRectangle.Height - minPercent));



            PointF pointCornerA1 = new PointF(clientRectangle.X, (float)(clientRectangle.Y + minHalfPercent));
            PointF pointCornerA2 = new PointF((float)(clientRectangle.X + minHalfPercent), clientRectangle.Y);

            PointF pointCornerB1 = new PointF((float)(clientRectangle.X + clientRectangle.Width - minHalfPercent), clientRectangle.Y);
            PointF pointCornerB2 = new PointF(clientRectangle.X + clientRectangle.Width, (float)(clientRectangle.Y + minHalfPercent));

            PointF pointCornerC1 = new PointF(clientRectangle.X + clientRectangle.Width, (float)(clientRectangle.Y + clientRectangle.Height - minHalfPercent));
            PointF pointCornerC2 = new PointF((float)(clientRectangle.X + clientRectangle.Width - minHalfPercent), clientRectangle.Y + clientRectangle.Height);

            PointF pointCornerD1 = new PointF((float)(clientRectangle.X + minHalfPercent), clientRectangle.Y + clientRectangle.Height);
            PointF pointCornerD2 = new PointF(clientRectangle.X, (float)(clientRectangle.Y + clientRectangle.Height - minHalfPercent));

            if ((_backStyle != BackStyleEnum.Transparent) || (_fillStyle != FillStyleEnum.Transparent))
            {
                using (GraphicsPath graphicsPath = new GraphicsPath())
                {
                    graphicsPath.AddLine(pointUp1, pointUp2);
                    graphicsPath.AddBezier(pointUp2, pointCornerB1, pointCornerB2, pointRight1);
                    graphicsPath.AddLine(pointRight1, pointRight2);
                    graphicsPath.AddBezier(pointRight2, pointCornerC1, pointCornerC2, pointDown1);
                    graphicsPath.AddLine(pointDown1, pointDown2);
                    graphicsPath.AddBezier(pointDown2, pointCornerD1, pointCornerD2, pointLeft1);
                    graphicsPath.AddLine(pointLeft1, pointLeft2);
                    graphicsPath.AddBezier(pointLeft2, pointCornerA1, pointCornerA2, pointUp1);
                    using (Region region = new Region(graphicsPath))
                    {
                        g.FillRegion(_shapeBrush, region);
                    }
                }
            }

            g.DrawLine(shapePen, pointUp1, pointUp2);
            g.DrawLine(shapePen, pointDown1, pointDown2);
            g.DrawLine(shapePen, pointLeft1, pointLeft2);
            g.DrawLine(shapePen, pointRight1, pointRight2);

            g.DrawBezier(shapePen, pointLeft2, pointCornerA1, pointCornerA2, pointUp1);
            g.DrawBezier(shapePen, pointUp2, pointCornerB1, pointCornerB2, pointRight1);
            g.DrawBezier(shapePen, pointRight2, pointCornerC1, pointCornerC2, pointDown1);
            g.DrawBezier(shapePen, pointDown2, pointCornerD1, pointCornerD2, pointLeft1);
        }

        /// <summary>
        /// Draws a circle.
        /// </summary>
        /// <param name="clientRectangle">The region where to draw.</param>
        /// <param name="g">The GDI used to draw the rectangle.</param>
        private void DrawCircle(Rectangle clientRectangle, Graphics g)
        {
            int maxDiameter = Math.Min(clientRectangle.Height, clientRectangle.Width);
            Rectangle newClientRectangle = new Rectangle(clientRectangle.Location.X + ((clientRectangle.Width - maxDiameter) / 2),
                clientRectangle.Location.Y + ((clientRectangle.Height - maxDiameter) / 2), maxDiameter, maxDiameter);

            if ((_backStyle != BackStyleEnum.Transparent) || (_fillStyle != FillStyleEnum.Transparent))
                g.FillEllipse(_shapeBrush, newClientRectangle);

            g.DrawEllipse(shapePen, newClientRectangle);
        }

        /// <summary>
        /// Draws an oval.
        /// </summary>
        /// <param name="clientRectangle">The region where to draw.</param>
        /// <param name="g">The GDI used to draw the rectangle.</param>
        private void DrawOval(Rectangle clientRectangle, Graphics g)
        {
            if ((_backStyle != BackStyleEnum.Transparent) || (_fillStyle != FillStyleEnum.Transparent))
                g.FillEllipse(_shapeBrush, clientRectangle);

            g.DrawEllipse(shapePen, clientRectangle);
        }

        /// <summary>
        /// Draws a square.
        /// </summary>
        /// <param name="clientRectangle">The region where to draw.</param>
        /// <param name="g">The GDI used to draw the rectangle.</param>
        private void DrawSquare(Rectangle clientRectangle, Graphics g)
        {
            int maxDiameter = Math.Min(clientRectangle.Height, clientRectangle.Width);
            Rectangle newClientRectangle = new Rectangle(clientRectangle.Location.X + ((clientRectangle.Width - maxDiameter) / 2),
                clientRectangle.Location.Y + ((clientRectangle.Height - maxDiameter) / 2), maxDiameter, maxDiameter);

            if ((_backStyle != BackStyleEnum.Transparent) || (_fillStyle != FillStyleEnum.Transparent))
                g.FillRectangle(_shapeBrush, newClientRectangle);

            g.DrawRectangle(shapePen, newClientRectangle);
        }

        /// <summary>
        /// Draws a rectangle.
        /// </summary>
        /// <param name="clientRectangle">The region where to draw.</param>
        /// <param name="g">The GDI used to draw the rectangle.</param>
        private void DrawRectangle(Rectangle clientRectangle, Graphics g)
        {
            if ((_backStyle != BackStyleEnum.Transparent) || (_fillStyle != FillStyleEnum.Transparent))
                g.FillRectangle(_shapeBrush, clientRectangle);

            g.DrawRectangle(shapePen, clientRectangle);
        }

        /// <summary>
        /// Overriding CreateParams method from UserControl.
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00000020; //WS_EX_TRANSPARENT 
                return cp;
            }
        }

        /// <summary>
        /// Overriding OnPaintBackground method from UserControl.
        /// </summary>
        /// <param name="pevent">The paint event arguments.</param>
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            //do not allow the background to be painted  
        }
    }
}
