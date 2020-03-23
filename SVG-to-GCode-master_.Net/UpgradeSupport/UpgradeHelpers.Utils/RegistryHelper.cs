namespace UpgradeHelpers.Helpers
{
    using System;
    using System.IO;
    using Microsoft.Win32;
    using System.Linq;

    /// <summary>
    /// The RegistryHelper is an utility that provides functionality related to registry operations.
    /// </summary>
    public class RegistryHelper
    {
        /// <summary>
        /// Writes a value to the registry key-value name provided
        /// </summary>
        /// <param name="keyValue">String with the key and value name.</param>
        /// <param name="value">String with the data value to write.</param>
        /// <param name="type">String with the type of the data value.</param>
        public static void RegWriteValue(string keyValue, object value, string typeName = null)
        {
            string keyName = Path.GetDirectoryName(keyValue);
            string valueName = Path.GetFileNameWithoutExtension(keyValue);
            RegistryValueKind valueType;
            
            if (string.IsNullOrEmpty(typeName))
            {
                valueType = RegistryValueKind.Unknown;
            }
            else
            {
                switch (typeName)
                {
#if !(NET10 || NET20 || NET30 || NET35)
                    case "REG_NONE":
                        valueType = RegistryValueKind.None;
                        break;
#endif
                    case "REG_SZ":
                        valueType = RegistryValueKind.String;
                        break;
                    case "REG_EXPAND_SZ":
                        valueType = RegistryValueKind.ExpandString;
                        break;
                    case "REG_BINARY":
                        valueType = RegistryValueKind.Binary;
                        break;
                    case "REG_DWORD":
                        valueType = RegistryValueKind.DWord;
                        break;
                    case "REG_MULTI_SZ":
                        valueType = RegistryValueKind.MultiString;
                        break;
                    case "REG_QWORD":
                        valueType = RegistryValueKind.QWord;
                        break;
                    default:
                        valueType = RegistryValueKind.Unknown;
                        break;
                }
            }

            GetRegistryKey(keyName, true).SetValue(valueName, value, valueType);
        }

        /// <summary>
        /// Deletes the value from the registry key-value name provided
        /// </summary>
        /// <param name="keyValue">String with the key and value name to delete.</param>
        public static void RegDeleteValue(string keyValue)
        {
            string keyName = Path.GetDirectoryName(keyValue);
            string valueName = Path.GetFileNameWithoutExtension(keyValue);

            GetRegistryKey(keyName, true).DeleteValue(valueName, true);
        }

        /// <summary>
        /// Obtains the data value from the registry key-value name provided
        /// </summary>
        /// <param name="keyValue">String with the key and value name to read.</param>
        /// <returns>An object with the data of the value from <i>keyValue</i> registry.</returns>
        public static object RegReadDataValue(string keyValue)
        {
            object dataValue;
            string keyName = Path.GetDirectoryName(keyValue);
            string valueName = Path.GetFileNameWithoutExtension(keyValue);

            dataValue = GetRegistryKey(keyName).GetValue(valueName, null);

            if (dataValue == null)
            {
                throw new ArgumentException("No value exists with that name.");
            }

            return dataValue;
        }

        /// <summary>
        /// Changes short base key name representation to a long base key name representation.
        /// </summary>
        /// <param name="keyName">String with a registry key name.</param>
        /// <returns>A string with a valid base key name from <i>keyName</i> registry.</returns>
        private static RegistryKey GetRegistryKey(string keyName, bool writable = false)
        {
            RegistryKey baseRegKey;
            string[] keyNameSplitted = keyName.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);

            switch (keyNameSplitted[0])
            {
                case "HKLM":
                    baseRegKey = Registry.LocalMachine;
                    break;
                case "HKCC":
                    baseRegKey = Registry.CurrentConfig;
                    break;
                case "HKCR":
                    baseRegKey = Registry.ClassesRoot;
                    break;
                case "HKU":
                    baseRegKey = Registry.Users;
                    break;
                case "HKCU":
                default:
                    baseRegKey = Registry.CurrentUser;
                    break;
            }

            return baseRegKey.OpenSubKey(string.Join("\\", keyNameSplitted.Skip(1).ToArray()), writable);
        }
    }
}
