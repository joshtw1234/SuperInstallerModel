using System;
using System.Runtime.InteropServices;

namespace SuperInstallModel.Model
{
    public enum Provider : int
    {
        ACPI = (byte)'A' << 24 | (byte)'C' << 16 | (byte)'P' << 8 | (byte)'I',
        FIRM = (byte)'F' << 24 | (byte)'I' << 16 | (byte)'R' << 8 | (byte)'M',
        RSMB = (byte)'R' << 24 | (byte)'S' << 16 | (byte)'M' << 8 | (byte)'B'
    }

    #region New SMBIOS Method
    #region SMBIOS

    #region SMBIOS Raw Data

    /// <summary>
    /// The Base SMBIOS type class
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class BaseSMBIOSType
    {
        /// <summary>
        /// The SMBIOS type
        /// </summary>
        protected byte Type;
        /// <summary>
        /// The SMBIOS type length
        /// </summary>
        protected byte Length;
        /// <summary>
        /// The SMBIOS type handle
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        protected byte[] Handle;

        /// <summary>
        /// Get SMBIOS raw byte array by type
        /// </summary>
        /// <param name="typeIdx"></param>
        /// <param name="rawSMBIOSTable"></param>
        /// <returns></returns>
        private byte[] GetSMBIOSTypeRawByte(int typeIdx, byte[] rawSMBIOSTable)
        {
            byte[] typeRawByte;
            int rawType = rawSMBIOSTable[0];
            int rawFormatLen = rawSMBIOSTable[1];
            int nextTypeStart = 0, curTypeEnd = 0;
            for (int i = rawFormatLen; ; i++)
            {
                if (rawSMBIOSTable[i] == 0 && rawSMBIOSTable[i + 1] == 0)
                {

                    if (rawType == typeIdx)
                    {
                        //Found Table
                        curTypeEnd = i + 1;
                        typeRawByte = new byte[curTypeEnd - nextTypeStart + 1];
                        Array.Copy(rawSMBIOSTable, nextTypeStart, typeRawByte, 0, typeRawByte.Length);
                        break;
                    }
                    else
                    {
                        //Goto Next Table
                        nextTypeStart = i + 2;
                        rawType = rawSMBIOSTable[nextTypeStart];
                        i += rawSMBIOSTable[nextTypeStart + 1] + 2;
                    }
                }
            }
            return typeRawByte;
        }
        /// <summary>
        /// Get SMBIOS type detail data.
        /// </summary>
        /// <param name="typeIdx"></param>
        /// <param name="rawSMBIOS"></param>
        /// <param name="smBIOSType"></param>
        /// <param name="typeStrings"></param>
        /// <returns></returns>
        protected object GetSMBIOSTypeData(int typeIdx, byte[] rawSMBIOS, Type smBIOSType, out string[] typeStrings)
        {
            //Get SMBIOS type data
            byte[] typeRawByte = GetSMBIOSTypeRawByte(typeIdx, rawSMBIOS);
            object tmpObj = null;
            //Turn the byte to structure
            GCHandle gch = GCHandle.Alloc(typeRawByte, GCHandleType.Pinned);
            tmpObj = Marshal.PtrToStructure(gch.AddrOfPinnedObject(), smBIOSType);
            gch.Free();
            //Get String in Table
            byte[] strByte = new byte[typeRawByte.Length - ((BaseSMBIOSType)tmpObj).Length];
            Array.Copy(typeRawByte, ((BaseSMBIOSType)tmpObj).Length, strByte, 0, strByte.Length);
            string str = System.Text.Encoding.Default.GetString(strByte);
            typeStrings = str.Split('\0');
            return tmpObj;
        }
    }

