using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
//using System.Runtime.InteropServices;
namespace OosterhofDesign
{

    public class G_GetVersionsIons
    {
        public static readonly string CncApiHeaderVersion = "CNC V4.03.52";
        public static readonly string WrapperVersion = "1.01";

        public G_GetServer CncServer { get; }

        public G_GetVersionsIons(G_GetServer CNC_SERVER)
        {
            CncServer = CNC_SERVER;

        }



        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern void CncGetAPIVersion(sbyte* Version);
        public virtual void GetApiVersion(out string VERSION)//
        {
            unsafe
            {
                sbyte* temp_version = stackalloc sbyte[(int)CncConstants.CNC_MAX_NAME_LENGTH];
                CncGetAPIVersion(temp_version);
                VERSION = StringConversie.CharArrayToString((IntPtr)temp_version, 0, (int)CncConstants.CNC_MAX_NAME_LENGTH);
            }
        }
        


        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CncRc CncGetServerVersion(sbyte * Version);
        public virtual CncRc GetServerVersion(out string version)//
        {
            unsafe
            {
                sbyte* temp_version = stackalloc sbyte[(int)CncConstants.CNC_MAX_NAME_LENGTH];
                CncRc rc = CncGetServerVersion(temp_version);
                version = StringConversie.CharArrayToString((IntPtr)temp_version, 0, (int)CncConstants.CNC_MAX_NAME_LENGTH);
                CncServer.LastKnowRcState = rc;
                return rc;
            }
        }

        public virtual void GetHeaderVersion(out string version)
        {
            version = CncApiHeaderVersion;
        }
        public virtual CncRc CheckVersionMatch()
        {

            string serverVersion = "";
            string apiVersion = "";
            string headerVersion = "";
            GetHeaderVersion(out headerVersion);
            GetApiVersion(out apiVersion);
            CncRc rc = CncRc.CNC_RC_OK;
            rc = GetServerVersion(out serverVersion);

            if (rc == CncRc.CNC_RC_OK)
            {
                if (serverVersion == apiVersion && serverVersion == headerVersion)
                {
                    rc = CncRc.CNC_RC_ERR_VERSION_MISMATCH;
                }
            }
            CncServer.LastKnowRcState = rc;
            return rc;
        }
    }
    public class G_GetServer
    {
        public const string CncApiDll = "cncapi_extern.dll";

        public CncRc LastKnowRcState { get; internal set; } = CncRc.CNC_RC_ERR_NOT_CONNECTED;

        public string IniFileName { get; set; } = "";

