using System.Runtime.InteropServices;

namespace SuperInstallModel.Model
{
    public enum Provider : int
    {
        ACPI = (byte)'A' << 24 | (byte)'C' << 16 | (byte)'P' << 8 | (byte)'I',
        FIRM = (byte)'F' << 24 | (byte)'I' << 16 | (byte)'R' << 8 | (byte)'M',
        RSMB = (byte)'R' << 24 | (byte)'S' << 16 | (byte)'M' << 8 | (byte)'B'
    }

    #region Raw SMBIOS Type
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

    [StructLayout(LayoutKind.Sequential)]
    public class SMBIOSType2 : BaseSMBIOSType
    {
        public byte byManufacturer;
        public byte byProduct;
        public byte byVersion;
        public byte bySerialNumber;
        public byte byAssetTag;
        public byte byFeatureFlags;
        public byte byLocationInChassis;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] wChassisHandle;
        public byte byBoardType;
        public byte byNoOfContainedObjectHandles;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
        public byte[] ContainedObjectHandles;
    }
    #endregion

    #region C# SMBIOS Type
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

    public class CSMBIOSType2 : CBaseSMBIOSType
    {
        public string Manufacturer;
        public string Product;
        public string Version;
        public string SerialNumber;
        public string AssetTag;
        public string LocationinChassis;
    }
    #endregion
}
