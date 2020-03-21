using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace UpgradeHelpers.Gui
{
    /// <summary>
    /// Extender that adds support to drawing features in PictureBox
    /// </summary>
    public class PictureBoxExtended : PictureBox, System.ComponentModel.ISupportInitialize
    {
        private Bitmap bitmap;

        private Graphics objGraphics;

        private bool firstResize = true;

        private const float TwipsPerInch = 1440f;

        private float m_dpiX;

        private float m_dpiY;

        private short m_scaleMode;

        private float m_originX;

        private float m_originY;

        private float m_positionX;

        private float m_positionY;

        private float m_userWidth;

        private float m_userHeight;

        private float m_drawWidth = 1;

        private DashStyle m_drawStyle = DashStyle.Solid;

        private Color m_fillColor;

        private FillStyleConstants m_fillStyle;

        private Rectangle m_marginBounds;

        /// <summary>
        /// Get or set Width property in HiMetric units
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new float Width
        {
            get
            {
                return this.ScaleX(base.Width, 3, 8);
            }
            set
            {
                base.Width = Convert.ToInt32(this.ScaleX(value, 8, 3));
            }
        }

        /// <summary>
        /// Get or set Height property in HiMetric units
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new float Height
        {
            get
            {
                return this.ScaleX(base.Height, 3, 8);
            }
            set
            {
                base.Height = Convert.ToInt32(this.ScaleX(value, 8, 3));
            }
        }

        /// <summary>
        /// Get or set Left property in twips units
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new float Left
        {
            get
            {
                return this.ScaleX(base.Left, 3, 1);
            }
            set
            {
                base.Left = Convert.ToInt32(this.ScaleX(value, 1, 3));
            }
        }

        /// <summary>
        /// Get or set Top property in twips units
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new float Top
        {
            get
            {
                return this.ScaleX(base.Top, 3, 1);
            }
            set
            {
                base.Top = Convert.ToInt32(this.ScaleX(value, 1, 3));
            }
        }


        /// <summary>
        /// Get or Set BackColor property
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
                if (objGraphics != null)
                    objGraphics.Clear(this.BackColor);
            }
        }

        /// <summary>
        /// Initialize all the necessary object for drawing features
        /// </summary>
        public void Initialize()
        {
            //if we already have an image then use it for all the drawing
            if (base.Image != null && base.Image is Bitmap)
            {
                if (objGraphics != null) objGraphics.Dispose();
                if (bitmap != null) bitmap.Dispose();
                bitmap = base.Image as Bitmap;
                if (bitmap.PixelFormat == PixelFormat.Format1bppIndexed || bitmap.PixelFormat == PixelFormat.Format4bppIndexed ||
                    bitmap.PixelFormat == PixelFormat.Format8bppIndexed || bitmap.PixelFormat == PixelFormat.Undefined ||
                    bitmap.PixelFormat == PixelFormat.DontCare || bitmap.PixelFormat == PixelFormat.Format16bppArgb1555 ||
                    bitmap.PixelFormat == PixelFormat.Format16bppGrayScale)
                {
                    bitmap = new Bitmap(bitmap.Width, bitmap.Height);
                    Rectangle rect = new Rectangle(0, 0, base.Image.Width, base.Image.Height);
                    objGraphics = Graphics.FromImage(bitmap);
                    objGraphics.DrawImage(bitmap, rect, 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel);
                    base.Image = bitmap;
                }
                else
                {
                    objGraphics = Graphics.FromImage(bitmap);
                }
            }
            else
            {
                if (objGraphics != null) objGraphics.Dispose();
                if (bitmap != null) bitmap.Dispose();
                bitmap = new Bitmap(this.ClientRectangle.Width, this.ClientRectangle.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                base.Image = this.bitmap;
                objGraphics = Graphics.FromImage(bitmap);
                objGraphics.Clear(this.BackColor);
            }

            this.CoordinateSpace(this.ClientRectangle, objGraphics.DpiX, objGraphics.DpiY);
            this.Disposed += PictureBoxExtended_Disposed;
        }

        /// <summary>
        /// Disposed Graphics object
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PictureBoxExtended_Disposed(object sender, EventArgs e)
        {
            if (objGraphics != null)
            {
                objGraphics.Dispose();
            }
        }

        /// <summary>
        /// Get or Set CurrentX property
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public float CurrentX
        {
            get
            {
                return this.FromPixelsX(this.m_positionX);
            }
            set
            {
                this.m_positionX = this.ToPixelsX(value);
                ControlHelper.setCurrentX(this, this.m_positionX);
            }
        }

        /// <summary>
        /// Get or Set CurrentY property
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public float CurrentY
        {
            get
            {
                return this.FromPixelsY(this.m_positionY);
            }
            set
            {
                this.m_positionY = this.ToPixelsY(value);

                ControlHelper.setCurrentY(this, this.m_positionY);
            }
        }

        /// <summary>
        /// Get or Set SclaeHeight property
        /// </summary>
        [DefaultValue(0f)]
        public float ScaleHeight
        {
            get
            {
                if (this.m_scaleMode == 0)
                {
                    return this.m_userHeight;
                }
                return (float)this.m_marginBounds.Height * this.ZoomY;
            }
            set
            {
                this.CheckOverflow(value);
                if (this.NearZero(value))
                {
                    //Information.Err().Raise(380, null, null, null, null);
                }
                this.ScaleMode = 0;
                this.m_userHeight = value;
            }
        }

        /// <summary>
        /// Get or Set ScaleLeft property
        /// </summary>
        [DefaultValue(0f)]
        public float ScaleLeft
        {
            get
            {
                return this.m_originX;
            }
            set
            {
                this.CheckOverflow(value);
                this.ScaleMode = 0;
                this.m_originX = value;
            }
        }

        /// <summary>
        /// Get or Set ScaleMode property
        /// </summary>
        public short ScaleMode
        {
            get
            {
                return this.m_scaleMode;
            } 
            set
            {
                if (value < 0 || value > 7)
                {
                    //Information.Err().Raise(380, null, null, null, null);
                }
                if (value != 0)
                {
                    this.m_originX = 0f;
                    this.m_originY = 0f;
                    this.m_userWidth = 0f;
                    this.m_userHeight = 0f;
                }
                else if (value == 0 && this.m_scaleMode != 0)
                {
                    this.m_originX = 0f;
                    this.m_originY = 0f;
                    this.m_userWidth = this.ScaleWidth;
                    this.m_userHeight = this.ScaleHeight;
                }
                this.m_scaleMode = value;
            }
        }

        /// <summary>
        /// Get or Set ScaleTop property
        /// </summary>
        [DefaultValue(0f)]
        public float ScaleTop
        {
            get
            {
                return this.m_originY;
            }
            set
            {
                this.CheckOverflow(value);
                this.ScaleMode = 0;
                this.m_originY = value;
            }
        }

        /// <summary>
        /// Get or Set ScaleWidth property
        /// </summary>
        [DefaultValue(0f)]
        public float ScaleWidth
        {
            get
            {
                if (this.m_scaleMode == 0)
                {
                    return this.m_userWidth;
                }
                return (float)this.m_marginBounds.Width * this.ZoomX;
            }
            set
            {
                this.CheckOverflow(value);
                if (this.NearZero(value))
                {
                    //Information.Err().Raise(380, null, null, null, null);
                }
                this.ScaleMode = 0;
                this.m_userWidth = value;
            }
        }

        /// <summary>
        /// Gets TwipsPerPixelX acording actual DPiX value
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public float TwipsPerPixelX
        {
            get
            {
                return 1440f / this.m_dpiX;
            }
        }

        /// <summary>
        /// Gets TwipsPerPixelY acording actual DPiY value
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public float TwipsPerPixelY
        {
            get
            {
                return 1440f / this.m_dpiY;
            }
        }

        /// <summary>
        /// Gets ZoomX value acording actual scaleMode value
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        private float ZoomX
        {
            get
            {
                return this.get_ZoomX(this.m_scaleMode);
            }
        }

        /// <summary>
        /// Gets actual ZoomX
        /// </summary>
        /// <param name="mode">ScaleMode value</param>
        /// <returns>ZoomX value for mode</returns>
        private float get_ZoomX(short mode)
        {
            switch (mode)
            {
                case 0:
                    return this.m_userWidth / (float)this.m_marginBounds.Width;
                case 1:
                    return 15f;
                case 2:
                    return 0.72f;
                case 3:
                    return 0.01f * this.m_dpiX;
                case 4:
                    return 0.12f;
                case 5:
                    return 0.01f;
                case 6:
                    return 0.254f;
                case 7:
                    return 0.0254f;
                case 8:
                    return 25.4f;
                default:
                    return 0f;
            }
        }

        /// <summary>
        /// Gets ZoomY value acording actual scaleMode value
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        private float ZoomY
        {
            get
            {
                return this.get_ZoomY(this.m_scaleMode);
            }
        }

        /// <summary>
        /// Gets actual ZoomY
        /// </summary>
        /// <param name="mode">ScaleMode value</param>
        /// <returns>ZoomY value for mode</returns>
        private float get_ZoomY(short mode)
        {
            switch (mode)
            {
                case 0:
                    return this.m_userHeight / (float)this.m_marginBounds.Height;
                case 1:
                    return 15.0f;
                case 2:
                    return 0.72f;
                case 3:
                    return 0.01f * this.m_dpiY;
                case 4:
                    return 0.06f;
                case 5:
                    return 0.01f;
                case 6:
                    return 0.254f;
                case 7:
                    return 0.0254f;
                case 8:
                    return 25.4f;
                default:
                    return 0f;
            }
        }

        /// <summary>
        /// Get or Set DrawWidth value
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public float DrawWidth
        {
            get
            {
                return m_drawWidth;
            }
            set
            {
                m_drawWidth = value;
            }
        }

        /// <summary>
        /// Get or Set DrawStyle value
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DashStyle DrawStyle
        {
            get
            {
                return m_drawStyle;
            }
            set
            {
                m_drawStyle = value;
            }
        }

        /// <summary>
        /// Get or Set FillColor value
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color FillColor
        {
            get
            {
                return m_fillColor;
            }
            set
            {
                m_fillColor = value;
            }
        }

        /// <summary>
        /// Get or Set FillStyle value
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FillStyleConstants FillStyle
        {
            get
            {
                return m_fillStyle;
            }
            set
            {
                m_fillStyle = value;
            }
        }

        /// <summary>
        /// Set main properties for the instances in order to manage Scale features.
        /// </summary>
        /// <param name="marginBounds">Bounds of PictureBox margins</param>
        /// <param name="dpiX">Horizontal resolution of this Graphics</param>
        /// <param name="dpiY">Vertical resolution of this Graphics</param>
        public void CoordinateSpace(Rectangle marginBounds, float dpiX, float dpiY)
        {
            this.SetMarginBounds(marginBounds);
            this.SetDpi(dpiX, dpiY);
            this.m_scaleMode = 1;
            this.m_positionX = 0f;
            this.m_positionY = 0f;
            this.m_originX = 0f;
            this.m_originY = 0f;
            this.m_userWidth = 0f;
            this.m_userHeight = 0f;
        }

        /// <summary>
        /// Reset ScaleMode property
        /// </summary>
        public void Scale()
        {
            this.ScaleMode = 1;
        }

        /// <summary>
        /// Set dimensions properties of the PictureBox
        /// </summary>
        /// <param name="x1">X1 value</param>
        /// <param name="y1">Y1 value</param>
        /// <param name="x2">X2 value</param>
        /// <param name="y2">Y2 value</param>
        public void Scale(float x1, float y1, float x2, float y2)
        {
            this.CheckOverflow(x1);
            this.CheckOverflow(y1);
            this.CheckOverflow(x2);
            this.CheckOverflow(y2);
            if (this.NearZero(x2 - x1))
            {
                Microsoft.VisualBasic.Information.Err().Raise(380, null, null, null, null);
            }
            if (this.NearZero(y2 - y1))
            {
                Microsoft.VisualBasic.Information.Err().Raise(380, null, null, null, null);
            }
            this.m_originX = x1;
            this.m_originY = y1;
            this.m_userWidth = x2 - x1;
            this.m_userHeight = y2 - y1;
            this.m_scaleMode = 0;
        }

        /// <summary>
        /// Convert X value from HiMetrics value to actual ScaleMode value
        /// </summary>
        /// <param name="value">value to convert</param>
        /// <returns>Converted value in actual ScaleMode value</returns>
        public float ScaleX(float value)
        {
            return ScaleX(value, 8, this.ScaleMode);
        }

        /// <summary>
        /// Convert X value fromScale Scale to toScale Scale
        /// </summary>
        /// <param name="value">value to convert</param>
        /// <param name="fromScale">From Scale</param>
        /// <param name="toScale">To Scale</param>
        /// <returns>Converted value</returns>
        public float ScaleX(float value, short fromScale, short toScale)
        {
            if (toScale == -1)
            {
                toScale = this.ScaleMode;
            }
            if (fromScale < 0 || fromScale > 8)
            {
                Microsoft.VisualBasic.Information.Err().Raise(380, null, null, null, null);
            }
            if (toScale < 0 || toScale > 8)
            {
                Microsoft.VisualBasic.Information.Err().Raise(380, null, null, null, null);
            }
            if ((fromScale == 0 || toScale == 0) && this.ScaleMode != 0)
            {
                Microsoft.VisualBasic.Information.Err().Raise(5, null, null, null, null);
            }
            this.CheckOverflow(value);
            return value * this.get_ZoomX(toScale) / this.get_ZoomX(fromScale);
        }

        /// <summary>
        /// Convert Y value from HiMetrics value to actual ScaleMode value
        /// </summary>
        /// <param name="value">value to convert</param>
        /// <returns>Converted value in actual ScaleMode value</returns>
        public float ScaleY(float value)
        {
            return ScaleY(value, 8, this.ScaleMode);
        }

        /// <summary>
        /// Convert Y value fromScale Scale to toScale Scale
        /// </summary>
        /// <param name="value">value to convert</param>
        /// <param name="fromScale">From Scale</param>
        /// <param name="toScale">To Scale</param>
        /// <returns>Converted value</returns>
        public float ScaleY(float value, short fromScale, short toScale)
        {
            if (toScale == -1)
            {
                toScale = this.ScaleMode;
            }
            if (fromScale < 0 || fromScale > 8)
            {
                Microsoft.VisualBasic.Information.Err().Raise(380, null, null, null, null);
            }
            if (toScale < 0 || toScale > 8)
            {
                Microsoft.VisualBasic.Information.Err().Raise(380, null, null, null, null);
            }
            if ((fromScale == 0 || toScale == 0) && this.ScaleMode != 0)
            {
                Microsoft.VisualBasic.Information.Err().Raise(5, null, null, null, null);
            }
            this.CheckOverflow(value);
            return value * this.get_ZoomY(toScale) / this.get_ZoomY(fromScale);
        }

        /// <summary>
        /// Convert heigth value from Pixels to actual ScaleMode
        /// </summary>
        /// <param name="height">Height value</param>
        /// <returns>Height value in actual ScaleMode</returns>
        public float FromPixelsHeight(float height)
        {
            return height * this.ZoomY / (this.m_dpiY / 100f);
        }

        /// <summary>
        /// Convert width value from Pixels to actual ScaleMode
        /// </summary>
        /// <param name="width">Width value</param>
        /// <returns>Width value in actual ScaleMode</returns>
        public float FromPixelsWidth(float width)
        {
            return width * this.ZoomX / (this.m_dpiX / 100f);
        }

        /// <summary>
        /// Convert x value from Pixels to actual ScaleMode
        /// </summary>
        /// <param name="x">X value</param>
        /// <returns>X value in actual ScaleMode</returns>
        public float FromPixelsX(float x)
        {
            return this.m_originX + x * this.ZoomX / (this.m_dpiX / 100f);
        }

        ///// <summary>
        /// Convert y value from Pixels to actual ScaleMode
        /// </summary>
        /// <param name="y">Y value</param>
        /// <returns>Y value in actual ScaleMode</returns>
        public float FromPixelsY(float y)
        {
            return this.m_originY + y * this.ZoomY / (this.m_dpiY / 100f);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dpiX"></param>
        /// <param name="dpiY"></param>
        public void SetDpi(float dpiX, float dpiY)
        {
            this.m_dpiX = dpiX;
            this.m_dpiY = dpiY;
        }

        /// <summary>
        /// Set the Bounds of the PictureBox
        /// </summary>
        /// <param name="marginBounds">Margin Bounds</param>
        public void SetMarginBounds(Rectangle marginBounds)
        {
            this.m_marginBounds = marginBounds;
        }

        /// <summary>
        /// Convert height value from actual ScaleMode to Pixels
        /// </summary>
        /// <param name="height">Height value</param>
        /// <returns>Converted value</returns>
        public float ToPixelsHeight(float height)
        {
            float num = height / this.ZoomY * (this.m_dpiY / 100f);
            this.CheckOverflow(num);
            return num;
        }

        /// <summary>
        /// Convert width value from actual ScaleMode to Pixels
        /// </summary>
        /// <param name="width">Width value</param>
        /// <returns>Converted value</returns>
        public float ToPixelsWidth(float width)
        {
            float num = width / this.ZoomX * (this.m_dpiX / 100f);
            this.CheckOverflow(num);
            return num;
        }

        /// <summary>
        /// Convert x value from actual ScaleMode to Pixels
        /// </summary>
        /// <param name="x">X value</param>
        /// <returns>Converted value</returns>
        public float ToPixelsX(float x)
        {
            float num = (x - this.m_originX) / this.ZoomX * (this.m_dpiX / 100f);
            this.CheckOverflow(num);
            return num;
        }

        /// <summary>
        /// Convert y value from actual ScaleMode to Pixels
        /// </summary>
        /// <param name="y">Y value</param>
        /// <returns>Converted value</returns>
        public float ToPixelsY(float y)
        {
            float num = (y - this.m_originY) / this.ZoomY * (this.m_dpiY / 100f);
            this.CheckOverflow(num);
            return num;
        }

        /// <summary>
        /// Check if value was OverFlow
        /// </summary>
        /// <param name="value">Value to check</param>
        private void CheckOverflow(float value)
        {
            if (Math.Abs(value) > 2.14748365E+09f)
            {
                Microsoft.VisualBasic.Information.Err().Raise(6, null, null, null, null);
            }
        }

        /// <summary>
        /// Check if value is near to zero
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <returns>true if is near to zero, false otherwise</returns>
        private bool NearZero(float value)
        {
            return Math.Abs(value) < 1E-10f;
        }


        /// <summary>
        /// Prints text to a PictureBox.
        /// </summary>
        /// <param name="toPrint">Text to print</param>
        public void Print(string toPrint)
        {

            objGraphics.DrawString(toPrint, this.Font, new SolidBrush(this.ForeColor), this.ToPixelsX(CurrentX), this.ToPixelsY(CurrentY));
        }

        /// <summary>
        /// Return a color value for the pixel at a specified point
        /// </summary>
        /// <param name="x1">Horizontal  cordinate</param>
        /// <param name="y1">Vertical cordinate</param>
        /// <returns>Color value at specific point</returns>
        public int Point(float x1, float y1)
        {
            return ColorTranslator.ToOle(bitmap.GetPixel(Convert.ToInt32(this.ToPixelsX(x1)), Convert.ToInt32(this.ToPixelsY(y1))));
        }

        /// <summary>
        /// Draws lines and rectangles on an object.
        /// </summary>
        /// <param name="x1">Horizontal staring point</param>
        /// <param name="y1">Vertical staring point</param>
        /// <param name="x2">Horizontal ending point</param>
        /// <param name="y2">Vertical ending point</param>
        public void Line(float x1, float y1, float x2, float y2)
        {
            Line(x1, y1, x2, y2, this.ForeColor);
        }

        /// <summary>
        /// Draws lines and rectangles on an object.
        /// </summary>
        /// <param name="x1">Horizontal staring point</param>
        /// <param name="y1">Vertical staring point</param>
        /// <param name="x2">Horizontal ending point</param>
        /// <param name="y2">Vertical ending point</param>
        /// <param name="color">Color used to draw the line</param>
        /// <param name="b">if is true causes a box to be drawn using the coordinates to specify opposite corners of the box</param>
        /// <param name="f">If the B option is true, the F option specifies that the box is filled with the same color used to draw the box</param>
        public void Line(float x1, float y1, float x2, float y2, Color color, bool b = false, bool f = false)
        {
            Pen tempPen = new Pen(color);
            if (this.DrawWidth <= 1)
            {
                tempPen.DashStyle = this.DrawStyle;
            }
            tempPen.Width = this.DrawWidth;
            if (b == false && f == false)
                objGraphics.DrawLine(tempPen, this.ToPixelsX(x1), this.ToPixelsY(y1), this.ToPixelsX(x2), this.ToPixelsY(y2));
            else if (b == true && f == false)
            {
                float p_x = x1;
                float p_y = y1;
                float p_width = x2 - p_x;
                float p_height = y2 - p_y;

                objGraphics.DrawRectangle(tempPen, this.ToPixelsX(p_x), this.ToPixelsY(p_y), this.ToPixelsWidth(p_width), this.ToPixelsHeight(p_height));

                Brush hb = FillBrush();
                objGraphics.FillRectangle(hb, this.ToPixelsX(p_x), this.ToPixelsY(p_y), this.ToPixelsWidth(p_width), this.ToPixelsHeight(p_height));
            }
            else if (b == true && f == true)
            {
                float p_x = x1;
                float p_y = y1;
                float p_width = x2 - p_x;
                float p_height = y2 - p_y;

                objGraphics.FillRectangle(new SolidBrush(color), this.ToPixelsX(p_x), this.ToPixelsY(p_y), this.ToPixelsWidth(p_width), this.ToPixelsHeight(p_height));
            }
        }

        /// <summary>
        /// Sets a point on an object to a specified color.
        /// </summary>
        /// <param name="x">Horizontal coordinate</param>
        /// <param name="y">Vertical coordinate</param>
        public void PSet(float x, float y)
        {
            this.PSet(x, y, this.ForeColor);
        }

        /// <summary>
        /// Sets a point on an object to a specified color.
        /// </summary>
        /// <param name="x">Horizontal coordinate</param>
        /// <param name="y">Vertical coordinate</param>
        /// <param name="color">Color used to draw the line</param>
        public void PSet(float x, float y, Color color)
        {
            objGraphics.DrawRectangle(new Pen(color), this.ToPixelsX(x), this.ToPixelsY(y), 1, 1);
            this.CurrentX = x;
            this.CurrentY = y;
        }

        /// <summary>
        /// Draws a circle, ellipse, or arc on an object.
        /// </summary>
        /// <param name="x">Horizontal coordinate of the center point of the circle</param>
        /// <param name="y">Vertical coordinate of the center point of the circle</param>
        /// <param name="radio">Circle radio</param>
        public void Circle(float x, float y, float radio)
        {
            Circle(x, y, radio, this.ForeColor);
        }

        /// <summary>
        /// Draws a circle, ellipse, or arc on an object.
        /// </summary>
        /// <param name="x">Horizontal coordinate of the center point of the circle</param>
        /// <param name="y">Vertical coordinate of the center point of the circle</param>
        /// <param name="radio">Circle radio</param>
        /// <param name="color">Color use to draw the circle</param>
        /// <param name="start">Starting point of the arc (Not Implemented)</param>
        /// <param name="end">Ending point of the arc (Not Implemented)</param>
        /// <param name="aspect">Value indicating the aspect ratio of the circle</param>
        public void Circle(float x, float y, float radio, Color color, float start = float.NaN, float end = float.NaN, float aspect = 1f)
        {

            float p_height = 0f;
            if (aspect >= 1f)
            {
                p_height = (radio * 2) / aspect;
            }
            else
            {
                p_height = (radio * 2) * aspect;
            }

            float p_x = x - (float)radio;
            float p_y = y - (p_height / 2);

            objGraphics.DrawEllipse(new Pen(color), this.ToPixelsX(p_x), this.ToPixelsY(p_y), this.ToPixelsX((float)radio * 2), this.ToPixelsY(p_height));
        }

        /// <summary>
        /// Clears graphics and text generated at run time
        /// </summary>
        public void Cls()
        {
            objGraphics.Clear(this.BackColor);
            this.CurrentX = 0;
            this.CurrentY = 0;
        }

        /// <summary>
        /// Returns a buils brush ready to be used in draw methods
        /// </summary>
        /// <returns>Brush to draw</returns>
        private Brush FillBrush()
        {
            Color color;
            if (this.FillStyle == FillStyleConstants.vbFsTransparent)
            {
                color = Color.Transparent;
            }
            else
            {
                color = this.FillColor;
            }
            Brush result;
            if (this.FillStyle == 0 || this.FillStyle == FillStyleConstants.vbFsTransparent)
            {
                result = new SolidBrush(color);
            }
            else
            {
                HatchStyle hatchstyle = HatchStyle.Horizontal;
                switch (this.FillStyle)
                {
                    case FillStyleConstants.vbHorizontalLine:
                        hatchstyle = HatchStyle.Horizontal;
                        break;
                    case FillStyleConstants.vbVerticalLine:
                        hatchstyle = HatchStyle.Vertical;
                        break;
                    case FillStyleConstants.vbUpwardDiagonal:
                        hatchstyle = HatchStyle.ForwardDiagonal;
                        break;
                    case FillStyleConstants.vbDownwardDiagonal:
                        hatchstyle = HatchStyle.BackwardDiagonal;
                        break;
                    case FillStyleConstants.vbCross:
                        hatchstyle = HatchStyle.Cross;
                        break;
                    case FillStyleConstants.vbDiagonalCross:
                        hatchstyle = HatchStyle.DiagonalCross;
                        break;
                }
                result = new HatchBrush(hatchstyle, color, Color.Transparent);
            }
            return result;
        }


        private bool initializing;
        void ISupportInitialize.BeginInit()
        {
            initializing = true;
        }

        void ISupportInitialize.EndInit()
        {
            Initialize();
            initializing = false;
        }

        public new Image Image
        {
            get
            {
                return base.Image;
            }
            set
            {
                base.Image = value;
                serializeImage = base.Image != null;
                if (!initializing)
                {
                    Initialize();
                }
            }
        }

        private bool serializeImage = false;
        private bool ShouldSerializeImage()
        {
            return serializeImage;
        }
    }

    /// <summary>
    /// FillStyle Constants
    /// </summary>
    public enum FillStyleConstants : short
    {
        vbFsSolid = 0,
        vbFsTransparent = 1,
        vbHorizontalLine = 2,
        vbVerticalLine = 3,
        vbUpwardDiagonal = 4,
        vbDownwardDiagonal = 5,
        vbCross = 6,
        vbDiagonalCross = 7
    }
}
