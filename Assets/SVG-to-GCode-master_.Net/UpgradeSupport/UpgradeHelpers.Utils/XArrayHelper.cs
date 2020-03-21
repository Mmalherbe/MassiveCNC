using System;
using System.Data;


namespace UpgradeHelpers.Helpers
{
    ///<summary>
    ///This simulates the XarrayDbObject funcionality based on DataTable class.
    ///</summary>
    ///<remarks>
    ///This class only supports two-dimensional arrays. Multi-dimensional arrays are not supported.
    ///</remarks>

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable")]
    public class XArrayHelper : DataTable
    {
        ///<summary>
        /// Stores the LowerBounds to handle indexes.
        ///</summary>
        private int[] DimensionLowerBounds = null;

        ///<summary>
        /// Stores the lengths to handle indexes.
        ///</summary>
        private int[] DimensionLengths = null;

        ///<summary>
        /// Constructor for the XArrayHelper.
        ///</summary>
        public XArrayHelper()
        {
            DimensionLowerBounds = null;
            DimensionLengths = null;
        }

      
      
        ///<summary>
        ///This function is a Factory to create Xarray instances. 
        ///</summary>
        ///<param name="Lengths">The length of each dimension.</param>
        ///<param name="LowerBounds">The lower bounds to use for each dimension.</param>
        ///<returns>A new XArrayHelper instance.</returns>
        public static XArrayHelper CreateInstanceXarray(int[] Lengths, int[] LowerBounds)
        {
            XArrayHelper xarr = new XArrayHelper();
            xarr.DimensionLowerBounds = LowerBounds;
            xarr.DimensionLengths = Lengths;

            for (int col = 0; col <= Lengths[1]; col++)
            {
                xarr.Columns.Add(new DataColumn());
            }

            for (int i = 0; i <= Lengths[0]; i++)
            {
                DataRow row = xarr.NewRow();
                xarr.Rows.Add(row);
            }
            return xarr;
        }

        ///<summary>
        ///This function redimensions a Xarray instance.
        ///</summary>
        ///<param name="lengths">The length of each dimension.</param>
        ///<param name="lowerBounds">The lower bounds to use for each dimension.</param>
        ///<returns>It returns a redimensioned instance of itself.</returns>
        ///<remarks></remarks>
        public XArrayHelper RedimXArray(int[] lengths, int[] lowerBounds)
        {

            DimensionLengths = lengths;
            DimensionLowerBounds = lowerBounds;

            if (Columns.Count == 0)
            {
                for (int colIndex = 0; colIndex <= lengths[1]; colIndex++)
                {
                    Columns.Add(new DataColumn());
                }
            }
            else if (Columns.Count < (lengths[1] + 1))
            {
                for (int colIndex = Columns.Count; colIndex <= lengths[1]; colIndex++)
                {
                    Columns.Add(new DataColumn());
                }
            }
            else if (Columns.Count > (lengths[1] + 1))
            {
                for (int colIndex = lengths[1] + 1; colIndex <= Columns.Count - 1; colIndex++)
                {
                    Columns.RemoveAt(colIndex);
                }
            }

            if (Rows.Count == 0)
            {
                for (int rowIndex = 0; rowIndex <= lengths[0]; rowIndex++)
                {
                    DataRow row = NewRow();
                    Rows.Add(row);
                }
            }
            else if (Rows.Count < (lengths[0] + 1))
            {
                for (int rowIndex = Rows.Count; rowIndex <= lengths[0]; rowIndex++)
                {
                    DataRow row = NewRow();
                    Rows.Add(row);
                }
            }
            else if (Rows.Count > (lengths[0] + 1))
            {
                for (int rowIndex = lengths[0] + 1; rowIndex <= Rows.Count - 1; rowIndex++)
                {
                    Rows.RemoveAt(rowIndex);
                }
            }
            return this;

        }

        ///<summary>
        ///Gets the upper bound of the specified dimension.
        ///</summary>
        ///<param name="dimension">A zero-based dimension whose upper bound needs to be determined.</param>
        ///<returns>The upper bound for the specificed dimension.</returns>
        public int GetUpperBound(int dimension)
        {
            return DimensionLengths[dimension];
        }

