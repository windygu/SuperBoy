using System;
using Microsoft.Win32;
using System.Windows.Forms;

namespace Core.Systems
{
    /// <summary>
    /// ע������������
    /// </summary>
    public sealed class RegistryHelper
    {
        #region ������������ȡ�ͱ��棩
        private static string _softwareKey = @"Software\DeepLand\OrderWater";

        /// <summary>
        /// Gets the value by registry key. If the key does not exist, return empty string.
        /// </summary>
        /// <param name="key">registry key</param>
        /// <returns>Returns the value of the specified key.</returns>
        public static string GetValue(string key)
        {
            return GetValue(_softwareKey, key);
        }

        /// <summary>
        /// Gets the value by registry key. If the key does not exist, return empty string.
        /// </summary>
        /// <param name="key">registry key</param>
        /// <returns>Returns the value of the specified key.</returns>
        public static string GetValue(string softwareKey, string key)
        {
            const string parameter = "key";
            if (null == key)
            {
                throw new ArgumentNullException(parameter);
            }

            var strRet = string.Empty;
            try
            {
                var regKey = Registry.CurrentUser.OpenSubKey(softwareKey);
                strRet = regKey.GetValue(key).ToString();
            }
            catch
            {
                strRet = "";
            }
            return strRet;
        }
        /// <summary>
        /// Saves the key and the value to registry.
        /// </summary>
        /// <param name="key">registry key</param>
        /// <param name="value">the value of the key</param>
        /// <returns>Returns true if successful, otherwise return false.</returns>
        public static bool SaveValue(string key, string value)
        {
            return SaveValue(_softwareKey, key, value);
        }

        /// <summary>
        /// Saves the key and the value to registry.
        /// </summary>
        /// <param name="key">registry key</param>
        /// <param name="value">the value of the key</param>
        /// <returns>Returns true if successful, otherwise return false.</returns>
        public static bool SaveValue(string softwareKey, string key, string value)
        {
            const string parameter1 = "key";
            const string parameter2 = "value";
            if (null == key)
            {
                throw new ArgumentNullException(parameter1);
            }

            if (null == value)
            {
                throw new ArgumentNullException(parameter2);
            }

            var bReturn = false;
            RegistryKey reg;
            reg = Registry.CurrentUser.OpenSubKey(softwareKey, true);

            if (null == reg)
            {
                reg = Registry.CurrentUser.CreateSubKey(softwareKey);
            }
            reg.SetValue(key, value);

            return bReturn;
        } 
        #endregion

        #region �Զ�������������

        public static bool CheckStartWithWindows()
        {
            var regkey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
            if (regkey != null && (string)regkey.GetValue(Application.ProductName, "null", RegistryValueOptions.None) != "null")
            {
                Registry.CurrentUser.Flush();
                return true;
            }

            Registry.CurrentUser.Flush();
            return false;
        }

        public static void SetStartWithWindows(bool startWin)
        {
            var regkey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            if (regkey != null)
            {
                if (startWin)
                {
                    regkey.SetValue(Application.ProductName, Application.ExecutablePath, RegistryValueKind.String);
                }
                else
                {
                    regkey.DeleteValue(Application.ProductName, false);
                }

                Registry.CurrentUser.Flush();
            }
        }

        #endregion
    }
}