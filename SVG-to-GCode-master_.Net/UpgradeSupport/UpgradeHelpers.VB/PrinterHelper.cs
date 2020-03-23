using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Management;
using System.Threading;
using System.Windows.Forms;
using UpgradeHelpers.Helpers;
using Microsoft.VisualBasic;
using UpgradeHelpers.SupportHelper;
using System.Runtime.InteropServices;

namespace UpgradeHelpers.VB
{
#pragma warning  disable 1591
    /// <summary>
    /// Class to emulate the VB6 object printer.
    /// All internal lengths and sizes will be stored in the current unit of measurement defined by either
    /// ScaleHeight, ScaleWidth, ScaleLeft, ScaleTop, ScaleMode
    /// </summary>
    public class PrinterHelper : IDisposable
    {
        private IntPtr currentPen = IntPtr.Zero;
        private const int LOGPIXELSX = 88;
        private const int LOGPIXELSY = 90;
        private const float TwipsPerInch = 1440f;
        private const int TabSize = 66; //Mesured in Pixels
        private static PrinterHelper _printer;

        private static List<PrinterHelper> _printers;
        private static readonly float DisplayDpi;

        /// <summary>
        /// The physical Height of the paper for the printer. 
        /// According to Microsoft's documentation this value is given in Twips units
        /// </summary>
        //To avoid performance issues
        private int _cacheHeight = -1;
        private int _cacheWidth = -1;

        private float _currentX;
        private float _currentY;

        /// <summary>
        /// DrawMode: Sets the appearance of output from graphics methods or of a Shape or Line control.
        /// </summary>
        private DrawModeConstants _drawMode = DrawModeConstants.VbBlackness;

        private int _drawStyle;
        private readonly string _driverName = string.Empty;
        private int _duplex;
        private FillStyleConstants _fillStyle = FillStyleConstants.VbFsTransparent;
        private Font _font = new Font("Arial", 8.28f);

        /// <summary>
        /// Private property to store the average height of a text
        /// </summary>
        private float _internalTextHeight = -1;

        private Dictionary<PaperKind, PaperSize> _listOfAvailableSizes;
        private readonly string _port = string.Empty;
        private Graphics _printerGraphics;
        private int _printQuality = -3;
        private ScaleModeConstants _scaleMode = ScaleModeConstants.VbTwips;
        private readonly Brush _brush;

        private PageInfo _bufferPage;
        private string _currentPrinterName = string.Empty;
        public Color FillColor;
        public bool FontTransparent;
        private readonly PrintDocument _innerPrinter = new PrintDocument();
        private int _lastCustomHeight;
        private int _lastCustomWidth;
        private bool _lastPrintUserCanceled;
        //private readonly Pen _objPen = new Pen(Brushes.Black);
        private Color _currentColor = Color.Black;
        //private DashStyle _currentDashStyle = DashStyle.Solid;
        private int _drawWidth = 1;

        private int _pageIndex;
        private readonly AutoResetEvent _beginNewPage = new AutoResetEvent(false);
        private readonly AutoResetEvent _pagePrintedEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent _pageToPrintEvent = new AutoResetEvent(false);
        private Thread _printerThread;
        private readonly object _printerThreadLockObject = new object();
        private bool _printerThreadStarted;

        #region constructors

        static PrinterHelper()
        {
            using (Control ctrl = new Control())
            {
                Graphics g = Graphics.FromHwnd(ctrl.Handle);
                DisplayDpi = g.DpiX;
            }
        }

        public PrinterHelper(Brush brush)
            : this(null, brush)
        {
        }

        protected PrinterHelper(string printerName, Brush brush)
        {
            _bufferPage = new PageInfo();
            _port = GetPort();
            _driverName = GetDriverName();
            _innerPrinter.PrinterSettings.PrinterName = printerName;
            _innerPrinter.PrintController = new StandardPrintController();
            _innerPrinter.PrintPage += PrinterPrintPage;
            _innerPrinter.EndPrint += PrinterEndPrint;
            _brush = brush;
        }

        #endregion

        #region singleton pattern

        /// <summary>
        /// Gets the default <code>PrinterHelper</code> object
        /// </summary>
        public static PrinterHelper Printer
        {
            get
            {
                if (_printer == null)
                    _printer = new PrinterHelper(null);
                return _printer;
            }
            set
            {
                _printer = value;
            }
        }

        public static List<PrinterHelper> Printers
        {
            get
            {
                if (_printers == null)
                {
                    _printers = new List<PrinterHelper>();
                    foreach (string printerName in PrinterSettings.InstalledPrinters)
                        _printers.Add(new PrinterHelper(printerName, null));
                }
                return _printers;
            }
        }
        #endregion

        /// <summary>
        /// Calcula el maximo de columnas disponibles para imprimir en la pagina
        /// </summary>
        private int MaxColumnAvailables
        {
            get { return (int)(ConvertFromPixelsX(ConvertToPixelsX(Width, ScaleModeConstants.VbTwips)) / GetColumnSize()); }
        }

        public Font Font
        {
            get { return _font; }
            set
            {

                _font = value;
                _internalTextHeight = -1;
                SetFontToHdc(_font.Size);
            }
        }

        public int FillStyle
        {
            get { return (int)_fillStyle; }
            set { _fillStyle = (value == 0) ? FillStyleConstants.VbFsSolid : FillStyleConstants.VbFsTransparent; }
        }

        public int ScaleMode
        {
            get { return (int)_scaleMode; }
            set
            {
                switch (value)
                {
                    case 0:
                        _scaleMode = ScaleModeConstants.VbUser;
                        break;
                    case 2:
                        _scaleMode = ScaleModeConstants.VbPoints;
                        break;
                    case 3:
                        _scaleMode = ScaleModeConstants.VbPixels;
                        break;
                    case 4:
                        _scaleMode = ScaleModeConstants.VbCharacters;
                        break;
                    case 5:
                        _scaleMode = ScaleModeConstants.VbInches;
                        break;
                    case 6:
                        _scaleMode = ScaleModeConstants.VbMilimeters;
                        break;
                    case 7:
                        _scaleMode = ScaleModeConstants.VbCentimeters;
                        break;
                    case 8:
                        _scaleMode = ScaleModeConstants.VbHimetric;
                        break;
                    case 9:
                        _scaleMode = ScaleModeConstants.VbContainerPosition;
                        break;
                    case 10:
                        _scaleMode = ScaleModeConstants.VbContainerSize;
                        break;
                    default:
                        _scaleMode = ScaleModeConstants.VbTwips;
                        break;
                }

                // Reset text height when scaling mode changes
                _internalTextHeight = -1;
            }
        }

        public string DriverName
        {
            get { return _driverName; }
        }

        public string Port
        {
            get { return _port; }
        }

        /// <summary>
        /// Utility property to indicate if the current printer is local or networked
        /// </summary>
        public bool IsLocal
        {
            get { return IsLocalPrinter(DeviceName); }
        }

        public int DrawMode
        {
            get { return (int)_drawMode; }
            set
            {
                switch (value)
                {
                    case 1:
                        _drawMode = DrawModeConstants.VbBlackness;
                        break;
                    case 2:
                        _drawMode = DrawModeConstants.VbNotMergePen;
                        break;
                    case 3:
                        _drawMode = DrawModeConstants.VbMaskNotPen;
                        break;
                    case 4:
                        _drawMode = DrawModeConstants.VbNotCopyPen;
                        break;
                    case 5:
                        _drawMode = DrawModeConstants.VbMaskPenNot;
                        break;
                    case 6:
                        _drawMode = DrawModeConstants.VbInvert;
                        break;
                    case 7:
                        _drawMode = DrawModeConstants.VbXorPen;
                        break;
                    case 8:
                        _drawMode = DrawModeConstants.VbNotMaskPen;
                        break;
                    case 9:
                        _drawMode = DrawModeConstants.VbMaskPen;
                        break;
                    case 10:
                        _drawMode = DrawModeConstants.VbNotXorPen;
                        break;
                    case 11:
                        _drawMode = DrawModeConstants.VbNop;
                        break;
                    case 12:
                        _drawMode = DrawModeConstants.VbMergeNotPen;
                        break;
                    case 13:
                        _drawMode = DrawModeConstants.VbCopyPen;
                        break;
                    case 14:
                        _drawMode = DrawModeConstants.VbMergePenNot;
                        break;
                    case 15:
                        _drawMode = DrawModeConstants.VbMergePen;
                        break;
                    case 16:
                        _drawMode = DrawModeConstants.VbWhiteness;
                        break;
                    default:
                        _drawMode = DrawModeConstants.VbBlackness;
                        break;
                }
            }
        }

        public int ColorMode
        {
            get { return (_innerPrinter.PrinterSettings.DefaultPageSettings.Color) ? 2 : 1; }
            set { _innerPrinter.PrinterSettings.DefaultPageSettings.Color = (value != 1); }
        }

        public int Copies
        {
            get { return _innerPrinter.PrinterSettings.Copies; }
            set { _innerPrinter.PrinterSettings.Copies = (short)value; }
        }

        /// <summary>
        /// Returns or set the horizontal coordinate for the next printing or drawing method
        /// Coordinates are expressed in the current unit of measurement defined by ScaleHeight, ScaleWidth, ScaleLeft,
        /// ScaleTop and ScaleMode
        /// </summary>
        public float CurrentX
        {
            get { return _currentX; }
            set { _currentX = value; }
        }

        /// <summary>
        /// Returns or set the vertical coordinate for the next printing or drawing method
        /// Coordinates are expressed in the current unit of measurement defined by ScaleHeight, ScaleWidth, ScaleLeft,
        /// ScaleTop and ScaleMode
        /// </summary>
        public float CurrentY
        {
            get { return _currentY; }
            set { _currentY = value; }
        }

        public string DeviceName
        {
            get { return _innerPrinter.PrinterSettings.PrinterName; }
        }

        public int DrawStyle
        {
            get { return _drawStyle; }
            set
            {
                _drawStyle = value;
                SetCurrentHdcPenSettings();
            }
        }

        public int DrawWidth
        {
            get { return _drawWidth; }
            set
            {
                _drawWidth = value; //(int)ConvertToPixelsX(value);
                SetCurrentHdcPenSettings();
            }
        }

        public float ScaleWidth
        {
            get
            {
                return ScaleY(Width, (int)ScaleModeConstants.VbTwips, (int)this._scaleMode);
            }
        }

        private int GetPixelsPerInchXFromDevice()
        {
            IntPtr printerHdc = PrinterGraphics.GetHdc();
            int caps = NativeMethods.GetDeviceCaps(printerHdc, LOGPIXELSX);
            PrinterGraphics.ReleaseHdc(printerHdc);
            return caps;
        }

        public float ScaleHeight
        {
            get
            {
                return ScaleY(Height, (int)ScaleModeConstants.VbTwips, (int)this._scaleMode);
            }
        }

        private int GetPixelsPerInchYFromDevice()
        {
            IntPtr printerHdc = PrinterGraphics.GetHdc();
            int caps = NativeMethods.GetDeviceCaps(printerHdc, LOGPIXELSY);
            PrinterGraphics.ReleaseHdc(printerHdc);
            return caps;
        }

