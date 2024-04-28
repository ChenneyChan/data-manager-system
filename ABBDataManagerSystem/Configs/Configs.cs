using System.Runtime.InteropServices;
using System.Text;

namespace ABBDataManagerSystem.Configs
{
    public class Configs
    {
        public static bool UsingSqlite = false;

        //public static UserInfo? CurrentUser { set; get; } = null;

        public static TestingType TestingType { set; get; } = TestingType.DryTypeTransformerTesting;

        public static string ProductSequence = "";
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


        #region 温度检测设置
        public static string TPSerialPort { set; get; } = string.Empty;
        public static string TPSerialBoundRate { set; get; } = string.Empty;
        public static string TPIPAddress { set; get; } = string.Empty;
        public static int TPPort{ set; get; } = 0;
        public static int TPUsingSerialPort { set; get; } = 0;
        public static string TPSlots { set; get; } = string.Empty;
        public static string TPInterval { set; get; } = string.Empty;
        #endregion

        #region 读写配置
        static string INIPATH = Utils.GetUserPath() + "\\ABBReportSystemConfig.ini";
        static string INIPDSerial = "PartialDischargeSerial";
        static string INIDatabase = "Database";
        static string INITemperature = "Temperature";

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
        }

        #endregion
    }
}
