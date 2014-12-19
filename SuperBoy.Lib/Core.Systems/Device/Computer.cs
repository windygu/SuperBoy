using System;
using System.Collections.Generic;
using System.Text;
using System.Management;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace Core.Systems
{
    /// <summary>
    /// 电脑信息
    /// </summary>
    public class Computer
    {
        #region CpuUsage类
        /// <summary>
        /// Defines an abstract base class for implementations of CPU usage counters.
        /// </summary>
        public abstract class CpuUsage
        {
            /// <summary>
            /// Creates and returns a CpuUsage instance that can be used to query the CPU time on this operating system.
            /// </summary>
            /// <returns>An instance of the CpuUsage class.</returns>
            /// <exception cref="NotSupportedException">This platform is not supported -or- initialization of the CPUUsage object failed.</exception>
            public static CpuUsage Create()
            {
                if (_mCpuUsage == null)
                {
                    if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                        _mCpuUsage = new CpuUsageNt();
                    else if (Environment.OSVersion.Platform == PlatformID.Win32Windows)
                        _mCpuUsage = new CpuUsage9X();
                    else
                        throw new NotSupportedException();
                }
                return _mCpuUsage;
            }

            /// <summary>
            /// Determines the current average CPU load.
            /// </summary>
            /// <returns>An integer that holds the CPU load percentage.</returns>
            /// <exception cref="NotSupportedException">One of the system calls fails. The CPU time can not be obtained.</exception>
            public abstract int Query();

            /// <summary>
            /// Holds an instance of the CPUUsage class.
            /// </summary>
            private static CpuUsage _mCpuUsage = null;
        }

        //------------------------------------------- win 9x ---------------------------------------

        /// <summary>
        /// Inherits the CPUUsage class and implements the Query method for Windows 9x systems.
        /// </summary>
        /// <remarks>
        /// <p>This class works on Windows 98 and Windows Millenium Edition.</p>
        /// <p>You should not use this class directly in your code. Use the CPUUsage.Create() method to instantiate a CPUUsage object.</p>
        /// </remarks>
        internal sealed class CpuUsage9X : CpuUsage
        {
            /// <summary>
            /// Initializes a new CPUUsage9x instance.
            /// </summary>
            /// <exception cref="NotSupportedException">One of the system calls fails.</exception>
            public CpuUsage9X()
            {
                try
                {
                    // start the counter by reading the value of the 'StartStat' key
                    var startKey = Registry.DynData.OpenSubKey(@"PerfStats\StartStat", false);
                    if (startKey == null)
                        throw new NotSupportedException();
                    startKey.GetValue(@"KERNEL\CPUUsage");
                    startKey.Close();
                    // open the counter's value key
                    _mStatData = Registry.DynData.OpenSubKey(@"PerfStats\StatData", false);
                    if (_mStatData == null)
                        throw new NotSupportedException();
                }
                catch (NotSupportedException e)
                {
                    throw e;
                }
                catch (Exception e)
                {
                    throw new NotSupportedException("Error while querying the system information.", e);
                }
            }

            /// <summary>
            /// Determines the current average CPU load.
            /// </summary>
            /// <returns>An integer that holds the CPU load percentage.</returns>
            /// <exception cref="NotSupportedException">One of the system calls fails. The CPU time can not be obtained.</exception>
            public override int Query()
            {
                try
                {
                    return (int)_mStatData.GetValue(@"KERNEL\CPUUsage");
                }
                catch (Exception e)
                {
                    throw new NotSupportedException("Error while querying the system information.", e);
                }
            }

            /// <summary>
            /// Closes the allocated resources.
            /// </summary>
            ~CpuUsage9X()
            {
                try
                {
                    _mStatData.Close();
                }
                catch { }
                // stopping the counter
                try
                {
                    var stopKey = Registry.DynData.OpenSubKey(@"PerfStats\StopStat", false);
                    stopKey.GetValue(@"KERNEL\CPUUsage", false);
                    stopKey.Close();
                }
                catch { }
            }

            /// <summary>Holds the registry key that's used to read the CPU load.</summary>
            private RegistryKey _mStatData;
        }

        //------------------------------------------- win nt ---------------------------------------

        /// <summary>
        /// Inherits the CPUUsage class and implements the Query method for Windows NT systems.
        /// </summary>
        /// <remarks>
        /// <p>This class works on Windows NT4, Windows 2000, Windows XP, Windows .NET Server and higher.</p>
        /// <p>You should not use this class directly in your code. Use the CPUUsage.Create() method to instantiate a CPUUsage object.</p>
        /// </remarks>
        internal sealed class CpuUsageNt : CpuUsage
        {
            /// <summary>
            /// Initializes a new CpuUsageNt instance.
            /// </summary>
            /// <exception cref="NotSupportedException">One of the system calls fails.</exception>
            public CpuUsageNt()
            {
                var timeInfo = new byte[32];         // SYSTEM_TIME_INFORMATION structure
                var perfInfo = new byte[312];        // SYSTEM_PERFORMANCE_INFORMATION structure
                var baseInfo = new byte[44];         // SYSTEM_BASIC_INFORMATION structure
                int ret;
                // get new system time
                ret = NtQuerySystemInformation(SystemTimeinformation, timeInfo, timeInfo.Length, IntPtr.Zero);
                if (ret != NoError)
                    throw new NotSupportedException();
                // get new CPU's idle time
                ret = NtQuerySystemInformation(SystemPerformanceinformation, perfInfo, perfInfo.Length, IntPtr.Zero);
                if (ret != NoError)
                    throw new NotSupportedException();
                // get number of processors in the system
                ret = NtQuerySystemInformation(SystemBasicinformation, baseInfo, baseInfo.Length, IntPtr.Zero);
                if (ret != NoError)
                    throw new NotSupportedException();
                // store new CPU's idle and system time and number of processors
                _oldIdleTime = BitConverter.ToInt64(perfInfo, 0); // SYSTEM_PERFORMANCE_INFORMATION.liIdleTime
                _oldSystemTime = BitConverter.ToInt64(timeInfo, 8); // SYSTEM_TIME_INFORMATION.liKeSystemTime
                _processorCount = baseInfo[40];
            }

            /// <summary>
            /// Determines the current average CPU load.
            /// </summary>
            /// <returns>An integer that holds the CPU load percentage.</returns>
            /// <exception cref="NotSupportedException">One of the system calls fails. The CPU time can not be obtained.</exception>
            public override int Query()
            {
                var timeInfo = new byte[32];         // SYSTEM_TIME_INFORMATION structure
                var perfInfo = new byte[312];        // SYSTEM_PERFORMANCE_INFORMATION structure
                double dbIdleTime, dbSystemTime;
                int ret;
                // get new system time
                ret = NtQuerySystemInformation(SystemTimeinformation, timeInfo, timeInfo.Length, IntPtr.Zero);
                if (ret != NoError)
                    throw new NotSupportedException();
                // get new CPU's idle time
                ret = NtQuerySystemInformation(SystemPerformanceinformation, perfInfo, perfInfo.Length, IntPtr.Zero);
                if (ret != NoError)
                    throw new NotSupportedException();
                // CurrentValue = NewValue - OldValue
                dbIdleTime = BitConverter.ToInt64(perfInfo, 0) - _oldIdleTime;
                dbSystemTime = BitConverter.ToInt64(timeInfo, 8) - _oldSystemTime;
                // CurrentCpuIdle = IdleTime / SystemTime
                if (dbSystemTime != 0)
                    dbIdleTime = dbIdleTime / dbSystemTime;
                // CurrentCpuUsage% = 100 - (CurrentCpuIdle * 100) / NumberOfProcessors
                dbIdleTime = 100.0 - dbIdleTime * 100.0 / _processorCount + 0.5;
                // store new CPU's idle and system time
                _oldIdleTime = BitConverter.ToInt64(perfInfo, 0); // SYSTEM_PERFORMANCE_INFORMATION.liIdleTime
                _oldSystemTime = BitConverter.ToInt64(timeInfo, 8); // SYSTEM_TIME_INFORMATION.liKeSystemTime
                return (int)dbIdleTime;
            }

            /// <summary>
            /// NtQuerySystemInformation is an internal Windows function that retrieves various kinds of system information.
            /// </summary>
            /// <param name="dwInfoType">One of the values enumerated in SYSTEM_INFORMATION_CLASS, indicating the kind of system information to be retrieved.</param>
            /// <param name="lpStructure">Points to a buffer where the requested information is to be returned. The size and structure of this information varies depending on the value of the SystemInformationClass parameter.</param>
            /// <param name="dwSize">Length of the buffer pointed to by the SystemInformation parameter.</param>
            /// <param name="returnLength">Optional pointer to a location where the function writes the actual size of the information requested.</param>
            /// <returns>Returns a success NTSTATUS if successful, and an NTSTATUS error code otherwise.</returns>
            [DllImport("ntdll", EntryPoint = "NtQuerySystemInformation")]
            private static extern int NtQuerySystemInformation(int dwInfoType, byte[] lpStructure, int dwSize, IntPtr returnLength);
            
            /// <summary>Returns the number of processors in the system in a SYSTEM_BASIC_INFORMATION structure.</summary>
            private const int SystemBasicinformation = 0;
            
            /// <summary>Returns an opaque SYSTEM_PERFORMANCE_INFORMATION structure.</summary>
            private const int SystemPerformanceinformation = 2;
            
            /// <summary>Returns an opaque SYSTEM_TIMEOFDAY_INFORMATION structure.</summary>
            private const int SystemTimeinformation = 3;
           
            /// <summary>The value returned by NtQuerySystemInformation is no error occurred.</summary>
            private const int NoError = 0;
            
            /// <summary>Holds the old idle time.</summary>
            private long _oldIdleTime;

            /// <summary>Holds the old system time.</summary>
            private long _oldSystemTime;

            /// <summary>Holds the number of processors in the system.</summary>
            private double _processorCount;
        }
        #endregion

        /// <summary>
        /// 获得Cpu使用率
        /// </summary>
        /// <returns>返回使用率</returns>
        public static int GetCpuUsage()
        {
            return CpuUsage.Create().Query();
        }

        /// <summary>
        /// 获取CPU序列号代码
        /// </summary>
        /// <returns></returns>
        public static string GetCpuId()
        {
            var cpuInfo = string.Empty;
            using (var mc = new ManagementClass("Win32_Processor"))
            {
                var moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    cpuInfo = mo.Properties["ProcessorId"].Value.ToString();
                }

            }
            return cpuInfo;
        }

        /// <summary>
        /// 获取网卡硬件地址
        /// </summary>
        /// <returns></returns>
        public static string GetMacAddress()
        {
            var mac = string.Empty;
            using (var mc = new ManagementClass("Win32_NetworkAdapterConfiguration"))
            {
                var moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"])
                    {
                        mac = mo["MacAddress"].ToString();
                        break;
                    }
                }
            }
            return mac;
        }

        /// <summary>
        /// 获取硬盘ID
        /// </summary>
        /// <returns></returns>
        public static string GetDiskId()
        {
            var hDid = string.Empty;
            using (var mc = new ManagementClass("Win32_DiskDrive"))
            {
                var moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    hDid = (string)mo.Properties["Model"].Value;
                }
            }
            return hDid;
        }
        

        /// <summary>
        /// 操作系统的登录用户名
        /// </summary>
        /// <returns></returns>
        public static string GetUserName()
        {
            return Environment.UserName;
        }

        /// <summary>
        /// 获取计算机名
        /// </summary>
        /// <returns></returns>
        public static string GetComputerName()
        {
            return System.Environment.MachineName;
        }

        /// <summary>
        /// PC类型
        /// </summary>
        /// <returns></returns>
        public static string GetSystemType()
        {
            var st = string.Empty;
            using (var mc = new ManagementClass("Win32_ComputerSystem"))
            {
                var moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    st = mo["SystemType"].ToString();
                }
            }
            return st;
        }


        /// <summary>
        /// 物理内存
        /// </summary>
        /// <returns></returns>
        public static string GetTotalPhysicalMemory()
        {
            var st = string.Empty;
            using (var mc = new ManagementClass("Win32_ComputerSystem"))
            {
                var moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    st = mo["TotalPhysicalMemory"].ToString();
                }
            }
            return st;
        }
    }
}
