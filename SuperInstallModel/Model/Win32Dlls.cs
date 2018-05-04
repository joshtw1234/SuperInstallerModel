using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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

    public enum Provider : int
    {
        ACPI = (byte)'A' << 24 | (byte)'C' << 16 | (byte)'P' << 8 | (byte)'I',
        FIRM = (byte)'F' << 24 | (byte)'I' << 16 | (byte)'R' << 8 | (byte)'M',
        RSMB = (byte)'R' << 24 | (byte)'S' << 16 | (byte)'M' << 8 | (byte)'B'
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RawSMBIOSData
    {
        public byte Used20CallingMethod;
        public byte SMBIOSMajorVersion;
        public byte SMBIOSMinorVersion;
        public byte DmiRevision;
        public int Length;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5120)]
        public byte[] SMBIOSTableData;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class BaseSMBIOSType
    {
        public byte Type;
        public byte Length;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Handle;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class SMBIOSType0 : BaseSMBIOSType
    {
        public byte byVendor;
        public byte byBiosVersion;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] wBIOSStartingSegment;
        public byte byBIOSReleaseDate;
        public byte byBIOSROMSize;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] qwBIOSCharacteristics;
        public byte byExtensionByte1;
        public byte byExtensionByte2;
        public byte bySystemBIOSMajorRelease;
        public byte bySystemBIOSMinorRelease;
        public byte byEmbeddedFirmwareMajorRelease;
        public byte byEmbeddedFirmwareMinorRelease;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class SMBIOSType1 : BaseSMBIOSType
    {
        public byte byManufacturer;
        public byte byProductName;
        public byte byVersion;
        public byte bySerialNumber;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] byUUID;
        public byte byWakeupType;
        public byte bySKUNumber;
        public byte byFamily;
    }

    public class CBaseSMBIOSType
    {
       public BaseSMBIOSType smBIOS { get; set; }
    }
    public class CSMBIOSType0 : CBaseSMBIOSType
    {
        public string Vendor;
        public string BIOSVersion;
        public string BIOSReleaseDate;
    }

    public class CSMBIOSType1 : CBaseSMBIOSType
    {
        public string Manufacturer;
        public string ProductName;
        public string Version;
        public string SerialNum;
        public string SKUNumber;
        public string Family;
    }
}
