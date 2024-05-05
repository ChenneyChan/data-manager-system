using HandyControl.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Yokogawa.Tm.WT1800CommSample.cs;

namespace ABBDataManagerSystem.PowerAnalyzer
{
    /// <summary>
    /// Define WT1800 Function list structure
    /// </summary>
    #region Funstr
    public struct Funstr
    {
        public string FunctionName;
        public bool ElementFlag;
        public bool OrderFlag;
    }
    #endregion

    /// <summary>
    /// UCPowerAanlyzer.xaml 的交互逻辑
    /// </summary>
    public partial class UCPowerAanlyzer : UserControl
    {
        enum EncodeType { ASCII, BINARY }


        #region Variables
        private readonly string[] errorMsg = new string[14];
        private readonly string[] updateRateList = new string[10];  //update rate combo list(foreach)
        private readonly string[,] voltageList = new string[2, 13]; //voltage combo list(while)
        private readonly string[,] currentList = new string[2, 23]; //current combo list(direct)
        private readonly string[] eList = new string[12];           //element combo list(while)
        public Funstr[] List = new Funstr[119];
        private readonly string[] oList = new string[103];         //order combo list(foreach)
        private readonly string rangeListAuto = "AUTO";            //auto range list item
        private const string MODEL = "WT1806";
        private const int MAX_ELEMENT = 6;
        private const int MAX_LINES = 100;
        private const int MAX_ITEM = 10;
        private const int MAX_FUNCTION = 119;
        private const int MAX_ORDER = 101;
        private const int NORMAL_FUNCTION_TOP = 0;
        private const int NORMAL_FUNCTION_BOTTOM = 66;
        private const int HARM_FUNCTION_TOP = 67;
        private const int HARM_FUNCTION_BOTTOM = 100;
        private const int DELTA_FUNCTION_TOP = 101;
        private const int DELTA_FUNCTION_BOTTOM = 109;
        private const int MOTOR_FUNCTION_TOP = 110;
        private const int MOTOR_FUNCTION_BOTTOM = 116;
        private const int AUX_FUNCTION_TOP = 117;
        private const int AUX_FUNCTION_BOTTOM = 118;
        private int lastElement;
        private string crestFactor;
        public int Currentelement;
        private int HarmonicOption;
        private bool wt1800e_flg;
        private bool wt1800_flg;
        private bool wt1800e_commandTypeEmulate;

        Connection connection = new Connection();
        private DispatcherTimer Timer1;
        private EncodeType encodeType = EncodeType.ASCII;

        #endregion

        public UCPowerAanlyzer()
        {
            InitializeComponent();
            btRequestContinue.Click += BtRequestContinue_Click;
            btRequestSingle.Click += BtRequestSingle_Click;
            btSetUpdateRate.Click += BtSetUpdateRate_Click;
            InitListItem();

            Timer1 = new DispatcherTimer();
            Timer1.Tick += Timer1_Tick; // 注册 Tick 事件处理程序  
            Timer1.Interval = TimeSpan.FromSeconds(1); // 设置时间间隔为1秒  
        }

        private void BtSetUpdateRate_Click(object sender, RoutedEventArgs e)
        {
            Growl.Info("暂未实现！");
        }

        private void BtRequestTimer_Click(object sender, RoutedEventArgs e)
        {
            Growl.Info("暂未实现！");
        }

        private void BtRequestSingle_Click(object sender, RoutedEventArgs e)
        {
            Growl.Info("暂未实现！");
        }

        private void BtRequestContinue_Click(object sender, RoutedEventArgs e)
        {
            Growl.Info("暂未实现！");
        }

        private void btRangeSet_Click(object sender, RoutedEventArgs e)
        {
            RangeSetCommand(3);
        }

        #region Function: InitListItem
        //********************************************
        ///<summary>
        ///Function: InitListItem
        ///</summary>
        //********************************************
        private void InitListItem()
        {
            InitErrorMsg();
            InitUpdateRateList();
            InitVoltageList();
            InitCurrDirList();
            IniteList();
            InitoList();
            InitList();
        }
        private void InitErrorMsg()
        {
            //===========================
            ///Error Code from Tmctl
            //===========================

            //Error Number  Problem                                 Solutions
            errorMsg[0] = "Device not found";            //   2  1 Check the wiring.  
            errorMsg[1] = "Connection to device failed"; //   4  2 Check the wiring.  
            errorMsg[2] = "Device not connected";        //   8  3 Connect the device using the initialization function.  
            errorMsg[3] = "Device already connected";    //  16  4 Two connections cannot be opened.  
            errorMsg[4] = "Incompatible PC";             //  32  5 Check the hardware you are using.  
            errorMsg[5] = "Illegal parameter";           //  64  6 Check parameter type etc. 
            errorMsg[6] = "";
            errorMsg[7] = "Send error";                  // 256  8 Check the wiring, address, and ID.
            errorMsg[8] = "Receive error";               // 512  9 Check whether an error occurred on the device.  
            errorMsg[9] = "Received data not block data";//1024 10
            errorMsg[10] = "";
            errorMsg[11] = "System error";                //4096 12 There is a problem with the operating environment.  
            errorMsg[12] = "Illegal device ID";           //8192 13 Use the ID of the device acquired by the initialization function. 
            errorMsg[13] = "";
        }

        private void InitUpdateRateList()
        {
            //===========================
            ///UpdateRate List
            //===========================
            updateRateList[0] = "50ms";
            updateRateList[1] = "100ms";
            updateRateList[2] = "200ms";
            updateRateList[3] = "500ms";
            updateRateList[4] = "1s";
            updateRateList[5] = "2s";
            updateRateList[6] = "5s";
            updateRateList[7] = "10s";
            updateRateList[8] = "20s";
            updateRateList[9] = "";
        }

        private void InitVoltageList()
        {
            //===========================
            ///Voltage Range List
            //===========================

            //CrestFactor = 3
            voltageList[0, 0] = "1.5V";
            voltageList[0, 1] = "3V";
            voltageList[0, 2] = "6V";
            voltageList[0, 3] = "10V";
            voltageList[0, 4] = "15V";
            voltageList[0, 5] = "30V";
            voltageList[0, 6] = "60V";
            voltageList[0, 7] = "100V";
            voltageList[0, 8] = "150V";
            voltageList[0, 9] = "300V";
            voltageList[0, 10] = "600V";
            voltageList[0, 11] = "1000V";
            voltageList[0, 12] = "";

            //CrestFactor = 6
            voltageList[1, 0] = "0.75V";
            voltageList[1, 1] = "1.5V";
            voltageList[1, 2] = "3V";
            voltageList[1, 3] = "5V";
            voltageList[1, 4] = "7.5V";
            voltageList[1, 5] = "15V";
            voltageList[1, 6] = "30V";
            voltageList[1, 7] = "50V";
            voltageList[1, 8] = "75V";
            voltageList[1, 9] = "150V";
            voltageList[1, 10] = "300V";
            voltageList[1, 11] = "500V";
            voltageList[1, 12] = "";
        }

