﻿using Newtonsoft.Json;
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

            Console.WriteLine($"[Caption] {Win32Dlls.GetManageObjValue(SuperInstallConstants.WMIRoot, SuperInstallConstants.WMIQueryStr, SuperInstallConstants.WinCaption)}");
            Console.WriteLine($"[Version] {Win32Dlls.GetManageObjValue(SuperInstallConstants.WMIRoot, SuperInstallConstants.WMIQueryStr, SuperInstallConstants.WinVersion)}");
            Console.WriteLine($"[Primary] {Win32Dlls.GetManageObjValue(SuperInstallConstants.WMIRoot, SuperInstallConstants.WMIQueryStr, SuperInstallConstants.WinPrimary)}");
            Console.WriteLine($"[OSArchitecture] {Win32Dlls.GetManageObjValue(SuperInstallConstants.WMIRoot, SuperInstallConstants.WMIQueryStr, SuperInstallConstants.WinArchitecture)}");
            Console.WriteLine($"[BIOS Version] {Win32Dlls.GetManageObjValue(SuperInstallConstants.WMIRoot, SuperInstallConstants.WMIBIOSQueryStry, SuperInstallConstants.WinCaption)}");
            Console.WriteLine($"[System Vendor] {Win32Dlls.GetManageObjValue(SuperInstallConstants.WMIRoot, SuperInstallConstants.WMISystemQueryStry, SuperInstallConstants.WinManufacturer)}");
            Console.WriteLine($"[CPU Name] {Win32Dlls.GetManageObjValue(SuperInstallConstants.WMIRoot, SuperInstallConstants.WMICPUQueryStry, SuperInstallConstants.WinName)}");
            Console.WriteLine($"[System ID] {Win32Dlls.GetManageObjValue(SuperInstallConstants.WMIHPRoot, SuperInstallConstants.WMIHPQueryStr, SuperInstallConstants.WinValue)}");

            Win32Dlls.SYSTEM_POWER_STATUS SysPower = new Win32Dlls.SYSTEM_POWER_STATUS();
            Win32Dlls.GetSystemPowerStatus(ref SysPower);
            string pwMsg = "Power code on";
            if (!Convert.ToBoolean(SysPower.ACLineStatus))
            {
                pwMsg = "Power code off";
            }
            Console.WriteLine($"{pwMsg}");
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
