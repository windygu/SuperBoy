using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.ComponentModel;
using Microsoft.Win32;

namespace Core.IO
{
    /// <summary>
    /// 注册文件关联的辅助类
    /// </summary>
    public class FileAssociationsHelper
    {
        private static RegistryKey _classesRoot;
        private static void Process(string[] args)
        {
            if (args.Length < 6)
            {
                const string error = ("Usage: <ProgId> <Register in HKCU: true|false> <AppId> <OpenWithSwitch> <Unregister: true|false> <Ext1> [Ext2 [Ext3] ...]");
                throw new ArgumentException(error);
            }
            try
            {
                var progId = args[0];
                var registerInHkcu = bool.Parse(args[1]);
                var appId = args[2];
                var openWith = args[3];
                var unregister = bool.Parse(args[4]);

                var argList = new List<string>();
                for (var i = 0; i < args.Length; i++)
                {
                    if (i < 5) continue;

                    argList.Add(args[i]);
                }
                var associationsToRegister = argList.ToArray();

                if (registerInHkcu)
                {
                    _classesRoot = Registry.CurrentUser.OpenSubKey(@"Software\Classes");
                }
                else
                {
                    _classesRoot = Registry.ClassesRoot;
                }

                //First of all, unregister:
                Array.ForEach(associationsToRegister,
                              assoc => UnregisterFileAssociation(progId, assoc));
                UnregisterProgId(progId);

                if (!unregister)
                {
                    RegisterProgId(progId, appId, openWith);
                    Array.ForEach(associationsToRegister,
                        assoc => RegisterFileAssociation(progId, assoc));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadLine();
            }
        }

        private static void RegisterProgId(string progId, string appId, string openWith)
        {
            var progIdKey = _classesRoot.CreateSubKey(progId);
            progIdKey.SetValue("FriendlyTypeName", "@shell32.dll,-8975");
            progIdKey.SetValue("DefaultIcon", "@shell32.dll,-47");
            progIdKey.SetValue("CurVer", progId);
            progIdKey.SetValue("AppUserModelID", appId);
            var shell = progIdKey.CreateSubKey("shell");
            shell.SetValue(String.Empty, "Open");
            shell = shell.CreateSubKey("Open");
            shell = shell.CreateSubKey("Command");
            shell.SetValue(String.Empty, openWith);

            shell.Close();
            progIdKey.Close();
        }
        private static void UnregisterProgId(string progId)
        {
            try
            {
                _classesRoot.DeleteSubKeyTree(progId);
            }
            catch { }
        }
        private static void RegisterFileAssociation(string progId, string extension)
        {
            var openWithKey = _classesRoot.CreateSubKey(
                Path.Combine(extension, "OpenWithProgIds"));
            openWithKey.SetValue(progId, String.Empty);
            openWithKey.Close();
        }
        private static void UnregisterFileAssociation(string progId, string extension)
        {
            try
            {
                var openWithKey = _classesRoot.CreateSubKey(
                    Path.Combine(extension, "OpenWithProgIds"));
                openWithKey.DeleteValue(progId);
                openWithKey.Close();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error while unregistering file association: " + e.Message);
            }
        }

        private static void InternalRegisterFileAssociations(
            bool unregister, string progId, bool registerInHkcu,
            string appId, string openWith, string[] extensions)
        {
            var arguments = string.Format("{0} {1} {2} \"{3}\" {4} {5}",
                    progId, // 0
                    registerInHkcu, // 1 
                    appId, // 2
                    openWith,
                    unregister,
                    string.Join(" ", extensions));
            try
            {
                Process(arguments.Split(' '));
            }
            catch (Win32Exception e)
            {
                if (e.NativeErrorCode == 1223) // 1223: The operation was canceled by the user. 
                {
                   //LogHelper.Info("该操作已经被用户取消。");
                }
            }
        }

        /// <summary>
        /// Registers file associations for an application.
        /// </summary>
        /// <param name="progId">The application's ProgID.</param>
        /// <param name="registerInHkcu">Whether to register the
        /// association per-user (in HKCU).  The only supported value
        /// at this time is <b>false</b>.</param>
        /// <param name="appId">The application's app-id.</param>
        /// <param name="openWith">The command and arguments to be used
        /// when opening a shortcut to a document.</param>
        /// <param name="extensions">The extensions to register.</param>
        public static void RegisterFileAssociations(string progId,
            bool registerInHkcu, string appId, string openWith,
            params string[] extensions)
        {
            InternalRegisterFileAssociations(
                false, progId, registerInHkcu, appId, openWith, extensions);
        }

        /// <summary>
        /// Unregisters file associations for an application.
        /// </summary>
        /// <param name="progId">The application's ProgID.</param>
        /// <param name="registerInHkcu">Whether to register the
        /// association per-user (in HKCU).  The only supported value
        /// at this time is <b>false</b>.</param>
        /// <param name="appId">The application's app-id.</param>
        /// <param name="openWith">The command and arguments to be used
        /// when opening a shortcut to a document.</param>
        /// <param name="extensions">The extensions to unregister.</param>
        public static void UnregisterFileAssociations(string progId,
            bool registerInHkcu, string appId, string openWith,
            params string[] extensions)
        {
            InternalRegisterFileAssociations(
                true, progId, registerInHkcu, appId, openWith, extensions);
        }
    }
}
