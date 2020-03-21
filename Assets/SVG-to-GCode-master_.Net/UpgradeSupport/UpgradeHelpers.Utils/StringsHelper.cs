using Microsoft.VisualBasic;
using System;
using System.Text;
namespace UpgradeHelpers.Helpers
{

    /// <summary>
    /// The StringsHelper is an utility that provides funcationality related to string operations.
    /// </summary>
    public class StringsHelper
    {

        /// <summary>
        /// VbStrConv Enum used for the runtime implementation of StringsHelper.StrConv.
        /// </summary>
        [Flags]
        public enum VbStrConvEnum
        {
            /// <summary>
            /// From Unicode
            /// </summary>
            VbFromUnicode = 128,
            /// <summary>
            /// Hiragana
            /// </summary>
            VbHiragana = 32,
            /// <summary>
            /// Katakana
            /// </summary>
            VbKatakana = 16,
            /// <summary>
            /// Lower case
            /// </summary>
            VbLowerCase = 2,
            /// <summary>
            /// Narrow
            /// </summary>
            VbNarrow = 8,
            /// <summary>
            /// ProperCase
            /// </summary>
            VbProperCase = 3,
            /// <summary>
            /// Unicode
            /// </summary>
            VbUnicode = 64,
            /// <summary>
            /// Upper case
            /// </summary>
            VbUpperCase = 1,
            /// <summary>
            /// Wide char
            /// </summary>
            VbWide = 4
        }
        /*
                /// <summary>Indicates the first day of the week to use when calling date-related functions.</summary>
                public enum FirstDayOfWeek
                {
                    /// <summary>The first day of the week as specified in your system settings This member is equivalent to the Visual Basic constant vbUseSystemDayOfWeek.</summary>
                    System,
                    /// <summary>Sunday (default) This member is equivalent to the Visual Basic constant vbSunday.</summary>
                    Sunday,
                    /// <summary>Monday This member is equivalent to the Visual Basic constant vbMonday.</summary>
                    Monday,
                    /// <summary>Tuesday This member is equivalent to the Visual Basic constant vbTuesday.</summary>
                    Tuesday,
                    /// <summary>Wednesday This member is equivalent to the Visual Basic constant vbWednesday.</summary>
                    Wednesday,
                    /// <summary>Thursday This member is equivalent to the Visual Basic constant vbThursday.</summary>
                    Thursday,
                    /// <summary>Friday This member is equivalent to the Visual Basic constant vbFriday.</summary>
                    Friday,
                    /// <summary>Saturday This member is equivalent to the Visual Basic constant vbSaturday.</summary>
                    Saturday
                }

                /// <summary>Indicates the first week of the year to use when calling date-related functions.</summary>
                public enum FirstWeekOfYear
                {
                    /// <summary>The weekspecified in your system settings as the first week of the year. This member is equivalent to the Visual Basic constant vbUseSystem.</summary>
                    System,
                    /// <summary>The week in which January 1 occurs (default). This member is equivalent to the Visual Basic constant vbFirstJan1.</summary>
                    Jan1,
                    /// <summary>The first week that has at least four days in the new year. This member is equivalent to the Visual Basic constant vbFirstFourDays.</summary>
                    FirstFourDays,
                    /// <summary>The first full week of the year. This member is equivalent to the Visual Basic constant vbFirstFullWeek.</summary>
                    FirstFullWeek
                }
        */
#if !PORTABLE
        /// <summary>
        /// Runtime implementation for VBA.Strings.StrConv
        /// note:
        ///     If Conversion == vbUnicode then the string returned will be encoded using
        ///     System.Text.Encoding.Default, otherwise the encoding System.Text.Encoding.Unicode
        ///     will be used.
        /// </summary>
        /// <param name="str">Byte array representing an string.</param>
        /// <param name="conversion">The type of the conversion to execute.</param>
        /// <returns>The converted string.</returns>
        public static string StrConv(string str, VbStrConvEnum conversion)
        {
            //0 is to indicate to use the default ANSI encode of the machine
            return StrConv(str, conversion, 0);
        }


