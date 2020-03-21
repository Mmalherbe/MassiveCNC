using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using UpgradeHelpers.Helpers;

namespace UpgradeHelpers.Gui
{
    /// <summary>
    /// Extender that adds support to special functionality in ListBoxes, 
    /// for example the properties SelectionMode and Selected.
    /// </summary>
    [ProvideProperty("SelectionMode", typeof(ListBox))]
    public partial class ListBoxHelper : Component, IExtenderProvider
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ListBoxHelper()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="container">The container where to add the controls.</param>
        public ListBoxHelper(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        /// <summary>
        /// Contains the current selected indexes in the ListBox.
        /// </summary>
        private static readonly WeakDictionary<ListBox, int> _SelectedIndexList = new WeakDictionary<ListBox, int>();

        /// <summary>
        /// Determinate which controls can use these extra properties.
        /// </summary>
        /// <param name="extender">The object to test.</param>
        /// <returns>True if the object can extend the properties.</returns>
        public bool CanExtend(object extender)
        {
            return extender is ListBox;
        }

        /// <summary>
        /// Returns the current value of SelectionMode provided by this control. 
        /// It happens to be the same value of the ListBox control.
        /// </summary>
        /// <param name="lstBox">The control to get the SelectionMode.</param>
        /// <returns>The current SelectionMode assigned to the control.</returns>
        public SelectionMode GetSelectionMode(ListBox lstBox)
        {
            return lstBox.SelectionMode;
        }

        /// <summary>
        /// Sets the SelectionMode for a control.
        /// </summary>
        /// <param name="lstBox">The control to set the SelectionMode.</param>
        /// <param name="mode">The selection mode to set.</param>
        public void SetSelectionMode(ListBox lstBox, SelectionMode mode)
        {
            lstBox.SelectionMode = mode;
            if ((mode == SelectionMode.MultiExtended) ||
                (mode == SelectionMode.MultiSimple))
            {
                if (!_SelectedIndexList.ContainsKey(lstBox))
                {
                    _SelectedIndexList.Add(lstBox, 0);
                    lstBox.DrawMode = DrawMode.OwnerDrawFixed;
                    lstBox.DrawItem += ListBox_DrawItem;
                }
            }
            else
            {
                if (_SelectedIndexList.ContainsKey(lstBox))
                {
                    _SelectedIndexList.Remove(lstBox);
                    lstBox.DrawMode = DrawMode.Normal;
                    lstBox.DrawItem -= ListBox_DrawItem;
                }
            }
        }

        /// <summary>
        /// For MultiExtended and MultiSimple selection modes we will draw the items ourselves 
        /// to keep track of which item has the focus.
        /// </summary>
        /// <param name="sender">The ListBox raising the event.</param>
        /// <param name="e">The DrawItemEventArgs for the current item to draw.</param>
        private void ListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            ListBox lstBox = (ListBox)sender;
            e.DrawBackground();
            using (Brush myBrush = new SolidBrush(e.ForeColor))
            {
                if ((e.Index < 0) || (e.Index >= lstBox.Items.Count))
                    return;

                if (_SelectedIndexList.ContainsKey(lstBox))
                {
                    if ((e.State & DrawItemState.Focus) == DrawItemState.Focus)
                        _SelectedIndexList[lstBox] = e.Index;
                }

                // Draw the current item text based on the current Font and the custom brush settings.
                e.Graphics.DrawString(lstBox.Items[e.Index].ToString(), e.Font, myBrush, e.Bounds, StringFormat.GenericDefault);
                // If the ListBox has focus, draw a focus rectangle around the selected item.
                e.DrawFocusRectangle();
            }
        }

