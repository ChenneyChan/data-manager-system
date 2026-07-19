using ABBDataManagerSystem.Bean.Base;
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;
using System.Text.Json;

namespace ABBDataManagerSystem.Configs
{
    public class Configs
    {
        public static bool UsingSqlite = false;

        //public static UserInfo? CurrentUser { set; get; } = null;

        public static TestingType TestingType { set; get; } = TestingType.DryTypeTransformerTesting;

        public static WorkflowInfo? WorkflowInfo { set; get; } = null;
        public static string WorkflowID = "";
        public static string ProductUserName = "";
        public static string ProductFigureNo = "";
        public static bool IsEnableTesting = false;

        #region DB设置
        public static string Host { get; set; } = string.Empty;

        public static int Port { get; set; } = 0;

        public static string Username { get; set; } = string.Empty;

        public static string Password { get; set; } = string.Empty;

        public static string DatabaseName { get; set; } = string.Empty;

        #endregion


        #region PD串口设置

        public static string PDSerialPort { set; get; } = string.Empty;
        public static string PDSerialBoudRate { set; get; } = string.Empty;
        public static string PDInterval { set; get; } = string.Empty;

        #endregion

        #region 功率分析仪设置
        public static float? VT { set; get; } = null;  
        public static float? CT { set; get; } = null;

        public static string cbVT = "";
        public static string cbCT = "";
        public static string PowerAnalyzerIP { set; get; } = string.Empty;
        #endregion

        #region 温度检测设置
        public static string TPSerialPort { set; get; } = string.Empty;
        public static string TPSerialBoundRate { set; get; } = string.Empty;
        public static string TPIPAddress { set; get; } = string.Empty;
        public static int TPPort{ set; get; } = 0;
        public static int TPUsingSerialPort { set; get; } = 0;
        public static string TPSlots { set; get; } = string.Empty;
        public static string TPInterval { set; get; } = string.Empty;
        
        public static bool TPIsSimulate { set; get; } = false;

        public static string WindingA {  set; get; } = string.Empty;
        public static string WindingB {  set; get; } = string.Empty;
        public static string WindingC {  set; get; } = string.Empty;
        public static string Core {  set; get; } = string.Empty;
        public static string EnvA {  set; get; } = string.Empty;
        public static string EnvB {  set; get; } = string.Empty;
        public static string EnvC {  set; get; } = string.Empty;
        public static string EnvD {  set; get; } = string.Empty;
        public static string OutletTemperature { set; get; } = string.Empty; // 出风口温度
        public static string InletTemperature { set; get; } = string.Empty;   // 进风口温度
        public static string TopTemperature { set; get; } = string.Empty;
        public static string ExtensionSlots { set; get; } = string.Empty;
        public static string TemperatureChannelsJson { set; get; } = string.Empty;
        public static List<TemperatureChannelSetting> TemperatureChannels { set; get; } = TemperatureChannelCatalog.CreateDefaultChannels();
        public static float TemperatureThreshold { set; get; } = 120f;
        #endregion

        #region 20W设置
        public static string SerialPort20W { set; get; } = string.Empty;
        public static string SerialBoundRate20W { set; get; } = string.Empty;
        #endregion

        #region 20E设置
        public static string SerialPort20E { set; get; } = string.Empty;
        public static string SerialBoundRate20E { set; get; } = string.Empty;
        public static string Current20E { set; get; } = string.Empty;
        #endregion

        #region 50E设置
        public static string SerialPort50E { set; get; } = string.Empty;
        public static string SerialBoundRate50E { set; get; } = string.Empty;
        public static string Current50E { set; get; } = string.Empty;
        #endregion

        #region JYT-A设置
        public static string SerialPortJYTA { set; get; } = string.Empty;
        public static string SerialBoundRateJYTA { set; get; } = string.Empty;
        #endregion

        #region 水冷设备设置
        public static int CoolDevice1Port { set; get; } = 8877;
        public static int CoolDevice2Port { set; get; } = 8878;
        public static int CoolDeviceSelectedIndex { set; get; } = 0;
        #endregion

