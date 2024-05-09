using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using TmctlAPINet;
using Yokogawa.Tm.WT1800CommSample.cs;

namespace ABBDataManagerSystem.PowerAnalyzer
{
    /// <summary>
    /// UCDeviceSearch.xaml 的交互逻辑
    /// </summary>
    public partial class PageDeviceSearch : Window
    {
        public PageDeviceSearch()
        {
            InitializeComponent();
        }

        #region Variables
        int m_nEditOrCombo_Ether = 0;   //0:edit 1:combo
        int m_nEditOrCombo_USB = 0; //0:edit 1:combo
        string INIPATH = GetAppPath() + "\\WT1800Demo.ini";
        const string Ini_Sec_Connect = "Connection";
        const string Ini_Key_GPIB_Address = "GIPB_Address";
        const string Ini_Key_Ether_IPAddr = "VXI11_IPAddr";
        const string Ini_Key_USB_Serial = "USBTMC_Serial";

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

        Connection connection = new Connection();
        private static TMCTL tmDev = new TMCTL();
        #endregion



        #region CommDialogLoad
        //********************************************
        ///Window_Loaded
        //********************************************

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> gbibs = new List<string>() {
                "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24",
            "25",
            "26",
            "27",
            "28",
            "29",
            "30" };
            cbGPIB.Items.Clear();
            foreach (var itme in gbibs)
            {
                cbGPIB.Items.Add(itme);
            }
            cbGPIB.SelectedIndex = 0;
            rbGPIB.IsChecked = true;

            rbGPIB.Checked += RbGPIB_Checked;
            rbSerial.Checked += RbUSB_Checked;
            rbEthNet.Checked += RbEthNet_Checked;
        }
        #endregion

        #region GPIBChecked
        //********************************************
        ///GPIBChecked
        //********************************************
        private void RbGPIB_Checked(object sender, RoutedEventArgs e)
        {
            rbEthNet.IsChecked = false;
            rbSerial.IsChecked = false;

            cbGPIB.IsEnabled = true;

            cbSerial.IsEnabled = false;
            btSerachSerial.IsEnabled = false;

            cbIP.IsEnabled = false;
            btSerachIP.IsEnabled = false;
        }
        #endregion

        #region USBChecked
        //********************************************
        ///USBChecked
        //********************************************
        private void RbUSB_Checked(object sender, RoutedEventArgs e)
        {
            rbGPIB.IsChecked = false;
            rbEthNet.IsChecked = false;

            cbGPIB.IsEnabled = false;

            cbSerial.IsEnabled = true;
            btSerachSerial.IsEnabled = true;

            cbIP.IsEnabled = false;
            btSerachIP.IsEnabled = false;
        }
        #endregion

        #region EtherNetChecked
        //********************************************
        ///EtherNetChecked
        //********************************************
        private void RbEthNet_Checked(object sender, RoutedEventArgs e)
        {
            rbGPIB.IsChecked = false;
            rbSerial.IsChecked = false;

            cbGPIB.IsEnabled = false;

            cbSerial.IsEnabled = false;
            btSerachSerial.IsEnabled = false;

            cbIP.IsEnabled = true;
            btSerachIP.IsEnabled = true;
        }
        #endregion

        #region OKButtonClick
        //********************************************
        ///OKButtonClick
        //********************************************
        private void btConfirm_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder decode;
            int len = 256;
            decode = new StringBuilder(256);

            //if (rbGPIB.IsChecked == true)
            //{
            //    //when GPIB is selected, connect with GPIB port.
            //    connection.devAddress = cbGPIB.Text;
            //    connection.wireType = 1;
            //}
            //else if (rbEthNet.IsChecked == true)
            //{
            //    //when Ether is selected, connect with Ether port.
            //    //set the address, username and password.
            //    connection.devAddress = cbIP.Text;
            //    connection.wireType = 8;
            //}
            //else if (rbSerial.IsChecked == true)
            //{
            //    //when USB is selected, connect with USB port.//
            //    connection.GetEncodeSerialNumber(decode, SinglePhaseCmdLen, cbSerial.Text);
            //    connection.devAddress = decode.ToString();
            //    connection.wireType = 7;
            //}
            ////run connection.
            //if (connection.Initialize() == 0)
            //{
            //    WriteConnectSettings();
            //    //if successed, close this form and display main form.
            //    //        this.Close();
            //    //this.Visible = false; // todo
            //    return;
            //}

