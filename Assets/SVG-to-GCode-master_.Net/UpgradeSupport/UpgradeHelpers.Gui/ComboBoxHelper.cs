using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace UpgradeHelpers.Gui
{
    /// <summary>
    /// ComboBoxHelper adds functionality to .NET ComboBox using multiple columns;
    /// </summary>
    public class ComboBoxHelper : ComboBox
    {
        /// <summary>
        /// Contructor
        /// </summary>
        public ComboBoxHelper()
        {
            InitializeComboBoxHelper();
        }

        private void InitializeComboBoxHelper()
        {
            Columns = 1;
            Text = "";
            SearchText = "";
            DrawMode = DrawMode.OwnerDrawFixed;
            // Handle the DrawItem event to draw the items.
            DrawItem += ComboBoxHelper_DrawItem;
            SelectedIndexChanged += ComboBoxHelper_SelectedIndexChanged;
            Click += ComboBoxHelper_Click;
            Clicked = false;
            _listwidth = Bounds.Width;
        }

        #region Events

        void ComboBoxHelper_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ColumnEdit >= 0)
            {

                string str = (string)Items[SelectedIndex];
                string[] values = str.Split(new char[] { Convert.ToChar(_columnSeparatorChar) });
                if (ColumnEdit < values.Length)
                {
                    Text = values[ColumnEdit];
                }
            }
        }
        /// <summary>
        /// Combo List comparer class
        /// </summary>
        class Comparer : IComparer<string>
        {
            private readonly char charSeparator;
            private readonly int columns;
            readonly Dictionary<int, SortedConstants> sort = new Dictionary<int, SortedConstants>();

            int type;

            /// <summary>
            /// Contructor
            /// </summary>
            /// <param name="charSeparator">The character separator to use.</param>
            /// <param name="columns">The number of columns.</param>
            public Comparer(char charSeparator, int columns)
            {
                Sorted = SortedConstants.SortedNone;
                Column = -1;
                this.charSeparator = charSeparator;
                this.columns = columns;
                type = 1;
            }

            /// <summary>
            /// Comparer using the sorts by columns and specify method
            /// </summary>
            /// <param name="charSeparator">separator</param>
            /// <param name="sorts">list of columns to use for sorting</param>
            /// <param name="sorted">method on each column to sort</param>
            /// <param name="columns">number of columns in the list</param>
            public Comparer(char charSeparator, int[] sorts, SortedConstants[] sorted, int columns)
            {
                this.charSeparator = charSeparator;
                this.columns = columns;
                Dictionary<int, int> d = new Dictionary<int, int>();
                for (int col = 0; col < sorted.Length; col++)
                {
                    d[col] = sorts[col];
                }
                SortedDictionary<int, int> dsorted = new SortedDictionary<int, int>(d);

                foreach (KeyValuePair<int, int> k in dsorted)
                {
                    int col = k.Key;
                    if (sorted[col] != SortedConstants.SortedNone)
                    {
                        sort[col] = sorted[col];
                    }
                }
                type = 2;
            }

            private SortedConstants _sorted1;

            /// <summary>
            /// Sort direction
            /// </summary>
            public SortedConstants Sorted
            {
                get { return _sorted1; }
                set { _sorted1 = value; }
            }

            private int _column;

            /// <summary>
            /// Column to use for sorting or -1 to use whole row
            /// </summary>
            public int Column
            {
                get { return _column; }
                set { _column = value; }
            }

            public int Compare(string x, string y)
            {
                if (type == 1)
                {
                    return CompareType1(x, y);
                }
                return CompareType2(x, y);
            }

            int CompareType1(string x, string y)
            {
                string val1 = x;
                string val2 = y;
                if (Column >= 0 && columns > 1)
                {
                    string[] values = val1.Split(new char[] { charSeparator });
                    if (values.Length > Column)
                    {
                        val1 = values[Column];
                    }
                    values = val2.Split(new char[] { charSeparator });
                    if (values.Length > Column)
                    {
                        val2 = values[Column];
                    }
                }
                switch (Sorted)
                {
                    case SortedConstants.SortedAscending:
                        return String.Compare(val1, val2, StringComparison.Ordinal);
                    case SortedConstants.SortedDescending:
                        return String.Compare(val2, val1, StringComparison.Ordinal);
                    default:
                        return 0;
                }
            }

            int CompareType2(string x, string y)
            {
                string val1 = x;
                string val2 = y;
                int result = 0;

                try
                {
                    if (columns > 1)
                    {
                        string[] values1 = val1.Split(new char[] { charSeparator });
                        string[] values2 = val2.Split(new char[] { charSeparator });
                        foreach (KeyValuePair<int, SortedConstants> k in sort)
                        {
                            val1 = values1[k.Key];
                            val2 = values2[k.Key];
                            SortedConstants constant = k.Value;
                            switch (constant)
                            {
                                case SortedConstants.SortedAscending:
                                    result = String.Compare(val1, val2, StringComparison.Ordinal);
                                    break;
                                case SortedConstants.SortedDescending:
                                    result = String.Compare(val2, val1, StringComparison.Ordinal);
                                    break;
                                default:
                                    result = 0;
                                    break;
                            }
                            if (result != 0)
                                break;
                        }
                    }
                }
                catch (Exception)
                {
                }
                return result;
            }
        }

        void ComboBoxHelper_Click(object sender, EventArgs e)
        {
            Clicked = true;
        }

        void ComboBoxHelper_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index > Items.Count) return;
            // Draw the default background
            e.DrawBackground();

            // The ComboBox is bound to a DataTable,
            // so the items are DataRowView objects.
            string str = (string)Items[e.Index];
            string[] values = str.Split(new char[] { Convert.ToChar(_columnSeparatorChar) });
            int x = 0;
            bool firstTime = false;
            for (int i = 0; i < Columns; i++)
            {
                if (_colhide[i] == false)
                {
                    // Get the bounds for the first column
                    Rectangle r = e.Bounds;
                    r.X = x;
                    r.Width = _columnwidths[i];
                    x += _columnwidths[i];


                    if (firstTime)
                    {
                        // Draw a line to isolate the columns 
                        using (Pen p = new Pen(Color.Black))
                        {
                            e.Graphics.DrawLine(p, r.Left, 0, r.Left, r.Bottom);
                        }
                    }
                    else
                    {
                        firstTime = true;
                    }
                    // Draw the text on the first column
                    using (SolidBrush sb = new SolidBrush(e.ForeColor))
                    {
                        str = "";
                        if (values.Length > i) str = values[i];
                        e.Graphics.DrawString(str, e.Font, sb, r);
                    }
                }
            }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Adds an item to the combo
        /// </summary>
        /// <param name="item">value of item to add</param>
        public void AddItem(string item)
        {
            InsertRow = item;
        }

        /// <summary>
        /// Adds an item to the combo as index position
        /// </summary>
        /// <param name="item">value of item to add</param>
        /// <param name="index">Index position</param>
        public void AddItem(string item, int index)
        {
            /*if (_datatable == null)
            {
                _datatable = new DataTable();
                _datatable.Columns.Add("field", typeof(string));
                this.DataSource = _datatable;
                this.DisplayMember = "field";
            }
            DataRow row = _datatable.NewRow();
            row["field"] = Item;
            _datatable.Rows.InsertAt(row, Index);*/
            int localIndex = Items.Count > index ? index : Items.Count;
            Items.Insert(localIndex, item);
        }

        /// <summary>
        /// Remove item as index specified
        /// </summary>
        /// <param name="index">index to be removed</param>
        public void RemoveItem(int index)
        {
            if (index >= 0 && index < Items.Count)
            {
                Items.RemoveAt(index);
            }
        }

        #endregion

        #region Properties

        private int _listwidth = 0;
        /// <summary>
        /// Gets or sets the width of the drop-down list in the combo box.
        /// </summary>
        /// <returns></returns>
        public int ListWidth
        {
            get { return _listwidth; }
            set
            {
                if (value > 0)
                {
                    this.DropDownWidth = value;
                    _listwidth = value;
                }
            }
        }

        private ComboBoxHelperActionConstants _action = ComboBoxHelperActionConstants.ActionClear;
        /// <summary>
        ///  Sets or returns a value that designates an action, such as inserting a column or deleting a row.
        /// </summary> 
        public ComboBoxHelperActionConstants Action
        {
            get { return _action; }
            set
            {
                _action = value;
                switch (_action)
                {
                    case ComboBoxHelperActionConstants.ActionClear:
                        this.Items.Clear();
                        this.Text = "";
                        break;
                    case ComboBoxHelperActionConstants.ActionSearch:
                        if (this.SearchText != null)
                        {
                            int index = 0;
                            foreach (string item in this.Items)
                            {
                                string s = item;
                                if (this.Columns > 1)
                                {
                                    if (this.ColumnSearch >= 0)
                                    {
                                        string[] values = s.Split(new char[] { Convert.ToChar(this._columnSeparatorChar) });
                                        if (values.Length > this.ColumnSearch)
                                        {
                                            s = values[this.ColumnSearch];
                                        }
                                    }
                                }
                                if (this.SearchMethod == SearchMethodConstants.SearchMethodExactMatch)
                                {
                                    if (s.CompareTo(this.SearchText) == 0)
                                    {
                                        this.SearchIndex = index;
                                        break;
                                    }
                                }
                                else if (this.SearchMethod == SearchMethodConstants.SearchMethodGreaterOrEqual)
                                {
                                    if (s.CompareTo(this.SearchText) > 0)
                                    {
                                        this.SearchIndex = index;
                                        break;
                                    }
                                }
                                else if (this.SearchMethod == SearchMethodConstants.SearchMethodPartialMatch)
                                {
                                    if (s.IndexOf(this.SearchText) > 0)
                                    {
                                        this.SearchIndex = index;
                                        break;
                                    }
                                }
                                index++;
                            }
                        }
                        break;
                    case ComboBoxHelperActionConstants.ActionClearSearchBuffer:
                        for (int i = 0; i < _columns; i++)
                        {
                            _columnsortsvalues[i] = SortedConstants.SortedNone;
                            _columnsorts[i] = -1;
                        }
                        break;
                }
            }
        }

        private int _columns = 1;
        /// <summary>
        ///   Sets or returns the number of columns to display in a ComboBox.
        /// </summary>
        public int Columns
        {
            get { return _columns; }
            set
            {
                if (value >= 0)           
                {
                    _columns = value;
                    _columnwidths = new int[_columns];
                    _columnsorts = new int[_columns];
                    _columnsortsvalues = new SortedConstants[_columns];
                    _colhide = new bool[_columns];
                    for (int i = 0; i < _columns; i++)
                    {
                        _columnwidths[i] = this.Bounds.Width / _columns;
                        _columnsortsvalues[i] = SortedConstants.SortedAscending;
                        _columnsorts[i] = 0;
                        _colhide[i] = false;
                    }
                }
            }
        }

        private int _row = -1;
        /// <summary>
        /// Sets or returns a row in an fpCombo or fpList control.
        /// </summary>
        public int Row
        {
            get { return _row; }
            set { _row = value; }
        }

        /// <summary>
        /// Sets whether to insert a new item or row of text in the list of an fpCombo or fpList control.
        /// </summary>
        public string InsertRow
        {
            set
            {
                /*if (_datatable == null)
                {
                    _datatable = new DataTable();
                    _datatable.Columns.Add("field", typeof(string));
                    this.DataSource = _datatable;
                    this.DisplayMember = "field";
                }
                _datatable.Rows.Add(value);*/
                this.Items.Add(value);
            }
        }

        private bool _clicked;

        /// <summary>
        /// Handles if ComboBox was clicked.
        /// </summary>
        public bool Clicked
        {
            get { return _clicked; }
            set { _clicked = value; }
        }

        private bool[] _colhide;
        /// <summary>
        /// Sets or returns whether to hide a column in a multiple-column fpCombo or fpList control.
        /// </summary>
        public bool ColHide
        {
            get
            {
                if (this.Col >= 0 && this.Col < _colhide.Length)
                {
                    return _colhide[this.Col];
                }
                else
                {
                    return false;
                }
            }
            set
            {
                int col = this.Col;
                if (col >= 0)
                {
                    if (col > _colhide.Length)
                        col = _colhide.Length - 1;
                    _colhide[col] = value;
                }
            }
        }

        private int _col = 0;

        /// <summary>
        /// Sets or returns the index number of a column in an fpCombo or fpList control.
        /// </summary>
        public int Col
        {
            get { return _col; }
            set
            {
                if (value >= 0 && value < this.Columns)
                {
                    _col = value;
                }
            }
        }

        private int[] _columnwidths;
        /// <summary>
        /// Get/Set widths for each column to display
        /// </summary>
        public int ColWidth
        {
            get
            {
                if (_columnwidths != null)
                {
                    return _columnwidths[_col];
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                if (_columnwidths != null)
                {
                    _columnwidths[_col] = value * 17;
                    /*int width = 0;
                    for (int i = 0; i < _columnwidths.Length; i++)
                    {
                        width += _columnwidths[i];
                    }
                    this.SetBounds(this.Location.X, this.Location.Y, width, this.Bounds.Height);*/
                }
            }
        }

        private int[] _columnsorts;
        /// <summary>
        /// Sets or returns the order in which a column in a multiple-column fpCombo or fpList control is sorted.
        /// </summary>
        public int ColSortSeq
        {
            get
            {
                if (_columnsorts != null)
                {
                    return _columnsorts[_col];
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                if (_columnsorts != null)
                {
                    _columnsorts[_col] = value;
                    SortList();
                }
            }
        }

        private void SortList()
        {
            List<string> s = new List<string>();
            foreach (object row in this.Items)
            {
                s.Add(row.ToString());
            }
            Comparer comparer = new Comparer(Convert.ToChar(ColumnSeparatorChar), _columnsorts, _columnsortsvalues, Columns);
            s.Sort(comparer);
            this.Items.Clear();
            foreach (string str in s)
            {
                this.Items.Add(str);
            }
        }

        /// <summary>
        /// Sets or returns the selected row's column value displayed in a multiple-column fpCombo or fpList control.
        /// </summary>
        public string ColText
        {
            get
            {
                string val = "";
                try
                {
                    val = this.Text;
                    if (this.Columns > 1)
                    {
                        int col = this.Col;
                        if (col <= 0)
                        {
                            col = 0;
                        }
                        else if (col >= this.Columns)
                        {
                            col = this.Columns - 1;
                        }
                        string[] vals = val.Split(new char[] { Convert.ToChar(this._columnSeparatorChar) });
                        val = vals[col];
                    }
                }
                catch
                {
                }
                return val;
            }
            set
            {
                if (this.Items.Count > this.Row && this.Row >= 0)
                    this.Items[this.Row] = value;
            }
        }

        private int _columnEdit;

        /// <summary>
        /// Sets or returns whether one column value or all column values are displayed in the edit field of a multiple-column fpCombo control.
        /// </summary>
        public int ColumnEdit
        {
            get { return _columnEdit; }
            set { _columnEdit = value; }
        }

        private int _columnSearch;

        /// <summary>
        ///  Sets or returns the number of the column searched when searching a multiple-column fpCombo or fpList control.
        /// </summary>
        public int ColumnSearch
        {
            get { return _columnSearch; }
            set { _columnSearch = value; }
        }

        private int _columnSeparatorChar = 9;
        /// <summary>
        /// Sets or returns the character used to separate column values in a multiple-column fpCombo or fpList control.
        /// </summary>
        public int ColumnSeparatorChar
        {
            get { return _columnSeparatorChar; }
            set { _columnSeparatorChar = value; }
        }

        private ColumnWidthScaleConstants _columnWidthScale;

        /// <summary>
        /// Sets or returns the measurement unit used to specify column and group widths in a multiple-column fpCombo or fpList control.
        /// </summary>
        [Obsolete("ColumnWidthScale is not been processed")]
        public ColumnWidthScaleConstants ColumnWidthScale
        {
            get { return _columnWidthScale; }
            set { _columnWidthScale = value; }
        }

        private SortedConstants[] _columnsortsvalues;
        /// <summary>
        ///  Sets or returns the type of sort performed on a column in a multiple-column fpCombo or fpList control.
        /// </summary>
        public SortedConstants ColSorted
        {
            get
            {
                if (_columnsortsvalues != null)
                {
                    return _columnsortsvalues[_col];
                }
                else
                {
                    return SortedConstants.SortedNone;
                }
            }
            set
            {
                if (_columnsortsvalues != null)
                {
                    _columnsortsvalues[_col] = value;
                    SortList();
                }
            }
        }

        /// <summary>
        /// Return number of rows inside the Combo.
        /// </summary>
        public int ListCount
        {
            get
            {
                return this.Items.Count;
            }
            set
            {
                //throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Get/Set List Index
        /// </summary>
        public int ListIndex
        {
            get
            {
                return this.SelectedIndex;
            }
            set
            {
                if (value >= 0 && value < this.Items.Count)
                {
                    this.SelectedIndex = value;
                }
            }
        }

        /// <summary>
        /// Access Item inside ComboBox Items
        /// </summary>
        /// <param name="index">index to search</param>
        /// <returns>value inside the list</returns>
        public string this[int index]
        {
            get
            {
                if (index >= 0 && index < this.Items.Count)
                {
                    return this.Items[index].ToString();
                }
                else
                {
                    return "";
                }
            }
            set
            {
                if (index >= 0 && index < this.Items.Count)
                {
                    this.Items[index] = value;
                }
            }
        }

        private int _searchIndex;

        /// <summary>
        /// Search Index
        /// </summary>
        public int SearchIndex
        {
            get { return _searchIndex; }
            set { _searchIndex = value; }
        }

        private string _searchText;

        /// <summary>
        /// Search for text
        /// </summary>
        public string SearchText
        {
            get { return _searchText; }
            set { _searchText = value; }
        }

        private SearchMethodConstants _searchMethod;

        /// <summary>
        /// Search Method
        /// </summary>
        public SearchMethodConstants SearchMethod
        {
            get { return _searchMethod; }
            set { _searchMethod = value; }
        }

        private SortedConstants _sorted;
        /// <summary>
        /// Sort for items in the Combo
        /// </summary>
        public new SortedConstants Sorted
        {
            get
            {
                return _sorted;
            }
            set
            {
                _sorted = value;
                List<string> s = new List<string>();
                foreach (object row in this.Items)
                {
                    s.Add(row.ToString());
                }
                Comparer comparer = new Comparer(Convert.ToChar(ColumnSeparatorChar), Columns);
                comparer.Sorted = _sorted;
                s.Sort(comparer);
                this.Items.Clear();
                foreach (string str in s)
                {
                    this.Items.Add(str);
                }
            }
        }

        #endregion

        #region Enum data types

        /// <summary>
        /// SearchMethod
        /// </summary>
        public enum SearchMethodConstants
        {
            /// <summary>
            /// Exact Match
            /// </summary>
            SearchMethodExactMatch,
            /// <summary>
            /// Greater or Equal
            /// </summary>
            SearchMethodGreaterOrEqual,
            /// <summary>
            /// Partial match
            /// </summary>
            SearchMethodPartialMatch
        }

        /// <summary>
        /// SortedConstants
        /// </summary>
        public enum SortedConstants
        {
            /// <summary>
            /// Sorted Ascending
            /// </summary>
            SortedAscending = 1,
            /// <summary>
            /// Sorted Descending
            /// </summary>
            SortedDescending = 2,
            /// <summary>
            /// Sorted None
            /// </summary>
            SortedNone = 0
        }

        /// <summary>
        /// ColumnWidthScaleConstants 
        /// </summary>
        public enum ColumnWidthScaleConstants
        {
            /// <summary>
            ///  Avg Char Width
            /// </summary>
            ColumnWidthScaleAvgCharWidth = 2,
            /// <summary>
            ///  Max Char Width
            /// </summary>
            ColumnWidthScaleMaxCharWidth = 3,
            /// <summary>
            /// Pixels
            /// </summary>
            ColumnWidthScalePixels = 1,
            /// <summary>
            /// Twips
            /// </summary>
            ColumnWidthScaleTwips = 0
        }


        /// <summary>
        /// Enum enumAction
        /// </summary>
        public enum ComboBoxHelperActionConstants
        {
            /// <summary>
            /// Clear
            /// </summary>
            ActionClear = 3,
            /// <summary>
            /// Clear Search Buffer
            /// </summary>
            ActionClearSearchBuffer = 6,
            /// <summary>
            /// Clone Col
            /// </summary>
            ActionCloneCol = 7,
            /// <summary>
            /// Delete Col
            /// </summary>
            ActionDeleteCol = 8,
            /// <summary>
            /// Delete Group
            /// </summary>
            ActionDeleteGroup = 10,
            /// <summary>
            /// Delete Row
            /// </summary>
            ActionDeleteRow = 4,
            /// <summary>
            /// Deselect All
            /// </summary>
            ActionDeselectAll = 2,
            /// <summary>
            /// Force Update
            /// </summary>
            ActionForceUpdate = 5,
            /// <summary>
            /// Insert Col
            /// </summary>
            ActionInsertCol = 7,
            /// <summary>
            /// Insert Group
            /// </summary>
            ActionInsertGroup = 11,
            /// <summary>
            /// Search
            /// </summary>
            ActionSearch = 0,
            /// <summary>
            /// Select All
            /// </summary>
            ActionSelectAll = 1,
            /// <summary>
            /// Virtual Refresh
            /// </summary>
            ActionVirtualRefresh = 9

        }
        #endregion
    }
}