        #region not implemented methods
        //PaperBin
        // Returns/sets the paper size for the current printer
        public int PaperBin
        {
            get { throw new NotSupportedException("Method or Property not implemented yet!"); }
            set { throw new NotSupportedException("Method or Property not implemented yet!"); }
        }

        //PSet
        //Sets a point on an object to a specified color.

        //RightToLeft
        //Returns a boolean value indicating text display direction and control visual appearance on a bidirectional system.
        public bool RightToLeft
        {
            get { throw new NotSupportedException("Method or Property not implemented yet!"); }
            set { throw new NotSupportedException("Method or Property not implemented yet!"); }
        }

        //ScaleX
        //Converts the value for the width of a Form, PictureBox, or Printer from one unit of measure to another.

        //TrackDefault
        //Returns/sets a value that determines if the Printer object considers the default printer setting in the Control Panel.
        public bool TrackDefault
        {
            get { throw new NotSupportedException("Method or Property not implemented yet!"); }
            set { throw new NotSupportedException("Method or Property not implemented yet!"); }
        }

        //Zoom
        //Returns/sets the percentage by which printed output is to be scaled up or down.
        public int Zoom
        {
            get { throw new NotSupportedException("Method or Property not implemented yet!"); }
            set { throw new NotSupportedException("Method or Property not implemented yet!"); }
        }

        public void PSet(Point p, Color color, int step)
        {
            throw new Exception("Method or Property not implemented yet!");
        }

        public void PSet(Point p, Color color)
        {
            throw new Exception("Method or Property not implemented yet!");
        }

        public void PSet(Point p)
        {
            throw new Exception("Method or Property not implemented yet!");
        }

        //Scale
        //Defines the coordinate system for a Form, PictureBox, or Printer.
        public void Scale(Point p1, Point p2, int flags)
        {
            throw new Exception("Method or Property not implemented yet!");
        }

        public void Scale(Point p1, Point p2)
        {
            throw new Exception("Method or Property not implemented yet!");
        }

        public void Scale()
        {
            throw new Exception("Method or Property not implemented yet!");
        }

        public float ScaleX(float width, int fromScale, int toScale)
        {
            if (fromScale == toScale)
            {
                return width;
            }
            else if (fromScale == (int)ScaleModeConstants.VbTwips &&
              toScale == (int)ScaleModeConstants.VbPixels)
            {
                return ConvertFromTwipsToPixelsX(width);
            }
            if (fromScale == (int)ScaleModeConstants.VbPixels &&
                toScale == (int)ScaleModeConstants.VbTwips)
            {
                return ConvertFromPixelsToTwipsX(width);
            }
            else
            {
                throw new NotImplementedException(string.Format("Conversion from {0} to {1} not implemented", fromScale, toScale));
            }

        }

        private float ConvertFromTwipsToPixelsX(float x)
        {
            return (x * GetPixelsPerInchXFromDevice()) / TwipsPerInch;
        }

        private float ConvertFromTwipsToPixelsY(float y)
        {
            return (y * GetPixelsPerInchYFromDevice()) / TwipsPerInch;
        }

        private float ConvertFromPixelsToTwipsX(float x)
        {
            return x * TwipsPerPixelX;
        }

        private float ConvertFromPixelsToTwipsY(float y)
        {
            return y * TwipsPerPixelY;
        }

        public float ScaleX(float width, int fromScale)
        {
            return ScaleX(width, fromScale, (int)_scaleMode);
        }

        public float ScaleX(float width)
        {
            throw new Exception("Method or Property not implemented yet!");
        }

        public float ScaleY(float height, int fromScale, int toScale)
        {
            if (fromScale == toScale)
            {
                return height;
            }
            else if (fromScale == (int)ScaleModeConstants.VbTwips &&
              toScale == (int)ScaleModeConstants.VbPixels)
            {
                return ConvertFromTwipsToPixelsY(height);
            }
            if (fromScale == (int)ScaleModeConstants.VbPixels &&
                toScale == (int)ScaleModeConstants.VbTwips)
            {
                return ConvertFromPixelsToTwipsY(height);
            }
            else
            {
                throw new NotImplementedException(string.Format("Conversion from {0} to {1} not implemented", fromScale, toScale));
            }
        }

        public float ScaleY(float height, int fromScale)
        {
            return ScaleY(height, fromScale, (int)_scaleMode);
        }

        public float ScaleY(float height)
        {
            throw new Exception("Method or Property not implemented yet!");
        }

        #endregion

        //Returns or sets a value that determines whether a page is printed on both sides (if the printer supports this feature). 
        //Not available at design time.
        public int Duplex
        {
            get { return _duplex; }
            set
            {
                _duplex = value;
                switch (value)
                {
                    case 1:
                        _innerPrinter.PrinterSettings.Duplex = System.Drawing.Printing.Duplex.Simplex;
                        break;
                    case 2:
                        _innerPrinter.PrinterSettings.Duplex = System.Drawing.Printing.Duplex.Vertical;
                        break;
                    case 3:
                        _innerPrinter.PrinterSettings.Duplex = System.Drawing.Printing.Duplex.Horizontal;
                        break;
                    default:
                        _innerPrinter.PrinterSettings.Duplex = System.Drawing.Printing.Duplex.Default;
                        _duplex = 0;
                        break;
                }
            }
        }

        public String DocumentName
        {
            get { return _innerPrinter.DocumentName; }
            set { _innerPrinter.DocumentName = value; }
        }

        public bool FontBold
        {
            get { return Font.Bold; }
            set
            {
                Font = new Font(Font.FontFamily,
                    Font.Size,
                    value ? (Font.Style | FontStyle.Bold) : Font.Style & ~FontStyle.Bold);
            }
        }

        public int FontCount
        {
            get
            {
                // Get an array of the available font families.
                FontFamily[] families = FontFamily.Families;
                return families.Length;

            }
        }

        public bool FontItalic
        {
            get { return Font.Italic; }
            set
            {
                Font = new Font(Font.FontFamily,
                    Font.Size,
                    value ? (Font.Style | FontStyle.Italic) : Font.Style & ~FontStyle.Italic);
            }
        }

        public string FontName
        {
            get { return Font.Name; }
            set
            {
                if (!value.Equals(Font.Name))
                {
                    Font = new Font(value, Font.Size, Font.Style);
                    if (!value.Equals(Font.Name))
                        Font = new Font("Arial", Font.Size, Font.Style);
                }
            }
        }

        public float FontSize
        {
            get { return Font.Size; }
            set
            {
                if (value != Font.Size)
                {
                    Font = new Font(Font.FontFamily, value, Font.Style);
                }
            }
        }

        public bool FontStrikethru
        {
            get { return Font.Strikeout; }
            set
            {
                Font = new Font(Font.FontFamily,
                    Font.Size,
                    value ? Font.Style | FontStyle.Strikeout : Font.Style & ~FontStyle.Strikeout);
            }
        }

        public bool FontUnderline
        {
            get { return Font.Underline; }
            set
            {
                Font = new Font(Font.FontFamily,
                    Font.Size,
                    value ? Font.Style | FontStyle.Underline : Font.Style & ~FontStyle.Underline);
            }
        }

        public Color ForeColor
        {
            get { return _currentColor; }
            set
            {
                _currentColor = value;
                SetCurrentHdcPenSettings();
            }
        }


        public int Hdc
        {
            get
            {
                System.Diagnostics.Debug.Assert(currentGraphicsHdc != 0, "Handle not created, a print method must be called before requesting the HDC");
                return currentGraphicsHdc;
            }
        }

        // The physical dimensions of the paper set up for the printing device; not available at design time. 
        // If set at run time, values in these properties are used instead of the setting of the PaperSize property.

        public int Height
        {
            get
            {
                if (_cacheHeight < 0)
                {
                    if (_innerPrinter.DefaultPageSettings.Landscape)
                        _cacheHeight =
                            (int)
                                ConvertFromPrinterUnitsY(_innerPrinter.DefaultPageSettings.PaperSize.Width,
                                    ScaleModeConstants.VbTwips);
                    else
                        _cacheHeight =
                            (int)
                                ConvertFromPrinterUnitsY(_innerPrinter.DefaultPageSettings.PaperSize.Height,
                                    ScaleModeConstants.VbTwips);
                }

                return _cacheHeight;
            }
            set
            {
                //In VB6, If you set the Height and Width properties for a printer driver that doesn't 
                //allow these properties to be set, no error occurs and the size 
                //of the paper remains as it was.
                try
                {
                    _lastCustomHeight = Convert.ToInt32(ConvertToPrinterUnitsY(value, ScaleModeConstants.VbTwips));
                    PaperSize = PrinterObjectConstants.VbPRPSUser;
                }
                catch (Exception e)
                {
                    Trace.TraceError("GetDriverName {0}", e.Message);
                }
            }
        }

        // The physical dimensions of the paper set up for the printing device; not available at design time. 
        // If set at run time, values in these properties are used instead of the setting of the PaperSize property.

        /// <summary>
        /// The physical Width of the paper for the printer. 
        /// According to Microsoft's documentation this value is given in Twips units
        /// </summary>
        public int Width
        {
            get
            {
                if (_cacheWidth < 0)
                {
                    if (_innerPrinter.DefaultPageSettings.Landscape)
                    {
                        _cacheWidth = (int)ConvertFromPrinterUnitsX(_innerPrinter.DefaultPageSettings.PrintableArea.Height, ScaleModeConstants.VbTwips);
                    }
                    else
                    {
                        _cacheWidth = (int)ConvertFromPrinterUnitsX(_innerPrinter.DefaultPageSettings.PrintableArea.Width, ScaleModeConstants.VbTwips);
                    }
                }
                return _cacheWidth;
            }
            set
            {
                //In VB6, If you set the Height and Width properties for a printer driver that doesn't 
                //allow these properties to be set, no error occurs and the size 
                //of the paper remains as it was.
                try
                {
                    _lastCustomWidth = Convert.ToInt32(ConvertToPrinterUnitsX(value, ScaleModeConstants.VbTwips));
                    PaperSize = PrinterObjectConstants.VbPRPSUser;
                }
                catch (Exception e)
                {
                    Trace.TraceError("GetDriverName {0}", e.Message);
                }
            }
        }

        public int Orientation
        {
            get { return (_innerPrinter.DefaultPageSettings.Landscape) ? 2 : 1; }
            set
            {
                _innerPrinter.DefaultPageSettings.Landscape = (value == 2);
                _cacheHeight = -1;
                _cacheWidth = -1;
            }
        }

        public int Page
        {
            get { return _pageIndex + 1; }
        }

        /////////////////////////////
        //Returns or sets a value indicating the paper size for the current printer. Not available at design time.
        public int PaperSize
        {
            get
            {
                return _innerPrinter.DefaultPageSettings.PaperSize.Kind == PaperKind.Custom
                    ?
                        PrinterObjectConstants.VbPRPSUser
                    : _innerPrinter.DefaultPageSettings.PaperSize.RawKind;
            }
            set
            {
                if ((value != PaperSize) || (value == PrinterObjectConstants.VbPRPSUser))
                {
                    _innerPrinter.DefaultPageSettings.PaperSize = GetPaperSize(value);
                    _cacheHeight = -1;
                    _cacheWidth = -1;
                    PrintingAreaChanged();
                }
            }
        }