            // 如下是测试代码
            var window = new Window()
            {
                Width = 1600,
                Height = 800,
                Title = "功率分析仪",
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
            };
            this.Close();
            window.Content = new UCPowerAanlyzer();
            window.ShowDialog();
            //if failed, select to try again or abort.
            //tryAgain = MessageBox.Show("Can not connect with the instrument, try again?", "Connection failed.", MessageBoxButton.OKCancel, MessageBoxImage.Error);
            return;
        }
        #endregion

        #region SearchSerialNoClick
        //********************************************
        ///SearchSerialNo_Click
        //********************************************
        private void btSerachSerial_Click(object sender, RoutedEventArgs e)
        {
            DEVICELIST[] listbuff = new DEVICELIST[128];
            DEVICELIST[] list = new DEVICELIST[128];
            int ret = 0;
            int listindex = 0;
            int num = 0;
            int n = 0;
            StringBuilder decode = new StringBuilder();


            ret = tmDev.SearchDevices(TMCTL.TM_CTL_USBTMC2, listbuff, 128, ref num, null);
            for (n = 0; n < num; n++)
            {
                ret = Check_WTSeries(TMCTL.TM_CTL_USBTMC2, listbuff[n].adr);
                if (ret == 0)
                {
                    list[listindex] = listbuff[n];
                    listindex++;
                }
            }
            if (listindex > 0)
            {
                MessageBox.Show("Device is found", "Search Result", MessageBoxButton.OK, MessageBoxImage.Information);
                if (listindex == 1)
                {
                    ret = tmDev.DecodeSerialNumber(decode, 256, list[0].adr);
                    cbSerial.Items.Clear();
                    cbSerial.Items.Add(decode.ToString());
                }
                else
                {
                    cbSerial.Items.Clear();
                    for (n = 0; n < listindex; n++)
                    {
                        ret = tmDev.DecodeSerialNumber(decode, 256, list[n].adr);
                        if (ret == 0)
                        {
                            cbSerial.Items.Add(decode);
                        }
                    }
                    cbSerial.SelectedIndex = 0;
                    m_nEditOrCombo_USB = 1;
                }
            }
            else
            {
                MessageBox.Show("Device is not found", "Search Result", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        #endregion

        #region SearchIPAddrClick
        //********************************************
        ///SearchIPAddrClick
        //********************************************
        private void btSerachIP_Click(object sender, RoutedEventArgs e)
        {
            DEVICELIST[] listbuff = new DEVICELIST[128];
            DEVICELIST[] list = new DEVICELIST[128];
            int listindex = 0;
            int ret = 0;
            int num = 0;
            int n = 0;

            ret = tmDev.SearchDevices(TMCTL.TM_CTL_VXI11, listbuff, 128, ref num, null);
            for (n = 0; n < num; n++)
            {
                ret = Check_WTSeries(TMCTL.TM_CTL_VXI11, listbuff[n].adr);
                if (ret == 0)
                {
                    list[listindex] = listbuff[n];
                    listindex++;
                }
            }
            if (listindex > 0)
            {
                cbIP.Items.Clear();
                MessageBox.Show("Device is found", "Search Result", MessageBoxButton.OK, MessageBoxImage.Information);
                for (n = 0; n < listindex; n++)
                {
                    cbIP.Items.Add(list[n].adr);
                }
            }
            else
            {
                MessageBox.Show("Device is not found", "Search Result", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        #endregion

        #region WriteConnectSettings
        //********************************************
        ///WriteConnectSettings
        //********************************************
        private void WriteConnectSettings()
        {
            string buff;
            if (rbGPIB.IsChecked == true)
            {
                WritePrivateProfileString(Ini_Sec_Connect, Ini_Key_GPIB_Address, cbGPIB.Text, INIPATH);
            }
            else if (rbSerial.IsChecked == true)
            {
                buff = cbSerial.Text;
                WritePrivateProfileString(Ini_Sec_Connect, Ini_Key_USB_Serial, buff, INIPATH);
            }
            else if (rbEthNet.IsChecked == true)
            {
                buff = cbIP.Text;
                WritePrivateProfileString(Ini_Sec_Connect, Ini_Key_Ether_IPAddr, buff, INIPATH);
            }
        }
        #endregion

        #region GetConnectSettings
        //********************************************
        ///GetConnectSettings
        //********************************************
        private void GetConnectSettings()
        {
            StringBuilder buff = new StringBuilder(16);
            //GPIB
            GetPrivateProfileString(Ini_Sec_Connect, Ini_Key_GPIB_Address, "", buff, 16, INIPATH);
            //AddressCombo.Text = buff.ToString();
            //USBTMC
            GetPrivateProfileString(Ini_Sec_Connect, Ini_Key_USB_Serial, "", buff, 16, INIPATH);
            //UsbSerialNoTextBox.Text = buff.ToString();
            //Ether(VXI11)
            GetPrivateProfileString(Ini_Sec_Connect, Ini_Key_Ether_IPAddr, "", buff, 16, INIPATH);
            //IpAddressText.Text = buff.ToString();
        }
        #endregion

        #region GetAppPath
        //********************************************
        ///GetAppPath
        //********************************************
        public static string GetAppPath()
        {
            return System.IO.Path.GetDirectoryName(
                  System.Reflection.Assembly.GetExecutingAssembly().Location);
        }
        #endregion

        #region Check_WTSeries
        //********************************************
        ///Check_WTSeries
        //********************************************
        public int Check_WTSeries(int wire, string adr)
        {
            int m_iID = -1;
            string model;
            int rtn;//return 0 when successed.

            rtn = tmDev.Initialize(wire, adr, ref m_iID);
            if (rtn != 0)
            {
                return rtn;
            }
            //set terminator of the message.
            rtn = tmDev.SetTerm(m_iID, 2, 1);
            if (rtn != 0)
            {
                tmDev.Finish(m_iID);
                return rtn;
            }
            //timeout settings, 1*100ms
            rtn = tmDev.SetTimeout(m_iID, 1);
            if (rtn != 0)
            {
                tmDev.Finish(m_iID);
                return rtn;
            }
            //test the device module connected.
            rtn = tmDev.Send(m_iID, "*IDN?");
            int maxLength = 256;
            int realLength = 0;
            StringBuilder buf;
            buf = new StringBuilder(256);
            rtn = tmDev.Receive(m_iID, buf, maxLength, ref realLength);
            model = buf.ToString();
            //check WTseries
            if (model.Contains("WT18"))
            {
                rtn = 0;
            }
            else
            {
                rtn = 1;
            }
            //timeout settings, 20*100ms
            tmDev.SetTimeout(m_iID, 20);
            tmDev.Finish(m_iID);
            return rtn;
        }

        #endregion


        private void btCancel_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
