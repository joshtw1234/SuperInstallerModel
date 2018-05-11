using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace SuperInstallModel.Model
{
    class Win32Dlls
    {
        private const string KERNEL = "kernel32.dll";

        [DllImport(KERNEL, EntryPoint= "EnumSystemFirmwareTables", CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        public static extern int EnumSystemFirmwareTables(Provider firmwareTableProviderSignature, IntPtr firmwareTableBuffer, int bufferSize);

        [DllImport(KERNEL, EntryPoint = "GetSystemFirmwareTable", CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        public static extern int GetSystemFirmwareTable(Provider firmwareTableProviderSignature, int firmwareTableID, IntPtr firmwareTableBuffer, int bufferSize);

        /// <summary>
        /// The power status structure.
        /// </summary>
        public struct SYSTEM_POWER_STATUS
        {
            /// <summary>
            /// The AC power status. This member can be one of the following values.
            /// 0 = offline,  1 = Online, 255 = UnKnown Status.
            /// </summary>
            public Byte ACLineStatus;

            /// <summary>
            /// The battery charge status. This member can contain one or more of the following flags.
            /// 1 = High—the battery capacity is at more than 66 percent
            /// 2 = Low—the battery capacity is at less than 33 percent
            /// 4 = Critical—the battery capacity is at less than five percent
            /// 8 = Charging
            /// 128 = No system battery
            /// 255 = Unknown status—unable to read the battery flag information
            /// </summary>
            public Byte BatteryFlag;

            /// <summary>
            /// The percentage of full battery charge remaining. 
            /// This member can be a value in the range 0 to 100, or 255 if status is unknown.
            /// </summary>
            public Byte BatteryLifePercent;

            /// <summary>
            /// The status of battery saver.
            /// 0 = Battery saver is off.
            /// 1 = Battery saver on.Save energy where possible.
            /// </summary>
            public Byte SystemStatusFlag;

            /// <summary>
            /// The number of seconds of battery life remaining, or –1 if remaining seconds are unknown 
            /// or if the device is connected to AC power.
            /// </summary>
            public int BatteryLifeTime;

            /// <summary>
            /// The number of seconds of battery life when at full charge, or –1 if full battery lifetime is unknown 
            /// or if the device is connected to AC power.
            /// </summary>
            public int BatteryFullLifeTime;
        }

        /// <summary>
        /// The Get System Power status API
        /// </summary>
        /// <param name="lpSystemPowerStatus"></param>
        [DllImport("kernel32", EntryPoint = "GetSystemPowerStatus")]
        public static extern void GetSystemPowerStatus(ref SYSTEM_POWER_STATUS lpSystemPowerStatus);

        public static bool IsElevated
        {
            get
            {
                return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        private static object locker = new Object();
        public static void Logger(string logPath, string lines)
        {
            try
            {
                string strOutput = $"[{DateTime.Now}]={lines}";
                lock (locker)
                {
                    using (FileStream file = new FileStream(logPath, FileMode.Append, FileAccess.Write, FileShare.Read))
                    {
                        StreamWriter writer = new StreamWriter(file);
                        writer.WriteLine(strOutput);
                        writer.Flush();
                        file.Flush(true);
                        writer.Close();
                        file.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                Logger(SuperInstallConstants.LogPath, $"{System.Reflection.MethodBase.GetCurrentMethod().Name}:Exception:{ex.Message}");
            }
        }

        public static object GetManageObjValue(string queryScope, string queryStr, string propertyStr)
        {
            object revStr = null;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(queryScope, queryStr);
            try
            {
                foreach (ManagementObject wmi in searcher.Get())
                {

                    revStr = wmi.GetPropertyValue(propertyStr);

                    if (null != revStr)
                    {
                        if (revStr is string[])
                        {
                            Logger(SuperInstallConstants.LogPath, $"{propertyStr} \"{((string[])revStr)[0].ToString()}\"");
                        }
                        else
                        {
                            Logger(SuperInstallConstants.LogPath, $"{propertyStr} \"{revStr.ToString()}\"");
                        }
                        return revStr;
                    }
                    else
                    {
                        Logger(SuperInstallConstants.LogPath, $"{propertyStr} \"Not Found!!\"");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger(SuperInstallConstants.LogPath, $"{propertyStr} \"{ex.Message}\"");
            }
            return revStr;
        }
    }

    public enum InstallStates
    {
        None,
        Processing,
        Done
    }

    public class SWInfo
    {
        public InstallStates SWInstallStates;
        public string SWBMinVer;
        public string SWFMinVer;
        public string SWEXEName;
        public string SWEXECmd;
    }

    public class PlatformInfo : SWInfo
    {
        public string PlatformSSID;
        public List<SWInfo> SWList;
    }

    public class SuperInstallInfo
    {
        public InstallStates SuperInstallStates;
        public List<PlatformInfo> PlatformLst;
    }
}
