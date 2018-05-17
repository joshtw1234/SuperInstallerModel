﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SuperInstallModel.Model
{
    class SuperInstallModel
    {
        PlatformInfo platfomInfo;
        private void ModelLogger(string msg)
        {
            Win32Dlls.Logger(SuperInstallConstants.LogPath, msg);
            Console.WriteLine(msg);
        }
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

            Console.WriteLine($"[Windows Name] {Win32Dlls.GetManageObjValue(SuperInstallConstants.WMICIMRoot, SuperInstallConstants.WMIQueryStr, SuperInstallConstants.WinCaption)}");
            Console.WriteLine($"[Windows Version] {Win32Dlls.GetManageObjValue(SuperInstallConstants.WMICIMRoot, SuperInstallConstants.WMIQueryStr, SuperInstallConstants.WinVersion)}");
            //Console.WriteLine($"[Primary] {Win32Dlls.GetManageObjValue(SuperInstallConstants.WMICIMRoot, SuperInstallConstants.WMIQueryStr, SuperInstallConstants.WinPrimary)}");
            Console.WriteLine($"[Windows OS] {Win32Dlls.GetManageObjValue(SuperInstallConstants.WMICIMRoot, SuperInstallConstants.WMIQueryStr, SuperInstallConstants.WinArchitecture)}");
            Console.WriteLine($"[System Vendor] {Win32Dlls.GetManageObjValue(SuperInstallConstants.WMICIMRoot, SuperInstallConstants.WMISystemQueryStry, SuperInstallConstants.WinManufacturer)}");
            CSMBIOSType2 smbios2 = new CSMBIOSType2();
            CSMBIOSType0 smbios0 = new CSMBIOSType0();
            Console.WriteLine($"[System ID] {smbios2.Product}");
            string biosVer = Win32Dlls.GetManageObjValue(SuperInstallConstants.WMICIMRoot, SuperInstallConstants.WMIBIOSQueryStry, SuperInstallConstants.WinCaption).ToString();
            Console.WriteLine($"[BIOS Version] {biosVer}");
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
            string logMsg = "IsSupporPlatform false";
            string jsonStr = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), SuperInstallConstants.SuperInstallJSONFile));
            var resultSPInstall = JsonConvert.DeserializeObject<SuperInstallInfo>(jsonStr);
            platfomInfo = resultSPInstall.PlatformLst.FirstOrDefault(x => x.PlatformSSID.Equals(smbios2.Product));
            if (null != platfomInfo)
            {
                logMsg = "IsSupporPlatform true";
            }
            ModelLogger(logMsg);
            //
            string cmpBIOSstr = SuperInstallConstants.BIOSFormalHeader;
            int compareVersion = int.Parse(platfomInfo.SWFMinVer.Split('.').Last());
            if (resultSPInstall.IsBetaMode)
            {
                cmpBIOSstr = SuperInstallConstants.BIOSBetaHeader;
                compareVersion = int.Parse(platfomInfo.SWBMinVer.Split('.').Last());
            }
            //Check BIOS install stage
            logMsg = "BIOS False";
            string[] curtBiosInfo = biosVer.Split('.');
            if (curtBiosInfo.Count() == 2 && curtBiosInfo.First().Equals(cmpBIOSstr) &&
                int.Parse(curtBiosInfo.Last()) >= compareVersion)
            {
                platfomInfo.SWInstallStates = InstallStage.Done;
                logMsg = "BIOS True";
            }
            ModelLogger($"[{logMsg}] BIOS ver {biosVer} {platfomInfo.SWInstallStates}");
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
            string CmdCreateTaskArgs = $"/Create /ru Users /f /sc ONLOGON /TN {SuperInstallConstants.SuperInstallerTaskName} /tr \"{Path.Combine(Directory.GetCurrentDirectory(), AppDomain.CurrentDomain.FriendlyName)}\" /RL HIGHEST";
            string CmdRunTaskArgs = $"/Run /TN \"{SuperInstallConstants.SuperInstallerTaskName}\"";
            if (!GetQueryTaskSchedulerResult(SuperInstallConstants.SuperInstallerTaskName))
            {
                Win32Dlls.RunProcess(SuperInstallConstants.CmdTasksSchedule, CmdCreateTaskArgs);
                Win32Dlls.RunProcess(SuperInstallConstants.CmdTasksSchedule, CmdRunTaskArgs);
            }
            return rev;
        }

        private bool GetQueryTaskSchedulerResult(string taskScheduleName)
        {
            bool rev = false;
            string CmdQueryTaskArgs = $"/query /TN \"{taskScheduleName}\"";
            int qRev = Win32Dlls.RunProcess(SuperInstallConstants.CmdTasksSchedule, CmdQueryTaskArgs);
            if (qRev == 0)
            {
                rev = true;
            }
            return rev;
        }

        public void SetStartInstall()
        {
            string installLog = $"BIOS Install {platfomInfo.SWInstallStates}";
            if (platfomInfo.SWInstallStates == InstallStage.None)
            {
                Win32Dlls.CreateShortcut("OCCSPInstall", Environment.GetFolderPath(Environment.SpecialFolder.Startup), System.IO.Path.Combine(Environment.CurrentDirectory, AppDomain.CurrentDomain.FriendlyName), "Test", string.Empty);
                installLog = "BIOS Start Install";
                ModelLogger(installLog);
                int rev = Win32Dlls.RunProcess(platfomInfo.SWEXEName, platfomInfo.SWEXECmd);
                if (rev != 0)
                {
                    installLog = $"{installLog} {rev}";
                }
                ModelLogger(installLog);
                rev = Win32Dlls.RunProcess(SuperInstallConstants.CmdShutdown, SuperInstallConstants.CmdShutdownArgs);
                installLog = $"Shutdown {rev}";
                ModelLogger(installLog);
                return;
            }
            ModelLogger(installLog);
            foreach(SWInfo sw in platfomInfo.SWList)
            {
                installLog = $"install {sw.SWEXEName} {sw.SWInstallStates}";
                if (sw.SWInstallStates == InstallStage.None)
                {
                    installLog = $"install {sw.SWEXEName}";
                    ModelLogger(installLog);
                    int rev = Win32Dlls.RunProcess(sw.SWEXEName, (sw.SWEXECmd == null? string.Empty : sw.SWEXECmd));
                    installLog = $"Install rev {rev}";
                    sw.SWInstallStates = InstallStage.Done;
                }
                ModelLogger(installLog);
            }

            string output = JsonConvert.SerializeObject(platfomInfo);
            File.WriteAllText(@".\OCCInstallResult.log", output);

            string scPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "OCCSPInstall.lnk");
            installLog = $"Delete {scPath} false";
            if (File.Exists(scPath))
            {
                installLog = $"Delete {scPath} true";
                File.Delete(scPath);
            }
            ModelLogger(installLog);
            string CmdDelTaskArgs = $"/delete /TN \"{SuperInstallConstants.SuperInstallerTaskName}\" /F";
            if (GetQueryTaskSchedulerResult(SuperInstallConstants.SuperInstallerTaskName))
            {
                Win32Dlls.RunProcess(SuperInstallConstants.CmdTasksSchedule, CmdDelTaskArgs);
            }
        }




    }
}