        /// <summary>
        /// Function to get the selected index of a ListBox. It depends on the selection mode property.
        /// </summary>
        /// <param name="lstBox">The listbox to return the SelectedIndex.</param>
        /// <returns>The current selected index for a list box or in the case of 
        /// SelectionMode = [MultiExtended|MultiSimple] it might throw an exception 
        /// if a ListBoxHelper component hasn't been added to the form with 
        /// the ListBox (The ListBoxHelper component will provide an extra 
        /// property to set SelectionMode).
        /// </returns>
        public static int GetSelectedIndex(ListBox lstBox)
        {
            if ((lstBox.SelectionMode == SelectionMode.MultiExtended) || (lstBox.SelectionMode == SelectionMode.MultiSimple))
            {
                if (_SelectedIndexList.ContainsKey(lstBox))
                    return _SelectedIndexList[lstBox];
                throw new Exception("SelectedIndex property not stored for a MultiSelect ListBox, "
                                    + "please add a ListBoxHelper to the form and set the property SelectionMode again");
            }
            return lstBox.SelectedIndex;
        }

        /// <summary>
        /// Function to set the selected index of a ListBox. Its behavior depends on 
        /// the selection mode property.
        /// </summary>
        /// <param name="lstBox">The listbox to set the SelectedIndex.</param>
        /// <param name="selectedIndex">The value to be set.</param>
        /// <returns>Returns the selectedIndex after the operation.</returns>
        public static int SetSelectedIndex(ListBox lstBox, int selectedIndex)
        {
            if ((lstBox.SelectionMode == SelectionMode.MultiSimple) || (lstBox.SelectionMode == SelectionMode.MultiExtended))
            {
                if (_SelectedIndexList.ContainsKey(lstBox))
                    _SelectedIndexList[lstBox] = selectedIndex;
                else
                    throw new Exception("SelectedIndex property not stored for a MultiSelect ListBox, "
                        + "please add a ListBoxHelper to the form and set the property SelectionMode again");

                int currSelectedIndex = lstBox.SelectedIndex;
                if ((selectedIndex > -1) && (selectedIndex < lstBox.Items.Count))
                {
                    bool mustBeClean = !lstBox.SelectedIndices.Contains(selectedIndex);

                    lstBox.SetSelected(selectedIndex, true);
                    if (mustBeClean)
                    {
                        ControlHelper.DisableControlEvents(lstBox, "SelectedIndexChanged");
                        lstBox.SetSelected(selectedIndex, false);
                        ControlHelper.EnableControlEvents(lstBox, "SelectedIndexChanged");
                    }
                }
                else
                {
                    if (lstBox.Items.Count > 0)
                        lstBox.SelectedIndex = 0;
                    lstBox.SelectedIndex = -1;
                }


                lstBox.SelectedIndex = currSelectedIndex;
            }
            else
            {
                if ((selectedIndex < -1) || (selectedIndex >= lstBox.Items.Count))
                    throw new Exception("Invalid property value");

                lstBox.SelectedIndex = selectedIndex;
            }

            return GetSelectedIndex(lstBox);
        }

        /// <summary>
        ///  Returns a value indicating whether the specified item is selected.
        /// </summary>
        /// <param name="lstBox">The listbox to test.</param>
        /// <param name="index">The index of the item to query if it is selected.</param>
        /// <returns>True if the item is selected.</returns>
        public static bool GetSelected(ListBox lstBox, int index)
        {
            return lstBox.GetSelected(index);
        }

        /// <summary>
        /// Selects or clears the selection for the specified item in a System.Windows.Forms.ListBox.
        /// </summary>
        /// <param name="lstBox">The listbox parent.</param>
        /// <param name="index">The index of the item.</param>
        /// <param name="value">The value to set to selected property.</param>
        public static void SetSelected(ListBox lstBox, int index, bool value)
        {
            if ((index < -1) || (index >= lstBox.Items.Count))
                throw new Exception("Invalid property value");

            if (lstBox.GetSelected(index) != value)
            {
                if (value && ((lstBox.SelectionMode == SelectionMode.MultiSimple) || (lstBox.SelectionMode == SelectionMode.MultiExtended)))
                {
                    if (_SelectedIndexList.ContainsKey(lstBox))
                        _SelectedIndexList[lstBox] = index;
                    else
                        throw new Exception("SelectedIndex property not stored for a MultiSelect ListBox, "
                            + "please add a ListBoxHelper to the form and set the property SelectionMode again");
                }

                lstBox.SetSelected(index, value);
            }
        }
    }
}