        private void InitCurrDirList()
        {
            //===========================
            ///Current Range List
            //===========================

            //CrestFactor = 3
            currentList[0, 0] = "10mA";
            currentList[0, 1] = "20mA";
            currentList[0, 2] = "50mA";
            currentList[0, 3] = "100mA";
            currentList[0, 4] = "200mA";
            currentList[0, 5] = "500mA";
            currentList[0, 6] = "1A";
            currentList[0, 7] = "2A";
            currentList[0, 8] = "5A";
            currentList[0, 9] = "10A";
            currentList[0, 10] = "20A";
            currentList[0, 11] = "50A";

            //Selecting the External Current Sensor Range
            currentList[0, 12] = "50mV";
            currentList[0, 13] = "100mV";
            currentList[0, 14] = "200mV";
            currentList[0, 15] = "500mV";
            currentList[0, 16] = "1V";
            currentList[0, 17] = "2V";
            currentList[0, 18] = "5V";
            currentList[0, 19] = "10V";
            currentList[0, 20] = "";
            currentList[0, 21] = "";
            currentList[0, 22] = "";

            //CrestFactor = 6
            currentList[1, 0] = "5mA";
            currentList[1, 1] = "10mA";
            currentList[1, 2] = "25mA";
            currentList[1, 3] = "50mA";
            currentList[1, 4] = "100mA";
            currentList[1, 5] = "250mA";
            currentList[1, 6] = "0.5A";
            currentList[1, 7] = "1A";
            currentList[1, 8] = "2A";
            currentList[1, 9] = "5A";
            currentList[1, 10] = "10A";
            currentList[1, 11] = "25A";

            //Selecting the External Current Sensor Range
            currentList[1, 12] = "25mV";
            currentList[1, 13] = "50mV";
            currentList[1, 14] = "100mV";
            currentList[1, 15] = "250mV";
            currentList[1, 16] = "500mV";
            currentList[1, 17] = "1V";
            currentList[1, 18] = "2.5V";
            currentList[1, 19] = "5V";
            currentList[1, 20] = "";
            currentList[1, 21] = "";
            currentList[1, 22] = "";
        }

        private void IniteList()
        {
            //===========================
            ///Element List
            //===========================
            eList[0] = "1";
            eList[1] = "2";
            eList[2] = "3";
            eList[3] = "4";
            eList[4] = "5";
            eList[5] = "6";
            eList[6] = "SIGMA";
            eList[7] = "SIGMB";
            eList[8] = "SIGMC";
            eList[9] = "";
            eList[10] = "";
            eList[11] = "";
        }

