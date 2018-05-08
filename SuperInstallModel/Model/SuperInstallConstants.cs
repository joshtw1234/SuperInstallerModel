using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperInstallModel.Model
{
    class SuperInstallConstants
    {
        public const string LogPath = @"C:\SWSetup\SuperInstallerLog.log";

        #region MyRegion
        //Root path
        public const string WMIRoot = "ROOT\\CIMV2";
        //Query strings
        public const string WMIBIOSQueryStry = "SELECT * FROM Win32_BIOS";
        public const string WMISystemQueryStry = "SELECT * FROM Win32_ComputerSystem";
        public const string WMICPUQueryStry = "SELECT * FROM Win32_Processor";
        public const string WMIQueryStr = "SELECT * FROM Win32_OperatingSystem";
        //Property string for Query
        public const string WinCaption = "Caption";
        public const string WinVersion = "Version";
        public const string WinPrimary = "Primary";
        public const string WinArchitecture = "OSArchitecture";
        public const string WinManufacturer = "Manufacturer";
        public const string WinName = "Name";
        #endregion
    }
}
