using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;

namespace UpgradeHelpers.Gui
{
    /// <summary>
    /// Extender that adds support to special functionality in ComboBoxes and ListBoxes, 
    /// mainly related to ItemData.
    /// </summary>
    [ProvideProperty("ItemData", typeof(System.Windows.Forms.ListControl))]
    public partial class ListControlHelper : Component, IExtenderProvider, ISupportInitialize
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ListControlHelper()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="container">The container where to add the controls.</param>
        public ListControlHelper(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        /// <summary>
        /// Indicates if EndInit hasn't been executed yet after a BeginInit.
        /// </summary>
        private bool _onInitialization;

        /// <summary>
        /// Implements BeginInit Method from ISupportInitialize. 
        /// Sets ListControl status to OnInitialization.
        /// </summary>
        public void BeginInit()
        {
            _onInitialization = true;
        }

        /// <summary>
        /// Implements EndInit Method from ISupportInitialize. 
        /// Sets ListControl status to Not OnInitialization.
        /// </summary>
        public void EndInit()
        {
            _onInitialization = false;
            RefreshItemsData();
        }

        /// <summary>
        /// Updates the list of items data of the controls in runtime after the EndInit has been invoked.
        /// </summary>
        private void RefreshItemsData()
        {
            if (!DesignMode)
            {
                foreach (System.Windows.Forms.ListControl lstControl in _itemsData.Keys)
                {
                    ComboBox box = lstControl as ComboBox;
                    if (box != null)
                    {
                        // ReSharper disable EmptyForStatement
                        for (int i = 0; (i < box.Items.Count) && (i < _itemsData[lstControl].Length); i++)
                        {
                            //((System.Windows.Forms.ComboBox)lstControl).Items[i] = ItemsData[lstControl][i];
                        }
                        // ReSharper restore EmptyForStatement
                    }
                    else
                    {
                        // ReSharper disable EmptyForStatement
                        for (int i = 0; (i < ((ListBox)lstControl).Items.Count) && (i < _itemsData[lstControl].Length); i++)
                        {
                            //((System.Windows.Forms.ListBox)lstControl).Items[i] = ItemsData[lstControl][i];
                        }
                        // ReSharper restore EmptyForStatement
                    }
                }
            }
        }

        /// <summary>
        /// Stores the ItemsData for each control temporarely during design time.
        /// </summary>
        private readonly Dictionary<ListControl, int[]> _itemsData = new Dictionary<ListControl, int[]>();

        /// <summary>
        /// Determinates which controls can use these extra properties.
        /// </summary>
        /// <param name="extender">The object to test.</param>
        /// <returns>True if the object can extend the properties.</returns>
        public bool CanExtend(object extender)
        {
            return extender is ListControl;
        }

        /// <summary>
        /// Gets the ItemData property of a specific list control.
        /// </summary>
        /// <param name="lstControl">The list control to test.</param>
        /// <returns>Returns an int array with the item data list of the control.</returns>
        public int[] GetItemData(ListControl lstControl)
        {
            int[] res;

            ComboBox box = lstControl as ComboBox;
            res = box != null ? GetItemData(box) : GetItemData((ListBox)lstControl);

            return res;
        }

        /// <summary>
        /// Gets the ItemData property of a specific list control. 
        /// This specific function applies just for a ComboBox control.
        /// </summary>
        /// <param name="lstControl">The list control to test.</param>
        /// <returns>Returns an int array with the item data list of the control.</returns>
        private int[] GetItemData(ComboBox lstControl)
        {
            int[] res = new int[lstControl.Items.Count];

            //In design time we will keep the list of itemsData in a separate list so 
            //we don't mess with the VS.NET Designer to display the Items property
            if (!_itemsData.ContainsKey(lstControl))
            {
                if (DesignMode)
                {
                    _itemsData.Add(lstControl, res);
                }
            }
            else
            {
                if (lstControl.Items.Count != _itemsData[lstControl].Length)
                {
                    for (int i = 0; (i < lstControl.Items.Count) && (i < _itemsData[lstControl].Length); i++)
                        res[i] = _itemsData[lstControl][i];

                    _itemsData[lstControl] = res;
                }
                else
                    res = _itemsData[lstControl];
            }

            return res;
        }

        /// <summary>
        /// Gets the ItemData property of a specific list control. 
        /// This specific function applies just for a ListBox control.
        /// </summary>
        /// <param name="lstControl">The list control to test.</param>
        /// <returns>Returns an int array with the item data list of the control.</returns>
        private int[] GetItemData(ListBox lstControl)
        {
            int[] res = new int[lstControl.Items.Count];

            //In design time we will keep the list of itemsData in a separate list so 
            //we don't mess with the VS.NET Designer to display the Items property
            if (!_itemsData.ContainsKey(lstControl))
            {
                if (DesignMode)
                {
                    _itemsData.Add(lstControl, res);
                }
            }
            else
            {
                if (lstControl.Items.Count != _itemsData[lstControl].Length)
                {
                    for (int i = 0; (i < lstControl.Items.Count) && (i < _itemsData[lstControl].Length); i++)
                        res[i] = _itemsData[lstControl][i];

                    _itemsData[lstControl] = res;
                }
                else
                    res = _itemsData[lstControl];
            }

            return res;
        }

        /// <summary>
        /// Sets the ItemData property of a specific list control.
        /// </summary>
        /// <param name="lstControl">The list control.</param>
        /// <param name="itemsData">The Item data list to set.</param>
        public void SetItemData(ListControl lstControl, int[] itemsData)
        {
            ComboBox box = lstControl as ComboBox;
            if (box != null)
                SetItemData(box, itemsData);
            else
                SetItemData((ListBox)lstControl, itemsData);
        }

        /// <summary>
        /// Sets ItemData property of a specific control
        /// </summary>
        /// <param name="lstControl">Listbox or combobox control to set</param>
        /// <param name="index">index inside the list to set</param>
        /// <param name="value">new value</param>
        public void SetItemData(ListControl lstControl, int index, object value)
        {
            if (_itemsData.ContainsKey(lstControl))
            {
                if (_itemsData[lstControl].Length > index && index >= 0)
                {
                    _itemsData[lstControl].SetValue(value, index);
                }
            }
        }

        /// <summary>
        /// Sets the ItemData property of a specific list control.
        /// This specific function applies just for a ComboBox control.
        /// </summary>
        /// <param name="lstControl">The list control.</param>
        /// <param name="itemsData">The Item data list to set.</param>
        private void SetItemData(ComboBox lstControl, int[] itemsData)
        {
            int[] items = new int[lstControl.Items.Count];
            if (itemsData != null)
            {
                if (DesignMode || _onInitialization)
                {
                    if (!_onInitialization)
                    {
                        for (int i = 0; (i < lstControl.Items.Count) && (i < itemsData.Length); i++)
                            items[i] = itemsData[i];
                    }
                    else
                        items = itemsData;

                    if (!_itemsData.ContainsKey(lstControl))
                        _itemsData.Add(lstControl, items);
                    else
                        _itemsData[lstControl] = items;
                }
                else
                {
                    for (int i = 0; (i < lstControl.Items.Count) && (i < itemsData.Length); i++)
                    {
                        //Microsoft.VisualBasic.Compatibility.VB6.Support.SetItemData(lstControl, i, itemsData[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Sets the ItemData property of a specific list control.
        /// This specific function applies just for a ListBox control.
        /// </summary>
        /// <param name="lstControl">The list control.</param>
        /// <param name="itemsData">The Item data list to set.</param>
        private void SetItemData(ListBox lstControl, int[] itemsData)
        {
            int[] items = new int[lstControl.Items.Count];
            if (itemsData != null)
            {
                if (DesignMode || _onInitialization)
                {
                    if (!_onInitialization)
                    {
                        for (int i = 0; (i < lstControl.Items.Count) && (i < itemsData.Length); i++)
                            items[i] = itemsData[i];
                    }
                    else
                        items = itemsData;

                    if (!_itemsData.ContainsKey(lstControl))
                        _itemsData.Add(lstControl, items);
                    else
                        _itemsData[lstControl] = items;
                }
                else
                {

                    //for (int i = 0; (i < lstControl.Items.Count) && (i < itemsData.Length); i++)
                    //    Microsoft.VisualBasic.Compatibility.VB6.Support.SetItemData(lstControl, i, itemsData[i]);
                    Dictionary<int, int> lstItemData = new Dictionary<int, int>();
                    for (int i = 0; (i < lstControl.Items.Count) && (i < itemsData.Length); i++)
                    {
                        lstItemData.Add(i, itemsData[i]);
                    }
                    lstControl.Tag = lstItemData;
                }
            }
        }
    }


    /// <summary>
    /// Static class that contains a List control extender methods
    /// </summary>
    public static class ListControl_Extenders
    {
        private class Item
        {
            public int itemdata;
            public string item;

            public Item()
            {
                itemdata = 0;
                item = "";
            }

            public override string ToString()
            {
                return item;
            }
        }

        /// <summary>
        /// Gets the list item.
        /// </summary>
        /// <param name="lstControl">The List control instance.</param>
        /// <param name="index">The index.</param>
        /// <returns>The list item.</returns>
        public static string GetListItem(this ListControl lstControl, int index)
        {
            ComboBox box = lstControl as ComboBox;
            if (box != null)
                return GetListItem(box, index);
            return GetListItem((ListBox)lstControl, index);
        }

        private static string GetListItem(ComboBox lstControl, int index)
        {
            if (index >= 0 && lstControl.Items.Count > index)
            {
                string item;
                Item item1 = lstControl.Items[index] as Item;
                if (item1 != null)
                {
                    item = item1.item;
                }
                else
                {
                    item = lstControl.Items[index] as string;
                }
                return item;
            }
            return "";
        }

        private static string GetListItem(ListBox lstControl, int index)
        {
            if (index >= 0 && lstControl.Items.Count > index)
            {
                string item;
                Item item1 = lstControl.Items[index] as Item;
                if (item1 != null)
                {
                    item = item1.item;
                }
                else
                {
                    item = lstControl.Items[index] as string;
                }
                return item;
            }
            return "";
        }

        /// <summary>
        /// Sets the list item.
        /// </summary>
        /// <param name="lstControl">The list control instance.</param>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        public static void SetListItem(this ListControl lstControl, int index, string value)
        {
            ComboBox box = lstControl as ComboBox;
            if (box != null)
                SetListItem(box, index, value);
            else
                SetListItem((ListBox)lstControl, index, value);
        }

        private static void SetListItem(ComboBox lstControl, int index, string value)
        {
            if (lstControl.Items.Count >= index)
            {
                Item item = new Item();
                item.item = value;
                if (lstControl.Items.Count == index)
                {
                    lstControl.Items.Add(item);
                }
                else
                {
                    lstControl.Items[index] = item;
                }
            }
            else
            {
                Microsoft.VisualBasic.Information.Err().Number = 381;
            }
        }
        private static void SetListItem(ListBox lstControl, int index, string value)
        {
            if (lstControl.Items.Count >= index)
            {
                Item item = new Item();
                item.item = value;
                if (lstControl.Items.Count == index)
                {
                    lstControl.Items.Add(item);
                }
                else
                {
                    Item item1 = lstControl.Items[index] as Item;
                    if (item1 != null)
                    {
                        item.itemdata = item1.itemdata;
                    }
                    lstControl.Items[index] = item;
                }
            }
            else
            {
                Microsoft.VisualBasic.Information.Err().Number = 381;
            }
        }

        /// <summary>
        /// Gets the item data.
        /// </summary>
        /// <param name="lstControl">The list control instance.</param>
        /// <param name="index">The index.</param>
        /// <returns>The item data.</returns>
        public static int GetItemData(this ListControl lstControl, int index)
        {
            ComboBox box = lstControl as ComboBox;
            if (box != null)
                return GetItemData(box, index);
            return GetItemData((ListBox)lstControl, index);
        }

        private static int GetItemData(this ListBox lstControl, int index)
        {
            if (lstControl.Items.Count > 0 && lstControl.Items.Count >= index && lstControl.Items[index] is Item)
            {
                return ((Item)lstControl.Items[index]).itemdata;
            }
            return 0;
        }

        private static int GetItemData(this ComboBox lstControl, int index)
        {
            if (lstControl.Items.Count > 0 && lstControl.Items.Count >= index && lstControl.Items[index] is Item)
            {
                return ((Item)lstControl.Items[index]).itemdata;
            }
            return 0;
        }
        /// <summary>
        /// Sets the item data.
        /// </summary>
        /// <param name="lstControl">The list control instance.</param>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        public static void SetItemData(this ListControl lstControl, int index, int value)
        {
            ListBox box = lstControl as ListBox;
            if (box != null)
            {
                SetItemData(box, index, value);
            }
            else
            {
                ComboBox control = lstControl as ComboBox;
                if (control != null)
                {
                    SetItemData(control, index, value);
                }
            }
        }

        private static void SetItemData(this ListBox lstControl, int index, int value)
        {
            if (lstControl.Items.Count > 0 && lstControl.Items.Count >= index)
            {
                Item item;
                Item item1 = lstControl.Items[index] as Item;
                if (item1 != null)
                {
                    item = item1;
                    item.itemdata = value;
                }
                else
                {
                    item = new Item();
                    item.item = lstControl.Items[index].ToString();
                    item.itemdata = value;
                }
                lstControl.Items[index] = item;
            }
            else
            {
                Microsoft.VisualBasic.Information.Err().Number = 381;
            }
        }
        private static void SetItemData(this ComboBox lstControl, int index, int value)
        {
            if (lstControl.Items.Count > 0 && lstControl.Items.Count >= index)
            {
                Item item;
                Item item1 = lstControl.Items[index] as Item;
                if (item1 != null)
                {
                    item = item1;
                    item.itemdata = value;
                }
                else
                {
                    item = new Item();
                    item.item = lstControl.Items[index].ToString();
                    item.itemdata = value;
                }
                lstControl.Items[index] = item;
            }
            else
            {
                Microsoft.VisualBasic.Information.Err().Number = 381;
            }
        }

        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="lstControl">The list control instance.</param>
        public static void Clear(this ListControl lstControl)
        {
            ListBox box = lstControl as ListBox;
            if (box != null)
            {
                box.Items.Clear();
            }
            else
            {
                ComboBox control = lstControl as ComboBox;
                if (control != null)
                {
                    control.Items.Clear();
                }
            }
            _DictionaryOfNewIndexes[lstControl.GetHashCode()] = -1;
        }

        /// <summary>
        /// Adds the item
        /// </summary>
        /// <param name="lstControl">The list control instance.</param>
        /// <param name="value">The value.</param>
        /// <param name="index">The index.</param>
        public static void AddItem(this ListControl lstControl, string value, int index)
        {
            ListBox box = lstControl as ListBox;
            if (box != null)
            {
                AddItem(box, value, index);
            }
            else
            {
                ComboBox control = lstControl as ComboBox;
                if (control != null)
                {
                    AddItem(control, value, index);
                }
            }
        }

        private static void AddItem(this ListBox lstControl, string value, int index)
        {
            try
            {
                lstControl.Items.Insert(index, value);
                _DictionaryOfNewIndexes[lstControl.GetHashCode()] = index;
            }
            catch (Exception)
            {
            }
        }

        private static void AddItem(this ComboBox lstControl, string value, int index)
        {
            try
            {
                lstControl.Items.Insert(index, value);
                _DictionaryOfNewIndexes[lstControl.GetHashCode()] = index;
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="lstControl">The list control instance.</param>
        /// <param name="value">The value.</param>
        public static void AddItem(this ListControl lstControl, string value)
        {
            ListBox box = lstControl as ListBox;
            if (box != null)
            {
                AddItem(box, value);
            }
            else
            {
                ComboBox control = lstControl as ComboBox;
                if (control != null)
                {
                    AddItem(control, value);
                }
            }
        }

        private static void AddItem(this ListBox lstControl, string value)
        {
            int newIndex = lstControl.Items.Add(value);
            _DictionaryOfNewIndexes[lstControl.GetHashCode()] = newIndex;
        }

        private static void AddItem(this ComboBox lstControl, string value)
        {
            int newIndex = lstControl.Items.Add(value);
            _DictionaryOfNewIndexes[lstControl.GetHashCode()] = newIndex;
        }

        static readonly Dictionary<int, int> _DictionaryOfNewIndexes = new Dictionary<int, int>();

        /// <summary>
        /// Gets the new index.
        /// </summary>
        /// <param name="lstControl">The list control instance.</param>
        /// <returns>The new index.</returns>
        public static int GetNewIndex(this ListControl lstControl)
        {
            if (_DictionaryOfNewIndexes.ContainsKey(lstControl.GetHashCode()))
            {
                return _DictionaryOfNewIndexes[lstControl.GetHashCode()];
            }
            return -1;
        }

        /// <summary>
        /// Removes the item.
        /// </summary>
        /// <param name="lstControl">The list control instance.</param>
        /// <param name="index">The index.</param>
        public static void RemoveItem(this ListControl lstControl, int index)
        {
            ListBox box = lstControl as ListBox;
            if (box != null)
            {
                RemoveItem(box, index);
            }
            else
            {
                ComboBox control = lstControl as ComboBox;
                if (control != null)
                {
                    RemoveItem(control, index);
                }
            }
        }

        private static void RemoveItem(this ListBox lstControl, int index)
        {
            try
            {
                lstControl.Items.RemoveAt(index);
                _DictionaryOfNewIndexes[lstControl.GetHashCode()] = -1;
            }
            catch (Exception)
            {
            }
        }

        private static void RemoveItem(this ComboBox lstControl, int index)
        {
            try
            {
                if (lstControl.Text == lstControl.Items[index].ToString())
                {
                    lstControl.Text = string.Empty;
                }
                lstControl.Items.RemoveAt(index);
                _DictionaryOfNewIndexes[lstControl.GetHashCode()] = -1;
            }
            catch
            {
            }
        }
    }
}
