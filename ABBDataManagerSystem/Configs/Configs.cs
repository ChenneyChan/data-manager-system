using System.Runtime.InteropServices;
using System.Text;

namespace ABBDataManagerSystem.Configs
{
    public class Configs
    {
        public static bool UsingSqlite = false;

        //public static UserInfo? CurrentUser { set; get; } = null;

        public static TestingType TestingType { set; get; } = TestingType.DryTypeTransformerTesting;

        public static string WorkflowID = "SHUILENG_002";
        public static string ProductUserName = "";
        public static string ProductFigureNo = "";

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
        #endregion

        #region 温度检测设置
        public static string TPSerialPort { set; get; } = string.Empty;
        public static string TPSerialBoundRate { set; get; } = string.Empty;
        public static string TPIPAddress { set; get; } = string.Empty;
        public static int TPPort{ set; get; } = 0;
        public static int TPUsingSerialPort { set; get; } = 0;
        public static string TPSlots { set; get; } = string.Empty;
        public static string TPInterval { set; get; } = string.Empty;

        public static string WindingA {  set; get; } = string.Empty;
        public static string WindingB {  set; get; } = string.Empty;
        public static string WindingC {  set; get; } = string.Empty;
        public static string Core {  set; get; } = string.Empty;
        public static string EnvA {  set; get; } = string.Empty;
        public static string EnvB {  set; get; } = string.Empty;
        public static string EnvC {  set; get; } = string.Empty;
        public static string EnvD {  set; get; } = string.Empty;
        #endregion

        #region 常规设置
        public static int WorkStationNo = 1;
        #endregion

        #region 读写配置
        static string INIPATH = Utils.GetUserPath() + "\\ABBReportSystemConfig.ini";
        static string INIPDSerial = "PartialDischargeSerial";
        static string INIDatabase = "Database";
        static string INITemperature = "Temperature";
        static string INIPowerAnalyzer = "PowerAnalyzer";
        static string INICommon = "Common";

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
            #endregion

            // 功率分析仪
            GetPrivateProfileString(INIPowerAnalyzer, "VT", "", buff, 32, INIPATH);
            VT = Utils.ParseFloatNull(buff.ToString());
            GetPrivateProfileString(INIPowerAnalyzer, "CT", "", buff, 32, INIPATH);
            CT = Utils.ParseFloatNull(buff.ToString());

            GetPrivateProfileString(INICommon, "WorkStationNo", "", buff, 32, INIPATH);
            WorkStationNo = Utils.ParseIntNull(buff.ToString()) ?? 1;
        }

        public static void SaveToFile()
        {
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

            // 功率分析仪
            WritePrivateProfileString(INIPowerAnalyzer, "VT", VT != null ? Utils.FloatFormat((float)VT, 4) : "" , INIPATH);
            WritePrivateProfileString(INIPowerAnalyzer, "CT", CT != null ? Utils.FloatFormat((float)CT, 4) : "" , INIPATH);

            WritePrivateProfileString(INICommon, "WorkStationNo", WorkStationNo.ToString(), INIPATH);
        }

        #endregion
    }
}