        /// <summary>
        /// <para>This method is invoked when a property that change the printing area is modified.</para>
        /// </summary>
        protected virtual void PrintingAreaChanged()
        {
        }

        /// <summary>
        /// Returns the list of available sizes for the current printer
        /// </summary>
        private Dictionary<PaperKind, PaperSize> ListOfAvailableSizes
        {
            get
            {
                if (!_currentPrinterName.Equals(_innerPrinter.PrinterSettings.PrinterName) ||
                    (_listOfAvailableSizes == null))
                {
                    _currentPrinterName = _innerPrinter.PrinterSettings.PrinterName;
                    _listOfAvailableSizes = new Dictionary<PaperKind, PaperSize>();
                    foreach (PaperSize pSize in _innerPrinter.PrinterSettings.PaperSizes)
                    {
                        if (!_listOfAvailableSizes.ContainsKey(pSize.Kind))
                            _listOfAvailableSizes.Add(pSize.Kind, pSize);
                    }
                }

                return _listOfAvailableSizes;
            }
        }

        private Graphics PrinterGraphics
        {
            get
            {
                if (_printerGraphics == null)
                    _printerGraphics = _innerPrinter.PrinterSettings.CreateMeasurementGraphics();

                return _printerGraphics;
            }
        }

        private float InternalTextHeight
        {
            get
            {
                if (_internalTextHeight < 0)
                    _internalTextHeight = TextHeight("Text Height");

                return _internalTextHeight;
            }
        }

        public int PrintQuality //System.Drawing.Printing.PrinterResolutionKind PrintQuality
        {
            get
            {
                return _printQuality; //_innerPrinter.DefaultPageSettings.PrinterResolution.Kind;
            }
            set
            {
                _printQuality = value;
                switch (value)
                {
                    case -1:
                        //Draft
                        _innerPrinter.DefaultPageSettings.PrinterResolution =
                            _innerPrinter.PrinterSettings.PrinterResolutions[3];
                        break;
                    case -3:
                        //Medium
                        _innerPrinter.DefaultPageSettings.PrinterResolution =
                            _innerPrinter.PrinterSettings.PrinterResolutions[2];
                        break;
                    case -2:
                        //Low
                        _innerPrinter.DefaultPageSettings.PrinterResolution =
                            _innerPrinter.PrinterSettings.PrinterResolutions[1];
                        break;
                    case -4:
                        //High
                        _innerPrinter.DefaultPageSettings.PrinterResolution =
                            _innerPrinter.PrinterSettings.PrinterResolutions[0];
                        break;
                    default:
                        //Default value is Medium Resolution
                        _innerPrinter.DefaultPageSettings.PrinterResolution =
                            _innerPrinter.PrinterSettings.PrinterResolutions[1];
                        _printQuality = -3;
                        break;
                }
            }
        }

        //Returns or sets a value indicating the printer resolution. Not available at design time.
        public int PrintQuality2
        {
            get { return (int)_innerPrinter.DefaultPageSettings.PrinterResolution.Kind; }
            set
            {
                switch (value)
                {
                    case -1:
                        //Draft
                        _innerPrinter.DefaultPageSettings.PrinterResolution =
                            _innerPrinter.PrinterSettings.PrinterResolutions[3];
                        break;
                    case -2:
                        //Low
                        _innerPrinter.DefaultPageSettings.PrinterResolution =
                            _innerPrinter.PrinterSettings.PrinterResolutions[2];
                        break;
                    case -3:
                        //Medium
                        _innerPrinter.DefaultPageSettings.PrinterResolution =
                            _innerPrinter.PrinterSettings.PrinterResolutions[1];
                        break;
                    case -4:
                        //High
                        _innerPrinter.DefaultPageSettings.PrinterResolution =
                            _innerPrinter.PrinterSettings.PrinterResolutions[0];
                        break;
                    default:
                        //Default value is Medium Resolution

                        _innerPrinter.DefaultPageSettings.PrinterResolution =
                            _innerPrinter.PrinterSettings.PrinterResolutions[1];
                        break;
                }
            }
        }

        public float TwipsPerPixelX
        {
            get { return TwipsPerInch / GetPixelsPerInchXFromDevice(); }
        }

        public float TwipsPerPixelY
        {
            get { return TwipsPerInch / GetPixelsPerInchYFromDevice(); }
        }

        /// <summary>
        /// Utility function to indicate if a printer is local or networked. This function will fail in Windows 2000
        /// and Windows NT 4.0 as the properties Local, Network are not supported in those systems.
        /// </summary>
        /// <param name="deviceName">The name of the printer</param>
        /// <returns>true if the printer is local</returns>
        public static bool IsLocalPrinter(string deviceName)
        {
            try
            {
                string queryString = "SELECT Local, Network FROM Win32_Printer WHERE Name=\"" +
                    deviceName.Replace("\\", "\\\\") + "\"";
                using (ManagementObjectSearcher query = new ManagementObjectSearcher(queryString))
                {
                    ManagementObjectCollection queryCollection = query.Get();
                    foreach (ManagementObject mo in queryCollection)
                        return Convert.ToBoolean(mo["Local"]) && !Convert.ToBoolean(mo["Network"]);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Exception thrown while getting information for the printer. " +
                    "The current implementation is not supported in Windows 2000 and Windows NT. " + e.Message);
            }
            return false;
        }

