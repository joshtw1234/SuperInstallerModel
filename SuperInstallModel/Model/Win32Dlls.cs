using System;
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
    }
}
