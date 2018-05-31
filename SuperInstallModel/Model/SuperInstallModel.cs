using Microsoft.Win32;
using Newtonsoft.Json;
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
            Win32Dlls.Logger(GetPhysicalPath(SuperInstallConstants.LogFileName), msg);
            Console.WriteLine(msg);
        }

        private string GetPhysicalPath(string fileSubPath)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileSubPath);
        }

        private void GetCapDriverStates(string devRootKey)
        {
            const string RegDriverKey = "Driver";
            RegistryKey reg = Registry.LocalMachine.OpenSubKey(devRootKey);
            if (reg != null)
            {
                string[] subKeys = reg.GetSubKeyNames();
                reg.Close();
                ModelLogger($"Get {devRootKey} {subKeys.Length} name {subKeys[0]}");
                devRootKey = Path.Combine(devRootKey, subKeys[0]);
                RegistryKey subReg = Registry.LocalMachine.OpenSubKey(devRootKey);
                if (subReg != null)
                {
                    subKeys = subReg.GetValueNames();
                    string msg = "Failed";
                    if (subKeys.Contains(RegDriverKey))
                    {
                        msg = "Sys Pass";
                        if (devRootKey.Contains("HPIC0003"))
                        {
                            platfomInfo.SWList.First().SWInstallStates = InstallStage.Done;
                        }
                        else
                        {
                            msg = "OMEN Pass";
                            platfomInfo.SWList[1].SWInstallStates = InstallStage.Done;
                        }
                    }
                    ModelLogger($"Get {devRootKey} {subKeys.Length} Driver {msg}");
                    subReg.Close();
                }
            }
        }
        public bool Initialize()
        {
            bool rev = false;
            string logPath = GetPhysicalPath(SuperInstallConstants.LogFileName);
            FileInfo fi = new FileInfo(logPath);
            if (!fi.Directory.Exists)
            {
                Directory.CreateDirectory(fi.Directory.FullName);
            }
            if (!File.Exists(logPath))
            {
                FileStream fs = File.Create(logPath);
                fs.Close();
            }

            if (!File.Exists(SuperInstallConstants.SuperInstallFlag))
            {
                FileStream fs = File.Create(SuperInstallConstants.SuperInstallFlag);
                fs.Close();
            }
            string platformInstallData = GetPhysicalPath(SuperInstallConstants.PlatformInstallData);
            PlatformInfo tmpP = null;
            if (File.Exists(platformInstallData))
            {
                string jsonInstallStr = File.ReadAllText(platformInstallData);
                tmpP = JsonConvert.DeserializeObject<PlatformInfo>(jsonInstallStr);
                if (tmpP.SuperInstallStates == InstallStage.Done)
                {
                    return false;
                }
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
            string jsonStr = File.ReadAllText(GetPhysicalPath(SuperInstallConstants.SuperInstallJSONFile));
            var resultSPInstall = JsonConvert.DeserializeObject<SuperInstallInfo>(jsonStr);
            platfomInfo = resultSPInstall.PlatformLst.FirstOrDefault(x => x.PlatformSSID.Equals(smbios2.Product));
            if (null != platfomInfo)
            {
                logMsg = "IsSupporPlatform true";
            }
            ModelLogger(logMsg);

            if (tmpP != null)
            {
                platfomInfo = tmpP;
            }
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
                ModelLogger($"[{logMsg}] BIOS ver {biosVer} {platfomInfo.SWInstallStates}");
                //Only BIOS installed then check Driver
                GetCapDriverStates(SuperInstallConstants.RegKeySYSInfoCap);
                GetCapDriverStates(SuperInstallConstants.RegKeyOMENCap);
            }
            

            
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

            string CmdCreateTaskArgs = $"/Create /ru Users /f /sc ONLOGON /TN \"{SuperInstallConstants.SuperInstallerTaskName}\" /tr \"{GetPhysicalPath(AppDomain.CurrentDomain.FriendlyName)}\" /RL HIGHEST";
            string CmdRunTaskArgs = $"/Run /TN \"{SuperInstallConstants.SuperInstallerTaskName}\"";
            if (platfomInfo.SWInstallStates == InstallStage.None && !GetQueryTaskSchedulerResult(SuperInstallConstants.SuperInstallerTaskName))
            {
                int revResu = Win32Dlls.RunProcess(SuperInstallConstants.CmdTasksSchedule, CmdCreateTaskArgs);
                ModelLogger($"Create Task Schedule {revResu}");
                revResu = Win32Dlls.RunProcess(SuperInstallConstants.CmdTasksSchedule, CmdRunTaskArgs);
                ModelLogger($"Run Task Schedule {revResu}");
            }
            rev = true;
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
            string output = JsonConvert.SerializeObject(platfomInfo);
            string installLog = $"BIOS Install {platfomInfo.SWInstallStates}";
            if (platfomInfo.SWInstallStates == InstallStage.None)
            {
                installLog = "BIOS Start Install";
                ModelLogger(installLog);
                int rev = Win32Dlls.RunProcess(platfomInfo.SWEXEName, platfomInfo.SWEXECmd);
                if (rev != 0)
                {
                    installLog = $"{installLog} {rev}";
                }
                platfomInfo.SuperInstallStates = InstallStage.Processing;
                File.WriteAllText(GetPhysicalPath(SuperInstallConstants.PlatformInstallData), output);
                ModelLogger(installLog);
                rev = Win32Dlls.RunProcess(SuperInstallConstants.CmdShutdown, SuperInstallConstants.CmdShutdownArgs);
                installLog = $"Shutdown {rev}";
                ModelLogger(installLog);
                return;
            }
            //Start install Apps
            ModelLogger(installLog);
            string cmdPath = string.Empty;
            foreach (SWInfo sw in platfomInfo.SWList)
            {
                cmdPath = GetPhysicalPath(sw.SWEXEName);
                if (!File.Exists(cmdPath))
                {
                    continue;
                }
                installLog = $"install {sw.SWEXEName} {sw.SWInstallStates}";
                if (sw.SWInstallStates == InstallStage.None)
                {
                    installLog = $"install {sw.SWEXEName}";
                    ModelLogger(installLog);
                    int rev = Win32Dlls.RunProcess(sw.SWEXEName, (sw.SWEXECmd == null ? string.Empty : sw.SWEXECmd));
                    installLog = $"Install rev {rev}";
                    sw.SWInstallStates = InstallStage.Done;
                }
                ModelLogger(installLog);
            }
            //Uninstall proc
            SetUninstallProc();
            //
            platfomInfo.SuperInstallStates = InstallStage.Done;
            output = JsonConvert.SerializeObject(platfomInfo);
            File.WriteAllText(GetPhysicalPath(SuperInstallConstants.PlatformInstallData), output);

            ModelLogger(installLog);
            if (platfomInfo.SWInstallStates == InstallStage.Done &&
                null == platfomInfo.SWList.FirstOrDefault(x => x.SWInstallStates == InstallStage.None))
            {
                string CmdDelTaskArgs = $"/delete /TN \"{SuperInstallConstants.SuperInstallerTaskName}\" /F";
                if (GetQueryTaskSchedulerResult(SuperInstallConstants.SuperInstallerTaskName))
                {
                    Win32Dlls.RunProcess(SuperInstallConstants.CmdTasksSchedule, CmdDelTaskArgs);
                    installLog = "Delete Task Schedule";
                    ModelLogger(installLog);
                }
            }
        }

        /// <summary>
        /// Set start to uninstall process.
        /// </summary>
        private void SetUninstallProc()
        {
            //Detect if the previous version of the OMEN CC App is installed
            //The location of the uninstaller in the registry depends on the app's architecture and installer technology. This is different for every app.
            Microsoft.Win32.RegistryKey reg = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry64).OpenSubKey(SuperInstallConstants.LegacyOmenSWKey);
            if (null == reg)
            {
                //Legacy OMEN not found
                return;
            }
            //Get Uninstall string.
            string _uninstallString = (string)reg.GetValue("UninstallString");
            reg.Close();
            ModelLogger(_uninstallString);
            //In this case, the uninstaller takes additional arguments.
            //Since the uninstallString is app-specific, make sure your call to the uninstaller is properly formatted before calling create process
            string[] uninstallArgs = _uninstallString.Split(' ');
            int rev = Win32Dlls.RunProcess(uninstallArgs[0], uninstallArgs[1]);
            if (rev != 0)
            {
                //Uninstallation was unsuccessful - App Developer can choose to block the app here
            }
        }

    }
    
}
            