        private void InitoList()
        {
            //===========================
            ///Order List
            //===========================
            oList[0] = "Total";
            oList[1] = "DC";
            oList[2] = "1";
            oList[3] = "2";
            oList[4] = "3";
            oList[5] = "4";
            oList[6] = "5";
            oList[7] = "6";
            oList[8] = "7";
            oList[9] = "8";
            oList[10] = "9";
            oList[11] = "10";
            oList[12] = "11";
            oList[13] = "12";
            oList[14] = "13";
            oList[15] = "14";
            oList[16] = "15";
            oList[17] = "16";
            oList[18] = "17";
            oList[19] = "18";
            oList[20] = "19";
            oList[21] = "20";
            oList[22] = "21";
            oList[23] = "22";
            oList[24] = "23";
            oList[25] = "24";
            oList[26] = "25";
            oList[27] = "26";
            oList[28] = "27";
            oList[29] = "28";
            oList[30] = "29";
            oList[31] = "30";
            oList[32] = "31";
            oList[33] = "32";
            oList[34] = "33";
            oList[35] = "34";
            oList[36] = "35";
            oList[37] = "36";
            oList[38] = "37";
            oList[39] = "38";
            oList[40] = "39";
            oList[41] = "40";
            oList[42] = "41";
            oList[43] = "42";
            oList[44] = "43";
            oList[45] = "44";
            oList[46] = "45";
            oList[47] = "46";
            oList[48] = "47";
            oList[49] = "48";
            oList[50] = "49";
            oList[51] = "50";
            oList[52] = "51";
            oList[53] = "52";
            oList[54] = "53";
            oList[55] = "54";
            oList[56] = "55";
            oList[57] = "56";
            oList[58] = "57";
            oList[59] = "58";
            oList[60] = "59";
            oList[61] = "60";
            oList[62] = "61";
            oList[63] = "62";
            oList[64] = "63";
            oList[65] = "64";
            oList[66] = "65";
            oList[67] = "66";
            oList[68] = "67";
            oList[69] = "68";
            oList[70] = "69";
            oList[71] = "70";
            oList[72] = "71";
            oList[73] = "72";
            oList[74] = "73";
            oList[75] = "74";
            oList[76] = "75";
            oList[77] = "76";
            oList[78] = "77";
            oList[79] = "78";
            oList[80] = "79";
            oList[81] = "80";
            oList[82] = "81";
            oList[83] = "82";
            oList[84] = "83";
            oList[85] = "84";
            oList[86] = "85";
            oList[87] = "86";
            oList[88] = "87";
            oList[89] = "88";
            oList[90] = "89";
            oList[91] = "90";
            oList[92] = "91";
            oList[93] = "92";
            oList[94] = "93";
            oList[95] = "94";
            oList[96] = "95";
            oList[97] = "96";
            oList[98] = "97";
            oList[99] = "98";
            oList[100] = "99";
            oList[101] = "100";
            oList[102] = "";
        }
        private void InitList()
        {
            //===========================
            //     init Struct
            //===========================
            List[0].FunctionName = "URMS";
            List[0].ElementFlag = true;
            List[0].OrderFlag = false;

            List[1].FunctionName = "UMN";
            List[1].ElementFlag = true;
            List[1].OrderFlag = false;

            List[2].FunctionName = "UDC";
            List[2].ElementFlag = true;
            List[2].OrderFlag = false;

            List[3].FunctionName = "URMN";
            List[3].ElementFlag = true;
            List[3].OrderFlag = false;

            List[4].FunctionName = "UAC";
            List[4].ElementFlag = true;
            List[4].OrderFlag = false;

            List[5].FunctionName = "UPPEAK"; //"U+peak[U+peak]"
            List[5].ElementFlag = true;
            List[5].OrderFlag = false;

            List[6].FunctionName = "UMPEAK"; //"U-peak[U-peak]"
            List[6].ElementFlag = true;
            List[6].OrderFlag = false;

            List[7].FunctionName = "CFU"; //"CfU"
            List[7].ElementFlag = true;
            List[7].OrderFlag = false;

            List[8].FunctionName = "IRMS";
            List[8].ElementFlag = true;
            List[8].OrderFlag = false;

            List[9].FunctionName = "IMN";
            List[9].ElementFlag = true;
            List[9].OrderFlag = false;

            List[10].FunctionName = "IDC";
            List[10].ElementFlag = true;
            List[10].OrderFlag = false;

            List[11].FunctionName = "IRMN";
            List[11].ElementFlag = true;
            List[11].OrderFlag = false;

            List[12].FunctionName = "IAC";
            List[12].ElementFlag = true;
            List[12].OrderFlag = false;

            List[13].FunctionName = "IPPEAK"; //"i +peak"
            List[13].ElementFlag = true;
            List[13].OrderFlag = false;

            List[14].FunctionName = "IMPEAK"; //"i -peak"
            List[14].ElementFlag = true;
            List[14].OrderFlag = false;

            List[15].FunctionName = "CFI"; //"CfI"
            List[15].ElementFlag = true;
            List[15].OrderFlag = false;

            List[16].FunctionName = "P";
            List[16].ElementFlag = true;
            List[16].OrderFlag = false;

            List[17].FunctionName = "S";
            List[17].ElementFlag = true;
            List[17].OrderFlag = false;

            List[18].FunctionName = "Q";
            List[18].ElementFlag = true;
            List[18].OrderFlag = false;

            List[19].FunctionName = "LAMBDA"; //"LAMBda"
            List[19].ElementFlag = true;
            List[19].OrderFlag = false;

            List[20].FunctionName = "PHI";
            List[20].ElementFlag = true;
            List[20].OrderFlag = false;

            List[21].FunctionName = "PC";
            List[21].ElementFlag = true;
            List[21].OrderFlag = false;

            List[22].FunctionName = "PPPEAK"; //"p +peak"
            List[22].ElementFlag = true;
            List[22].OrderFlag = false;

            List[23].FunctionName = "PMPEAK"; //"p -peak"
            List[23].ElementFlag = true;
            List[23].OrderFlag = false;

            List[24].FunctionName = "FU"; //"FreqU[fU]"
            List[24].ElementFlag = true;
            List[24].OrderFlag = false;

            List[25].FunctionName = "FI"; //"FreqI[fI]"
            List[25].ElementFlag = true;
            List[25].OrderFlag = false;

            List[26].FunctionName = "TIME"; //"i-Time"
            List[26].ElementFlag = true;
            List[26].OrderFlag = false;

            List[27].FunctionName = "WH"; //"Wp"
            List[27].ElementFlag = true;
            List[27].OrderFlag = false;

            List[28].FunctionName = "WHP"; //"Wp+"
            List[28].ElementFlag = true;
            List[28].OrderFlag = false;

            List[29].FunctionName = "WHM"; //"Wp-"
            List[29].ElementFlag = true;
            List[29].OrderFlag = false;

            List[30].FunctionName = "AH"; //"q"
            List[30].ElementFlag = true;
            List[30].OrderFlag = false;

            List[31].FunctionName = "AHP"; //"q+"
            List[31].ElementFlag = true;
            List[31].OrderFlag = false;

            List[32].FunctionName = "AHM"; //"q-"
            List[32].ElementFlag = true;
            List[32].OrderFlag = false;

            List[33].FunctionName = "WS"; //"WS"
            List[33].ElementFlag = true;
            List[33].OrderFlag = false;

            List[34].FunctionName = "WQ"; //"WQ"
            List[34].ElementFlag = true;
            List[34].OrderFlag = false;

            List[35].FunctionName = "ETA1"; //"ETA1"
            List[35].ElementFlag = false;
            List[35].OrderFlag = false;

            List[36].FunctionName = "ETA2"; //"ETA2"
            List[36].ElementFlag = false;
            List[36].OrderFlag = false;

            List[37].FunctionName = "ETA3"; //"ETA3"
            List[37].ElementFlag = false;
            List[37].OrderFlag = false;

            List[38].FunctionName = "ETA4"; //"ETA4"
            List[38].ElementFlag = false;
            List[38].OrderFlag = false;

            //-------------F1~F8----------------
            List[39].FunctionName = "F1";
            List[39].ElementFlag = false;
            List[39].OrderFlag = false;

            List[40].FunctionName = "F2";
            List[40].ElementFlag = false;
            List[40].OrderFlag = false;

            List[41].FunctionName = "F3";
            List[41].ElementFlag = false;
            List[41].OrderFlag = false;

            List[42].FunctionName = "F4";
            List[42].ElementFlag = false;
            List[42].OrderFlag = false;

            List[43].FunctionName = "F5";
            List[43].ElementFlag = false;
            List[43].OrderFlag = false;

            List[44].FunctionName = "F6";
            List[44].ElementFlag = false;
            List[44].OrderFlag = false;

            List[45].FunctionName = "F7";
            List[45].ElementFlag = false;
            List[45].OrderFlag = false;

            List[46].FunctionName = "F8";
            List[46].ElementFlag = false;
            List[46].OrderFlag = false;

            List[47].FunctionName = "F9";
            List[47].ElementFlag = false;
            List[47].OrderFlag = false;

            List[48].FunctionName = "F10";
            List[48].ElementFlag = false;
            List[48].OrderFlag = false;

            List[49].FunctionName = "F11";
            List[49].ElementFlag = false;
            List[49].OrderFlag = false;

            List[50].FunctionName = "F12";
            List[50].ElementFlag = false;
            List[50].OrderFlag = false;

            List[51].FunctionName = "F13";
            List[51].ElementFlag = false;
            List[51].OrderFlag = false;

            List[52].FunctionName = "F14";
            List[52].ElementFlag = false;
            List[52].OrderFlag = false;

            List[53].FunctionName = "F15";
            List[53].ElementFlag = false;
            List[53].OrderFlag = false;

            List[54].FunctionName = "F16";
            List[54].ElementFlag = false;
            List[54].OrderFlag = false;

            List[55].FunctionName = "F17";
            List[55].ElementFlag = false;
            List[55].OrderFlag = false;

            List[56].FunctionName = "F18";
            List[56].ElementFlag = false;
            List[56].OrderFlag = false;

            List[57].FunctionName = "F19";
            List[57].ElementFlag = false;
            List[57].OrderFlag = false;

            List[58].FunctionName = "F20";
            List[58].ElementFlag = false;
            List[58].OrderFlag = false;

            //-------------EV1~EV8----------------

            List[59].FunctionName = "EV1";
            List[59].ElementFlag = false;
            List[59].OrderFlag = false;

            List[60].FunctionName = "EV2";
            List[60].ElementFlag = false;
            List[60].OrderFlag = false;

            List[61].FunctionName = "EV3";
            List[61].ElementFlag = false;
            List[61].OrderFlag = false;

            List[62].FunctionName = "EV4";
            List[62].ElementFlag = false;
            List[62].OrderFlag = false;

            List[63].FunctionName = "EV5";
            List[63].ElementFlag = false;
            List[63].OrderFlag = false;

            List[64].FunctionName = "EV6";
            List[64].ElementFlag = false;
            List[64].OrderFlag = false;

            List[65].FunctionName = "EV7";
            List[65].ElementFlag = false;
            List[65].OrderFlag = false;

            List[66].FunctionName = "EV8";
            List[66].ElementFlag = false;
            List[66].OrderFlag = false;

            //-------------'/G6 optional--------------

            List[67].FunctionName = "UK";
            List[67].ElementFlag = true;
            List[67].OrderFlag = true;

            List[68].FunctionName = "IK";
            List[68].ElementFlag = true;
            List[68].OrderFlag = true;

            List[69].FunctionName = "PK";
            List[69].ElementFlag = true;
            List[69].OrderFlag = true;

            List[70].FunctionName = "SK";
            List[70].ElementFlag = true;
            List[70].OrderFlag = true;

            List[71].FunctionName = "QK";
            List[71].ElementFlag = true;
            List[71].OrderFlag = true;

            List[72].FunctionName = "LAMBDAK"; //"兩(K)"
            List[72].ElementFlag = true;
            List[72].OrderFlag = true;

            List[73].FunctionName = "PHIK";
            List[73].ElementFlag = true;
            List[73].OrderFlag = true;

            List[74].FunctionName = "PHIUK";
            List[74].ElementFlag = true;
            List[74].OrderFlag = true;

            List[75].FunctionName = "PHIIK";
            List[75].ElementFlag = true;
            List[75].OrderFlag = true;

            List[76].FunctionName = "ZK";
            List[76].ElementFlag = true;
            List[76].OrderFlag = true;

            List[77].FunctionName = "RSK";
            List[77].ElementFlag = true;
            List[77].OrderFlag = true;

            List[78].FunctionName = "XSK";
            List[78].ElementFlag = true;
            List[78].OrderFlag = true;

            List[79].FunctionName = "RPK";
            List[79].ElementFlag = true;
            List[79].OrderFlag = true;

            List[80].FunctionName = "XPK";
            List[80].ElementFlag = true;
            List[80].OrderFlag = true;

            List[81].FunctionName = "UHDFK";
            List[81].ElementFlag = true;
            List[81].OrderFlag = true;

            List[82].FunctionName = "IHDFK";
            List[82].ElementFlag = true;
            List[82].OrderFlag = true;

            List[83].FunctionName = "PHDFK";
            List[83].ElementFlag = true;
            List[83].OrderFlag = true;

            List[84].FunctionName = "UTHD"; //"Uthd"
            List[84].ElementFlag = true;
            List[84].OrderFlag = false;

            List[85].FunctionName = "ITHD"; //"Ithd"
            List[85].ElementFlag = true;
            List[85].OrderFlag = false;

            List[86].FunctionName = "PTHD"; //"Pthd"
            List[86].ElementFlag = true;
            List[86].OrderFlag = false;

            List[87].FunctionName = "UTHF"; //"Uthf"
            List[87].ElementFlag = true;
            List[87].OrderFlag = false;

            List[88].FunctionName = "ITHF"; //"Ithf"
            List[88].ElementFlag = true;
            List[88].OrderFlag = false;

            List[89].FunctionName = "UTIF"; //"Utif"
            List[89].ElementFlag = true;
            List[89].OrderFlag = false;

            List[90].FunctionName = "ITIF"; //"Itif"
            List[90].ElementFlag = true;
            List[90].OrderFlag = false;

            List[91].FunctionName = "HVF"; //"hvf"
            List[91].ElementFlag = true;
            List[91].OrderFlag = false;

            List[92].FunctionName = "HCF"; //"hcf"
            List[92].ElementFlag = true;
            List[92].OrderFlag = false;

            List[93].FunctionName = "KFACTOR"; //"K-factor"
            List[93].ElementFlag = true;
            List[93].OrderFlag = false;

            List[94].FunctionName = "FPLL1"; //"PllFreq1"
            List[94].ElementFlag = false;
            List[94].OrderFlag = false;

            List[95].FunctionName = "FPLL2"; //"PllFreq2"
            List[95].ElementFlag = false;
            List[95].OrderFlag = false;

            List[96].FunctionName = "PHI_U1U2"; //"PHIU1-U2"
            List[96].ElementFlag = true;
            List[96].OrderFlag = false;

            List[97].FunctionName = "PHI_U1U3"; //"PHIU1-U3"
            List[97].ElementFlag = true;
            List[97].OrderFlag = false;

            List[98].FunctionName = "PHI_U1I1"; //"PHIU1-I1"
            List[98].ElementFlag = true;
            List[98].OrderFlag = false;

            List[99].FunctionName = "PHI_U2I2"; //"PHIU1-I2"
            List[99].ElementFlag = true;
            List[99].OrderFlag = false;

            List[100].FunctionName = "PHI_U3I3"; //"PHIU1-I3
            List[100].ElementFlag = true;
            List[100].OrderFlag = false;

            //---------/DT delta computation version-------

            List[101].FunctionName = "DU1"; //"DELTAU1"
            List[101].ElementFlag = true;
            List[101].OrderFlag = false;

            List[102].FunctionName = "DU2"; //"DELTAU2"
            List[102].ElementFlag = true;
            List[102].OrderFlag = false;

            List[103].FunctionName = "DU3"; //"DELTAU3"
            List[103].ElementFlag = true;
            List[103].OrderFlag = false;

            List[104].FunctionName = "DUS"; //"DELTAUSIG"
            List[104].ElementFlag = true;
            List[104].OrderFlag = false;

            List[105].FunctionName = "DI"; //"DELTAI"
            List[105].ElementFlag = true;
            List[105].OrderFlag = false;

            List[106].FunctionName = "DP1"; //"DELTAP1"
            List[106].ElementFlag = true;
            List[106].OrderFlag = false;

            List[107].FunctionName = "DP2"; //"DELTAP2"
            List[107].ElementFlag = true;
            List[107].OrderFlag = false;

            List[108].FunctionName = "DP3"; //"DELTAP3"
            List[108].ElementFlag = true;
            List[108].OrderFlag = false;

            List[109].FunctionName = "DPS"; //"DELTAPSIG"
            List[109].ElementFlag = true;
            List[109].OrderFlag = false;

            //---------/MTR motor version-------

            List[110].FunctionName = "SPEED"; //"Speed"
            List[110].ElementFlag = false;
            List[110].OrderFlag = false;

            List[111].FunctionName = "TORQUE"; //"Torque"
            List[111].ElementFlag = false;
            List[111].OrderFlag = false;

            List[112].FunctionName = "SYNCSP"; //"SyncSpeed"
            List[112].ElementFlag = false;
            List[112].OrderFlag = false;

            List[113].FunctionName = "SLIP"; //"Slip"
            List[113].ElementFlag = false;
            List[113].OrderFlag = false;

            List[114].FunctionName = "PM"; //"Pm"
            List[114].ElementFlag = false;
            List[114].OrderFlag = false;

            List[115].FunctionName = "EAU"; //"EaI"
            List[115].ElementFlag = true;
            List[115].OrderFlag = false;

            List[116].FunctionName = "EAI"; //"EaI"
            List[116].ElementFlag = true;
            List[116].OrderFlag = false;

            //---------/AUX motor version-------

            List[117].FunctionName = "AUX1"; //"Aux1"
            List[117].ElementFlag = false;
            List[117].OrderFlag = false;

            List[118].FunctionName = "AUX2"; //"Aux2"
            List[118].ElementFlag = false;
            List[118].OrderFlag = false;

        }

