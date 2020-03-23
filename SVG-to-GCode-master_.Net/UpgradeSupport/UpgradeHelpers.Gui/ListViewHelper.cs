using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;
using System.Drawing;
using UpgradeHelpers.Helpers;

namespace UpgradeHelpers.Gui
{
    /// <summary>
    /// Extender that adds support to special functionality in ListView controls.
    /// </summary>
    [ProvideProperty("Sorted", typeof(ListView))]
    [ProvideProperty("SortKey", typeof(ListView))]
    [ProvideProperty("SortOrder", typeof(ListView))]
    [ProvideProperty("CorrectEventsBehavior", typeof(ListView))]
    [ProvideProperty("ItemClickMethod", typeof(ListView))]
    [ProvideProperty("LargeIcons", typeof(ListView))]
    [ProvideProperty("SmallIcons", typeof(ListView))]
    [ProvideProperty("ColumnHeaderIcons", typeof(ListView))]
    public partial class ListViewHelper : Component, IExtenderProvider, ISupportInitialize
    {

        /// <summary>
        /// Indicates if EndInit hasn't been executed yet after a BeginInit.
        /// </summary>
        private bool _onInitialization;
        /// <summary>
        /// Delegate for ItemClick event.
        /// </summary>
        /// <param name="item">The ListViewItem</param>
        private delegate void ListViewItemClickDelegate(ListViewItem item);
        /// <summary>
        /// Events to be locked during several processes.
        /// </summary>
        private static readonly object ObjLockEvents = new object();
        /// <summary>
        /// List of events to be corrected for this provider.
        /// </summary>
        private static readonly Dictionary<string, Delegate> EventsToCorrect = new Dictionary<string, Delegate>();
        /// <summary>
        /// List of events to be patched for this provider.
        /// </summary>
        private static readonly WeakDictionary<ListView, Dictionary<String, List<Delegate>>> EventsPatched = new WeakDictionary<ListView, Dictionary<string, List<Delegate>>>();
        /// <summary>
        /// List of properties and values that are supplied by this Helper.
        /// </summary>
        private static readonly WeakDictionary<ListView, Dictionary<NewPropertiesEnum, object>> NewProperties = new WeakDictionary<ListView, Dictionary<NewPropertiesEnum, object>>();
        /// <summary>
        /// Keeps a list of Icons set for different properties.
        /// </summary>
        private static readonly List<ListView> PendingListIconsToProcess = new List<ListView>();

        private const string ItemClickEventName = "ItemClick";
        private const string DrawItemEventName = "DrawItem";
        private const string DrawSubItemEventName = "DrawSubItem";
        private const string DrawColumnHeaderEventName = "DrawColumnHeader";
        private const string KeyUpEventName = "KeyUp";
        private const string ClickEventName = "Click";
        private const string MouseUpEventName = "MouseUp";
        private const string DoubleClickEventName = "DoubleClick";

