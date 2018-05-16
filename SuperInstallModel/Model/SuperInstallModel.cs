using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace SuperInstallModel.Model
{
    class SuperInstallModel
    {
        SuperInstallInfo spInstallInfo;
        public bool Initialize()
        {
            bool rev = false;
            FileInfo fi = new FileInfo(SuperInstallConstants.LogPath);
            if (!fi.Directory.Exists)
            {
                Directory.CreateDirectory(fi.Directory.FullName);
            }
            if (!File.Exists(SuperInstallConstants.LogPath))
            {
                FileStream fs = File.Create(SuperInstallConstants.LogPath);
                fs.Close();
            }
            string dataStr = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "SPInstallInfo.json"));
            var result = JsonConvert.DeserializeObject<SuperInstallInfo>(dataStr);

            Console.WriteLine($"[Windows Name] {Win32Dlls.GetManageObjValue(SuperInstallConstants.WMICIMRoot, SuperInstallConstants.WMIQueryStr, SuperInstallConstants.WinCaption)}");
            Console.WriteLine($"[Windows Version] {Win32Dlls.GetManageObjValue(SuperInstallConstants.WMICIMRoot, SuperInstallConstants.WMIQueryStr, SuperInstallConstants.WinVersion)}");
            //Console.WriteLine($"[Primary] {Win32Dlls.GetManageObjValue(SuperInstallConstants.WMICIMRoot, SuperInstallConstants.WMIQueryStr, SuperInstallConstants.WinPrimary)}");
            Console.WriteLine($"[Windows OS] {Win32Dlls.GetManageObjValue(SuperInstallConstants.WMICIMRoot, SuperInstallConstants.WMIQueryStr, SuperInstallConstants.WinArchitecture)}");
            Console.WriteLine($"[System Vendor] {Win32Dlls.GetManageObjValue(SuperInstallConstants.WMICIMRoot, SuperInstallConstants.WMISystemQueryStry, SuperInstallConstants.WinManufacturer)}");
            CSMBIOSType2 smbios2 = new CSMBIOSType2();
            CSMBIOSType0 smbios0 = new CSMBIOSType0();
            Console.WriteLine($"[System ID] {smbios2.Product}");
            Console.WriteLine($"[BIOS Version] {Win32Dlls.GetManageObjValue(SuperInstallConstants.WMICIMRoot, SuperInstallConstants.WMIBIOSQueryStry, SuperInstallConstants.WinCaption)}");
            Console.WriteLine($"[CPU Name] {Win32Dlls.GetManageObjValue(SuperInstallConstants.WMICIMRoot, SuperInstallConstants.WMICPUQueryStry, SuperInstallConstants.WinName)}");
            
            SYSTEM_POWER_STATUS SysPower = new SYSTEM_POWER_STATUS();
            Win32Dlls.GetSystemPowerStatus(ref SysPower);
            string pwMsg = "Power Connected";
            if (!Convert.ToBoolean(SysPower.ACLineStatus))
            {
                pwMsg = "Power not Connected";
            }
            Console.WriteLine($"[Power connect] {pwMsg}");
            pwMsg = "Administrator";
            if (!Win32Dlls.IsElevated)
            {
                pwMsg = "Not Administrator";
            }
            Console.WriteLine($"[App privilege] {pwMsg}");
            Console.WriteLine();
#if false
            var dicSMBIOS = (new MSFWTableHelper().GetSMBIOSData(Provider.RSMB)) as Dictionary<int, CBaseSMBIOSType>;
            if (dicSMBIOS.Count > 0)
            {
                Console.WriteLine($"[Vendor] {((CSMBIOSType0)dicSMBIOS[0]).Vendor} [Bios Ver] {((CSMBIOSType0)dicSMBIOS[0]).BIOSVersion} [Bios Date] {((CSMBIOSType0)dicSMBIOS[0]).BIOSReleaseDate}");

                Console.WriteLine($"[Factor] {((CSMBIOSType1)dicSMBIOS[1]).Manufacturer} [ProductName] {((CSMBIOSType1)dicSMBIOS[1]).ProductName} [SKUNumber] {((CSMBIOSType1)dicSMBIOS[1]).SKUNumber}");
                Console.WriteLine($"[Version] {((CSMBIOSType1)dicSMBIOS[1]).Version} [Family] {((CSMBIOSType1)dicSMBIOS[1]).Family} [SerialNum] {((CSMBIOSType1)dicSMBIOS[1]).SerialNum}");

                Console.WriteLine($"[BD Factor] {((CSMBIOSType2)dicSMBIOS[2]).Manufacturer} [BD ProductName] {((CSMBIOSType2)dicSMBIOS[2]).Product} [BD Asset] {((CSMBIOSType2)dicSMBIOS[2]).AssetTag}");
                Console.WriteLine($"[BD Version] {((CSMBIOSType2)dicSMBIOS[2]).Version} [BD Chassis] {((CSMBIOSType2)dicSMBIOS[2]).LocationinChassis} [BD SerialNum] {((CSMBIOSType2)dicSMBIOS[2]).SerialNumber}");

                Console.WriteLine($"[Chas Factor] {((CSMBIOSType3)dicSMBIOS[3]).Manufacturer} [Chas Version] {((CSMBIOSType3)dicSMBIOS[3]).Version} [Chas Asset] {((CSMBIOSType3)dicSMBIOS[3]).AssetTag}");
                Console.WriteLine($"[Chas SKUNumber] {((CSMBIOSType3)dicSMBIOS[3]).SKUNumber} [Chas SerialNum] {((CSMBIOSType3)dicSMBIOS[3]).SerialNumber}");

                Console.WriteLine($"[Proc Socket] {((CSMBIOSType4)dicSMBIOS[4]).SocketDesignation} [Proc Factor] {((CSMBIOSType4)dicSMBIOS[4]).ProcessorManufacturer} [Proc Version] {((CSMBIOSType4)dicSMBIOS[4]).ProcessorVersion}");
                Console.WriteLine($"[Proc PartNumber] {((CSMBIOSType4)dicSMBIOS[4]).PartNumber} [Proc AssetTag] {((CSMBIOSType4)dicSMBIOS[4]).AssetTag} [Proc SerialNum] {((CSMBIOSType4)dicSMBIOS[4]).SerialNumber}");
                rev = true;
            }
#endif
            return rev;
        }

        

        
    }
}