        /// <summary>
        /// Runtime implementation for VBA.Strings.StrConv
        /// note:
        ///     If Conversion == vbUnicode then the string returned will be encoded using
        ///     System.Text.Encoding.Default, otherwise the encoding System.Text.Encoding.Unicode
        ///     will be used.
        /// </summary>
        /// <param name="str">Byte array representing an string.</param>
        /// <param name="conversion">The type of the conversion to execute.</param>
        /// <param name="localeId">The LocaleID to use in the conversion.</param>
        /// <returns>The converted string.</returns>
        public static string StrConv(string str, VbStrConvEnum conversion, int localeId)
        {

            string res;

            switch (conversion)
            {
                //Please do not modify the implementations for vbFromUnicode and vbUnicode because they have been
                //already proveed with several systems
                case VbStrConvEnum.VbFromUnicode:
                    IntPtr strPtr = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi(str);
                    res = System.Runtime.InteropServices.Marshal.PtrToStringUni(strPtr);
                    System.Runtime.InteropServices.Marshal.FreeHGlobal(strPtr);
                    break;
                case VbStrConvEnum.VbUnicode:
                    //It is also possible to use the specific encoding:
                    //     - Encoding.GetEncoding("Windows-1252") or
                    //     - Encoding.GetEncoding(1252)
                    byte[] b = Encoding.Convert(Encoding.Default, Encoding.Unicode, Encoding.Unicode.GetBytes(str));
                    res = Encoding.Unicode.GetString(b);
                    break;
                default:
                    res = Microsoft.VisualBasic.Strings.StrConv(str, (Microsoft.VisualBasic.VbStrConv)((int)conversion), localeId);
                    break;
            }

            return res;
        }

        /// <summary>
        /// Runtime implementation for VBA.Strings.StrConv
        /// note:
        ///     If Conversion == vbUnicode then the string returned will be encoded using
        ///     System.Runtime.InteropServices.Marshal.StringToHGlobalUni.
        /// </summary>
        /// <param name="str">Byte array representing an string.</param>
        /// <param name="conversion">The type of the conversion to execute.</param>
        /// <returns>The converted string.</returns>
        public static string StrConv2(string str, VbStrConvEnum conversion)
        {
            //0 is to indicate to use the default ANSI encode of the machine
            return StrConv2(str, conversion, 0);
        }


        /// <summary>
        /// Runtime implementation for VBA.Strings.StrConv VERSION 2
        /// note:
        ///     If Conversion == vbUnicode then the string returned will be encoded using
        ///     System.Runtime.InteropServices.Marshal.StringToHGlobalUni.
        /// </summary>
        /// <param name="str">Byte array representing an string.</param>
        /// <param name="conversion">The type of the conversion to execute.</param>
        /// <param name="localeId">The LocaleID to use in the conversion.</param>
        /// <returns>The converted string.</returns>
        public static string StrConv2(string str, VbStrConvEnum conversion, int localeId)
        {

            string res;
            IntPtr strPtr;

            switch (conversion)
            {
                //Please do not modify the implementations for vbFromUnicode and vbUnicode because they have been
                //already proveed with several systems (C995_045)
                case VbStrConvEnum.VbFromUnicode:
                    strPtr = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi(str);
                    res = System.Runtime.InteropServices.Marshal.PtrToStringUni(strPtr);
                    System.Runtime.InteropServices.Marshal.FreeHGlobal(strPtr);
                    break;
                case VbStrConvEnum.VbUnicode:
                    strPtr = System.Runtime.InteropServices.Marshal.StringToHGlobalUni(str);
                    res = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(strPtr, str.Length * 2);
                    System.Runtime.InteropServices.Marshal.FreeHGlobal(strPtr);
                    break;
                default:
                    res = Microsoft.VisualBasic.Strings.StrConv(str, (Microsoft.VisualBasic.VbStrConv)((int)conversion), localeId);
                    break;
            }
            return res;

        }
#endif

