using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SuperInstallModel.Model
{
    class SuperInstallModel
    {
        const string WMIRoot = "ROOT\\CIMV2";
        const string WMIQueryStr = "SELECT * FROM Win32_OperatingSystem";
        const string WinCaption = "Caption";
        const string WinVersion = "Version";
        const string WinPrimary = "Primary";
        const string WinArchitecture = "OSArchitecture";
        const string WinManufacturer = "Manufacturer";
        const string WinName = "Name";

        const string WMIBIOSQueryStry = "SELECT * FROM Win32_BIOS";
        const string WMISystemQueryStry = "SELECT * FROM Win32_ComputerSystem";
        const string WMICPUQueryStry = "SELECT * FROM Win32_Processor";
        
        public bool Initialize()
        {
            bool rev = false;
            Console.WriteLine($"[Caption] {Win32Dlls.GetManageObjValue(WMIRoot, WMIQueryStr, WinCaption)}");
            Console.WriteLine($"[Version] {Win32Dlls.GetManageObjValue(WMIRoot, WMIQueryStr, WinVersion)}");
            Console.WriteLine($"[Primary] {Win32Dlls.GetManageObjValue(WMIRoot, WMIQueryStr, WinPrimary)}");
            Console.WriteLine($"[OSArchitecture] {Win32Dlls.GetManageObjValue(WMIRoot, WMIQueryStr, WinArchitecture)}");
            Console.WriteLine($"[BIOS Version] {Win32Dlls.GetManageObjValue(WMIRoot, WMIBIOSQueryStry, WinCaption)}");
            Console.WriteLine($"[System Vendor] {Win32Dlls.GetManageObjValue(WMIRoot, WMISystemQueryStry, WinManufacturer)}");
            Console.WriteLine($"[CPU Name] {Win32Dlls.GetManageObjValue(WMIRoot, WMICPUQueryStry, WinName)}");

            //var resu = Win32Dlls.GetManageObjValue(WMIRoot, WMIBIOSQueryStry, WinCaption);
            Console.WriteLine();
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
            return rev;
        }

        

        
    }
}
