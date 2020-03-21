using System.Reflection;

namespace UpgradeHelpers.Helpers
{
    /// <summary>
    /// Provides support for excel object
    /// </summary>
    public class ExcelHelper
    {
        /// <summary>
        /// Excel Automatic constant -4105
        /// </summary>
        public const int xlAutomatic = -4105;

        /// <summary>
        /// Excel Manual constant -4135
        /// </summary>
        public const int xlManual = -4135;

        /// <summary>
        /// Excel Upward constant -4171
        /// </summary>
        public const int xlUpward = -4171;

        /// <summary>
        /// Excel Wait constant value 2
        /// </summary>
        public const int xlWait = 2;

        /// <summary>
        /// Method to Set Excel properties using InvokeMember
        /// </summary>
        /// <param name="obj">Excel instance</param>
        /// <param name="sProperty">Property to Set</param>
        /// <param name="oValue">Value to Set</param>
        public static void Set(object obj, string sProperty, object oValue)
        {
            object[] oParam = new object[1];
            oParam[0] = oValue;
            obj.GetType().InvokeMember(sProperty, BindingFlags.SetProperty, null, obj, oParam);
        }

        /// <summary>
        /// Method to Get values from Excel property
        /// </summary>
        /// <param name="obj">Excel instance</param>
        /// <param name="sProperty">Property to get</param>
        /// <param name="oValue">Value</param>
        /// <returns></returns>
        public static object Get(object obj, string sProperty, object oValue)
        {
            object[] oParam = new object[1];
            oParam[0] = oValue;
            return obj.GetType().InvokeMember(sProperty, BindingFlags.GetProperty, null, obj, oParam);
        }

        /// <summary>
        /// Met
        /// Method to Get values from Excel property
        /// </summary>
        /// <param name="obj">Excel instance</param>
        /// <param name="sProperty">Property to get</param>
        /// <param name="oValue1">Value</param>
        /// <param name="oValue2">Value</param>
        /// <returns></returns>
        public static object Get(object obj, string sProperty, object oValue1, object oValue2)
        {
            object[] oParam = new object[2];
            oParam[0] = oValue1;
            oParam[1] = oValue2;
            return obj.GetType().InvokeMember(sProperty, BindingFlags.GetProperty, null, obj, oParam);
        }

        /// <summary>
        /// Method to Get values from Excel property
        /// </summary>
        /// <param name="obj">Excel instance</param>
        /// <param name="sProperty">Property to get</param>
        /// <returns></returns>
        public static object Get(object obj, string sProperty)
        {
            return obj.GetType().InvokeMember(sProperty, BindingFlags.GetProperty, null, obj, null);
        }

        /// <summary>
        /// Method to invoke methods in excel using object array parameters
        /// </summary>
        /// <param name="obj">Excel instance</param>
        /// <param name="sMethod">Method to call</param>
        /// <param name="oParam">Parameters</param>
        /// <returns></returns>
        public static object Invoke(object obj, string sMethod, object[] oParam)
        {
            return obj.GetType().InvokeMember(sMethod, BindingFlags.InvokeMethod, null, obj, oParam);
        }

        /// <summary>
        /// Method to invoke methods in excel using object array parameters
        /// </summary>
        /// <param name="obj">Excel instance</param>
        /// <param name="sMethod">Method to call</param>
        /// <param name="oValue">Parameter</param>
        /// <returns></returns>
        public static object Invoke(object obj, string sMethod, object oValue)
        {
            object[] oParam = new object[1];
            oParam[0] = oValue;
            return obj.GetType().InvokeMember(sMethod, BindingFlags.InvokeMethod, null, obj, oParam);
        }

        /// <summary>
        /// Method to invoke methods in excel using object array parameters
        /// </summary>
        /// <param name="obj">Excel instance</param>
        /// <param name="sMethod">Method to call</param>
        /// <returns></returns>
        public static object Invoke(object obj, string sMethod)
        {
            return Invoke(obj, sMethod, null);
        }


    }
}