        /// <summary>
        /// Converts a byte array to a string.
        /// </summary>
        /// <param name="array">Byte array to be converted.</param>
        /// <returns>The string converted in Unicode encoding.</returns>
        public static string ByteArrayToString(byte[] array)
        {
            if (array != null)
            {
                byte[] sArray;
                if (array.Length % 2 == 0)
                    sArray = array;
                else
                {
                    sArray = new byte[array.Length + 1];
                    Array.Copy(array, sArray, array.Length);
                }

                return Encoding.Unicode.GetString(sArray, 0, sArray.Length);
            }
            return string.Empty;
        }

        /// <summary>
        /// Obtains exactly the indicated amount of characters from the start of the string
        /// (and add spaces to the right if needed to complete the amount of characters).
        /// </summary>
        /// <param name="str">String from which characters are to be extracted.</param>
        /// <param name="nChars">The amount of characters to get.</param>
        /// <returns>A string of <i>nChars</i> characters of length from <i>str</i>.</returns>
        public static string GetFixedLengthString(string str, int nChars)
        {
            string result = Left(str, nChars);

            if (result != null && result.Length < nChars)
            {
                result = result.PadRight(nChars);
            }

            return result;
        }

        /// <summary>
        /// Obtains the indicated amount of characters from the start of the string.
        /// </summary>
        /// <param name="str">String from which characters are to be extracted.</param>
        /// <param name="nChars">The amount of characters to get.</param>
        /// <returns>A string with the first <i>nChars</i> characters from <i>str</i>.</returns>
        public static string Left(string str, int nChars)
        {
            if (str == null) return null;
            return str.Substring(0, Math.Min(nChars, str.Length));
        }

        /// <summary>
        /// Obtains the indicated amount of characters from the end of the string.
        /// </summary>
        /// <param name="str">String from which characters are to be extracted.</param>
        /// <param name="nChars">The amount of characters to get.</param>
        /// <returns>A string with the last <i>nChars</i> characters from <i>str</i>.</returns>
        public static string Right(string str, int nChars)
        {
            if (str == null) return null;
            return str.Substring(Math.Max(str.Length - nChars, 0));
        }

        /// <summary>
        /// Obtains the remaining characters starting from a specified point in the string <i>str</i>.
        /// </summary>
        /// <param name="str">String from which characters are to be extracted.</param>
        /// <param name="start">The starting point.</param>
        /// <returns>A string with the characters from the <i>str</i> starting from the specified position.</returns>
        public static string Mid(string str, int start)
        {
            if (str == null) return null;
            return (start <= str.Length) ? str.Substring(start) : "";
        }

        /// <summary>
        /// Obtains the indicated amount of characters starting from a specified point in the string <i>str</i>.
        /// </summary>
        /// <param name="str">String from which characters are to be extracted.</param>
        /// <param name="start">The starting point.</param>
        /// <param name="nChars">The amount of characters to get.</param>
        /// <returns>A string with the first <i>nChars</i> characters from the <i>str</i> starting from the specified position.</returns>
        public static string Mid(string str, int start, int nChars)
        {        
            if (str == null) return null;
            
            if (str.Length <= start) return string.Empty;
            if (str.Length - start < nChars) nChars = str.Length - start;
            if (nChars < 0) return string.Empty;

            return str.Substring(start, nChars);

        }

        /// <summary>
        /// Replaces a portion of a string with other string. Provides the same functionality than
        /// MidAssignment for Visual Basic 6.
        /// </summary>
        /// <param name="str">The string to be changed.</param>
        /// <param name="start">The index into the string where to start the changing.</param>
        /// <param name="length">The length of the portion of string to change.</param>
        /// <param name="val">The new string to change into the other one.</param>
        /// <returns>The changed string with the new portion.</returns>
        public static string MidAssignment(string str, int start, int length, string val)
        {
            int minTmp = Math.Min(length, Math.Min(val.Length, str.Length - (start - 1)));

            return str.Substring(0, start - 1) + val.Substring(0, minTmp) + str.Substring(start - 1 + minTmp);
        }

