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
            // 一次性迁移：把老版本 INI 固定字段转成带历史名称的通道（仅导入已配置探头的）。
            // 新版本通道列表无任何预设语义字段，历史名称只在此迁移路径中保留。
            var channels = new List<TemperatureChannelSetting>();

            void Add(string key, string name, string probe)
            {
                if (string.IsNullOrWhiteSpace(probe))
                {
                    return;
                }
                channels.Add(new TemperatureChannelSetting
                {
                    RoleKey = key,
                    RoleName = name,
                    Title = name,
                    Probe = probe,
                    IsActive = true,
                });
            }

            Add("WindingA", "绕组A", WindingA);
            Add("WindingB", "绕组B", WindingB);
            Add("WindingC", "绕组C", WindingC);
            Add("Core", "铁心", Core);
            Add("EnvA", "环境A", EnvA);
            Add("EnvB", "环境B", EnvB);
            Add("EnvC", "环境C", EnvC);
            Add("EnvD", "环境D", EnvD);
            Add("Outlet1", "出风口温度1", SplitValue(OutletTemperature, 0));
            Add("Outlet2", "出风口温度2", SplitValue(OutletTemperature, 1));
            Add("Outlet3", "出风口温度3", SplitValue(OutletTemperature, 2));
            Add("Outlet4", "出风口温度4", SplitValue(OutletTemperature, 3));
            Add("Outlet5", "出风口温度5", SplitValue(OutletTemperature, 4));
            Add("Outlet6", "出风口温度6", SplitValue(OutletTemperature, 5));
            Add("Inlet1", "进风口温度1", SplitValue(InletTemperature, 0));
            Add("Inlet2", "进风口温度2", SplitValue(InletTemperature, 1));
            Add("Inlet3", "进风口温度3", SplitValue(InletTemperature, 2));
            Add("TopTemperature", "壳内顶部温度", TopTemperature);

            var extensions = string.IsNullOrWhiteSpace(ExtensionSlots)
                ? Array.Empty<string>()
                : ExtensionSlots.Split(',');
            Add("Extension1", "额外温度1", SplitValue(extensions, 0));
            Add("Extension2", "额外温度2", SplitValue(extensions, 1));
            Add("Extension3", "额外温度3", SplitValue(extensions, 2));
            Add("Extension4", "额外温度4", SplitValue(extensions, 3));
            Add("Extension5", "额外温度5", SplitValue(extensions, 4));
            Add("Extension6", "额外温度6", SplitValue(extensions, 5));
            Add("Extension7", "额外温度7", SplitValue(extensions, 6));
            Add("Extension8", "额外温度8", SplitValue(extensions, 7));
            Add("Extension9", "额外温度9", SplitValue(extensions, 8));

            return channels;
        }

        private static void SyncTemperatureChannels()
        {
            if (TemperatureChannels == null || TemperatureChannels.Count == 0)
            {
                TemperatureChannels = new List<TemperatureChannelSetting>();
            }

            TemperatureChannelsJson = JsonSerializer.Serialize(TemperatureChannels);

            var lookup = TemperatureChannels.ToDictionary(item => item.RoleKey, item => item);
            // 旧 INI 固定字段已弃用：仅当存在同 RoleKey 的通道时才回写，
            // 否则保留原值，避免清空未迁移的历史配置。
            WindingA = SyncProbe(lookup, "WindingA", WindingA);
            WindingB = SyncProbe(lookup, "WindingB", WindingB);
            WindingC = SyncProbe(lookup, "WindingC", WindingC);
            Core = SyncProbe(lookup, "Core", Core);
            EnvA = SyncProbe(lookup, "EnvA", EnvA);
            EnvB = SyncProbe(lookup, "EnvB", EnvB);
            EnvC = SyncProbe(lookup, "EnvC", EnvC);
            EnvD = SyncProbe(lookup, "EnvD", EnvD);
            OutletTemperature = SyncJoined(lookup, new[] { "Outlet1", "Outlet2", "Outlet3", "Outlet4", "Outlet5", "Outlet6" }, OutletTemperature);
            InletTemperature = SyncJoined(lookup, new[] { "Inlet1", "Inlet2", "Inlet3" }, InletTemperature);
            TopTemperature = SyncProbe(lookup, "TopTemperature", TopTemperature);
            ExtensionSlots = SyncJoined(lookup, new[] { "Extension1", "Extension2", "Extension3", "Extension4", "Extension5", "Extension6", "Extension7", "Extension8", "Extension9" }, ExtensionSlots);
            TPSlots = string.Join(",", TemperatureChannels
                .Where(item => item.IsActive && !string.IsNullOrWhiteSpace(item.Probe))
                .Select(item => item.Probe));
        }

        // 存在同 RoleKey 通道时回写其探头，否则保留 current 原值（无损）
        private static string SyncProbe(Dictionary<string, TemperatureChannelSetting> lookup, string roleKey, string current)
        {
            return lookup.TryGetValue(roleKey, out var channel) ? (channel.Probe ?? string.Empty) : current;
        }

        // 拼接多个 RoleKey 的探头；若全部缺失则保留 current 原值
        private static string SyncJoined(Dictionary<string, TemperatureChannelSetting> lookup, string[] roleKeys, string current)
        {
            if (!roleKeys.Any(k => lookup.ContainsKey(k)))
            {
                return current;
            }
            return string.Join(",", roleKeys
                .Where(k => lookup.ContainsKey(k))
                .Select(k => lookup[k].Probe ?? string.Empty));
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
