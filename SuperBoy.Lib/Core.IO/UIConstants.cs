using System;
using System.Collections.Generic;
using System.Text;

namespace Core.IO
{
    public static class UiConstants
    {
        private static string _applicationExpiredDate = "12/29/2009";
        private static string _softwareVersion = "3.0";
        private static string _softwareProductName = "OrderWaterEnterprise";
        private static string _softwareRegistryKey = "SOFTWARE\\Microsoft\\OrderWaterEnterprise\\" + _softwareVersion;
        public static int SoftwareProbationDay = 20;//软件的试用期

        public static string IsolatedStorage = "UserNameDir\\OrderWaterEnterprise.txt";
        public const string IsolatedStorageDirectoryName = "UserNameDir";
        public const string IsolatedStorageEncryptKey = "12345678";

        private static string _publicKey = @"<RSAKeyValue><Modulus>mtDtu679/0quhftVyOc6/cBov/i534Dkh3AB8RwrpC9Vq2RIFB3uvjRUuaAEPR8vMcijQjVzqLZgMM7jFKclzbh21rWTM+YlOeraKz5FPCC7rSLnv6Tfbzia9VI/r5cfM8ogVMuUKCZeU+PTEmVviasCl8nPYyqOQchlf/MftMM=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        public static string WebRegisterUrl = "http://www.iqidi.com/WebRegister.aspx";

        public static void SetValue(string expiredDate, string version, string name, string publicKey)
        {
            UiConstants._applicationExpiredDate = expiredDate;
            UiConstants._softwareVersion = version;
            UiConstants._softwareProductName = name;
            UiConstants._softwareRegistryKey = "SOFTWARE\\Microsoft\\" + name + "\\" + version;
            UiConstants.IsolatedStorage = "UserNameDir\\" + name + ".txt";
            UiConstants._publicKey = publicKey;
        }
    }
}