        /// <summary>
        /// Replaces a portion of a string with other string. Provides the same functionality than
        /// MidAssignment for Visual Basic 6.
        /// </summary>
        /// <param name="str">The string to be changed.</param>
        /// <param name="start">The index into the string where to start the replace.</param>
        /// <param name="val">The new string to change into the other one.</param>
        /// <returns>The changed string with the new portion.</returns>
        public static string MidAssignment(string str, int start, string val)
        {
            return MidAssignment(str, start, int.MaxValue, val);
        }

        /// <summary>
        /// Matches a string value with a regular expression pattern.
        /// </summary>
        /// <param name="value">The string to be matched.</param>
        /// <param name="pattern">The regular expression used to match the string.</param>
        /// <returns>True if the pattern matches into the string.</returns>
        public static bool Like(string value, string pattern)
        {
            System.Diagnostics.Debug.WriteLine("WARNING: Using VB6 'Like' operator pattern affects performace. Pattern used " + pattern);

            // convert VB6 regular expressions to C# regular expressions
            string transformedPattern = string.Empty;
            string[] splittedPattern = System.Text.RegularExpressions.Regex.Split(pattern, @"(\[[^]]*\])");
            for (int i = 0; i < splittedPattern.Length; i++)
            {
                if (i % 2 == 0)
                {
                    // replace '#' , '?' and '*' that are outside the square brackets
                    transformedPattern += splittedPattern[i].Replace("#", "\\d").Replace('?', '.').Replace("*", ".*");
                }
                else
                {
                    // leave intact the insides of the square brackets
                    transformedPattern += splittedPattern[i];
                }
            }
            transformedPattern = "^" + transformedPattern + "$";

            return System.Text.RegularExpressions.Regex.IsMatch(value, transformedPattern);
        }

        /// <summary>
        /// Returns the String toFormat formatted with the given mask.
        /// </summary>
        /// <param name="_toFormat">The String object to format.</param>
        /// <param name="_mask">The format to apply.</param>
        /// <param name="dayOfWeek">A value chosen from the FirstDayOfWeek enumeration that specifies the first day of the week.</param>
        /// <param name="weekOfYear">A value chosen from the FirstWeekOfYear enumeration that specifies the first week of the year.</param>
        /// <returns>Empty String if toFormat is null or empty, othewise the formatted string.</returns>
        public static String Format(object _toFormat, object _mask, FirstDayOfWeek dayOfWeek, FirstWeekOfYear weekOfYear)
        {
            //if (_toFormat == null) throw new ArgumentNullException("_toFormat");
            //if (_mask == null) throw new ArgumentNullException("_mask");
            string toFormat = Convert.ToString(_toFormat);
            string mask = PreprocessMask(_toFormat, _mask);
            DateTime dt;
            decimal _decimal;
            double _double;
            float _float;
            int _int;
            byte _byte;
            if (String.IsNullOrEmpty(toFormat))
            {
                return String.Empty;
            }
            else if (_toFormat is decimal)
            {
                return ((decimal)_toFormat).ToString(mask);
            }
            else if (Decimal.TryParse(toFormat, out _decimal))
            {
                return _decimal.ToString(mask);
            }
            else if (_toFormat is double)
            {
                return ((double)_toFormat).ToString(mask);
            }
            else if (Double.TryParse(toFormat, out _double))
            {
                return _double.ToString(mask);
            }
            else if (_toFormat is float)
            {
                return ((float)_toFormat).ToString(mask);
            }
            else if (float.TryParse(toFormat, out _float))
            {
                return _float.ToString(mask);
            }
            else if (_toFormat is int)
            {
                return ((int)_toFormat).ToString(mask);
            }
            else if (Int32.TryParse(toFormat, out _int))
            {
                return _int.ToString(mask);
            }
            else if (_toFormat is byte)
            {
                return ((byte)_toFormat).ToString(mask);
            }
            else if (Byte.TryParse(toFormat, out _byte))
            {
                return _byte.ToString(mask);
            }
            else if (_toFormat is DateTime)
            {
                return ((DateTime)_toFormat).ToString(mask);
            }
            else if (DateTime.TryParse(toFormat, out dt))
            {
                if (mask == "0")
                    return dt.ToOADate().ToString();
                else
                    return dt.ToString(mask);
            }

#if WPF || WINFORMS      
            //UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("VB.Strings.Format");
#endif
            return toFormat;
        }