        #endregion

        #region UserControl_Loaded
        //******************************************************
        /// <summary>
        /// UserControl_Loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //******************************************************
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ///Set ModelType Display#
            string model = Connection.DevModel;

            //--GET IDN--
            string msg = "*IDN?"; // *IDN? -> YOKOGAWA,WT1806E-5A3-50A3,1234567890,F1.01
            string tem_idn = "";
            SetSendMonitor(msg);
            if (!QueriesData(60, msg, ref tem_idn))
            {
                return;
            }
            SetReceiveMonitor(tem_idn);

            model = CutLeft("\n", ref model);
            tbModelType.Text = model;
            //check model.
            CutLeft(",", ref model);
            int symbol = model.IndexOf("-");
            wt1800_flg = false;
            wt1800e_flg = false;

            if (model.Substring(0, symbol) != "WT1801" && model.Substring(0, symbol) != "WT1802" && model.Substring(0, symbol) != "WT1803"
            && model.Substring(0, symbol) != "WT1804" && model.Substring(0, symbol) != "WT1805" && model.Substring(0, symbol) != "WT1806")
                wt1800_flg = false;
            else
                wt1800_flg = true;

            if (model.Substring(0, symbol) != "WT1801E" && model.Substring(0, symbol) != "WT1802E" && model.Substring(0, symbol) != "WT1803E"
            && model.Substring(0, symbol) != "WT1804E" && model.Substring(0, symbol) != "WT1805E" && model.Substring(0, symbol) != "WT1806E")
                wt1800e_flg = false;
            else
                wt1800e_flg = true;

            if (0 <= tem_idn.IndexOf("EM"))
                wt1800e_commandTypeEmulate = true;
            else
                wt1800e_commandTypeEmulate = false;

            if (wt1800e_commandTypeEmulate == true)
            {
                DispError("it seems CommandType is not 'WT1800E', program may run incorrectly");
            }
            else if ((wt1800_flg == false) && (wt1800e_flg == false))
            {
                DispError("it seems not WT1800/WT1800E, program may run incorrectly!");
            }

            //set unuse element RadioBox disable.
            lastElement = Convert.ToInt32(model.Substring(5, 1));

            ///Queries the CrestFactor#
            string tempFactor = "";
            msg = ":INPUT:CFACTOR?";
            SetSendMonitor(msg);
            if (!QueriesData(20, msg, ref tempFactor))
            {
                return;
            }
            SetReceiveMonitor(tempFactor);
            crestFactor = CutLeft("\n", ref tempFactor);//cut left with LF.

            GetUpdateRate();

            //initialize  Function base on  option
            GetOption();

            ///get current item settings from instrument
            ReadItemSettings();
            return;
        }

        #endregion

