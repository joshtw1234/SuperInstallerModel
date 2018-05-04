using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SuperInstallModel.Model
{
    class SMBIOSHelper
    {
        Dictionary<int, CBaseSMBIOSType> dicSMBIOS;
        public SMBIOSHelper()
        {
            dicSMBIOS = new Dictionary<int, CBaseSMBIOSType>();
            var smData = GetFirmwareData(Provider.RSMB);
            var type0 = GetSMBIOSTypeData(0, smData.SMBIOSTableData);
            dicSMBIOS.Add(0, type0);
            var type1 = GetSMBIOSTypeData(1, smData.SMBIOSTableData);
            dicSMBIOS.Add(1, type1);
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
            object revObj = null;
            byte[] tmpByte;
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
                        tmpByte = new byte[curTypeEnd - nextTypeStart + 1];
                        Array.Copy(rawSMBIOS, nextTypeStart, tmpByte, 0, tmpByte.Length);
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
            CBaseSMBIOSType revType = null;
            GCHandle gch = GCHandle.Alloc(tmpByte, GCHandleType.Pinned);
            switch (typeIdx)
            {
                case 1:
                    revObj = Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(SMBIOSType1));
                    revType = new CSMBIOSType1();
                    revType.smBIOS = (SMBIOSType1)revObj;
                    break;
                default:
                    revObj = (SMBIOSType0)Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(SMBIOSType0));
                    revType = new CSMBIOSType0();
                    revType.smBIOS = (SMBIOSType0)revObj;
                    break;
            }
            gch.Free();
            //Get String in Table
            byte[] strByte = new byte[tmpByte.Length - ((BaseSMBIOSType)revObj).Length];
            Array.Copy(tmpByte, ((BaseSMBIOSType)revObj).Length, strByte, 0, strByte.Length);
            string str = System.Text.Encoding.Default.GetString(strByte);
            string[] strs = str.Split('\0');
            switch (typeIdx)
            {
                case 1:
                    ((CSMBIOSType1)revType).Manufacturer = strs[((SMBIOSType1)revType.smBIOS).byManufacturer - 1];
                    ((CSMBIOSType1)revType).ProductName = strs[((SMBIOSType1)revType.smBIOS).byProductName - 1];
                    ((CSMBIOSType1)revType).Version = strs[((SMBIOSType1)revType.smBIOS).byVersion - 1];
                    ((CSMBIOSType1)revType).SerialNum = strs[((SMBIOSType1)revType.smBIOS).bySerialNumber - 1];
                    ((CSMBIOSType1)revType).SKUNumber = strs[((SMBIOSType1)revType.smBIOS).bySKUNumber - 1];
                    ((CSMBIOSType1)revType).Family = strs[((SMBIOSType1)revType.smBIOS).byFamily - 1];
                    break;
                default:
                    ((CSMBIOSType0)revType).Vendor = strs[((SMBIOSType0)revType.smBIOS).byVendor - 1];
                    ((CSMBIOSType0)revType).BIOSVersion = strs[((SMBIOSType0)revType.smBIOS).byBiosVersion - 1];
                    ((CSMBIOSType0)revType).BIOSReleaseDate = strs[((SMBIOSType0)revType.smBIOS).byBIOSReleaseDate - 1];
                    break;
            }

            return revType;
        }

        public Dictionary<int, CBaseSMBIOSType> GetSMBIOSData()
        {
            return dicSMBIOS;
        }
    }
}
