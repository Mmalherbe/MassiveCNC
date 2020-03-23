using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Layout;
using UpgradeHelpers.Helpers;

namespace UpgradeHelpers.Gui
{
    /// <summary>
    /// It is very common in VB6 to implement algorithms that traverse the Controls collection
    /// for a Form or UserControl.
    /// During migration some issues appear in the target platform because the controls collection
    /// in .NET is not flat but hierarchical.
    /// This issue will break a lot of the original logic, that assumes that all controls present in the 
    /// Form or UserControl will be present in one big collection.
    /// This Helper Class provides several methods that allows to traverse the .NET components collections
    /// in an easy and direct way, just as it was possible in VB6.
    /// </summary>
    public class ContainerHelper
    {

        /// <summary>
        /// The .NET Framework does not implement the VB6 concept of controls arrays.
        /// During migration these arrays are generated as Arrays of Controls, but
        /// these arrays are not Controls. So they are not present in the Controls collections of a Form or UserControl.
        /// So to perform things like: <br></br>
        /// <code>
        /// Form1.Controls("MyTextBoxControlArray")
        /// </code>
        /// A class that will be able to "wrap" the array of controls and make it "behave" like a Control.
        /// and gives access to those elements.
        /// </summary>
        public class ControlArray : Control
        {
            /// <summary>
            /// A reference to the actual array.
            /// </summary>
            private readonly Control[] controlArray;

            /// <summary>
            /// Builds a ControlArray object that will "wrap" the specified array, to make it be "seen" as a Control.
            /// </summary>
            /// <param name="controlArray">The control array that will be wrapped.</param>
            public ControlArray(Control[] controlArray)
            {
                this.controlArray = controlArray;
            }

            /// <summary>
            /// Builds a ControlArray object that will "wrap" the specified array, to make it be "seen" as a Control.
            /// This overload is used when the Control Array has a different type. 
            /// Supported array types are:
            /// * Control
            /// * ToolStripItem
            /// * MenuStrip
            /// </summary>
            /// <exception cref="System.InvalidCastException">Thrown when an array with unsupported type is used.</exception>
            /// <param name="ctrlArray">The control array that will be wrapped.</param>
            public ControlArray(Array ctrlArray)
            {
                object itemArray;
                List<Control> lstControls = new List<Control>();
                Type elemType = ctrlArray.GetType().GetElementType();

                if (ctrlArray.Length == 0)
                {
                    controlArray = new Control[] { };
                }
                else
                {
                    itemArray = ctrlArray.GetValue(ctrlArray.GetLowerBound(0));

                    if (itemArray is Control)
                    {
                        controlArray = (Control[])ctrlArray;
                    }
                    else if (itemArray is ToolStripItem)
                    {
                        foreach (object item in ctrlArray)
                        {
                            ToolStripItem toolStrip = item as ToolStripItem;
                            lstControls.Add(new MenuItemControl(toolStrip));
                        }
                        controlArray = lstControls.ToArray();
                    }
                    else if (itemArray is MenuStrip)
                    {
                        foreach (object item in ctrlArray)
                        {
                            MenuStrip mnuStrip = item as MenuStrip;
                            lstControls.Add(new MenuItemControl(mnuStrip));
                        }
                        controlArray = lstControls.ToArray();
                    }
                    else
                        throw new InvalidCastException("Invalid element type for control array: " + elemType.Name);
                }
            }

            /// <summary>
            /// Returns the Control at the specified index position.
            /// </summary>
            /// <exception cref="System.IndexOutOfRangeException">Raised when the index is out of range.</exception>
            /// <param name="index">The index of the control to obtain.</param>
            /// <returns>The control found at the specified index.</returns>
            public Control this[int index]
            {
                get
                {
                    return controlArray[index];
                }
            }

            /// <summary>
            /// Returns the Control at the specified index position.
            /// </summary>
            /// <exception cref="System.IndexOutOfRangeException">Raised when the index is out of range.</exception>
            /// <param name="index">The index of the control to obtain.</param>
            /// <returns>The control found at the specified index.</returns>
            public Control this[double index]
            {
                get
                {
                    return controlArray[(int)index];
                }
            }

            /// <summary>
            /// Returns the Length of the subjacent array.
            /// </summary>
            public int Length
            {
                get
                {
                    return controlArray.Length;
                }
            }

        }

        /// <summary>
        /// VB6 Menus are migrated to classes of the ToolStripItems objects.
        /// .NET menus are not Controls like in VB6.
        /// To iterate thru the collection of menu items we must "wrap" all the items.
        /// </summary>
        public class MenuItemControl : Control
        {
            /// <summary>
            /// Internal reference to the menuItems or mainMenu.
            /// </summary>
            private readonly ToolStripItem menuItem;

            /// <summary>
            /// Returns the internal reference to the "wrapped" instance of a menuItem.
            /// </summary>
            public ToolStripItem ToolStripItemInstance
            {
                get
                {
                    return menuItem;
                }
            }

            /// <summary>
            /// Variable to hold the reference to main menu element.
            /// </summary>
            private readonly MenuStrip mainMenu;

            /// <summary>
            /// Returns a reference to the MenuStrip that represent the main menu element.
            /// </summary>
            public MenuStrip MenuStrip
            {
                get
                {
                    return mainMenu;
                }
            }

            /// <summary>
            /// Overriding of casting operations.
            /// </summary>
            /// <param name="item">The item to cast.</param>
            /// <exception cref="System.Exception">Throw if the MenuStrip property is NULL.</exception>
            /// <returns></returns>
            public static explicit operator MenuStrip(MenuItemControl item)
            {
                if (item.mainMenu != null)
                    return item.mainMenu;
                throw new Exception("AIS-Exception. Item does not contains a reference to a MenuStrip type");
            }

            /// <summary>
            /// Implements a casting operator to unwrap the ToolStripItem.
            /// </summary>
            /// <exception cref="System.Exception">Throw if contained MenuItem is null.</exception>
            /// <param name="item">The item to cast.</param>
            /// <returns></returns>
            public static explicit operator ToolStripItem(MenuItemControl item)
            {
                if (item.menuItem != null)
                    return item.menuItem;
                throw new Exception("AIS-Exception. Item does not contains a reference to a ToolStripItem type");
            }

            /// <summary>
            /// Implements a casting operator to wrap MenuStrip inside MenuItemControl instance.
            /// </summary>
            /// <param name="item">The item to cast.</param>
            /// <returns></returns>
            public static explicit operator MenuItemControl(MenuStrip item)
            {
                return new MenuItemControl(item);
            }

            /// <summary>
            /// Implements a casting operator to wrap ToolStripItem inside a MenuItemControl instance.
            /// </summary>
            /// <param name="item">The item to cast.</param>
            /// <returns></returns>
            public static explicit operator MenuItemControl(ToolStripItem item)
            {
                return new MenuItemControl(item);
            }


            /// <summary>
            /// Constructs a new instance wrapping a MenuStrip item inside of it.
            /// </summary>
            /// <param name="mainMenu">The MenuStrip item to wrap.</param>
            public MenuItemControl(MenuStrip mainMenu)
            {
                this.mainMenu = mainMenu;
                InitializeProperties();
                AddEventHandlers();
            }

            /// <summary>
            /// Constructs a new instance wrapping a ToolStripItem inside of it.
            /// </summary>
            /// <param name="menuItem">The ToolStripItem to wrap.</param>
            public MenuItemControl(ToolStripItem menuItem)
            {
                this.menuItem = menuItem;
                InitializeProperties();
                AddEventHandlers();
            }

            /// <summary>
            /// Returns true if this wrapper contains a reference that is not null.
            /// </summary>
            public bool IsToolStripItem
            {
                get
                {
                    return menuItem != null;
                }
            }

            /// <summary>
            /// Takes care of initialize some of the properties of the base control class with 
            /// the contained instance.
            /// </summary>
            private void InitializeProperties()
            {
                try
                {
                    base.Enabled = Enabled;
                }
                catch
                {
                }
                try
                {
                    base.Name = Name;
                }
                catch
                {
                }
                try
                {
                    base.Tag = Tag;
                }
                catch
                {
                }
                try
                {
                    base.Visible = Visible;
                }
                catch
                {
                }
            }

            /// <summary>
            /// Adds some eventHandlers of the base control class to track when some properties values 
            /// have been changed, specifically the Visible and Enable properties.
            /// </summary>
            private void AddEventHandlers()
            {
                VisibleChanged += MenuItemControl_VisibleChanged;
                EnabledChanged += MenuItemControl_EnabledChanged;
            }

            /// <summary>
            /// Event Handler to handle changes to the visible property.
            /// </summary>
            /// <param name="sender">The sender of the event.</param>
            /// <param name="e">The event arguments.</param>
            private void MenuItemControl_VisibleChanged(object sender, EventArgs e)
            {
                Visible = base.Enabled;
            }