        #region 常规设置
        #region PLC报警设置
        public static string PLCAlertIP { set; get; } = string.Empty;
        public static int PLCAlertRack { set; get; } = 0;
        public static int PLCAlertSlot { set; get; } = 1;
        public static string PLCAlertAddress { set; get; } = "DB1.DBD136";
        #endregion

        public static int WorkStationNo = 1;
        public static bool IsEnableVerboseDebug = false;
        public static bool IsEnableRatioInputMode = false;
        public static bool IsShowRatioInputControls = false;
        public static bool IsShowRealTimeTemperature = true;
        public static bool IsShowTemperatureChart = true;
        #endregion

        #region 读写配置
        static string INIPATH = Utils.GetUserPath() + "\\ABBReportSystemConfig.ini";
        static string INIPDSerial = "PartialDischargeSerial";
        static string INIDatabase = "Database";
        static string INITemperature = "Temperature";
        static string INIPowerAnalyzer = "PowerAnalyzer";
        static string INICommon = "Common";
        static string INIJinYuan20W = "JinYuan20W";
        static string INIJinYuan20E = "JinYuan20E";
        static string INIJinYuan50E = "JinYuan50E";
        static string INIJinYuanJYTA = "JinYuanJYTA";
        static string INICoolDevice = "CoolDevice";
        static string INIPLCAlert = "PLCAlert";

