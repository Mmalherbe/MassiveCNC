using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using UpgradeHelpers.Helpers;


namespace UpgradeHelpers.Gui
{

    /// <summary>
    /// Implements several contol-related functionalities which were present in VB6 and are not in .NET.
    /// </summary>
    public static partial class ControlHelper
    {

        internal static class ControlHelperNativeMethods
        {
            /// <summary>
            /// External API to Get Window Rect from user32.dll
            /// </summary>
            /// <param name="hWnd">handler pointer</param>
            /// <param name="rect">RECT structure output</param>
            /// <returns>true if successful, false otherwise.</returns>
            [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
            internal static extern bool GetWindowRect(HandleRef hWnd, [In, Out] ref RECT rect);

            /// <summary>
            /// External API to Get Window from user32.dll
            /// </summary>
            /// <param name="hWnd">handler to get</param>
            /// <param name="uCmd">int cmd</param>
            /// <returns>Window handle.</returns>
            [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
            internal static extern IntPtr GetWindow(HandleRef hWnd, int uCmd);

            /// <summary>
            /// External API to get if Is Window Visible from user32.dll
            /// </summary>
            /// <param name="hWnd">window handler</param>
            /// <returns>returs true if is visible</returns>
            [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
            internal static extern bool IsWindowVisible(HandleRef hWnd);
        }

        /// <summary>
        /// Internal RECT structure.
        /// </summary>
        internal struct RECT
        {
            /// <summary>
            /// left value
            /// </summary>
            public int left;
            /// <summary>
            /// top value
            /// </summary>
            public int top;
            /// <summary>
            /// right value
            /// </summary>
            public int right;
            /// <summary>
            /// bottom value
            /// </summary>
            public int bottom;
            /// <summary>
            /// Constructor, set the left,top,right,bottom values
            /// </summary>
            /// <param name="left">left position value</param>
            /// <param name="top">top position value</param>
            /// <param name="right">right position value</param>
            /// <param name="bottom">bottom position value</param>
            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }
            /// <summary>
            /// Constructor using the a Rectangle values
            /// </summary>
            /// <param name="r">Rectangle variable to get the position values</param>
            public RECT(Rectangle r)
            {
                this.left = r.Left;
                this.top = r.Top;
                this.right = r.Right;
                this.bottom = r.Bottom;
            }
            /// <summary>
            /// Returns a RECT structure from a x, y position and width, height values
            /// </summary>
            /// <param name="x">x position</param>
            /// <param name="y">y position</param>
            /// <param name="width">width value</param>
            /// <param name="height">height value</param>
            /// <returns>The RECT structure.</returns>
            public static RECT FromXYWH(int x, int y, int width, int height)
            {
                return new RECT(x, y, x + width, y + height);
            }
            /// <summary>
            /// Gets the Size structure from internal values
            /// </summary>
            public Size Size
            {
                get
                {
                    return new Size(this.right - this.left, this.bottom - this.top);
                }
            }
        }



        /// <summary>
        /// This hash has a map of control to ControlGraphics structures.
        /// </summary>
        private static WeakDictionary<Control, ControlGraphics> printHash = new WeakDictionary<Control, ControlGraphics>();

        /// <summary>
        /// Sets DrawWidth extended property.
        /// </summary>
        /// <param name="mControl">The control whose DrawWidth will be set.</param>
        /// <param name="val">The new DrawWidth value.</param>
        public static void setDrawWidth(this Control mControl, int val)
        {
            ControlGraphics fg;
            fg = AddToHash(mControl);
            fg.DrawWidth = val;
        }

        /// <summary>
        /// Sets DrawWidth extended property.
        /// </summary>
        /// <param name="mControl">The control whose DrawWidth will be set.</param>
        /// <param name="val">The new DrawWidth value.</param>
        public static void setDrawWidth(this Control mControl, double val)
        {
            setDrawWidth(mControl, (int)val);
        }


        /// <summary>
        /// Obtains the DrawWidth value for a given control.
        /// </summary>
        /// <param name="mControl">The control whose DrawWidth value will be obtained.</param>
        /// <returns>The DrawWidth value for the given control.</returns>
        public static int getDrawWidth(this Control mControl)
        {
            ControlGraphics fg;
            fg = AddToHash(mControl);

            return fg.DrawWidth;
        }

        /// <summary>
        /// Sets CurrentX extended property.
        /// </summary>
        /// <param name="mControl">The control whose CurrentX will be set.</param>
        /// <param name="val">The new CurrentX value.</param>
        public static void setCurrentX(Control mControl, int val)
        {
            ControlGraphics fg;
            fg = AddToHash(mControl);
            fg.CurrentX = val;
        }

        /// <summary>
        /// Sets CurrentX extended property.
        /// </summary>
        /// <param name="mControl">The control whose CurrentX will be set.</param>
        /// <param name="val">The new CurrentX value.</param>
        public static void setCurrentX(this Control mControl, double val)
        {
            setCurrentX(mControl, (int)val);
        }

        /// <summary>
        /// Obtains the CurrentX value for a given control.
        /// </summary>
        /// <param name="mControl">The control whose CurrentX value will be obtained.</param>
        /// <returns>The CurrentX value for the given control.</returns>
        public static int getCurrentX(this Control mControl)
        {
            ControlGraphics fg;
            fg = AddToHash(mControl);
            return (int)fg.CurrentX;
        }

        /// <summary>
        /// Sets CurrentY extended property.
        /// </summary>
        /// <param name="mControl">The control whose CurrentY will be set.</param>
        /// <param name="val">The new CurrentY value.</param>
        public static void setCurrentY(Control mControl, double val)
        {
            setCurrentY(mControl, (int)val);
        }

        /// <summary>
        /// Sets CurrentY extended property.
        /// </summary>
        /// <param name="mControl">The control whose CurrentY will be set.</param>
        /// <param name="val">The new CurrentY value.</param>
        public static void setCurrentY(this Control mControl, float val)
        {
            ControlGraphics fg;
            fg = AddToHash(mControl);
            fg.CurrentY = val;
        }

        /// <summary>
        /// Obtains the CurrentY value for a given control.
        /// </summary>
        /// <param name="mControl">The control whose CurrentY value will be obtained.</param>
        /// <returns>The CurrentY value for the given control.</returns>
        public static int getCurrentY(this Control mControl)
        {
            ControlGraphics fg;
            fg = AddToHash(mControl);
            return (int)fg.CurrentY;
        }

        /// <summary>
        /// Prints the given parameters inside the specified control.
        /// </summary>
        /// <param name="mControl">The control to print in.</param>
        /// <param name="parameters">The elements to be printed.</param>
        public static void Print(this Control mControl, params object[] parameters)
        {
            ControlGraphics fg;
            fg = AddToHash(mControl);
            fg.Print(parameters);
        }

        /// <summary>
        /// Draws a line inside the given control with the specified parameters.
        /// </summary>
        /// <param name="mControl">The control to print in.</param>
        /// <param name="x1">The x value for the starting point.</param>
        /// <param name="y1">The y value for the starting point.</param>
        /// <param name="x2">The x value for the ending point.</param>
        /// <param name="y2">The y value for the ending point.</param>
        /// <param name="olecolor">The desired line color</param>
        public static void Line(Control mControl, int x1, int y1, int x2, int y2, int olecolor)
        {
            Line(mControl, x1, y1, x2, y2, ColorTranslator.FromOle(olecolor));
        }

        /// <summary>
        /// Draws a line inside the given control with the specified parameters.
        /// </summary>
        /// <param name="mControl">The control to print in.</param>
        /// <param name="x1">The x value for the starting point.</param>
        /// <param name="y1">The y value for the starting point.</param>
        /// <param name="x2">The x value for the ending point.</param>
        /// <param name="y2">The y value for the ending point.</param>
        /// <param name="olecolor">The desired line color.</param>
        public static void Line(Control mControl, double x1, double y1, double x2, double y2, int olecolor)
        {
            Line(mControl, (int)x1, (int)y1, (int)x2, (int)y2, ColorTranslator.FromOle(olecolor));
        }

        /// <summary>
        /// Draws a line inside the given control with the specified parameters.
        /// </summary>
        /// <param name="mControl">The control to print in.</param>
        /// <param name="x1">The x value for the starting point.</param>
        /// <param name="y1">The y value for the starting point.</param>
        /// <param name="x2">The x value for the ending point.</param>
        /// <param name="y2">The y value for the ending point.</param>
        /// <param name="color">The desired line color.</param>
        public static void Line(Control mControl, int x1, int y1, int x2, int y2, Color color)
        {
            ControlGraphics fg;
            fg = AddToHash(mControl);
            fg.Line(x1, y1, x2, y2, color);
        }

        /// <summary>
        /// Draws a circle inside the given control with the specified parameters.
        /// </summary>
        /// <param name="mControl">The control to print in.</param>
        /// <param name="x">The x value for the center point.</param>
        /// <param name="y">The y value for the center point.</param>
        /// <param name="radius">The circle radius value.</param>
        /// <param name="olecolor">The desired circle color.</param>
        public static void Circle(Control mControl, int x, int y, double radius, int olecolor)
        {
            Circle(mControl, x, y, radius, ColorTranslator.FromOle(olecolor));
        }

        /// <summary>
        /// Draws a circle inside the given control with the specified parameters.
        /// </summary>
        /// <param name="mControl">The control to print in.</param>
        /// <param name="x">The x value for the center point.</param>
        /// <param name="y">The y value for the center point.</param>
        /// <param name="radius">The circle radius value.</param>
        /// <param name="olecolor">The desired circle color.</param>
        public static void Circle(Control mControl, double x, double y, double radius, int olecolor)
        {
            Circle(mControl, (int)x, (int)y, radius, olecolor);
        }

        /// <summary>
        /// Draws a circle inside the given control with the specified parameters.
        /// </summary>
        /// <param name="mControl">The control to print in.</param>
        /// <param name="x">The x value for the center point.</param>
        /// <param name="y">The y value for the center point.</param>
        /// <param name="radius">The circle radius value.</param>
        /// <param name="color">The desired circle color.</param>
        public static void Circle(Control mControl, int x, int y, double radius, Color color)
        {
            ControlGraphics fg;
            fg = AddToHash(mControl);
            fg.Circle(x, y, radius, color);
        }

        /// <summary>
        /// Clears the graphics for the given control.
        /// </summary>
        /// <param name="mControl">The control to be cleared.</param>
        public static void Cls(this Control mControl)
        {
            ControlGraphics fg;
            fg = AddToHash(mControl);
            fg.Cls();
        }

        /// <summary>
        /// Function created to return the TextHeight of a control.
        /// Use this function for controls, when TextHeight applies to the print object use 
        /// PrinterHelper.TextHeight instead.
        /// </summary>
        /// <param name="con">The control.</param>
        /// <param name="str">The string to use in the calculus.</param>
        /// <returns>The text height required to print the str in the control.</returns>
        public static float TextHeight(Control con, string str)
        {
            System.Drawing.Graphics mesur = null;
            System.Drawing.SizeF size = new System.Drawing.SizeF();
            mesur = con.CreateGraphics();
            size = mesur.MeasureString(str, con.Font);
            return size.Height;
        }

        /// <summary>
        /// Function created to return the TextWidth of a control.
        /// Use this function for controls, when TextWidth applies to the print object use 
        /// PrinterHelper.TextWidth instead.
        /// </summary>
        /// <param name="con">The control.</param>
        /// <param name="str">The string to use in the calculus.</param>
        /// <returns>The text width required to print the str in the control.</returns>
        public static float TextWidth(Control con, string str)
        {
            System.Drawing.SizeF size = new System.Drawing.SizeF();
            using (System.Drawing.Graphics mesur = con.CreateGraphics())
            {
                size = mesur.MeasureString(str, con.Font);
            }
            return size.Width * 15;
        }

        /// <summary>
        /// Support method to return the Enabled state of a control for special cases like 
        /// when a "ForEach control in Form.Control" is used.
        /// </summary>
        /// <param name="ctrl">The source control.</param>
        /// <returns>The state of the control.</returns>
        public static bool GetEnabled(Control ctrl)
        {
            if (ctrl is AxHost)
                return ((AxHost)ctrl).Enabled;

            if (ctrl is ContainerHelper.MenuItemControl)
                return ((ContainerHelper.MenuItemControl)ctrl).Enabled;

            //fsaborio. Correccion para invocar el metodo enable correcto (casos donde se sobreescribio como new Enabled)
            return Convert.ToBoolean(UpgradeHelpers.Helpers.ReflectionHelper.GetMember(ctrl, "Enabled"));
        }

        /// <summary>
        /// Support method to set the Enabled state of a control for special cases like 
        /// when a "ForEach control in Form.Control" is used.
        /// </summary>
        /// <param name="ctrl">The source control.</param>
        /// <param name="value">set the boolean value to Enabled property</param>
        public static void SetEnabled(this Control ctrl, bool value)
        {
            if (ctrl is AxHost)
            {
                ((AxHost)ctrl).Enabled = value;
            }
            else if (ctrl is ContainerHelper.MenuItemControl)
                ((ContainerHelper.MenuItemControl)ctrl).Enabled = value;
            else
                //fsaborio. Correccion para invocar el metodo enable correcto (casos donde se sobreescribio como new Enabled)
                UpgradeHelpers.Helpers.ReflectionHelper.SetMember(ctrl, "Enabled", value);
        }

        /// <summary>
        /// Support method to return the Visible state of a control for special cases like 
        /// when a "ForEach control in Form.Control" is used.
        /// </summary>
        /// <param name="ctrl">The source control.</param>
        /// <returns>The state of the control.</returns>
        public static bool GetVisible(this Control ctrl)
        {
            if (ctrl is AxHost)
                return ((AxHost)ctrl).Visible;

            if (ctrl is ContainerHelper.MenuItemControl)
                return ((ContainerHelper.MenuItemControl)ctrl).Visible;

            return ctrl.Visible;
        }
        /// <summary>
        /// Support method to set the Visible state of a control for special cases like 
        /// when a "ForEach control in Form.Control" is used.
        /// </summary>
        /// <param name="ctrl">The source control.</param>
        /// <param name="value">set the Visible property to the control</param>
        public static void SetVisible(Control ctrl, bool value)
        {
            if (ctrl is AxHost)
                ((AxHost)ctrl).Visible = value;
            else if (ctrl is ContainerHelper.MenuItemControl)
                ((ContainerHelper.MenuItemControl)ctrl).Visible = value;
            else
                ctrl.Visible = value;
        }

        /// <summary>
        /// Support method to return the Tag state of a control for special cases like 
        /// when a "ForEach control in Form.Control" is used.
        /// </summary>
        /// <param name="ctrl">The source control.</param>
        /// <returns>The state of the control.</returns>
        public static string GetTag(this Control ctrl)
        {
            if (ctrl is AxHost)
                return Convert.ToString(((AxHost)ctrl).Tag);

            if (ctrl is ContainerHelper.MenuItemControl)
                return Convert.ToString(((ContainerHelper.MenuItemControl)ctrl).Tag);

            return Convert.ToString(ctrl.Tag);
        }
        /// <summary>
        /// Support method to set the Tag state of a control for special cases like 
        /// when a "ForEach control in Form.Control" is used.
        /// </summary>
        /// <param name="ctrl">The source control.</param>
        /// <param name="value">set the Tag value to the control</param>
        public static void SetTag(this Control ctrl, string value)
        {
            if (ctrl is AxHost)
                ((AxHost)ctrl).Tag = value;

            if (ctrl is ContainerHelper.MenuItemControl)
                ((ContainerHelper.MenuItemControl)ctrl).Tag = value;

            ctrl.Tag = value;
        }

        /// <summary>
        /// Returns true if the control is not completely visible given a window that 
        /// its partially or completely hiding it.
        /// </summary>
        /// <param name="ctrl">The source control.</param>
        /// <returns>True if the control is partially or completely hidden by a window.</returns>
        public static bool IsControlPartiallyObscured(this Control ctrl)
        {
            Graphics g = ctrl.CreateGraphics();
            Region controlRegion = null;
            Region notObscuredControlRegion = null;

            GetVisibilityRegionsForControl(ctrl, out controlRegion, out notObscuredControlRegion);
            return !controlRegion.IsEmpty(g) && !controlRegion.Equals(notObscuredControlRegion, g);
        }

        /// <summary>
        /// Returns true if another window is completely hidding this control.
        /// </summary>
        /// <param name="ctrl">The source control.</param>
        /// <returns>True if the control is hidden by a window.</returns>
        public static bool IsControlObscured(this Control ctrl)
        {
            Graphics g = ctrl.CreateGraphics();
            Region controlRegion = null;
            Region notObscuredControlRegion = null;

            GetVisibilityRegionsForControl(ctrl, out controlRegion, out notObscuredControlRegion);
            return notObscuredControlRegion.IsEmpty(g);
        }

        /// <summary>
        /// Given a control returns the region of the control, also it returs the region of the control 
        /// that is not obscured by another window.
        /// </summary>
        /// <param name="ctrl">The sources control.</param>
        /// <param name="controlRegion">The region of the control.</param>
        /// <param name="notObscuredControlRegion">The region not obscured by another windows.</param>
        private static void GetVisibilityRegionsForControl(Control ctrl, out Region controlRegion, out Region notObscuredControlRegion)
        {
            Control parentInternal = null;
            Control parentInternalParent = null;

            if (!ctrl.IsHandleCreated || !ctrl.Visible)
            {
                controlRegion = new Region();
                controlRegion.MakeEmpty();
                notObscuredControlRegion = new Region();
                notObscuredControlRegion.MakeEmpty();
                return;
            }

            RECT rect = new RECT();
            parentInternal = ctrl.GetType().GetProperty("ParentInternal", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(ctrl, null) as Control;
            if (parentInternal != null)
            {
                parentInternalParent = parentInternal.GetType().GetProperty("ParentInternal", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(parentInternal, null) as Control;
                while (parentInternalParent != null)
                {
                    parentInternal = parentInternalParent;
                    parentInternalParent = parentInternal.GetType().GetProperty("ParentInternal", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(parentInternal, null) as Control;
                }
            }
            ControlHelperNativeMethods.GetWindowRect(new HandleRef(ctrl, ctrl.Handle), ref rect);
            controlRegion = new Region(Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom));
            notObscuredControlRegion = new Region(Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom));

            IntPtr ptr2;
            IntPtr handle;
            if (parentInternal != null)
                handle = parentInternal.Handle;
            else
                handle = ctrl.Handle;

            for (IntPtr ptr = handle; (ptr2 = ControlHelperNativeMethods.GetWindow(new HandleRef(null, ptr), 3)) != IntPtr.Zero; ptr = ptr2)
            {
                ControlHelperNativeMethods.GetWindowRect(new HandleRef(null, ptr2), ref rect);
                Rectangle rectangle = Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom);
                if (ControlHelperNativeMethods.IsWindowVisible(new HandleRef(null, ptr2)))
                {
                    notObscuredControlRegion.Exclude(rectangle);
                }
            }
        }

        private static ControlGraphics AddToHash(Control mControl)
        {
            ControlGraphics fg;
            if (printHash.ContainsKey(mControl))
            {
                fg = printHash[mControl];
            }
            else
            {
                fg = new ControlGraphics(mControl);
                mControl.Disposed += new EventHandler(fg.mControl_Disposed);
                printHash.Add(mControl, fg);
            }
            return fg;
        }

        /// <summary>
        /// To store temporarely removed events from controls.
        /// </summary>
        private static WeakDictionary<Control, Dictionary<string, List<Delegate>>> _eventsDisabled = new WeakDictionary<Control, Dictionary<string, List<Delegate>>>();

        /// <summary>
        /// Remove the event handlers for a control (Disable).
        /// </summary>
        /// <param name="ctrl">The control.</param>
        /// <param name="eventName">The event name.</param>
        static internal void DisableControlEvents(Control ctrl, string eventName)
        {
            Delegate[] _eventDelegates = ContainerHelper.GetEventSubscribers(ctrl, eventName);

            if (_eventDelegates != null)
            {
                EventInfo eventInfo = ctrl.GetType().GetEvent(eventName);
                if (eventInfo != null)
                {
                    if (!_eventsDisabled.ContainsKey(ctrl))
                        _eventsDisabled.Add(ctrl, new Dictionary<string, List<Delegate>>());

                    if (!_eventsDisabled[ctrl].ContainsKey(eventName))
                        _eventsDisabled[ctrl].Add(eventName, new List<Delegate>());

                    foreach (Delegate del in _eventDelegates)
                    {
                        _eventsDisabled[ctrl][eventName].Add(del);
                        eventInfo.RemoveEventHandler(ctrl, del);
                    }
                }
            }
        }

        /// <summary>
        /// Append the event handlers previously removed for a control (Enable).
        /// </summary>
        /// <param name="ctrl">The control.</param>
        /// <param name="eventName">The event name.</param>
        static internal void EnableControlEvents(Control ctrl, string eventName)
        {
            if (_eventsDisabled.ContainsKey(ctrl) && _eventsDisabled[ctrl].ContainsKey(eventName))
            {
                EventInfo eventInfo = ctrl.GetType().GetEvent(eventName);
                if (eventInfo != null)
                {
                    foreach (Delegate del in _eventsDisabled[ctrl][eventName])
                    {
                        eventInfo.AddEventHandler(ctrl, del);
                    }

                    _eventsDisabled[ctrl].Remove(eventName);

                    if (_eventsDisabled[ctrl].Count == 0)
                        _eventsDisabled.Remove(ctrl);
                }
            }
        }

        /// <summary>
        /// Print, Cls, Line operations work using some values like
        /// CurrentX, CurrentY, and DrawWidth.
        /// For that reason this values must be keep associated with the control.
        /// </summary>
        public class ControlGraphics
        {
            Control control;
            Graphics _g;

            int _scaleMode = 1;

            private void AdjustXAndY(ref int x, ref int y)
            {
                if (_scaleMode == 0)
                {
                    if (_scaleWidth.HasValue)
                    {
                        x = (int)((x * control.Width) / _scaleWidth.Value);
                    }
                    if (_scaleHeight.HasValue)
                    {
                        y = (int)((y * control.Height) / _scaleHeight.Value);
                    }
                }
            }

            /// <summary>
            /// Returns or sets a value indicating the unit of measurement for coordinates of 
            /// an object when using graphics methods or when positioning controls.
            /// vbUser              0   Indicates that one or more of the ScaleHeight, ScaleWidth, ScaleLeft, and ScaleTop properties are set to custom values.
            /// vbTwips             1   (Default) Twip (1440 twips per logical inch; 567 twips per logical centimeter).
            /// vbPoints            2   Point (72 points per logical inch).
            /// vbPixels            3   Pixel (smallest unit of monitor or printer resolution).
            /// vbCharacters        4   Character (horizontal = 120 twips per unit; vertical = 240 twips per unit).
            /// vbInches            5   Inch.
            /// vbMillimeters       6   Millimeter.
            /// vbCentimeters       7   Centimeter.
            /// vbHimetric          8   HiMetric
            /// vbContainerPosition 9   Units used by the control's container to determine the control's position.
            /// vbContainerSize     10  Units used by the control's container to determine the control's size.
            /// <para>Only 1 and 0 are supported</para>
            /// </summary>
            public int ScaleMode
            {
                get
                {
                    return _scaleMode;
                }
                set
                {
                    if (value != 0 && value != 1)
                    {
                        throw new NotSupportedException("Only values 0 and 1 are supported for scale mode");
                    }
                    _scaleMode = value;
                }
            }

            private double? _scaleHeight;
            /// <summary>
            /// Return or set the number of units for the vertical measurement of the interior of an object when using graphics methods
            /// </summary>
            public double ScaleHeight
            {
                get
                {
                    if (_scaleHeight.HasValue)
                        return _scaleHeight.Value;
                    return double.NaN;
                }
                set
                {
                    _scaleHeight = value;
                    _scaleMode = 0;
                }
            }

            double? _scaleWidth;
            /// <summary>
            /// Return or set the number of units for the horizontal (ScaleWidth) measurement of the interior of an object when using graphics methods
            /// </summary>
            public double ScaleWidth
            {
                get
                {
                    if (_scaleWidth.HasValue)
                        return _scaleWidth.Value;
                    return double.NaN;
                }
                set
                {
                    _scaleWidth = value;
                    _scaleMode = 0;
                }
            }

            double? _scaleLeft;



            /// <summary>
            /// Return or set the horizontal (ScaleLeft) coordinate for the left edge of an object when using graphics methods or when positioning controls.
            /// </summary>
            public double ScaleLeft
            {
                get
                {
                    if (_scaleLeft.HasValue)
                        return _scaleLeft.Value;
                    return double.NaN;
                }
                set
                {
                    _scaleLeft = value;
                    _scaleMode = 0;
                }
            }

            double? _scaleTop;
            /// <summary>
            /// Return or set the vertical (ScaleTop) coordinate for top edges of an object when using graphics methods or when positioning controls.
            /// </summary>
            public double ScaleTop
            {
                get
                {
                    if (_scaleTop.HasValue)
                        return _scaleTop.Value;
                    return double.NaN;
                }
                set
                {
                    _scaleTop = value;
                    _scaleMode = 0;
                }
            }
            private Graphics GetGraphics()
            {
                if (_g != null)
                    return _g;
                else
                {
                    if (_autoRedraw)
                    {
                        if (control is PictureBox)
                        {
                            PictureBox box = (PictureBox)control;
                            if (box.Image == null)
                                box.Image = new Bitmap(box.Width, box.Height);
                            return Graphics.FromImage(box.Image);
                        }
                        throw new NotSupportedException();
                    }
                    return control.CreateGraphics();
                }
            }

            bool _autoRedraw = false;
            /// <summary>
            /// Returns the autoredraw setting
            /// </summary>
            public bool AutoRedraw
            {
                get
                {
                    return _autoRedraw;
                }
                set
                {
                    _autoRedraw = value;
                }
            }

            /// <summary>
            /// Provides a graphics object that allows performing draw functions on controls
            /// </summary>
            /// <param name="controlToDraw">The control to draw.</param>
            public ControlGraphics(Control controlToDraw)
            {
                this.control = controlToDraw;
            }

            /// <summary>
            /// Use this method when there are several drawing operations, to catch graphics objects
            /// </summary>
            public void Start()
            {
                _g = GetGraphics();
            }


            /// <summary>
            /// Use this method to release graphics objects
            /// </summary>
            public void End()
            {
                if (_g != null)
                {
                    _g.Dispose();
                    _g = null;
                }
            }

            private int drawWidth = 1;
            /// <summary>
            /// Width to use for drawing operations
            /// </summary>
            public int DrawWidth
            {
                get
                {
                    return this.drawWidth;
                }
                set
                {
                    this.drawWidth = value;
                }
            }

            private float currentX = 0;
            /// <summary>
            /// "Current" x-axis value
            /// </summary>
            public float CurrentX
            {
                get
                {
                    return this.currentX;
                }
                set
                {
                    this.currentX = value;
                }
            }

            private float currentY = 0;
            /// <summary>
            /// "Current" y-axis value
            /// </summary>
            public float CurrentY
            {
                get
                {
                    return this.currentY;
                }
                set
                {
                    this.currentY = value;
                }
            }

            /// <summary>
            /// Clears the control from any previous drawings.
            /// </summary>
            public void Cls()
            {
                Graphics g = GetGraphics();
                try
                {
                    g.Clear(control.BackColor);
                    CurrentX = 0;
                    CurrentY = 0;
                }
                finally
                {
                    Free(g);
                }
            }

            private void Free(Graphics g)
            {
                if (_g != null) return;
                g.Dispose();

            }

            /// <summary>
            /// Draws an image with its actual size.
            /// </summary>
            /// <param name="x">X coordinate position.</param>
            /// <param name="y">Y coordinate position.</param>
            /// <param name="filename">Filename for the image to draw.</param>
            public void DrawImage(int x, int y, string filename)
            {
                Graphics g = GetGraphics();
                AdjustXAndY(ref x, ref y);
                try
                {
                    Bitmap imagen = new Bitmap(filename);
                    g.DrawImage(imagen, x, y);
                }
                finally
                {
                    Free(g);
                }

            }

            /// <summary>
            /// Draws an image with the specified size.
            /// </summary>
            /// <param name="x">X coordinate position.</param>
            /// <param name="y">Y coordinate position.</param>
            /// <param name="width">Width for the image.</param>
            /// <param name="height">Height for the image.</param>
            /// <param name="filename">Filename for the image to draw.</param>
            public void DrawImage(int x, int y, int width, int height, string filename)
            {

                Graphics g = GetGraphics();
                AdjustXAndY(ref x, ref y);
                try
                {

                    Bitmap imagen = new Bitmap(filename);
                    g.DrawImage(imagen, x, y, width, height);
                }
                finally
                {
                    Free(g);
                }
            }

            /// <summary>
            /// Draws a circle with the specified color.
            /// </summary>
            /// <param name="x">The x position.</param>
            /// <param name="y">The y position.</param>
            /// <param name="radius">The circle's radius.</param>
            /// <param name="color">The color to paint the circle.</param>
            public void Circle(int x, int y, double radius, Color color)
            {
                Graphics g = GetGraphics();
                AdjustXAndY(ref x, ref y);
                SolidBrush brush = new SolidBrush(color);
                Pen pen = new Pen(brush);
                try
                {
                    radius = radius * 1.108;
                    x -= (int)radius / 2;
                    y -= (int)radius / 2;
                    g.DrawEllipse(pen, x, y, (float)radius, (float)radius);
                    CurrentX = x;
                    CurrentY = y;
                }
                finally
                {
                    Free(g);
                    brush.Dispose();
                    pen.Dispose();
                }
            }

            /// <summary>
            /// Draws a line with the specified points and color.
            /// </summary>
            /// <param name="x1">The x coordinate of the starting point.</param>
            /// <param name="y1">The y coordinate of the starting point.</param>
            /// <param name="x2">The x coordinate of the ending point.</param>
            /// <param name="y2">The y coordinate of the ending point.</param>
            /// <param name="color">The color to paint the line.</param>
            public void Line(int x1, int y1, int x2, int y2, Color color)
            {
                Graphics g = GetGraphics();
                AdjustXAndY(ref x1, ref y1);
                AdjustXAndY(ref x2, ref y2);
                Pen pen = new Pen(color); //, DrawWidth);
                try
                {
                    g.DrawLine(pen, x1, y1, x2, y2);
                    CurrentX = x2;
                    CurrentY = y2;
                }
                finally
                {
                    Free(g);
                    pen.Dispose();
                }
            }

            /// <summary>
            /// Prints the specified parameters in the control.
            /// </summary>
            /// <param name="parameters">Parameters to print.</param>
            public void Print(params object[] parameters)
            {
                Graphics g = GetGraphics();
                SolidBrush brush = new SolidBrush(control.ForeColor);
                try
                {
                    Font font = control.Font;
                    foreach (object o in parameters)
                    {
                        if (o == null)
                        {
                            //In VB6 this causes an exception
                            //TODO:  should we throw that same exception?
                        }
                        else
                            g.DrawString(o.ToString(), font, brush, CurrentX, CurrentY);

                        CurrentX += ((int)font.Size) * 12;
                    }
                    CurrentY += font.Height;
                    CurrentX = 0;
                }
                finally
                {
                    Free(g);
                    brush.Dispose();
                }
            }



            /// <summary>
            /// Implements Line Method 
            /// object.Line [Step] (x1, y1) [Step] - (x2, y2), [color], [B][F]
            /// </summary>
            /// <param name="x1">starting x coordinate</param>
            /// <param name="y1">starting y coordinate</param>
            /// <param name="x2">Ending x2 coordinate</param>
            /// <param name="y2">Ending y2 coordinate</param>
            /// <param name="color">Color used to draw the line</param>
            /// <param name="b">Optional. If included, causes a box to be drawn using the coordinates to specify opposite corners of the box.</param>
            /// <param name="f">Optional. If the B option is used, the F option specifies that the box is filled with the same color used to draw the box. You cannot use F without B</param>
            public void Line(int x1, int y1, int x2, int y2, Color color, bool b = false, bool f = false)
            {
                Graphics g = GetGraphics();
                AdjustXAndY(ref x1, ref y1);
                AdjustXAndY(ref x2, ref y2);
                Pen pen = new System.Drawing.Pen(color);
                try
                {

                    if (b)
                    {
                        g.DrawRectangle(pen, x1, y1, x2 - x1, y2 - y1);
                    }
                    else if (f)
                    {
                        System.Diagnostics.Debug.Assert(b != false, "You cannot use F without B");
                        g.FillRectangle(new SolidBrush(color), x1, y1, x2 - x1, y2 - y1);
                    }
                    else
                    {
                        g.DrawLine(pen, x1, y1, x2, y2);
                    }
                }
                finally
                {
                    Free(g);
                    pen.Dispose();
                }

            }


            /// <summary>
            /// Implements Line Method 
            /// object.Line (x1, y1) - (x2, y2)
            /// </summary>
            /// <param name="x1">starting x coordinate</param>
            /// <param name="y1">starting y coordinate</param>
            /// <param name="x2">Ending x2 coordinate</param>
            /// <param name="y2">Ending y2 coordinate</param>
            public void Line(int x1, int y1, int x2, int y2)
            {
                Line(x1, y1, x2, y2, Color.Black);
            }

            /// <summary>
            /// Sets a point on an object to a specified color
            /// </summary>
            /// <param name="x">(x-axis) coordinate of the point to set</param>
            /// <param name="y">(y-axis) coordinate of the point to set</param>
            /// <param name="color">color specified for point. If omitted, the current ForeColor property setting is used</param>
            public void PSet(int x, int y, Color color)
            {
                Graphics g = GetGraphics();
                AdjustXAndY(ref x, ref y);
                Pen pen = new System.Drawing.Pen(color);
                try
                {

                    g.DrawEllipse(pen, x, y, 1, 1);
                }
                finally
                {
                    Free(g);
                    pen.Dispose();
                }
            }

            /// <summary>
            /// Sets a point on an object to a specified color
            /// The current ForeColor property setting is used for point color
            /// </summary>
            /// <param name="x">(x-axis) coordinate of the point to set</param>
            /// <param name="y">(y-axis) coordinate of the point to set</param>
            public void PSet(int x, int y)
            {
                PSet(x, y, Color.Black);
            }


            /// <summary>
            /// This is used to handle the dispose event of the associated control
            /// to make sure that the hash table is removed.
            /// </summary>
            /// <param name="sender">The sender of the event.</param>
            /// <param name="e">The event arguments.</param>
            internal void mControl_Disposed(object sender, EventArgs e)
            {
                Dispose();
            }


            /// <summary>
            /// Releases elements from hastables and other associated resources
            /// </summary>
            public void Dispose()
            {
                printHash.Remove(control);
            }
        }
    }
}