        /// <summary>
        /// Constructor.
        /// </summary>
        static ListViewHelper()
        {
            //Initializes the list of events that should be patched
            EventsToCorrect.Add(ClickEventName, new EventHandler(ListView_Click));
            EventsToCorrect.Add(MouseUpEventName, new MouseEventHandler(ListView_MouseUp));
            EventsToCorrect.Add(DoubleClickEventName, new EventHandler(ListView_DoubleClick));
            EventsToCorrect.Add(KeyUpEventName, new KeyEventHandler(ListView_KeyUp));

            Application.AddMessageFilter(new IMessageFilterImplementer());
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ListViewHelper()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="container">The container where to add the controls.</param>
        public ListViewHelper(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        /// <summary>
        /// Enum to handle the different properties and custom behaviors supplied by this Helper.
        /// </summary>
        private enum NewPropertiesEnum
        {
            CustomSortingClass = 0,
            SortedProperty = 1,
            SortKeyProperty = 2,
            SortOrderProperty = 3,
            CorrectEventsBehavior = 4,
            ItemClickMethod = 5,
            LargeIcons = 6,
            SmallIcons = 7,
            ColumnHeaderIcons = 8,
            InternalColumnHeaderImageListHelper = 9,
            ListItemIcon = 10,
            ListItemSmallIcon = 11
        }


        /// <summary>
        /// Determinates which controls can use these extra properties.
        /// </summary>
        /// <param name="extender">The object to test.</param>
        /// <returns>True if the object can extend the properties.</returns>
        public bool CanExtend(object extender)
        {
            return extender is ListView;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////// INSTANCE PROPERTIES DEFINITION //////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets the Sorted property for a ListView.
        /// </summary>
        /// <param name="listView">The ListView to be test.</param>
        /// <returns>True elements are ordered, otherwise false.</returns>
        [Description("Indicates whether the elements of a control are automatically sorted alphabetically"), Category("Custom Properties")]
        public bool GetSorted(ListView listView)
        {
            return GetSortedProperty(listView);
        }
        /// <summary>
        /// Sets the Sorted property for a ListView.
        /// </summary>
        /// <param name="listView">The ListView to be set.</param>
        /// <param name="value">Indicates if values in ListView must be ordered or not.</param>
        public void SetSorted(ListView listView, bool value)
        {
            SetSortedProperty(listView, value);
        }

        /// <summary>
        /// Gets SortKey property for a ListView.
        /// </summary>
        /// <param name="listView">The ListView to be test.</param>
        /// <returns>The current SortKey value.</returns>
        [Description("Returns/sets the current sort key"), Category("Custom Properties")]
        public int GetSortKey(ListView listView)
        {
            return GetSortKeyProperty(listView);
        }
        /// <summary>
        /// Sets SortKey property for a ListView.
        /// </summary>
        /// <param name="listView">The ListView to be set.</param>
        /// <param name="value">The new sortkey value.</param>
        public void SetSortKey(ListView listView, int value)
        {
            SetSortKeyProperty(listView, value, DesignMode);
        }

        /// <summary>
        /// Gets SortOrder property for a ListView.
        /// </summary>
        /// <param name="listView">The ListView to be test.</param>
        /// <returns>Indicates if values of ListView are ordered ascending or descending.</returns>
        [Description("Returns/sets whether or not the ListItems will be sorted in ascending or descending order."), Category("Custom Properties")]
        public SortOrder GetSortOrder(ListView listView)
        {
            return GetSortOrderProperty(listView);
        }
        /// <summary>
        /// Sets SortOrder property for a ListView.
        /// </summary>
        /// <param name="listView">The ListView to be set.</param>
        /// <param name="value">The new SortOrder value indicating the kind of ordering for the ListView.</param>
        public void SetSortOrder(ListView listView, SortOrder value)
        {
            SetSortOrderProperty(listView, value, DesignMode);
        }

        /// <summary>
        /// Gets CorrectEventsBehavior property for a ListView.
        /// </summary>
        /// <param name="listView">The ListView to be test.</param>
        /// <returns>If events must be corrected or not.</returns>
        [Description("Indicates if some events should be patched to be fired in the same way that used to be fired in VB6"), Category("Custom Properties")]
        public bool GetCorrectEventsBehavior(ListView listView)
        {
            if (CheckForProperty(listView, NewPropertiesEnum.CorrectEventsBehavior))
                return Convert.ToBoolean(NewProperties[listView][NewPropertiesEnum.CorrectEventsBehavior]);
            return false;
        }

        /// <summary>
        /// Sets CorrectEventsBehavior property for a ListView.
        /// </summary>
        /// <param name="listView">The ListView to be set.</param>
        /// <param name="value">The new value indicating if events must be corrected.</param>
        public void SetCorrectEventsBehavior(ListView listView, bool value)
        {
            if (CheckForProperty(listView, NewPropertiesEnum.CorrectEventsBehavior))
                NewProperties[listView][NewPropertiesEnum.CorrectEventsBehavior] = value;
        }

        /// <summary>
        /// Gets the name of the method to be invoked when the item click is fired, 
        /// this is a custom event that will be handled internally.
        /// </summary>
        /// <param name="listView">The listView to get the property.</param>
        /// <returns>The name of the method which will be invoked when the event should be fired,
        ///  it should receive a ListViewItem item as parameter.</returns>
        [Description("The name of the item click method, it should receive a ListViewItem item as parameter"), Category("Custom Event Handlers")]
        public string GetItemClickMethod(ListView listView)
        {
            if (CheckForProperty(listView, NewPropertiesEnum.ItemClickMethod))
                return (string)NewProperties[listView][NewPropertiesEnum.ItemClickMethod];
            return string.Empty;
        }

        /// <summary>
        /// Sets the name of the method to be invoked when the item click is fired, 
        /// this is a custom event that will be handled internally.
        /// </summary>
        /// <param name="listView">The listView to set the property.</param>
        /// <param name="value">The name of the method which will be invoked when 
        /// the event should be fired, it should receive a ListViewItem item as parameter.</param>
        public void SetItemClickMethod(ListView listView, string value)
        {
            if (CheckForProperty(listView, NewPropertiesEnum.ItemClickMethod))
                NewProperties[listView][NewPropertiesEnum.ItemClickMethod] = value;
        }

        /// <summary>
        /// Gets the name of the VB6 ListView in the form to use for the list of large icons.
        /// </summary>
        /// <param name="listView">The ListView where to find the name.</param>
        /// <returns>The name of the VB6 ListView.</returns>
        [Description("Returns/sets the name of the VB6 ListView control to use when displaying items as large icons. Note: The property LargeImageList will be affected in runtime"), Category("Custom Properties")]
        public string GetLargeIcons(ListView listView)
        {
            object value = GetLargeIconsProperty(listView);
            string s = value as string;
            if (s != null)
                return s;
            if (ImageListHelper.IsValid(value))
                return (string)ReflectionHelper.GetMember(value, "Name");
            throw new InvalidCastException("Invalid property value");
        }

        /// <summary>
        /// Sets the name of the VB6 ListView in the form to use for the list of large icons.
        /// </summary>
        /// <param name="listView">The ListView where to set the name.</param>
        /// <param name="value">The new name of the VB6 ListView.</param>
        public void SetLargeIcons(ListView listView, string value)
        {
            SetLargeIconsProperty(listView, value, _onInitialization, DesignMode);
        }

        /// <summary>
        /// Gets the name of the VB6 listview in the form to use for the list of small icons.
        /// </summary>
        /// <param name="listView">The ListView where to find the name.</param>
        /// <returns>The name of the VB6 ListView.</returns>
        [Description("Returns/sets the name of the VB6 ListView control to use when displaying items as small icons. Note: The property SmallImageList will be affected in runtime"), Category("Custom Properties")]
        public string GetSmallIcons(ListView listView)
        {
            object value = GetSmallIconsProperty(listView);
            string s = value as string;
            if (s != null)
                return s;
            if (ImageListHelper.IsValid(value))
                return (string)ReflectionHelper.GetMember(value, "Name");
            throw new InvalidCastException("Invalid property value");
        }

        /// <summary>
        /// Sets the name of the VB6 ListView in the form to use for the list of small icons.
        /// </summary>
        /// <param name="listView">The ListView where to set the name.</param>
        /// <param name="value">The new name of the VB6 ListView.</param>
        public void SetSmallIcons(ListView listView, string value)
        {
            SetSmallIconsProperty(listView, value, _onInitialization, DesignMode);
        }

        /// <summary>
        /// Gets the name of the VB6 ListView in the form to use for the column headers icons.
        /// </summary>
        /// <param name="listView">The ListView where to find the name.</param>
        /// <returns>The name of the VB6 ListView.</returns>
        [Description("Returns/sets the name of the VB6 ListView control used to store the images to show in the column headers"), Category("Custom Properties")]
        public string GetColumnHeaderIcons(ListView listView)
        {
            object value = GetColumnHeaderIconsProperty(listView);
            string s = value as string;
            if (s != null)
                return s;
            if (ImageListHelper.IsValid(value))
                return (string)ReflectionHelper.GetMember(value, "Name");
            throw new InvalidCastException("Invalid property value");
        }
        /// <summary>
        /// Sets the name of the VB6 ListView in the form to use for the column headers icons.
        /// </summary>
        /// <param name="listView">>The ListView where to set the name.</param>
        /// <param name="value">The name of the VB6 ListView.</param>
        public void SetColumnHeaderIcons(ListView listView, string value)
        {
            SetColumnHeaderIconsProperty(listView, value, _onInitialization, DesignMode);
        }


        //////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////// INSTANCE PROPERTIES DEFINITION //////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////


        //////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////// STATIC PROPERTIES DEFINITION ////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets static property for Sorted property.
        /// </summary>
        /// <param name="listView">The ListView to test.</param>
        /// <returns>The Sorted value in the ListView.</returns>
        public static bool GetSortedProperty(ListView listView)
        {
            if (CheckForProperty(listView, NewPropertiesEnum.SortedProperty))
            {
                bool res = Convert.ToBoolean(NewProperties[listView][NewPropertiesEnum.SortedProperty]);
                SyncSortedProperty(listView, res);

                return res;
            }
            return false;
        }

        /// <summary>
        /// Sets static property for Sorted property.
        /// </summary>
        /// <param name="listView">The ListView to set.</param>
        /// <param name="value">The new Sorted value.</param>
        public static void SetSortedProperty(ListView listView, bool value)
        {
            if (CheckForProperty(listView, NewPropertiesEnum.SortedProperty))
            {
                NewProperties[listView][NewPropertiesEnum.SortedProperty] = value;

                GetCustomListItemComparer(listView).Sorted = value;
                SyncSortedProperty(listView, value);
            }
        }

        /// <summary>
        /// Gets static property for SortKey property.
        /// </summary>
        /// <param name="listView">The ListView to test.</param>
        /// <returns>The SortKey value in the ListView.</returns>
        public static int GetSortKeyProperty(ListView listView)
        {
            if (CheckForProperty(listView, NewPropertiesEnum.SortKeyProperty))
                return Convert.ToInt32(NewProperties[listView][NewPropertiesEnum.SortKeyProperty]);
            return 0;
        }

        /// <summary>
        /// Sets static property for SortKey property.
        /// </summary>
        /// <param name="listView">The ListView to set.</param>
        /// <param name="value">The new SortKey value.</param>
        public static void SetSortKeyProperty(ListView listView, int value)
        {
            SetSortKeyProperty(listView, value, false);
        }
        /// <summary>
        /// Sets static property for SortKey property.
        /// </summary>
        /// <param name="listView">The ListView to set.</param>
        /// <param name="value">The new SortKey value.</param>
        /// <param name="onDesignMode">Indicates if design mode is currently active.</param>
        private static void SetSortKeyProperty(ListView listView, int value, bool onDesignMode)
        {
            if (CheckForProperty(listView, NewPropertiesEnum.SortKeyProperty))
            {
                if ((listView.Columns.Count > 0) && ((value < 0) || (value >= listView.Columns.Count)))
                    throw new InvalidOperationException("Invalid property value");

                NewProperties[listView][NewPropertiesEnum.SortKeyProperty] = value;
                GetCustomListItemComparer(listView).SortKey = value;
                if (!onDesignMode)
                    listView.Sort();
            }
        }

        /// <summary>
        /// Gets static property for SortOrder property.
        /// </summary>
        /// <param name="listView">The ListView to test.</param>
        /// <returns>The SortOrder value in the ListView.</returns>
        public static SortOrder GetSortOrderProperty(ListView listView)
        {
            if (CheckForProperty(listView, NewPropertiesEnum.SortOrderProperty))
                return (SortOrder)NewProperties[listView][NewPropertiesEnum.SortOrderProperty];
            return SortOrder.Ascending;
        }

        /// <summary>
        /// Sets static property for SortOrder property.
        /// </summary>
        /// <param name="listView">The ListView to set.</param>
        /// <param name="value">The new SortOrder value.</param>
        public static void SetSortOrderProperty(ListView listView, SortOrder value)
        {
            SetSortOrderProperty(listView, value, false);
        }
        /// <summary>
        /// Sets static property for SortOrder property.
        /// </summary>
        /// <param name="listView">The ListView to set.</param>
        /// <param name="value">The new SortOrder value.</param>
        /// <param name="onDesignMode">Indicates if design mode is currently active.</param>
        private static void SetSortOrderProperty(ListView listView, SortOrder value, bool onDesignMode)
        {
            if (CheckForProperty(listView, NewPropertiesEnum.SortOrderProperty))
            {

                if (value != SortOrder.None)
                {
                    NewProperties[listView][NewPropertiesEnum.SortOrderProperty] = value;
                    GetCustomListItemComparer(listView).SortOrder = value;
                    if (!onDesignMode)
                        listView.Sort();
                }
            }
        }

        /// <summary>
        /// Gets the name of the VB6 ListView in the form to use for the list of large icons.
        /// </summary>
        /// <param name="listView">The ListView to test.</param>
        /// <returns>The name of the VB6 ListView.</returns>
        public static object GetLargeIconsProperty(ListView listView)
        {
            if (CheckForProperty(listView, NewPropertiesEnum.LargeIcons))
                return NewProperties[listView][NewPropertiesEnum.LargeIcons];
            return string.Empty;
        }

        /// <summary>
        /// Sets the name of the VB6 ListView in the form to use for the list of large icons.
        /// </summary>
        /// <param name="listView">The ListView to set.</param>
        /// <param name="value">The new name for the VB6 ListView.</param>
        /// <param name="onDesignMode">Is Design Mode</param>
        public static void SetLargeIconsProperty(ListView listView, object value, bool onDesignMode)
        {
            SetLargeIconsProperty(listView, value, false, onDesignMode);
        }
        /// <summary>
        /// Sets the name of the VB6 ListView in the form to use for the list of large icons.
        /// </summary>
        /// <param name="listView">The ListView to set.</param>
        /// <param name="value">The new name for the VB6 ListView.</param>
        /// <param name="delayProcessing">Delays the processing of the property to after the EndInit.</param>
        /// <param name="onDesignMode">Is Design Mode</param>
        private static void SetLargeIconsProperty(ListView listView, object value, bool delayProcessing, bool onDesignMode)
        {
            if ((value is string) || ImageListHelper.IsValid(value))
            {
                if (CheckForProperty(listView, NewPropertiesEnum.LargeIcons))
                {
                    NewProperties[listView][NewPropertiesEnum.LargeIcons] = value;
                    if (!delayProcessing)
                        ProcessLargeIconsProperty(listView, onDesignMode);
                    else
                    {
                        if (!PendingListIconsToProcess.Contains(listView))
                            PendingListIconsToProcess.Add(listView);
                    }
                }
            }
            else
                throw new InvalidCastException("Invalid property value");
        }

        /// <summary>
        /// Gets the name of the VB6 ListView in the form to use for the list of small icons.
        /// </summary>
        /// <param name="listView">The ListView to test.</param>
        /// <returns>The name of the VB6 ListView.</returns>
        public static object GetSmallIconsProperty(ListView listView)
        {
            if (CheckForProperty(listView, NewPropertiesEnum.SmallIcons))
                return NewProperties[listView][NewPropertiesEnum.SmallIcons];
            return string.Empty;
        }

        /// <summary>
        /// Sets the name of the VB6 ListView in the form to use for the list of small icons.
        /// </summary>
        /// <param name="listView">The ListView to set.</param>
        /// <param name="value">The new name for the VB6 ListView.</param>
        /// <param name="onDesignMode">Is Design Mode</param>
        public static void SetSmallIconsProperty(ListView listView, object value, bool onDesignMode)
        {
            SetSmallIconsProperty(listView, value, false, onDesignMode);
        }
        /// <summary>
        /// Sets the name of the VB6 ListView in the form to use for the list of small icons.
        /// </summary>
        /// <param name="listView">The ListView to set.</param>
        /// <param name="value">The new value for the property.</param>
        /// <param name="delayProcessing">Delays the processing of the property to after the EndInit</param>
        /// <param name="onDesignMode">Is Design Mode</param>
        private static void SetSmallIconsProperty(ListView listView, object value, bool delayProcessing, bool onDesignMode)
        {
            if ((value is string) || ImageListHelper.IsValid(value))
            {
                if (CheckForProperty(listView, NewPropertiesEnum.SmallIcons))
                {
                    NewProperties[listView][NewPropertiesEnum.SmallIcons] = value;
                    if (!delayProcessing)
                        ProcessSmallIconsProperty(listView, onDesignMode);
                    else
                    {
                        if (!PendingListIconsToProcess.Contains(listView))
                            PendingListIconsToProcess.Add(listView);
                    }
                }
            }
            else
                throw new InvalidCastException("Invalid property value");
        }

        /// <summary>
        /// Gets the name of the VB6 ListView in the form to use for the column headers.
        /// </summary>
        /// <param name="listView">The ListView to test.</param>
        /// <returns>The name of the VB6 ListView.</returns>
        public static object GetColumnHeaderIconsProperty(ListView listView)
        {
            if (CheckForProperty(listView, NewPropertiesEnum.ColumnHeaderIcons))
                return NewProperties[listView][NewPropertiesEnum.ColumnHeaderIcons];
            return string.Empty;
        }

        /// <summary>
        /// Sets the name of the VB6 ListView in the form to use for the column headers
        /// </summary>
        /// <param name="listView">The ListView to set</param>
        /// <param name="value">The name for VB6 ListView</param>
        /// <param name="onDesignMode">Is Design Mode</param>
        public static void SetColumnHeaderIconsProperty(ListView listView, object value, bool onDesignMode)
        {
            SetColumnHeaderIconsProperty(listView, value, false, onDesignMode);
        }
        /// <summary>
        /// Sets the name of the VB6 listview in the form to use for the column headers.
        /// </summary>
        /// <param name="listView">The ListView to set</param>
        /// <param name="value">The new value for the property.</param>
        /// <param name="delayProcessing">Delays the processing of the property to after the EndInit.</param>
        /// <param name="onDesignMode">Is Design Mode</param>
        private static void SetColumnHeaderIconsProperty(ListView listView, object value, bool delayProcessing, bool onDesignMode)
        {
            if ((value is string) || ImageListHelper.IsValid(value))
            {
                if (CheckForProperty(listView, NewPropertiesEnum.ColumnHeaderIcons))
                {
                    NewProperties[listView][NewPropertiesEnum.ColumnHeaderIcons] = value;
                    if (!delayProcessing)
                        ProcessColumnHeaderIconsProperty(listView, onDesignMode);
                    else
                    {
                        if (!PendingListIconsToProcess.Contains(listView))
                            PendingListIconsToProcess.Add(listView);
                    }
                }
            }
            else
                throw new InvalidCastException("Invalid property value");
        }

        /// <summary>
        /// Gets the Icon property of a ColumnHeader.
        /// </summary>
        /// <param name="columnHeader">The source ColumnHeader.</param>
        /// <returns>The key|index of the Icon for the ColumnHeader.</returns>
        public static object GetColumnHeaderItemIconProperty(ColumnHeader columnHeader)
        {
            if (!string.IsNullOrEmpty(columnHeader.ImageKey))
                return columnHeader.ImageKey;
            return columnHeader.ImageIndex;
        }

        /// <summary>
        /// Sets the Icon property of a ColumnHeader.
        /// </summary>
        /// <param name="columnHeader">The source ColumnHeader.</param>
        /// <param name="value">The new key|index of the Icon for the ColumnHeader.</param>
        public static void SetColumnHeaderItemIconProperty(ColumnHeader columnHeader, object value)
        {
            string s = value as string;
            if (s != null)
            {
                columnHeader.ImageIndex = -1;
                columnHeader.ImageKey = s;
            }

            if (value is Int32)
            {
                columnHeader.ImageKey = string.Empty;
                columnHeader.ImageIndex = (int)value;
            }
        }

        /// <summary>
        /// Gets the Icon property of a ListItem (Key|Index to use for LargeIcons).
        /// </summary>
        /// <param name="listViewItem">The source ListItem.</param>
        /// <returns>The Key|Index of the Icon to use when LargeIcons are shown.</returns>
        public static object GetListItemIconProperty(ListViewItem listViewItem)
        {
            ListView listView = listViewItem.ListView;

            if (CheckForProperty(listView, NewPropertiesEnum.ListItemIcon))
            {
                Dictionary<ListViewItem, object> listViewItemIconLists = (Dictionary<ListViewItem, object>)NewProperties[listView][NewPropertiesEnum.ListItemIcon];
                if (listViewItemIconLists.ContainsKey(listViewItem))
                {
                    return listViewItemIconLists[listViewItem];
                }
                if (!string.IsNullOrEmpty(listViewItem.ImageKey))
                    return listViewItem.ImageKey;
                return listViewItem.ImageIndex;
            }
            return string.Empty;
        }
        /// <summary>
        /// Sets the Icon property of a ListItem (Key|Index to use for LargeIcons).
        /// </summary>
        /// <param name="listViewItem">The source ListItem.</param>
        /// <param name="value">The new Key|Index of the Icon to use when LargeIcons are shown.</param>
        public static void SetListItemIconProperty(ListViewItem listViewItem, object value)
        {
            ListView listView = listViewItem.ListView;

            if ((value is string) || (value is Int32))
            {
                if (CheckForProperty(listView, NewPropertiesEnum.ListItemIcon))
                {
                    Dictionary<ListViewItem, object> listViewItemIconLists = (Dictionary<ListViewItem, object>)NewProperties[listView][NewPropertiesEnum.ListItemIcon];

                    if (listViewItemIconLists.ContainsKey(listViewItem))
                        listViewItemIconLists[listViewItem] = value;
                    else
                    {
                        string s = value as string;
                        if (s != null)
                        {
                            listViewItem.ImageIndex = -1;
                            listViewItem.ImageKey = s;
                        }

                        if (value is Int32)
                        {
                            listViewItem.ImageKey = string.Empty;
                            listViewItem.ImageIndex = (int)value;
                        }
                    }
                }
            }
            else
                throw new InvalidCastException("Invalid property value");
        }

        /// <summary>
        /// Gets the SmallIcon property of a ListItem (Key|Index to use for SmallIcons).
        /// </summary>
        /// <param name="listViewItem">The source ListItem.</param>
        /// <returns>The Key|Index of the SmallIcon to use when SmallIcons are shown.</returns>
        public static object GetListItemSmallIconProperty(ListViewItem listViewItem)
        {
            ListView listView = listViewItem.ListView;

            if (CheckForProperty(listView, NewPropertiesEnum.ListItemSmallIcon))
            {
                Dictionary<ListViewItem, object> listViewItemSmallIconLists = (Dictionary<ListViewItem, object>)NewProperties[listView][NewPropertiesEnum.ListItemSmallIcon];
                if (listViewItemSmallIconLists.ContainsKey(listViewItem))
                    return listViewItemSmallIconLists[listViewItem];
                return -1;
            }
            return -1;
        }
        /// <summary>
        /// Sets the SmallIcon property of a ListItem (Key|Index to use for SmallIcons).
        /// </summary>
        /// <param name="listViewItem">The source ListItem.</param>
        /// <param name="value">The new Key|Index of the SmallIcon to use when SmallIcons are shown.</param>
        public static void SetListItemSmallIconProperty(ListViewItem listViewItem, object value)
        {
            ListView listView = listViewItem.ListView;

            if ((value is string) || (value is Int32))
            {
                if (CheckForProperty(listView, NewPropertiesEnum.ListItemSmallIcon))
                {
                    Dictionary<ListViewItem, object> listViewItemSmallIconLists = (Dictionary<ListViewItem, object>)NewProperties[listView][NewPropertiesEnum.ListItemSmallIcon];

                    if (!listViewItemSmallIconLists.ContainsKey(listViewItem))
                        listViewItemSmallIconLists.Add(listViewItem, value);
                    else
                        listViewItemSmallIconLists[listViewItem] = value;

                    PatchDrawItemEvents(listView);
                }
                return;
            }

            throw new InvalidCastException("Invalid property value");
        }

        /// <summary>
        /// Sets the SmallIcon property of a ListItem, it works as SetListItemSmallIconProperty does 
        /// but the ListItem is returned so it can be used in the normal upgrade of the functions 
        /// ListView.Add and ListView.Insert.
        /// </summary>
        /// <param name="listViewItem">The ListItem source.</param>
        /// <param name="value">The key/index of the SmallIcon to use.</param>
        /// <returns>The resultant ListView item.</returns>
        public static ListViewItem AddListItemSmallIconProperty(ListViewItem listViewItem, object value)
        {
            SetListItemSmallIconProperty(listViewItem, value);
            return listViewItem;
        }

        /// <summary>
        /// In order to handle the property SmallIcons of a ListItem some Draw events must be handled.
        /// </summary>
        /// <param name="listView">The ListView source.</param>
        private static void PatchDrawItemEvents(ListView listView)
        {
            Delegate[] eventDelegates;
            bool patchDrawItem = true;
            bool patchDrawSubItem = true;
            bool patchDrawColumnHeader = true;
            EventInfo eventInfo;

            if (EventsPatched.ContainsKey(listView))
            {
                //The events were previously patched
                if (EventsPatched[listView].ContainsKey(DrawItemEventName))
                    patchDrawItem = false;

                if (EventsPatched[listView].ContainsKey(DrawColumnHeaderEventName))
                    patchDrawColumnHeader = false;

                if (EventsPatched[listView].ContainsKey(DrawSubItemEventName))
                    patchDrawSubItem = false;
            }
            else
                EventsPatched.Add(listView, new Dictionary<string, List<Delegate>>());

            if (patchDrawItem)
            {
                eventInfo = listView.GetType().GetEvent(DrawItemEventName);
                if (eventInfo == null)
                    throw new InvalidOperationException("Event info for event '" + DrawItemEventName + "' could not be found");

                EventsPatched[listView].Add(DrawItemEventName, new List<Delegate>());
                eventDelegates = ContainerHelper.GetEventSubscribers(listView, DrawItemEventName);
                if (eventDelegates != null)
                {
                    foreach (Delegate del in eventDelegates)
                    {
                        EventsPatched[listView][DrawItemEventName].Add(del);
                        eventInfo.RemoveEventHandler(listView, del);
                    }
                }
                listView.DrawItem += ListView_DrawItem;
            }

            if (patchDrawColumnHeader)
            {
                eventInfo = listView.GetType().GetEvent(DrawColumnHeaderEventName);
                if (eventInfo == null)
                    throw new InvalidOperationException("Event info for event '" + DrawColumnHeaderEventName + "' could not be found");

                EventsPatched[listView].Add(DrawColumnHeaderEventName, new List<Delegate>());
                eventDelegates = ContainerHelper.GetEventSubscribers(listView, DrawColumnHeaderEventName);
                if (eventDelegates != null)
                {
                    foreach (Delegate del in eventDelegates)
                    {
                        EventsPatched[listView][DrawColumnHeaderEventName].Add(del);
                        eventInfo.RemoveEventHandler(listView, del);
                    }
                }
                listView.DrawColumnHeader += ListView_DrawColumnHeader;
            }

            if (patchDrawSubItem)
            {
                eventInfo = listView.GetType().GetEvent(DrawSubItemEventName);
                if (eventInfo == null)
                    throw new InvalidOperationException("Event info for event '" + DrawSubItemEventName + "' could not be found");

                EventsPatched[listView].Add(DrawSubItemEventName, new List<Delegate>());
                eventDelegates = ContainerHelper.GetEventSubscribers(listView, DrawSubItemEventName);
                if (eventDelegates != null)
                {
                    foreach (Delegate del in eventDelegates)
                    {
                        EventsPatched[listView][DrawSubItemEventName].Add(del);
                        eventInfo.RemoveEventHandler(listView, del);
                    }
                }
                listView.DrawSubItem += ListView_DrawSubItem;
            }

            listView.OwnerDraw = true;
        }

        /// <summary>
        /// Returns a subItem from a ListView item.
        /// </summary>
        /// <param name="listViewItem">The parent item.</param>
        /// <param name="index">The index of the item that has to be returned.</param>
        /// <returns>The found ListViewSubItem.</returns>
        public static ListViewItem.ListViewSubItem GetListViewSubItem(ListViewItem listViewItem, int index)
        {
            return GetListViewSubItem(listViewItem, listViewItem.ListView, index);
        }

        /// <summary>
        /// Returns a subItem from a ListView item.
        /// </summary>
        /// <param name="listViewItem">The parent item</param>
        /// <param name="parentListView">The parent ListView that will contain the ListView item.</param>
        /// <param name="index">The index of the item that has to be returned.</param>
        /// <returns>The found ListViewSubItem.</returns>
        public static ListViewItem.ListViewSubItem GetListViewSubItem(ListViewItem listViewItem, ListView parentListView, int index)
        {
            if ((parentListView.Columns.Count <= index) || (index < 0))
                throw new InvalidOperationException("Invalid property value");

            if (listViewItem.SubItems.Count <= index)
            {
                //lItem.SubItems.AddRange(Utils.ArraysHelper.InitializeArray<string>(index - lItem.SubItems.Count + 1));
                string[] strings = new string[index - listViewItem.SubItems.Count + 2];
                for (int i = 0; i < strings.Length; i++)
                    strings[i] = String.Empty;
                listViewItem.SubItems.AddRange(strings);
            }

            return listViewItem.SubItems[index];
        }

        /// <summary>
        /// Returns the left property for a column.
        /// </summary>
        /// <param name="listView">The ListView containing the column.</param>
        /// <param name="column">The Column to test.</param>
        /// <returns>The left value of the column.</returns>
        public static int GetListViewColumnLeftProperty(ListView listView, ColumnHeader column)
        {
            int left = 0;
            for (int i = 0; i < column.Index; i++)
                left += listView.Columns[i].Width;

            return left;
        }

        /// <summary>
        /// Returns the left property for a column.
        /// </summary>
        /// <param name="columns">The ListView columns</param>
        /// <param name="columnIndex">The Column to test.</param>
        /// <returns>The left value of the column.</returns>
        public static int GetListViewColumnLeftProperty(ListView.ColumnHeaderCollection columns, int columnIndex)
        {
            int left = 0;
            for (int i = 0; i < columnIndex; i++)
                left += columns[i].Width;

            return left;
        }


        //////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////// STATIC PROPERTIES DEFINITION ////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Check if the property 'newPropertiesEnum' is already defined for this list view.
        /// </summary>
        /// <param name="listView">The list view to test.</param>
        /// <param name="prop">The new PropertiesEnum.</param>
        /// <returns>true if successful, false otherwise.</returns>
        private static bool CheckForProperty(ListView listView, NewPropertiesEnum prop)
        {
            if (listView == null)
                return false;

            CheckNewProperties(listView);
            if (!NewProperties[listView].ContainsKey(prop))
                NewProperties[listView][prop] = GetDefaultValueForProperty(prop);

            //Ensures that a custom class has been set to do the ordering
            if ((prop == NewPropertiesEnum.SortedProperty) || (prop == NewPropertiesEnum.SortKeyProperty)
                || (prop == NewPropertiesEnum.SortOrderProperty))
            {
                if ((listView.ListViewItemSorter == null) || !(listView.ListViewItemSorter is ListViewItemComparer))
                {
                    if (!NewProperties[listView].ContainsKey(NewPropertiesEnum.CustomSortingClass))
                        NewProperties[listView][NewPropertiesEnum.CustomSortingClass] = GetDefaultValueForProperty(NewPropertiesEnum.CustomSortingClass);

                    listView.ListViewItemSorter = (System.Collections.IComparer)NewProperties[listView][NewPropertiesEnum.CustomSortingClass];
                }
            }

            return true;
        }

        /// <summary>
        /// Checks if the lView is controlled by the newProperties Dictionary.
        /// </summary>
        /// <param name="listView">The ListView to test.</param>
        private static void CheckNewProperties(ListView listView)
        {
            if (!NewProperties.ContainsKey(listView))
            {
                NewProperties[listView] = new Dictionary<NewPropertiesEnum, object>();
                listView.Disposed += ListView_Disposed;
            }
        }

        /// <summary>
        /// Returns a default value for the specified property.
        /// </summary>
        /// <param name="prop">The property requesting a default value.</param>
        /// <returns>A default value casted as object.</returns>
        private static object GetDefaultValueForProperty(NewPropertiesEnum prop)
        {
            switch (prop)
            {
                case NewPropertiesEnum.SortedProperty:
                case NewPropertiesEnum.CorrectEventsBehavior:
                    return false;
                case NewPropertiesEnum.SortKeyProperty:
                    return 0;
                case NewPropertiesEnum.SortOrderProperty:
                    return SortOrder.Ascending;
                case NewPropertiesEnum.CustomSortingClass:
                    return new ListViewItemComparer();
                case NewPropertiesEnum.LargeIcons:
                case NewPropertiesEnum.SmallIcons:
                case NewPropertiesEnum.ColumnHeaderIcons:
                case NewPropertiesEnum.ItemClickMethod:
                    return string.Empty;
                case NewPropertiesEnum.ListItemSmallIcon:
                case NewPropertiesEnum.ListItemIcon:
                    return new Dictionary<ListViewItem, object>();
                case NewPropertiesEnum.InternalColumnHeaderImageListHelper:
                    return new ImageListHelper();
            }

            return null;
        }

        /// <summary>
        /// Gets the Custom List Item Comparer for a ListView.
        /// </summary>
        /// <param name="listView">The ListView to test.</param>
        /// <returns>The custom comparer for the ListView.</returns>
        private static ListViewItemComparer GetCustomListItemComparer(ListView listView)
        {
            return (ListViewItemComparer)NewProperties[listView][NewPropertiesEnum.CustomSortingClass];
        }

        /// <summary>
        /// The value for Sorted depends in the value of the property Sorting, so 
        /// whenever you set Sorted it syncs to Sorting.
        /// </summary>
        /// <param name="listView">The ListView to sync.</param>
        /// <param name="res">Indicates if sort must be done.</param>
        private static void SyncSortedProperty(ListView listView, bool res)
        {
            listView.Sorting = res ? SortOrder.Ascending : SortOrder.None;
        }

        /// <summary>
        /// Signals the object that initialization is starting.
        /// </summary>
        public void BeginInit()
        {
            _onInitialization = true;
        }

        /// <summary>
        /// Signals the object that initialization is complete.
        /// </summary>
        public void EndInit()
        {
            if (!DesignMode)
            {
                CleanDeadReferences();
                CorrectEventsBehavior();
                ProcessIconListsProperties();
            }
            _onInitialization = false;
        }

        /// <summary>
        /// Loads the .NET ImageLists to be used for ListViews from the VB6 ImageLists.
        /// </summary>
        private void ProcessIconListsProperties()
        {
            lock (ObjLockEvents)
            {
                try
                {
                    foreach (ListView listView in PendingListIconsToProcess)
                    {
                        if (NewProperties[listView].ContainsKey(NewPropertiesEnum.LargeIcons))
                            ProcessLargeIconsProperty(listView, DesignMode);

                        if (NewProperties[listView].ContainsKey(NewPropertiesEnum.SmallIcons))
                            ProcessSmallIconsProperty(listView, DesignMode);

                        if (NewProperties[listView].ContainsKey(NewPropertiesEnum.ColumnHeaderIcons))
                            ProcessColumnHeaderIconsProperty(listView, DesignMode);
                    }

                }
                catch (Exception)
                {
                }
                PendingListIconsToProcess.Clear();
            }
        }

        /// <summary>
        /// Process the Column Header Icons property for a ListView.
        /// </summary>
        /// <param name="listView">The ListView to set the property.</param>
        /// <param name="onDesignMode">Is Design Mode</param>
        private static void ProcessColumnHeaderIconsProperty(ListView listView, bool onDesignMode)
        {
            string name;

            ImageListHelper imgHelper;
            ImageListHelper currentImgHelper;
            object value = NewProperties[listView][NewPropertiesEnum.ColumnHeaderIcons];
            string s = value as string;
            if (s != null)
            {
                name = s;
                if (!string.IsNullOrEmpty(name))
                {
                    if (CheckForProperty(listView, NewPropertiesEnum.InternalColumnHeaderImageListHelper))
                    {
                        currentImgHelper = (ImageListHelper)NewProperties[listView][NewPropertiesEnum.InternalColumnHeaderImageListHelper];

                        if (!currentImgHelper.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                        {
                            imgHelper = GetImageListHelper(listView, name, onDesignMode);
                            imgHelper.Name = name;
                            NewProperties[listView][NewPropertiesEnum.InternalColumnHeaderImageListHelper] = imgHelper;
                            CleanColumnHeaderItemIconProperty(listView);
                        }
                    }
                }
                PatchDrawItemEvents(listView);
            }
            else if (ImageListHelper.IsValid(value))
            {
                if (CheckForProperty(listView, NewPropertiesEnum.InternalColumnHeaderImageListHelper))
                {
                    currentImgHelper = (ImageListHelper)NewProperties[listView][NewPropertiesEnum.InternalColumnHeaderImageListHelper];

                    name = (string)ReflectionHelper.GetMember(value, "Name");
                    if (!currentImgHelper.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        imgHelper = GetImageListHelper(value);
                        imgHelper.Name = name;
                        NewProperties[listView][NewPropertiesEnum.InternalColumnHeaderImageListHelper] = imgHelper;
                        CleanColumnHeaderItemIconProperty(listView);
                    }
                    PatchDrawItemEvents(listView);
                }
            }
        }

        /// <summary>
        /// Cleans the values for the ColumHeaderItemIcon of each ColumnHeader in the ListView.
        /// </summary>
        /// <param name="listView">The parent ListView.</param>
        private static void CleanColumnHeaderItemIconProperty(ListView listView)
        {
            foreach (ColumnHeader columnHeader in listView.Columns)
            {
                SetColumnHeaderItemIconProperty(columnHeader, -1);
            }
        }

        /// <summary>
        /// Process the small icons property for a ListView.
        /// </summary>
        /// <param name="listView">The ListView to set the property.</param>
        /// <param name="onDesignMode">Is Design Mode</param>
        private static void ProcessSmallIconsProperty(ListView listView, bool onDesignMode)
        {
            ImageListHelper imgHelper;
            object value = NewProperties[listView][NewPropertiesEnum.SmallIcons];
            string s = value as string;
            if (s != null)
            {
                if (!string.IsNullOrEmpty(s))
                {
                    imgHelper = GetImageListHelper(listView, s, onDesignMode);
                    NewProperties[listView][NewPropertiesEnum.SmallIcons] = imgHelper;
                    listView.SmallImageList = imgHelper.NETImageList;
                }
            }
            else if (ImageListHelper.IsValid(value))
            {
                imgHelper = GetImageListHelper(value);
                listView.SmallImageList = imgHelper.NETImageList;
            }
        }

        /// <summary>
        /// Process the large icons property for a ListView.
        /// </summary>
        /// <param name="listView">The ListView to set the property.</param>
        /// <param name="onDesignMode">Is Design Mode</param>
        private static void ProcessLargeIconsProperty(ListView listView, bool onDesignMode)
        {
            ImageListHelper imgHelper;
            object value = NewProperties[listView][NewPropertiesEnum.LargeIcons];
            string s = value as string;
            if (s != null)
            {
                if (!string.IsNullOrEmpty(s))
                {
                    imgHelper = GetImageListHelper(listView, s, onDesignMode);
                    listView.LargeImageList = imgHelper.NETImageList;
                }
            }
            else if (ImageListHelper.IsValid(value))
            {
                imgHelper = GetImageListHelper(value);
                listView.LargeImageList = imgHelper.NETImageList;
            }
        }

        /// <summary>
        /// Returns a ImageListHelper created based on a VB6 ImageList (name).
        /// </summary>
        /// <param name="listView">The ListView is used to get access to 
        /// the original VB6 ImageList based on its name.</param>
        /// <param name="vb6ImageListName">The name of the VB6 Image List.</param>
        /// <param name="onDesignMode">Is Design Mode?</param>
        /// <returns>An instance of a ImageListHelper.</returns>
        private static ImageListHelper GetImageListHelper(ListView listView, string vb6ImageListName, bool onDesignMode)
        {
            ImageListHelper imgHelper = new ImageListHelper();
            Form parentForm = listView.FindForm();
            if (parentForm != null)
            {
                object imlControl = ContainerHelper.Controls(parentForm)[vb6ImageListName];
                if (imlControl != null)
                {
                    imgHelper.LoadVB6ImageList(imlControl);
                }
                else
                {
                    if (!onDesignMode)
                    {
                        Type type = parentForm.GetType();
                        FieldInfo finfo = type.GetField(vb6ImageListName);
                        if (finfo != null)
                        {
                            object fieldValue = finfo.GetValue(parentForm);
                            imgHelper.NETImageList = fieldValue as ImageList;
                        }
                    }
                }
            }
            return imgHelper;
        }

        /// <summary>
        /// Returns a ImageListHelper created based on a VB6 ImageList object.
        /// </summary>
        /// <param name="vb6ImageList">The VB6 Imagelist object.</param>
        /// <returns>An instance of a ImageListHelper.</returns>
        private static ImageListHelper GetImageListHelper(object vb6ImageList)
        {
            ImageListHelper imgHelper = new ImageListHelper();
            imgHelper.LoadVB6ImageList(vb6ImageList);
            return imgHelper;
        }

        /// <summary>
        /// Cleans the public dictionaries from old references of ListViews alreay disposed.
        /// </summary>
        private void CleanDeadReferences()
        {
            try
            {
                List<ListView> toClean = new List<ListView>();
                foreach (ListView listView in NewProperties.Keys)
                {
                    if (listView.IsDisposed)
                        toClean.Add(listView);
                }
                foreach (ListView listView in toClean)
                {
                    NewProperties.Remove(listView);
                }

                toClean.Clear();
                foreach (ListView listView in EventsPatched.Keys)
                {
                    if (listView.IsDisposed)
                        toClean.Add(listView);
                }
                foreach (ListView listView in toClean)
                {
                    EventsPatched.Remove(listView);
                }
            }
            catch
            {
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////// FUNCTIONS TO PATCH THE EVENTS ///////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////
        /* This is how this path of events is going to work:
         *  When in design code the property "CorrectEventsBehavior" is set to true for a specific 
         *  listview, the following code will be applied at the end of execution of InitializeComponent,
         *  that means at the end of the design code.
         *  This code will:
         *      - Remove the event handlers for certains event as they were specified in design time
         *      - Add a custom event handler for the specific event being patch (defined below)
         *      - The custome events defined here will decide how and under what circunstances the
         *          original events will be called
         * 
         *  This mean that we will remove the events defined by the user and add our owns and we decide
         *  how and when to call the user defined events.
         * 
         *  Restrictions:
         *      This will path the events defined in design time, if the user specify another events in
         *      runtime code they will not be patched.
         */



        /// <summary>
        /// Deattach some events for the ListViews in order to be managed internally. 
        /// It means to replace the current behaviour.
        /// </summary>
        private static void CorrectEventsBehavior()
        {
            List<ListView> listViewsToCorrect = new List<ListView>();
            lock (ObjLockEvents)
            {
                foreach (ListView listView in NewProperties.Keys)
                {
                    if (NewProperties[listView].ContainsKey(NewPropertiesEnum.CorrectEventsBehavior)
                        && Convert.ToBoolean(NewProperties[listView][NewPropertiesEnum.CorrectEventsBehavior]))
                    {
                        listViewsToCorrect.Add(listView);
                        CorrectEventsForListView(listView);
                    }

                    //Patch for the ItemClicEvent
                    if (NewProperties[listView].ContainsKey(NewPropertiesEnum.ItemClickMethod))
                    {
                        PatchItemClickEvent(listView);
                    }
                }

                foreach (ListView listView in listViewsToCorrect)
                {
                    NewProperties[listView].Remove(NewPropertiesEnum.CorrectEventsBehavior);
                }
            }
        }

        /// <summary>
        /// Patchs the custom event ItemClick for a listView.
        /// </summary>
        /// <param name="listView">The source ListView.</param>
        private static void PatchItemClickEvent(ListView listView)
        {
            try
            {
                string methodName = Convert.ToString(NewProperties[listView][NewPropertiesEnum.ItemClickMethod]).Trim();
                if (!string.IsNullOrEmpty(methodName))
                {
                    if (!EventsPatched[listView].ContainsKey(ItemClickEventName))
                    {
                        MethodInfo mInfo = listView.FindForm().GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

                        Delegate del;
                        del = mInfo.IsStatic ? Delegate.CreateDelegate(typeof(ListViewItemClickDelegate), mInfo) : Delegate.CreateDelegate(typeof(ListViewItemClickDelegate), listView.FindForm(), mInfo);

                        if (!EventsPatched.ContainsKey(listView))
                            EventsPatched.Add(listView, new Dictionary<string, List<Delegate>>());

                        EventsPatched[listView].Add(ItemClickEventName, new List<Delegate>());
                        EventsPatched[listView][ItemClickEventName].Add(del);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format(Resources.UpgradeHelpers_VB6.UpgradeHelpers_VB6_Help_ListViewHelper_PatchItemClickEvent_Err_Msg, e.Message));
            }
        }

        /// <summary>
        /// Patches the events for a specific ListView.
        /// </summary>
        /// <param name="listView">The source ListView.</param>
        private static void CorrectEventsForListView(ListView listView)
        {
            if (EventsPatched.ContainsKey(listView))
                throw new InvalidOperationException("Events for this list view has been previously patched: '" + listView.Name + "'");

            EventsPatched.Add(listView, new Dictionary<string, List<Delegate>>());
            foreach (string eventName in EventsToCorrect.Keys)
            {
                EventInfo eventInfo = listView.GetType().GetEvent(eventName);
                if (eventInfo == null)
                    throw new InvalidOperationException("Event info for event '" + eventName + "' could not be found");

                EventsPatched[listView].Add(eventName, new List<Delegate>());
                Delegate[] eventDelegates = ContainerHelper.GetEventSubscribers(listView, eventName);
                if (eventDelegates != null)
                {

                    foreach (Delegate del in eventDelegates)
                    {
                        EventsPatched[listView][eventName].Add(del);
                        eventInfo.RemoveEventHandler(listView, del);
                    }
                }
                eventInfo.AddEventHandler(listView, EventsToCorrect[eventName]);
            }
        }

        /// <summary>
        /// Allows to invoke the patched events for a ListView.
        /// </summary>
        /// <param name="source">The source ListView.</param>
        /// <param name="eventName">The name of the event to be invoked.</param>
        /// <param name="args">The args of the event to be used in the invokation.</param>
        private static void InvokeEvents(ListView source, string eventName, object[] args)
        {
            if (EventsPatched.ContainsKey(source) && EventsPatched[source].ContainsKey(eventName))
            {
                foreach (Delegate del in EventsPatched[source][eventName])
                {
                    del.DynamicInvoke(args);
                }
            }
        }

        /// <summary>
        /// Event handler for the Disposed event of a ListView so we can clean it from EventsPatched.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private static void ListView_Disposed(object sender, EventArgs e)
        {
            ListView listView = (ListView)sender;
            if (NewProperties.ContainsKey(listView))
                NewProperties.Remove(listView);

            if (EventsPatched.ContainsKey(listView))
                EventsPatched.Remove(listView);
        }

        /// <summary>
        /// Removes a control from the patched events list.
        /// </summary>
        /// <param name="listView">The ListView to remove.</param>
        public void ManuallyRemoveFromPatchedEvents(ListView listView)
        {
            if (NewProperties.ContainsKey(listView))
                NewProperties.Remove(listView);
            if (EventsPatched.ContainsKey(listView))
                EventsPatched.Remove(listView);
        }

        /// <summary>
        /// Event handler for the MouseUp event of a ListView.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private static void ListView_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                ListView source = (ListView)sender;
                if (source.Focused)
                    InvokeEvents(source, ClickEventName, new object[] { sender, new EventArgs() });

                InvokeEvents(source, MouseUpEventName, new object[] { sender, e });

            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Event handler for the Click event of a ListView.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private static void ListView_Click(object sender, EventArgs e)
        {
            try
            {
                //This event won't be fired from here, it will be fired by MouseUp event
                ListView source = (ListView)sender;

                //It will fire ItemClick event instead
                if (source.FocusedItem != null)
                    InvokeEvents(source, ItemClickEventName, new object[] { source.FocusedItem });

            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Event handler for the Double Click event of a ListView.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private static void ListView_DoubleClick(object sender, EventArgs e)
        {
            //Nothing to do with this event
            //It will be fired by IMessageFilterImplementer.PreFilterMessage
        }

        /// <summary>
        /// Event handler for the Key Up event of a ListView.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The key event arguments.</param>
        private static void ListView_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                ListView source = (ListView)sender;

                //It will fire ItemClick event
                if ((source.FocusedItem != null) && (e.KeyCode != Keys.Tab) && (e.KeyCode != Keys.Enter))
                    InvokeEvents(source, ItemClickEventName, new object[] { source.FocusedItem });

                InvokeEvents(source, KeyUpEventName, new object[] { sender, e });

            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Event handler for the DrawItem event of a ListView, 
        /// required to manage the property SmallIcon of the ListItems.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The DrawListView event arguments.</param>
        private static void ListView_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            ListView listView = (ListView)sender;

            if (CheckForProperty(listView, NewPropertiesEnum.ListItemIcon))
            {
                Dictionary<ListViewItem, object> listViewItemIconLists = (Dictionary<ListViewItem, object>)NewProperties[listView][NewPropertiesEnum.ListItemIcon];

                if (CheckForProperty(listView, NewPropertiesEnum.ListItemSmallIcon))
                {
                    Dictionary<ListViewItem, object> listViewItemSmallIconLists = (Dictionary<ListViewItem, object>)NewProperties[listView][NewPropertiesEnum.ListItemSmallIcon];
                    object value;
                    if (listView.View == View.LargeIcon)
                    {
                        if (listViewItemIconLists.ContainsKey(e.Item))
                        {
                            value = listViewItemIconLists[e.Item];
                            string s = value as string;
                            if (s != null)
                            {
                                e.Item.ImageIndex = -1;
                                e.Item.ImageKey = s;
                            }
                            else
                            {
                                e.Item.ImageKey = string.Empty;
                                e.Item.ImageIndex = (int)value;
                            }
                            listViewItemIconLists.Remove(e.Item);
                        }
                    }
                    else
                    {
                        if (!listViewItemIconLists.ContainsKey(e.Item))
                        {
                            if (!string.IsNullOrEmpty(e.Item.ImageKey))
                                listViewItemIconLists.Add(e.Item, e.Item.ImageKey);
                            else
                                listViewItemIconLists.Add(e.Item, e.Item.ImageIndex);

                            if (listViewItemSmallIconLists.ContainsKey(e.Item))
                            {
                                value = listViewItemSmallIconLists[e.Item];
                                string s = value as string;
                                if (s != null)
                                {
                                    e.Item.ImageIndex = -1;
                                    e.Item.ImageKey = s;
                                }
                                else
                                {
                                    e.Item.ImageKey = string.Empty;
                                    e.Item.ImageIndex = (int)value;
                                }
                            }
                            /*
                            else
                            {
                                e.Item.ImageIndex = -1;
                                e.Item.ImageKey = string.Empty;
                            }
                             */
                        }
                    }
                    InvokeEvents(listView, DrawItemEventName, new object[] { sender, e });
                }
            }
            //e.DrawDefault = true;
        }

        /// <summary>
        /// Event handler for the DrawSubItem event of a ListView, 
        /// required in order to process the property 
        /// listviewSubItem.UseItemStyleForSubItems when it is set to false.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The DrawListViewSubItem event arguments.</param>
        public static void ListView_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            ListView listView = (ListView)sender;
            if (e.ColumnIndex == 0) //do default drawing for the first column
            {
                e.DrawDefault = true;
            }
            else
            {
                //Draw SubItem
                ListViewItem.ListViewSubItem subitem = e.SubItem;
                if (subitem == null || subitem.Tag == null)
                {
                    e.DrawDefault = true;
                }
                else if (subitem.Tag != null)
                {
                    Image img = null;
                    ImageListHelper imgHelper = (ImageListHelper)NewProperties[listView][NewPropertiesEnum.SmallIcons];
                    if (imgHelper != null)
                    {
                        if (subitem.Tag is string)
                        {
                            string imgKey = subitem.Tag.ToString();
                            if (imgHelper.NETImageList.Images.ContainsKey(imgKey))
                            {
                                img = imgHelper.NETImageList.Images[imgKey];
                            }
                        }
                        else
                        {
                            if (subitem.Tag is int)
                            {
                                int imgIndex = Convert.ToInt32(subitem.Tag);
                                if (imgHelper.NETImageList.Images.Count > imgIndex)
                                {
                                    img = imgHelper.NETImageList.Images[imgIndex];
                                }
                            }
                        }
                    }
                    if (img != null)
                    {
                        e.DrawBackground();
                        bool focused = e.Item.ListView.Focused;
                        Color back = e.Item.Selected ? (focused ? SystemColors.Highlight : SystemColors.Menu) : e.SubItem.ForeColor;
                        Color fore = e.Item.Selected ? (focused ? SystemColors.HighlightText : e.SubItem.ForeColor) : e.SubItem.ForeColor;
                        int fonty = e.Bounds.Y + e.Bounds.Height / 2 - e.SubItem.Font.Height / 2;
                        int x = e.Bounds.X + 2;
                        if (e.Item.Selected)
                        {
                            using (Brush backBrush = new SolidBrush(back))
                                e.Graphics.FillRectangle(backBrush, e.Bounds);
                        }
                        x = DrawSubItemIcon(e, x, img);
                        using (Brush foreBrush = new SolidBrush(fore))
                            e.Graphics.DrawString(e.SubItem.Text, e.SubItem.Font, foreBrush, x, fonty);
                    }
                    else
                        e.DrawDefault = true;
                }
                else
                    e.DrawDefault = true;
            }
            InvokeEvents(listView, DrawSubItemEventName, new object[] { sender, e });
            //e.DrawDefault = true;
        }

        /// <summary>
        /// Method to draw the icon on the subitem 
        /// </summary>
        /// <param name="e">The event arguments</param>
        /// <param name="x">The x coordinate </param>
        /// <param name="image">The image to be drawn</param>
        /// <returns>The x position after the drawing action</returns>
        private static int DrawSubItemIcon(DrawListViewSubItemEventArgs e, int x, Image image)
        {
            Bitmap myBitmap = new Bitmap(image);
            Icon myIcon = Icon.FromHandle(myBitmap.GetHicon());
            int y = e.Bounds.Y + ((e.Bounds.Height / 2) - (myIcon.Height / 2));
            e.Graphics.DrawIcon(myIcon, x, y);
            x += myIcon.Width + 2;
            return x;
        }

        /// <summary>
        /// Event handler for the DrawColumnHeader event of a ListView, 
        /// required to manage the property SmallIcon of the ListItems.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private static void ListView_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            ListView listView = (ListView)sender;

            if (listView.View == View.Details)
            {
                if (NewProperties.ContainsKey(listView) && NewProperties[listView].ContainsKey(NewPropertiesEnum.InternalColumnHeaderImageListHelper))
                {
                    ColumnHeader colH = e.Header;
                    ImageListHelper imgHelper = (ImageListHelper)NewProperties[listView][NewPropertiesEnum.InternalColumnHeaderImageListHelper];
                    Image img;
                    if (!string.IsNullOrEmpty(colH.ImageKey))
                    {
                        if (imgHelper.NETImageList.Images.ContainsKey(colH.ImageKey))
                        {
                            img = imgHelper.NETImageList.Images[colH.ImageKey];
                            DrawColumnHeader(e, colH, img);
                        }
                        else
                            DrawColumnHeader(e, colH, null);
                    }
                    else if (colH.ImageIndex >= 0)
                    {
                        if (imgHelper.NETImageList.Images.Count > colH.ImageIndex)
                        {
                            img = imgHelper.NETImageList.Images[colH.ImageIndex];
                            DrawColumnHeader(e, colH, img);
                        }
                        else
                            DrawColumnHeader(e, colH, null);
                    }
                    else
                        e.DrawDefault = true;
                }
                else
                    e.DrawDefault = true;
            }
            else
                e.DrawDefault = true;

            InvokeEvents(listView, DrawColumnHeaderEventName, new object[] { sender, e });
        }

        /// <summary>
        /// Takes care of drawing a column header using an image.
        /// </summary>
        /// <param name="e">The DrawListViewColumnHeader event arguments.</param>
        /// <param name="colH">The ColumnHeader to be drawn.</param>
        /// <param name="img">The Image where to draw the column.</param>
        private static void DrawColumnHeader(DrawListViewColumnHeaderEventArgs e, ColumnHeader colH, Image img)
        {
            int width = TextRenderer.MeasureText(" ", e.Font).Width;
            string text = colH.Text;
            HorizontalAlignment textAlign = colH.TextAlign;
            TextFormatFlags flags = (textAlign == HorizontalAlignment.Left) ? TextFormatFlags.GlyphOverhangPadding : ((textAlign == HorizontalAlignment.Center) ? TextFormatFlags.HorizontalCenter : TextFormatFlags.Right);
            flags |= TextFormatFlags.WordEllipsis | TextFormatFlags.VerticalCenter;

            if (img != null)
            {
                int halfWidth = width / 2;

                Rectangle imgBounds = new Rectangle(e.Bounds.Location.X + halfWidth, e.Bounds.Location.Y + 1, e.Bounds.Size.Height, e.Bounds.Size.Height - 3);
                Rectangle txtBounds = new Rectangle(imgBounds.Location.X + imgBounds.Size.Width + halfWidth, e.Bounds.Location.Y, e.Bounds.Size.Width - (imgBounds.Size.Width + 2 * halfWidth), e.Bounds.Size.Height);

                e.DrawBackground();
                e.Graphics.DrawImage(img, imgBounds);
                TextRenderer.DrawText(e.Graphics, text, e.Font, txtBounds, e.ForeColor, flags);
            }
            else
            {
                e.DrawBackground();
                TextRenderer.DrawText(e.Graphics, text, e.Font, e.Bounds, e.ForeColor, flags);
            }
        }




        /// <summary>
        /// Class provided to patch some events that require to catch the messages from windows
        /// like DoubleClick event for a ListView.
        /// </summary>
        private class IMessageFilterImplementer : IMessageFilter
        {
            /// <summary>
            /// Catches the DoubleClick Windows Message.
            /// </summary>
            /// <param name="m">The Windows Message.</param>
            /// <returns>Always returns false.</returns>
            public bool PreFilterMessage(ref Message m)
            {
                try
                {
                    //DoubleClick message
                    if (m.Msg == 0x203)
                    {
                        foreach (ListView listView in EventsPatched.Keys)
                        {
                            if ((!listView.IsDisposed) && listView.Handle.Equals(m.HWnd))
                            {
                                //Fire the DoubleClick events
                                InvokeEvents(listView, DoubleClickEventName, new object[] { listView, new EventArgs() });
                                return false;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }

                return false;
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////// FUNCTIONS TO PATCH THE EVENTS ///////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Custom class to do the comparison of columns of a ListView.
        /// </summary>
        public class ListViewItemComparer : System.Collections.IComparer
        {
            /// <summary>
            /// Stores if ListView is sorted.
            /// </summary>
            private bool _sorted;
            /// <summary>
            /// Indicates if ListView is sorted.
            /// </summary>
            public bool Sorted
            {
                get { return _sorted; }
                set { _sorted = value; }
            }

            /// <summary>
            /// Stores the column used for sorting
            /// </summary>
            private int _sortKey = 0;
            /// <summary>
            /// Indicates the SortKey value for the ListView.
            /// </summary>
            public int SortKey
            {
                get { return _sortKey; }
                set { _sortKey = value; }
            }

            /// <summary>
            /// Stores the SortOrder value for the ListView.
            /// </summary>
            private SortOrder _sortOrder = SortOrder.Ascending;
            /// <summary>
            /// Indicates the SortOrder value for the ListView.
            /// </summary>
            public SortOrder SortOrder
            {
                get { return _sortOrder; }
                set { _sortOrder = value; }
            }

            /// <summary>
            /// Does the comparison between two ListView items.
            /// </summary>
            /// <param name="x">A ListView item to be compared.</param>
            /// <param name="y">A ListView item to be compared.</param>
            /// <returns>The result of the comparison.</returns>
            public int Compare(object x, object y)
            {
                if (_sorted)
                {
                    if ((((ListViewItem)x).SubItems.Count > SortKey) && (((ListViewItem)y).SubItems.Count > SortKey))
                    {
                        if (_sortOrder == SortOrder.Ascending)
                            return String.CompareOrdinal(((ListViewItem)x).SubItems[SortKey].Text, ((ListViewItem)y).SubItems[SortKey].Text);
                        if (_sortOrder == SortOrder.Descending)
                            return String.CompareOrdinal(((ListViewItem)y).SubItems[SortKey].Text, ((ListViewItem)x).SubItems[SortKey].Text);
                    }
                }

                return 0;
            }
        }
    }

    /// <summary>
    /// Class To Extend ListViewItems
    /// </summary>
    public static class ListViewExtensions
    {
        /// <summary>
        /// Get the actual selected item
        /// </summary>
        /// <param name="list">ListView to check on</param>
        /// <returns>ListViewItem selected</returns>
        public static ListViewItem SelectedItem(ListView list)
        {
            return list.SelectedItems.Count > 0 ? list.SelectedItems[0] : null;
        }
    }
}
