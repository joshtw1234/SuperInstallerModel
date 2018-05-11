using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;

namespace SuperInstallModel.Model
{
    class Win32Dlls
    {
        private const string KERNEL = "kernel32.dll";

        [DllImport(KERNEL, EntryPoint= "EnumSystemFirmwareTables", CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        public static extern int EnumSystemFirmwareTables(Provider firmwareTableProviderSignature, IntPtr firmwareTableBuffer, int bufferSize);

        [DllImport(KERNEL, EntryPoint = "GetSystemFirmwareTable", CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        public static extern int GetSystemFirmwareTable(Provider firmwareTableProviderSignature, int firmwareTableID, IntPtr firmwareTableBuffer, int bufferSize);

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

            foreach (ManagementObject wmi in searcher.Get())
            {
                try
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
                catch (Exception ex)
                {
                    Logger(SuperInstallConstants.LogPath, $"{propertyStr} \"{ex.Message}\"");
                }
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
        public string SWMinVer;
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
        public InstallStates InstallResult;
        public List<PlatformInfo> PlatformLst;
    }
}
