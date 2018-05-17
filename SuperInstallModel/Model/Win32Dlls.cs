using IWshRuntimeLibrary;
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
                Logger(SuperInstallConstants.LogFileName, $"{System.Reflection.MethodBase.GetCurrentMethod().Name}:Exception:{ex.Message}");
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
                    if (null == revStr)
                    {
                        Logger(SuperInstallConstants.LogFileName, $"{propertyStr} \"Not Found!!\"");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger(SuperInstallConstants.LogFileName, $"{propertyStr} \"{ex.Message}\"");
            }
            return revStr;
        }

        public static int RunProcess(string appName, string arguments)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = appName;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            process.WaitForExit();
            return process.ExitCode;
        }

        public static string CreateShortcut(string shortcutName, string shortcutPath, string targetFileLocation, string shortcutDesc, string shortcutIcon)
        {
            string shortcutLocation = Path.Combine(shortcutPath, shortcutName + ".lnk");
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);

            shortcut.Description = shortcutDesc;   // The description of the shortcut
            if (!string.IsNullOrEmpty(shortcutIcon))
            {
                shortcut.IconLocation = shortcutIcon;           // The icon of the shortcut
            }
            shortcut.TargetPath = targetFileLocation;                 // The path of the file that will launch when the shortcut is run
            shortcut.WorkingDirectory = new FileInfo(targetFileLocation).DirectoryName;
            shortcut.Save();                                    // Save the shortcut
            return shortcutLocation;
        }
    }

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

    public enum InstallStage
    {
        None,
        Processing,
        Done
    }

    public class SWInfo
    {
        public InstallStage SWInstallStates;
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
        /// <summary>
        /// The test mode
        /// </summary>
        public bool IsBetaMode;
        /// <summary>
        /// The super install states.
        /// </summary>
        public InstallStage SuperInstallStates;
        /// <summary>
        /// The list of platform info
        /// </summary>
        public List<PlatformInfo> PlatformLst;
    }
}