        ///<summary>
        ///Gets the Lower bound of the specified dimension.
        ///</summary>
        ///<param name="dimension">A zero-based dimension whose lower bound needs to be determined.</param>
        ///<returns>The lower bound for the specificed dimension.</returns>
        public int GetLowerBound(int dimension)
        {
            return DimensionLowerBounds[dimension];
        }

        ///<summary>
        ///Gets the number of elements in the specified dimension.
        ///</summary>
        ///<param name="dimension">A zero-based dimension whose length needs to be determined.</param>
        ///<returns>The length of elements of the specified dimension.</returns>
        public int GetLength(int dimension)
        {
            return DimensionLengths[dimension];
        }

        ///<summary>
        ///Returns the element at the specified row and column.
        ///</summary>
        ///<param name="row">Row index where the element is located.</param>
        ///<param name="column">Column index where the element is located.</param>
        ///<value>Value for the specified element.</value>
        ///<returns>The element at the specified index.</returns>
        public Object this[int row, int column]
        {
            get
            {
                return Rows[row - DimensionLowerBounds[0]][column - DimensionLowerBounds[1]];
            }
            set
            {
                Rows[row - DimensionLowerBounds[0]][column - DimensionLowerBounds[1]] = value;
            }
        }

        ///<summary>
        ///Gets the value at the specified position.
        ///</summary>
        ///<param name="row">Index row where the element is located.</param>
        ///<param name="column">Index column where the element is located.</param>
        ///<returns>The value at the specified position.</returns>
        public Object GetValue(int row, int column)
        {
            return Rows[row - DimensionLowerBounds[0]][column - DimensionLowerBounds[1]];
        }

        ///<summary>
        ///Sets a value to the element at the specified position.
        ///</summary>
        ///<param name="value">The new value for the specified element.</param>
        ///<param name="row">Index row where the element is located.</param>
        ///<param name="column">Index column where the element is located.</param>
        public void SetValue(Object value, int row, int column)
        {
            Rows[row - DimensionLowerBounds[0]][column - DimensionLowerBounds[1]] = value;
        }

        ///<summary>
        ///Clears a range of elements in the XArrayHelper.
        ///</summary>
        ///<param name="arr">XArrayHelper whose elements need to be cleared.</param>
        ///<param name="index">The starting index of the range of elements.</param>
        ///<param name="length">The number of elements to be cleared.</param>
        public static void Clear(XArrayHelper arr, int index, int length)
        {

            int realIndexi = arr.GetLowerBound(0);
            int realIndexj = arr.GetLowerBound(1);

            index = index - arr.GetLowerBound(0);

            while (index > 0)
            {
                if (index > arr.GetUpperBound(1))
                {
                    realIndexi = realIndexi + 1;
                    index = index - arr.GetLength(1);
                }
                else
                {
                    realIndexj = realIndexj + index;
                    index = 0;
                }
            }

            for (int j = realIndexj; j <= arr.GetUpperBound(1); j++)
            {
                if (length < 0) return;
                arr[realIndexi, j] = null;
                length = length - 1;
            }

            realIndexi = realIndexi + 1;

            for (int i = realIndexi; i <= arr.GetUpperBound(0); i++)
            {
                for (int j = arr.GetLowerBound(1); j <= arr.GetUpperBound(1); j++)
                {
                    if (length < 1) return;
                    arr[i, j] = null;
                    length = length - 1;
                }
            }
        }


        ///<summary>
        ///Creates a cleared a XArrayHelper.
        ///</summary>
        ///<param name="arr">XArrayHelper whose elements need to be cleared.</param>
        public void Clear(ref XArrayHelper arr)
        {
            int[] length = new int[] { 1, 0 };
            int[] lowerB = new int[] { arr.DimensionLowerBounds[0], arr.DimensionLowerBounds[1] };
            Clear();
            arr.RedimXArray(length, lowerB);
        }

