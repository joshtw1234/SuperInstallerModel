using System;
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
                            //OMENEventSource.Log.Info(string.Format("{0} \"{1}\"", propertyStr, ((string[])revStr)[0].ToString()));
                        }
                        else
                        {
                            //OMENEventSource.Log.Info(string.Format("{0} \"{1}\"", propertyStr, revStr.ToString()));
                        }
                        return revStr;
                    }
                    else
                    {
                        //OMENEventSource.Log.Info(string.Format("{0} \"{1}\"", propertyStr, "Not Found!!"));
                    }

                }

                catch (Exception ex)
                {
                    //OMENEventSource.Log.Error(string.Format("{0} \"{1}\"", propertyStr, ex.Message));
                }

            }
            return revStr;
        }
    }
}