        [DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileString")]
        private static extern uint GetPrivateProfileString(
            string lpApplicationName
            , string lpKeyName
            , string lpDefault
            , System.Text.StringBuilder StringBuilder
            , uint nSize
            , string lpFileName);

        [DllImport("kernel32.dll", EntryPoint = "WritePrivateProfileString")]
        private static extern uint WritePrivateProfileString(
            string lpApplicationName
            , string lpEntryName
            , string lpEntryString
            , string lpFileName);

        public static void LoadFromFile()
        {
            StringBuilder buff = new StringBuilder(128);
            StringBuilder jsonBuff = new StringBuilder(8192);
            GetPrivateProfileString(INIPDSerial, "SerialPort", "", buff, 16, INIPATH);
            PDSerialPort = buff.ToString();
            GetPrivateProfileString(INIPDSerial, "SerialBoudRate", "", buff, 16, INIPATH);
            PDSerialBoudRate = buff.ToString();
            GetPrivateProfileString(INIPDSerial, "Interval", "", buff, 16, INIPATH);
            PDInterval = buff.ToString();

            GetPrivateProfileString(INIDatabase, "Host", "", buff, 16, INIPATH);
            Host = buff.ToString();
            GetPrivateProfileString(INIDatabase, "Port", "", buff, 16, INIPATH);
            Port = Utils.ParseInt(buff.ToString(), 3306);
            GetPrivateProfileString(INIDatabase, "Username", "", buff, 16, INIPATH);
            Username = buff.ToString();
            GetPrivateProfileString(INIDatabase, "Password", "", buff, 16, INIPATH);
            Password = buff.ToString();
            GetPrivateProfileString(INIDatabase, "DatabaseName", "", buff, 16, INIPATH);
            DatabaseName = buff.ToString();

            #region 温度测试仪
            GetPrivateProfileString(INITemperature, "SerialPort", "", buff, 16, INIPATH);
            TPSerialPort = buff.ToString();
            GetPrivateProfileString(INITemperature, "SerialBoudRate", "", buff, 16, INIPATH);
            TPSerialBoundRate = buff.ToString();
            GetPrivateProfileString(INITemperature, "IPAddress", "", buff, 32, INIPATH);
            TPIPAddress = buff.ToString();
            GetPrivateProfileString(INITemperature, "Port", "", buff, 16, INIPATH);
            TPPort = Utils.ParseInt(buff.ToString());
            GetPrivateProfileString(INITemperature, "SelectedSlots", "", buff, 128, INIPATH);
            TPSlots = buff.ToString();
            GetPrivateProfileString(INITemperature, "UsingSerialPort", "", buff, 16, INIPATH);
            TPUsingSerialPort = Utils.ParseInt(buff.ToString());
            GetPrivateProfileString(INITemperature, "Interval", "", buff, 32, INIPATH);
            TPInterval = buff.ToString();
            GetPrivateProfileString(INITemperature, "WindingA", "", buff, 32, INIPATH);
            WindingA = buff.ToString();
            GetPrivateProfileString(INITemperature, "WindingB", "", buff, 32, INIPATH);
            WindingB = buff.ToString();
            GetPrivateProfileString(INITemperature, "WindingC", "", buff, 32, INIPATH);
            WindingC = buff.ToString();
            GetPrivateProfileString(INITemperature, "Core", "", buff, 32, INIPATH);
            Core = buff.ToString();
            GetPrivateProfileString(INITemperature, "EnvA", "", buff, 32, INIPATH);
            EnvA = buff.ToString();
            GetPrivateProfileString(INITemperature, "EnvB", "", buff, 32, INIPATH);
            EnvB = buff.ToString();
            GetPrivateProfileString(INITemperature, "EnvC", "", buff, 32, INIPATH);
            EnvC = buff.ToString();
            GetPrivateProfileString(INITemperature, "EnvD", "", buff, 32, INIPATH);
            EnvD = buff.ToString();
            GetPrivateProfileString(INITemperature, "OutletTemperature", "", buff, 64, INIPATH);
            OutletTemperature = buff.ToString();
            if (OutletTemperature.Trim().Length == 0)
            {
                OutletTemperature = "Slot-1,Slot-2,Slot-3,Slot-4,Slot-5,Slot-6";
            }
            GetPrivateProfileString(INITemperature, "InletTemperature", "", buff, 64, INIPATH);
            InletTemperature = buff.ToString();
            if (InletTemperature.Trim().Length == 0)
            {
                InletTemperature = "Slot-1,Slot-2,Slot-3";
            }
            GetPrivateProfileString(INITemperature, "TopTemperature", "", buff, 32, INIPATH);
            TopTemperature = buff.ToString();
            GetPrivateProfileString(INITemperature, "IsSimulate", "", buff, 32, INIPATH);
            TPIsSimulate = buff.ToString().Trim().ToLower() == "true";
            GetPrivateProfileString(INITemperature, "ExtensionSlots", "", buff, 64, INIPATH);
            ExtensionSlots = buff.ToString();
            GetPrivateProfileString(INITemperature, "TemperatureChannelsJson", "", jsonBuff, 8192, INIPATH);
            TemperatureChannelsJson = jsonBuff.ToString();
            LoadTemperatureChannels();
            GetPrivateProfileString(INITemperature, "TemperatureThreshold", "120", buff, 16, INIPATH);
            TemperatureThreshold = Utils.ParseFloat(buff.ToString(), 120f);
            #endregion

            #region 公共配置
            GetPrivateProfileString(INICommon, "WorkStationNo", "", buff, 32, INIPATH);
            WorkStationNo = Utils.ParseIntNull(buff.ToString()) ?? 1;
            GetPrivateProfileString(INICommon, "WorkflowID", "", buff, 32, INIPATH);
            WorkflowID = buff.ToString();
            GetPrivateProfileString(INICommon, "IsEnableVerboseDebug", "", buff, 32, INIPATH);
            IsEnableVerboseDebug = buff.ToString().Trim().ToLower() == "true";
            GetPrivateProfileString(INICommon, "IsShowRealTimeTemperature", "true", buff, 32, INIPATH);
            IsShowRealTimeTemperature = buff.ToString().Trim().ToLower() == "true";
            GetPrivateProfileString(INICommon, "IsShowTemperatureChart", "true", buff, 32, INIPATH);
            IsShowTemperatureChart = buff.ToString().Trim().ToLower() == "true";
            #endregion

            #region 功率分析仪
            GetPrivateProfileString(INIPowerAnalyzer, "VT", "", buff, 16, INIPATH);
            VT = Utils.ParseFloatNull(buff.ToString());
            GetPrivateProfileString(INIPowerAnalyzer, "CT", "", buff, 16, INIPATH);
            CT = Utils.ParseFloatNull(buff.ToString());
            GetPrivateProfileString(INIPowerAnalyzer, "cbVT", "", buff, 16, INIPATH);
            cbVT = buff.ToString();
            GetPrivateProfileString(INIPowerAnalyzer, "cbCT", "", buff, 16, INIPATH);
            cbCT = buff.ToString();
            GetPrivateProfileString(INIPowerAnalyzer, "IPAddress", "", buff, 32, INIPATH);
            PowerAnalyzerIP = buff.ToString();
            #endregion

            #region JinYuan20W
            GetPrivateProfileString(INIJinYuan20W, "SerialPort", "", buff, 32, INIPATH);
            SerialPort20W = buff.ToString();
            GetPrivateProfileString(INIJinYuan20W, "BoundRate", "", buff, 32, INIPATH);
            SerialBoundRate20W = buff.ToString();
            #endregion

            #region JinYuan20E
            GetPrivateProfileString(INIJinYuan20E, "SerialPort", "", buff, 32, INIPATH);
            SerialPort20E = buff.ToString();
            GetPrivateProfileString(INIJinYuan20E, "BoundRate", "", buff, 32, INIPATH);
            SerialBoundRate20E = buff.ToString();
            #endregion

            #region JinYuan50E
            GetPrivateProfileString(INIJinYuan50E, "SerialPort", "", buff, 32, INIPATH);
            SerialPort50E = buff.ToString();
            GetPrivateProfileString(INIJinYuan50E, "BoundRate", "", buff, 32, INIPATH);
            SerialBoundRate50E = buff.ToString();
            #endregion

            #region JinYuanJYTA
            GetPrivateProfileString(INIJinYuanJYTA, "SerialPort", "", buff, 32, INIPATH);
            SerialPortJYTA = buff.ToString();
            GetPrivateProfileString(INIJinYuanJYTA, "BoundRate", "", buff, 32, INIPATH);
            SerialBoundRateJYTA = buff.ToString();
            GetPrivateProfileString(INIJinYuanJYTA, "Current", "", buff, 32, INIPATH);
            Current20E = buff.ToString();
            #endregion

            #region 水冷设备
            GetPrivateProfileString(INICoolDevice, "Port1", "8877", buff, 16, INIPATH);
            CoolDevice1Port = Utils.ParseInt(buff.ToString(), 8877);
            GetPrivateProfileString(INICoolDevice, "Port2", "8878", buff, 16, INIPATH);
            CoolDevice2Port = Utils.ParseInt(buff.ToString(), 8878);
            GetPrivateProfileString(INICoolDevice, "SelectedIndex", "0", buff, 16, INIPATH);
            CoolDeviceSelectedIndex = Utils.ParseInt(buff.ToString(), 0);
            GetPrivateProfileString(INIPLCAlert, "IP", "", buff, 32, INIPATH);
            PLCAlertIP = buff.ToString();
            GetPrivateProfileString(INIPLCAlert, "Rack", "0", buff, 16, INIPATH);
            PLCAlertRack = Utils.ParseInt(buff.ToString(), 0);
            GetPrivateProfileString(INIPLCAlert, "Slot", "1", buff, 16, INIPATH);
            PLCAlertSlot = Utils.ParseInt(buff.ToString(), 1);
            GetPrivateProfileString(INIPLCAlert, "Address", "DB1.DBD136", buff, 32, INIPATH);
            PLCAlertAddress = buff.ToString();
            #endregion
        }

        public static void SaveToFile()
        {
            SyncTemperatureChannels();

            WritePrivateProfileString(INIPDSerial, "SerialPort", PDSerialPort, INIPATH);
            WritePrivateProfileString(INIPDSerial, "SerialBoudRate", PDSerialBoudRate, INIPATH);
            WritePrivateProfileString(INIPDSerial, "Interval", PDInterval, INIPATH);

            WritePrivateProfileString(INIDatabase, "Host", Host, INIPATH);
            WritePrivateProfileString(INIDatabase, "Port", Port.ToString(), INIPATH);
            WritePrivateProfileString(INIDatabase, "Username", Username, INIPATH);
            WritePrivateProfileString(INIDatabase, "Password", Password, INIPATH);
            WritePrivateProfileString(INIDatabase, "DatabaseName", DatabaseName, INIPATH);

            WritePrivateProfileString(INITemperature, "SerialPort", TPSerialPort, INIPATH);
            WritePrivateProfileString(INITemperature, "SerialBoudRate", TPSerialBoundRate, INIPATH);
            WritePrivateProfileString(INITemperature, "IPAddress", TPIPAddress, INIPATH);
            WritePrivateProfileString(INITemperature, "Port", TPPort.ToString(), INIPATH);
            WritePrivateProfileString(INITemperature, "SelectedSlots", TPSlots, INIPATH);
            WritePrivateProfileString(INITemperature, "UsingSerialPort", TPUsingSerialPort.ToString(), INIPATH);
            WritePrivateProfileString(INITemperature, "Interval", TPInterval, INIPATH);
            WritePrivateProfileString(INITemperature, "WindingA", WindingA, INIPATH);
            WritePrivateProfileString(INITemperature, "WindingB", WindingB, INIPATH);
            WritePrivateProfileString(INITemperature, "WindingC", WindingC, INIPATH);
            WritePrivateProfileString(INITemperature, "Core", Core, INIPATH);
            WritePrivateProfileString(INITemperature, "EnvA", EnvA, INIPATH);
            WritePrivateProfileString(INITemperature, "EnvB", EnvB, INIPATH);
            WritePrivateProfileString(INITemperature, "EnvC", EnvC, INIPATH);
            WritePrivateProfileString(INITemperature, "EnvD", EnvD, INIPATH);
            WritePrivateProfileString(INITemperature, "OutletTemperature", OutletTemperature, INIPATH);
            WritePrivateProfileString(INITemperature, "InletTemperature", InletTemperature, INIPATH);
            WritePrivateProfileString(INITemperature, "TopTemperature", TopTemperature, INIPATH);
            WritePrivateProfileString(INITemperature, "IsSimulate", TPIsSimulate.ToString(), INIPATH);
            WritePrivateProfileString(INITemperature, "ExtensionSlots", ExtensionSlots, INIPATH);
            WritePrivateProfileString(INITemperature, "TemperatureChannelsJson", TemperatureChannelsJson, INIPATH);
            WritePrivateProfileString(INITemperature, "TemperatureThreshold", TemperatureThreshold.ToString(), INIPATH);

            #region 公共配置
            WritePrivateProfileString(INICommon, "WorkStationNo", WorkStationNo.ToString(), INIPATH);
            WritePrivateProfileString(INICommon, "WorkflowID", WorkflowID, INIPATH);
            WritePrivateProfileString(INICommon, "IsEnableVerboseDebug", IsEnableVerboseDebug.ToString(), INIPATH);
            WritePrivateProfileString(INICommon, "IsShowRealTimeTemperature", IsShowRealTimeTemperature.ToString(), INIPATH);
            WritePrivateProfileString(INICommon, "IsShowTemperatureChart", IsShowTemperatureChart.ToString(), INIPATH);
            #endregion

            #region 功率分析仪
            WritePrivateProfileString(INIPowerAnalyzer, "VT", VT != null ? Utils.FloatFormat((float)VT, 4) : "" , INIPATH);
            WritePrivateProfileString(INIPowerAnalyzer, "CT", CT != null ? Utils.FloatFormat((float)CT, 4) : "" , INIPATH);
            WritePrivateProfileString(INIPowerAnalyzer, "cbVT", cbVT, INIPATH);
            WritePrivateProfileString(INIPowerAnalyzer, "cbCT", cbCT, INIPATH);
            WritePrivateProfileString(INIPowerAnalyzer, "IPAddress", PowerAnalyzerIP, INIPATH);
            #endregion

            #region JinaYuan20W
            WritePrivateProfileString(INIJinYuan20W, "SerialPort", SerialPort20W, INIPATH);
            WritePrivateProfileString(INIJinYuan20W, "BoundRate", SerialBoundRate20W, INIPATH);
            #endregion

            #region JinaYuan20E
            WritePrivateProfileString(INIJinYuan20E, "SerialPort", SerialPort20E, INIPATH);
            WritePrivateProfileString(INIJinYuan20E, "BoundRate", SerialBoundRate20E, INIPATH);
            WritePrivateProfileString(INIJinYuan20E, "Current", Current20E, INIPATH);
            #endregion

            #region JinaYuan50E
            WritePrivateProfileString(INIJinYuan50E, "SerialPort", SerialPort50E, INIPATH);
            WritePrivateProfileString(INIJinYuan50E, "BoundRate", SerialBoundRate50E, INIPATH);
            WritePrivateProfileString(INIJinYuan50E, "Current", Current50E, INIPATH);
            #endregion

            #region JinaYuanJYTA
            WritePrivateProfileString(INIJinYuanJYTA, "SerialPort", SerialPortJYTA, INIPATH);
            WritePrivateProfileString(INIJinYuanJYTA, "BoundRate", SerialBoundRateJYTA, INIPATH);
            #endregion

            #region 水冷设备
            WritePrivateProfileString(INICoolDevice, "Port1", CoolDevice1Port.ToString(), INIPATH);
            WritePrivateProfileString(INICoolDevice, "Port2", CoolDevice2Port.ToString(), INIPATH);
            WritePrivateProfileString(INICoolDevice, "SelectedIndex", CoolDeviceSelectedIndex.ToString(), INIPATH);
            WritePrivateProfileString(INIPLCAlert, "IP", PLCAlertIP, INIPATH);
            WritePrivateProfileString(INIPLCAlert, "Rack", PLCAlertRack.ToString(), INIPATH);
            WritePrivateProfileString(INIPLCAlert, "Slot", PLCAlertSlot.ToString(), INIPATH);
            WritePrivateProfileString(INIPLCAlert, "Address", PLCAlertAddress, INIPATH);
            #endregion
        }

        private static void LoadTemperatureChannels()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(TemperatureChannelsJson))
                {
                    var saved = JsonSerializer.Deserialize<List<TemperatureChannelSetting>>(TemperatureChannelsJson);
                    TemperatureChannels = TemperatureChannelCatalog.NormalizeChannels(saved);
                }
                else
                {
                    TemperatureChannels = TemperatureChannelCatalog.NormalizeChannels(BuildChannelsFromLegacy());
                }
            }
            catch
            {
                TemperatureChannels = TemperatureChannelCatalog.NormalizeChannels(BuildChannelsFromLegacy());
            }

