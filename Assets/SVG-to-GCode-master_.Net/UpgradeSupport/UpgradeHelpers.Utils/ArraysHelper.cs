using System;
using System.Collections.Generic;
using System.Reflection;

namespace UpgradeHelpers.Helpers
{
    /// <summary>
    /// The ArraysHelper contains functionality for some array operations, such as: 
    /// initialization, casting, and redimension.
    /// </summary>
    public class ArraysHelper
    {
        /// <summary>
        /// Initializes a one-dimensional array.
        /// </summary>
        /// <typeparam name="TE">The type of the elements of the array like 'string' for instance.</typeparam>
        /// <param name="length">The length of the new array.</param>
        /// <param name="bFixedLengthString">Optional bolean flag that indicates if the current array type is fixed length string (original vb6 code) - 
        /// its default value is false, because it will be generated in upgraded code when current array type used to be a fixed length string.</param>
        /// <param name="iFixedLengthStringSize">Optional integer value that indicates what is the fixed length string size, used in conjunction with previous (bFixedLengthString) parameter.</param>
        /// <returns>A new one-dimensional array with its values initialized to a default value.</returns>
        public static TE[] InitializeArray<TE>(int length, bool bFixedLengthString = false, int iFixedLengthStringSize = 0)
        {
            return InitializeArray<TE[]>(new int[] { length }, bFixedLengthString, iFixedLengthStringSize);
        }

        /// <summary>
        /// Initializes a one-dimensional array.
        /// </summary>
        /// <typeparam name="TE">The type of the elements of the array like 'string' for instance.</typeparam>
        /// <param name="length">The length of the new array.</param>
        /// <param name="lowerBound">The lower bound for the new array.</param>
        /// <param name="bFixedLengthString">Optional bolean flag that indicates if the current array type is fixed length string (original vb6 code) - 
        /// its default value is false, because it will be generated in upgraded code when current array type used to be a fixed length string.</param>
        /// <param name="iFixedLengthStringSize">Optional integer value that indicates what is the fixed length string size, used in conjunction with previous (bFixedLengthString) parameter.</param>
        /// <returns>A new one-dimensional array with its values initialized to a default value.</returns>
        public static TE[] InitializeArray<TE>(int length, int lowerBound, bool bFixedLengthString = false, int iFixedLengthStringSize = 0)
        {
            return InitializeArray<TE[]>(new int[] { length }, new int[] { lowerBound }, bFixedLengthString, iFixedLengthStringSize);
        }

        /// <summary>
        /// Initializes a one-dimensional array.
        /// </summary>
        /// <typeparam name="TE">The type of the elements of the array like 'string' for instance.</typeparam>
        /// <param name="length">The length of the new array.</param>
        /// <param name="constructorParams">The list of values to be sent to 
        /// the constructor of the item type of the array.</param>
        /// <param name="bFixedLengthString">Optional bolean flag that indicates if the current array type is fixed length string (original vb6 code) - 
        /// its default value is false, because it will be generated in upgraded code when current array type used to be a fixed length string.</param>
        /// <param name="iFixedLengthStringSize">Optional integer value that indicates what is the fixed length string size, used in conjunction with previous (bFixedLengthString) parameter.</param>
        /// <returns>A new one-dimensional array with its values initialized to a default value.</returns>
        public static TE[] InitializeArray<TE>(int length, object[] constructorParams, bool bFixedLengthString = false, int iFixedLengthStringSize = 0)
        {
            return InitializeArray<TE[]>(new int[] { length }, constructorParams, bFixedLengthString, iFixedLengthStringSize);
        }

        /// <summary>
        /// Initializes a one-dimensional array.
        /// </summary>
        /// <typeparam name="TE">The type of the elements of the array like 'string' for instance.</typeparam>
        /// <param name="length">The length of the new array.</param>
        /// <param name="lowerBound">The lower bound for the new array.</param>
        /// <param name="constructorParams">The list of values to be sent to 
        /// the constructor of the item type of the array.</param>
        /// <param name="bFixedLengthString">Optional bolean flag that indicates if the current array type is fixed length string (original vb6 code) - 
        /// its default value is false, because it will be generated in upgraded code when current array type used to be a fixed length string.</param>
        /// <param name="iFixedLengthStringSize">Optional integer value that indicates what is the fixed length string size, used in conjunction with previous (bFixedLengthString) parameter.</param>
        /// <returns>A new one-dimensional array with its values initialized to a default value.</returns>
        public static TE[] InitializeArray<TE>(int length, int lowerBound, object[] constructorParams, bool bFixedLengthString = false, int iFixedLengthStringSize = 0)
        {
            return InitializeArray<TE[]>(new int[] { length }, new int[] { lowerBound }, constructorParams, bFixedLengthString, iFixedLengthStringSize);
        }