            /// <summary>
            /// Event Handler to handle changes to the enable property.
            /// </summary>
            /// <param name="sender">The sender of the event.</param>
            /// <param name="e">The event arguments.</param>
            private void MenuItemControl_EnabledChanged(object sender, EventArgs e)
            {
                Enabled = base.Enabled;
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem AccessibilityObject.
            /// </summary>
            /// <exception cref="System.Exception">Thrown if object is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new AccessibleObject AccessibilityObject
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.AccessibilityObject;

                    if (menuItem != null)
                        return menuItem.AccessibilityObject;

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem AccessibleDefaultActionDescription.
            /// <seealso cref="System.Windows.Forms.Control.AccessibleDefaultActionDescription"/>
            /// </summary>
            /// <exception cref="System.Exception">Thrown if object is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new string AccessibleDefaultActionDescription
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.AccessibleDefaultActionDescription;

                    if (menuItem != null)
                        return menuItem.AccessibleDefaultActionDescription;

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
                set
                {
                    if (mainMenu != null)
                        mainMenu.AccessibleDefaultActionDescription = value;
                    else if (menuItem != null)
                        menuItem.AccessibleDefaultActionDescription = value;
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem AccessibleDescription.
            /// <seealso cref="System.Windows.Forms.Control.AccessibleDescription"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if object is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new string AccessibleDescription
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.AccessibleDescription;

                    if (menuItem != null)
                        return menuItem.AccessibleDescription;

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
                set
                {
                    if (mainMenu != null)
                        mainMenu.AccessibleDescription = value;
                    else if (menuItem != null)
                        menuItem.AccessibleDescription = value;
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem AccessibleName.
            /// <seealso cref="System.Windows.Forms.Control.AccessibleName"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if object is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new string AccessibleName
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.AccessibleName;

                    if (menuItem != null)
                        return menuItem.AccessibleName;

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
                set
                {
                    if (mainMenu != null)
                        mainMenu.AccessibleName = value;
                    else if (menuItem != null)
                        menuItem.AccessibleName = value;
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }


            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem AccessibleRole.
            /// <seealso cref="System.Windows.Forms.Control.AccessibleRole"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if object is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new AccessibleRole AccessibleRole
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.AccessibleRole;

                    if (menuItem != null)
                        return menuItem.AccessibleRole;

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
                set
                {
                    if (mainMenu != null)
                        mainMenu.AccessibleRole = value;
                    else if (menuItem != null)
                        menuItem.AccessibleRole = value;
                    else
                        throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem AllowDrop.
            /// <seealso cref="System.Windows.Forms.Control.AllowDrop"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if object is not set.</exception>
            public override bool AllowDrop
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.AllowDrop;

                    if (menuItem != null)
                        return menuItem.AllowDrop;

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
                set
                {
                    if (mainMenu != null)
                        mainMenu.AllowDrop = value;
                    else if (menuItem != null)
                        menuItem.AllowDrop = value;
                    else
                        throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }


            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem Anchor.
            /// <seealso cref="System.Windows.Forms.Control.Anchor"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if object is not set.</exception>
            public override AnchorStyles Anchor
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.Anchor;

                    if (menuItem != null)
                        return menuItem.Anchor;

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
                set
                {
                    if (mainMenu != null)
                        mainMenu.Anchor = value;
                    else if (menuItem != null)
                        menuItem.Anchor = value;
                    else
                        throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem AutoScrollOffset.
            /// <seealso cref="System.Windows.Forms.Control.AutoScrollOffset"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if object is not set.</exception>
            public override Point AutoScrollOffset
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.AutoScrollOffset;

                    if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
                set
                {
                    if (mainMenu != null)
                        mainMenu.AutoScrollOffset = value;
                    else if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");
                    else
                        throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem AutoSize.
            /// <seealso cref="System.Windows.Forms.Control.AutoSize"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if object is not set.</exception>
            public override bool AutoSize
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.AutoSize;

                    if (menuItem != null)
                        return menuItem.AutoSize;

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
                set
                {
                    if (mainMenu != null)
                        mainMenu.AutoSize = value;
                    else if (menuItem != null)
                        menuItem.AutoSize = value;
                    else
                        throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }


            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem BackColor.
            /// <seealso cref="System.Windows.Forms.Control.BackColor"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if object is not set.</exception>
            public override Color BackColor
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.BackColor;

                    if (menuItem != null)
                        return menuItem.BackColor;

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
                set
                {
                    if (mainMenu != null)
                        mainMenu.BackColor = value;
                    else if (menuItem != null)
                        menuItem.BackColor = value;
                    else
                        throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem BackgroundImage.
            /// <seealso cref="System.Windows.Forms.Control.BackgroundImage"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if object is not set.</exception>
            public override Image BackgroundImage
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.BackgroundImage;

                    if (menuItem != null)
                        return menuItem.BackgroundImage;

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
                set
                {
                    if (mainMenu != null)
                        mainMenu.BackgroundImage = value;
                    else if (menuItem != null)
                        menuItem.BackgroundImage = value;
                    else
                        throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }


            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem BackgroundImageLayout.
            /// <seealso cref="System.Windows.Forms.Control.BackgroundImageLayout"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if object is not set.</exception>
            public override ImageLayout BackgroundImageLayout
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.BackgroundImageLayout;

                    if (menuItem != null)
                        return menuItem.BackgroundImageLayout;

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
                set
                {
                    if (mainMenu != null)
                        mainMenu.BackgroundImageLayout = value;
                    else if (menuItem != null)
                        menuItem.BackgroundImageLayout = value;
                    else
                        throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }


            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem BindingContext.
            /// <seealso cref="System.Windows.Forms.Control.BindingContext"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if object is not set.</exception>
            public override BindingContext BindingContext
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.BindingContext;

                    if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
                set
                {
                    if (mainMenu != null)
                        mainMenu.BindingContext = value;
                    else if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");
                    else
                        throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }


            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem Bottom.
            /// <seealso cref="System.Windows.Forms.Control.Bottom"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if object is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new int Bottom
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.Bottom;

                    if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem Bounds.
            /// <seealso cref="System.Windows.Forms.Control.Bounds"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if object is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Rectangle Bounds
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.Bounds;

                    if (menuItem != null)
                        return menuItem.Bounds;

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
                set
                {
                    if (mainMenu != null)
                        mainMenu.Bounds = value;
                    else if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");
                    else
                        throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem CanFocus.
            /// <seealso cref="System.Windows.Forms.Control.CanFocus"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if object is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new bool CanFocus
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.CanFocus;

                    if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem CanSelect.
            /// <seealso cref="System.Windows.Forms.Control.CanSelect"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if object is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new bool CanSelect
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.CanSelect;

                    if (menuItem != null)
                        return menuItem.CanSelect;

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem Capture.
            /// <seealso cref="System.Windows.Forms.Control.Capture"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if object is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new bool Capture
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.Capture;

                    if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
                set
                {
                    if (mainMenu != null)
                        mainMenu.Capture = value;
                    else if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");
                    else
                        throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem CausesValidation.
            /// <seealso cref="System.Windows.Forms.Control.CausesValidation"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if object is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new bool CausesValidation
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.CausesValidation;

                    if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
                set
                {
                    if (mainMenu != null)
                        mainMenu.CausesValidation = value;
                    else if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");
                    else
                        throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem CheckForIllegalCrossThreadCalls.
            /// <seealso cref="System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if object is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new static bool CheckForIllegalCrossThreadCalls
            {
                get
                {
                    return Control.CheckForIllegalCrossThreadCalls;
                }
                set
                {
                    Control.CheckForIllegalCrossThreadCalls = true;
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem ClientRectangle.
            /// <seealso cref="System.Windows.Forms.Control.ClientRectangle"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if object is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Rectangle ClientRectangle
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.ClientRectangle;

                    if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem ClientSize.
            /// <seealso cref="System.Windows.Forms.Control.ClientSize"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if object is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Size ClientSize
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.ClientSize;

                    if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
                set
                {
                    if (mainMenu != null)
                        mainMenu.ClientSize = value;
                    else if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");
                    else
                        throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip CompanyName property.
            /// <seealso cref="System.Windows.Forms.Control.CompanyName"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object does not support 
            /// the property or it is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new string CompanyName
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.CompanyName;

                    if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip ContainsFocus property.
            /// <seealso cref="System.Windows.Forms.Control.ContainsFocus"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object does not support 
            /// the property or it is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new bool ContainsFocus
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.ContainsFocus;

                    if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip ContextMenu property.
            /// <seealso cref="System.Windows.Forms.Control.ContextMenu"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object does not support 
            /// the property or it is not set.</exception>
            public override ContextMenu ContextMenu
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.ContextMenu;

                    if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
                set
                {
                    if (mainMenu != null)
                        mainMenu.ContextMenu = value;
                    else if (menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }


            /// <summary>
            /// Gives access to the internal MenuStrip ContextMenuStrip property.
            /// <seealso cref="System.Windows.Forms.Control.ContextMenuStrip"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object does not support 
            /// the property or it is not set.</exception>
            public override ContextMenuStrip ContextMenuStrip
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.ContextMenuStrip;

                    if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
                set
                {
                    if (mainMenu != null)
                        mainMenu.ContextMenuStrip = value;
                    else if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");
                    else
                        throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Returns a flat collection of Controls for a MenuStrip or a ToolStripItem or 
            /// null if the internal object is not set.
            /// </summary>
            public new ControlCollection Controls
            {
                get
                {
                    if (mainMenu != null)
                        return new MenuItemsCollection(mainMenu);

                    if (menuItem != null)
                        return new MenuItemsCollection(menuItem);

                    return null;
                }
                set { throw new NotImplementedException(); }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip Created property.
            /// <seealso cref="System.Windows.Forms.Control.ContextMenuStrip"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object does not support 
            /// the property or it is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new bool Created
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.Created;

                    if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip Cursor property.
            /// <seealso cref="System.Windows.Forms.Control.ContextMenuStrip"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object does not support 
            /// the property or it is not set.</exception>
            public override Cursor Cursor
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.Cursor;

                    if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
                set
                {
                    if (mainMenu != null)
                        mainMenu.Cursor = value;
                    else if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");
                    else
                        throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip DataBindings property.
            /// <seealso cref="System.Windows.Forms.Control.DataBindings"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object does not support 
            /// the property or it is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new ControlBindingsCollection DataBindings
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.DataBindings;

                    if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Returns the DefaultBackColor for Controls.
            /// <seealso cref="System.Windows.Forms.Control.DefaultBackColor"/>
            /// </summary>            
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new static Color DefaultBackColor
            {
                get
                {
                    return Control.DefaultBackColor;
                }
            }

            /// <summary>
            /// Returns the DefaultBackColor for Controls.
            /// <seealso cref="System.Windows.Forms.Control.DefaultFont"/>
            /// </summary>            
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new static Font DefaultFont
            {
                get
                {
                    return Control.DefaultFont;
                }
            }

            /// <summary>
            /// Returns the DefaultBackColor for Controls.
            /// <seealso cref="System.Windows.Forms.Control.DefaultForeColor"/>
            /// </summary>            
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new static Color DefaultForeColor
            {
                get
                {
                    return Control.DefaultForeColor;
                }
            }


            /// <summary>
            /// Gives access to the internal MenuStrip DisplayRectangle property.
            /// <seealso cref="System.Windows.Forms.Control.DisplayRectangle"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object does not support 
            /// the property or it is not set.</exception>
            public override Rectangle DisplayRectangle
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.DisplayRectangle;

                    if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip Disposing property.
            /// <seealso cref="System.Windows.Forms.Control.Disposing"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object does not support 
            /// the property or it is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new bool Disposing
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.Disposing;

                    if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem Dock property.
            /// <seealso cref="System.Windows.Forms.Control.Dock"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set.</exception>
            public override DockStyle Dock
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.Dock;

                    if (menuItem != null)
                        return menuItem.Dock;

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
                set
                {
                    if (mainMenu != null)
                        mainMenu.Dock = value;
                    else if (menuItem != null)
                        menuItem.Dock = value;
                    else
                        throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem Enabled property.
            /// <seealso cref="System.Windows.Forms.Control.Dock"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new bool Enabled
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.Enabled;

                    if (menuItem != null)
                        return menuItem.Enabled;

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
                set
                {
                    if (mainMenu != null)
                        mainMenu.Enabled = value;
                    else if (menuItem != null)
                        menuItem.Enabled = value;
                    else
                        throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }


            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem Focused property.
            /// <seealso cref="System.Windows.Forms.Control.Focused"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object does not support 
            /// the property or it is not set.</exception>
            public override bool Focused
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.Focused;

                    if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem Font property.
            /// <seealso cref="System.Windows.Forms.Control.Font"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set.</exception>
            public override Font Font
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.Font;

                    if (menuItem != null)
                        return menuItem.Font;

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
                set
                {
                    if (mainMenu != null)
                        mainMenu.Font = value;
                    else if (menuItem != null)
                        menuItem.Font = value;
                    else
                        throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem ForeColor property.
            /// <seealso cref="System.Windows.Forms.Control.ForeColor"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set.</exception>
            public override Color ForeColor
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.ForeColor;

                    if (menuItem != null)
                        return menuItem.ForeColor;

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
                set
                {
                    if (mainMenu != null)
                        mainMenu.ForeColor = value;
                    else if (menuItem != null)
                        menuItem.ForeColor = value;
                    else
                        throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gets Window Handle
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new IntPtr Handle
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.Handle;

                    if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Not supported.
            /// </summary>
            /// <exception cref="System.Exception">Throws an exception indicating that it is not supported.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new bool HasChildren
            {
                get
                {
                    throw new NotSupportedException("AIS-Exception, Object does not support the property");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem Height property.
            /// <seealso cref="System.Windows.Forms.Control.Height"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new int Height
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.Height;

                    if (menuItem != null)
                        return menuItem.Height;

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
                set
                {
                    if (mainMenu != null)
                        mainMenu.Height = value;
                    else if (menuItem != null)
                        menuItem.Height = value;
                    else
                        throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }


            /// <summary>
            /// Gives access to the internal MenuStrip IME Mode property.
            /// <seealso cref="System.Windows.Forms.Control.ImeMode"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set or
            /// if the internal object does not support the property.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new ImeMode ImeMode
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.ImeMode;

                    if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
                set
                {
                    if (mainMenu != null)
                        mainMenu.ImeMode = value;
                    else if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");
                    else
                        throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip InvokeRequired property.
            /// <seealso cref="System.Windows.Forms.Control.InvokeRequired"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set or 
            /// if the internal object does not support the property.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new bool InvokeRequired
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.InvokeRequired;

                    if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip IsAccessible property.
            /// <seealso cref="System.Windows.Forms.Control.IsAccessible"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set or 
            /// if the internal object does not support the property.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new bool IsAccessible
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.IsAccessible;

                    if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
                set
                {
                    if (mainMenu != null)
                        mainMenu.IsAccessible = value;
                    else if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");
                    else
                        throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }


            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem IsDisposed property.
            /// <seealso cref="System.Windows.Forms.Control.IsDisposed"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new bool IsDisposed
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.IsDisposed;

                    if (menuItem != null)
                        return menuItem.IsDisposed;

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }



            /// <summary>
            /// Gives access to the internal MenuStrip IsHandleCreated property.
            /// <seealso cref="System.Windows.Forms.Control.Height"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set or 
            /// if the internal object does not support the property.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new bool IsHandleCreated
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.IsHandleCreated;

                    if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip IsMirrored property.
            /// <seealso cref="System.Windows.Forms.Control.Height"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set or 
            /// if the internal object does not support the property.</exception>            
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new bool IsMirrored
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.IsMirrored;

                    if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }


            /// <summary>
            /// Gives access to the internal MenuStrip LayoutEngine property.
            /// <seealso cref="System.Windows.Forms.Control.Height"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set or 
            /// if the internal object does not support the property.</exception>            
            public override LayoutEngine LayoutEngine
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.LayoutEngine;

                    if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip Left property.
            /// <seealso cref="System.Windows.Forms.Control.Left"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set or 
            /// if the internal object does not support the property.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new int Left
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.Left;

                    if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
                set
                {
                    if (mainMenu != null)
                        mainMenu.Left = value;
                    else if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");
                    else
                        throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip Location property.
            /// <seealso cref="System.Windows.Forms.Control.Location"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set or 
            /// if the internal object does not support the property.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Point Location
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.Location;

                    if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
                set
                {
                    if (mainMenu != null)
                        mainMenu.Location = value;
                    else if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");
                    else
                        throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }


            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem Margin property.
            /// <seealso cref="System.Windows.Forms.Control.Margin"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Padding Margin
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.Margin;

                    if (menuItem != null)
                        return menuItem.Margin;

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
                set
                {
                    if (mainMenu != null)
                        mainMenu.Margin = value;
                    else if (menuItem != null)
                        menuItem.Margin = value;
                    else
                        throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip MaximumSize property.
            /// <seealso cref="System.Windows.Forms.Control.MaximumSize"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set or 
            /// if the internal object does not support the property.</exception>
            public override Size MaximumSize
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.MaximumSize;

                    if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
                set
                {
                    if (mainMenu != null)
                        mainMenu.MaximumSize = value;
                    else if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");
                    else
                        throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip MinimumSize property.
            /// <seealso cref="System.Windows.Forms.Control.MinimumSize"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set or 
            /// if the internal object does not support the property.</exception>

            public override Size MinimumSize
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.MinimumSize;

                    if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
                set
                {
                    if (mainMenu != null)
                        mainMenu.MinimumSize = value;
                    else if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");
                    else
                        throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gets the value for modifier key (Ctrl, Shift and Alt)
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new static Keys ModifierKeys
            {
                get
                {
                    return Control.ModifierKeys;
                }
            }

            /// <summary>
            /// Gets which Mouse button is pressed.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new static MouseButtons MouseButtons
            {
                get
                {
                    return Control.MouseButtons;
                }
            }

            /// <summary>
            /// Gets the Point position of the mouse.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new static Point MousePosition
            {
                get
                {
                    return Control.MousePosition;
                }
            }


            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem Name property.
            /// <seealso cref="System.Windows.Forms.Control.Name"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new string Name
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.Name;

                    if (menuItem != null)
                        return menuItem.Name;

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
                set
                {
                    if (mainMenu != null)
                        mainMenu.Name = value;
                    else if (menuItem != null)
                        menuItem.Name = value;
                    else
                        throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }


            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem Padding property.
            /// <seealso cref="System.Windows.Forms.Control.Padding"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Padding Padding
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.Padding;

                    if (menuItem != null)
                        return menuItem.Padding;

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
                set
                {
                    if (mainMenu != null)
                        mainMenu.Padding = value;
                    else if (menuItem != null)
                        menuItem.Padding = value;
                    else
                        throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip Parent property.
            /// <seealso cref="System.Windows.Forms.Control.Parent"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set or 
            /// if the internal object does not support the property.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Control Parent
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.Parent;

                    if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
                set
                {
                    if (mainMenu != null)
                        mainMenu.Parent = value;
                    else if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");
                    else
                        throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip PreferredSize property.
            /// <seealso cref="System.Windows.Forms.Control.PreferredSize"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set or 
            /// if the internal object does not support the property.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Size PreferredSize
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.PreferredSize;

                    if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip ProductName property.
            /// <seealso cref="System.Windows.Forms.Control.ProductName"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set or 
            /// if the internal object does not support the property.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new string ProductName
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.ProductName;

                    if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip ProductVersion property.
            /// <seealso cref="System.Windows.Forms.Control.ProductVersion"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set or 
            /// if the internal object does not support the property.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new string ProductVersion
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.ProductVersion;

                    if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip RecreatingHandle property.
            /// <seealso cref="System.Windows.Forms.Control.RecreatingHandle"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set or 
            /// if the internal object does not support the property.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new bool RecreatingHandle
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.RecreatingHandle;

                    if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }


            /// <summary>
            /// Gives access to the internal MenuStrip Region property.
            /// <seealso cref="System.Windows.Forms.Control.Region"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set or 
            /// if the internal object does not support the property.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Region Region
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.Region;

                    if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
                set
                {
                    if (mainMenu != null)
                        mainMenu.Region = value;
                    else if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");
                    else
                        throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip Right property.
            /// <seealso cref="System.Windows.Forms.Control.Right"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set or 
            /// if the internal object does not support the property.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new int Right
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.Right;

                    if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem RightToLeft property.
            /// <seealso cref="System.Windows.Forms.Control.RightToLeft"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set.</exception>
            public override RightToLeft RightToLeft
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.RightToLeft;

                    if (menuItem != null)
                        return menuItem.RightToLeft;

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
                set
                {
                    if (mainMenu != null)
                        mainMenu.RightToLeft = value;
                    else if (menuItem != null)
                        menuItem.RightToLeft = value;
                    else
                        throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem Site property.
            /// <seealso cref="System.Windows.Forms.Control.Site"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set.</exception>
            public override ISite Site
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.Site;

                    if (menuItem != null)
                        return menuItem.Site;

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
                set
                {
                    if (mainMenu != null)
                        mainMenu.Site = value;
                    else if (menuItem != null)
                        menuItem.Site = value;
                    else
                        throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem Size property.
            /// <seealso cref="System.Windows.Forms.Control.Size"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Size Size
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.Size;

                    if (menuItem != null)
                        return menuItem.Size;

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
                set
                {
                    if (mainMenu != null)
                        mainMenu.Size = value;
                    else if (menuItem != null)
                        menuItem.Size = value;
                    else
                        throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip TabIndex property.
            /// <seealso cref="System.Windows.Forms.Control.TabIndex"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set or 
            /// if the internal object does not support the property.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new int TabIndex
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.TabIndex;

                    if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
                set
                {
                    if (mainMenu != null)
                        mainMenu.TabIndex = value;
                    else if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");
                    else
                        throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip TabStop property.
            /// <seealso cref="System.Windows.Forms.Control.TabStop"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set or 
            /// if the internal object does not support the property.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new bool TabStop
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.TabStop;

                    if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
                set
                {
                    if (mainMenu != null)
                        mainMenu.TabStop = value;
                    else if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");
                    else
                        throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem Tag property.
            /// <seealso cref="System.Windows.Forms.Control.Tag"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new object Tag
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.Tag;

                    if (menuItem != null)
                        return menuItem.Tag;

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
                set
                {
                    if (mainMenu != null)
                        mainMenu.Tag = value;
                    else if (menuItem != null)
                        menuItem.Tag = value;
                    else
                        throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem Text property.
            /// <seealso cref="System.Windows.Forms.Control.Text"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set.</exception>
            public override string Text
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.Text;

                    if (menuItem != null)
                        return menuItem.Text;

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
                set
                {
                    if (mainMenu != null)
                        mainMenu.Text = value;
                    else if (menuItem != null)
                        menuItem.Text = value;
                    else
                        throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip Top property.
            /// <seealso cref="System.Windows.Forms.Control.Top"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set or 
            /// if the internal object does not support the property.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new int Top
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.Top;

                    if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
                set
                {
                    if (mainMenu != null)
                        mainMenu.Top = value;
                    else if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");
                    else
                        throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip TopLevelControl property.
            /// <seealso cref="System.Windows.Forms.Control.TopLevelControl"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set or 
            /// if the internal object does not support the property.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Control TopLevelControl
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.TopLevelControl;

                    if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip UseWaitCursor property.
            /// <seealso cref="System.Windows.Forms.Control.Left"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set or 
            /// if the internal object does not support the property.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new bool UseWaitCursor
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.UseWaitCursor;

                    if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
                set
                {
                    if (mainMenu != null)
                        mainMenu.UseWaitCursor = value;
                    else if (menuItem != null)
                        throw new NotSupportedException("AIS-Exception, Object does not support the property");
                    else
                        throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }


            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem Visible property.
            /// <seealso cref="System.Windows.Forms.Control.Visible"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new bool Visible
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.Visible;

                    if (menuItem != null)
                        return menuItem.Available;

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
                set
                {
                    if (mainMenu != null)
                        mainMenu.Visible = value;
                    else if (menuItem != null)
                        menuItem.Available = value;
                    else
                        throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem Width property.
            /// <seealso cref="System.Windows.Forms.Control.Width"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new int Width
            {
                get
                {
                    if (mainMenu != null)
                        return mainMenu.Width;

                    if (menuItem != null)
                        return menuItem.Width;

                    throw new NotSupportedException("AIS-Exception, Object not set");
                }
                set
                {
                    if (mainMenu != null)
                        mainMenu.Width = value;
                    else if (menuItem != null)
                        menuItem.Width = value;
                    else
                        throw new NotSupportedException("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Not supported.
            /// </summary>
            /// <exception cref="System.Exception">Throws an exception indicating that 
            /// it is not a supported property.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new IWindowTarget WindowTarget
            {
                get
                {
                    throw new NotSupportedException("AIS-Exception, Object does not support the property");
                }
                set
                {
                    throw new NotSupportedException("AIS-Exception, Object does not support the property");
                }
            }


            /// <summary>
            /// Sets the Begin Invoke method
            /// </summary>
            /// <param name="method">Pointer to method to call</param>
            /// <returns>The asynchronous result.</returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new IAsyncResult BeginInvoke(Delegate method)
            {
                if (mainMenu != null)
                    return mainMenu.BeginInvoke(method);

                if (menuItem != null)
                    throw new NotSupportedException("AIS-Exception, Object does not support the method");

                throw new NotSupportedException("AIS-Exception, Object not set");
            }
            /// <summary>
            /// Sets the Begin Invoke method
            /// </summary>
            /// <param name="method">Pointer to method to call</param>
            /// <param name="args">Array of parameters to use</param>
            /// <returns>The asynchronous result.</returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new IAsyncResult BeginInvoke(Delegate method, params object[] args)
            {
                if (mainMenu != null)
                    return mainMenu.BeginInvoke(method, args);

                if (menuItem != null)
                    throw new NotSupportedException("AIS-Exception, Object does not support the method");

                throw new NotSupportedException("AIS-Exception, Object not set");
            }

            /// <summary>
            /// If the internal object is a MenuStrip, it calls the BringToFront method.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void BringToFront()
            {
                if (mainMenu != null)
                    mainMenu.BringToFront();

                if (menuItem != null)
                    throw new NotSupportedException("AIS-Exception, Object does not support the method");

                throw new NotSupportedException("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Not supported.
            /// </summary>
            /// <param name="ctl">Not supported.</param>
            /// <returns>Not supported.</returns>
            /// <exception cref="System.Exception">Throws an exception indicating that it is not supported.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new bool Contains(Control ctl)
            {
                throw new Exception("AIS-Exception, Object does not support the method");
            }

            /// <summary>
            /// Creates Control
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void CreateControl()
            {
                if (mainMenu != null)
                    mainMenu.CreateControl();

                if (menuItem != null)
                    throw new NotSupportedException("AIS-Exception, Object does not support the method");

                throw new NotSupportedException("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Creates Graphics
            /// </summary>
            /// <returns>The created Graphics object.</returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Graphics CreateGraphics()
            {
                if (mainMenu != null)
                    return mainMenu.CreateGraphics();

                if (menuItem != null)
                    throw new NotSupportedException("AIS-Exception, Object does not support the method");

                throw new NotSupportedException("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Do Drag and Drop use data object and effects
            /// </summary>
            /// <param name="data">Data object</param>
            /// <param name="allowedEffects">DragDropEffects enumeration</param>
            /// <returns>Exception in case is not set the main menu or menu item</returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new DragDropEffects DoDragDrop(object data, DragDropEffects allowedEffects)
            {
                if (mainMenu != null)
                    return mainMenu.DoDragDrop(data, allowedEffects);

                if (menuItem != null)
                    return menuItem.DoDragDrop(data, allowedEffects);

                throw new NotSupportedException("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Draws a bitmap in the Rectangle target position
            /// </summary>
            /// <param name="bitmap">Pointer to bitmap</param>
            /// <param name="targetBounds">Rectangle position values</param>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void DrawToBitmap(Bitmap bitmap, Rectangle targetBounds)
            {
                if (mainMenu != null)
                    mainMenu.DrawToBitmap(bitmap, targetBounds);

                if (menuItem != null)
                    throw new NotSupportedException("AIS-Exception, Object does not support the method");

                throw new NotSupportedException("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Call End Invoke method
            /// </summary>
            /// <param name="asyncResult">Use the IAsyncResult parameter</param>
            /// <returns>Returns object</returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new object EndInvoke(IAsyncResult asyncResult)
            {
                if (mainMenu != null)
                    return mainMenu.EndInvoke(asyncResult);

                if (menuItem != null)
                    throw new NotSupportedException("AIS-Exception, Object does not support the method");

                throw new NotSupportedException("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Finds the Form
            /// </summary>
            /// <returns>Form found</returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Form FindForm()
            {
                if (mainMenu != null)
                    return mainMenu.FindForm();

                if (menuItem != null)
                    throw new NotSupportedException("AIS-Exception, Object does not support the method");

                throw new NotSupportedException("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Sets the Focus
            /// </summary>
            /// <returns>True if successful, false otherwise.</returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new bool Focus()
            {
                if (mainMenu != null)
                    return mainMenu.Focus();

                if (menuItem != null)
                    throw new NotSupportedException("AIS-Exception, Object does not support the method");

                throw new NotSupportedException("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Retrieves the control that has the specific handle.
            /// </summary>
            /// <param name="handle">The window handle (HWND) to search for.</param>
            /// <returns>Control with the specified handle.</returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new static Control FromChildHandle(IntPtr handle)
            {
                return Control.FromChildHandle(handle);
            }

            /// <summary>
            /// Returns the control that is associated to the specified handle.
            /// </summary>
            /// <param name="handle">The window handle (HWND) to search for.</param>
            /// <returns>Control with the specified handle.</returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new static Control FromHandle(IntPtr handle)
            {
                return Control.FromHandle(handle);
            }
            /// <summary>
            /// It's not supported.
            /// </summary>
            /// <param name="pt">Not supported.</param>
            /// <returns>Not supported.</returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Control GetChildAtPoint(Point pt)
            {
                throw new NotSupportedException("AIS-Exception, Object does not support the method");
            }

            /// <summary>
            /// It's not supported
            /// </summary>
            /// <param name="pt">Not supported.</param>
            /// <param name="skipValue">Not supported.</param>
            /// <returns>Not supported.</returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Control GetChildAtPoint(Point pt, GetChildAtPointSkip skipValue)
            {
                throw new Exception("AIS-Exception, Object does not support the method");
            }

            /// <summary>
            /// Gets Parent control.
            /// </summary>
            /// <returns>The parent control.</returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new IContainerControl GetContainerControl()
            {
                if (mainMenu != null)
                    return mainMenu.GetContainerControl();

                if (menuItem != null)
                    throw new NotSupportedException("AIS-Exception, Object does not support the method");

                throw new NotSupportedException("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Gets Next or Back Control in the tab order
            /// </summary>
            /// <param name="ctl">Control to start the search</param>
            /// <param name="forward">Next or Back</param>
            /// <returns>The specified control.</returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Control GetNextControl(Control ctl, bool forward)
            {
                if (mainMenu != null)
                    return mainMenu.GetNextControl(ctl, forward);

                if (menuItem != null)
                    throw new NotSupportedException("AIS-Exception, Object does not support the method");

                throw new NotSupportedException("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Retrieves the size of a rectangular area into which a control can be fitted
            /// </summary>
            /// <param name="proposedSize">Custom size area</param>
            /// <returns>Returns size used</returns>
            public override Size GetPreferredSize(Size proposedSize)
            {
                if (mainMenu != null)
                    return mainMenu.GetPreferredSize(proposedSize);

                if (menuItem != null)
                    return menuItem.GetPreferredSize(proposedSize);

                throw new Exception("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Hides the control
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void Hide()
            {
                if (mainMenu != null)
                    mainMenu.Hide();

                if (menuItem != null)
                    throw new NotSupportedException("AIS-Exception, Object does not support the method");

                throw new NotSupportedException("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Invalidates the specified region of the control
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void Invalidate()
            {
                if (mainMenu != null)
                    mainMenu.Invalidate();

                if (menuItem != null)
                    menuItem.Invalidate();

                throw new NotSupportedException("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Invalidates the specified region of the control
            /// </summary>
            /// <param name="invalidateChildren">Invalidate the control's children as well.</param>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void Invalidate(bool invalidateChildren)
            {
                if (mainMenu != null)
                    mainMenu.Invalidate(invalidateChildren);

                if (menuItem != null)
                    throw new NotSupportedException("AIS-Exception, Object does not support the method");

                throw new NotSupportedException("AIS-Exception, Object not set");
            }
            /// <summary>
            /// Invalidates the specified region of the control
            /// </summary>
            /// <param name="rc">Use Rectangle area</param>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void Invalidate(Rectangle rc)
            {
                if (mainMenu != null)
                    mainMenu.Invalidate(rc);

                if (menuItem != null)
                    menuItem.Invalidate(rc);

                throw new NotSupportedException("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Invalidates the specified region of the control
            /// </summary>
            /// <param name="region">Use the Region area</param>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void Invalidate(Region region)
            {
                if (mainMenu != null)
                    mainMenu.Invalidate(region);

                if (menuItem != null)
                    throw new NotSupportedException("AIS-Exception, Object does not support the method");

                throw new NotSupportedException("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Invalidates the specified region of the control
            /// </summary>
            /// <param name="rc">Use the Rectangle Area</param>
            /// <param name="invalidateChildren">Invalidates children too?</param>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void Invalidate(Rectangle rc, bool invalidateChildren)
            {
                if (mainMenu != null)
                    mainMenu.Invalidate(rc, invalidateChildren);

                if (menuItem != null)
                    throw new NotSupportedException("AIS-Exception, Object does not support the method");

                throw new NotSupportedException("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Invalidates the specified region of the control
            /// </summary>
            /// <param name="region">Use the Region area</param>
            /// <param name="invalidateChildren">Invalidates children too?</param>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void Invalidate(Region region, bool invalidateChildren)
            {
                if (mainMenu != null)
                    mainMenu.Invalidate(region, invalidateChildren);

                if (menuItem != null)
                    throw new NotSupportedException("AIS-Exception, Object does not support the method");

                throw new NotSupportedException("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Execute the specified delegate method
            /// </summary>
            /// <param name="method">A delegate method to call in the control context</param>
            /// <returns>The return value from the delegate being invoked, or null if the delegate has no return value.</returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new object Invoke(Delegate method)
            {
                if (mainMenu != null)
                    return mainMenu.Invoke(method);

                if (menuItem != null)
                    throw new NotSupportedException("AIS-Exception, Object does not support the method");

                throw new NotSupportedException("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Execute the specified method with the parameters
            /// </summary>
            /// <param name="method">A delegate method to call in the control context</param>
            /// <param name="args">Use the array of arguments</param>
            /// <returns>The return value from the delegate being invoked, or null if the delegate has no return value.</returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new object Invoke(Delegate method, params object[] args)
            {
                if (mainMenu != null)
                    return mainMenu.Invoke(method, args);

                if (menuItem != null)
                    throw new NotSupportedException("AIS-Exception, Object does not support the method");

                throw new NotSupportedException("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Determines whether the CAPS LOCK, NUM LOCK or SCROLL LOCK key is on.
            /// </summary>
            /// <param name="keyVal">The keys for which to check.</param>
            /// <returns>Returns true if the specified keys are on..</returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new static bool IsKeyLocked(Keys keyVal)
            {
                return Control.IsKeyLocked(keyVal);
            }

            /// <summary>
            /// Is mnemonic the char code for the control in the specified text.
            /// </summary>
            /// <param name="charCode">Char code to look up</param>
            /// <param name="text">Specified text</param>
            /// <returns>true if is mnemonic</returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new static bool IsMnemonic(char charCode, string text)
            {
                return Control.IsMnemonic(charCode, text);
            }

            /// <summary>
            /// Force to perform layout logic and it's children too.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void PerformLayout()
            {
                if (mainMenu != null)
                    mainMenu.PerformLayout();

                if (menuItem != null)
                    throw new NotSupportedException("AIS-Exception, Object does not support the method");

                throw new NotSupportedException("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Force to perform layout logic and it's children too.
            /// </summary>
            /// <param name="affectedControl">Control recently changed</param>
            /// <param name="affectedProperty">Name of the control</param>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void PerformLayout(Control affectedControl, string affectedProperty)
            {
                if (mainMenu != null)
                    mainMenu.PerformLayout(affectedControl, affectedProperty);

                if (menuItem != null)
                    throw new NotSupportedException("AIS-Exception, Object does not support the method");

                throw new NotSupportedException("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Computes the location of the specified screen
            /// </summary>
            /// <param name="p">Screen coordinate to convert</param>
            /// <returns>The new Point</returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Point PointToClient(Point p)
            {
                if (mainMenu != null)
                    return mainMenu.PointToClient(p);

                if (menuItem != null)
                    throw new NotSupportedException("AIS-Exception, Object does not support the method");

                throw new NotSupportedException("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Computes the location of the specified screen
            /// </summary>
            /// <param name="p">Point to convert</param>
            /// <returns>The Point in screen coordinates.</returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Point PointToScreen(Point p)
            {
                if (mainMenu != null)
                    return mainMenu.PointToScreen(p);

                if (menuItem != null)
                    throw new NotSupportedException("AIS-Exception, Object does not support the method");

                throw new NotSupportedException("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Preprocess the keyboard or input messages
            /// </summary>
            /// <param name="msg">String message to process</param>
            /// <returns>One of the PreProcessControlState values, depending on whether PreProcessMessage is true or false and whether IsInputKey or IsInputChar are true or false.</returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new PreProcessControlState PreProcessControlMessage(ref Message msg)
            {
                if (mainMenu != null)
                    return mainMenu.PreProcessControlMessage(ref msg);

                if (menuItem != null)
                    throw new NotSupportedException("AIS-Exception, Object does not support the method");

                throw new NotSupportedException("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Preprocess the keyboard or input messages
            /// </summary>
            /// <param name="msg">Message to process</param>
            /// <returns>True if the message was processed by the control; otherwise, false.</returns>
            public override bool PreProcessMessage(ref Message msg)
            {
                if (mainMenu != null)
                    return mainMenu.PreProcessMessage(ref msg);

                if (menuItem != null)
                    throw new NotSupportedException("AIS-Exception, Object does not support the property");

                throw new NotSupportedException("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Computes the Rectangle to Client of the specific location
            /// </summary>
            /// <param name="r">Rectangle location to convert</param>
            /// <returns>Converted Rectangle</returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Rectangle RectangleToClient(Rectangle r)
            {
                if (mainMenu != null)
                    return mainMenu.RectangleToClient(r);

                if (menuItem != null)
                    throw new NotSupportedException("AIS-Exception, Object does not support the method");

                throw new NotSupportedException("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Computes the Rectangle to Client of the specific location
            /// </summary>
            /// <param name="r">Rectangle area to convert</param>
            /// <returns>Resulted Rectangle </returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Rectangle RectangleToScreen(Rectangle r)
            {
                if (mainMenu != null)
                    return mainMenu.RectangleToScreen(r);

                if (menuItem != null)
                    throw new NotSupportedException("AIS-Exception, Object does not support the method");

                throw new NotSupportedException("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Invalidates and Redraw the control
            /// </summary>
            public override void Refresh()
            {
                if (mainMenu != null)
                    mainMenu.Refresh();

                if (menuItem != null)
                    throw new NotSupportedException("AIS-Exception, Object does not support the property");

                throw new NotSupportedException("AIS-Exception, Object not set");
            }

            /// <summary>
            /// It's not supported
            /// </summary>
            public override void ResetBackColor()
            {
                throw new NotSupportedException("AIS-Exception, Object does not support the property");
            }

            /// <summary>
            /// It's not supported
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void ResetBindings()
            {
                throw new NotSupportedException("AIS-Exception, Object does not support the method");
            }

            /// <summary>
            /// Is not supported
            /// </summary>
            public override void ResetCursor()
            {
                throw new NotSupportedException("AIS-Exception, Object does not support the property");
            }

            /// <summary>
            /// It's not supported
            /// </summary>
            public override void ResetFont()
            {
                throw new NotSupportedException("AIS-Exception, Object does not support the property");
            }

            /// <summary>
            /// Is not supported
            /// </summary>
            public override void ResetForeColor()
            {
                throw new NotSupportedException("AIS-Exception, Object does not support the property");
            }

            /// <summary>
            /// Is not supported
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void ResetImeMode()
            {
                throw new NotSupportedException("AIS-Exception, Object does not support the method");
            }

            /// <summary>
            /// It's not supported
            /// </summary>
            public override void ResetRightToLeft()
            {
                throw new NotSupportedException("AIS-Exception, Object does not support the property");
            }

            /// <summary>
            /// Sets default text to Text property
            /// </summary>
            public override void ResetText()
            {
                if (mainMenu != null)
                    mainMenu.ResetText();

                if (menuItem != null)
                    throw new NotSupportedException("AIS-Exception, Object does not support the property");

                throw new NotSupportedException("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Resume usual layout logic
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void ResumeLayout()
            {
                if (mainMenu != null)
                    mainMenu.ResumeLayout();

                if (menuItem != null)
                    throw new NotSupportedException("AIS-Exception, Object does not support the method");

                throw new NotSupportedException("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Resume usual layout logic and performs pending request
            /// </summary>
            /// <param name="performLayout">True to perform pending layout requests.</param>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void ResumeLayout(bool performLayout)
            {
                if (mainMenu != null)
                    mainMenu.ResumeLayout(performLayout);

                if (menuItem != null)
                    throw new NotSupportedException("AIS-Exception, Object does not support the method");

                throw new NotSupportedException("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Scale the controls to ratio
            /// </summary>
            /// <param name="ratio">The ratio with which to scale.</param>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void Scale(float ratio)
            {
                if (mainMenu != null)
                    mainMenu.Scale(new SizeF(ratio, ratio));
                if (menuItem != null)
                    throw new NotSupportedException("AIS-Exception, Object does not support the method");

                throw new NotSupportedException("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Scale the controls to size factor
            /// </summary>
            /// <param name="factor">A size containing the horizontal and vertical scale factors.</param>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void Scale(SizeF factor)
            {
                if (mainMenu != null)
                    mainMenu.Scale(factor);

                if (menuItem != null)
                    throw new NotSupportedException("AIS-Exception, Object does not support the method");

                throw new NotSupportedException("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Scale the controls to specified size.
            /// </summary>
            /// <param name="dx">The horizontal size</param>
            /// <param name="dy">The vertical size</param>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void Scale(float dx, float dy)
            {
                if (mainMenu != null)
                    mainMenu.Scale(new SizeF(dx, dy));

                if (menuItem != null)
                    throw new NotSupportedException("AIS-Exception, Object does not support the method");

                throw new NotSupportedException("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Activate the control
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void Select()
            {
                if (mainMenu != null)
                    mainMenu.Select();

                if (menuItem != null)
                    menuItem.Select();

                throw new NotSupportedException("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Activates the next control
            /// </summary>
            /// <param name="ctl">The starting control to search</param>
            /// <param name="forward">Is Forward or Backward?</param>
            /// <param name="tabStopOnly">Use the Tab Stop?</param>
            /// <param name="nested">Search in children?</param>
            /// <param name="wrap">Go to first control and continue search?</param>
            /// <returns>True if a control was activated; otherwise, false.</returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new bool SelectNextControl(Control ctl, bool forward, bool tabStopOnly, bool nested, bool wrap)
            {
                if (mainMenu != null)
                    return mainMenu.SelectNextControl(ctl, forward, tabStopOnly, nested, wrap);

                if (menuItem != null)
                    throw new NotSupportedException("AIS-Exception, Object does not support the method");

                throw new NotSupportedException("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Send the control back to z-order
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void SendToBack()
            {
                if (mainMenu != null)
                    mainMenu.SendToBack();

                if (menuItem != null)
                    throw new NotSupportedException("AIS-Exception, Object does not support the method");

                throw new NotSupportedException("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Specific bounds for location and size
            /// </summary>
            /// <param name="x">X position</param>
            /// <param name="y">Y position</param>
            /// <param name="width">Width size</param>
            /// <param name="height">Height size</param>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void SetBounds(int x, int y, int width, int height)
            {
                if (mainMenu != null)
                    mainMenu.SetBounds(x, y, width, height);

                if (menuItem != null)
                    throw new NotSupportedException("AIS-Exception, Object does not support the method");

                throw new NotSupportedException("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Sets the bounds of the MenuItemControl.
            /// </summary>
            /// <param name="x">The x position.</param>
            /// <param name="y">The y position.</param>
            /// <param name="width">Width size.</param>
            /// <param name="height">Height size.</param>
            /// <param name="specified">Bounds specified, do a bitwise between specified and parameters.</param>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void SetBounds(int x, int y, int width, int height, BoundsSpecified specified)
            {
                if (mainMenu != null)
                    mainMenu.SetBounds(x, y, width, height, specified);

                if (menuItem != null)
                    throw new NotSupportedException("AIS-Exception, Object does not support the method");

                throw new NotSupportedException("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Displays the control to user
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void Show()
            {
                if (mainMenu != null)
                    mainMenu.Show();

                if (menuItem != null)
                    throw new NotSupportedException("AIS-Exception, Object does not support the method");

                throw new NotSupportedException("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Temporary suspend the layout logic to user
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void SuspendLayout()
            {
                if (mainMenu != null)
                    mainMenu.SuspendLayout();

                if (menuItem != null)
                    throw new NotSupportedException("AIS-Exception, Object does not support the method");

                throw new NotSupportedException("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Redraw the control to the invalidated areas
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void Update()
            {
                if (mainMenu != null)
                    mainMenu.Update();

                if (menuItem != null)
                    throw new NotSupportedException("AIS-Exception, Object does not support the method");

                throw new NotSupportedException("AIS-Exception, Object not set");
            }

        }

        /// <summary>
        /// Menu Items Collection
        /// </summary>
        public class MenuItemsCollection : Control.ControlCollection, IEnumerator<Control>, IEnumerable<Control>, IDisposable
        {
            private IEnumerator _controlEnumerator;

            /// <summary>
            /// Constructor Menu Items Collection
            /// </summary>
            /// <param name="owner">Use the menu system for a form</param>
            public MenuItemsCollection(MenuStrip owner)
                : base(owner)
            {
                _controlEnumerator = owner.Items.GetEnumerator();
            }

            /// <summary>
            /// Constructor Menu Items Collection
            /// </summary>
            /// <param name="owner">Adds the ToolStripItem</param>
            public MenuItemsCollection(ToolStripItem owner)
                : base(new MenuItemControl(owner))
            {
                if (owner is ToolStripMenuItem)
                    _controlEnumerator = ((ToolStripMenuItem)owner).DropDownItems.GetEnumerator();
            }

            /// <summary>
            /// Overwriting for inherited members ControlCollection
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Control Owner
            {
                get
                {
                    throw new NotSupportedException("AIS-Exception, Implementation of member not supported");
                }
            }

            /// <summary>
            /// Array access using an index
            /// </summary>
            /// <param name="index">Index position</param>
            /// <returns>Control at index position</returns>
            public override Control this[int index]
            {
                get
                {
                    Control result = null;
                    MenuStrip strip = base.Owner as MenuStrip;
                    if (strip != null)
                        result = new MenuItemControl(strip.Items[index]);

                    MenuItemControl owner = base.Owner as MenuItemControl;
                    if (owner != null)
                        result = owner.Controls[index];

                    return result;
                }
            }

            /// <summary>
            /// Array access using key string
            /// </summary>
            /// <param name="key">String name of the control</param>
            /// <returns>Control indexed with key name</returns>
            public override Control this[string key]
            {
                get
                {
                    Control result = null;
                    MenuStrip strip = base.Owner as MenuStrip;
                    if (strip != null)
                        result = new MenuItemControl(strip.Items[key]);

                    MenuItemControl owner = base.Owner as MenuItemControl;
                    if (owner != null)
                        result = owner.Controls[key];

                    return result;
                }
            }

            /// <summary>
            /// Not supported.
            /// </summary>
            /// <param name="value">Not supported.</param>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public override void Add(Control value)
            {
                throw new NotSupportedException("AIS-Exception, Implementation of member not supported");
            }

            /// <summary>
            /// Not supported.
            /// </summary>
            /// <param name="controls">Not supported.</param>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public override void AddRange(Control[] controls)
            {
                throw new NotSupportedException("AIS-Exception, Implementation of member not supported");
            }

            /// <summary>
            /// Not supported.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public override void Clear()
            {
                throw new NotSupportedException("AIS-Exception, Implementation of member not supported");
            }

            /// <summary>
            /// Not supported.
            /// </summary>
            /// <param name="control">Not supported.</param>
            /// <returns>Not supported.</returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new bool Contains(Control control)
            {
                throw new NotSupportedException("AIS-Exception, Implementation of member not supported");
            }

            /// <summary>
            /// Not supported.
            /// </summary>
            /// <param name="key">Not supported.</param>
            /// <returns>Not supported.</returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public override bool ContainsKey(string key)
            {
                throw new NotSupportedException("AIS-Exception, Implementation of member not supported");
            }

            /// <summary>
            /// Not supported.
            /// </summary>
            /// <param name="key">Not supported.</param>
            /// <param name="searchAllChildren">Not supported.</param>
            /// <returns>Not supported.</returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Control[] Find(string key, bool searchAllChildren)
            {
                throw new NotSupportedException("AIS-Exception, Implementation of member not supported");
            }

            /// <summary>
            /// Not supported.
            /// </summary>
            /// <param name="child">Not supported.</param>
            /// <returns>Not supported.</returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new int GetChildIndex(Control child)
            {
                throw new NotSupportedException("AIS-Exception, Implementation of member not supported");
            }

            /// <summary>
            /// Not supported.
            /// </summary>
            /// <param name="child">Not supported.</param>
            /// <param name="throwException">Not supported.</param>
            /// <returns>Not supported.</returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public override int GetChildIndex(Control child, bool throwException)
            {
                throw new Exception("AIS-Exception, Implementation of member not supported");
            }

            /// <summary>
            /// Returns the instance collection.
            /// </summary>
            /// <returns>The instance collection.</returns>
            IEnumerator<Control> IEnumerable<Control>.GetEnumerator()
            {
                return this;
            }

            /// <summary>
            /// Returns the Enumerator instance.
            /// </summary>
            /// <returns>The enumerator instance.</returns>
            public override IEnumerator GetEnumerator()
            {
                return this;
            }

            /// <summary>
            /// Not supported.
            /// </summary>
            /// <param name="control">Not supported.</param>
            /// <returns>Not supported.</returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new int IndexOf(Control control)
            {
                throw new NotSupportedException("AIS-Exception, Implementation of member not supported");
            }

            /// <summary>
            /// Not supported.
            /// </summary>
            /// <param name="key">Not supported.</param>
            /// <returns>Not supported.</returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public override int IndexOfKey(string key)
            {
                throw new NotSupportedException("AIS-Exception, Implementation of member not supported");
            }

            /// <summary>
            /// It's Not supported.
            /// </summary>
            /// <param name="value">Not supported.</param>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public override void Remove(Control value)
            {
                throw new NotSupportedException("AIS-Exception, Implementation of member not supported");
            }

            /// <summary>
            /// It's Not supported.
            /// </summary>
            /// <param name="index">Not supported.</param>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void RemoveAt(int index)
            {
                throw new NotSupportedException("AIS-Exception, Implementation of member not supported");
            }

            /// <summary>
            /// It's Not supported.
            /// </summary>
            /// <param name="key">Not supported.</param>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public override void RemoveByKey(string key)
            {
                throw new NotSupportedException("AIS-Exception, Implementation of member not supported");
            }

            /// <summary>
            /// Not supported.
            /// </summary>
            /// <param name="child">Not supported.</param>
            /// <param name="newIndex">Not supported.</param>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public override void SetChildIndex(Control child, int newIndex)
            {
                throw new NotSupportedException("AIS-Exception, Implementation of member not supported");
            }


            /// <summary>
            /// Current element in the collection as a control
            /// </summary>
            Control IEnumerator<Control>.Current
            {
                get
                {
                    if (_controlEnumerator != null)
                    {
                        if (_controlEnumerator.Current != null)
                            return new MenuItemControl((ToolStripItem)_controlEnumerator.Current);
                    }
                    return null;
                }
            }

            /// <summary>
            /// Current element in the collection as an object
            /// </summary>
            public object Current
            {
                get
                {
                    if (_controlEnumerator != null)
                    {
                        if (_controlEnumerator.Current != null)
                            return new MenuItemControl((ToolStripItem)_controlEnumerator.Current);
                    }
                    return null;
                }
            }

            /// <summary>
            /// Finalizes an instance of the <see cref="MenuItemsCollection"/> class.
            /// </summary>
            ~MenuItemsCollection()
            {
                Dispose(false);
            }

            /// <summary>
            /// Disposes the instance
            /// </summary>
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            private bool _disposed;

            /// <summary>
            /// Disposes the instance
            /// </summary>
            /// <param name="disposing">Indicates if the call is made from the Dispose method.</param>
            protected virtual void Dispose(bool disposing)
            {
                if (!_disposed)
                {
                    if (disposing)
                    {
                        //No managed resources to dispose
                    }
                }
                //No unmanaged resources to dispose
                _disposed = true;
            }

            /// <summary>
            /// Advance to next control in the collection
            /// </summary>
            /// <returns>True if successful, false otherwise.</returns>
            public bool MoveNext()
            {
                bool hasNext = false;

                if (_controlEnumerator != null)
                    hasNext = _controlEnumerator.MoveNext();

                return hasNext;
            }

            /// <summary>
            /// Internal enumerator is set to empty
            /// </summary>
            public void Reset()
            {
                _controlEnumerator = null;
            }
        }
        /// <summary>
        /// Returns the NestedControlEnumerator for the control
        /// </summary>
        /// <param name="control">Used to get the NestedControlEnumerator</param>
        /// <returns>The NestedControlEnumerator for the control.</returns>
        public static NestedControlEnumerator Controls(Control control)
        {
            return new NestedControlEnumerator(control);
        }

        /// <summary>
        /// A structure to store the list of events for an object.
        /// </summary>
        private static readonly WeakDictionary<object, Dictionary<string, Delegate[]>> EventSubscribersCache = new WeakDictionary<object, Dictionary<string, Delegate[]>>();

        /// <summary>
        /// Gets the delegates bound to an event in an object.
        /// </summary>
        /// <param name="target">The object.</param>
        /// <param name="eventName">The event name.</param>
        /// <returns>Null if no delegates or event were found.</returns>
        public static Delegate[] GetEventSubscribers(object target, string eventName)
        {
            if (EventSubscribersCache.ContainsKey(target) && EventSubscribersCache[target].ContainsKey(eventName))
                return EventSubscribersCache[target][eventName];

            string[] winFormsEventsName = new string[] { "Event" + eventName, "Event_" + eventName, "EVENT" + eventName.ToUpper(), "EVENT_" + eventName.ToUpper() };
            Type targetType = target.GetType();
            FieldInfo fieldInfo = null;

            while (targetType != null)
            {
                //Look for a field in the Target with the name of the event
                fieldInfo = targetType.GetField(eventName, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
                Delegate del = null;
                if (fieldInfo != null)
                {
                    //Gets the current value in the Target instance
                    del = (Delegate)fieldInfo.GetValue(target);
                    if (del != null)
                    {
                        AddListOfEventsToChache(target, eventName, del.GetInvocationList());
                        return EventSubscribersCache[target][eventName];
                    }
                }
                else
                {
                    foreach (string winEventName in winFormsEventsName)
                    {
                        //Look for a field in the Target with the name of the event as defined in some cases
                        fieldInfo = targetType.GetField(winEventName, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
                        if (fieldInfo != null)
                        {
                            EventHandlerList eventHandlerList = (EventHandlerList)target.GetType().GetProperty("Events", BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(target, null);

                            del = eventHandlerList[fieldInfo.GetValue(target)];
                            if (del != null)
                            {
                                AddListOfEventsToChache(target, eventName, del.GetInvocationList());
                                return EventSubscribersCache[target][eventName];
                            }
                        }
                    }
                }

                //Repeats the process in the base types if nothing has been found so far
                targetType = targetType.BaseType;
            }

            AddListOfEventsToChache(target, eventName, null);
            return null;
        }

        /// <summary>
        /// Method to add a list of events to the cache.
        /// </summary>
        /// <param name="target">The object target to use as key.</param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="delList">The list of event handlers.</param>
        private static void AddListOfEventsToChache(object target, string eventName, Delegate[] delList)
        {
            if (!EventSubscribersCache.ContainsKey(target))
            {
                EventSubscribersCache.Add(target, new Dictionary<string, Delegate[]>());
                Component cmp = target as Component;
                if (cmp != null)
                    cmp.Disposed += Component_Disposed;
            }

            if (!EventSubscribersCache[target].ContainsKey(eventName))
                EventSubscribersCache[target].Add(eventName, delList);
            else
                EventSubscribersCache[target][eventName] = delList;
        }

        /// <summary>
        /// Event handler release resources when a component is disposed.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private static void Component_Disposed(object sender, EventArgs e)
        {
            try
            {
                if (EventSubscribersCache.ContainsKey(sender))
                    EventSubscribersCache.Remove(sender);
            }
            catch
            {
            }
        }
    }

    /// <summary>
    /// To flatten the .NET controls collection and expose all controls in a 1-dimensional array, this 
    /// IEnumerator implementation is provided that enumerates the controls contained
    /// by the given control and all their children too.
    /// </summary>
    public class NestedControlEnumerator : IEnumerator<Control>, IEnumerable<Control>
    {

        /// <summary>
        /// Fields to use with the IEnumerator.
        /// </summary>
        private NestedControlEnumerator _currentNestedEnumerator;
        private readonly IEnumerator _controlEnumerator;
        private Boolean _currentIsValid;

        /// <summary>
        /// Reference to the control at was used to create this enumerator.
        /// </summary>
        private readonly Control _control;
        private readonly bool mustDisposeControl;


        /// <summary>
        /// Creates an enumerator to transverse thru the control and all its children components.
        /// </summary>
        /// <param name="control">The root component to start the iteration.</param>
        public NestedControlEnumerator(Control control)
        {
            this._control = control;
            _controlEnumerator = control.Controls.GetEnumerator();
        }

        /// <summary>
        /// Creates an enumerator to transverse thru a MenuStrip and all its children components.
        /// </summary>
        /// <param name="menu">The menu strip component where the control enumeration will start.</param>
        public NestedControlEnumerator(MenuStrip menu)
        {
            mustDisposeControl = true;
            _control = new ContainerHelper.MenuItemControl(menu);
            _controlEnumerator = ((ContainerHelper.MenuItemControl)_control).Controls.GetEnumerator();
        }

        /// <summary>
        /// Creates an enumerator to transverse thru a ToolStripItem and all its children components.
        /// </summary>
        /// <param name="menuItem">The ToolStripItem component where the control enumeration will start.</param>
        public NestedControlEnumerator(ToolStripItem menuItem)
        {
            mustDisposeControl = true;
            _control = new ContainerHelper.MenuItemControl(menuItem);
            _controlEnumerator = ((ContainerHelper.MenuItemControl)_control).Controls.GetEnumerator();
        }

        #region IEnumerator, IEnumerable Implementation
        /// <summary>
        /// Properties and methods related with IEnumerator and IEnumerable.
        /// </summary>
        Control IEnumerator<Control>.Current
        {
            get
            {
                if (_currentNestedEnumerator != null)
                    return (Control)_currentNestedEnumerator.Current;
                return (Control)_controlEnumerator.Current;
            }
        }

        /// <summary>
        /// Returns the current control in the enumeration.
        /// </summary>
        public object Current
        {
            get
            {
                if (_currentNestedEnumerator != null)
                    return (Control)_currentNestedEnumerator.Current;
                else
                    return (Control)_controlEnumerator.Current;
            }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="NestedControlEnumerator"/> class.
        /// </summary>
        ~NestedControlEnumerator()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes the instance
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool disposed;
        /// <summary>
        /// Disposes the instance
        /// </summary>
        /// <param name="disposing">Indicates if the call is made from the Dispose method.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (_currentNestedEnumerator != null)
                        _currentNestedEnumerator.Dispose();
                    if (mustDisposeControl && _control != null)
                        _control.Dispose();
                }
            }
            //No unmanaged resources to dispose
            disposed = true;
        }

        /// <summary>
        /// Move to next Control in the collection.
        /// </summary>
        /// <returns>False if it is at the end of the collection, True otherwise.</returns>
        public bool MoveNext()
        {
            bool hasNext = false;
            if (_currentNestedEnumerator != null)
            {
                if (!(hasNext = _currentNestedEnumerator.MoveNext()))
                {
                    _currentNestedEnumerator.Dispose();
                    _currentNestedEnumerator = null;
                }
            }
            else if (_currentIsValid)
            {
                if (_controlEnumerator.Current is GroupBox || _controlEnumerator.Current is Panel || _controlEnumerator.Current is TabControl
                    || _controlEnumerator.Current is MenuStrip || _controlEnumerator.Current is PictureBox)
                {
                    _currentNestedEnumerator =
                        new NestedControlEnumerator((Control)_controlEnumerator.Current);
                    hasNext = MoveNext();
                }
                else if (_currentIsValid && (_controlEnumerator.Current is ContainerHelper.MenuItemControl)
                         && ((ContainerHelper.MenuItemControl)_controlEnumerator.Current).IsToolStripItem)
                {
                    _currentNestedEnumerator = new NestedControlEnumerator((ToolStripItem)((ContainerHelper.MenuItemControl)_controlEnumerator.Current));
                    hasNext = MoveNext();
                }
            }



            if (!hasNext)
                hasNext = _controlEnumerator.MoveNext();

            if (hasNext && (Current is MdiClient))
                hasNext = false;

            _currentIsValid = hasNext;

            //TabPages must be ommited but the constrols inside not
            if (hasNext && (Current is TabPage))
                hasNext = MoveNext();


            return hasNext;
        }

        /// <summary>
        /// Clears all internal structures, reset the enumerator to the initial state.
        /// </summary>
        public void Reset()
        {
            if (_currentNestedEnumerator != null)
                _currentNestedEnumerator.Dispose();
            _currentNestedEnumerator = null;
            _controlEnumerator.Reset();
            _currentIsValid = false;
        }

        /// <summary>
        /// Generics implementation to return an IEnumerator for Control.
        /// </summary>
        /// <returns>The enumerator.</returns>
        IEnumerator<Control> IEnumerable<Control>.GetEnumerator()
        {
            return this;
        }

        /// <summary>
        /// Provides an IEnumerator implementation.
        /// </summary>
        /// <returns>A collection reference that can be use to enumerate.</returns>
        public IEnumerator GetEnumerator()
        {
            return this;
        }


        /// <summary>
        /// Returns the control in the collection with the specified name.
        /// </summary>
        /// <param name="name">The name of the desired control.</param>
        /// <returns>The specified control.</returns>
        public Control this[String name]
        {
            get
            {
                Control result = _control.Controls[name];
                if (result == null)
                {
                    IDictionary<string, Control> nc = GetNestedControls();
                    if (nc.ContainsKey(name))
                        result = nc[name];
                    if (result == null)
                    {
                        //We need to look for a control array
                        Type type = _control.GetType();
                        FieldInfo finfo = type.GetField(name);
                        if (finfo != null)
                        {
                            object field_value = finfo.GetValue(_control);
                            if (field_value is Array)
                            {
                                return new Gui.ContainerHelper.ControlArray((Array)field_value);
                            }
                        }
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Returns the control in the collection at the specified index.
        /// </summary>
        /// <param name="index">The index of the desired control.</param>
        /// <returns>The specified control.</returns>
        public Control this[int index]
        {
            get
            {
                IList<Control> nc = GetIndexedNestedControls();
                return nc[index];
            }
        }

        /// <summary>
        /// Returns the number of Controls in the collection.
        /// </summary>
        public int Count
        {
            get
            {
                return GetIndexedNestedControls().Count;
            }
        }

        /// <summary>
        /// Removes the Control element at the specified position.
        /// </summary>
        /// <param name="i">The index at which the control should be removed.</param>
        public void RemoveAt(int i)
        {
            if (_currentNestedEnumerator != null)
            {
                _currentNestedEnumerator.RemoveAt(i);
            }
            else
            {
                _control.Controls.RemoveAt(i);
            }
        }
        #endregion

        /// <summary>
        /// Obtains a Dictionary mapping controls name to the control reference.
        /// </summary>
        /// <returns>A Dictionary mapping controls name to the control reference.</returns>
        private IDictionary<string, Control> GetNestedControls()
        {
            IDictionary<String, Control> nc = new SortedDictionary<String, Control>();
            Reset();
            foreach (Control ctl in this)
                nc[ctl.Name] = ctl;

            return nc;
        }

        /// <summary>
        /// Provides a list of all Controls in the collection.
        /// </summary>
        /// <returns>A list of all Controls in the collection.</returns>
        private IList<Control> GetIndexedNestedControls()
        {
            IList<Control> nc = new List<Control>();
            WeakReference wr = new WeakReference(nc);
            Reset();
            foreach (Control ctl in this)
                nc.Add(ctl);

            return nc;
        }

        /// <summary>
        /// Provides a list of all Controls in the collection.
        /// </summary>
        /// <returns>A list of all Controls in the collection.</returns>
        public IList GetControls()
        {
            IList nc = new List<Control>();
            WeakReference wr = new WeakReference(nc);
            Reset();
            foreach (Control ctl in this)
                nc.Add(ctl);

            return nc;
        }
    }
}