    /// <summary>
    /// The SMBIOS Type 0
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    sealed class SMBIOSType0 : BaseSMBIOSType
    {
        #region SMBIOS property
        //SMBIOS type 0 spec
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
        #endregion

        /// <summary>
        /// Parameter less constructor
        /// </summary>
        private SMBIOSType0()
        {

        }
        /// <summary>
        /// Main constructor
        /// </summary>
        /// <param name="rawSMBIOS"></param>
        public SMBIOSType0(byte[] rawSMBIOS, out string[] typeStrs)
        {
            const int typeIdx = 0;
            object typeObj = this.GetSMBIOSTypeData(typeIdx, rawSMBIOS, this.GetType(), out typeStrs);
            //Assign property
            this.Type = (typeObj as SMBIOSType0).Type;
            this.Length = (typeObj as SMBIOSType0).Length;
            this.Handle = (typeObj as SMBIOSType0).Handle;
            this.byVendor = (typeObj as SMBIOSType0).byVendor;
            this.byBiosVersion = (typeObj as SMBIOSType0).byBiosVersion;
            this.wBIOSStartingSegment = (typeObj as SMBIOSType0).wBIOSStartingSegment;
            this.byBIOSReleaseDate = (typeObj as SMBIOSType0).byBIOSReleaseDate;
            this.byBIOSROMSize = (typeObj as SMBIOSType0).byBIOSROMSize;
            this.qwBIOSCharacteristics = (typeObj as SMBIOSType0).qwBIOSCharacteristics;
            this.byExtensionByte1 = (typeObj as SMBIOSType0).byExtensionByte1;
            this.byExtensionByte2 = (typeObj as SMBIOSType0).byExtensionByte2;
            this.bySystemBIOSMajorRelease = (typeObj as SMBIOSType0).bySystemBIOSMajorRelease;
            this.bySystemBIOSMinorRelease = (typeObj as SMBIOSType0).bySystemBIOSMinorRelease;
            this.byEmbeddedFirmwareMajorRelease = (typeObj as SMBIOSType0).byEmbeddedFirmwareMajorRelease;
            this.byEmbeddedFirmwareMinorRelease = (typeObj as SMBIOSType0).byEmbeddedFirmwareMinorRelease;
        }
    }

    /// <summary>
    /// The SMBIOS Type 2 class
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    sealed class SMBIOSType2 : BaseSMBIOSType
    {
        #region SMBIOS property
        //SMBIOS type 2 spec
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
        #endregion

        /// <summary>
        /// Parameter less constructor
        /// </summary>
        private SMBIOSType2()
        {

        }
        /// <summary>
        /// Main constructor
        /// </summary>
        /// <param name="rawSMBIOS"></param>
        public SMBIOSType2(byte[] rawSMBIOS, out string[] typeStrs)
        {
            const int typeIdx = 2;
            object typeObj = this.GetSMBIOSTypeData(typeIdx, rawSMBIOS, this.GetType(), out typeStrs);
            //Assign property
            this.Type = (typeObj as SMBIOSType2).Type;
            this.Length = (typeObj as SMBIOSType2).Length;
            this.Handle = (typeObj as SMBIOSType2).Handle;
            this.byManufacturer = (typeObj as SMBIOSType2).byManufacturer;
            this.byProduct = (typeObj as SMBIOSType2).byProduct;
            this.byVersion = (typeObj as SMBIOSType2).byVersion;
            this.bySerialNumber = (typeObj as SMBIOSType2).bySerialNumber;
            this.byAssetTag = (typeObj as SMBIOSType2).byAssetTag;
            this.byFeatureFlags = (typeObj as SMBIOSType2).byFeatureFlags;
            this.byLocationInChassis = (typeObj as SMBIOSType2).byLocationInChassis;
            this.wChassisHandle = (typeObj as SMBIOSType2).wChassisHandle;
            this.byBoardType = (typeObj as SMBIOSType2).byBoardType;
            this.byNoOfContainedObjectHandles = (typeObj as SMBIOSType2).byNoOfContainedObjectHandles;
            this.ContainedObjectHandles = (typeObj as SMBIOSType2).ContainedObjectHandles;
        }
    }
    #endregion

    #region C# SMBIOS Type