        /// <summary>
        /// Initializes a one-dimensional array.
        /// </summary>
        /// <typeparam name="TE">The type of the elements of the array like 'string' for instance.</typeparam>
        /// <param name="length">The length of the new array.</param>
        /// <param name="initValue">An initial value to set to each element.</param>
        /// <param name="bFixedLengthString">Optional bolean flag that indicates if the current array type is fixed length string (original vb6 code) - 
        /// its default value is false, because it will be generated in upgraded code when current array type used to be a fixed length string.</param>
        /// <param name="iFixedLengthStringSize">Optional integer value that indicates what is the fixed length string size, used in conjunction with previous (bFixedLengthString) parameter.</param>
        /// <returns>A new one-dimensional array with its values initialized to initValue.</returns>
        public static TE[] InitializeArray<TE>(int length, object initValue, bool bFixedLengthString = false, int iFixedLengthStringSize = 0)
        {
            return InitializeArray<TE[]>(new int[] { length }, initValue, bFixedLengthString, iFixedLengthStringSize);
        }

        /// <summary>
        /// Initializes a one-dimensional array.
        /// </summary>
        /// <typeparam name="TE">The type of the elements of the array like 'string' for instance.</typeparam>
        /// <param name="length">The length of the new array.</param>
        /// <param name="lowerBound">The lower bound for the new array.</param>
        /// <param name="initValue">An initial value to set to each element.</param>
        /// <param name="bFixedLengthString">Optional bolean flag that indicates if the current array type is fixed length string (original vb6 code) - 
        /// its default value is false, because it will be generated in upgraded code when current array type used to be a fixed length string.</param>
        /// <param name="iFixedLengthStringSize">Optional integer value that indicates what is the fixed length string size, used in conjunction with previous (bFixedLengthString) parameter.</param>
        /// <returns>A new one-dimensional array with its values initialized to initValue.</returns>
        public static TE[] InitializeArray<TE>(int length, int lowerBound, object initValue, bool bFixedLengthString = false, int iFixedLengthStringSize = 0)
        {
            return InitializeArray<TE[]>(new int[] { length }, new int[] { lowerBound }, initValue, bFixedLengthString, iFixedLengthStringSize);
        }

        /// <summary>
        /// Initializes a multi-dimensional array.
        /// </summary>
        /// <typeparam name="TA">The type of the array including the dimensions in the form 'string[,,]'.</typeparam>
        /// <param name="lengths">The length of each dimension.</param>
        /// <param name="bFixedLengthString">Optional bolean flag that indicates if the current array type is fixed length string (original vb6 code) - 
        /// its default value is false, because it will be generated in upgraded code when current array type used to be a fixed length string.</param>
        /// <param name="iFixedLengthStringSize">Optional integer value that indicates what is the fixed length string size, used in conjunction with previous (bFixedLengthString) parameter.</param>
        /// <returns>A new multi-dimensional array with its values initialized to a default value.</returns>
        public static TA InitializeArray<TA>(int[] lengths, bool bFixedLengthString = false, int iFixedLengthStringSize = 0) where TA : class
        {
            return InitializeArray<TA>(lengths, new object[] { }, bFixedLengthString, iFixedLengthStringSize);
        }

        /// <summary>
        /// Initializes a multi-dimensional array.
        /// </summary>
        /// <typeparam name="TA">The type of the array including the dimensions in the form 'string[,,]'.</typeparam>
        /// <param name="lengths">The length of each dimension.</param>
        /// <param name="lowerBounds">The lower bounds to use for each dimension.</param>
        /// <param name="bFixedLengthString">Optional bolean flag that indicates if the current array type is fixed length string (original vb6 code) - 
        /// its default value is false, because it will be generated in upgraded code when current array type used to be a fixed length string.</param>
        /// <param name="iFixedLengthStringSize">Optional integer value that indicates what is the fixed length string size, used in conjunction with previous (bFixedLengthString) parameter.</param>
        /// <returns>A new multi-dimensional array with its values initialized to a default value.</returns>
        public static TA InitializeArray<TA>(int[] lengths, int[] lowerBounds, bool bFixedLengthString = false, int iFixedLengthStringSize = 0) where TA : class
        {
            return InitializeArray<TA>(lengths, lowerBounds, new object[] { }, bFixedLengthString, iFixedLengthStringSize);
        }

        /// <summary>
        /// Initializes a multi-dimensional array.
        /// </summary>
        /// <typeparam name="TA">The type of the array including the dimensions in the form 'string[,,]'.</typeparam>
        /// <param name="lengths">The length of each dimension.</param>
        /// <param name="constructorParams">The list of values to be sent to 
        /// the constructor of the item type of the array.</param>
        /// <param name="bFixedLengthString">Optional bolean flag that indicates if the current array type is fixed length string (original vb6 code) - 
        /// its default value is false, because it will be generated in upgraded code when current array type used to be a fixed length string.</param>
        /// <param name="iFixedLengthStringSize">Optional integer value that indicates what is the fixed length string size, used in conjunction with previous (bFixedLengthString) parameter.</param>
        /// <returns>A new multi-dimensional array with its values initialized to a default value.</returns>
        public static TA InitializeArray<TA>(int[] lengths, object[] constructorParams, bool bFixedLengthString = false, int iFixedLengthStringSize = 0) where TA : class
        {
            if ((lengths == null) || (constructorParams == null))
                throw new NullReferenceException("AIS-Exception. Either 'lengths' or 'constructorParams' parameter is null");

            return InitializeArray<TA>(lengths, new int[lengths.Length], constructorParams, bFixedLengthString, iFixedLengthStringSize);
        }

