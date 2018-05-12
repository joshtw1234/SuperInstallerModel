using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SuperInstallModel.Model
{
#if false
    class MSFWTableHelper
    {
        int SMBIOSMajorV = 0, SMBIOSMinorV = 0;
        Dictionary<int, CBaseSMBIOSType> dicSMBIOS;
        public MSFWTableHelper()
        {
            dicSMBIOS = new Dictionary<int, CBaseSMBIOSType>();
            var smData = GetFirmwareData(Provider.RSMB);
            SMBIOSMajorV = smData.SMBIOSMajorVersion;
            SMBIOSMinorV = smData.SMBIOSMinorVersion;
            Console.WriteLine($"SMBIOS Maj {SMBIOSMajorV} Min {SMBIOSMinorV}");
            var type0 = GetSMBIOSTypeData(0, smData.SMBIOSTableData);
            dicSMBIOS.Add(0, type0);
            var type1 = GetSMBIOSTypeData(1, smData.SMBIOSTableData);
            dicSMBIOS.Add(1, type1);
            var type2 = GetSMBIOSTypeData(2, smData.SMBIOSTableData);
            dicSMBIOS.Add(2, type2);
            var type3 = GetSMBIOSTypeData(3, smData.SMBIOSTableData);
            dicSMBIOS.Add(3, type3);
            var type4 = GetSMBIOSTypeData(4, smData.SMBIOSTableData);
            dicSMBIOS.Add(4, type4);
        }

        private RawSMBIOSData GetFirmwareData(Provider provider)
        {
            RawSMBIOSData smData = new RawSMBIOSData();
            int revSize = 0;
            revSize = Win32Dlls.GetSystemFirmwareTable(provider, 0, IntPtr.Zero, 0);
            IntPtr nativeBuffer = Marshal.AllocHGlobal(revSize);
            Win32Dlls.GetSystemFirmwareTable(provider, 0, nativeBuffer, revSize);

            if (Marshal.GetLastWin32Error() != 0)
            {
                return smData;
            }
            smData = (RawSMBIOSData)Marshal.PtrToStructure(nativeBuffer, typeof(RawSMBIOSData));
            Marshal.FreeHGlobal(nativeBuffer);
            return smData;
        }

        private CBaseSMBIOSType GetSMBIOSTypeData(int typeIdx, byte[] rawSMBIOS)
        {
            byte[] typeRawByte;
            int rawType = rawSMBIOS[0];
            int rawFormatLen = rawSMBIOS[1];
            int nextTypeStart = 0, curTypeEnd = 0;
            for (int i = rawFormatLen; ; i++)
            {
                if (rawSMBIOS[i] == 0 && rawSMBIOS[i + 1] == 0)
                {

                    if (rawType == typeIdx)
                    {
                        //Found Table
                        curTypeEnd = i + 1;
                        typeRawByte = new byte[curTypeEnd - nextTypeStart + 1];
                        Array.Copy(rawSMBIOS, nextTypeStart, typeRawByte, 0, typeRawByte.Length);
                        break;
                    }
                    else
                    {
                        //Goto Next Table
                        nextTypeStart = i + 2;
                        rawType = rawSMBIOS[nextTypeStart];
                        i += rawSMBIOS[nextTypeStart + 1] + 2;
                    }
                }
            }
            return GetCSMBIOSType(typeIdx, typeRawByte);
        }

        private CBaseSMBIOSType GetCSMBIOSType(int typeIdx, byte[] typeRawByte)
        {
            CBaseSMBIOSType revType = null;
            object tmpObj = null;
            GCHandle gch = GCHandle.Alloc(typeRawByte, GCHandleType.Pinned);
            switch (typeIdx)
            {
                case 0:
                    tmpObj = (SMBIOSType0)Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(SMBIOSType0));
                    revType = new CSMBIOSType0();
                    revType.smBIOS = (SMBIOSType0)tmpObj;
                    break;
                case 1:
                    tmpObj = Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(SMBIOSType1));
                    revType = new CSMBIOSType1();
                    revType.smBIOS = (SMBIOSType1)tmpObj;
                    break;
                case 2:
                    tmpObj = Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(SMBIOSType2));
                    revType = new CSMBIOSType2();
                    revType.smBIOS = (SMBIOSType2)tmpObj;
                    break;
                case 3:
                    tmpObj = Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(SMBIOSType3));
                    revType = new CSMBIOSType3();
                    revType.smBIOS = (SMBIOSType3)tmpObj;
                    break;
                case 4:
                    tmpObj = Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(SMBIOSType4));
                    revType = new CSMBIOSType4();
                    revType.smBIOS = (SMBIOSType4)tmpObj;
                    break;

            }
            gch.Free();
            //Get String in Table
            byte[] strByte = new byte[typeRawByte.Length - ((BaseSMBIOSType)tmpObj).Length];
            Array.Copy(typeRawByte, ((BaseSMBIOSType)tmpObj).Length, strByte, 0, strByte.Length);
            string str = System.Text.Encoding.Default.GetString(strByte);
            string[] strs = str.Split('\0');
            try
            {
                switch (typeIdx)
                {
                    case 0:
                        ((CSMBIOSType0)revType).Vendor = strs[((SMBIOSType0)revType.smBIOS).byVendor - 1];
                        ((CSMBIOSType0)revType).BIOSVersion = strs[((SMBIOSType0)revType.smBIOS).byBiosVersion - 1];
                        ((CSMBIOSType0)revType).BIOSReleaseDate = strs[((SMBIOSType0)revType.smBIOS).byBIOSReleaseDate - 1];
                        break;
                    case 1:
                        ((CSMBIOSType1)revType).Manufacturer = strs[((SMBIOSType1)revType.smBIOS).byManufacturer - 1];
                        ((CSMBIOSType1)revType).ProductName = strs[((SMBIOSType1)revType.smBIOS).byProductName - 1];
                        ((CSMBIOSType1)revType).Version = strs[((SMBIOSType1)revType.smBIOS).byVersion - 1];
                        ((CSMBIOSType1)revType).SerialNum = strs[((SMBIOSType1)revType.smBIOS).bySerialNumber - 1];
                        ((CSMBIOSType1)revType).SKUNumber = strs[((SMBIOSType1)revType.smBIOS).bySKUNumber - 1];
                        ((CSMBIOSType1)revType).Family = strs[((SMBIOSType1)revType.smBIOS).byFamily - 1];
                        break;
                    case 2:
                        ((CSMBIOSType2)revType).Manufacturer = strs[((SMBIOSType2)revType.smBIOS).byManufacturer - 1];
                        ((CSMBIOSType2)revType).Product = strs[((SMBIOSType2)revType.smBIOS).byProduct - 1];
                        ((CSMBIOSType2)revType).Version = strs[((SMBIOSType2)revType.smBIOS).byVersion - 1];
                        ((CSMBIOSType2)revType).SerialNumber = strs[((SMBIOSType2)revType.smBIOS).bySerialNumber - 1];
                        ((CSMBIOSType2)revType).AssetTag = strs[((SMBIOSType2)revType.smBIOS).byAssetTag - 1];
                        ((CSMBIOSType2)revType).LocationinChassis = strs[((SMBIOSType2)revType.smBIOS).byLocationInChassis - 1];
                        break;
                    case 3:
                        ((CSMBIOSType3)revType).Manufacturer = strs[((SMBIOSType3)revType.smBIOS).byManufacturer - 1];
                        ((CSMBIOSType3)revType).Version = strs[((SMBIOSType3)revType.smBIOS).byVersion - 1];
                        ((CSMBIOSType3)revType).SerialNumber = strs[((SMBIOSType3)revType.smBIOS).bySerialNumber - 1];
                        try
                        {
                            //Usually This type value will not fill completed.
                            ((CSMBIOSType3)revType).AssetTag = strs[((SMBIOSType3)revType.smBIOS).byAssetTag - 1];
                        }catch(Exception ex)
                        {
                            Console.WriteLine($"[Exception type] {typeIdx} [Asset] {((SMBIOSType3)revType.smBIOS).byAssetTag} {ex.Message} {ex.StackTrace}");
                        }
                        break;
                    case 4:
                        ((CSMBIOSType4)revType).SocketDesignation = strs[((SMBIOSType4)revType.smBIOS).bySocketDesignation - 1];
                        ((CSMBIOSType4)revType).ProcessorManufacturer = strs[((SMBIOSType4)revType.smBIOS).byProcessorManufacturer - 1];
                        ((CSMBIOSType4)revType).PartNumber = strs[((SMBIOSType4)revType.smBIOS).byPartNumber - 1];
                        ((CSMBIOSType4)revType).ProcessorVersion = strs[((SMBIOSType4)revType.smBIOS).byProcessorVersion - 1];
                        ((CSMBIOSType4)revType).SerialNumber = strs[((SMBIOSType4)revType.smBIOS).bySerialNumber - 1];
                        ((CSMBIOSType4)revType).AssetTag = strs[((SMBIOSType4)revType.smBIOS).byAssetTagNumber - 1];
                        break;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"[Exception type] {typeIdx} {ex.Message} {ex.StackTrace}");
            }
            return revType;
        }

        public object GetSMBIOSData(Provider proVider)
        {
            switch(proVider)
            {
                case Provider.RSMB:
                    return dicSMBIOS;
                case Provider.ACPI:
                    return null;

            }
            return null;
        }
    }
#endif
}
