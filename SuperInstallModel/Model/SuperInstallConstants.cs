namespace SuperInstallModel.Model
{
    class SuperInstallConstants
    {
        public const string LegacyOmenSWKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{B13CB0A1-4411-404C-A7DB-BB1441B089EC}";

        public const string FlagPath = @"C:\system.sav\flags\OCCSuperinstaller.flg";

        public const string LogFileName = @"OCCSuperInstaller.log";
        public const string SuperInstallFlag = @"C:\SYSTEM.SAV\flags\OCCSuperinstaller.flg";
        public const string PlatformInstallData = @"PlatformInstall.data";

        /// <summary>
        /// The file name of JSON file
        /// </summary>
        public const string SuperInstallJSONFile = "SPInstallInfo.json";

        /// <summary>
        /// The BIOS Header
        /// </summary>
        public const string BIOSFormalHeader = "F";
        public const string BIOSBetaHeader = "B";

        public const string CmdShutdown = "shutdown";
        public const string CmdShutdownArgs = "/r /t 1";

        public const string CmdTasksSchedule = "schtasks";
        public const string SuperInstallerTaskName = "OMEN Command Center Super Installer";

        //Test PR modify

        #region MyRegion
        //Root path
        public const string WMICIMRoot = "ROOT\\CIMV2";
        public const string WMIRoot = "ROOT\\WMI";
        public const string WMIHPRoot = "ROOT\\HP\\InstrumentedBIOS";
        //Query strings
        public const string WMIBIOSQueryStry = "SELECT * FROM Win32_BIOS";
        public const string WMISystemQueryStry = "SELECT * FROM Win32_ComputerSystem";
        public const string WMICPUQueryStry = "SELECT * FROM Win32_Processor";
        public const string WMIQueryStr = "SELECT * FROM Win32_OperatingSystem";
        public const string WMIHPQueryStr = "SELECT * FROM HP_BIOSString";
        public const string WMISMBIOSQueryStr = "SELECT * FROM MSSmBios_RawSMBiosTables";
        //Property string for Query
        public const string WinCaption = "Caption";
        public const string WinVersion = "Version";
        public const string WinPrimary = "Primary";
        public const string WinArchitecture = "OSArchitecture";
        public const string WinManufacturer = "Manufacturer";
        public const string WinName = "Name";
        public const string WinValue = "Value";
        public const string WinSMBIOS = "SMBiosData";
        #endregion
    }
}