        /// <summary>
        /// Initializes a multi-dimensional array.
        /// </summary>
        /// <typeparam name="TA">The type of the array including the dimensions in the form 'string[,,]'.</typeparam>
        /// <param name="lengths">The length of each dimension.</param>
        /// <param name="lowerBounds">The lower bounds to use for each dimension.</param>
        /// <param name="constructorParams">The list of values to be sent to 
        /// the constructor of the item type of the array.</param>
        /// <param name="bFixedLengthString">Optional bolean flag that indicates if the current array type is fixed length string (original vb6 code) - 
        /// its default value is false, because it will be generated in upgraded code when current array type used to be a fixed length string.</param>
        /// <param name="iFixedLengthStringSize">Optional integer value that indicates what is the fixed length string size, used in conjunction with previous (bFixedLengthString) parameter.</param>
        /// <returns>A new multi-dimensional array with its values initialized to a default value.</returns>
        public static TA InitializeArray<TA>(int[] lengths, int[] lowerBounds, object[] constructorParams, bool bFixedLengthString = false, int iFixedLengthStringSize = 0) where TA : class
        {
            if ((lengths == null) || (lowerBounds == null) || (constructorParams == null))
                throw new NullReferenceException("AIS-Exception. Either 'lengths', 'lowerBounds' or 'constructorParams' parameter is null");

            Type arrayType = typeof(TA);
            if (!arrayType.IsArray)
                throw new Exception("AIS-Exception. Array type is expected as parameter");

            Type itemType = arrayType.GetElementType();
            if (itemType == null)
                throw new NullReferenceException("AIS-Exception. itemType for the array couldn't be resolved");

            InitialValueProvider valueProvider = new InitialValueProvider(itemType, constructorParams);

            //Only Primitive types and strings can be initialized with the same value, for other types a new instance 
            //will be used for each element
            if (itemType.IsPrimitive || itemType == typeof(string))
                return InternalInitializeArray(lengths, lowerBounds, itemType, valueProvider.GetInitialValue(bFixedLengthString, iFixedLengthStringSize), bFixedLengthString, iFixedLengthStringSize) as TA;
            return InternalInitializeArray(lengths, lowerBounds, itemType, valueProvider, bFixedLengthString, iFixedLengthStringSize) as TA;
        }

        /// <summary>
        /// Initializes a multi-dimensional array.
        /// </summary>
        /// <typeparam name="TA">The type of the array including the dimensions in the form 'string[,,]'.</typeparam>
        /// <param name="lengths">The length of each dimension.</param>
        /// <param name="initValue">The init value to use for each element in the array.</param>
        /// <param name="bFixedLengthString">Optional bolean flag that indicates if the current array type is fixed length string (original vb6 code) - 
        /// its default value is false, because it will be generated in upgraded code when current array type used to be a fixed length string.</param>
        /// <param name="iFixedLengthStringSize">Optional integer value that indicates what is the fixed length string size, used in conjunction with previous (bFixedLengthString) parameter.</param>
        /// <returns>A new multi-dimensional array with its values initialized with initValue.</returns>
        public static TA InitializeArray<TA>(int[] lengths, object initValue, bool bFixedLengthString = false, int iFixedLengthStringSize = 0) where TA : class
        {
            if (lengths == null)
                throw new NullReferenceException("AIS-Exception. 'lengths' parameter is null");

            return InitializeArray<TA>(lengths, new int[lengths.Length], initValue, bFixedLengthString, iFixedLengthStringSize);
        }

        /// <summary>
        /// Initializes a multi-dimensional array.
        /// </summary>
        /// <typeparam name="TA">The type of the array including the dimensions in the form 'string[,,]'.</typeparam>
        /// <param name="lengths">The length of each dimension.</param>
        /// <param name="lowerBounds">The lower bounds to use for each dimension.</param>
        /// <param name="initValue">The init value to use for each element in the array.</param>
        /// <param name="bFixedLengthString">Optional bolean flag that indicates if the current array type is fixed length string (original vb6 code) - 
        /// its default value is false, because it will be generated in upgraded code when current array type used to be a fixed length string.</param>
        /// <param name="iFixedLengthStringSize">Optional integer value that indicates what is the fixed length string size, used in conjunction with previous (bFixedLengthString) parameter.</param>
        /// <returns>A new multi-dimensional array with its values initialized with initValue.</returns>
        public static TA InitializeArray<TA>(int[] lengths, int[] lowerBounds, object initValue, bool bFixedLengthString = false, int iFixedLengthStringSize = 0) where TA : class
        {
            if ((lengths == null) || (lowerBounds == null))
                throw new NullReferenceException("AIS-Exception. Either 'lengths', 'lowerBounds' parameter is null");

            if (lengths.Length != lowerBounds.Length)
                throw new Exception("AIS-Exception. The length of 'lengths' and 'lowerBounds' parameters is different");

            Type arrayType = typeof(TA);
            if (!arrayType.IsArray)
                throw new Exception("AIS-Exception. Array type is expected as parameter");

            Type itemType = arrayType.GetElementType();
            if (itemType == null)
                throw new NullReferenceException("AIS-Exception. itemType for the array couldn't be resolved");

            return InternalInitializeArray(lengths, lowerBounds, itemType, initValue, bFixedLengthString, iFixedLengthStringSize) as TA;
        }