        #region UserControl_Unloaded
        //********************************************
        ///Dialog Close
        //********************************************
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //close connection after exiting.
            connection.Finish();
            if (Timer1.IsEnabled)
            {
                Timer1.Stop();
            }
        }
        #endregion

        //#-------------------------#
        ///--Common Functions--
        //#-------------------------#


        #region Function: CutLeft
        //*********************************************
        /// <summary> Function: CutLeft </summary>
        /// <remarks>
        ///cut the left half to outData,
        ///and the right portion remain in inData.
        /// </remarks>
        /// <example>
        ///symbol:"2", in:"12345" => out:"1", in:"345"
        /// </example>
        //*********************************************
        private string CutLeft(string symbol, ref string inData)
        {
            string outData = inData;
            int pos = inData.IndexOf(symbol);
            if (pos == -1)
            {
                //if no symbol, cut with LF.
                pos = inData.IndexOf((char)10);
            }
            if (pos != -1)
            {
                outData = inData.Substring(0, pos);
                inData = inData.Substring(pos + 1);
            }

            //cut data when harmonics mode
            pos = outData.IndexOf(" ");
            if (pos != -1)
            {
                outData = outData.Substring(pos + 1);
            }
            return outData;
        }
        #endregion

        #region Function: QueriesData
        //********************************************
        ///<summary>Function: QueriesData</summary>
        //********************************************
        private bool QueriesData(int maxLength, string msg, ref string data)
        {
            int rtn;
            //Send Command.
            rtn = connection.Send(msg);
            if (rtn != 0)
            {
                DispError(connection.GetLastError());
                return false;
            }

            //Queries Data.
            int realLength = 0;
            rtn = connection.Receive(ref data, maxLength, ref realLength);
            if (rtn != 0)
            {
                DispError(connection.GetLastError());
                return false;
            }
            return true;
        }

        #endregion

        #region Function: SetSendMonitor
        //***********************************************
        ///<summary>Function: SetSendMonitor</summary>
        //***********************************************
        private void SetSendMonitor(string msg)
        {
        }
        #endregion

        #region Function: SetReceiveMonitor
        //*****************************************
        /// <summary>
        /// Function: SetReceiveMonitor
        /// </summary>
        //*****************************************
        private void SetReceiveMonitor(string data)
        {
        }
        #endregion

        #region Function: DisplayError
        //********************************************
        /// <summary> Function: DispError </summary>
        ///<remark>
        ///the errorID is received from tmctl.
        ///get the errorMsg corresponding the errorID.
        ///</remark>
        //********************************************
        private void DispError(int errorID)
        {
            if (errorID == 0)
            {
                ErrInfoText.Text = "Getting error failed!";
                Log.Error("Power Analyzer Error: " + ErrInfoText.Text);
                return;
            }
            int n = 0;
            while (2 << n != errorID)
            {
                n++;
            }
            //set errorMsg to display.
            ErrInfoText.Text = errorMsg[n];
            Log.Error("Power Analyzer Error: " + ErrInfoText.Text);
        }
        private void DispError(string errorInfo)
        {
            //set errorMsg to display.
            ErrInfoText.Text = errorInfo;
            Log.Error("Power Analyzer Error: " + ErrInfoText.Text);

        }
        #endregion

        #region Function: GetItemData
        //********************************************
        /// <summary> Function: GetData </summary>
        //********************************************
        private void GetItemData()
        {
            int n;
            int rtn;
            string msg;

            ///----------------------#get data#
            msg = ":NUMERIC:NORMAL:VALUE?";

            ///----------------------#send message#
            SetSendMonitor(msg);
            //###ASCII:TmcSend(); FLOAT:TmcSendBuLength()###
            if (encodeType == EncodeType.ASCII)
            {
                rtn = connection.Send(msg);
            }
            else
            {
                rtn = connection.SendByLength(msg, msg.Length);
            }
            if (rtn != 0)
            {
                DispError(connection.GetLastError());
                return;
            }

            ///----------------------#receive values#
            int maxLength = 0;
            int realLength = 0;
            string data = "";

            if (encodeType == EncodeType.ASCII)
            {
                ///----------------------#receive values by ASCII#
                //###ASCII:TmcReceive()###
                maxLength = 15 * ItemSettings.Count;
                rtn = connection.Receive(ref data, maxLength, ref realLength);
                if (rtn != 0)
                {
                    DispError(connection.GetLastError());
                    return;
                }
                SetReceiveMonitor(data);
                data = CutLeft("\n", ref data);
            }
            else
            {
                byte[] byteData;
                byte[] bytes = new byte[4];
                ///----------------------#receive values by Float#
                //###FLOAT:TmcReceiveBlock()###
                rtn = connection.ReceiveBlockHeader(ref maxLength);
                if (maxLength < 1)
                {
                    return;
                }
                maxLength += 2;//see tmctl's help
                byteData = new byte[maxLength];

                int isEnd = 0;
                string outputValue = "";
                float floatBuf;
                string temstr = "";
                while (isEnd == 0)
                {
                    rtn = connection.ReceiveBlockData(ref byteData, maxLength, ref realLength, ref isEnd);
                    if (rtn != 0)
                    {
                        DispError(connection.GetLastError());
                        return;
                    }
                    for (n = 0; n <= realLength - 1; n++)
                    {
                        /****************************************************/
                        temstr = byteData[n].ToString("X");
                        if (byteData[n] < 16)
                        {
                            temstr = "0" + temstr;
                        }
                        outputValue = outputValue + temstr;
                        //outputValue = outputValue + (byteData[n].ToString("X"));
                        /*****************************************************/
                    }
                    for (n = 0; n < realLength / 4; n++)
                    {
                        bytes[3] = byteData[n * 4];
                        bytes[2] = byteData[n * 4 + 1];
                        bytes[1] = byteData[n * 4 + 2];
                        bytes[0] = byteData[n * 4 + 3];
                        floatBuf = System.BitConverter.ToSingle(bytes, 0);
                        data += floatBuf.ToString() + ",";
                    }
                }
                SetReceiveMonitor(outputValue);
            }

            for (n = 0; n < ItemSettings.Count; n++)
            {
                //set display
                ItemSettings[n].Value = Utils.ParseFloat(CutLeft(",", ref data));
            }
        }
        #endregion

        //#-------------------------#
        ///--Communications--
        //#-------------------------#

        #region Function: GetRanges
        //********************************************
        /// <summary> Function: GetRanges </summary>
        //********************************************
        private bool GetRanges(int elementIndex)
        {
            int n;
            string msg;

            cbVoltageRange.Items.Clear();
            cbCurrentRange.Items.Clear();

            ///----------------------#Set the Range Lists#
            //when crest factor == 3, set ranges.
            if (crestFactor == "3")
            {
                //#set voltage list#
                n = 0;
                while (voltageList[0, n] != "")
                {
                    cbVoltageRange.Items.Add(voltageList[0, n]);
                    n++;
                }
                cbVoltageRange.Items.Add(rangeListAuto);
                //#set current list#
                n = 0;
                while (currentList[0, n] != "")
                {
                    cbCurrentRange.Items.Add(currentList[0, n]);
                    n++;
                }

                cbCurrentRange.Items.Add(rangeListAuto);
            }
            //when crest factor == 6, set ranges.
            else if ((crestFactor == "6") || (crestFactor == "A6"))
            {
                //#set voltage list#
                n = 0;
                while (voltageList[1, n] != "")
                {
                    cbVoltageRange.Items.Add(voltageList[1, n]);
                    n++;
                }
                cbVoltageRange.Items.Add(rangeListAuto);
                //#set current list#
                n = 0;
                while (currentList[1, n] != "")
                {
                    cbCurrentRange.Items.Add(currentList[1, n]);
                    n++;
                }
                cbCurrentRange.Items.Add(rangeListAuto);
            }

            ///----------------------#Get Voltage Range Settings#
            //###":VOLT:RANG:ELEM1 3.00E+00;ELEM2 1.00E+03"###
            string range = "";
            float floatRange;

            msg = ":INPUT:VOLTAGE:RANGE:ELEMENT" + (elementIndex + 1).ToString() + "?";
            SetSendMonitor(msg);
            if (!QueriesData(50, msg, ref range))
            {
                return false;
            }
            SetReceiveMonitor(range);
            range = CutLeft("\n", ref range);//cut left with LF.
            /***************************************************/
            string auto_tem = "";
            //float  floatRange;
            msg = ":INPUT:VOLTAGE:AUTO:ELEMENT" + (elementIndex + 1).ToString() + "?";
            SetSendMonitor(msg);
            if (!QueriesData(50, msg, ref auto_tem))
            {
                return false;
            }
            SetReceiveMonitor(auto_tem);
            auto_tem = CutLeft("\n", ref auto_tem);//cut left with LF.
            /***************************************************/

            if (auto_tem == "1")
            {
                cbVoltageRange.SelectedItem = rangeListAuto;
            }
            else
            {
                cbVoltageRange.Text = Convert.ToSingle(range).ToString() + "V";
            }

            ///----------------------#Get Current Range Settings#
            msg = ":INPUT:CURRENT:RANGE:ELEMENT" + (elementIndex + 1).ToString() + "?";
            SetSendMonitor(msg);
            if (!QueriesData(50, msg, ref range))
            {
                return false;
            }
            SetReceiveMonitor(range);
            range = CutLeft("\n", ref range);//cut left with LF.

            //wether have external current sensor input
            string external;
            external = "";
            external = CutLeft(",", ref range);//cut left with ",".

            /********************************************************/
            string tem_auto = "";
            msg = ":INPUT:CURRENT:AUTO:ELEMENT" + (elementIndex + 1).ToString() + "?";
            SetSendMonitor(msg);
            if (!QueriesData(50, msg, ref tem_auto))
            {
                return false;
            }
            SetReceiveMonitor(tem_auto);
            tem_auto = CutLeft("\n", ref tem_auto);//cut left with LF.
            /********************************************************/

            if (tem_auto == "1")
            {
                cbCurrentRange.Text = rangeListAuto;
            }
            else
            {
                floatRange = Convert.ToSingle(range);
                if (floatRange < 1)
                {
                    //when "mA/mV" unit, multiply 1k.
                    range = (floatRange * 1000).ToString("n");
                    if (external == "EXT" || external == "EXTERNAL")
                        cbCurrentRange.Text = Convert.ToSingle(range).ToString() + "mV";
                    else
                        cbCurrentRange.Text = Convert.ToSingle(range).ToString() + "mA";
                }
                else
                {
                    if (external == "EXT" || external == "EXTERNAL")
                        cbCurrentRange.Text = Convert.ToSingle(range).ToString() + "V";
                    else
                        cbCurrentRange.Text = Convert.ToSingle(range).ToString() + "A";
                }
            }
            return true;
        }
        #endregion

        #region SetRanges
        //********************************************
        ///Set Range of Selected Element#
        //********************************************
        private void RangeSetCommand(int elementIndex)
        {
            ///---------------------#Send Voltage Range#
            string msg;
            msg = cbVoltageRange.Text;
            if (msg != "AUTO")
            {
                msg = ":INPUT:VOLTAGE:RANGE:ELEMENT" + (elementIndex + 1).ToString() + " " + cbVoltageRange.Text;
            }
            else
            {
                msg = ":INPUT:VOLT:AUTO:ELEMENT" + (elementIndex + 1).ToString() + " " + "ON";
            }
            SetSendMonitor(msg);
            int rtn = connection.Send(msg);
            if (rtn != 0)
            {
                DispError(connection.GetLastError());
                ///when setting failed, resume the original value.
                GetRanges(elementIndex);
            }

            ///---------------------#Send Current Range#
            int rtn_tmp = -1;
            string Current_Sen;
            Current_Sen = cbCurrentRange.Text;
            rtn_tmp = Current_Sen.IndexOf("V");

            msg = cbCurrentRange.Text;
            if (msg != "AUTO")
            {
                if (rtn_tmp > 0)
                    msg = ":INPUT:CURRENT:RANGE:ELEMENT" + (elementIndex + 1).ToString() + " " + "EXTERNAL," + " " + cbCurrentRange.Text;
                else
                    msg = ":INPUT:CURRENT:RANGE:ELEMENT" + (elementIndex + 1).ToString() + " " + cbCurrentRange.Text;
            }
            else
            {
                msg = ":INPUT:CURRENT:AUTO:ELEMENT" + (elementIndex + 1).ToString() + " " + "ON";
            }
            SetSendMonitor(msg);
            rtn = connection.Send(msg);
            if (rtn != 0)
            {
                DispError(connection.GetLastError());
                ///when setting failed, resume the original value.
                GetRanges(elementIndex);
            }
            GetRanges(Currentelement);
        }
        #endregion

        #region Function: GetUpdateRate
        //********************************************
        /// <summary>
        /// Function: Get Update Rate
        /// </summary>
        //********************************************
        private void GetUpdateRate()
        {
            string msg;

            ///#set updateRate combo list#
            cbUpdateRate.Items.Clear();
            btSetUpdateRate.IsEnabled = true;
            cbUpdateRate.IsEnabled = true;
            foreach (string item in updateRateList)
            {
                if (item != "")
                { cbUpdateRate.Items.Add(item); }
            }

            ///#get updateRate value#
            string data = "";
            msg = ":RATE?";
            SetSendMonitor(msg);
            if (!QueriesData(40, msg, ref data))
            {
                return;
            }
            SetReceiveMonitor(data);
            data = CutLeft("\n", ref data);//cut left with LF.

            if (0 <= data.IndexOf("AUTO"))
            {
                msg = "RATE 500ms";
                SetSendMonitor(msg);
                int rtn = connection.Send(msg);
                System.Threading.Thread.Sleep(6000);
                msg = ":RATE?";
                SetSendMonitor(msg);
                if (!QueriesData(40, msg, ref data))
                {
                    return;
                }
                SetReceiveMonitor(data);
                data = CutLeft("\n", ref data);//cut left with LF.
            }

            float floatRate = Convert.ToSingle(data);
            if (floatRate < 1)
            {
                //when "mA/mV" unit, multiply 1k.
                cbUpdateRate.Text = (floatRate * 1000).ToString() + "ms";
            }
            else
            {
                cbUpdateRate.Text = floatRate.ToString() + "s";
            }
        }
        #endregion

        #region SetUpdateRate
        //********************************************
        ///Set UpdateRate
        //********************************************
        private void UpdateRateSetCommand_Click(object sender, System.EventArgs e)
        {
            string msg;
            //Send Command.
            msg = ":RATE " + cbUpdateRate.Text;
            SetSendMonitor(msg);
            int rtn = connection.Send(msg);
            if (rtn != 0)
            {
                DispError(connection.GetLastError());
                //when setting failed, resume the original value.
                GetUpdateRate();
            }
            GetUpdateRate();
        }
        #endregion

        #region GetErrorInformation
        //***************************
        ///Getting Error Information
        //***************************
        private void ErrInfoGetCommand_Click(object sender, System.EventArgs e)
        {
            //----------------------#queries error from instrument#
            string msg;
            string errInfo = "";
            msg = ":STATUS:ERROR?";
            SetSendMonitor(msg);
            if (!QueriesData(200, msg, ref errInfo))
            {
                return;
            }
            errInfo = errInfo.Substring(0, errInfo.IndexOf("\n"));
            SetReceiveMonitor(errInfo);
            DispError(errInfo);
        }
        #endregion

        #region Set Header
        //***************************
        ///Set Header On/Off
        //***************************
        private void HeaderCommand_Click(object sender, System.EventArgs e)
        {
            string header = "";
            string msg;
            //----------------------#queries header status#
            msg = ":COMMUNICATE:HEADER?";
            SetSendMonitor(msg);
            if (!QueriesData(30, msg, ref header))
            {
                return;
            }
            SetReceiveMonitor(header);
            header = CutLeft("\n", ref header);//cut left with LF.

            //----------------------#set header status#
            if (header == "1")
            {
                msg = ":COMMUNICATE:HEADER OFF";
            }
            else
            {
                msg = ":COMMUNICATE:HEADER ON";
            }
            SetSendMonitor(msg);
            int rtn = connection.Send(msg);
            if (rtn != 0)
            {
                DispError(connection.GetLastError());
                return;
            }
        }
        #endregion

        #region Function: ReadItemSettings
        //*************************************
        ///<summary>
        ///Function: ReadItemSettings
        ///</summary>
        //*************************************
        private void ReadItemSettings()
        {
            string msg;
            string buf = "";
            string fun_msg;
            int pos;
            pos = -1;

            ///----------#get ASCII/BINARY#
            msg = ":NUMERIC:FORMAT?";
            SetSendMonitor(msg);
            if (!QueriesData(30, msg, ref buf))
            {
                return;
            }
            SetReceiveMonitor(buf);
            buf = CutLeft("\n", ref buf);//cut left with LF.

            ///---------#set ASCII/BINARY option#
            if (buf.IndexOf("ASC") != -1)
            {
                encodeType = EncodeType.ASCII;
            }
            else
            {
                encodeType = EncodeType.BINARY;
            }

            ///----------------------#get item count#
            //msg = ":NUMERIC:NORMAL:NUMBER?";
            //SetSendMonitor(msg);
            //if (!QueriesData(50, msg, ref buf))
            //{
            //    return;
            //}
            //SetReceiveMonitor(buf);
            //buf = CutLeft("\n", ref buf);//cut left with LF.

            /////----------------------#set item count combo#
            //if (Convert.ToInt32(buf) >= MAX_ITEM)
            //{
            //    ItemNumberCombo.Text = MAX_ITEM.ToString();
            //}
            //else
            //{
            //    ItemNumberCombo.Text = " " + buf;
            //}

            ///----------------------#get item settings#
            msg = ":NUMERIC:NORMAL?";

            SetSendMonitor(msg);
            if (!QueriesData(25 + 40 * Convert.ToInt32(buf), msg, ref buf))
            {
                return;
            }
            SetReceiveMonitor(buf);
            buf = CutLeft("\n", ref buf);//cut left with LF.
                                         //cut off the item number(item count) portion.
            CutLeft(";", ref buf);
        }
        #endregion

        private class FCODefine
        {
            public string Function { set; get; } = string.Empty;
            public string Element { set; get; } = string.Empty;
            public string Order { set; get; } = string.Empty;
            public float Value { set; get; } = 0;
        }

        /**
         * IRMS 电流有效值
         * URMS 电压有效值
         * UMN  电压平均值
         * P    功率
         * fU   电压频率
         */

        private List<FCODefine> ItemSettings = new List<FCODefine>()
        {
            new FCODefine {Function = "IRMS", Element = "1", Order = ""},
            new FCODefine {Function = "IRMS", Element = "2", Order = ""},
            new FCODefine {Function = "IRMS", Element = "3", Order = ""},
            new FCODefine {Function = "URMS", Element = "1", Order = ""},
            new FCODefine {Function = "URMS", Element = "2", Order = ""},
            new FCODefine {Function = "URMS", Element = "3", Order = ""},
            new FCODefine {Function = "UMN",  Element = "1", Order = ""},
            new FCODefine {Function = "UMN",  Element = "2", Order = ""},
            new FCODefine {Function = "UMN",  Element = "3", Order = ""},
            new FCODefine {Function = "P", Element = "1", Order = ""},
            new FCODefine {Function = "P", Element = "2", Order = ""},
            new FCODefine {Function = "P", Element = "3", Order = ""},
            new FCODefine {Function = "fU", Element = "0", Order = ""},
        };

        #region Function: SendItemSettings
        //********************************************
        ///<summary>
        ///Function: SendItemSettings
        ///</summary>
        //********************************************
        private void SendItemSettings()
        {
            string msg;
            int rtn;

            ///----------------------#set ASCII/Float(Binary)#
            if (encodeType == EncodeType.ASCII)
            {
                msg = ":NUMERIC:FORMAT ASCII";
            }
            else
            {
                msg = ":NUMERIC:FORMAT FLOAT";
            }
            SetSendMonitor(msg);
            rtn = connection.Send(msg);
            if (rtn != 0)
            {
                DispError(connection.GetLastError());
                return;
            }

            ///----------------------#set items number#
            msg = ":NUMERIC:NORMAL:NUMBER " + ItemSettings.Count;
            SetSendMonitor(msg);
            rtn = connection.Send(msg);
            if (rtn != 0)
            {
                DispError(connection.GetLastError());
                return;
            }

            ///----------------------#send message detail#
            msg = ":NUMERIC:NORMAL:";
            for (int n = 0; n < ItemSettings.Count; n++)
            {
                //set function parameter into message.
                msg = msg + "ITEM" + (n + 1).ToString() + " " + ItemSettings[n].Function;
                //set element parameter into message.
                if (ItemSettings[n].Element.Length > 0)
                {
                    msg = msg + "," + ItemSettings[n].Element;
                }
                //set order parameter into message, if have.
                msg = msg + "," + ItemSettings[n].Order;
                //set separator into message.
                if (n != Convert.ToInt32(ItemSettings.Count) - 1)
                {
                    msg = msg + ";";
                }
            }
            SetSendMonitor(msg);
            rtn = connection.Send(msg);
            if (rtn != 0)
            {
                DispError(connection.GetLastError());
                return;
            }
        }
        #endregion

        #region GetItems
        //***************************
        ///Read Item Settings
        //***************************
        private void GetAllItemsCommand_Click(object sender, System.EventArgs e)
        {
            ReadItemSettings();
        }
        #endregion

        #region SetItems
        //********************************************
        ///Send Item Settings
        //********************************************
        private void SetAllItemsCommand_Click(object sender, System.EventArgs e)
        {
            SendItemSettings();
        }
        #endregion

        #region OnTimer
        //*********************************
        ///<summary>
        ///Timer Event
        ///</summary>
        //*********************************
        private void Timer1_Tick(object? sender, System.EventArgs e)
        {
            string eesr = "";
            string msg = ":STATUS:EESR?";
            SetSendMonitor(msg);
            if (!QueriesData(20, msg, ref eesr))
            {
                return;
            }
            SetReceiveMonitor(eesr);
            eesr = CutLeft("\n", ref eesr); //cut left with LF.

            if ((Convert.ToInt64(eesr) & 0X00000001) == 1)
            {
                GetItemData();
            }
        }
        private void Timer2_Tick(object sender, System.EventArgs e)
        {
            GetItemData();
            return;
        }
        #endregion

        #region GetDataSingle
        //********************************************
        ///GetData Single
        //********************************************
        private void GetDataSgCommand_Click(object sender, System.EventArgs e)
        {
            SendItemSettings();
            GetItemData();
        }
        #endregion

        #region GetDataByUpdateRate
        //****************************************
        ///GetData by UpdateRate
        //****************************************
        private void GetDataURateCommand_Click(object sender, System.EventArgs e)
        {
            //----------------------#resume all#
            if (btRequestContinue.Content == "STOP")
            {
                Timer1.Stop();
                btRequestContinue.Content = "Get Data   (Update Rate)";
            }
            //----------------------#getting datas#
            else
            {
                SendItemSettings();

                //reset filter1.
                string msg = ":STATUS:FILTER1 FALL";
                SetSendMonitor(msg);
                int rtn = connection.Send(msg);
                if (rtn != 0)
                {
                    DispError(connection.GetLastError());
                    return;
                }
                //************************************
                msg = ":STATUS:EESR?";
                String eesr = "";
                SetSendMonitor(msg);
                if (!QueriesData(20, msg, ref eesr))
                {
                    return;
                }
                SetReceiveMonitor(eesr);
                //************************************

                //reset other controls' display.
                btRequestContinue.IsEnabled = true;
                btRequestContinue.Content = "STOP";
                //set timer interval and start getting data.
                Timer1.Start();
                Timer1.Interval = TimeSpan.FromMilliseconds(10);
            }
        }
        #endregion

        #region ResetSystem
        //***************************
        ///System Reset
        //***************************
        private void AllResetCommand_Click(object sender, System.EventArgs e)
        {
            MessageBoxResult doReset =
                HandyControl.Controls.MessageBox.Show("System will be All Reset, continue?", "Confirm", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (doReset == MessageBoxResult.Cancel)
            {
                return;
            }
            else
            {
                string msg = "*RST";
                SetSendMonitor(msg);
                int rtn = connection.Send(msg);
                if (rtn != 0)
                {
                    DispError(connection.GetLastError());
                    return;
                }
                GetRanges(0);
                ReadItemSettings();
                GetUpdateRate();
            }
        }
        #endregion

        //#-------------------------#
        ///--Display Controls--
        //#-------------------------#

        #region GetOption
        //********************************************
        ///Normal IsChecked
        //********************************************
        private void GetOption()
        {
            int DT = -1;
            int G5 = -1;
            int G6 = -1;
            int MTR = -1;
            int AUX = -1;

            string opt = "";
            string msg = "*OPT?";
            SetSendMonitor(msg);
            if (!QueriesData(50, msg, ref opt))
            {
                return;
            }
            SetReceiveMonitor(opt);

            //get /G5G6 option 
            HarmonicOption = -1;
            G5 = opt.IndexOf("G5");
            G6 = opt.IndexOf("G6");
            if (G5 >= 0 || G6 >= 0)
            {
                HarmonicOption = 0;
            }

            //get /DT option 
            DT = opt.IndexOf("DT");
            if (wt1800e_flg == true)
            {
                DT = 1;
            }

            //get /MTR option 
            MTR = opt.IndexOf("MTR");

            //get /AUX option 
            AUX = opt.IndexOf("AUX");
        }
        #endregion

        #region Set Verbose
        //***************************
        ///Set Verbose On/Off
        //***************************
        private void VerboseCommand_Click(object sender, System.EventArgs e)
        {
            string verbose = "";
            string msg;
            //----------------------#queries header status#
            msg = ":COMMUNICATE:VERBOSE?";
            SetSendMonitor(msg);
            if (!QueriesData(30, msg, ref verbose))
            {
                return;
            }
            SetReceiveMonitor(verbose);
            verbose = CutLeft("\n", ref verbose);//cut left with LF.

            //----------------------#set header status#
            if (verbose == "1")
            {
                msg = ":COMMUNICATE:VERBOSE OFF";
            }
            else
            {
                msg = ":COMMUNICATE:VERBOSE ON";
            }
            SetSendMonitor(msg);
            int rtn = connection.Send(msg);
            if (rtn != 0)
            {
                DispError(connection.GetLastError());
                return;
            }
        }
        #endregion

        #region GetRatios
        //********************************************
        ///Set Ratio of All Elements#
        //********************************************
        private bool GetRatios()
        {
            ///---------------------#Query Voltage Transformer Ratio#
            string tem_auto = "";
            string msg = ":INPUT:SCALING:VT?";
            SetSendMonitor(msg);
            if (!QueriesData(50, msg, ref tem_auto))
            {
                DispError(connection.GetLastError());
                return false;
            }
            SetReceiveMonitor(tem_auto);
            tem_auto = CutLeft("\n", ref tem_auto);//cut left with LF.
            float vtRatio = Convert.ToSingle(tem_auto);
            cbVoltageRatio.Text = vtRatio.ToString();

            ///---------------------#Query Current Transformer Ratio#
            tem_auto = "";
            msg = ":INPUT:SCALING:CT?";
            SetSendMonitor(msg);
            if (!QueriesData(50, msg, ref tem_auto))
            {
                DispError(connection.GetLastError());
                return false;
            }
            SetReceiveMonitor(tem_auto);
            tem_auto = CutLeft("\n", ref tem_auto);//cut left with LF.
            float ctRatio = Convert.ToSingle(tem_auto);
            cbCurrentRatio.Text = ctRatio.ToString();

            return true;
        }
        #endregion

        #region SetRatios
        //********************************************
        ///Set Ratio of All Elements#
        //********************************************
        private void RatioSetCommand()
        {
            ///---------------------#Send Voltage Transformer#
            string msg = ":INPUT:SCALING:VT:ALL " + cbVoltageRange.Text;
            SetSendMonitor(msg);
            int rtn = connection.Send(msg);
            if (rtn != 0)
            {
                DispError(connection.GetLastError());
                ///when setting failed, resume the original value.
                GetRatios();
            }

            ///---------------------#Send Current Transformer#
            msg = ":INPUT:SCALING:CT:ALL " + cbCurrentRatio.Text;
            SetSendMonitor(msg);
            rtn = connection.Send(msg);
            if (rtn != 0)
            {
                DispError(connection.GetLastError());
                ///when setting failed, resume the original value.
                GetRatios();
            }
            GetRatios();
        }
        #endregion

        #region GetWiringSystem
        /**
         * [:INPut]:WIRing {(P1W2|P1W3|P3W3|
         * P3W4|V3A3)[,(P1W2|P1W3|P3W3|P3W4|
         * V3A3)][,(P1W2|P1W3|P3W3|P3W4|V3A3)]
         * [,(P1W2|P1W3|P3W3|P3W4|V3A3)]
         * [,(P1W2|P1W3|P3W3)][,P1W2]}
         * [:INPut]:WIRing?
         * P1W2 = Single-phase, two-wire system
         * [1P2W]
         * P1W3 = Single-phase, three-wire
         * system [1P3W]
         * P3W3 = Three-phase, three-wire
         * system [3P3W]
         * P3W4 = Three-phase, four-wire system
         * [3P3W]
         * V3A3 = Three-phase, three-wire
         * system with a three-voltage, threecurrent method
         * [3P3W(3V3A)]
         * 
         * • Example for a 3-element model
         * INPUT:WIRING P1W2,P3W3
         * INPUT:WIRING? ->
         * :INPUT:WIRING P1W2,P3W3
         * INPUT:WIRING P3W4
         * INPUT:WIRING? ->
         * :INPUT:WIRING P3W4
         */
        //********************************************
        ///Set Wiring System Config#
        //********************************************
        private bool GetWiringSystem()
        {
            cbWire.Items.Clear();
            cbWire.Items.Add("P1W2");
            cbWire.Items.Add("P1W3");
            cbWire.Items.Add("P3W3");
            cbWire.Items.Add("P3W4");
            cbWire.Items.Add("V3A3");
            ///---------------------#Query Wiring System#
            string tem_auto = "";
            string msg = ":INPUT:WIRING?";
            SetSendMonitor(msg);
            if (!QueriesData(50, msg, ref tem_auto))
            {
                DispError(connection.GetLastError());
                return false;
            }
            SetReceiveMonitor(tem_auto);
            tem_auto = CutLeft(",", ref tem_auto);//cut left with LF.
            cbWire.Text = tem_auto;
            return true;
        }
        #endregion

        #region SetWiringSystem
        //********************************************
        ///Set Wiring System#
        //********************************************
        private void WiringSetCommand()
        {
            if (cbWire.SelectedIndex < 0)
            {
                return;
            }
            ///---------------------#Send Wiring System#
            string msg = ":INPUT:WIRING " + cbWire.Text;
            SetSendMonitor(msg);
            int rtn = connection.Send(msg);
            if (rtn != 0)
            {
                DispError(connection.GetLastError());
            }

            GetWiringSystem();
        }
        #endregion
    }
}