        private string GetDriverName()
        {
            string driverName = string.Empty;
            try
            {
                string queryString = "SELECT DriverName FROM Win32_Printer WHERE Name=\"" +
                    DeviceName.Replace("\\", "\\\\") + "\"";
                using (ManagementObjectSearcher query = new ManagementObjectSearcher(queryString))
                {
                    ManagementObjectCollection queryCollection = query.Get();
                    foreach (ManagementObject mo in queryCollection)
                    {
                        driverName = mo["DriverName"] as string;
                    }
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("GetDriverName {0}", e.Message);
            }
            return driverName;
        }

        /// <summary>
        /// Function to return the port of the current printer
        /// </summary>
        /// <returns></returns>
        private string GetPort()
        {
            string portName = string.Empty;
            try
            {
                string queryString = "SELECT PortName FROM Win32_Printer WHERE Name=\"" +
                    DeviceName.Replace("\\", "\\\\") + "\"";
                using (ManagementObjectSearcher query = new ManagementObjectSearcher(queryString))
                {
                    ManagementObjectCollection queryCollection = query.Get();
                    foreach (ManagementObject mo in queryCollection)
                    {
                        if (!string.IsNullOrEmpty(portName))
                            portName += ", ";

                        portName += mo["PortName"] as string;
                    }
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("GetDriverName {0}", e.Message);
            }
            return portName;
        }

        /// <summary>
        /// Terminates a print operation sent to the Printer object, releasing the document to the print device or spooler. 
        /// </summary>
        public void EndDoc()
        {
            //Current page is marked to end the printing processs
            _bufferPage.EndDoc = true;
            try
            {
                SendPageToPrint();
            }
            finally
            {
                //Returns the PaperSize to the default value after printing
                _lastCustomHeight = _lastCustomWidth = 0;
                PaperSize = (int)_innerPrinter.PrinterSettings.DefaultPageSettings.PaperSize.Kind;

                ResetBufferPage();
                _pageIndex = 0;
            }
        }

        public string GetFonts(int index)
        {
            // Get an array of the available font families.
            FontFamily[] families = FontFamily.Families;
            return families[index].Name;
        }

        /// <summary>
        /// The column size for the function Tab given in the internal unit given by ScaleMode
        /// </summary>
        private float GetColumnSize()
        {
            //El tamaño de una columna para ser utilizado por la funcion Tab deberia ser el promedio de todos los caracteres
            //para el font actual, pero no se ha podido emular el mismo valor que utiliza VB6, asi que se utiliza
            //el string de abajo para hacer un calculo aproximado
            SizeF size = PrinterGraphics.MeasureString(
                "1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890",
                Font);
            return (float)ConvertFromPrinterUnitsX(size.Width / 100);
        }


        #region Circle
        [Obsolete("Use the methods receiving PointF in order to prevent loss of precision")]
        public void Circle(Point point, int radius)
        {
            Circle(point, radius, false);
        }

        [Obsolete("Use the methods receiving PointF in order to prevent loss of precision")]
        public void Circle(Point point, int radius, bool step)
        {
            Circle(point, radius, _currentColor, step);
        }

        [Obsolete("Use the methods receiving PointF in order to prevent loss of precision")]
        public void Circle(Point point, int radius, Color color)
        {
            Circle(point, radius, color, false);
        }

        [Obsolete("Use the methods receiving PointF in order to prevent loss of precision")]
        public void Circle(Point point, int radius, Color color, bool step)
        {
            PointF newP = new PointF(point.X, point.Y);
            Circle(newP, radius, color, step);
        }

        /// <summary>
        /// point and radius are given in the unit specified by Scalemode
        /// </summary>
        public void Circle(PointF point, double radius)
        {
            Circle(point, radius, false);
        }

        /// <summary>
        /// point and radius are given in the unit specified by Scalemode
        /// </summary>
        public void Circle(PointF point, double radius, bool step)
        {
            Circle(point, radius, _currentColor, step);
        }

        /// <summary>
        /// point and radius are given in the unit specified by Scalemode
        /// </summary>
        public void Circle(PointF point, double radius, Color color)
        {
            Circle(point, radius, color, false);
        }

        /// <summary>
        /// point and radius are given in the unit specified by Scalemode
        /// </summary>
        public void Circle(PointF point, double radius, Color color, bool step)
        {
            if (step)
            {
                point.X = point.X + (float)CurrentX;
                point.Y = point.Y + (float)CurrentY;
            }
            double diameter = radius * 2;

            // Moves the CurrentX and CurrentY properties
            CurrentX = (float)(point.X + radius);
            CurrentY = (float)(point.Y + radius);

            //The final values are stored in pixels
            RectangleF circleRec = new RectangleF(ConvertToPrinterUnitsX(point.X - radius),
                ConvertToPrinterUnitsY(point.Y - radius),
                ConvertToPrinterUnitsX(diameter),
                ConvertToPrinterUnitsY(diameter));

            RectangleInfo circleInfo = new RectangleInfo();

            circleInfo.Rec = circleRec;
            //circleInfo.Pen = _objPen;
            circleInfo.PenInfo = new PenInfo(this.DrawWidth, this.DrawStyle, color);

            circleInfo.FillColor = FillColor;
            circleInfo.FillStyle = _fillStyle;
            _bufferPage.AddCircle(circleInfo);
        }
        #endregion

        //The Line method was developed using a combination of optional parameters and overloading because:
        //   -  VB.NET doesn’t accepts Structures (i.e Color, Point) as optional parameters
        //   -  In VB.BET the optional parameters have to be located at the end of the method declaration

        #region line
        [Obsolete("Use the methods receiving PointF in order to prevent lost of precision")]
        public void Line(Point point1, Point point2, bool step1, bool step2, bool box)
        {
            Line(point1, point2, step1, step2, box, false);
        }

        [Obsolete("Use the methods receiving PointF in order to prevent lost of precision")]
        public void Line(Point point1, Point point2, bool step1, bool step2)
        {
            Line(point1, point2, step1, step2, false, false);
        }

        [Obsolete("Use the methods receiving PointF in order to prevent lost of precision")]
        public void Line(Point point1, Point point2, bool step1)
        {
            Line(point1, point2, step1, false, false, false);
        }

        [Obsolete("Use the methods receiving PointF in order to prevent lost of precision")]
        public void Line(Point point1, Point point2)
        {
            Line(point1, point2, false, false, false, false);
        }

        [Obsolete("Use the methods receiving PointF in order to prevent lost of precision")]
        public void Line(Point point1, Point point2, bool step1, bool step2, bool box, bool fill)
        {
            Line(point1, point2, _currentColor, step1, step2, box, fill);
        }

        [Obsolete("Use the methods receiving PointF in order to prevent lost of precision")]
        public void Line(Point point1, Point point2, Color color, bool step1, bool step2, bool box)
        {
            Line(point1, point2, color, step1, step2, box, false);
        }

        [Obsolete("Use the methods receiving PointF in order to prevent lost of precision")]
        public void Line(Point point1, Point point2, Color color, bool step1, bool step2)
        {
            Line(point1, point2, color, step1, step2, false, false);
        }

        [Obsolete("Use the methods receiving PointF in order to prevent lost of precision")]
        public void Line(Point point1, Point point2, Color color, bool step1)
        {
            Line(point1, point2, color, step1, false, false, false);
        }

        [Obsolete("Use the methods receiving PointF in order to prevent lost of precision")]
        public void Line(Point point1, Point point2, Color color)
        {
            Line(point1, point2, color, false, false, false, false);
        }

        [Obsolete("Use the methods receiving PointF in order to prevent lost of precision")]
        public void Line(Point point1, Point point2, Color color, bool step1, bool step2, bool box, bool fill)
        {
            PointF newP1 = new PointF(point1.X, point1.Y);
            PointF newP2 = new PointF(point2.X, point2.Y);
            Line(newP1, newP2, color, step1, step2, box, fill);
        }


        /// <summary>
        /// point1 and point2 are given in the unit specified by Scalemode
        /// </summary>
        public void Line(PointF point1, PointF point2, bool step1, bool step2, bool box)
        {
            Line(point1, point2, step1, step2, box, false);
        }

        /// <summary>
        /// point1 and point2 are given in the unit specified by Scalemode
        /// </summary>
        public void Line(PointF point1, PointF point2, bool step1, bool step2)
        {
            Line(point1, point2, step1, step2, false, false);
        }

        /// <summary>
        /// point1 and point2 are given in the unit specified by Scalemode
        /// </summary>
        public void Line(PointF point1, PointF point2, bool step1)
        {
            Line(point1, point2, step1, false, false, false);
        }

        /// <summary>
        /// point1 and point2 are given in the unit specified by Scalemode
        /// </summary>
        public void Line(PointF point1, PointF point2)
        {
            Line(point1, point2, false, false, false, false);
        }

        /// <summary>
        /// point1 and point2 are given in the unit specified by Scalemode
        /// </summary>
        public void Line(PointF point1, PointF point2, bool step1, bool step2, bool box, bool fill)
        {
            Line(point1, point2, _currentColor, step1, step2, box, fill);
        }

        /// <summary>
        /// point1 and point2 are given in the unit specified by Scalemode
        /// </summary>
        public void Line(PointF point1, PointF point2, Color color, bool step1, bool step2, bool box)
        {
            Line(point1, point2, color, step1, step2, box, false);
        }

        /// <summary>
        /// point1 and point2 are given in the unit specified by Scalemode
        /// </summary>
        public void Line(PointF point1, PointF point2, Color color, bool step1, bool step2)
        {
            Line(point1, point2, color, step1, step2, false, false);
        }

        /// <summary>
        /// point1 and point2 are given in the unit specified by Scalemode
        /// </summary>
        public void Line(PointF point1, PointF point2, Color color, bool step1)
        {
            Line(point1, point2, color, step1, false, false, false);
        }

        /// <summary>
        /// point1 and point2 are given in the unit specified by Scalemode
        /// </summary>
        public void Line(PointF point1, PointF point2, Color color)
        {
            Line(point1, point2, color, false, false, false, false);
        }

        /// <summary>
        /// point1 and point2 are given in the unit specified by Scalemode
        /// </summary>
        public void Line(PointF point1, PointF point2, Color color, bool step1, bool step2, bool box, bool fill)
        {
            if (step1)
            {
                point1.X = point1.X + (float)CurrentX;
                point1.Y = point1.Y + (float)CurrentY;
            }

            if (step2)
            {
                point2.X = point2.X + (float)CurrentX;
                point2.Y = point2.Y + (float)CurrentY;
            }

            // Moves the CurrentX and CurrentY properties
            CurrentX = point2.X;
            CurrentY = point2.Y;

            if (box) // Draw a box
            {
                DrawRectangle(new RectangleF(point1.X, point1.Y, point2.X - point1.X, point2.Y - point1.Y), color, fill);
            }
            else // Draw a single line
            {
                DrawLine(point1, point2, color);
            }
        }
        #endregion

        #region picture
        //Draw an image
        [Obsolete("Use the method receiving PointF in order to prevent lost of precision")]
        public void PaintPicture(Image picture, Point p)
        {
            PointF newP = new PointF(p.X, p.Y);
            PaintPicture(picture, newP);
        }

        /// <summary>
        /// point is given in the unit specified by Scalemode
        /// </summary>
        public void PaintPicture(Image picture, PointF point)
        {
            //point is stored in pixels
            PointF newP = new PointF(ConvertToPrinterUnitsX(point.X), ConvertToPrinterUnitsY(point.Y));
            InternalPaintPicture(picture, newP, SizeF.Empty);
        }

        /// <summary>
        /// Paint a picture in the current page
        /// </summary>
        /// <param name="picture">The picture to draw</param>
        /// <param name="point1">The upper left corner</param>
        /// <param name="point2">The lower right corner</param>
        public void PaintPicture(Image picture, PointF point1, PointF point2)
        {
            //P is stored in pixels
            PointF newP = new PointF(ConvertToPrinterUnitsX(point1.X), ConvertToPrinterUnitsY(point1.Y));
            SizeF newSize = new SizeF((float)ConvertToPixelsX(point2.X - point1.X),
                (float)ConvertToPixelsY(point2.Y - point1.Y));

            InternalPaintPicture(picture, newP, newSize);
        }

        /// <summary>
        /// Paint a picture in the current page
        /// </summary>
        /// <param name="picture">The picture to draw</param>
        /// <param name="point">Position</param>
        /// <param name="size">Image size</param>
        public void PaintPicture(Image picture, PointF point, SizeF size)
        {
            PointF newP = new PointF(ConvertToPrinterUnitsX(point.X), ConvertToPrinterUnitsY(point.Y));
            SizeF newSize = new SizeF(ConvertToPrinterUnitsX(size.Width), ConvertToPrinterUnitsY(size.Height));
            InternalPaintPicture(picture, newP, newSize);
        }

        /// <summary>
        ///   Paints a picture in the current page
        /// </summary>
        /// <param name="picture">Picture reference</param>
        /// <param name="x1">X position in the paper</param>
        /// <param name="y1">Y position in the paper</param>
        /// <param name="w1">Width of the image to be drawn in the paper</param>
        /// <param name="h1">Height of the image to be drawn in the paper</param>
        /// <param name="x2">X position inside the original image</param>
        /// <param name="y2">Y position inside the original image</param>
        /// <param name="w2">width of the fragment show from the original image</param>
        /// <param name="h2">Height of the fragment show from the original image</param>
        public void PaintPicture(Image picture,
                                 float x1, float y1,
                                 float w1, float h1,
                                 float x2, float y2, float w2, float h2)
        {
            PaintPicture(picture,
                         new PointF(x1, y1),
                         new SizeF(w1, h1),
                         new RectangleF(x2, y2, w2, h2));

        }

        /// <summary>
        ///   Paints a picture in the current page
        /// </summary>
        /// <param name="picture">Picture reference</param>
        /// <param name="point">position of the image in the paper</param>
        /// <param name="size"> size of the image in the paper</param>
        /// <param name="clippingInfo">clipping information for the original image</param>
        public void PaintPicture(Image picture, PointF point, SizeF size, RectangleF clippingInfo)
        {
            PaintPicture(picture, point, size);
            ImageInfo newImageInformation = _bufferPage.Images[_bufferPage.Images.Count - 1];

            newImageInformation.hasClippingInfo = true;
            newImageInformation.clippingInfo =
               new RectangleF(ConvertToPrinterUnitsX(clippingInfo.X),
                              ConvertToPrinterUnitsY(clippingInfo.Y),
                              ConvertToPrinterUnitsX(clippingInfo.Width),
                              ConvertToPrinterUnitsY(clippingInfo.Height));
            _bufferPage.Images[_bufferPage.Images.Count - 1] = newImageInformation;
        }


        private void InternalPaintPicture(Image picture, PointF point, SizeF size)
        {
            ImageInfo newImage = new ImageInfo();

            newImage.P = point;
            newImage.Picture = (Image)picture.Clone();
            newImage.Size = size;
            _bufferPage.AddImageINfo(newImage);

        }

        #endregion

        /// <summary>
        /// point1 and point2 are given in the unit specified by Scalemode
        /// </summary>
        private void DrawLine(PointF point1, PointF point2, Color color)
        {
            //Stores the position in pixels within the structure Line
            PointF newP1 = new PointF(ConvertToPrinterUnitsX(point1.X), ConvertToPrinterUnitsY(point1.Y));
            PointF newP2 = new PointF(ConvertToPrinterUnitsX(point2.X), ConvertToPrinterUnitsY(point2.Y));

            LineInfo lineInfo = new LineInfo();

            lineInfo.P1 = newP1;
            lineInfo.P2 = newP2;
            // lineInfo.Pen = (Pen)_objPen.Clone();
            // lineInfo.Pen.Color = color;
            lineInfo.PenInfo = new PenInfo((int)ConvertToPixelsX(this.DrawWidth), this.DrawStyle, color);


            _bufferPage.AddLine(lineInfo);
        }

        /// <summary>
        /// rec is given in the unit specified by Scalemode
        /// </summary>
        private void DrawRectangle(RectangleF rec, Color color, bool fill)
        {
            //Stores the position in pixels within the structure Rectangle
            RectangleF newRec = new RectangleF(ConvertToPrinterUnitsX(rec.X),
                ConvertToPrinterUnitsY(rec.Y),
                ConvertToPrinterUnitsX(rec.Width),
                ConvertToPrinterUnitsY(rec.Height));

            RectangleInfo newRectangle = new RectangleInfo();

            newRectangle.Rec = newRec;

            //newRectangle.Pen = (Pen)_objPen.Clone();
            //newRectangle.Pen.Color = color;
            newRectangle.PenInfo = new PenInfo((int)ConvertToPixelsX(this.DrawWidth), this.DrawStyle, color);
            if (fill)
            {
                newRectangle.FillStyle = FillStyleConstants.VbFsSolid;
                newRectangle.FillColor = color;
            }
            else
            {
                newRectangle.FillStyle = _fillStyle;
                newRectangle.FillColor = FillColor;
            }
            _bufferPage.AddRectangle(newRectangle);
        }

        /// <summary>
        /// Ends the current page and advances to the next page on the Printer object.
        /// </summary>
        public void NewPage()
        {
            try
            {
                SendPageToPrint();
                _pageIndex++;
            }
            finally
            {
                ResetBufferPage();
            }
        }

        //Wait handles to synchronize the printing of a page

        /// <summary>
        /// Send the current page to the spooler
        /// </summary>
        private void SendPageToPrint()
        {
            bool sendSignal = StartPrintThread();
            //Signals a page to print
            if (sendSignal)
            {
                _pageToPrintEvent.Set();
            }

            //Waits for the page to be printed            
            WaitPollingForPrinterCancelation(_pagePrintedEvent);

            // Wait for the next page to be available 
            if (_printerThreadStarted && !(_bufferPage.EndDoc || _bufferPage.KillDoc))
            {
                _beginNewPage.WaitOne();
            }
        }

        private void WaitPollingForPrinterCancelation(AutoResetEvent eventToPoll)
        {
            while (!eventToPoll.WaitOne(500, false))
            {
                if ((_printerThread == null) || (!_printerThread.IsAlive))
                {
                    lock (_printerThreadLockObject)
                    {
                        _printerThreadStarted = false;
                        _printerThread = null;
                        _pageToPrintEvent.Reset();
                        _pagePrintedEvent.Reset();
                        _beginNewPage.Reset();
                        if (_lastPrintUserCanceled && !_bufferPage.KillDoc)
                            throw new Exception("Printer error");
                        break;
                    }
                }
            }
        }

        private bool StartPrintThread()
        {
            _lastPrintUserCanceled = false;
            bool sendSignal = true;
            lock (_printerThreadLockObject)
            {
                if (!_printerThreadStarted)
                {
                    if ((_printerThread != null) && (_printerThread.IsAlive))
                    {
                        _printerThread.Abort();
                        _printerThread = null;
                        _pageToPrintEvent.Reset();
                        _pagePrintedEvent.Reset();
                        throw new Exception("Printer error");
                    }

                    _printerThread = (new Thread(new ThreadStart(StartPrinting)));
                    //_printerThread.SetApartmentState(ApartmentState.STA);
                    _printerThread.Start();
                    _printerThreadStarted = true;
                    sendSignal = false;

                    if (_printerThreadStarted)
                    {

                        // Wait for the print process to start                    
                        WaitPollingForPrinterCancelation(_beginNewPage);

                    }
                }
            }

            return sendSignal;
        }

        /// <summary>
        /// Start the printing process
        /// </summary>
        private void StartPrinting()
        {
            _innerPrinter.Print();
        }

        /// <summary>
        /// Returns a paper size suitable for the current printer
        /// </summary>
        /// <param name="value">The enum value given the desired paper size</param>
        /// <returns>The paper size that can be used or an exception if the paper size desired is not available</returns>
        private PaperSize GetPaperSize(int value)
        {
            if (value == PrinterObjectConstants.VbPRPSUser)
            {
                if (ListOfAvailableSizes.ContainsKey(PaperKind.Custom))
                {
                    PaperSize paperSize = new PaperSize(string.Empty, _lastCustomWidth, _lastCustomHeight);
                    paperSize.PaperName = ListOfAvailableSizes[PaperKind.Custom].PaperName;
                    paperSize.RawKind = ListOfAvailableSizes[PaperKind.Custom].RawKind;
                    return paperSize;
                }
            }
            else if ((Enum.IsDefined(typeof(PaperKind), value)) && (ListOfAvailableSizes.ContainsKey((PaperKind)value)))
            {
                return ListOfAvailableSizes[(PaperKind)value];
            }

            //Here we have a problem because the documentation of the original PaperSize property in VB6 states:
            //"Settings outside the accepted range may or may not produce an error. For more information, see the 
            //manufacturer's documentation for the specific driver", so basically we don't have a way to know when
            //is correct or not to send and exception
            return _innerPrinter.DefaultPageSettings.PaperSize;
            //throw new InvalidOperationException("Invalid property value");
        }

        /// <summary>
        /// Returns the height of a text string as it would be printed in the current font of the printer.
        /// The height is expressed in terms of the ScaleMode property setting
        /// </summary>
        /// <param name="str">The string text to use in the calculation</param>
        /// <returns>The text height expressed in terms of the ScaleMode units</returns>
        public float TextWidth(string str)
        {
            SizeF size = PrinterGraphics.MeasureString(str, Font);
            return ConvertFromPrinterUnitsX(size.Width);
        }

        /// <summary>
        /// Returns the width of a text string as it would be printed in the current font of the printer.
        /// The width is expressed in terms of the ScaleMode property setting
        /// </summary>
        /// <param name="str">The string text to use in the calculation</param>
        /// <returns>The text width expressed in terms of the ScaleMode units</returns>
        public float TextHeight(string str)
        {
            SizeF size = PrinterGraphics.MeasureString(str, Font);
            return ConvertFromPrinterUnitsY(size.Height);
        }

        /// <summary>
        /// Print an empty line
        /// </summary>
        public void Print()
        {
            CurrentY += InternalTextHeight;
            CurrentX = 0;

            //If no caracter can be printed in the next line then a new page is inserted
            if ((CurrentY + InternalTextHeight) >
                ConvertFromPixelsY(ConvertToPixelsY(Height, ScaleModeConstants.VbTwips)))
                NewPage();
        }

        public void Print(bool noNewLine)
        {
            if (!noNewLine)
                Print();
        }

        public void Print(params object[] str)
        {
            StartPrintThread();
            Print(false, str);
        }

        public void Print(bool noNewLine, params object[] str)
        {
            //     All positions and sizes are calculated in the unit measurement given by ScaleMode,
            //     when the final numbers are stored in the Data structures then they are converted to
            //     pixels

            SpcInfo sInfo;

            for (int j = 0; j < str.Length; j++)
            {
                string myStr = str[j].ToString();
                if (myStr.Trim(' ').Length == 0)
                {
                    sInfo.Count = (short)myStr.Length;
                    str[j] = sInfo;
                }

                if ((myStr == Environment.NewLine) | (myStr == "\r") | (myStr == "\n") | (myStr == Environment.NewLine))
                {
                    CurrentY += InternalTextHeight;
                    CurrentX = 0;
                }
                else if (myStr == "\t")
                {
                    CurrentX += (float)ConvertFromPixelsX(TabSize);
                }
                else if (str[j] is TabInfo)
                {
                    TabInfo tInfo = ((TabInfo)str[j]);
                    if (tInfo.Column > MaxColumnAvailables)
                        tInfo.Column = (short)(tInfo.Column % MaxColumnAvailables);

                    if (tInfo.Column < 1)
                        tInfo.Column = 1;

                    tInfo.Column--;

                    float tabPos = tInfo.Column * GetColumnSize();
                    if (CurrentX > tabPos)
                        CurrentY += InternalTextHeight;

                    CurrentX = tabPos;
                }
                else if (str[j] is SpcInfo)
                {
                    sInfo = ((SpcInfo)str[j]);
                    if (sInfo.Count > 0)
                    {
                        CurrentX += sInfo.Count * GetColumnSize();
                    }
                }
                else
                {
                    DataInfo newDataInfo = new DataInfo();


                    newDataInfo.Value = myStr;
                    newDataInfo.Color = _currentColor;
                    newDataInfo.Font = (Font)Font.Clone();
                    newDataInfo.X = ConvertToPrinterUnitsX(CurrentX);
                    newDataInfo.Y = ConvertToPrinterUnitsY(CurrentY);

                    _bufferPage.AddDataInfo(newDataInfo);
                    CurrentX += TextWidth(myStr);
                }
            }

            // Adds a Carriage return–linefeed 
            if (!noNewLine)
            {
                CurrentY += InternalTextHeight;
                CurrentX = 0;
            }


            //If no caracter can be printed in the next line then a new page is inserted
            if ((CurrentY + InternalTextHeight) >
                ConvertFromPixelsY(ConvertToPixelsY(Height, ScaleModeConstants.VbTwips)))
                NewPage();
        }

        // Cleans the information stored in the bufferPage
        private void ResetBufferPage()
        {
            _bufferPage.Data = null;
            _bufferPage.Circles = null;
            _bufferPage.Images = null;
            _bufferPage.Lines = null;
            _bufferPage.Rectangles = null;
            _bufferPage.Dirty = false;
            _bufferPage.KillDoc = false;
            _bufferPage.EndDoc = false;

            _currentX = 0;
            _currentY = 0;


        }

        /// <summary>
        /// Event raised when the printing process has finished
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PrinterEndPrint(object sender, PrintEventArgs e)
        {
            //    lock (_printerThreadLockObject)
            //     {
            _lastPrintUserCanceled = e.Cancel;
            _printerThreadStarted = false;
            _printerThread = null;
            _pageToPrintEvent.Reset();
            _pagePrintedEvent.Reset();
            _beginNewPage.Reset();
            //    }
        }


        /// <summary>
        ///    Graphics HDC which is only available when the print thread is started
        /// </summary>
        private int currentGraphicsHdc;

        /// <summary>
        /// Print the pages
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PrinterPrintPage(object sender, PrintPageEventArgs e)
        {


            // Capture the HDC for the printer           
            currentGraphicsHdc = e.Graphics.GetHdc().ToInt32();
            SetFontToHdc(_font.Size);
            SetCurrentHdcPenSettings();

            // Signal the creation of the print handle           

            _beginNewPage.Set();



            //Waits for a page to be printed. This will pause the thread
            //until a new page is printed           

            _pageToPrintEvent.WaitOne();


            // Release HDC
            e.Graphics.ReleaseHdc(new IntPtr(currentGraphicsHdc));
            ReleaseCurrentHdcPen();
            currentGraphicsHdc = 0;


            if (_bufferPage.Dirty)
            {
                e.Graphics.PageUnit = GraphicsUnit.Point;
                ScaleModeConstants scaleMode = ScaleModeConstants.VbPoints;

                using (Pen drawPen = new Pen(ForeColor))
                {

                    // Draw all Lines
                    if ((_bufferPage.Lines == null) == false)
                    {

                        for (int i = 0; i < _bufferPage.Lines.Count; i++)
                        {
                            ChangePenSettings(drawPen, _bufferPage.Lines[i].PenInfo);

                            e.Graphics.DrawLine(drawPen,
                            (float)ConvertFromPixelsX(_bufferPage.Lines[i].P1.X, scaleMode),
                            (float)ConvertFromPixelsY(_bufferPage.Lines[i].P1.Y, scaleMode),
                            (float)ConvertFromPixelsX(_bufferPage.Lines[i].P2.X, scaleMode),
                            (float)ConvertFromPixelsY(_bufferPage.Lines[i].P2.Y, scaleMode));
                        }
                    }
                    // Draw all Rectangles
                    if ((_bufferPage.Rectangles == null) == false)
                    {
                        for (int i = 0; i < _bufferPage.Rectangles.Count; i++)
                        {
                            ChangePenSettings(drawPen, _bufferPage.Rectangles[i].PenInfo);
                            e.Graphics.DrawRectangle(drawPen,
                            (float)ConvertFromPixelsX(_bufferPage.Rectangles[i].Rec.X, scaleMode),
                            (float)ConvertFromPixelsY(_bufferPage.Rectangles[i].Rec.Y, scaleMode),
                            (float)ConvertFromPixelsX(_bufferPage.Rectangles[i].Rec.Width, scaleMode),
                            (float)ConvertFromPixelsY(_bufferPage.Rectangles[i].Rec.Height, scaleMode));
                            if (_bufferPage.Rectangles[i].FillStyle == FillStyleConstants.VbFsSolid)
                            {
                                using (SolidBrush brush = new SolidBrush(_bufferPage.Rectangles[i].FillColor))
                                {
                                    e.Graphics.FillRectangle(brush, RectangleFFromPixels(_bufferPage.Rectangles[i].Rec, scaleMode));
                                }
                            }
                        }
                    }
                    // Draw all Images
                    if ((_bufferPage.Images == null) == false)
                    {
                        for (int i = 0; i < _bufferPage.Images.Count; i++)
                        {
                            if (!_bufferPage.Images[i].clippingInfo.Equals(RectangleF.Empty))
                            {
                                RectangleF srcRect = new RectangleF();
                                srcRect.X = (float)ConvertFromPixelsX(_bufferPage.Images[i].P.X, scaleMode);
                                srcRect.Y = (float)ConvertFromPixelsY(_bufferPage.Images[i].P.Y, scaleMode);
                                srcRect.Width = (float)ConvertFromPixelsX(_bufferPage.Images[i].Size.Width, scaleMode);
                                srcRect.Height = (float)ConvertFromPixelsY(_bufferPage.Images[i].Size.Height, scaleMode);
                                e.Graphics.DrawImage(_bufferPage.Images[i].Picture,
                                       srcRect, _bufferPage.Images[i].clippingInfo, GraphicsUnit.Pixel);
                            }
                            else
                            {
                                if (_bufferPage.Images[i].Size.Equals(SizeF.Empty))
                                    e.Graphics.DrawImage(_bufferPage.Images[i].Picture,
                                        new PointF((float)ConvertFromPixelsX(_bufferPage.Images[i].P.X, scaleMode),
                                        (float)ConvertFromPixelsY(_bufferPage.Images[i].P.Y, scaleMode)));
                                else
                                    e.Graphics.DrawImage(_bufferPage.Images[i].Picture,
                                        (float)ConvertFromPixelsX(_bufferPage.Images[i].P.X, scaleMode),
                                        (float)ConvertFromPixelsY(_bufferPage.Images[i].P.Y, scaleMode),
                                        (float)ConvertFromPixelsX(_bufferPage.Images[i].Size.Width, scaleMode),
                                        (float)ConvertFromPixelsY(_bufferPage.Images[i].Size.Height, scaleMode));
                            }
                        }
                    }

                    // Draw all circles
                    if ((_bufferPage.Circles == null) == false)
                    {
                        for (int i = 0; i < _bufferPage.Circles.Count; i++)
                        {
                            //int x = 0;
                            //int y = 0;
                            ChangePenSettings(drawPen, _bufferPage.Circles[i].PenInfo);
                            e.Graphics.DrawEllipse(drawPen, RectangleFFromPixels(_bufferPage.Circles[i].Rec, scaleMode));
                            if (_bufferPage.Circles[i].FillStyle == FillStyleConstants.VbFsSolid)
                            {
                                using (SolidBrush brush = new SolidBrush(_bufferPage.Circles[i].FillColor))
                                {
                                    e.Graphics.FillEllipse(brush, RectangleFFromPixels(_bufferPage.Circles[i].Rec, scaleMode));
                                }
                            }
                        }
                    }
                    if ((_bufferPage.Data == null) == false)
                    {
                        using (SolidBrush brush = new SolidBrush(Color.Black))
                        {
                            for (int i = 0; i < _bufferPage.Data.Count; i++)
                            {
                                brush.Color = _bufferPage.Data[i].Color;
                                e.Graphics.DrawString(_bufferPage.Data[i].Value,
                                       _bufferPage.Data[i].Font,
                                       brush,
                                       (float)ConvertFromPixelsX(_bufferPage.Data[i].X, scaleMode),
                                       (float)ConvertFromPixelsY(_bufferPage.Data[i].Y, scaleMode));
                            }
                        }
                    }
                }
            }


            e.HasMorePages = !_bufferPage.EndDoc;
            e.Cancel = _bufferPage.KillDoc;

            //Signals a page printed

            _pagePrintedEvent.Set();
        }

        private static RectangleF RectangleFFromPixels(RectangleF rectangleF, ScaleModeConstants scaleMode)
        {
            RectangleF result = new RectangleF();
            result.X = (float)ConvertFromPixelsX(rectangleF.X, scaleMode);
            result.Y = (float)ConvertFromPixelsY(rectangleF.Y, scaleMode);
            result.Width = (float)ConvertFromPixelsY(rectangleF.Width, scaleMode);
            result.Height = (float)ConvertFromPixelsY(rectangleF.Height, scaleMode);
            return result;
        }

        private static void ChangePenSettings(Pen drawPen, PenInfo pInfo)
        {
            drawPen.Color = pInfo.Color;
            drawPen.Width = pInfo.Width;
            drawPen.DashStyle = pInfo.Style;
        }

        private void SetCurrentHdcPenSettings()
        {
            ReleaseCurrentHdcPen();
            if (currentGraphicsHdc != 0)
            {
                int newWidth = _drawWidth; //(int)ScaleX(_drawWidth, (int)ScaleModeConstants.VbPixels, (int)ScaleModeConstants.VbTwips);
                this.currentPen = NativeMethods.CreatePen(this._drawStyle, newWidth, (uint)ColorTranslator.ToWin32(_currentColor));
                NativeMethods.SelectObject((IntPtr)currentGraphicsHdc, this.currentPen);
            }
        }

        private void ReleaseCurrentHdcPen()
        {
            if (this.currentPen != IntPtr.Zero)
            {
                NativeMethods.DeleteObject(this.currentPen);
                this.currentPen = IntPtr.Zero;
            }
        }

        private void SetFontToHdc(float pointSize)
        {
            if (currentGraphicsHdc != 0)
            {
                // Set the size of the font to the HDC according to the
                // printer (see http://msdn.microsoft.com/en-us/library/dd183499%28VS.85%29.aspx)
                int capSize = NativeMethods.GetDeviceCaps(new IntPtr(currentGraphicsHdc), 90 /*LOGPIXELSY*/);
                float size = (capSize * pointSize) / 72f;
                Font tmpFont = new Font(_font.FontFamily, size, _font.Style, GraphicsUnit.Pixel);
                NativeMethods.SelectObject(new IntPtr(currentGraphicsHdc), tmpFont.ToHfont());
            }
        }

        #region units handling




        /// <summary>
        /// Converts the parameter to printer units X. 
        /// It is assumed that the parameter is given in the units specified by ScaleMode
        /// </summary>
        /// <param name="num">The number to convert to printer units X</param>
        /// <returns>The parameter converted to printer units given the value of ScaleMode</returns>
        public float ConvertToPrinterUnitsX(double num)
        {

            return ConvertToPrinterUnitsX(num, _scaleMode);
        }

        /// <summary>
        /// Converts the parameter to printer units Y. 
        /// It is assumed that the parameter is given in the units specified by ScaleMode
        /// </summary>
        /// <param name="num">The number to convert to printer units Y</param>
        /// <returns>The parameter converted to printer units given the value of ScaleMode</returns>
        public float ConvertToPrinterUnitsY(double num)
        {
            return ConvertToPrinterUnitsY(num, _scaleMode);
        }

        /// <summary>
        /// Converts the parameter to printer units Y. 
        /// It is assumed that the parameter is given in the units specified by scaleMode
        /// </summary>
        /// <param name="num">The number to convert to printer units Y</param>
        /// <param name="scaleMode">The ScaleMode to use</param>
        /// <returns>The parameter converted to printer units given the value of scaleMode</returns>
        private float ConvertToPrinterUnitsY(double num, ScaleModeConstants scaleMode)
        {
            //The parameter is converted to pixels then to the printer units
            //return ConvertToPixelsY(num, scaleMode) / (DisplayDpi / 100);
            return ConvertToPixelsY(num, scaleMode); // (DisplayDpi / 100);
        }

        /// <summary>
        /// Converts the paratemer to the new numeric system given by ScaleMode.
        /// It is assumed that the parameter is represented in the internal units of the printer
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public float ConvertFromPrinterUnitsX(double num)
        {
            return ConvertFromPrinterUnitsX(num, _scaleMode);
        }

        /// <summary>
        /// Converts the paratemer to the new numeric system given by ScaleMode.
        /// It is assumed that the parameter is represented in the internal units of the printer
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public float ConvertFromPrinterUnitsY(double num)
        {
            //The parameter is converted to pixels then to the internal units given by ScaleMode
            return ConvertFromPrinterUnitsY(num, _scaleMode);
        }

        /// <summary>
        /// Converts the parameter to the new numeric system given by ScaleMode.
        /// It is assumed that the parameter represents pixels X
        /// </summary>
        /// <param name="num">The pixels to convert</param>
        /// <returns>The pixels converted according to the ScaleMode property</returns>
        public float ConvertFromPixelsX(double num)
        {
            return ConvertFromPixelsX(num, _scaleMode);
        }

        /// <summary>
        /// Converts the parameter to the new numeric system given by ScaleMode.
        /// It is assumed that the parameter represents pixels Y
        /// </summary>
        /// <param name="num">The pixels to convert</param>
        /// <returns>The pixels converted according to the ScaleMode property</returns>
        public float ConvertFromPixelsY(double num)
        {
            return ConvertFromPixelsY(num, _scaleMode);
        }

        /// <summary>
        /// Converts the parameter to pixels X. 
        /// It is assumed that the parameter is given in the units specified by ScaleMode
        /// </summary>
        /// <param name="num">The number to convert to pixels X</param>
        /// <returns>The parameter converted to pixels given the value of ScaleMode</returns>
        public float ConvertToPixelsX(double num)
        {
            return ConvertToPixelsX(num, _scaleMode);
        }

        /// <summary>
        /// Converts the parameter to pixels Y. 
        /// It is assumed that the parameter is given in the units specified by ScaleMode
        /// </summary>
        /// <param name="num">The number to convert to pixels Y</param>
        /// <returns>The parameter converted to pixels given the value of ScaleMode</returns>
        public float ConvertToPixelsY(double num)
        {
            return ConvertToPixelsY(num, _scaleMode);
        }

        /// <summary>
        /// Converts the parameter to printer units X. 
        /// It is assumed that the parameter is given in the units specified by scaleMode
        /// </summary>
        /// <param name="num">The number to convert to printer units X</param>
        /// <param name="scaleMode">The ScaleMode to use</param>
        /// <returns>The parameter converted to printer units given the value of scaleMode</returns>
        private float ConvertToPrinterUnitsX(double num, ScaleModeConstants scaleMode)
        {
            //The parameter is converted to pixels then to the printer units
            //return ConvertToPixelsX(num, scaleMode)///(DisplayDpi/100);
            return ConvertToPixelsX(num, scaleMode);
        }

        /*/// <summary>
        /// Converts the parameter to printer units Y. 
        /// It is assumed that the parameter is given in the units specified by scaleMode
        /// </summary>
        /// <param name="num">The number to convert to printer units Y</param>
        /// <param name="scaleMode">The ScaleMode to use</param>
        /// <returns>The parameter converted to printer units given the value of scaleMode</returns>
 */
        /// <summary>
        /// Converts the paratemer to the new numeric system given by scaleMode.
        /// It is assumed that the parameter is represented in the internal units of the printer
        /// </summary>
        /// <param name="num"></param>
        /// <param name="scaleMode"></param>
        /// <returns></returns>
        private static float ConvertFromPrinterUnitsX(double num, ScaleModeConstants scaleMode)
        {
            //The parameter is converted to pixels then to the internal units given by scaleMode
            return ConvertFromPixelsX(num * (DisplayDpi / 100), scaleMode);
        }

        /// <summary>
        /// Converts the paratemer to the new numeric system given by scaleMode.
        /// It is assumed that the parameter is represented in the internal units of the printer
        /// </summary>
        /// <param name="num"></param>
        /// <param name="scaleMode"></param>
        /// <returns></returns>
        private static float ConvertFromPrinterUnitsY(double num, ScaleModeConstants scaleMode)
        {
            return ConvertFromPixelsY(num * (DisplayDpi / 100), scaleMode);
        }

        /// <summary>
        /// Converts the parameter to the new numeric system given by scaleMode.
        /// It is assumed that the parameter represents pixels X
        /// </summary>
        /// <param name="num">The pixels to convert</param>
        /// <param name="scaleMode">The ScaleMode to use</param>
        /// <returns>The pixels converted according to the scaleMode parameter</returns>
        private static float ConvertFromPixelsX(double num, ScaleModeConstants scaleMode)
        {
            float res;
            switch (scaleMode)
            {
                case ScaleModeConstants.VbTwips:
                    res = UpgradeHelpers.SupportHelper.Support.PixelsToTwipsX(num);

                    break;
                case ScaleModeConstants.VbPoints:
                    res = Support.FromPixelsX(num, UpgradeHelpers.SupportHelper.ScaleMode.Points);

                    break;
                case ScaleModeConstants.VbCentimeters:
                    res = Support.FromPixelsX(num, UpgradeHelpers.SupportHelper.ScaleMode.Centimeters);

                    break;
                case ScaleModeConstants.VbCharacters:
                    res = Support.FromPixelsX(num, UpgradeHelpers.SupportHelper.ScaleMode.Characters);
                    break;
                case ScaleModeConstants.VbHimetric:
                    res = Support.FromPixelsX(num, UpgradeHelpers.SupportHelper.ScaleMode.Himetric);
                    break;
                case ScaleModeConstants.VbInches:
                    res = Support.FromPixelsX(num, UpgradeHelpers.SupportHelper.ScaleMode.Inches);

                    break;
                case ScaleModeConstants.VbMilimeters:
                    res = Support.FromPixelsX(num, UpgradeHelpers.SupportHelper.ScaleMode.Millimeters);

                    break;
                default:
                    res = (float)num;
                    break;
            }

            return (float)res;
        }

        /// <summary>
        /// Converts the parameter to the new numeric system given by scaleMode.
        /// It is assumed that the parameter represents pixels Y
        /// </summary>
        /// <param name="num">The pixels to convert</param>
        /// <param name="scaleMode">The ScaleMode to use</param>
        /// <returns>The pixels converted according to the scaleMode parameter</returns>
        private static float ConvertFromPixelsY(double num, ScaleModeConstants scaleMode)
        {
            float res;
            switch (scaleMode)
            {
                case ScaleModeConstants.VbTwips:
                    res = Support.PixelsToTwipsY(num);

                    break;
                case ScaleModeConstants.VbPoints:
                    res = Support.FromPixelsY(num, UpgradeHelpers.SupportHelper.ScaleMode.Points);

                    break;
                case ScaleModeConstants.VbCentimeters:
                    res = Support.FromPixelsY(num, UpgradeHelpers.SupportHelper.ScaleMode.Centimeters);

                    break;
                case ScaleModeConstants.VbCharacters:
                    res = Support.FromPixelsY(num, UpgradeHelpers.SupportHelper.ScaleMode.Characters);

                    break;
                case ScaleModeConstants.VbHimetric:
                    res = Support.FromPixelsY(num, UpgradeHelpers.SupportHelper.ScaleMode.Himetric);

                    break;
                case ScaleModeConstants.VbInches:
                    res = Support.FromPixelsY(num, UpgradeHelpers.SupportHelper.ScaleMode.Inches);

                    break;
                case ScaleModeConstants.VbMilimeters:
                    res = Support.FromPixelsY(num, UpgradeHelpers.SupportHelper.ScaleMode.Millimeters);

                    break;
                default:
                    res = (float)num;
                    break;
            }

            return res;
        }

        /// <summary>
        /// Converts the parameter to pixels X. 
        /// It is assumed that the parameter is given in the units specified by scaleMode
        /// </summary>
        /// <param name="num">The number to convert to pixels X</param>
        /// <param name="scaleMode">The ScaleMode to use</param>
        /// <returns>The parameter converted to pixels given the value of scaleMode</returns>
        private float ConvertToPixelsX(double num, ScaleModeConstants scaleMode)
        {
            float res;
            switch (scaleMode)
            {
                case ScaleModeConstants.VbTwips:
                    res = Support.TwipsToPixelsX(num);

                    break;
                case ScaleModeConstants.VbPoints:
                    res = Support.ToPixelsX(num, UpgradeHelpers.SupportHelper.ScaleMode.Points);

                    break;
                case ScaleModeConstants.VbCentimeters:
                    res = Support.ToPixelsX(num, UpgradeHelpers.SupportHelper.ScaleMode.Centimeters);

                    break;
                case ScaleModeConstants.VbCharacters:
                    res = Support.ToPixelsX(num, UpgradeHelpers.SupportHelper.ScaleMode.Characters);
                    break;
                case ScaleModeConstants.VbHimetric:
                    res = Support.ToPixelsX(num, UpgradeHelpers.SupportHelper.ScaleMode.Himetric);
                    break;
                case ScaleModeConstants.VbInches:
                    res = Support.ToPixelsX(num, UpgradeHelpers.SupportHelper.ScaleMode.Inches);

                    break;
                case ScaleModeConstants.VbMilimeters:
                    res = Support.ToPixelsX(num, UpgradeHelpers.SupportHelper.ScaleMode.Millimeters);

                    break;
                case ScaleModeConstants.VbPixels:
                    res = (this._innerPrinter.DefaultPageSettings.Landscape ?
                           PrinterGraphics.VisibleClipBounds.Height :
                           PrinterGraphics.VisibleClipBounds.Width)
                       * ((float)num / ScaleWidth);
                    break;
                default:
                    res = (float)num;
                    break;
            }

            return res;
        }

        /// <summary>
        /// Converts the parameter to pixels Y. 
        /// It is assumed that the parameter is given in the units specified by scaleMode
        /// </summary>
        /// <param name="num">The number to convert to pixels Y</param>
        /// <param name="scaleMode">The ScaleMode to use</param>
        /// <returns>The parameter converted to pixels given the value of scaleMode</returns>
        private float ConvertToPixelsY(double num, ScaleModeConstants scaleMode)
        {
            float res;
            switch (scaleMode)
            {
                case ScaleModeConstants.VbTwips:
                    res = Support.TwipsToPixelsY(num);

                    break;
                case ScaleModeConstants.VbPoints:
                    res = Support.ToPixelsY(num, UpgradeHelpers.SupportHelper.ScaleMode.Points);

                    break;
                case ScaleModeConstants.VbCentimeters:
                    res = Support.ToPixelsY(num, UpgradeHelpers.SupportHelper.ScaleMode.Centimeters);

                    break;
                case ScaleModeConstants.VbCharacters:
                    res = Support.ToPixelsY(num, UpgradeHelpers.SupportHelper.ScaleMode.Characters);
                    break;
                case ScaleModeConstants.VbHimetric:
                    res = Support.ToPixelsY(num, UpgradeHelpers.SupportHelper.ScaleMode.Himetric);
                    break;
                case ScaleModeConstants.VbInches:
                    res = Support.ToPixelsY(num, UpgradeHelpers.SupportHelper.ScaleMode.Inches);

                    break;
                case ScaleModeConstants.VbMilimeters:
                    res = Support.ToPixelsY(num, UpgradeHelpers.SupportHelper.ScaleMode.Millimeters);

                    break;
                case ScaleModeConstants.VbPixels:
                    res =
                       (this._innerPrinter.DefaultPageSettings.Landscape ?
                             PrinterGraphics.VisibleClipBounds.Width :
                             PrinterGraphics.VisibleClipBounds.Height)
                        * ((float)num / ScaleHeight);
                    break;
                default:
                    res = (float)num;
                    break;
            }

            return res;
        }

        #endregion

        /// <summary>
        /// Immediately terminates the current print job.
        /// </summary>
        public void KillDoc()
        {
            //Current page is marked to kill the printing process
            _bufferPage.KillDoc = true;
            try
            {
                SendPageToPrint();
            }
            finally
            {
                //Returns the PaperSize to the default value after printing
                _lastCustomHeight = _lastCustomWidth = 0;
                PaperSize = (int)_innerPrinter.PrinterSettings.DefaultPageSettings.PaperSize.Kind;

                ResetBufferPage();
                _pageIndex = 0;
            }
        }

        #region Nested type: DataInfo

        private struct DataInfo
        {
            public Color Color;
            public Font Font;
            public string Value;
            public float X;
            public float Y;
        }

        #endregion

        #region Nested type: DrawModeConstants

        private enum DrawModeConstants
        {
            VbBlackness = 1,
            VbNotMergePen = 2,
            VbMaskNotPen = 3,
            VbNotCopyPen = 4,
            VbMaskPenNot = 5,
            VbInvert = 6,
            VbXorPen = 7,
            VbNotMaskPen = 8,
            VbMaskPen = 9,
            VbNotXorPen = 10,
            VbNop = 11,
            VbMergeNotPen = 12,
            VbCopyPen = 13,
            VbMergePenNot = 14,
            VbMergePen = 15,
            VbWhiteness = 16
        }

        #endregion

        #region Nested type: FillStyleConstants

        private enum FillStyleConstants : short
        {
            VbFsSolid = 0,
            VbFsTransparent = 1
        }

        #endregion


        #region Nested type: PenInfo

        private struct PenInfo
        {
            public int Width;
            public DashStyle Style;
            public Color Color;
            public PenInfo(int width, DashStyle style, Color color)
            {
                this.Width = width;
                this.Style = style;
                this.Color = color;
            }

            public PenInfo(int width, int style, Color color) :
                this(width, (DashStyle)style, color)
            {

            }

        }

        #endregion

        #region Nested type: ImageInfo

        private struct ImageInfo
        {
            public PointF P;
            public Image Picture;
            public SizeF Size;
            public bool hasClippingInfo;
            public RectangleF clippingInfo;
        }

        #endregion

        #region Nested type: LineInfo

        private struct LineInfo
        {
            public PointF P1;
            public PointF P2;
            //public Pen Pen;
            public PenInfo PenInfo;
        }

        #endregion

        #region Nested type: PageInfo

        private struct PageInfo
        {
            private List<RectangleInfo> _circles;
            private List<DataInfo> _data;
            private bool _dirty;
            private bool _endDoc;
            private List<ImageInfo> _images;

            private bool _killDoc;
            private List<LineInfo> _lines;
            private List<RectangleInfo> _rectangles;

            public bool EndDoc
            {
                get { return _endDoc; }
                set { _endDoc = value; }
            }

            public bool KillDoc
            {
                get { return _killDoc; }
                set { _killDoc = value; }
            }

            public bool Dirty
            {
                get { return _dirty; }
                set { _dirty = value; }
            }

            public List<DataInfo> Data
            {
                get { return _data; }
                set
                {
                    Dirty = true;
                    _data = value;
                }
            }

            public List<LineInfo> Lines
            {
                get { return _lines; }
                set
                {
                    _lines = value;
                }
            }

            public List<RectangleInfo> Circles
            {
                get { return _circles; }
                set
                {
                    _circles = value;
                }
            }

            public List<RectangleInfo> Rectangles
            {
                get { return _rectangles; }
                set { _rectangles = value; }
            }

            public List<ImageInfo> Images
            {
                get { return _images; }
                set
                {
                    Dirty = true;
                    _images = value;
                }
            }

            public void AddImageINfo(ImageInfo imageInformation)
            {
                Dirty = true;
                if (this._images == null)
                {
                    this._images = new List<ImageInfo>();
                }
                this._images.Add(imageInformation);
            }

            public void AddDataInfo(DataInfo coodinates)
            {
                Dirty = true;
                if (this._data == null)
                {
                    this._data = new List<DataInfo>();
                }
                this._data.Add(coodinates);
            }

            public void AddLine(LineInfo coordinateInfo)
            {
                Dirty = true;
                if (this._lines == null)
                {
                    this._lines = new List<LineInfo>();
                }
                this._lines.Add(coordinateInfo);
            }
            public void AddRectangle(RectangleInfo coordinateInfo)
            {
                Dirty = true;
                if (this._rectangles == null)
                {
                    this._rectangles = new List<RectangleInfo>();
                }
                this._rectangles.Add(coordinateInfo);
            }

            public void AddCircle(RectangleInfo coordinateInfo)
            {
                Dirty = true;
                if (this._circles == null)
                {
                    this._circles = new List<RectangleInfo>();
                }
                this._circles.Add(coordinateInfo);
            }
        }

        #endregion

        #region Nested type: PrinterObjectConstants

        public static class PrinterObjectConstants
        {
            //PRBN
            public static readonly short VbPRBNAuto = 7;
            public static readonly short VbPRBNCassette = 14;
            public static readonly short VbPRBNEnvelope = 5;
            public static readonly short VbPRBNEnvManual = 6;
            public static readonly short VbPRBNLargeCapacity = 11;
            public static readonly short VbPRBNLargeFmt = 10;
            public static readonly short VbPRBNLower = 2;
            public static readonly short VbPRBNManual = 4;
            public static readonly short VbPRBNMiddle = 3;
            public static readonly short VbPRBNSmallFmt = 9;
            public static readonly short VbPRBNTractor = 8;
            public static readonly short VbPRBNUpper = 1;
            public static readonly short VbPRCMColor = 2;
            public static readonly short VbPRCMMonochrome = 1;
            //PRDP
            public static readonly short VbPRDPHorizontal = 2;
            public static readonly short VbPRDPSimplex = 1;
            public static readonly short VbPRDPVertical = 3;
            //PROR
            public static readonly short VbPRORLandscape = 2;
            public static readonly short VbPRORPortrait = 1;
            //PRPQ 
            public static readonly short VbPRPQDraft = -1;
            public static readonly short VbPRPQHigh = -4;
            public static readonly short VbPRPQLow = -2;
            public static readonly short VbPRPQMedium = -3;
            public static readonly short VbPRPS10X14 = 16;
            public static readonly short VbPRPS11X17 = 17;
            public static readonly short VbPRPSA3 = 8;
            public static readonly short VbPRPSA4 = 9;
            public static readonly short VbPRPSA4Small = 10;
            public static readonly short VbPRPSA5 = 11;
            public static readonly short VbPRPSB4 = 12;
            public static readonly short VbPRPSB5 = 13;
            public static readonly short VbPRPSCSheet = 24;
            public static readonly short VbPRPSDSheet = 25;
            public static readonly short VbPRPSEnv10 = 20;
            public static readonly short VbPRPSEnv11 = 21;
            public static readonly short VbPRPSEnv12 = 22;
            public static readonly short VbPRPSEnv14 = 23;
            public static readonly short VbPRPSEnv9 = 19;
            public static readonly short VbPRPSEnvB4 = 33;
            public static readonly short VbPRPSEnvB5 = 34;
            public static readonly short VbPRPSEnvB6 = 35;
            public static readonly short VbPRPSEnvC3 = 29;
            public static readonly short VbPRPSEnvC4 = 30;
            public static readonly short VbPRPSEnvC5 = 28;
            public static readonly short VbPRPSEnvC6 = 31;
            public static readonly short VbPRPSEnvC65 = 32;
            public static readonly short VbPRPSEnvDL = 27;
            public static readonly short VbPRPSEnvItaly = 36;
            public static readonly short VbPRPSEnvMonarch = 37;
            public static readonly short VbPRPSEnvPersonal = 38;
            public static readonly short VbPRPSESheet = 26;
            public static readonly short VbPRPSExecutive = 7;
            public static readonly short VbPRPSFanfoldLglGerman = 41;
            public static readonly short VbPRPSFanfoldStdGerman = 40;
            public static readonly short VbPRPSFanfoldUs = 39;
            public static readonly short VbPRPSFolio = 14;
            public static readonly short VbPRPSLedger = 4;
            public static readonly short VbPRPSLegal = 5;
            public static readonly short VbPRPSLetter = 1;
            public static readonly short VbPRPSLetterSmall = 2;
            public static readonly short VbPRPSNote = 18;
            public static readonly short VbPRPSQuarto = 15;
            public static readonly short VbPRPSStatement = 6;
            public static readonly short VbPRPSTabloid = 3;
            public static readonly short VbPRPSUser = 256;
        }

        #endregion

        #region Nested type: RectangleInfo

        private struct RectangleInfo
        {
            public Color FillColor;
            public FillStyleConstants FillStyle;
            //public Pen Pen;
            public PenInfo PenInfo;
            public RectangleF Rec;
        }

        #endregion

        #region Nested type: ScaleModeConstants

        public enum ScaleModeConstants : short
        {
            VbCentimeters = 7,
            VbCharacters = 4,
            VbContainerPosition = 9,
            VbContainerSize = 10,
            VbHimetric = 8,
            VbInches = 5,
            VbMilimeters = 6,
            VbPixels = 3,
            VbPoints = 2,
            VbTwips = 1,
            VbUser = 0
        }

        #endregion

        #region Nested type: StdFont

        public class StdFont
        {
            //******************************************************************************************************
            //These are the variables if the font class of VB6
            public bool Bold;
            public int CharSet;
            public bool Italic;
            public string Name = "Arial";
            public float Size = 8.28f;
            public bool StrikeThrough;
            public bool UnderLine;
            public int Weight = 400;
            // Convert the Font Type of VB6 to FontStyle in .NET
            private FontStyle GetStyle
            {
                get
                {
                    FontStyle auxStyle = 0;
                    if (Bold)
                    {
                        auxStyle = auxStyle | FontStyle.Bold;
                    }
                    if (Italic)
                    {
                        auxStyle = auxStyle | FontStyle.Italic;
                    }
                    if (StrikeThrough)
                    {
                        auxStyle = auxStyle | FontStyle.Strikeout;
                    }
                    if (UnderLine)
                    {
                        auxStyle = auxStyle | FontStyle.Underline;
                    }
                    return auxStyle;
                }
            }

            //Converts this Font to a .NET Font
            public Font NetFont
            {
                get { return new Font(Name, (float)(Size), GetStyle); }
            }

            public StdFont Clone()
            {
                StdFont tempFont = new StdFont();
                tempFont.Bold = Bold;
                tempFont.Italic = Italic;
                tempFont.Name = Name;
                tempFont.Size = Size;
                tempFont.StrikeThrough = StrikeThrough;
                tempFont.UnderLine = UnderLine;
                tempFont.CharSet = CharSet;
                tempFont.Weight = Weight;

                return tempFont;
            }
        }

        #endregion

        public string OutputFileName
        {
            get
            {
                return "";
            }
            set
            {
                _innerPrinter.PrinterSettings.PrintToFile = true;
                _innerPrinter.PrinterSettings.PrintFileName = value;
            }
        }

        #region GDI imports

        internal static class NativeMethods
        {
            [DllImport("gdi32.dll")]
            internal static extern int GetMapMode(IntPtr hdc);

            [DllImport("gdi32.dll", ExactSpelling = true, PreserveSig = true, SetLastError = true)]
            internal static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);


            [DllImport("gdi32.dll")]
            internal static extern uint SetBkMode(IntPtr hdc, int crColor);

            [DllImport("gdi32.dll")]
            internal static extern uint SetBkColor(IntPtr hdc, int crColor);

            [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
            internal static extern bool TextOut(IntPtr hdc, int nXStart, int nYStart, string lpString, int cbString);

            [DllImport("gdi32.dll")]
            internal static extern IntPtr GetStockObject(int fnObject);

            [DllImport("gdi32.dll")]
            internal static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

            [DllImport("gdi32.dll")]
            internal static extern IntPtr CreatePen(int fnPenStyle, int nWidth, uint crColor);

            [DllImport("gdi32.dll")]
            internal static extern bool DeleteObject(IntPtr hObject);
        }




        #endregion

        #region "Dispose"

        ~PrinterHelper()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (_pageToPrintEvent != null)
                    {
                        _pageToPrintEvent.Close();
                    }
                    if (_pagePrintedEvent != null)
                    {
                        _pagePrintedEvent.Close();
                    }
                    if (_beginNewPage != null)
                    {
                        _beginNewPage.Close();
                    }
                    if (_innerPrinter != null)
                    {
                        _innerPrinter.Dispose();
                    }
                    if (_font != null)
                    {
                        _font.Dispose();
                    }
                }
            }
            disposed = true;
        }

        #endregion

    }
}