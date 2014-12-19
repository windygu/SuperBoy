﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Systems
{
    /// <summary>
    /// 提供用户硬件唯一信息的辅助类
    /// </summary>
    public class FingerprintHelper
    {
        public static string Value()
        {
            return Pack(CpuId()
                    + BiosId()
                    + DiskId()
                    + BaseId()
                    + VideoId()
                    + MacId());
        }

        //Return a hardware identifier
        private static string identifier(string wmiClass, string wmiProperty, string wmiMustBeTrue)
        {
            var result="";
            var mc = new System.Management.ManagementClass(wmiClass);
            var moc = mc.GetInstances();
            foreach (System.Management.ManagementObject mo in moc)
            {
                if (mo[wmiMustBeTrue].ToString()=="True")
                {

                    //Only get the first one
                    if (result=="")
                    {
                        try
                        {
                            result = mo[wmiProperty].ToString();
                            break;
                        }
                        catch
                        {
                        }
                    }

                }
            }
            return result;
        }

        //Return a hardware identifier
        private static string identifier(string wmiClass, string wmiProperty)
        {
            var result="";
            var mc = new System.Management.ManagementClass(wmiClass);
            var moc = mc.GetInstances();
            foreach (System.Management.ManagementObject mo in moc)
            {

                //Only get the first one
                if (result=="")
                {
                    try
                    {
                        result = mo[wmiProperty].ToString();
                        break;
                    }
                    catch
                    {
                    }
                }

            }
            return result;
        }

        private static string CpuId()
        {
            //Uses first CPU identifier available in order of preference
            //Don't get all identifiers, as very time consuming
            var retVal = identifier("Win32_Processor", "UniqueId");
            if (retVal=="") //If no UniqueID, use ProcessorID
            {
                retVal = identifier("Win32_Processor", "ProcessorId");

                if (retVal=="") //If no ProcessorId, use Name
                {
                    retVal = identifier("Win32_Processor", "Name");


                    if (retVal=="") //If no Name, use Manufacturer
                    {
                        retVal = identifier("Win32_Processor", "Manufacturer");
                    }

                    //Add clock speed for extra security
                    retVal +=identifier("Win32_Processor", "MaxClockSpeed");
                }
            }

            return retVal;
        }

        //BIOS Identifier
        private static string BiosId()
        {
            return identifier("Win32_BIOS", "Manufacturer")
                    + identifier("Win32_BIOS", "SMBIOSBIOSVersion")
                    + identifier("Win32_BIOS", "IdentificationCode")
                    + identifier("Win32_BIOS", "SerialNumber")
                    + identifier("Win32_BIOS", "ReleaseDate")
                    + identifier("Win32_BIOS", "Version");
        }

        //Main physical hard drive ID
        private static string DiskId()
        {
            return identifier("Win32_DiskDrive", "Model")
                    + identifier("Win32_DiskDrive", "Manufacturer")
                    + identifier("Win32_DiskDrive", "Signature")
                    + identifier("Win32_DiskDrive", "TotalHeads");
        }

        //Motherboard ID
        private static string BaseId()
        {
            return identifier("Win32_BaseBoard", "Model")
                    + identifier("Win32_BaseBoard", "Manufacturer")
                    + identifier("Win32_BaseBoard", "Name")
                    + identifier("Win32_BaseBoard", "SerialNumber");
        }

        //Primary video controller ID
        private static string VideoId()
        {
            return identifier("Win32_VideoController", "DriverVersion")
                    + identifier("Win32_VideoController", "Name");
        }

        //First enabled network card ID
        private static string MacId()
        {
            return identifier("Win32_NetworkAdapterConfiguration", "MACAddress", "IPEnabled");
        }

        //Packs the string to 8 digits
        private static string Pack(string text)
        {
            string retVal;
            var x = 0;
            var y = 0;
            foreach (var n in text)
            {
                y++;
                x += (n*y);
            }
            retVal = x.ToString() + "00000000";

            return retVal.Substring(0, 8);
        }
    }
}