        /// <summary>
        /// Returns the String toFormat formatted with the given mask.
        /// </summary>
        /// <param name="toFormat">The String object to format.</param>
        /// <param name="mask">The format to apply.</param>
        /// <param name="dayOfWeek">A value chosen from the FirstDayOfWeek enumeration that specifies the first day of the week.</param>
        /// <returns>Empty String if toFormat is null or empty, othewise the formatted string.</returns>
        public static String Format(object toFormat, object mask, FirstDayOfWeek dayOfWeek)
        {
            return Format(toFormat, mask, dayOfWeek, FirstWeekOfYear.Jan1);
        }

        /// <summary>
        /// Returns the String toFormat formatted with the given mask.
        /// </summary>
        /// <param name="toFormat">The String object to format.</param>
        /// <param name="mask">The format to apply.</param>
        /// <param name="weekOfYear">A value chosen from the FirstWeekOfYear enumeration that specifies the first week of the year.</param>
        /// <returns>Empty String if toFormat is null or empty, othewise the formatted string.</returns>
        public static String Format(object toFormat, object mask, FirstWeekOfYear weekOfYear)
        {
            return Format(toFormat, mask, FirstDayOfWeek.Sunday, weekOfYear);
        }

        /// <summary>
        /// Returns the String toFormat formatted with the given mask.
        /// </summary>
        /// <param name="toFormat">The String object to format.</param>
        /// <param name="mask">The format to apply.</param>
        /// <returns>Empty String if toFormat is null or empty, otherwise the formatted string.</returns>
        public static String Format(object toFormat, object mask)
        {
            return Format(toFormat, mask, FirstDayOfWeek.Sunday, FirstWeekOfYear.Jan1);
        }

        /// <summary>
        /// Returns the String toFormat formatted with an empty mask.
        /// </summary>
        /// <param name="toFormat">The String object to format.</param>
        /// <returns>Empty String if toFormat is null or empty, othewise the formatted string.</returns>
        public static String Format(object toFormat)
        {
            return Format(toFormat, String.Empty, FirstDayOfWeek.Sunday, FirstWeekOfYear.Jan1);
        }

        private static String PreprocessMask(object _toFormat, object _mask)
        {
            string toFormat = Convert.ToString(_toFormat);
            string mask = Convert.ToString(_mask);
            DateTime dt;
            if (_toFormat is double)
            {
                mask = mask.Replace("Standard", "N");
            }
            else if (mask != null && mask.ToLower() == "currency")
            {
                mask = "C";
            }
            else if (mask != null && mask.ToLower() == "percent")
            {
                mask = "P2";
            }
            else if (_toFormat is DateTime || DateTime.TryParse(toFormat, out dt))
            {
                if (string.Compare(mask, "general date", true) == 0)
                {
                    mask = "d";
                }
                else
                {
                    mask = mask.Replace("AM", "tt");
                    mask = mask.Replace("PM", "tt");
                    mask = mask.Replace("am", "tt");
                    mask = mask.Replace("pm", "tt");
                    mask = mask.Replace('m', 'M');
                    mask = mask.Replace("nn", "mm");
                    mask = mask.Replace("n", "m");
                    mask = mask.Replace("Y", "y");
                }
            }
            return mask;
        }

        /// <summary>
        /// Gets a double value represented by the given String value. If value contains an 
        /// invalid number then a Double.NaN is returned.  
        /// This method is used to do safe castings between strings and numeric values.
        /// It is required for comparisons between strings and primitive types which were allowed by VB6 but are invalid in .NET.
        /// </summary>
        /// <param name="value">String containing the double value to convert.</param>
        /// <returns>A double value.</returns>
        public static double ToDoubleSafe(String value)
        {
            double dValue;
            return Double.TryParse(value, System.Globalization.NumberStyles.Any, null, out dValue) ? dValue : Double.NaN;
        }

    }
}