        ///<summary>
        ///Adds a new row to the current instance of XArrayHelper.
        ///</summary>
        public void AppendRows()
        {
            int[] length = new int[] { DimensionLengths[0] + 1, DimensionLengths[1] };
            int[] lowerB = new int[] { DimensionLowerBounds[0], DimensionLowerBounds[1] };
            RedimXArray(length, lowerB);
        }

        ///<summary>
        ///Adds a new row to the current instance of XArrayHelper and sets a value to the specified
        ///row and column.
        ///</summary>
        ///<param name="value">The value to be set the specified position.</param>
        ///<param name="row">The row in the XArrayHelper where to be set the value.</param>
        ///<param name="column">The column in the XArrayHelper where to be set the value.</param>
        public void AppendRows(Object value, int row, int column)
        {
            int[] length = new int[] { DimensionLengths[0] + 1, DimensionLengths[1] };
            int[] lowerB = new int[] { DimensionLowerBounds[0], DimensionLowerBounds[1] };
            RedimXArray(length, lowerB);

            Rows[row - DimensionLowerBounds[0]][column - DimensionLowerBounds[1]] = value;
        }

        ///<summary>
        ///Deletes a row in the specified position and redimensions the XArrayHelper.
        ///</summary>
        ///<param name="row">The row in the XArrayHelper to be deleted.</param>
        public void DeleteRows(int row)
        {
            Rows[row - DimensionLowerBounds[0]].Delete();
            int[] length = new int[] { DimensionLengths[0] - 1, DimensionLengths[1] };
            int[] lowerB = new int[] { DimensionLowerBounds[0], DimensionLowerBounds[1] };
            RedimXArray(length, lowerB);
        }

        ///<summary>
        ///Creates a XArrayHelper and copies the values from an object array.
        ///</summary>
        ///<param name="array">The source array to be copied.</param>
        public void LoadRows(Object[,] array)
        {
            RedimXArray(new int[] { array.GetUpperBound(0), array.GetUpperBound(1) }, new int[] { array.GetLowerBound(0), array.GetLowerBound(1) });
            for (int row = array.GetLowerBound(0); row <= array.GetUpperBound(0); row++)
            {
                for (int col = array.GetLowerBound(1); col <= array.GetUpperBound(1); col++)
                {
                    SetValue(array[row, col], row, col);
                }
            }
        }

        ///<summary>
        ///Creates a XArrayHelper and copies the values from a XArrayHelper.
        ///</summary>
        ///<param name="table">The source XArrayHelper to be copied.</param>
        public void LoadRows(XArrayHelper table)
        {
            RedimXArray(new int[] { table.GetUpperBound(0), table.GetUpperBound(1) }, new int[] { table.GetLowerBound(0), table.GetLowerBound(1) });
            for (int row = table.GetLowerBound(0); row <= table.GetUpperBound(0); row++)
            {
                for (int col = table.GetLowerBound(1); col <= table.GetUpperBound(1); col++)
                {
                    SetValue(table.GetValue(row, col), row, col);
                }
            }
        }

        ///<summary>
        ///Finds a value into a XArrayHelper.
        ///</summary>
        ///<param name="value">The value to be found.</param>
        ///<returns>True if the value is found into the XArrayHelper.</returns>
        public Object Find(Object value)
        {
            Boolean result = false;
            for (int row = GetLowerBound(0); row <= GetUpperBound(0); row++)
            {
                for (int col = GetLowerBound(1); col <= GetUpperBound(1); col++)
                {
                    if (GetValue(row, col) == value)
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        ///<summary>
        ///Finds a value into a XArrayHelper from a specified position.
        ///</summary>
        ///<param name="value">The value to be found.</param>
        ///<param name="lowerBound">The lowerbound where to start searching.</param>
        ///<param name="upperBound">The upperbound where to finish searching.</param>
        ///<returns>The index where the values is found or -1 if it is not found.</returns>
        public Object Find(Object value, int lowerBound, int upperBound)
        {
            long index = -1;
            for (int row = lowerBound; row <= GetUpperBound(0); row++)
            {
                for (int col = upperBound; col <= GetUpperBound(1); col++)
                {
                    if (GetValue(row, col) == value)
                    {
                        index = row;
                        break;
                    }
                }
            }
            return index;
        }
    }
}