        /// <summary>
        /// Internal method to initialize a multi-dimensional array.
        /// </summary>
        /// <param name="lengths">The length of each dimension.</param>
        /// <param name="lowerBounds">The lower bounds to use for each dimension.</param>
        /// <param name="itemType">The type to create the array.</param>
        /// <param name="value">The init value to use for each element in the array.</param>
        /// <param name="bFixedLengthString">Optional bolean flag that indicates if the current array type is fixed length string (original vb6 code) - 
        /// its default value is false, because it will be generated in upgraded code when current array type used to be a fixed length string.</param>
        /// <param name="iFixedLengthStringSize">Optional integer value that indicates what is the fixed length string size, used in conjunction with previous (bFixedLengthString) parameter.</param>
        /// <returns>A new multi-dimensional array with its values initialized with initValue.</returns>
        private static Array InternalInitializeArray(int[] lengths, int[] lowerBounds, Type itemType, Object value, bool bFixedLengthString = false, int iFixedLengthStringSize = 0)
        {
#if PORTABLE            
            Array res = Array.CreateInstance(itemType, lengths);
#else
            //Creates the array
            Array res = Array.CreateInstance(itemType, lengths, lowerBounds);
#endif

            //Initialize each element
            int[] upperBounds = new int[lowerBounds.Length];
            for (int i = 0; i < res.Rank; i++)
                upperBounds[i] = res.GetUpperBound(i);

            int[] indexes = new int[lengths.Length];
            Array.Copy(lowerBounds, indexes, lowerBounds.Length);

            int pos = res.Rank - 1;
            indexes[pos]--;
            pos = CalculateIndexes(ref indexes, pos, lowerBounds, upperBounds);

            //Won't initialize anything if the default values are the expected
            if (pos >= 0)
            {
                object sampleValue = res.GetValue(indexes);
                Object initValue = (value is InitialValueProvider) ? ((InitialValueProvider)value).GetInitialValue(bFixedLengthString, iFixedLengthStringSize) : value;
                if (!(sampleValue == null && initValue == null) &&
                    (sampleValue == null || !sampleValue.Equals(initValue)))
                {
                    while (pos >= 0)
                    {
                        res.SetValue((value is InitialValueProvider) ? ((InitialValueProvider)value).GetInitialValue(bFixedLengthString, iFixedLengthStringSize) : value, indexes);
                        pos = CalculateIndexes(ref indexes, pos, lowerBounds, upperBounds);
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// Executes a RedimPreserve over an array.
        /// </summary>
        /// <typeparam name="TA">The type of the array including the dimensions, for instance 'string[,,,]'.</typeparam>
        /// <param name="arraySource">The source array.</param>
        /// <param name="lengths">The length of the new dimensions.</param>
        /// <param name="bFixedLengthString">Optional bolean flag that indicates if the current array type is fixed length string (original vb6 code) - 
        /// its default value is false, because it will be generated in upgraded code when current array type used to be a fixed length string.</param>
        /// <param name="iFixedLengthStringSize">Optional integer value that indicates what is the fixed length string size, used in conjunction with previous (bFixedLengthString) parameter.</param>
        /// <returns>The new array with the elements of the old one.</returns>
        public static TA RedimPreserve<TA>(TA arraySource, int[] lengths, bool bFixedLengthString = false, int iFixedLengthStringSize = 0) where TA : class
        {
            if (lengths == null)
                throw new NullReferenceException("AIS-Exception. 'lengths' parameter is null");

            return RedimPreserve(arraySource, lengths, new int[lengths.Length], bFixedLengthString, iFixedLengthStringSize);
        }

        /// <summary>
        /// Executes a RedimPreserve over an array.
        /// </summary>
        /// <typeparam name="TA">The type of the array including the dimensions, for instance 'string[,,,]'.</typeparam>
        /// <param name="arraySource">The source array.</param>
        /// <param name="lengths">The length of the new dimensions.</param>
        /// <param name="lowerBounds">The lower bound of the new dimensions.</param>
        /// <param name="bFixedLengthString">Optional bolean flag that indicates if the current array type is fixed length string (original vb6 code) - 
        /// its default value is false, because it will be generated in upgraded code when current array type used to be a fixed length string.</param>
        /// <param name="iFixedLengthStringSize">Optional integer value that indicates what is the fixed length string size, used in conjunction with previous (bFixedLengthString) parameter.</param>
        /// <returns>The new array with the elements of the old one.</returns>
        public static TA RedimPreserve<TA>(TA arraySource, int[] lengths, int[] lowerBounds, bool bFixedLengthString = false, int iFixedLengthStringSize = 0) where TA : class
        {
            Array res;

            Type arrayType;
            Type arrayElementType = null;
            if (typeof(TA).ToString() == "System.Array")
            {
                arrayType = null;
                Array array1 = arraySource as Array;
                if (array1 != null) arrayElementType = array1.GetValue(0).GetType();
            }
            else
            {
                arrayType = typeof(TA);
                arrayElementType = arrayType.GetElementType();
            }

            if (arraySource == null)
                return InitializeArray<TA>(lengths, lowerBounds, bFixedLengthString, iFixedLengthStringSize);

            if (typeof(TA).ToString() != "System.Array")
                RunRedimPreserveVerifications(arraySource, arrayType, lengths, lowerBounds);
            Array array = arraySource as Array;
            //There is something to copy
            if (array != null)
            {
#if PORTABLE
                res = Array.CreateInstance(arrayElementType, lengths);
#else
                res = Array.CreateInstance(arrayElementType, lengths, lowerBounds);
#endif
                InitialValueProvider valueProvider = new InitialValueProvider(arrayElementType, null);
                //Multiple dimensions
                if (array.Rank > 1)
                    FillsMultiDimensionalArray(array, res, valueProvider, bFixedLengthString, iFixedLengthStringSize);
                else
                    FillsOneDimensionArray(array, res, valueProvider, bFixedLengthString, iFixedLengthStringSize);
            }
            else
            {
                res = InitializeArray<TA>(lengths, lowerBounds, bFixedLengthString, iFixedLengthStringSize) as Array;
            }
            return res as TA;
        }

        /// <summary>
        /// Fills the one-dimension targetArray with either matching cell values from 
        /// sourceArray or with a initial value.
        /// </summary>
        /// <param name="sourceArray">The array object containing the values to copy.</param>
        /// <param name="targetArray">The new array where to copy the values.</param>
        /// <param name="valueProvider">a <c>InitialValueProvider</c> object used to get 
        /// the default values for the new cells.</param>
        /// <param name="bFixedLengthString">Optional bolean flag that indicates if the current array type is fixed length string (original vb6 code) - 
        /// its default value is false, because it will be generated in upgraded code when current array type used to be a fixed length string.</param>
        /// <param name="iFixedLengthStringSize">Optional integer value that indicates what is the fixed length string size, used in conjunction with previous (bFixedLengthString) parameter.</param>
        private static void FillsOneDimensionArray(Array sourceArray, Array targetArray, InitialValueProvider valueProvider, bool bFixedLengthString = false, int iFixedLengthStringSize = 0)
        {
            if (targetArray.Length > sourceArray.Length)
            {
                for (int i = sourceArray.Length; i < targetArray.Length; i++)
                    targetArray.SetValue(valueProvider.GetInitialValue(bFixedLengthString, iFixedLengthStringSize), i + targetArray.GetLowerBound(0));
            }
            Array.Copy(sourceArray, sourceArray.GetLowerBound(0), targetArray, sourceArray.GetLowerBound(0),
                    Math.Min(targetArray.GetLength(0), sourceArray.GetLength(0)));
        }

        /// <summary>
        /// Fills the n-dimension targetArray with either matching cell values from 
        /// sourceArray or with a initial value.
        /// </summary>
        /// <param name="sourceArray">The array object containing the values to copy.</param>
        /// <param name="targetArray">The new array where to copy the values.</param>
        /// <param name="valueProvider">a <c>InitialValueProvider</c> object used to get
        /// the default values for the new cells.</param>
        /// <param name="bFixedLengthString">Optional bolean flag that indicates if the current array type is fixed length string (original vb6 code) - 
        /// its default value is false, because it will be generated in upgraded code when current array type used to be a fixed length string.</param>
        /// <param name="iFixedLengthStringSize">Optional integer value that indicates what is the fixed length string size, used in conjunction with previous (bFixedLengthString) parameter.</param>
        private static void FillsMultiDimensionalArray(Array sourceArray, Array targetArray, InitialValueProvider valueProvider, bool bFixedLengthString = false, int iFixedLengthStringSize = 0)
        {
            int rowsToCopy = GetFirstDimensionsSize(sourceArray);
            int originalLastDimensionSize = GetLastDimensionSize(sourceArray);
            int newLastDimensionSize = GetLastDimensionSize(targetArray);
            int newCells = newLastDimensionSize - originalLastDimensionSize;
            int lowerBound = sourceArray.GetLowerBound(0);
            // creates a new array with same dimensions than the target array (but a smaller one version) to hold the 
            // default values to copy
            Array arrayLen = new int[targetArray.Rank];
            for (int i = 0; i < targetArray.Rank - 1; i++)
                arrayLen.SetValue(1, i);
            arrayLen.SetValue(targetArray.GetLength(targetArray.Rank - 1), targetArray.Rank - 1);
            Array defaultValues = Array.CreateInstance(targetArray.GetType().GetElementType(), (int[])arrayLen);

            for (int i = 0; i < rowsToCopy; i++)
            {
                //copies the values from source array to target aray
                Array.Copy(sourceArray, (i * originalLastDimensionSize) + lowerBound, targetArray, (i * newLastDimensionSize) + lowerBound,
                    Math.Min(originalLastDimensionSize, newLastDimensionSize));
                // fills the remaining cells with the default value
                if (newCells > 0)
                {
                    for (int k = 0; k < arrayLen.Length; k++)
                        arrayLen.SetValue(0, k); // initilizing the indixes to first array element (0 on all dimensions)
                    // sets up the default values array with new values (we delegate to the value provider the responsability to get either
                    // a new or an old instance.
                    for (int j = originalLastDimensionSize; j < newLastDimensionSize; j++)
                    {
                        arrayLen.SetValue(j - originalLastDimensionSize, arrayLen.Length - 1); // moves the index
                        defaultValues.SetValue(valueProvider.GetInitialValue(bFixedLengthString, iFixedLengthStringSize), (int[])arrayLen);
                    }
                    Array.Copy(defaultValues, 0, targetArray,
                               (i * newLastDimensionSize) + originalLastDimensionSize + lowerBound, newCells);
                }
            }
        }

        /// <summary>
        /// Maps a multidimensional array to an array of with the same structure, converting the internal data, even to expressions with different inner types.
        /// </summary>
        /// <typeparam name="TInput">The type of the original array elements.</typeparam>
        /// <typeparam name="TOutput">The type of the target array elements.</typeparam>
        /// <param array="originalStruct">The source array to convert.</param>
        /// <param converter="converter">The convertion function to change each of the elements.</param>
        /// <returns>A new array, resulting from mapping all the elements of the original array with the 'convert' function.</returns>
        public static object DeepConvertAll<TInput, TOutput>(object originalStruct, Converter<TInput, TOutput> converter)
        {
            Type originalType = originalStruct.GetType();
            if (originalType.Equals(typeof(TInput)))
                return converter((TInput)originalStruct);
            else if (!originalType.IsArray)
                throw new Exception("Invalid type received in DeepConvertAll");
            else
            {
                // Calculate dimensions depth
                int dimensions = -1;
                for (Type tempType = originalType; tempType.IsArray; tempType = tempType.GetElementType()) dimensions++;
                for (Type tempType = typeof(TInput); tempType.IsArray; tempType = tempType.GetElementType()) dimensions--;
                // Calculate targetType at this level
                Type targetType = typeof(TOutput);
                for (int i = 0; i < dimensions; i++)
                    targetType = targetType.MakeArrayType();
                // Apply conversion and return result
                Array array = ((Array)originalStruct);
                Array result = Array.CreateInstance(targetType, array.Length);
                for (int i = 0; i < array.Length; i++)
                {
                    result.SetValue(DeepConvertAll(array.GetValue(i), converter), i);
                }
                return result;
            }
        }

        /// <summary>
        /// Converts a char[], or a deeper array containing char[] in the bottom, to a string or a equivalent array containing strings in the bottom.
        /// </summary>
        /// <param array="originalStruct">The source array to convert.</param>
        /// <returns>The resulting string or array of strings.</returns>
        public static object CharArrayToString(object originalStruct)
        {
           return DeepConvertAll<char[], string>(originalStruct, x => new string(x));
        }

        /// <summary>
        /// Casts an array from one type to another.
        /// </summary>
        /// <typeparam name="TA">The type of the array including the dimensions in the form 'string[,,]'.</typeparam>
        /// <param name="srcArray">The source array to cast.</param>
        /// <returns>A new array with the correct new target type.</returns>
        public static TA CastArray<TA>(Array srcArray) where TA : class
        {
            Array tempResult;
            try
            {
                if (srcArray == null) return null;

                Type arrayType = typeof(TA);
                if (!arrayType.IsArray)
                    throw new Exception("AIS-Exception. Array type is expected as parameter");

                Type itemType = arrayType.GetElementType();
                if (itemType == null)
                    throw new NullReferenceException("AIS-Exception. itemType for the array couldn't be resolved");

                int[] lengths = new int[srcArray.Rank];
                int[] lowerBounds = new int[srcArray.Rank];
                int[] upperBounds = new int[srcArray.Rank];
                for (int i = 0; i < srcArray.Rank; i++)
                {
                    lengths[i] = srcArray.GetLength(i);
                    lowerBounds[i] = srcArray.GetLowerBound(i);
                    upperBounds[i] = srcArray.GetUpperBound(i);
                }

                //Creates the array
#if PORTABLE
                tempResult = Array.CreateInstance(itemType, lengths);
#else
                tempResult = Array.CreateInstance(itemType, lengths, lowerBounds);
#endif

                int[] indexes = new int[lengths.Length];
                Array.Copy(lowerBounds, indexes, lowerBounds.Length);

                int pos = tempResult.Rank - 1;
                indexes[pos]--;
                pos = CalculateIndexes(ref indexes, pos, lowerBounds, upperBounds);

                while (pos >= 0)
                {
#if PORTABLE
                    tempResult.SetValue(Convert.ChangeType(srcArray.GetValue(indexes), itemType,null), indexes);
#else
                    tempResult.SetValue(Convert.ChangeType(srcArray.GetValue(indexes), itemType), indexes);
#endif
                    pos = CalculateIndexes(ref indexes, pos, lowerBounds, upperBounds);
                }
            }
            catch (Exception e)
            {
                throw new Exception("AIS-Exception. Array casting is generating an exception: " + e.Message);
            }
            TA finalResult = tempResult as TA;
            if (finalResult == null)
            {
                throw new Exception("AIS-Exception. Cannot cast a " + srcArray.GetType() + " to a " + typeof(TA));
            }
            return finalResult;
        }

        /// <summary>
        /// Calculate the indexes of the next element to copy.
        /// </summary>
        /// <param name="indexes">The list of the indexes in the different dimensions for 
        /// the element to copy.</param>
        /// <param name="pos">The current position within the list of indexes.</param>
        /// <param name="lBounds">The list of lower bounds to use as limit.</param>
        /// <param name="uBounds">The list of upper bounds to use as limit.</param>
        /// <returns>The current position or -1 if the operation failed which means 
        /// there is no next element to copy.</returns>
        internal static int CalculateIndexes(ref int[] indexes, int pos, int[] lBounds, int[] uBounds)
        {
            indexes[pos]++;
            if (indexes[pos] > uBounds[pos])
            {
                indexes[pos] = lBounds[pos];
                pos--;
                if (pos >= 0)
                {
                    pos = CalculateIndexes(ref indexes, pos, lBounds, uBounds);
                    if (pos >= 0)
                        pos++;
                }
            }

            return pos;
        }

        /// <summary>
        /// Run some basic verifications on the parameters sent to RedimPreserve function.
        /// </summary>
        /// <param name="arrayPrototype">The source array to verify.</param>
        /// <param name="arrayType">The type of the source array.</param>
        /// <param name="lengths">The length of the dimensions.</param>
        /// <param name="lowerBounds">The lower bound of each dimension.</param>
        private static void RunRedimPreserveVerifications(object arrayPrototype, Type arrayType, int[] lengths, int[] lowerBounds)
        {
            if (!arrayType.IsArray)
                throw new Exception("AIS-Exception. Array type is expected as parameter");

            Type itemType = arrayType.GetElementType();
            if (itemType == null)
                throw new NullReferenceException("AIS-Exception. itemType for the array couldn't be resolved");

            if ((lengths == null) || (lowerBounds == null))
                throw new NullReferenceException("AIS-Exception. Either 'lengths' or 'lowerBounds' parameter is null");

            if (lengths.Length != lowerBounds.Length)
                throw new Exception("AIS-Exception. The length of 'lengths' and 'lowerBounds' parameters is different");

            if ((arrayPrototype != null) && (arrayType.GetArrayRank() != lengths.Length))
                throw new Exception("AIS-Exception. Can't change the number of dimensions of the current array");
            Array array = (Array)arrayPrototype;
            for (int i = 0; i < lengths.Length - 1; i++)
            {
                if (array != null && array.GetLength(i) != lengths[i])
                    throw new Exception("AIS-Exception.  Only last dimension can be modified.");
                if (array != null && array.GetLowerBound(i) != lowerBounds[i])
                    throw new Exception("AIS-Exception.  Only last dimension can be modified.");
            }
        }

        /// <summary>
        /// Gets the size for the first dimension for an array.
        /// </summary>
        /// <param name="array">The array to process.</param>
        /// <returns>The size of the first dimension of the array.</returns>
        private static int GetFirstDimensionsSize(Array array)
        {
            int result = 1;
            for (int i = 0; i < array.Rank - 1; i++)
                result = result * array.GetLength(i);
            return result;
        }

        /// <summary>
        /// Gets the size for the last dimension for an array.
        /// </summary>
        /// <param name="array">The array to process.</param>
        /// <returns>The size of the last dimension of the array.</returns>
        private static int GetLastDimensionSize(Array array)
        {
            return array.GetLength(array.Rank - 1);
        }

        /// <summary>
        /// The InitialValueProvider provides an initial value from several methods.
        /// Used for initialization of element types of arrays.
        /// </summary>
        private class InitialValueProvider
        {
            /// <summary>
            /// The Enumeration of the different kind of methods of initialization.
            /// </summary>
            private enum InitialValueMethod
            {
                String, Constructor, ValueType, CreateInstanceValueType, Null
#if ACTIVEX_COMPONENTSERVER
                , CsFactory 
#endif
            }
            /// <summary>
            /// The Type of array's elements.
            /// </summary>
            private readonly Type _elementType;
            /// <summary>
            /// The list of values to be sent to the constructor used in the method CreateInstanceValueType.
            /// </summary>
            private Object[] _constructorParams;
            /// <summary>
            /// Indicates if provider was already initialized.
            /// </summary>
            private bool _initialized;
            /// <summary>
            /// The InitializeMethod for the current provider.
            /// </summary>
            private InitialValueMethod _initializeMethod = InitialValueMethod.String;
            /// <summary>
            /// The Constructor method if constructor is gotten from elementType.
            /// </summary>
            private ConstructorInfo _constructor;
            /// <summary>
            /// Some Method used for initialization of the elementType, like "CreateInstance".
            /// </summary>
            private MethodInfo _method;

            /// <summary>
            /// Constructor for IniatialValueProvider.
            /// </summary>
            /// <param name="elementType">The type of the array's elements.</param>
            /// <param name="constructorParams">The list of values to be sent to the constructor of 
            /// the item type of the array.</param>
            public InitialValueProvider(Type elementType, Object[] constructorParams)
            {
                _elementType = elementType;
                _constructorParams = constructorParams ?? new object[0];

                _initialized = false;
            }

            /// <summary>
            /// Gets the value of initialization according to the InitialValueMethod of this provider.
            /// </summary>
            /// <param name="bFixedLengthString">Optional bolean flag that indicates if the current array type is fixed length string (original vb6 code) - 
            /// its default value is false, because it will be generated in upgraded code when current array type used to be a fixed length string.</param>
            /// <param name="iFixedLengthStringSize">Optional integer value that indicates what is the fixed length string size, used in conjunction with previous (bFixedLengthString) parameter.</param>
            /// <returns>The value of initialization.</returns>
            public Object GetInitialValue(bool bFixedLengthString = false, int iFixedLengthStringSize = 0)
            {
                Initialize();
                switch (_initializeMethod)
                {
#if ACTIVEX_COMPONENTSERVER
                    case InitialValueMethod.CsFactory:
                        try
                        {
                            Type factoryType = null;
                            foreach (Type possibleFactory in _elementType.Assembly.GetExportedTypes())
                            {
                                if (possibleFactory.BaseType.Name == "ComponentServerFactory")
                                {
                                    factoryType = possibleFactory;
                                }
                            }
                            if (factoryType != null)
                            {
                                MethodInfo mi = factoryType.GetMethod("CreateInstance", BindingFlags.Static | BindingFlags.Public);
                                MethodInfo miGeneric = mi.MakeGenericMethod(_elementType);
                                return miGeneric.Invoke(null, new object[] { });
                            }
                        }
                        catch (Exception ex)
                        {
                            return new Exception("Error while trying to get initial value for an array", ex);
                        }
                        break;
#endif
                    case InitialValueMethod.String:
                        return bFixedLengthString ? new string('\0', iFixedLengthStringSize) : string.Empty;
                    case InitialValueMethod.Constructor:
                        return _constructor.Invoke(_constructorParams);
                    case InitialValueMethod.ValueType:
                        return Activator.CreateInstance(_elementType);
                    case InitialValueMethod.CreateInstanceValueType:
                        return _method.Invoke(null, new object[] { });
                    case InitialValueMethod.Null:
                        return null;
                }
                return null;
            }

            /// <summary>
            /// Initialize this provider to be able to gets the intialization value.
            /// </summary>
            private void Initialize()
            {
                if (!_initialized)
                {
                    _initialized = true;
#if ACTIVEX_COMPONENTSERVER
                    if (_elementType.BaseType.Name == "ComponentClassHelper" 
                        || _elementType.BaseType.Name == "ComponentSingleUseClassHelper"
                        || _elementType.BaseType.Name == "GlbComponentSingleUseClassHelper")
                    {
                        _initializeMethod = InitialValueMethod.CsFactory;
                    }
                    else
#endif
                    if (_elementType == typeof(Object))
                    {
                        _initializeMethod = InitialValueMethod.Null;
                    }
                    else if (!(_elementType == typeof(String)))
                    {
                        //try for a constructor method
                        if (_constructorParams == null)
                            _constructorParams = new object[] { };
#if PORTABLE
                        Type[] typeArray = new Type[_constructorParams.Length];
                        for(int i=0;i<_constructorParams.Length;i++)
                        {
                            typeArray[i] = _constructorParams[i].GetType();
                        }
#else
                        Type[] typeArray = Type.GetTypeArray(_constructorParams);
#endif



                        if ((_constructor = _elementType.GetConstructor(typeArray)) == null)
                        {
                            if (_elementType.IsValueType && (_constructorParams == null || _constructorParams.Length == 0))
                                _initializeMethod = (_method = _elementType.GetMethod("CreateInstance")) == null ? InitialValueMethod.ValueType : InitialValueMethod.CreateInstanceValueType;
                        }
                        else
                            _initializeMethod = InitialValueMethod.Constructor;
                    }
                }
            }


        }

        /// <summary>
        /// Makes a deep copy of an array.
        /// </summary>
        /// <param name="objectToCopy">Array to copy.</param>
        /// <returns>A deep copy of the array.</returns>
        public static object DeepCopy(object objectToCopy)
        {
            using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, objectToCopy);
                memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
                return (object)binaryFormatter.Deserialize(memoryStream);
            }
        }

        /// <summary>
        /// Copies a string to a character array
        /// </summary>
        /// <param name="destinationArray"></param>
        /// <param name="sourceValue"></param>
        public static void CopyStringToCharArray(char[] destinationArray, string sourceValue)
        {
            if (sourceValue != null)
            {
                int destinationSize = destinationArray.Length;

                if (destinationSize > sourceValue.Length)
                {
                    sourceValue = sourceValue.PadRight(destinationSize);
                }

                char[] sourceArray = sourceValue.ToCharArray();

                Array.Copy(sourceArray, destinationArray, Math.Min(destinationSize, sourceArray.Length));
            }
            else
            {
                throw new Exception("You must supply a valid string value");
            }
        }
    }
}