            SyncTemperatureChannels();
        }

        private static List<TemperatureChannelSetting> BuildChannelsFromLegacy()
        {
            var channels = TemperatureChannelCatalog.CreateDefaultChannels();
            var lookup = channels.ToDictionary(item => item.RoleKey, item => item);

            SetProbe(lookup, "WindingA", WindingA);
            SetProbe(lookup, "WindingB", WindingB);
            SetProbe(lookup, "WindingC", WindingC);
            SetProbe(lookup, "Core", Core);
            SetProbe(lookup, "EnvA", EnvA);
            SetProbe(lookup, "EnvB", EnvB);
            SetProbe(lookup, "EnvC", EnvC);
            SetProbe(lookup, "EnvD", EnvD);
            SetProbe(lookup, "Outlet1", SplitValue(OutletTemperature, 0));
            SetProbe(lookup, "Outlet2", SplitValue(OutletTemperature, 1));
            SetProbe(lookup, "Outlet3", SplitValue(OutletTemperature, 2));
            SetProbe(lookup, "Outlet4", SplitValue(OutletTemperature, 3));
            SetProbe(lookup, "Outlet5", SplitValue(OutletTemperature, 4));
            SetProbe(lookup, "Outlet6", SplitValue(OutletTemperature, 5));
            SetProbe(lookup, "Inlet1", SplitValue(InletTemperature, 0));
            SetProbe(lookup, "Inlet2", SplitValue(InletTemperature, 1));
            SetProbe(lookup, "Inlet3", SplitValue(InletTemperature, 2));
            SetProbe(lookup, "TopTemperature", TopTemperature);

            var extensions = string.IsNullOrWhiteSpace(ExtensionSlots)
                ? Array.Empty<string>()
                : ExtensionSlots.Split(',');
            SetProbe(lookup, "Extension1", SplitValue(extensions, 0));
            SetProbe(lookup, "Extension2", SplitValue(extensions, 1));
            SetProbe(lookup, "Extension3", SplitValue(extensions, 2));
            SetProbe(lookup, "Extension4", SplitValue(extensions, 3));
            SetProbe(lookup, "Extension5", SplitValue(extensions, 4));
            SetProbe(lookup, "Extension6", SplitValue(extensions, 5));
            SetProbe(lookup, "Extension7", SplitValue(extensions, 6));
            SetProbe(lookup, "Extension8", SplitValue(extensions, 7));
            SetProbe(lookup, "Extension9", SplitValue(extensions, 8));

            foreach (var channel in channels)
            {
                channel.IsActive = !string.IsNullOrWhiteSpace(channel.Probe);
            }

            return channels;
        }

        private static void SyncTemperatureChannels()
        {
            if (TemperatureChannels == null || TemperatureChannels.Count == 0)
            {
                TemperatureChannels = TemperatureChannelCatalog.CreateDefaultChannels();
            }

            TemperatureChannelsJson = JsonSerializer.Serialize(TemperatureChannels);

            var lookup = TemperatureChannels.ToDictionary(item => item.RoleKey, item => item);
            WindingA = GetProbe(lookup, "WindingA");
            WindingB = GetProbe(lookup, "WindingB");
            WindingC = GetProbe(lookup, "WindingC");
            Core = GetProbe(lookup, "Core");
            EnvA = GetProbe(lookup, "EnvA");
            EnvB = GetProbe(lookup, "EnvB");
            EnvC = GetProbe(lookup, "EnvC");
            EnvD = GetProbe(lookup, "EnvD");
            OutletTemperature = string.Join(",", new[]
            {
                GetProbe(lookup, "Outlet1"),
                GetProbe(lookup, "Outlet2"),
                GetProbe(lookup, "Outlet3"),
                GetProbe(lookup, "Outlet4"),
                GetProbe(lookup, "Outlet5"),
                GetProbe(lookup, "Outlet6"),
            });
            InletTemperature = string.Join(",", new[]
            {
                GetProbe(lookup, "Inlet1"),
                GetProbe(lookup, "Inlet2"),
                GetProbe(lookup, "Inlet3"),
            });
            TopTemperature = GetProbe(lookup, "TopTemperature");
            ExtensionSlots = string.Join(",", new[]
            {
                GetProbe(lookup, "Extension1"),
                GetProbe(lookup, "Extension2"),
                GetProbe(lookup, "Extension3"),
                GetProbe(lookup, "Extension4"),
                GetProbe(lookup, "Extension5"),
                GetProbe(lookup, "Extension6"),
                GetProbe(lookup, "Extension7"),
                GetProbe(lookup, "Extension8"),
                GetProbe(lookup, "Extension9"),
            });
            TPSlots = string.Join(",", TemperatureChannels
                .Where(item => item.IsActive && !string.IsNullOrWhiteSpace(item.Probe))
                .Select(item => item.Probe));
        }

        private static void SetProbe(Dictionary<string, TemperatureChannelSetting> lookup, string roleKey, string probe)
        {
            if (lookup.TryGetValue(roleKey, out var channel))
            {
                channel.Probe = probe ?? string.Empty;
            }
        }

        private static string GetProbe(Dictionary<string, TemperatureChannelSetting> lookup, string roleKey)
        {
            return lookup.TryGetValue(roleKey, out var channel) ? channel.Probe ?? string.Empty : string.Empty;
        }

        private static string SplitValue(string? value, int index)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }
            var values = value.Split(',');
            if (index < 0 || index >= values.Length)
            {
                return string.Empty;
            }
            return values[index];
        }

        private static string SplitValue(string[] values, int index)
        {
            if (index < 0 || index >= values.Length)
            {
                return string.Empty;
            }
            return values[index];
        }

        #endregion
    }
}
