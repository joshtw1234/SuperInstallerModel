using System;
using System.Collections.Generic;

namespace SuperInstallModel.Model
{
    class SuperInstallModel
    {
        

        public bool Initialize()
        {
            bool rev = false;
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