    /// <summary>
    /// Base C# SMBIOS type class
    /// </summary>
    public class BaseCSMBIOSType
    {
        /// <summary>
        /// The Raw SMBIOS type
        /// </summary>
        protected BaseSMBIOSType RawSMBIOSType;
    }
    /// <summary>
    /// The C# SMBIOS type 0
    /// </summary>
    public sealed class CSMBIOSType0 : BaseCSMBIOSType
    {
        //SMBIOS type 0 string
        public string Vendor;
        public string BIOSVersion;
        public string BIOSReleaseDate;
        /// <summary>
        /// Construct
        /// </summary>
        /// <param name="rawSMBIOSTable"></param>
        public CSMBIOSType0(byte[] rawSMBIOSTable)
        {
            string[] typeStrs = null;
            this.RawSMBIOSType = new SMBIOSType0(rawSMBIOSTable, out typeStrs);
            //Assign string
            Vendor = typeStrs[(this.RawSMBIOSType as SMBIOSType0).byVendor - 1];
            BIOSVersion = typeStrs[(this.RawSMBIOSType as SMBIOSType0).byBiosVersion - 1];
            BIOSReleaseDate = typeStrs[(this.RawSMBIOSType as SMBIOSType0).byBIOSReleaseDate - 1];
        }
    }
    /// <summary>
    /// The C# SMBIOS type 2
    /// </summary>
    public sealed class CSMBIOSType2 : BaseCSMBIOSType
    {
        //SMBIOS type 2 strings
        public string Manufacturer;
        public string Product;
        public string Version;
        public string SerialNumber;
        public string AssetTag;
        public string LocationinChassis;
        /// <summary>
        /// Construct
        /// </summary>
        /// <param name="rawSMBIOSTable"></param>
        public CSMBIOSType2(byte[] rawSMBIOSTable)
        {
            string[] typeStrs = null;
            this.RawSMBIOSType = new SMBIOSType2(rawSMBIOSTable, out typeStrs);
            //Assign string
            Manufacturer = typeStrs[(this.RawSMBIOSType as SMBIOSType2).byManufacturer - 1];
            Product = typeStrs[(this.RawSMBIOSType as SMBIOSType2).byProduct - 1];
            Version = typeStrs[(this.RawSMBIOSType as SMBIOSType2).byVersion - 1];
            SerialNumber = typeStrs[(this.RawSMBIOSType as SMBIOSType2).bySerialNumber - 1];
            AssetTag = typeStrs[(this.RawSMBIOSType as SMBIOSType2).byAssetTag - 1];
            LocationinChassis = typeStrs[(this.RawSMBIOSType as SMBIOSType2).byLocationInChassis - 1];
        }
    }
    #endregion

    #endregion
    #endregion

#if false
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

    [StructLayout(LayoutKind.Sequential)]
    public class SMBIOSType3 : BaseSMBIOSType
    {
        public byte byManufacturer;
        public byte byType;
        public byte byVersion;
        public byte bySerialNumber;
        public byte byAssetTag;
        public byte byBootupState;
        public byte byPowerSupplyState;
        public byte byThermalState;
        public byte bySecurityStatus;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] dwOEMdefined;
        public byte byHeight;
        public byte byNumberOfPowerCords;
        public byte byContainedElementCount;
        public byte byContainedElementRecordLength;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 255)]
        public byte[] ContainedElements;
        public byte bySKUNumber;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class SMBIOSType4 : BaseSMBIOSType
    {
        public byte bySocketDesignation;
        public byte byProcessorType;
        public byte byProcessorFamily;
        public byte byProcessorManufacturer;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] qwProcessorID;
        public byte byProcessorVersion;
        public byte byVoltage;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] wExternalClock;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] wMaxSpeed;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] wCurrentSpeed;
        public byte byStatus;
        public byte byProcessorUpgrade;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] wL1CacheHandle;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] wL2CacheHandle;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] wL3CacheHandle;
        public byte bySerialNumber;
        public byte byAssetTagNumber;
        public byte byPartNumber;
        public byte byCoreCount;
        public byte byCoreEnabled;
        public byte byThreadCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] wProcessorCharacteristics;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] wProcessorFamily2;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] wCoreCount2;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] wCoreEnabled2;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] wThreadCount2;
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

    public class CSMBIOSType3 : CBaseSMBIOSType
    {
        public string Manufacturer;
        public string Version;
        public string SerialNumber;
        public string AssetTag;
        public string SKUNumber;
    }

    public class CSMBIOSType4 : CBaseSMBIOSType
    {
        public string SocketDesignation;
        public string ProcessorManufacturer;
        public string ProcessorVersion;
        public string SerialNumber;
        public string AssetTag;
        public string PartNumber;
    }
    #endregion
#endif
}
