using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace UpgradeHelpers.Gui
{
    /// <summary>
    /// Extender that adds support to special functionality of the menus in .NET.
    /// </summary>
    [ProvideProperty("CheckDropDownBehavior", typeof(System.Windows.Forms.MenuStrip))]
    public partial class MenuHelper : Component, IExtenderProvider
    {

        /// <summary>
        /// List of menus that has a CheckDropDown behaviour.
        /// </summary>
        private static List<System.Windows.Forms.MenuStrip> _checkDropDownBehavior = new List<MenuStrip>();

        /// <summary>
        /// Constructor.
        /// </summary>
        public MenuHelper()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="container">The container where to add the controls.</param>
        public MenuHelper(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        /// <summary>
        /// Determinates which controls can use these extra properties.
        /// </summary>
        /// <param name="extender">The object to test.</param>
        /// <returns>True if the object can extend the properties.</returns>
        public bool CanExtend(object extender)
        {
            return extender is System.Windows.Forms.MenuStrip;
        }

        /// <summary>
        /// Gets the current value for the property CheckDropDownBehavior for a Main Menu.
        /// </summary>
        /// <param name="mainMenu">The MainMenu to consult for its property value.</param>
        /// <returns>The current stored value or false.</returns>
        public bool GetCheckDropDownBehavior(System.Windows.Forms.MenuStrip mainMenu)
        {
            return _checkDropDownBehavior.Contains(mainMenu);
        }

        /// <summary>
        /// Sets the value for the property CheckDropDownBehavior for a Main Menu.
        /// </summary>
        /// <param name="mainMenu">The MainMenu to set the value.</param>
        /// <param name="value">The new value.</param>
        public void SetCheckDropDownBehavior(System.Windows.Forms.MenuStrip mainMenu, bool value)
        {
            ToolStripMenuItem menuItem;
            if (value && !_checkDropDownBehavior.Contains(mainMenu))
            {
                foreach (ToolStripItem item in mainMenu.Items)
                {
                    menuItem = item as ToolStripMenuItem;
                    if (menuItem != null)
                        AddCheckDropDownBehavior(menuItem);
                }

                mainMenu.ItemAdded += new ToolStripItemEventHandler(ToolStripItem_ItemAdded);
                mainMenu.ItemRemoved += new ToolStripItemEventHandler(ToolStripItem_ItemRemoved);
                _checkDropDownBehavior.Add(mainMenu);
            }
            else if (!value && _checkDropDownBehavior.Contains(mainMenu))
            {
                foreach (ToolStripItem item in mainMenu.Items)
                {
                    menuItem = item as ToolStripMenuItem;
                    if (menuItem != null)
                        RemoveCheckDropDownBehavior(menuItem);
                }

                mainMenu.ItemAdded -= new ToolStripItemEventHandler(ToolStripItem_ItemAdded);
                mainMenu.ItemRemoved -= new ToolStripItemEventHandler(ToolStripItem_ItemRemoved);
                _checkDropDownBehavior.Remove(mainMenu);
            }
        }

        /// <summary>
        /// This event handler is included just in case that a new menuItem is removed in runtime, 
        /// so it should be included in the RemoveCheckDropDownBehavior.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The ToolStripItem event arguments.</param>
        private void ToolStripItem_ItemRemoved(object sender, ToolStripItemEventArgs e)
        {
            ToolStripMenuItem menuItem = e.Item as ToolStripMenuItem;
            if (menuItem != null)
                RemoveCheckDropDownBehavior(menuItem);
        }

        /// <summary>
        /// This event handler is included just in case that a new menuItem is added in runtime, 
        /// so it should be included in the AddCheckDropDownBehavior.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The ToolStripItem event arguments.</param>
        private void ToolStripItem_ItemAdded(object sender, ToolStripItemEventArgs e)
        {
            ToolStripMenuItem menuItem = e.Item as ToolStripMenuItem;
            if (menuItem != null)
                AddCheckDropDownBehavior(menuItem);
        }

        /// <summary>
        /// Removes the event handler used to check for the correct dropdown behavior.
        /// </summary>
        /// <param name="menuItem">The menuItem to remove the event handlers.</param>
        private void RemoveCheckDropDownBehavior(ToolStripMenuItem menuItem)
        {
            ToolStripMenuItem childMenuItem;

            menuItem.DropDown.ItemAdded -= new ToolStripItemEventHandler(ToolStripItem_ItemAdded);
            menuItem.DropDown.ItemRemoved -= new ToolStripItemEventHandler(ToolStripItem_ItemRemoved);
            menuItem.DropDown.Opening -= new CancelEventHandler(ToolStripDropDownItem_Opening);
            foreach (ToolStripItem item in menuItem.DropDownItems)
            {
                childMenuItem = item as ToolStripMenuItem;

                if (childMenuItem != null)
                    RemoveCheckDropDownBehavior(childMenuItem);
            }
        }

        /// <summary>
        /// Adds the proper event handler to check for the correct dropdown behavior.
        /// </summary>
        /// <param name="menuItem">The menuItem to set the event handlers.</param>
        private void AddCheckDropDownBehavior(ToolStripMenuItem menuItem)
        {
            ToolStripMenuItem childMenuItem;

            menuItem.DropDown.ItemAdded -= new ToolStripItemEventHandler(ToolStripItem_ItemAdded);
            menuItem.DropDown.ItemAdded += new ToolStripItemEventHandler(ToolStripItem_ItemAdded);
            menuItem.DropDown.ItemRemoved -= new ToolStripItemEventHandler(ToolStripItem_ItemRemoved);
            menuItem.DropDown.ItemRemoved += new ToolStripItemEventHandler(ToolStripItem_ItemRemoved);
            menuItem.DropDown.Opening -= new CancelEventHandler(ToolStripDropDownItem_Opening);
            menuItem.DropDown.Opening += new CancelEventHandler(ToolStripDropDownItem_Opening);

            foreach (ToolStripItem item in menuItem.DropDownItems)
            {
                childMenuItem = item as ToolStripMenuItem;

                if (childMenuItem != null)
                    AddCheckDropDownBehavior(childMenuItem);
            }
        }

        /// <summary>
        /// Event handler included to manage propertly the case where any parent is disabled so 
        /// it shouldn't be displayed.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The Cancel event arguments.</param>
        private void ToolStripDropDownItem_Opening(object sender, CancelEventArgs e)
        {
            if (DesignMode)
                return;

            ToolStripDropDownMenu source = sender as ToolStripDropDownMenu;
            e.Cancel = !source.OwnerItem.Enabled || HasParentMenuDisabled(source.OwnerItem);
        }

        /// <summary>
        /// Finds out if the parent menu is disabled.
        /// </summary>
        /// <param name="menu">The child menu</param>
        /// <param name="recursive">A flag to look into the parent's parent and so on.</param>
        /// <returns>True if the menu parent is disabled, if the flag recursive is set then it will
        /// return true if any parent menu is disabled, otherwise it returns false.
        /// </returns>
        public static bool HasParentMenuDisabled(ToolStripItem menu, bool recursive)
        {
            bool res = false;

            if (menu.OwnerItem != null)
            {
                res = !menu.OwnerItem.Enabled;

                if ((!res) && recursive)
                    res = HasParentMenuDisabled(menu.OwnerItem, recursive);
            }

            return res;
        }

        /// <summary>
        /// Finds out if the parent menu is disabled.
        /// </summary>
        /// <param name="menu">The child menu.</param>
        /// <returns>True if any parent menu is disabled, otherwise it returns false.
        /// </returns>
        public static bool HasParentMenuDisabled(ToolStripItem menu)
        {
            return HasParentMenuDisabled(menu, true);
        }
    }
}
