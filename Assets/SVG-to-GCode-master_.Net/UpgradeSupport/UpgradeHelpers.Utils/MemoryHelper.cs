using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace UpgradeHelpers.Helpers
{
    /// <summary>
    /// The MemoryHelper class has utility methods that handle memory-related issues, such as releasing and cleaning up memory.
    /// </summary>
    public class MemoryHelper
    {
        /// <summary>
        /// List of strings with COM Objects that are needed to invoke Close method in ReleaseAndCleanObject.
        /// </summary>
        private static List<string> ComObjectsWithClose = new List<string>(new string[]
        {
            "adodb.recordset",
            "adodb.connection"
        });

        /// <summary>
        /// Function to call the Garbage Collector and reclaim any available memory.
        /// </summary>
        public static void ReleaseMemory()
        {
            try
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Function to release the memory bound to an object, it is expected that the
        /// parameter is bound to a COM Object which memory wants to be reclaimed.
        /// </summary>
        /// <param name="obj">A .NET object representing a COM Object.</param>
        public static void ReleaseAndCleanObject(object obj)
        {
            if (obj != null)
            {
                try
                {
                    if (ComObjectsWithClose.Contains(obj.GetType().FullName.ToLower()))
                    {
                        //ReflectionHelper.Invoke(obj, "Close", new object[] { });
                        string memberName = "Close";
                        System.Reflection.PropertyInfo pInfo = obj.GetType().GetProperty(memberName);
                        Microsoft.VisualBasic.Interaction.CallByName(obj, memberName, Microsoft.VisualBasic.CallType.Method, new object[] { });
                    }
                    while (Marshal.ReleaseComObject(obj) > 0)
                    {
                    }
                }
                catch (Exception)
                {
                }
                finally
                {
                    ReleaseMemory();
                }
            }
        }

        /// <summary>
        /// Function to release the memory bound to an object, it is expected that the
        /// parameter is bound to a COM Object which memory wants to be reclaimed.
        /// </summary>
        /// <param name="obj">A .NET object representing a COM Object.</param>
        /// <param name="value">Value being assigned to the obj after releasing.</param>
        /// <returns>The value to be assigned to the obj.</returns>
        public static T ReleaseAndCleanObject<T>(object obj, object value)
        {
            if (obj != value) ReleaseAndCleanObject(obj);
            return (T)value;
        }

        /// <summary>
        /// Function to release the memory bound to an object, it is expected that the
        /// parameter is bound to a COM Object which memory wants to be reclaimed.
        /// </summary>
        /// <param name="obj">A .NET object representing a COM Object.</param>
        /// <param name="value">Value being assigned to the obj after releasing.</param>
        /// <returns>The value to be assigned to the obj.</returns>
        public static void ReleaseAndCleanObject(object obj, object value)
        {
            if (obj != value) ReleaseAndCleanObject(obj);
        }

        /// <summary>
        /// In VB6, it was possible to copy variables using API functions like hMemCpy or 
        /// statements like LSET.
        /// This helper function is used to provide a mechanism to easily turn an structure into
        /// an array of bytes that can then be easily manipulated
        /// </summary>
        /// <param name="obj">The structure that will be copied to an array of bytes</param>
        /// <returns>An array of bytes containing a copy of the information hold by a struct</returns>
        public static byte[] StructureToByteArray(object obj)
        {
            int len = Marshal.SizeOf(obj);
            byte[] arr = new byte[len];
            IntPtr ptr = Marshal.AllocHGlobal(len);
            Marshal.StructureToPtr(obj, ptr, true);
            Marshal.Copy(ptr, arr, 0, len);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        /// <summary>
        /// This overload calls ByteArrayToStructure assuming an startIndex of 0
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytearray"></param>
        /// <param name="obj"></param>
        public static void ByteArrayToStructure<T>(byte[] bytearray, ref T obj)
        {
            ByteArrayToStructure(bytearray, 0, ref obj);
        }

        /// <summary>
        /// In VB6, it was possible to copy variables using API functions like hMemCpy or 
        /// statements like LSET.
        /// This helper function is used to provide a mechanism to easily take an array
        /// of bytes and put that information inside an struct.
        /// </summary>
        /// <param name="bytearray">The array containing the information</param>
        /// <param name="startIndex">This index established the place where to start copying data from this array to the structure</param>
        /// <param name="obj">The target structure</param>
        public static void ByteArrayToStructure<T>(byte[] bytearray, int startIndex, ref T obj)
        {
            int len = Marshal.SizeOf(obj);
            IntPtr i = Marshal.AllocHGlobal(len);
            if (startIndex + len > bytearray.Length)
            {
                throw new IndexOutOfRangeException("The array does not hold enough information to update the structure");
            }
            Marshal.Copy(bytearray, startIndex, i, len);
            obj = (T)Marshal.PtrToStructure(i, typeof(T));
            Marshal.FreeHGlobal(i);
        }

        /// <summary>
        /// Copies data from one struct to another.
        /// This will be an exact copy of bytes.
        /// Users must be aware that reference types 
        /// might still be pointing to the same memory areas.
        /// Source and Destination might have different struct types
        /// </summary>
        /// <typeparam name="T2">Destination Struct Type</typeparam>
        /// <typeparam name="T1">Source Struct Type</typeparam>
        /// <param name="dest">Destination Struct</param>
        /// <param name="source">Source Struct</param>
        public static void CopyMemory<T2, T1>(ref T2 dest, T1 source)
        {
            byte[] data;
            data = StructureToByteArray(source);
            object temp = dest;
            ByteArrayToStructure(data, ref temp);
            dest = (T2)temp;
        }
    }
}