        public G_GetServer(string INI_FILE_NAME)
        {
           
            IniFileName = INI_FILE_NAME;

        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CncRc CncConnectServer(sbyte * iniFileName);

        public virtual CncRc ConnectServer()//
        {
            unsafe
            {
                sbyte* temp_iniFile = stackalloc sbyte[(int)CncConstants.CNC_MAX_PATH] ;
                StringConversie.StringToMaxCharArray(IniFileName, (IntPtr)temp_iniFile, 0, (int)CncConstants.CNC_MAX_PATH);
                CncRc return_result = CncConnectServer(temp_iniFile);
                LastKnowRcState = return_result;
                return return_result;
            }
        }

        [DllImport(G_GetServer.CncApiDll)]
        public static extern CncRc CncDisConnectServer();

        public virtual CncRc DisConnectServer()
        {
            CncRc returnResult = CncDisConnectServer();
            LastKnowRcState = returnResult;
            return returnResult;
        }
    }
    public class G_GetConfigItems
    {
        private CncSystemConfig _CncSystemConfig = null;
        private CncInterpreterConfig _CncInterpreterConfig = null;
        private CncSafetyConfig _CncSafetyConfig = null;
        private CncTrafficLightCfg _CncTrafficLightCfg = null;
        private CncProbingCfg _CncProbingCfg = null;
        private CncIoConfig _CncIoConfig = null;
        private CncI2cgpioCardConfig _CncI2cgpioCardConfig = null;
        private CncJointCfg[] _CncJointCfg = new CncJointCfg[(int)CncConstants.CNC_MAX_JOINTS];
        private CncSpindleConfig[] _CncSpindleConfig = new CncSpindleConfig[(int)CncConstants.CNC_MAX_SPINDLES];
        private CncHandwheelCfg _CncHandwheelCfg = null;
        private CncFeedspeedCfg _CncFeedspeedCfg = null;
        private CncTrajectoryCfg _CncTrajectoryCfg = null;
        private CncKinCfg _CncKinCfg = null;
        private CncVacuumbedConfig _CncVacuumbedConfig = null;
        private CncUiCfg _CncUiCfg = null;
        private CncCameraConfig _CncCameraConfig = null;
        private CncThcCfg _CncThcCfg = null;
        private CncServiceCfg _CncServiceCfg = null;
        private Cnc3dprintingConfig _Cnc3dprintingConfig = null;
        private CncUioConfig _CncUioConfig = null;
        private CncIoPortSts[] _CncIoPortSts = null;
        private CncGpioPortSts[,] _CncGpioPortSts = null;
        private FieldInfo[] _CncGpioId_CncGpioPortSts = null;
        private FieldInfo[] _CncIoId_CncIoPortSts = null;
        public G_GetServer CncServer { get; }
        public G_GetConfigItems(G_GetServer CNC_SERVER)
        {
            CncServer = CNC_SERVER;
            Type cncIoPortSts = typeof(CncIoPortSts);
            _CncIoId_CncIoPortSts = cncIoPortSts.GetFields();

            _CncIoPortSts = new CncIoPortSts[_CncIoId_CncIoPortSts.Length];

            Type cncGpioId = typeof(CncGpioId);
            _CncGpioId_CncGpioPortSts = cncGpioId.GetFields();
            _CncGpioPortSts = new CncGpioPortSts[(int)CncConstants.CNC_MAX_GPIOCARD_CARDS, _CncGpioId_CncGpioPortSts.Length];

        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern sbyte * CncGetSetupPassword();
        public unsafe virtual string GetSetupPassword()//
        {
            return StringConversie.CharArrayToString((IntPtr)CncGetSetupPassword(), 0, (int)Offst_CncUiCfg.setupPasswordRankL_1);
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CncRc CncSetSetupPassword(sbyte * newPassword);
        public virtual CncRc SetSetupPassword(string newPassword)//
        {
            unsafe
            {
                sbyte* temp_newPassword = stackalloc sbyte[(int)Offst_CncUiCfg.setupPasswordRankL_1];
                CncRc return_result = CncSetSetupPassword(temp_newPassword);
                CncServer.LastKnowRcState = return_result;
                return return_result;
            }
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CNC_SYSTEM_CONFIG * CncGetSystemConfig();
        public unsafe virtual CncSystemConfig GetSystemConfig()
        {
            if (_CncSystemConfig == null || _CncSystemConfig.IsDisposed == true)
            {
                _CncSystemConfig = new CncSystemConfig((IntPtr)CncGetSystemConfig());
            }
            else
            {
                _CncSystemConfig.Pointer = (IntPtr)CncGetSystemConfig();
            }
            return _CncSystemConfig;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CNC_INTERPRETER_CONFIG * CncGetInterpreterConfig();
        public unsafe virtual CncInterpreterConfig GetInterpreterConfig()
        {
            if (_CncInterpreterConfig == null || _CncInterpreterConfig.IsDisposed == true)
            {
                _CncInterpreterConfig = new CncInterpreterConfig((IntPtr)CncGetInterpreterConfig());
            }
            else
            {
                _CncInterpreterConfig.Pointer = (IntPtr)CncGetInterpreterConfig();
            }
            return _CncInterpreterConfig;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CNC_SAFETY_CONFIG * CncGetSafetyConfig();
        public unsafe virtual CncSafetyConfig GetSafetyConfig()
        {
            if (_CncSafetyConfig == null || _CncSafetyConfig.IsDisposed == true)
            {
                _CncSafetyConfig = new CncSafetyConfig((IntPtr)CncGetSafetyConfig());
            }
            else
            {
                _CncSafetyConfig.Pointer = (IntPtr)CncGetSafetyConfig();
            }
            return _CncSafetyConfig;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CNC_TRAFFIC_LIGHT_CFG * CncGetTrafficLightConfig();
        public unsafe virtual CncTrafficLightCfg GetTrafficLightConfig()
        {
            if (_CncTrafficLightCfg == null || _CncTrafficLightCfg.IsDisposed == true)
            {
                _CncTrafficLightCfg = new CncTrafficLightCfg((IntPtr)CncGetTrafficLightConfig());
            }
            else
            {
                _CncTrafficLightCfg.Pointer = (IntPtr)CncGetTrafficLightConfig();
            }
            return _CncTrafficLightCfg;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CNC_PROBING_CFG * CncGetProbingConfig();
        public unsafe virtual CncProbingCfg GetProbingConfig()
        {
            if (_CncProbingCfg == null || _CncProbingCfg.IsDisposed == true)
            {
                _CncProbingCfg = new CncProbingCfg((IntPtr)CncGetProbingConfig());
            }
            else
            {
                _CncProbingCfg.Pointer = (IntPtr)CncGetProbingConfig();
            }

            return _CncProbingCfg;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CNC_IO_CONFIG * CncGetIOConfig();
        public unsafe virtual CncIoConfig GetIOConfig()
        {
            if (_CncIoConfig == null || _CncIoConfig.IsDisposed == true)
            {
                _CncIoConfig = new CncIoConfig((IntPtr)CncGetIOConfig());
            }
            else
            {
                _CncIoConfig.Pointer = (IntPtr)CncGetIOConfig();
            }

            return _CncIoConfig;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CNC_I2CGPIO_CARD_CONFIG * CncGetGPIOConfig();
        public unsafe virtual CncI2cgpioCardConfig GetGPIOConfig()
        {
            if (_CncI2cgpioCardConfig == null || _CncI2cgpioCardConfig.IsDisposed == true)
            {
                _CncI2cgpioCardConfig = new CncI2cgpioCardConfig((IntPtr)CncGetGPIOConfig());
            }
            else
            {
                _CncI2cgpioCardConfig.Pointer = (IntPtr)CncGetGPIOConfig();
            }

            return _CncI2cgpioCardConfig;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CNC_JOINT_CFG * CncGetJointConfig(int joint);
        public unsafe virtual CncJointCfg GetJointConfig(int joint)
        {
            if (_CncJointCfg[joint] == null || _CncJointCfg[joint].IsDisposed == true)
            {
                _CncJointCfg[joint] = new CncJointCfg((IntPtr)CncGetJointConfig(joint));
            }
            else
            {
                _CncJointCfg[joint].Pointer = (IntPtr)CncGetJointConfig(joint);
            }

            return _CncJointCfg[joint];
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CNC_SPINDLE_CONFIG * CncGetSpindleConfig(int spindle);
        public unsafe virtual CncSpindleConfig GetSpindleConfig(int spindle)
        {
            if (_CncSpindleConfig[spindle] == null || _CncSpindleConfig[spindle].IsDisposed == true)
            {
                _CncSpindleConfig[spindle] = new CncSpindleConfig((IntPtr)CncGetSpindleConfig(spindle));
            }
            else
            {
                _CncSpindleConfig[spindle].Pointer = (IntPtr)CncGetSpindleConfig(spindle);
            }

            return _CncSpindleConfig[spindle];
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CNC_FEEDSPEED_CFG * CncGetFeedSpeedConfig();
        public unsafe virtual CncFeedspeedCfg GetFeedSpeedConfig()
        {
            if(_CncFeedspeedCfg == null || _CncFeedspeedCfg.IsDisposed == true)
            {
                _CncFeedspeedCfg = new CncFeedspeedCfg((IntPtr)CncGetFeedSpeedConfig());
            }
            else
            {
                _CncFeedspeedCfg.Pointer = (IntPtr)CncGetFeedSpeedConfig();
            }
            return _CncFeedspeedCfg;
        }

        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CNC_HANDWHEEL_CFG * CncGetHandwheelConfig();
        public unsafe virtual CncHandwheelCfg GetHandwheelConfig()
        {
            if (_CncHandwheelCfg == null || _CncHandwheelCfg.IsDisposed == true)
            {
                _CncHandwheelCfg = new CncHandwheelCfg((IntPtr)CncGetHandwheelConfig());
            }
            else
            {
                _CncHandwheelCfg.Pointer = (IntPtr)CncGetHandwheelConfig();
            }
            return _CncHandwheelCfg;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CNC_TRAJECTORY_CFG * CncGetTrajectoryConfig();
        public unsafe virtual CncTrajectoryCfg GetTrajectoryConfig()
        {
            if (_CncTrajectoryCfg == null || _CncTrajectoryCfg.IsDisposed == true)
            {
                _CncTrajectoryCfg = new CncTrajectoryCfg((IntPtr)CncGetTrajectoryConfig());
            }
            else
            {
                _CncTrajectoryCfg.Pointer = (IntPtr)CncGetTrajectoryConfig();
            }

            return _CncTrajectoryCfg;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CNC_KIN_CFG * CncGetKinConfig();
        public unsafe virtual CncKinCfg GetKinConfig()
        {
            if (_CncKinCfg == null || _CncKinCfg.IsDisposed == true)
            {
                _CncKinCfg = new CncKinCfg((IntPtr)CncGetKinConfig());
            }
            else
            {
                _CncKinCfg.Pointer = (IntPtr)CncGetKinConfig();
            }

            return _CncKinCfg;
        }
        [DllImport(G_GetServer.CncApiDll,EntryPoint = "_CncGetVacuumConfig@0")]
        public unsafe static extern CNC_VACUUMBED_CONFIG* CncGetVacuumConfig();
        public unsafe virtual CncVacuumbedConfig GetVacuumConfig()
        {
            if(_CncVacuumbedConfig == null || _CncVacuumbedConfig.IsDisposed == true)
            {
                _CncVacuumbedConfig = new CncVacuumbedConfig((IntPtr)CncGetVacuumConfig());
            }
            else
            {
                _CncVacuumbedConfig.Pointer = (IntPtr)CncGetVacuumConfig();
            }

            return _CncVacuumbedConfig;
        }


        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CNC_UI_CFG * CncGetUIConfig();
        public unsafe virtual CncUiCfg GetUIConfig()
        {
            if (_CncUiCfg == null || _CncUiCfg.IsDisposed == true)
            {
                _CncUiCfg = new CncUiCfg((IntPtr)CncGetUIConfig());
            }
            else
            {
                _CncUiCfg.Pointer = (IntPtr)CncGetUIConfig();
            }

            return _CncUiCfg;
        }
        


        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CNC_CAMERA_CONFIG * CncGetCameraConfig();
        public unsafe virtual CncCameraConfig GetCameraConfig()
        {
            if (_CncCameraConfig == null || _CncCameraConfig.IsDisposed == true)
            {
                _CncCameraConfig = new CncCameraConfig((IntPtr)CncGetCameraConfig());
            }
            else
            {
                _CncCameraConfig.Pointer = (IntPtr)CncGetCameraConfig();
            }

            return _CncCameraConfig;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CNC_THC_CFG * CncGetTHCConfig();
        public unsafe virtual CncThcCfg GetTHCConfig()
        {
            if (_CncThcCfg == null || _CncThcCfg.IsDisposed == true)
            {
                _CncThcCfg = new CncThcCfg((IntPtr)CncGetTHCConfig());
            }
            else
            {
                _CncThcCfg.Pointer = (IntPtr)CncGetTHCConfig();
            }

            return _CncThcCfg;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CNC_SERVICE_CFG * CncGetServiceConfig();
        public unsafe virtual CncServiceCfg GetServiceConfig()
        {
            if (_CncServiceCfg == null || _CncServiceCfg.IsDisposed == true)
            {
                _CncServiceCfg = new CncServiceCfg((IntPtr)CncGetServiceConfig());
            }
            else
            {
                _CncServiceCfg.Pointer = (IntPtr)CncGetServiceConfig();
            }

            return _CncServiceCfg;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CNC_3DPRINTING_CONFIG * CncGet3DPrintConfig();
        public unsafe virtual Cnc3dprintingConfig Get3DPrintConfig()
        {
            if (_Cnc3dprintingConfig == null || _Cnc3dprintingConfig.IsDisposed == true)
            {
                _Cnc3dprintingConfig = new Cnc3dprintingConfig((IntPtr)CncGet3DPrintConfig());
            }
            else
            {
                _Cnc3dprintingConfig.Pointer = (IntPtr)CncGet3DPrintConfig();
            }

            return _Cnc3dprintingConfig;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CNC_UIO_CONFIG * CncGetUIOConfig();
        public unsafe virtual CncUioConfig GetUIOConfig()
        {
            if (_CncUioConfig == null || _CncUioConfig.IsDisposed == true)
            {
                _CncUioConfig = new CncUioConfig((IntPtr)CncGetUIOConfig());
            }
            else
            {
                _CncUioConfig.Pointer = (IntPtr)CncGetUIOConfig();
            }

            return _CncUioConfig;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CNC_IO_PORT_STS * CncGetIOStatus(CncIoId ioID);
        public unsafe virtual CncIoPortSts GetIOStatus(CncIoId ioID)
        {
            int index = -1;
            for (int i = 0; i < _CncIoId_CncIoPortSts.Length; i++)
            {
                CncIoId value = (CncIoId)_CncIoId_CncIoPortSts[i].GetValue(null);
                if (value == ioID)
                {
                    index = i;
                    break;
                }
            }

            if (_CncIoPortSts[index] == null || _CncIoPortSts[index].IsDisposed == true)
            {
                _CncIoPortSts[index] = new CncIoPortSts((IntPtr)CncGetIOStatus(ioID));
            }
            else
            {
                _CncIoPortSts[index].Pointer = (IntPtr)CncGetIOStatus(ioID);
            }
            return _CncIoPortSts[index];
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CNC_GPIO_PORT_STS* CncGetGPIOStatus(int cardNr, CncGpioId ioID);
        public unsafe virtual CncGpioPortSts GetGPIOStatus(int cardNr, CncGpioId ioID)
        {
            int indexCard = cardNr;
            int indexioID = -1;

            for(int i =0;i< _CncGpioId_CncGpioPortSts.Length;i++)
            {
                CncGpioId value = (CncGpioId)_CncGpioId_CncGpioPortSts[i].GetValue(null);
                if (value == ioID)
                {
                    indexioID = i;
                    break;
                }
            }


            if(_CncGpioPortSts[indexCard, indexioID] == null || _CncGpioPortSts[indexCard, indexioID].IsDisposed == true)
            {
                _CncGpioPortSts[indexCard, indexioID] = new CncGpioPortSts((IntPtr)CncGetGPIOStatus(cardNr, ioID));
            }
            else
            {
                _CncGpioPortSts[indexCard, indexioID].Pointer = (IntPtr)CncGetGPIOStatus(cardNr, ioID);
            }
            return _CncGpioPortSts[indexCard, indexioID];
        }
        [DllImport(G_GetServer.CncApiDll)]
        public  static extern CncRc CncStoreIniFile(int saveFixtures);
        public virtual CncRc StoreIniFile(int saveFixtures)
        {
            CncRc return_result = CncStoreIniFile(saveFixtures);
            CncServer.LastKnowRcState = return_result;

            return return_result;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern CncRc CncReInitialize();
        public virtual CncRc ReInitialize()
        {
            CncRc return_result = CncReInitialize();
            CncServer.LastKnowRcState = return_result;

            return return_result;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CncRc CncGetMacroFileName(sbyte * name);
        public virtual CncRc GetMacroFileName(out string name)//
        {
            unsafe
            {
                sbyte* temp_name = stackalloc sbyte[(int)Offst_CncInterpreterConfig.macroFileNameRankL_1];
                CncRc return_result = CncGetMacroFileName(temp_name);
                name = StringConversie.CharArrayToString((IntPtr)temp_name, 0, (int)Offst_CncInterpreterConfig.macroFileNameRankL_1);
                CncServer.LastKnowRcState = return_result;
                return return_result;
            }
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CncRc CncGetUserMacroFileName(sbyte * name);
        public virtual CncRc GetUserMacroFileName(out string name)//
        {
            unsafe
            {
                sbyte* temp_name = stackalloc sbyte[(int)Offst_CncInterpreterConfig.userMacroFileNameRankL_1];
                CncRc return_result = CncGetUserMacroFileName(temp_name);
                name = StringConversie.CharArrayToString((IntPtr)temp_name, 0, (int)Offst_CncInterpreterConfig.userMacroFileNameRankL_1);
                CncServer.LastKnowRcState = return_result;
                return return_result;
            }
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CncRc CncSetMacroFileName(sbyte * name);
        public virtual CncRc SetMacroFileName(string name)//
        {
            unsafe
            {
                sbyte* temp_name = stackalloc sbyte[(int)Offst_CncInterpreterConfig.macroFileNameRankL_1];
                StringConversie.StringToMaxCharArray(name, (IntPtr)temp_name, 0, (int)Offst_CncInterpreterConfig.macroFileNameRankL_1);
                CncRc return_result = CncSetMacroFileName(temp_name);
                CncServer.LastKnowRcState = return_result;
                return return_result;
            }

        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CncRc CncSetUserMacroFileName(sbyte * name);//
        public virtual CncRc SetUserMacroFileName(string name)
        {
            unsafe
            {
                sbyte* temp_name = stackalloc sbyte[(int)Offst_CncInterpreterConfig.userMacroFileNameRankL_1];
                StringConversie.StringToMaxCharArray(name, (IntPtr)temp_name, 0, (int)Offst_CncInterpreterConfig.userMacroFileNameRankL_1);
                CncRc return_result = CncSetUserMacroFileName(temp_name);
                CncServer.LastKnowRcState = return_result;
                return return_result;
            }
        }
    }
    public class G_GetConfigControllerCpu
    {
        private CncJointSts _CncJointSts = null;
        public G_GetServer CncServer { get; }

        public G_GetConfigControllerCpu(G_GetServer CNC_SERVER)
        {
            CncServer = CNC_SERVER;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern sbyte * CncGetControllerFirmwareVersion();
        public unsafe virtual string GetControllerFirmwareVersion()//
        {
            return StringConversie.CharArrayToString((IntPtr)CncGetControllerFirmwareVersion(), 0, (int)CncConstants.CNC_MAX_NAME_LENGTH);
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CncRc CncGetControllerSerialNumber(byte * serial);
        public virtual CncRc GetControllerSerialNumber(out string serial)
        {
            unsafe
            {
                int serialMaxLength = 6;
                byte* temp_serial = stackalloc byte[serialMaxLength];
                CncRc return_result = CncGetControllerSerialNumber(temp_serial);
                serial = StringConversie.UCharArrayToString((IntPtr)temp_serial, 0, serialMaxLength);

                if(serial == "" || serial == null)
                {
                    for(int i =0;i< serialMaxLength;i++)
                    {
                        serial = serial + 0;
                    }
                }
                return return_result;
            }
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern int CncGetControllerNumberOfFrequencyItems();
        public virtual int GetControllerNumberOfFrequencyItems()
        {
            return CncGetControllerNumberOfFrequencyItems();
        }

        [DllImport(G_GetServer.CncApiDll)]
        public static extern double CncGetControllerFrequencyItem(uint index);
        public double GetControllerFrequencyItem(uint index)
        {
            return CncGetControllerFrequencyItem(index);
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern int CncGetControllerConnectionNumberOfItems();
        public int GetControllerConnectionNumberOfItems()
        {
            return CncGetControllerConnectionNumberOfItems();
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern sbyte * CncGetControllerConnectionItem(int itemNumber);
        public unsafe string GetControllerConnectionItem(int itemNumber)//
        {
            return StringConversie.CharArrayToString((IntPtr)CncGetControllerConnectionItem(itemNumber), 0, (int)CncConstants.CNC_COMMPORT_NAME_LEN);
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern void CncGetNrOfAxesOnController(int* maxNrOfAxes, int* availableNrOfAxes);
        public virtual void GetNrOfAxesOnController(out int maxNrOfAxes, out int availableNrOfAxes)
        {
            unsafe
            {
                int* temp_maxNrOfAxes = stackalloc int[1];
                int* temp_availableNrOfAxes = stackalloc int[1];
                CncGetNrOfAxesOnController(temp_maxNrOfAxes, temp_availableNrOfAxes);
                maxNrOfAxes = temp_maxNrOfAxes[0];
                availableNrOfAxes = temp_availableNrOfAxes[0];
            }


        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern int CncGetAxisIsConfigured(int axis, bool includingSlaves);
        public virtual int GetAxisIsConfigured(int axis, bool includingSlaves)
        {
            return CncGetAxisIsConfigured(axis, includingSlaves);
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern int CncGetFirmwareHasOptions();
        public virtual int GetFirmwareHasOptions()
        {
            return CncGetFirmwareHasOptions();
        }
        [DllImport(G_GetServer.CncApiDll)]//
        public unsafe static extern CncRc CncGetActiveOptions(sbyte * actCustomerName,
        int* actNumberOfAxes,
        uint* actCPUEnabled,
        uint* actGPIOAVXEnabled,
        uint* actGPIOEDIEnabled,
        uint* actWolfcutCameraEnabled,
        uint* actTURNMACRO,
        uint* actXHCPendant,
        uint* actLimitedSoftwareEnabled);
        public virtual CncRc GetActiveOptions(out string actCustomerName,
       out int actNumberOfAxes,
       out uint actCPUEnabled,
       out uint actGPIOAVXEnabled,
       out uint actGPIOEDIEnabled,
       out uint actWolfcutCameraEnabled,
       out uint actTURNMACRO,
       out uint actXHCPendant,
       out uint actLimitedSoftwareEnabled)
        {
            unsafe
            {
                sbyte* temp_actCustomerName = stackalloc sbyte[(int)CncConstants.CNC_MAX_CUSTOMER_NAME ];
                int temp_actNumberOfAxes = 0;
                uint temp_actCPUEnabled = 0;
                uint temp_actGPIOAVXEnabled = 0;
                uint temp_actGPIOEDIEnabled = 0;
                uint temp_actWolfcutCameraEnabled = 0;
                uint temp_actTURNMACRO = 0;
                uint temp_actXHCPendant = 0;
                uint temp_actLimitedSoftwareEnabled = 0;


                CncRc return_result = CncGetActiveOptions(temp_actCustomerName,
                    &temp_actNumberOfAxes,
                    &temp_actCPUEnabled,
                    &temp_actGPIOAVXEnabled,
                    &temp_actGPIOEDIEnabled,
                    &temp_actWolfcutCameraEnabled,
                    &temp_actTURNMACRO,
                    &temp_actXHCPendant,
                    &temp_actLimitedSoftwareEnabled);
                actCustomerName = StringConversie.CharArrayToString((IntPtr)temp_actCustomerName, 0, (int)CncConstants.CNC_MAX_CUSTOMER_NAME);
                actNumberOfAxes = temp_actNumberOfAxes;
                actCPUEnabled = temp_actCPUEnabled;
                actGPIOAVXEnabled = temp_actGPIOAVXEnabled;
                actGPIOEDIEnabled = temp_actGPIOEDIEnabled;
                actWolfcutCameraEnabled = temp_actWolfcutCameraEnabled;
                actTURNMACRO = temp_actTURNMACRO;
                actXHCPendant = temp_actXHCPendant;
                actLimitedSoftwareEnabled = temp_actLimitedSoftwareEnabled;


                CncServer.LastKnowRcState = return_result;
                return return_result;
            }

        }

        [DllImport(G_GetServer.CncApiDll)]//
        public unsafe static extern CncRc CncGetOptionRequestCode(sbyte * newCustomerName,
        int newNumberOfAxes,
        uint newGPIOAVXEnabled,
        uint newGPIOEDIEnabled,
        uint newPLASMAEnabled,
        uint newTURMACROEnabled,
        uint newXHCPendant,
        uint newLimitedSoftwareEnabled,
        sbyte * requestCode);
        public virtual CncRc GetOptionRequestCode(string newCustomerName,
        int newNumberOfAxes,
        uint newGPIOAVXEnabled,
        uint newGPIOEDIEnabled,
        uint newPLASMAEnabled,
        uint newTURMACROEnabled,
        uint newXHCPendant,
        uint newLimitedSoftwareEnabled,
        out string requestCode)
        {
            unsafe
            {
                sbyte* temp_newCustomerName = stackalloc sbyte[(int)CncConstants.CNC_MAX_CUSTOMER_NAME];
                sbyte* temp_requestCode = stackalloc sbyte[256 * (int)Offst_Char.TotalSize];
                StringConversie.StringToMaxCharArray(newCustomerName, (IntPtr)temp_newCustomerName, 0, (int)CncConstants.CNC_MAX_CUSTOMER_NAME);
                CncRc return_result = CncGetOptionRequestCode(temp_newCustomerName,
                newNumberOfAxes,
                newGPIOAVXEnabled,
                newGPIOEDIEnabled,
                newPLASMAEnabled,
                newTURMACROEnabled,
                newXHCPendant,
                newLimitedSoftwareEnabled,
                temp_requestCode);
                requestCode = StringConversie.CharArrayToString((IntPtr)temp_requestCode, 0, 256);
                CncServer.LastKnowRcState = return_result;
                return return_result;
            }
        }

        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CncRc CncGetOptionRequestCodeCurrent(sbyte * requestCode);
        public virtual CncRc GetOptionRequestCodeCurrent(out string requestCode)
        {
            unsafe
            {
                sbyte* temp_requestCode = stackalloc sbyte[256];
                CncRc return_result = CncGetOptionRequestCodeCurrent(temp_requestCode);
                requestCode=StringConversie.CharArrayToString((IntPtr)temp_requestCode,0,256);
                CncServer.LastKnowRcState = return_result;
                return return_result;
            }
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CncRc CncActivateOption(sbyte * activationKey);
        public virtual CncRc ActivateOption(string activationKey)
        {
            unsafe
            {
                sbyte* temp_activationKey = stackalloc sbyte[activationKey.Length];
                CncRc return_result = CncActivateOption(temp_activationKey);
                CncServer.LastKnowRcState = return_result;
                return return_result;
            }
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CNC_JOINT_STS* CncGetJointStatus(int joint);
        public unsafe virtual CncJointSts GetJointStatus(int joint)
        {
            if(_CncJointSts == null || _CncJointSts.IsDisposed == true)
            {
                _CncJointSts = new CncJointSts((IntPtr)CncGetJointStatus(joint));
            }
            else
            {
                _CncJointSts.Pointer = (IntPtr)CncGetJointStatus(joint);
            }


            return _CncJointSts;
        }

    }
    public class G_GetSetToolTableData
    {
        private CncToolData[] _CncToolData = new CncToolData[(int)CncConstants.CNC_MAX_TOOLS];
        public G_GetServer CncServer { get; } = null;


        public G_GetSetToolTableData(G_GetServer CNC_SERVER)
        {
            CncServer = CNC_SERVER;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern CNC_TOOL_DATA CncGetToolData(int index);
        public virtual CncToolData GetToolData(int index)
        {
            if(_CncToolData[index] == null || _CncToolData[index].IsDisposed == true)
            {
                _CncToolData[index] = new CncToolData();

            }
            CNC_TOOL_DATA temp_tooldata = CncGetToolData(index);
            _CncToolData[index].SetStructValue(ref temp_tooldata);
            return _CncToolData[index];
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CncRc CncUpdateToolData(CNC_TOOL_DATA * pTool, int index);
        public unsafe virtual CncRc UpdateToolData(CncToolData pTool, int index)
        {
            CncRc return_result = CncUpdateToolData((CNC_TOOL_DATA*)pTool.Pointer, index);

            CncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern CncRc CncLoadToolTable();
        public virtual CncRc LoadToolTable()
        {
            CncRc return_result = CncLoadToolTable();
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }
    }
    public class G_VariableAccess
    {
        public G_GetServer CncServer { get; } = null;
        public G_VariableAccess(G_GetServer CNC_SERVER)
        {
            CncServer = CNC_SERVER;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern double CncGetVariable(int varIndex);
        public virtual double GetVariable(int varIndex)
        {
            return CncGetVariable(varIndex);
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern void CncSetVariable(int varIndex, double value);
        public virtual void SetVariable(int varIndex, double value)
        {
            CncSetVariable( varIndex,  value);
        }
    }
    public class G_StatusItems
    {
        private CncRunningStatus _CncRunningStatus = null;
        private CncMotionStatus _CncMotionStatus = null;
        private CncControllerStatus _CncControllerStatus = null;
        private CncControllerConfigStatus _CncControllerConfigStatus = null;
        private CncTrafficLightStatus _CncTrafficLightStatus = null;
        private CncJobStatus _CncJobStatus = null;
        private CncTrackingStatus _CncTrackingStatus = null;
        private CncThcStatus _CncThcStatus = null;
        private CncNestingStatus _CncNestingStatus = null;
        private CncKinStatus _CncKinStatus = null;
        private CncSpindleSts _CncSpindleSts = null;
        private CncPauseSts _CncPauseSts = null;
        private CncSearchStatus _CncSearchStatus = null;
        private Cnc3dprintingSts _Cnc3dprintingSts = null;
        private CncCompensationStatus _CncCompensationStatus = null;
        private CncVacuumStatus _CncVacuumStatus = null;
        

        public G_GetServer CncServer { get; } = null;

        public G_StatusItems(G_GetServer CNC_SERVER)
        {
            CncServer = CNC_SERVER;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CNC_RUNNING_STATUS * CncGetRunningStatus();
        public unsafe virtual CncRunningStatus GetRunningStatus()
        {
            if(_CncRunningStatus == null || _CncRunningStatus.IsDisposed == true)
            {
                _CncRunningStatus = new CncRunningStatus((IntPtr)CncGetRunningStatus());
            }
            else
            {
                _CncRunningStatus.Pointer = (IntPtr)CncGetRunningStatus();
            }

            return _CncRunningStatus;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CNC_MOTION_STATUS * CncGetMotionStatus();
        public unsafe virtual CncMotionStatus GetMotionStatus()
        {
            if (_CncMotionStatus == null || _CncMotionStatus.IsDisposed == true)
            {
                _CncMotionStatus = new CncMotionStatus((IntPtr)CncGetMotionStatus());
            }
            else
            {
                _CncMotionStatus.Pointer = (IntPtr)CncGetMotionStatus();
            }

            return _CncMotionStatus;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CNC_CONTROLLER_STATUS* CncGetControllerStatus();
        public unsafe virtual CncControllerStatus GetControllerStatus()
        {
            if (_CncControllerStatus == null || _CncControllerStatus.IsDisposed == true)
            {
                _CncControllerStatus = new CncControllerStatus((IntPtr)CncGetControllerStatus());
            }
            else
            {
                _CncControllerStatus.Pointer = (IntPtr)CncGetControllerStatus();
            }

            return _CncControllerStatus;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CNC_CONTROLLER_CONFIG_STATUS* CncGetControllerConfigStatus();
        public unsafe virtual CncControllerConfigStatus GetControllerConfigStatus()
        {
            if (_CncControllerConfigStatus == null || _CncControllerConfigStatus.IsDisposed == true)
            {
                _CncControllerConfigStatus = new CncControllerConfigStatus((IntPtr)CncGetControllerConfigStatus());
            }
            else
            {
                _CncControllerConfigStatus.Pointer = (IntPtr)CncGetControllerConfigStatus();
            }

            return _CncControllerConfigStatus;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CNC_TRAFFIC_LIGHT_STATUS * CncGetTrafficLightStatus();
        public unsafe virtual CncTrafficLightStatus GetTrafficLightStatus()
        {
            if (_CncTrafficLightStatus == null || _CncTrafficLightStatus.IsDisposed == true)
            {
                _CncTrafficLightStatus = new CncTrafficLightStatus((IntPtr)CncGetTrafficLightStatus());
            }
            else
            {
                _CncTrafficLightStatus.Pointer = (IntPtr)CncGetTrafficLightStatus();
            }

            return _CncTrafficLightStatus;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CNC_JOB_STATUS* CncGetJobStatus();
        public unsafe virtual CncJobStatus GetJobStatus()
        {
            if (_CncJobStatus == null || _CncJobStatus.IsDisposed == true)
            {
                _CncJobStatus = new CncJobStatus((IntPtr)CncGetJobStatus());
            }
            else
            {
                _CncJobStatus.Pointer = (IntPtr)CncGetJobStatus();
            }

            return _CncJobStatus;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CNC_TRACKING_STATUS* CncGetTrackingStatus();
        public unsafe virtual CncTrackingStatus GetTrackingStatus()
        {
            if (_CncTrackingStatus == null || _CncTrackingStatus.IsDisposed == true)
            {
                _CncTrackingStatus = new CncTrackingStatus((IntPtr)CncGetTrackingStatus());
            }
            else
            {
                _CncTrackingStatus.Pointer = (IntPtr)CncGetTrackingStatus();
            }

            return _CncTrackingStatus;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CNC_THC_STATUS * CncGetTHCStatus();
        public unsafe virtual CncThcStatus GetTHCStatus()
        {
            if (_CncThcStatus == null || _CncThcStatus.IsDisposed == true)
            {
                _CncThcStatus = new CncThcStatus((IntPtr)CncGetTHCStatus());
            }
            else
            {
                _CncThcStatus.Pointer = (IntPtr)CncGetTHCStatus();
            }

            return _CncThcStatus;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CNC_NESTING_STATUS* CncGetNestingStatus();
        public unsafe virtual CncNestingStatus GetNestingStatus()
        {
            if (_CncNestingStatus == null || _CncNestingStatus.IsDisposed == true)
            {
                _CncNestingStatus = new CncNestingStatus((IntPtr)CncGetNestingStatus());
            }
            else
            {
                _CncNestingStatus.Pointer = (IntPtr)CncGetNestingStatus();
            }

            return _CncNestingStatus;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CNC_KIN_STATUS* CncGetKinStatus();
        public unsafe virtual CncKinStatus GetKinStatus()
        {
            if (_CncKinStatus == null || _CncKinStatus.IsDisposed == true)
            {
                _CncKinStatus = new CncKinStatus((IntPtr)CncGetKinStatus());
            }
            else
            {
                _CncKinStatus.Pointer = (IntPtr)CncGetKinStatus();
            }

            return _CncKinStatus;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CNC_SPINDLE_STS* CncGetSpindleStatus();
        public unsafe virtual CncSpindleSts GetSpindleStatus()
        {
            if (_CncSpindleSts == null || _CncSpindleSts.IsDisposed == true)
            {
                _CncSpindleSts = new CncSpindleSts((IntPtr)CncGetSpindleStatus());
            }
            else
            {
                _CncSpindleSts.Pointer = (IntPtr)CncGetSpindleStatus();
            }

            return _CncSpindleSts;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CNC_PAUSE_STS* CncGetPauseStatus();
        public unsafe virtual CncPauseSts GetPauseStatus()
        {
            if (_CncPauseSts == null || _CncPauseSts.IsDisposed == true)
            {
                _CncPauseSts = new CncPauseSts((IntPtr)CncGetPauseStatus());
            }
            else
            {
                _CncPauseSts.Pointer = (IntPtr)CncGetPauseStatus();
            }

            return _CncPauseSts;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CNC_SEARCH_STATUS* CncGetSearchStatus();
        public unsafe virtual CncSearchStatus GetSearchStatus()
        {
            if (_CncSearchStatus == null || _CncSearchStatus.IsDisposed == true)
            {
                _CncSearchStatus = new CncSearchStatus((IntPtr)CncGetSearchStatus());
            }
            else
            {
                _CncSearchStatus.Pointer = (IntPtr)CncGetSearchStatus();
            }

            return _CncSearchStatus;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CNC_3DPRINTING_STS* CncGet3DPrintStatus();
        public unsafe virtual Cnc3dprintingSts Get3DPrintStatus()
        {
            if (_Cnc3dprintingSts == null || _Cnc3dprintingSts.IsDisposed == true)
            {
                _Cnc3dprintingSts = new Cnc3dprintingSts((IntPtr)CncGet3DPrintStatus());
            }
            else
            {
                _Cnc3dprintingSts.Pointer = (IntPtr)CncGet3DPrintStatus();
            }

            return _Cnc3dprintingSts;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CNC_COMPENSATION_STATUS* CncGetCompensationStatus();
        public unsafe virtual CncCompensationStatus GetCompensationStatus()
        {
            if (_CncCompensationStatus == null || _CncCompensationStatus.IsDisposed == true)
            {
                _CncCompensationStatus = new CncCompensationStatus((IntPtr)CncGetCompensationStatus());
            }
            else
            {
                _CncCompensationStatus.Pointer = (IntPtr)CncGetCompensationStatus();
            }

            return _CncCompensationStatus;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CNC_VACUUM_STATUS* CncGetVacuumStatus();
        public unsafe virtual CncVacuumStatus GetVacuumStatus()
        {
            if (_CncVacuumStatus == null || _CncVacuumStatus.IsDisposed == true)
            {
                _CncVacuumStatus = new CncVacuumStatus((IntPtr)CncGetVacuumStatus());
            }
            else
            {
                _CncVacuumStatus.Pointer = (IntPtr)CncGetVacuumStatus();
            }

            return _CncVacuumStatus;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern int CncGet10msHeartBeat();
        public virtual int Get10msHeartBeat()
        {
            return CncGet10msHeartBeat();
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern int CncIsServerConnected();
        public virtual int IsServerConnected()
        {
            return CncIsServerConnected();
        }

    }
    public class G_StatusItemsposition
    {
        private CncCartDouble _CncCartDouble = null;
        private CncJointDouble _CncJointDouble_GetWorkPosition = null;
        private CncCartDouble _CncJointDouble_GetMachinePosition = null;
        private CncCartDouble _CncCartDouble_GetActualOriginOffset = null;
        public G_GetServer CncServer { get; } = null;

        public G_StatusItemsposition(G_GetServer CNC_SERVER)
        {
            CncServer = CNC_SERVER;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern CncIeState CncGetState();
        public virtual CncIeState GetState()
        {
            return CncGetState();
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern sbyte * CncGetStateText(CncIeState state);//unkown length cnciestate
        public unsafe virtual string GetStateText(CncIeState state)//
        {
            return StringConversie.CharArrayToString((IntPtr)CncGetStateText(state), 0, Convert.ToString(state).Length);
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern int CncInMillimeterMode();
        public virtual int InMillimeterMode()
        {
            return CncInMillimeterMode();
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern CncPlane CncGetActualPlane();
        public virtual CncPlane GetActualPlane()
        {
            return CncGetActualPlane();

        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern CNC_CART_DOUBLE CncGetWorkPosition();
        public virtual CncCartDouble GetWorkPosition()
        {
            if (_CncCartDouble == null || _CncCartDouble.IsDisposed == true)
            {
                _CncCartDouble = new CncCartDouble();
            }
            CNC_CART_DOUBLE temp_CncCartDouble = CncGetWorkPosition();
            _CncCartDouble.SetStructValue(ref temp_CncCartDouble);

            return _CncCartDouble;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern CNC_JOINT_DOUBLE CncGetMotorPosition();
        public virtual CncJointDouble GetMotorPosition()
        {
            if (_CncJointDouble_GetWorkPosition == null || _CncJointDouble_GetWorkPosition.IsDisposed == true)
            {
                _CncJointDouble_GetWorkPosition = new CncJointDouble();
            }
            CNC_JOINT_DOUBLE temp_CncJointDouble = CncGetMotorPosition();
            _CncJointDouble_GetWorkPosition.SetStructValue(ref temp_CncJointDouble);
            return _CncJointDouble_GetWorkPosition;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern CNC_CART_DOUBLE CncGetMachinePosition();
        public virtual CncCartDouble GetMachinePosition()
        {
            if (_CncJointDouble_GetMachinePosition == null || _CncJointDouble_GetMachinePosition.IsDisposed == true)
            {
                _CncJointDouble_GetMachinePosition = new CncCartDouble();
            }
            CNC_CART_DOUBLE temp__CncJointDouble = CncGetMachinePosition();
            _CncJointDouble_GetMachinePosition.SetStructValue(ref temp__CncJointDouble);
            return _CncJointDouble_GetMachinePosition;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern void CncGetMachineZeroWorkPoint(CNC_CART_DOUBLE* pos, int* rotationActive);
        public virtual void GetMachineZeroWorkPoint(CncCartDouble pos, out int rotationActive)
        {
            unsafe
            {
                int temp_rotationActive = 0;
                CncGetMachineZeroWorkPoint((CNC_CART_DOUBLE*)pos.Pointer, &temp_rotationActive);
                rotationActive = temp_rotationActive;
            }
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern CNC_CART_DOUBLE CncGetActualOriginOffset();
        public virtual CncCartDouble GetActualOriginOffset()
        {
            if (_CncCartDouble_GetActualOriginOffset == null || _CncCartDouble_GetActualOriginOffset.IsDisposed == true)
            {
                _CncCartDouble_GetActualOriginOffset = new CncCartDouble();
            }
            CNC_CART_DOUBLE temp_CncCartDouble = CncGetActualOriginOffset();
            _CncCartDouble_GetActualOriginOffset.SetStructValue(ref temp_CncCartDouble);

            return _CncCartDouble_GetActualOriginOffset;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern double CncGetActualToolZOffset();
        public virtual double GetActualToolZOffset()
        {
            return CncGetActualToolZOffset();
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern double CncGetActualToolXOffset();
        public virtual double GetActualToolXOffset()
        {
            return CncGetActualToolXOffset();
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern double CncGetActualG68Rotation();
        public virtual double GetActualG68Rotation()
        {
            return CncGetActualG68Rotation();
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern CncPlane CncGetActualG68RotationPlane();
        public virtual CncPlane GetActualG68RotationPlane()
        {
            return CncGetActualG68RotationPlane();
        }
    }
    public class G_StatusItemsInterpreter
    {
        public G_GetServer CncServer { get; } = null;
        public G_StatusItemsInterpreter(G_GetServer CNC_SERVER)
        {
            CncServer = CNC_SERVER;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern void CncGetCurrentGcodesText(sbyte* activeGCodes);
        public virtual void GetCurrentGcodesText(out string activeGCodes)//
        {
            unsafe
            {
                sbyte* temp_activeGCodes = stackalloc sbyte[80];
                CncGetCurrentGcodesText(temp_activeGCodes);
                activeGCodes= StringConversie.CharArrayToString((IntPtr)temp_activeGCodes,0,80);
            }
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern void CncGetCurrentMcodesText(sbyte* activeGCodes);
        public virtual void GetCurrentMcodesText(out string activeMCodes)//
        {
            unsafe
            {
                sbyte* temp_activeMCodes = stackalloc sbyte[80];
                CncGetCurrentMcodesText(temp_activeMCodes);
                activeMCodes = StringConversie.CharArrayToString((IntPtr)temp_activeMCodes, 0, 80);
            }
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern void CncGetCurrentGcodeSettingsText(sbyte * activeGCodes);
        public virtual void GetCurrentGcodeSettingsText(out string actualGCodeSettings)//
        {
            unsafe
            {
                sbyte* temp_actualGCodeSettings = stackalloc sbyte[80];
                CncGetCurrentGcodeSettingsText(temp_actualGCodeSettings);
                actualGCodeSettings = StringConversie.CharArrayToString((IntPtr)temp_actualGCodeSettings, 0, 80);
            }
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern double CncGetProgrammedSpeed();
        public virtual double GetProgrammedSpeed()
        {
            return CncGetProgrammedSpeed();
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern double CncGetProgrammedFeed();
        public virtual double GetProgrammedFeed()
        {
            return CncGetProgrammedFeed();
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern int CncGetCurrentToolNumber();
        public virtual int GetCurrentToolNumber()
        {
            return CncGetCurrentToolNumber();
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern int CncG43Active();
        public virtual int G43Active()
        {
            return CncG43Active();
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern int CncG95Active();
        public virtual int G95Active()
        {
            return CncG95Active();
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern int CncGetCurInterpreterLineNumber();
        public virtual int GetCurInterpreterLineNumber()
        {
            return CncGetCurInterpreterLineNumber();
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern int CncGetCurInterpreterLineText(sbyte * text);
        public virtual int GetCurInterpreterLineNumber(out string text)//
        {
            unsafe
            {
                sbyte* temp_text = stackalloc sbyte[(int)CncConstants.CNC_MAX_INTERPRETER_LINE];
                int return_result = CncGetCurInterpreterLineText(temp_text);
                text = StringConversie.CharArrayToString((IntPtr)temp_text,0, (int)CncConstants.CNC_MAX_INTERPRETER_LINE);
                return return_result;
            }
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern int CncCurrentInterpreterLineContainsToolChange();
        public virtual int CurrentInterpreterLineContainsToolChange()
        {
            return CncCurrentInterpreterLineContainsToolChange();
        }
    }
    public class G_StatusErrorSafety
    {
        public G_GetServer CncServer { get; } = null;

        public G_StatusErrorSafety(G_GetServer CNC_SERVER)
        {
            CncServer = CNC_SERVER;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern int CncGetSwLimitError();
        public virtual int GetSwLimitError()
        {
            return CncGetSwLimitError();
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern int CncGetFifoError();
        public virtual int GetFifoError()
        {
            return CncGetFifoError();
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern int CncGetEMStopActive();
        public virtual int GetEMStopActive()
        {
            return CncGetEMStopActive();
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern int CncGetAllAxesHomed();
        public virtual int GetAllAxesHomed()
        {
            return CncGetAllAxesHomed();
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern int CncGetSafetyMode();
        public virtual int GetSafetyMode()
        {
            return CncGetSafetyMode();
        }
    }
    public class G_Kinematics
    {
        private CncVector _CncVector_KinGetARotationPoint = null;
        public G_GetServer CncServer { get; } = null;

        public G_Kinematics(G_GetServer CNC_SERVER)
        {
            CncServer = CNC_SERVER;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern CncKinematicsType CncKinGetActiveType();
        public virtual CncKinematicsType KinGetActiveType()
        {
            return CncKinGetActiveType();
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern int CncKinActivate(int active);
        public virtual int KinActivate(int active)
        {
            return CncKinActivate(active);
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern int CncKinInit();
        public virtual int KinInit()
        {
            return CncKinInit();
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern int CncKinControl(KinControlId controlID, KIN_CONTROLDATA * pControlData);
        public unsafe virtual int KinControl(KinControlId controlID, KinControldata pControlData)
        {
            return CncKinControl(controlID, (KIN_CONTROLDATA*)pControlData.Pointer);
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern CNC_VECTOR CncKinGetARotationPoint();
        public virtual CncVector KinGetARotationPoint()
        {
            if(_CncVector_KinGetARotationPoint == null || _CncVector_KinGetARotationPoint.IsDisposed == true)
            {
                _CncVector_KinGetARotationPoint = new CncVector();
            }
            CNC_VECTOR temp_CncVector = CncKinGetARotationPoint();
            _CncVector_KinGetARotationPoint.SetStructValue(ref temp_CncVector);

            return _CncVector_KinGetARotationPoint;
        }

    }
    public class G_StatusItemsIO
    {
        public G_GetServer CncServer { get; } = null;
        public G_StatusItemsIO(G_GetServer CNC_SERVER)
        {
            CncServer = CNC_SERVER;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern sbyte * CncGetIOName(CncIoId id);
        public unsafe virtual string GetIOName(CncIoId id)//
        {
            return StringConversie.CharArrayToString((IntPtr)CncGetIOName(id),0,(int)CncConstants.CNC_MAX_IO_NAME_LENGTH);
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern int CncGetOutput(CncIoId id);
        public virtual int GetOutput(CncIoId id)
        {
            return CncGetOutput( id);
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern int CncGetOutputRaw(CncIoId id);
        public virtual int GetOutputRaw(CncIoId id)
        {
            return CncGetOutputRaw(id);
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern int CncGetGPIOOutput(int gpioCardIndex, CncGpioId ioId);
        public virtual int GetGPIOOutput(int gpioCardIndex, CncGpioId ioId)
        {
            return CncGetGPIOOutput( gpioCardIndex,  ioId);
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern int CncGetInput(CncIoId id);
        public virtual int GetInput(CncIoId id)
        {
            return CncGetInput(id);
        }


        [DllImport(G_GetServer.CncApiDll)]
        public static extern int CncGetInputRaw(CncIoId id);
        public virtual int GetInputRaw(CncIoId id)
        {
            return CncGetInputRaw( id);
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern int CncGetGPIOInput(int gpioCardIndex, CncGpioId ioId);
        public virtual int GetGPIOInput(int gpioCardIndex, CncGpioId ioId)
        {
            return CncGetGPIOInput( gpioCardIndex,  ioId);
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern CncRc CncSetOutput(CncIoId id, int value);
        public virtual CncRc SetOutput(CncIoId id, int value)
        {
            CncRc return_result = CncSetOutput( id,  value);
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern CncRc CncSetOutputRaw(CncIoId id, int value);
        public virtual CncRc SetOutputRaw(CncIoId id, int value)
        {
            CncRc return_result = CncSetOutputRaw( id,  value);
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern CncRc CncSetGPIOOutput(int gpioCardIndex, CncGpioId ioId, int value);
        public virtual CncRc SetGPIOOutput(int gpioCardIndex, CncGpioId ioId, int value)
        {
            CncRc return_result = CncSetGPIOOutput(gpioCardIndex, ioId, value);
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CncRc CncCheckStartConditionOK(int generateMessage, int ignoreHoming, int* result);
        public virtual CncRc CheckStartConditionOK(int generateMessage, int ignoreHoming,out int result)
        {
            unsafe
            {
                int temp_result = 0;
                CncRc return_result = CncCheckStartConditionOK(generateMessage, ignoreHoming, &temp_result);
                CncServer.LastKnowRcState = return_result;
                result = temp_result;
                return return_result;
            }
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern CncRc CncSetSpindleOutput(int onOff, int direction, double absSpeed);
        public virtual CncRc SetSpindleOutput(int onOff, int direction, double absSpeed)
        {
            CncRc return_result = CncSetSpindleOutput( onOff,  direction,  absSpeed);
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }
    }
    
    public class G_LogMessagesRealtime
    {
        private CncLogMessage _CncLogMessage_LogFifoGet = null;
        private CncPosFifoData _CncPosFifoData_PosFifoGet = null;
        private CncPosFifoData _CncPosFifoData_PosFifoGet2 = null;
        private CncGraphFifoData _CncGraphFifoData_GraphFifoGet = null;
        public G_GetServer CncServer { get; } = null;

        public G_LogMessagesRealtime(G_GetServer CNC_SERVER)
        {
            CncServer = CNC_SERVER;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CncRc CncLogFifoGet(CNC_LOG_MESSAGE* data);
        public unsafe virtual CncRc LogFifoGet(CncLogMessage data)
        {
            CncRc return_result = CncLogFifoGet((CNC_LOG_MESSAGE*)data.Pointer);
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }
        public CncRc LogFifoGet(out CncLogMessage data)
        {
            if (_CncLogMessage_LogFifoGet == null || _CncLogMessage_LogFifoGet.IsDisposed == true)
            {
                _CncLogMessage_LogFifoGet = new CncLogMessage();
            }
            data = _CncLogMessage_LogFifoGet;
            return LogFifoGet(_CncLogMessage_LogFifoGet);
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CncRc CncPosFifoGet(CNC_POS_FIFO_DATA* data);
        public unsafe virtual CncRc PosFifoGet(CncPosFifoData data)
        {
            CncRc return_result = CncPosFifoGet((CNC_POS_FIFO_DATA*)data.Pointer);
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }
        public CncRc PosFifoGet(out CncPosFifoData data)
        {
            if (_CncPosFifoData_PosFifoGet == null || _CncPosFifoData_PosFifoGet.IsDisposed == true)
            {
                _CncPosFifoData_PosFifoGet = new CncPosFifoData();
            }
            data = _CncPosFifoData_PosFifoGet;
            return PosFifoGet(data);
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CncRc CncPosFifoGet2(CNC_POS_FIFO_DATA* data, int* isLast);
        public virtual CncRc PosFifoGet2(CncPosFifoData data, out int isLast)
        {
            unsafe
            {
                int temp_isLast = 0;
                CncRc return_result = CncPosFifoGet2((CNC_POS_FIFO_DATA*)data.Pointer, &temp_isLast);
                isLast = temp_isLast;
                CncServer.LastKnowRcState = return_result;
                return return_result;
            }
        }
        public CncRc PosFifoGet2(out CncPosFifoData data, out int isLast)
        {
            if (_CncPosFifoData_PosFifoGet2 == null || _CncPosFifoData_PosFifoGet2.IsDisposed == true)
            {
                _CncPosFifoData_PosFifoGet2 = new CncPosFifoData();
            }
            data = _CncPosFifoData_PosFifoGet2;
            return PosFifoGet2(data, out isLast);
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CncRc CncGraphFifoGet(CNC_GRAPH_FIFO_DATA* data);
        public unsafe virtual CncRc GraphFifoGet(CncGraphFifoData data)
        {
            CncRc return_result = CncGraphFifoGet( (CNC_GRAPH_FIFO_DATA*)data.Pointer);
            
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }
        public CncRc GraphFifoGet(out CncGraphFifoData data)
        {
            if(_CncGraphFifoData_GraphFifoGet == null || _CncGraphFifoData_GraphFifoGet.IsDisposed == true)
            {
                _CncGraphFifoData_GraphFifoGet = new CncGraphFifoData();
            }
            data = _CncGraphFifoData_GraphFifoGet;
            return GraphFifoGet(_CncGraphFifoData_GraphFifoGet);
        }


    }

    
    public class G_CommandsJobInterpreter
    {
        private CncCmdArrayData GetJobArrayParameters_CncCmdArrayData = null;
        private CncCmdArrayData SetJobArrayParameters_CncCmdArrayData = null;
        private CncVector GetJobMaterialSize_CncVector = null;
        public G_GetServer CncServer { get; } = null;

        public G_CommandsJobInterpreter(G_GetServer CNC_SERVER)
        {
            CncServer = CNC_SERVER;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern CncRc CncReset();
        public virtual CncRc Reset()
        {
            CncRc return_result = CncReset();
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern CncRc CncReset2(uint resetFlags);
        public virtual CncRc Reset2(uint resetFlags)
        {
            CncRc return_result = CncReset2(resetFlags);
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CncRc CncRunSingleLine(sbyte * text);
        public virtual CncRc RunSingleLine(string text)
        {
            unsafe
            {
                sbyte* temp_text = stackalloc sbyte[text.Length+1];
                StringConversie.StringToMaxCharArray(text,(IntPtr)temp_text,0, text.Length+1);
                CncRc return_result = CncRunSingleLine(temp_text);
                CncServer.LastKnowRcState = return_result;
                return return_result;
            }
        }
        //[SuppressUnmanagedCodeSecurity]
        [DllImport(G_GetServer.CncApiDll)]
        public static extern int CncWaitSingleLine(IntPtr pKeepAlive, IntPtr  pKeepAliveParameter);
        public virtual int WaitSingleLine(CncKeepUiAliveFunction pKeepAlive)
        {
            return CncWaitSingleLine(pKeepAlive.Functionpfunc, pKeepAlive._Functionparameter);
        }
        public async Task<int> AsyncWaitSingleLine(CncKeepUiAliveFunction pKeepAlive)
        {
            return await Task.Run(()=>
            {
                return WaitSingleLine(pKeepAlive);
            });
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CncRc CncLoadJobA(sbyte * fileName);
        public unsafe virtual CncRc LoadJobA(string fileName)
        {
            CncRc return_result = CncRc.CNC_RC_OK;
            sbyte * pointer =  stackalloc sbyte[(int)CncConstants.CNC_MAX_PATH]; 
            StringConversie.StringToMaxCharArray(fileName, (IntPtr)pointer, 0, (int)CncConstants.CNC_MAX_PATH);
            return_result = CncLoadJobA(pointer);
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }

        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CncRc CncLoadJobW(char * fileName);
        public unsafe virtual CncRc LoadJobW(string fileName)
        {
            CncRc return_result = CncRc.CNC_RC_OK;
            char* pointer = stackalloc char[(int)CncConstants.CNC_MAX_PATH];
            StringConversie.StringToMaxWCharArray(fileName, (IntPtr)pointer, 0, (int)CncConstants.CNC_MAX_PATH);
            return_result = CncLoadJobW(pointer);
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }

        public virtual CncRc LoadJob(string fileName)
        {
            CncRc return_result = CncRc.CNC_RC_OK;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) == true)
            {
                return_result = LoadJobW(fileName);
            }
            else
            {
                return_result = LoadJobA(fileName);
            }
            return return_result;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern CncRc CncRunOrResumeJob();
        public virtual CncRc RunOrResumeJob()
        {
            CncRc return_result = CncRunOrResumeJob();
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern CncRc CncStartRenderGraph(int outLines, int contour);
        public virtual CncRc StartRenderGraph(int outLines, int contour)
        {
            CncRc return_result = CncStartRenderGraph( outLines,  contour);
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern CncRc CncStartRenderSearch(int outLines, int contour, int lineNr, int toolNr, int arrayX, int arrayY);
        public virtual CncRc StartRenderSearch(int outLines, int contour, int lineNr, int toolNr, int arrayX, int arrayY)
        {
            CncRc return_result = CncStartRenderSearch( outLines,  contour,  lineNr,  toolNr,  arrayX,  arrayY);
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern CncRc CncRewindJob();
        public virtual CncRc RewindJob()
        {
            CncRc return_result = CncRewindJob();
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern CncRc CncAbortJob();
        public virtual CncRc AbortJob()
        {
            CncRc return_result = CncAbortJob();
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CncRc CncSetJobArrayParameters(CNC_CMD_ARRAY_DATA* runJobData);
        public unsafe virtual CncRc SetJobArrayParameters(CncCmdArrayData runJobData)
        {
            CncRc return_result = CncSetJobArrayParameters((CNC_CMD_ARRAY_DATA*)runJobData.Pointer);
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }
        public CncRc SetJobArrayParameters(out CncCmdArrayData runJobData)
        {
            if(SetJobArrayParameters_CncCmdArrayData == null || SetJobArrayParameters_CncCmdArrayData.IsDisposed == true)
            {
                SetJobArrayParameters_CncCmdArrayData = new CncCmdArrayData();
            }
            runJobData = SetJobArrayParameters_CncCmdArrayData;
            return SetJobArrayParameters(SetJobArrayParameters_CncCmdArrayData);
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CncRc CncGetJobArrayParameters(CNC_CMD_ARRAY_DATA* runJobData);
        public unsafe virtual CncRc GetJobArrayParameters(CncCmdArrayData runJobData)
        {
            CncRc return_result = CncGetJobArrayParameters((CNC_CMD_ARRAY_DATA*)runJobData.Pointer);
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }
        public CncRc GetJobArrayParameters(out CncCmdArrayData runJobData)
        {
            if(GetJobArrayParameters_CncCmdArrayData == null || GetJobArrayParameters_CncCmdArrayData.IsDisposed == true)
            {
                GetJobArrayParameters_CncCmdArrayData = new CncCmdArrayData();
            }
            runJobData = GetJobArrayParameters_CncCmdArrayData;
            return GetJobArrayParameters(GetJobArrayParameters_CncCmdArrayData);
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern CNC_VECTOR CncGetJobMaterialSize();
        public virtual CncVector GetJobMaterialSize()
        {
            if(GetJobMaterialSize_CncVector == null || GetJobMaterialSize_CncVector.IsDisposed == true)
            {
                GetJobMaterialSize_CncVector = new CncVector();
            }
            CNC_VECTOR temp = CncGetJobMaterialSize();
            GetJobMaterialSize_CncVector.SetStructValue(ref temp);
            return GetJobMaterialSize_CncVector;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CncRc CncGetJobFiducual(int n, CNC_FIDUCIAL_DATA* fiducial);
        public unsafe virtual CncRc GetJobFiducual(int n, CncFiducialData fiducial)
        {
            CncRc return_result = CncGetJobFiducual(n,(CNC_FIDUCIAL_DATA*)fiducial.Pointer);
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern CncRc CncEnableBlockDelete(int enable);
        public virtual CncRc EnableBlockDelete(int enable)
        {
            CncRc return_result = CncEnableBlockDelete(enable);
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern int CncGetBlocDelete();
        public virtual int GetBlocDelete()
        {
            return CncGetBlocDelete();
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern CncRc CncEnableOptionalStop(int enable);
        public virtual CncRc EnableOptionalStop(int enable)
        {
            return CncEnableOptionalStop( enable);
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern int CncGetOptionalStop();
        public virtual int GetOptionalStop()
        {
            return CncGetOptionalStop();
        }


        [DllImport(G_GetServer.CncApiDll)]
        public static extern CncRc CncSingleStepMode(int enable);
        public virtual CncRc SingleStepMode(int enable)
        {
            CncRc return_result = CncSingleStepMode( enable);
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern int CncGetSingleStepMode();
        public virtual int GetSingleStepMode()
        {
            return CncGetSingleStepMode();
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CncRc CncSetExtraJobOptions(sbyte* extraLine, int doRepeat, uint numberOfRepeats);
        public unsafe virtual CncRc SetExtraJobOptions(string extraLine, int doRepeat, uint numberOfRepeats)
        {
            CncRc return_result = CncRc.CNC_RC_OK;
            sbyte* temp = stackalloc sbyte[(int)CncConstants.CNC_MAX_INTERPRETER_LINE];
            return_result = CncSetExtraJobOptions(temp, doRepeat, numberOfRepeats);
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern void CncGetExtraJobOptions(sbyte * extraLine, int* doRepeat, uint* numberOfRepeats);
        public unsafe virtual void GetExtraJobOptions(out string extraLine,out int doRepeat,out uint numberOfRepeats)
        {
            sbyte* temp = stackalloc sbyte[(int)CncConstants.CNC_MAX_INTERPRETER_LINE];
            
            int temp_doRepeat = 0;
            uint temp_numberOfRepeats = 0;
            CncGetExtraJobOptions(temp, &temp_doRepeat, &temp_numberOfRepeats);
            doRepeat = temp_doRepeat;
            numberOfRepeats = temp_numberOfRepeats;
            extraLine = StringConversie.CharArrayToString((IntPtr)temp, 0, (int)CncConstants.CNC_MAX_INTERPRETER_LINE);
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern CncRc CncSetSimulationMode(int enable);
        public virtual CncRc SetSimulationMode(int enable)
        {
            CncRc return_result = CncSetSimulationMode(enable);
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern int CncGetSimulationMode();
        public virtual int GetSimulationMode()
        {
            return CncGetSimulationMode();
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern CncRc CncSetFeedOverride(double factor);
        public virtual CncRc SetFeedOverride(double factor)
        {
            CncRc return_result = CncSetFeedOverride(factor);
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern CncRc CncSetArcFeedOverride(double factor);
        public virtual CncRc SetArcFeedOverride(double factor)
        {
            CncRc return_result = CncSetArcFeedOverride(factor);
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern double CncGetActualFeedOverride();
        public virtual double GetActualFeedOverride()
        {
            return CncGetActualFeedOverride();
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern double CncGetActualArcFeedOverride();
        public virtual double GetActualArcFeedOverride()
        {
            return CncGetActualArcFeedOverride();
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern double CncGetActualFeed();
        public virtual double GetActualFeed()
        {
            return CncGetActualFeed();
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern CncRc CncSetSpeedOverride(double factor);
        public virtual CncRc SetSpeedOverride(double factor)
        {
            CncRc return_result = CncSetSpeedOverride(factor);
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern double CncGetActualSpeedOverride();
        public virtual double GetActualSpeedOverride()
        {
            return CncGetActualSpeedOverride();
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern double CncGetActualSpeed();
        public virtual double GetActualSpeed()
        {
            return GetActualSpeed();
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CncRc CncFindFirstJobLine(sbyte * text, int * endOfJob, int * totNumOfLines);
        public unsafe virtual CncRc FindFirstJobLine(out string text, out int endOfJob, out int totNumOfLines)
        {
            CncRc return_result = CncRc.CNC_RC_OK;
            sbyte* temp = stackalloc sbyte[(int)CncConstants.CNC_MAX_INTERPRETER_LINE];
            int temp_endOfJob = 0;
            int temp_totNumOfLines = 0;
            return_result = CncFindFirstJobLine(temp, &temp_endOfJob, &temp_totNumOfLines);
            text = StringConversie.CharArrayToString((IntPtr)temp,0, (int)CncConstants.CNC_MAX_INTERPRETER_LINE);
            endOfJob = temp_endOfJob;
            totNumOfLines = temp_totNumOfLines;
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CncRc CncFindFirstJobLineF(sbyte * text, int * endOfJob);
        public unsafe virtual CncRc FindFirstJobLineF(out string text,out int endOfJob)
        {
            CncRc return_result = CncRc.CNC_RC_OK;
            sbyte* temp = stackalloc sbyte[(int)CncConstants.CNC_MAX_INTERPRETER_LINE];
            int temp_endOfJob = 0;
            return_result = CncFindFirstJobLineF(temp, &temp_endOfJob);
            text = StringConversie.CharArrayToString((IntPtr)temp,0, (int)CncConstants.CNC_MAX_INTERPRETER_LINE);
            endOfJob = temp_endOfJob;
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CncRc CncFindNextJobLine(sbyte * text, int* endOfJob);
        public unsafe virtual CncRc FindNextJobLine(out string text,out int endOfJob)
        {
            CncRc return_result = CncRc.CNC_RC_OK;
            sbyte* temp = stackalloc sbyte[(int)CncConstants.CNC_MAX_INTERPRETER_LINE];
            int temp_endOfJob = 0;
            return_result = CncFindNextJobLine(temp, &temp_endOfJob);
            text = StringConversie.CharArrayToString((IntPtr)temp, 0, (int)CncConstants.CNC_MAX_INTERPRETER_LINE);
            endOfJob = temp_endOfJob;
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CncRc CncFindNextJobLineF(sbyte * text, int* endOfJob);
        public unsafe virtual CncRc FindNextJobLineF(out string text,out int endOfJob)
        {
            CncRc return_result = CncRc.CNC_RC_OK;
            sbyte * temp = stackalloc sbyte[(int)CncConstants.CNC_MAX_INTERPRETER_LINE];
            int temp_endOfJob = 0;
            return_result = CncFindNextJobLineF(temp, &temp_endOfJob);
            text = StringConversie.CharArrayToString((IntPtr)temp,0, (int)CncConstants.CNC_MAX_INTERPRETER_LINE);
            endOfJob = temp_endOfJob;
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }
    }
    public class G_PauseFunctions
    {
        public G_GetServer CncServer { get; } = null;
        public G_PauseFunctions(G_GetServer CNC_SERVER)
        {
            CncServer = CNC_SERVER;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern int CncSwitchOnSpindleAndWaitUntilOn(IntPtr pFunc, IntPtr pFuncParameter);
        public virtual int SwitchOnSpindleAndWaitUntilOn(CncKeepUiAliveFunction pFunc)
        {
            return CncSwitchOnSpindleAndWaitUntilOn(pFunc.Functionpfunc, pFunc._Functionparameter);
        }
        public async Task<int> AsyncSwitchOnSpindleAndWaitUntilOn(CncKeepUiAliveFunction pFunc)
        {
            return await Task.Run(()=>
            {
                return SwitchOnSpindleAndWaitUntilOn(pFunc);
            });
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern CncRc CncPauseJob();
        public virtual CncRc PauseJob()
        {
            CncRc return_result = CncPauseJob();
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetServer.CncApiDll, EntryPoint = "_CncPauseJob2@8")]
        public static extern CncRc CncPauseJob2(IntPtr pFunc, IntPtr pFuncParameter);
        public virtual CncRc PauseJob2(CncKeepUiAliveFunction pFunc)
        {
            CncRc return_result = CncPauseJob2(pFunc.Functionpfunc, pFunc._Functionparameter);
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }
        public async Task<CncRc> AsyncPauseJob2(CncKeepUiAliveFunction pFunc)
        {
            return await Task.Run(()=>
            {
                return PauseJob2(pFunc);
            });
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern int CncSyncPauseZSafe();
        public virtual int SyncPauseZSafe()
        {
            return SyncPauseZSafe();
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern int CncSyncPauseXSafe();
        public virtual int SyncPauseXSafe()
        {
            return CncSyncPauseXSafe();
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern int CncSyncPauseAxis(int axis, double feed);
        public virtual int SyncPauseAxis(int axis, double feed)
        {
            return CncSyncPauseAxis(axis,feed);
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern int CncSyncFromPauseAndStartAutomatic(double approachFeed, IntPtr pFunc, IntPtr pFuncParameter);
        public virtual int SyncFromPauseAndStartAutomatic(double approachFeed, CncKeepUiAliveFunction pFunc)
        {
            return CncSyncFromPauseAndStartAutomatic(approachFeed,pFunc.Functionpfunc, pFunc._Functionparameter);
        }
        public async Task<int> AsyncSyncFromPauseAndStartAutomatic(double approachFeed, CncKeepUiAliveFunction pFunc)
        {
            return await Task.Run(()=>
            {
                return SyncFromPauseAndStartAutomatic(approachFeed,pFunc);
            });
        }
    }
    public class G_SearchFunctions
    {
        public G_GetServer CncServer { get; } = null;
        public G_SearchFunctions(G_GetServer CNC_SERVER)
        {
            CncServer = CNC_SERVER;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern int CncSyncSearchZSafe(IntPtr pFunc, IntPtr pFuncParameter);
        public virtual int SyncSearchZSafe(CncKeepUiAliveFunction pFunc)
        {
            return CncSyncSearchZSafe(pFunc.Functionpfunc,pFunc._Functionparameter);
        }
        public async Task<int> AsyncSyncSearchZSafe(CncKeepUiAliveFunction pFunc)
        {
            return await Task.Run(()=>
            {
                return SyncSearchZSafe(pFunc);
            });
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern int CncSyncSearchXSafe(IntPtr pFunc, IntPtr pFuncParameter);
        public virtual int SyncSearchXSafe(CncKeepUiAliveFunction pFunc)
        {
            return CncSyncSearchXSafe(pFunc.Functionpfunc, pFunc._Functionparameter);
        }
        public async Task<int> AsyncSyncSearchXSafe(CncKeepUiAliveFunction pFunc)
        {
            return await Task.Run(()=>
            {
                return SyncSearchXSafe(pFunc);
            });
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern int CncSyncSearchTool(IntPtr pFunc, IntPtr pFuncParameter);
        public virtual int SyncSearchTool(CncKeepUiAliveFunction pFunc)
        {
            return CncSyncSearchTool(pFunc.Functionpfunc, pFunc._Functionparameter);
        }
        public async Task<int> AsyncSyncSearchTool(CncKeepUiAliveFunction pFunc)
        {
            return await Task.Run(()=>
            {
                return SyncSearchTool(pFunc);
            });
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern int CncSyncInchModeAndParametersAndOffset(IntPtr pFunc, IntPtr pFuncParameter);
        public virtual int SyncInchModeAndParametersAndOffset(CncKeepUiAliveFunction pFunc)
        {
            return CncSyncInchModeAndParametersAndOffset(pFunc.Functionpfunc, pFunc._Functionparameter);
        }
        public async Task<int> AsyncSyncInchModeAndParametersAndOffset(CncKeepUiAliveFunction pFunc)
        {
            return await Task.Run(()=>
            {
                return SyncInchModeAndParametersAndOffset(pFunc);
            });
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern int CncSyncSearchAxis(int axis, double feed, IntPtr pFunc, IntPtr pFuncParameter);
        public virtual int SyncSearchAxis(int axis, double feed, CncKeepUiAliveFunction pFunc)
        {
            return CncSyncSearchAxis(axis,feed, pFunc.Functionpfunc, pFunc._Functionparameter);
        }
        public async Task<int> AsyncSyncSearchAxis(int axis, double feed, CncKeepUiAliveFunction pFunc)
        {
            return await Task.Run(()=>
            {
                return SyncSearchAxis(axis, feed, pFunc);
            });
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern int CncSyncFromSearchAndStartAutomatic(double approachFeed, IntPtr pFunc, IntPtr pFuncParameter);
        public virtual int SyncFromSearchAndStartAutomatic(double approachFeed, CncKeepUiAliveFunction pFunc)
        {
            return CncSyncFromSearchAndStartAutomatic(approachFeed,pFunc.Functionpfunc, pFunc._Functionparameter);
        }
        public async Task<int> AsyncSyncFromSearchAndStartAutomatic(double approachFeed, CncKeepUiAliveFunction pFunc)
        {
            return await Task.Run(()=>
            {
                return SyncFromSearchAndStartAutomatic(approachFeed,pFunc);
            });
        }

    }
    public class G_JoggingFunctions
    {
        public G_GetServer CncServer { get; } = null;
        public G_JoggingFunctions(G_GetServer CNC_SERVER)
        {
            CncServer = CNC_SERVER;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CncRc CncStartJog(double* axes,
                                    double velocityFactor,
                                    int continuous);
        public virtual unsafe CncRc StartJog(double[] axes,double velocityFactor,int continuous)
        {
            double* temp = stackalloc double[(int)CncConstants.CNC_MAX_AXES];
            for(int i =0;i< (int)CncConstants.CNC_MAX_AXES;i++)
            {
                if(i< axes.Length)
                {
                    temp[i] = axes[i];
                }
                else
                {
                    temp[i] = 0;
                }
            }
            CncRc return_result = CncStartJog(temp,velocityFactor, continuous);
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }
        public CncRc StartJog(CncCartDouble axes, double velocityFactor, int continuous)
        {
            return StartJog(new double[] 
            {
                axes.x, 
                axes.y, 
                axes.z, 
                axes.a, 
                axes.b, 
                axes.c 
            }, velocityFactor, continuous);
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern CncRc CncStartJog2(int axis,double step,double velocityFactor,int continuous);
        public virtual CncRc StartJog2(int axis,double step,double velocityFactor,int continuous)
        {
            CncRc return_result = CncStartJog2(axis, step, velocityFactor,continuous);
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern CncRc CncStopJog(int axis);
        public virtual CncRc StopJog(int axis)
        {
            CncRc return_Result = CncStopJog(axis);
            CncServer.LastKnowRcState = return_Result;
            return return_Result;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern CncRc CncMoveTo(CNC_CART_DOUBLE pos, CNC_CART_BOOL move, double velocityFactor);
        public virtual CncRc MoveTo(CncCartDouble pos, CncCartBool move, double velocityFactor)
        {
            CncRc return_result = CncMoveTo(pos.GetStructValue(), move.GetStructValue(), velocityFactor);
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }

    }
    public class G_TrackingFunctions
    {
        private CncThcProcessParameters CncThcProcessParameters_GetPlasmaParameters = null;

        public G_GetServer CncServer { get; } = null;
        public G_TrackingFunctions(G_GetServer CNC_SERVER)
        {
            CncServer = CNC_SERVER;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern CncRc CncStartPositionTracking();
        public virtual CncRc StartPositionTracking()
        {
            CncRc return_result = CncStartPositionTracking();
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern CncRc CncStartVelocityTracking();
        public virtual CncRc StartVelocityTracking()
        {
            CncRc return_result = CncStartVelocityTracking();
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern CncRc CncStartHandweelTracking(int axis, double vLimit, int handwheelID,int velMode,double multiplicationFactor,int handwheelCountsPerRevolution);
        public virtual CncRc StartHandweelTracking(int axis, double vLimit, int handwheelID,int velMode,double multiplicationFactor,int handwheelCountsPerRevolution)
        {
            CncRc return_result = CncStartHandweelTracking( axis,  vLimit,  handwheelID,velMode,multiplicationFactor,handwheelCountsPerRevolution);
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern CncRc CncSetTrackingPosition(CNC_CART_DOUBLE pos,CNC_CART_DOUBLE vel,CNC_CART_BOOL move);
        public virtual CncRc SetTrackingPosition(CncCartDouble pos, CncCartDouble vel, CncCartBool move)
        {
            CncRc return_result = CncSetTrackingPosition(pos.GetStructValue(), vel.GetStructValue(), move.GetStructValue());
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern CncRc CncSetTrackingPosition2(CNC_CART_DOUBLE pos,CNC_CART_DOUBLE vel,CNC_CART_DOUBLE acc,CNC_CART_BOOL move);
        public virtual CncRc SetTrackingPosition2(CncCartDouble pos, CncCartDouble vel, CncCartDouble acc, CncCartBool move)
        {
            CncRc return_result = CncSetTrackingPosition2(pos.GetStructValue(),vel.GetStructValue(), acc.GetStructValue(), move.GetStructValue());
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern CncRc CncSetTrackingVelocity(CNC_CART_DOUBLE vel,CNC_CART_BOOL move);
        public virtual CncRc SetTrackingVelocity(CncCartDouble vel, CncCartBool move)
        {
            CncRc return_result = CncSetTrackingVelocity(vel.GetStructValue(),move.GetStructValue());
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern CncRc CncSetTrackingVelocity2(CNC_CART_DOUBLE vel,CNC_CART_DOUBLE acc,CNC_CART_BOOL axes);
        public virtual CncRc SetTrackingVelocity2(CncCartDouble vel, CncCartDouble acc, CncCartBool axes)
        {
            CncRc return_result = CncSetTrackingVelocity2(vel.GetStructValue(),acc.GetStructValue(), axes.GetStructValue());
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern CncRc CncSetTrackingHandwheelCounter(int hw1Count, int hw2Count, int hw3Count);
        public virtual CncRc SetTrackingHandwheelCounter(int hw1Count, int hw2Count, int hw3Count)
        {
            CncRc return_result = CncSetTrackingHandwheelCounter(hw1Count, hw2Count, hw3Count);
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetServer.CncApiDll)] 
        public static extern CncRc CncStartPlasmaTHCTracking(double pLimit, double nLimit);
        public virtual CncRc StartPlasmaTHCTracking(double pLimit, double nLimit)
        {
            CncRc return_result = CncStartPlasmaTHCTracking( pLimit,  nLimit);
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern CncRc CncSetPlasmaParameters(CNC_THC_PROCESS_PARAMETERS thcCfg);
        public virtual CncRc SetPlasmaParameters(CncThcProcessParameters thcCfg)
        {
            CncRc return_result = CncSetPlasmaParameters(thcCfg.GetStructValue());
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CNC_THC_PROCESS_PARAMETERS* CncGetPlasmaParameters();
        public unsafe CncThcProcessParameters GetPlasmaParameters()
        {
            if(CncThcProcessParameters_GetPlasmaParameters == null || CncThcProcessParameters_GetPlasmaParameters.IsDisposed == true)
            {
                CncThcProcessParameters_GetPlasmaParameters = new CncThcProcessParameters((IntPtr)CncGetPlasmaParameters());
            }
            else
            {
                CncThcProcessParameters_GetPlasmaParameters.Pointer = (IntPtr)CncGetPlasmaParameters();
            }
            return CncThcProcessParameters_GetPlasmaParameters;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern CncRc CncStopTracking();
        public virtual CncRc StopTracking()
        {
            CncRc return_result = CncStopTracking();
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }
    }
    public class G_3DPrinter
    {
        private Cnc3dprintingCommand Cnc3dprintingCommand_3DPrintCommand = null;
        public G_GetServer CncServer { get; } = null;

        public G_3DPrinter(G_GetServer CNC_SERVER)
        {
            CncServer = CNC_SERVER;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CncRc Cnc3DPrintCommand(CNC_3DPRINTING_COMMAND* pCmd);
        public unsafe virtual CncRc _3DPrintCommand(Cnc3dprintingCommand pCmd)
        {
            CncRc return_result = Cnc3DPrintCommand((CNC_3DPRINTING_COMMAND*)pCmd.Pointer);
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }
        public virtual CncRc _3DPrintCommand(out Cnc3dprintingCommand pCmd)
        {
            if(Cnc3dprintingCommand_3DPrintCommand == null || Cnc3dprintingCommand_3DPrintCommand.IsDisposed == true)
            {
                Cnc3dprintingCommand_3DPrintCommand = new Cnc3dprintingCommand();
            }
            pCmd = Cnc3dprintingCommand_3DPrintCommand;
            return _3DPrintCommand(Cnc3dprintingCommand_3DPrintCommand);
        }
    }
    public class G_UtilityItems
    {
        public G_GetServer CncServer { get; } = null;
        public G_UtilityItems(G_GetServer CNC_SERVER)
        {
            CncServer = CNC_SERVER;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern sbyte * CncGetRCText(CncRc rc);
        public unsafe virtual string GetRCText(CncRc rc)
        {
            return StringConversie.CharArrayToString((IntPtr)CncGetRCText(rc), 0, (int)CncConstants.CNC_MAX_NAME_LENGTH);
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern void CncSendUserMessage(sbyte * functionName, sbyte * fileName, int lineNumber, CncErrorClass ec, CncRc rc, sbyte * msg);
        public unsafe virtual void SendUserMessage(string functionName, string fileName, int lineNumber, CncErrorClass ec, CncRc rc, string msg)
        {
            sbyte* temp_functionName = stackalloc sbyte[(int)CncConstants.CNC_MAX_FUNCTION_NAME_TEXT];
            sbyte* temp_fileName = stackalloc sbyte[(int)CncConstants.CNC_MAX_PATH];
            sbyte* temp_msg = stackalloc sbyte[(int)CncConstants.CNC_MAX_MESSAGE_TEXT];

            StringConversie.StringToMaxCharArray(functionName,(IntPtr)temp_functionName,0, (int)CncConstants.CNC_MAX_FUNCTION_NAME_TEXT);
            StringConversie.StringToMaxCharArray(fileName, (IntPtr)temp_fileName,0, (int)CncConstants.CNC_MAX_PATH);
            StringConversie.StringToMaxCharArray(msg, (IntPtr)temp_msg,0, (int)CncConstants.CNC_MAX_MESSAGE_TEXT);
            CncSendUserMessage(temp_functionName, temp_fileName,lineNumber,ec,rc, temp_msg);
        }
        [DllImport(G_GetServer.CncApiDll)]
        public static extern CncRc CncSendToGUI(CncUioActions action, int value1, int value2);
        public virtual CncRc SendToGUI(CncUioActions action, int value1, int value2)
        {
            CncRc return_result = CncSendToGUI( action,  value1,  value2);
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetServer.CncApiDll)]
        public unsafe static extern CncRc CncGetGUICommand(CncUioActions * pAction, int* pValue1, int* pValue2);
        public unsafe virtual CncRc GetGUICommand(out CncUioActions pAction, out int pValue1,out int pValue2)
        {
            CncUioActions temp_pAction = CncUioActions.CNC_UIOACTION_NONE;
            int temp_pValue1 = 0;
            int temp_pValue2 = 0;
            CncRc return_result = CncGetGUICommand(  &temp_pAction,   &temp_pValue1,  &temp_pValue2);
            pAction = temp_pAction;
            pValue1 = temp_pValue1;
            pValue2 = temp_pValue2;
            CncServer.LastKnowRcState = return_result;
            return return_result;
        }

    }
    public interface IG_GetConfigItems
    {
        string GetSetupPassword();
        CncRc SetSetupPassword(string newPassword);
        CncSystemConfig GetSystemConfig();
        CncInterpreterConfig GetInterpreterConfig();
        CncSafetyConfig GetSafetyConfig();
        CncTrafficLightCfg GetTrafficLightConfig();
        CncProbingCfg GetProbingConfig();
        CncIoConfig GetIOConfig();
        CncI2cgpioCardConfig GetGPIOConfig();
        CncJointCfg GetJointConfig(int joint);
        CncSpindleConfig GetSpindleConfig(int spindle);
        CncFeedspeedCfg GetFeedSpeedConfig();
        CncHandwheelCfg GetHandwheelConfig();
        CncTrajectoryCfg GetTrajectoryConfig();
        CncKinCfg GetKinConfig();
        CncUiCfg GetUIConfig();
        CncCameraConfig GetCameraConfig();
        CncThcCfg GetTHCConfig();
        CncServiceCfg GetServiceConfig();
        Cnc3dprintingConfig Get3DPrintConfig();
        CncUioConfig GetUIOConfig();
        CncVacuumbedConfig GetVacuumConfig();
        CncIoPortSts GetIOStatus(CncIoId ioID);
        CncGpioPortSts GetGPIOStatus(int cardNr, CncGpioId ioID);
        CncRc StoreIniFile(int saveFixtures);
        CncRc ReInitialize();
        CncRc GetMacroFileName(out string name);
        CncRc GetUserMacroFileName(out string name);
        CncRc SetMacroFileName(string name);
        CncRc SetUserMacroFileName(string name);
    }
    public interface IG_GetConfigControllerCpu
    {
        string GetControllerFirmwareVersion();
        CncRc GetControllerSerialNumber(out string serial);//unsigned char *
        int GetControllerNumberOfFrequencyItems();
        double GetControllerFrequencyItem(uint index);
        int GetControllerConnectionNumberOfItems();
        string GetControllerConnectionItem(int itemNumber);
        void GetNrOfAxesOnController(out int maxNrOfAxes,out int availableNrOfAxes);
        int GetAxisIsConfigured(int axis, bool includingSlaves);
        int GetFirmwareHasOptions();
        CncRc GetActiveOptions(out char actCustomerName,
        out int actNumberOfAxes,
        out uint actCPUEnabled,
        out uint actGPIOAVXEnabled,
        out uint actGPIOEDIEnabled,
        out uint actWolfcutCameraEnabled,
        out uint actTURNMACRO,
        out uint actXHCPendant,
        out uint actLimitedSoftwareEnabled);

        CncRc CncGetOptionRequestCode(out string newCustomerName,
        int newNumberOfAxes,
         uint newGPIOAVXEnabled,
         uint newGPIOEDIEnabled,
         uint newPLASMAEnabled,
         uint newTURMACROEnabled,
         uint newXHCPendant,
         uint newLimitedSoftwareEnabled,
        out string requestCode);

        CncRc GetOptionRequestCodeCurrent(out string requestCode);
        CncRc ActivateOption(out string activationKey);
        CncJointSts GetJointStatus(int joint);
    }
    public interface IG_GetSetToolTableData
    {
        CncToolData GetToolData(int index);
        CncRc UpdateToolData(CncToolData pTool, int index);
        CncRc LoadToolTable();
    }
    public interface IG_VariableAccess
    {
        double GetVariable(int varIndex);
        void SetVariable(int varIndex, double value);
    }
    public interface IG_StatusItems
    {
        CncRunningStatus GetRunningStatus();
        CncMotionStatus GetMotionStatus();
        CncControllerStatus GetControllerStatus();
        CncControllerConfigStatus GetControllerConfigStatus();
        CncTrafficLightStatus GetTrafficLightStatus();
        CncJobStatus GetJobStatus();
        CncTrackingStatus GetTrackingStatus();
        CncThcStatus GetTHCStatus();
        CncNestingStatus GetNestingStatus();
        CncKinStatus GetKinStatus();
        CncSpindleSts GetSpindleStatus();
        CncPauseSts GetPauseStatus();
        CncSearchStatus GetSearchStatus();
        Cnc3dprintingSts Get3DPrintStatus();
        CncCompensationStatus GetCompensationStatus();
        CncVacuumStatus GetVacuumStatus();
        int Get10msHeartBeat();
        int IsServerConnected();
    }
    public interface IG_StatusItemsposition
    {
        CncIeState GetState();
        string GetStateText(CncIeState state);
        int InMillimeterMode();
        CncPlane GetActualPlane();
        CncCartDouble GetWorkPosition();
        CncJointDouble GetMotorPosition();
        CncCartDouble GetMachinePosition();
        void GetMachineZeroWorkPoint(CncCartDouble pos,out int rotationActive);
        CncCartDouble GetActualOriginOffset();
        double GetActualToolZOffset();
        double GetActualToolXOffset();
        double GetActualG68Rotation();
        CncPlane GetActualG68RotationPlane();

    }
    public interface IG_StatusItemsInterpreter
    {
        void GetCurrentGcodesText(out string activeGCodes);
        void GetCurrentMcodesText(out string activeMCodes);
        void GetCurrentGcodeSettingsText(out string actualGCodeSettings);
        double GetProgrammedSpeed();
        double GetProgrammedFeed();
        int GetCurrentToolNumber();
        int G43Active();
        int G95Active();
        int GetCurInterpreterLineNumber();
        int GetCurInterpreterLineText(out string text);
        int CurrentInterpreterLineContainsToolChange();

    }
    public interface IG_StatusErrorSafety
    {
        int GetSwLimitError();
        int GetFifoError();
        int GetEMStopActive();
        int GetAllAxesHomed();
        int GetSafetyMode();
    }
    public interface IG_Kinematics
    {
        CncKinematicsType KinGetActiveType();
        int KinActivate(int active);
        int KinInit();
        int KinControl(KinControlId controlID, KinControldata pControlData);
        CncVector KinGetARotationPoint();
    }
    public interface IG_StatusItemsIO
    {
        string GetIOName(CncIoId id);
        int GetOutput(CncIoId id);
        int GetOutputRaw(CncIoId id);
        int GetGPIOOutput(int gpioCardIndex, CncGpioId ioId);
        int GetInput(CncIoId id);
        int GetInputRaw(CncIoId id);
        int GetGPIOInput(int gpioCardIndex, CncGpioId ioId);
        CncRc SetOutput(CncIoId id, int value);
        CncRc SetOutputRaw(CncIoId id, int value);
        CncRc SetGPIOOutput(int gpioCardIndex, CncGpioId ioId, int value);
        CncRc CheckStartConditionOK(int generateMessage, int ignoreHoming,out int result);
        CncRc SetSpindleOutput(int onOff, int direction, double absSpeed);

    }
    public interface IG_LogMessagesRealtime
    {
        CncRc LogFifoGet(CncLogMessage data);
        CncRc PosFifoGet(CncPosFifoData data);
        CncRc GraphFifoGet(CncGraphFifoData data);

    }
    public interface IG_CommandsJobInterpreter
    {
        CncRc Reset();
        CncRc Reset2(uint resetFlags);
        CncRc RunSingleLine(string text);
        int WaitSingleLine(CncKeepUiAliveFunction pKeepAlive);
        CncRc CncLoadJob(string fileName);
        CncRc RunOrResumeJob();
        CncRc StartRenderGraph(int outLines, int contour);
        CncRc StartRenderSearch(int outLines, int contour, int lineNr, int toolNr, int arrayX, int arrayY);
        CncRc RewindJob();
        CncRc AbortJob();
        CncRc SetJobArrayParameters(CncCmdArrayData runJobData);
        CncRc GetJobArrayParameters(CncCmdArrayData runJobData);
        CncVector GetJobMaterialSize();
        CncRc GetJobFiducual(int n, CncFiducialData fiducial);
        CncRc EnableBlockDelete(int enable);
        int GetBlocDelete();
        CncRc EnableOptionalStop(int enable);
        CncRc SingleStepMode(int enable);
        int GetSingleStepMode();
        CncRc SetExtraJobOptions(string extraLine, int doRepeat, uint numberOfRepeats);
        void GetExtraJobOptions(out string extraLine, out int doRepeat, out uint numberOfRepeats);
        CncRc SetSimulationMode(int enable);
        int GetSimulationMode();
        CncRc SetFeedOverride(double factor);
        CncRc SetArcFeedOverride(double factor);
        double GetActualFeedOverride();
        double GetActualArcFeedOverride();
        double GetActualFeed();
        CncRc SetSpeedOverride(double factor);
        double GetActualSpeedOverride();
        double GetActualSpeed();
        CncRc FindFirstJobLine(out string text, out int endOfJob, out int totNumOfLines);
        CncRc FindFirstJobLineF(out string text, out int endOfJob);
        CncRc FindNextJobLine(out string text, out int endOfJob);
        CncRc FindNextJobLineF(out string text, out int endOfJob);


    }
}
