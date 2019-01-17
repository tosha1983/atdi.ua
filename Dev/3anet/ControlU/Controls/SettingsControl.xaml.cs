using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Drawing.Imaging;
using System.Windows.Media;
//using System.Windows.Forms;
using System.IO.Ports;
using NationalInstruments.VisaNS;
using System.Windows.Input;
using System.Windows.Data;

namespace ControlU.Controls
{
    /// <summary>
    /// Логика взаимодействия для SettingsControl.xaml
    /// </summary>
    public partial class SettingsControl : System.Windows.Controls.UserControl, INotifyPropertyChanged
    {
        string AppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        Equipment.GPSNMEA gps = MainWindow.gps;

        Settings.XMLSettings Sett;//= App.Sett;

        public Controls.ChangeIP cip = new ChangeIP();

        Helpers.Helper help = new Helpers.Helper();

        private MessageBasedSession mbSession;
        private IAsyncResult asyncHandle = null;

        InputDialogControl idc;

        private string _SensorNameTemp = "";
        public string SensorNameTemp
        {
            get { return _SensorNameTemp; }
            set { _SensorNameTemp = value; OnPropertyChanged("SensorNameTemp"); }
        }        
        #region gsm

        //private ObservableCollection<gsmunifreq> _GSMUniFreq = new ObservableCollection<gsmunifreq>() { };
        //public ObservableCollection<gsmunifreq> GSMUniFreq
        //{
        //    get { return _GSMUniFreq; }
        //    set { _GSMUniFreq = value; OnPropertyChanged("GSMUniFreq"); }
        //}
        //public class gsmunifreq
        //{
        //    public decimal Start { get; set; }
        //    public decimal Stop { get; set; }
        //}
        #endregion
        #region wcdma

        private decimal _UMTSFreqADD = 0;
        public decimal UMTSFreqADD
        {
            get { return _UMTSFreqADD; }
            set { _UMTSFreqADD = value; OnPropertyChanged("UMTSFreqADD"); }
        }
        private decimal _LTEFreqADD = 0;
        public decimal LTEFreqADD
        {
            get { return _LTEFreqADD; }
            set { _LTEFreqADD = value; OnPropertyChanged("LTEFreqADD"); }
        }
        private decimal _CDMAFreqADD = 0;
        public decimal CDMAFreqADD
        {
            get { return _CDMAFreqADD; }
            set { _CDMAFreqADD = value; OnPropertyChanged("CDMAFreqADD"); }
        }
        private Settings.MeasMonBand _MeasMonBandADD = new Settings.MeasMonBand() { };
        public Settings.MeasMonBand MeasMonBandADD
        {
            get { return _MeasMonBandADD; }
            set { _MeasMonBandADD = value; OnPropertyChanged("MeasMonBandADD"); }
        }
        #endregion
        #region wrls
        private string _WRLSBoardIP = "";
        public string WRLSBoardIP
        {
            get { return _WRLSBoardIP; }
            set { _WRLSBoardIP = value; OnPropertyChanged("WRLSBoardIP"); }
        }
        private string _WRLSBoardName = "";
        public string WRLSBoardName
        {
            get { return _WRLSBoardName; }
            set { _WRLSBoardName = value; OnPropertyChanged("WRLSBoardName"); }
        }
        private decimal _WRLSFreqStart = 0;
        public decimal WRLSFreqStart
        {
            get { return _WRLSFreqStart; }
            set { _WRLSFreqStart = value; OnPropertyChanged("WRLSFreqStart"); }
        }
        private decimal _WRLSFreqStop = 0;
        public decimal WRLSFreqStop
        {
            get { return _WRLSFreqStop; }
            set { _WRLSFreqStop = value; OnPropertyChanged("WRLSFreqStop"); }
        }
        private decimal _WRLSBW = 0;
        public decimal WRLSBW
        {
            get { return _WRLSBW; }
            set { _WRLSBW = value; OnPropertyChanged("WRLSBW"); }
        }
        private string _WRLSBoard = "";
        public string WRLSBoard
        {
            get { return _WRLSBoard; }
            set { _WRLSBoard = value; OnPropertyChanged("WRLSBoard"); }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении

        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public SettingsControl()
        {
            InitializeComponent();
            Sett = Helpers.ExtensionMethods.DeepCloneXML(App.Sett);
            SensorNameTemp = Sett.ATDIConnection_Settings.Selected.sensor_equipment_tech_id;
            this.DataContext = Sett;

            //ATDIConnection_EX.DataContext = MainWindow.db_v2;
            //cmbColors.ItemsSource = typeof(Colors).GetProperties();
            GSMBand_cb.DataContext = this;
            UMTSFreqADD_tb.DataContext = this;
            LTEFreqADD_tb.DataContext = this;
            CDMAFreqADD_tb.DataContext = this;
            MeasMonBands.DataContext = this;
            MeasMonBands_DG.DataContext = Sett;
            ATDI_SensorNameTemp.DataContext = this;            
        }




        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            App.Sett.SetFromAnotherSett(Sett);
            App.Sett.SaveAll();

            //App.Sett.ReloadAll();

            MainWindow mainWindow = ((SplashWindow)App.Current.MainWindow).m_mainWindow;
            mainWindow.CloseGlobalUC(mainWindow.SetCtrl);
            mainWindow.SetCtrl = null;
            //OnPropertyChanged("SettingsClose");
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = ((SplashWindow)App.Current.MainWindow).m_mainWindow;
            mainWindow.UpdateLayout(); mainWindow.InvalidateVisual();
            mainWindow.CloseGlobalUC(mainWindow.SetCtrl);
            mainWindow.SetCtrl = null;
            //OnPropertyChanged("SettingsClose");
        }
        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            App.Sett.SetFromAnotherSett(Sett); //App.Sett = Helpers.ExtensionMethods.DeepCloneXML(Sett);
            App.Sett.SaveAll();
            Sett = Helpers.ExtensionMethods.DeepCloneXML(App.Sett);

        }
        private void SaveSettings()
        {
            App.Sett.SaveAll();
        }

        #region ATDI
        #region Users
        private void RegisterSensor_Click(object sender, RoutedEventArgs e)
        {
            Settings.ATDIConnection con = MainWindow.db_v2.ATDIConnectionData_Selsected;
            con.sensor_equipment_tech_id = SensorNameTemp;
            if (MainWindow.db_v2.ServerIsLoaded && MainWindow.atdi.RabbitIsUsed == false)
                MainWindow.atdi.Initialize();
            if (MainWindow.db_v2.ServerIsLoaded && MainWindow.atdi.RabbitIsUsed == true)
            {
                if (MainWindow.atdi.RegisterSensor(SensorNameTemp))
                {
                    MainWindow.db_v2.UpdateATDIConnection(con);
                    Sett.ATDIConnection_Settings.Selected = con;
                }
            }
        }
        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            AddUser au = new AddUser();
            ((SplashWindow)App.Current.MainWindow).m_mainWindow.ShowGlobalUC(au, true, false);

        }
        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            //Controls.InputDialogControl idc = new Controls.InputDialogControl(this.FindResource("RemoveUser").ToString(), "");
            //MainWindow mainWindow = (MainWindow)App.Current.MainWindow;
            //mainWindow.ShowGlobalUC(idc);
            //if (idc.Value == true)
            //{
            if (Sett.UsersApps_Settings.Users.Count > 0)
                Sett.UsersApps_Settings.Users.Remove((Settings.UserApps_Set)Users_Gui.SelectedItem);
            //    mainWindow.CloseGlobalUC(idc);
            //}
            //else { mainWindow.CloseGlobalUC(idc); }
        }

        #endregion
        #endregion

        private void AddEquipment_Click(object sender, RoutedEventArgs e)
        {
            if (Equipment_cb.SelectedItem != null)
            {
                Settings.Equipment_Set Item = (Settings.Equipment_Set)((ComboBoxItem)Equipment_cb.SelectedItem).Tag;
                if (Item != null)
                {
                    if (Equipment_cb.SelectedIndex < 0)
                    { ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = this.FindResource("SettingsControl_SelectTypeEquipment").ToString(); }//"Выбирете тип оборудования!"; }
                    else
                    {
                        if (item != null)
                        {
                            //Settings.Equipment_Set b = (Settings.Equipment_Set)item.Tag;
                            Item.UseEquipment = true;
                        }
                    }
                }
            }
        }

        private void DeleteEquipment_Click(object sender, RoutedEventArgs e)
        {
            ((Settings.Equipment_Set)((Button)sender).Tag).UseEquipment = false;
        }

        #region Analyzer

        private void InstrGetResourcesInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                mbSession = (MessageBasedSession)ResourceManager.GetLocalManager().Open(String.Concat("TCPIP0::", Sett.Analyzer_Settings.IPAdress, "::inst0::INSTR"));
            }
            catch { }

            string responseString = "Установленные опции: ";
            try
            {
                string temp1;
                string[] temp2;
                temp1 = mbSession.Query("*IDN?\n");
                responseString += temp1;
                temp2 = responseString.Split(',', '/');
                mbSession.Dispose();
            }
            catch { responseString = "Проверьте подключение!"; }
        }
        private void InstrGetOptionInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                mbSession = (MessageBasedSession)ResourceManager.GetLocalManager().Open(String.Concat("TCPIP0::", Sett.Analyzer_Settings.IPAdress, "::inst0::INSTR"));
            }
            catch { }
            try
            {

                mbSession.Dispose();
            }
            catch { }


        }
        private void PresetFile_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog OF = new System.Windows.Forms.OpenFileDialog();
            OF.Filter = "Файл настроек Presets(*.Xml)|*.xml";
            if (OF.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                Sett.Analyzer_Settings.UserPresetFilePath = OF.FileName;
        }
        private void ScreenFolder_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog FD = new System.Windows.Forms.FolderBrowserDialog();
            FD.Description = "Выбор папки хранения скриншотов";

            if (Sett.Screen_Settings.ScreenFolder == "")
            {
                if (FD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Sett.Screen_Settings.ScreenFolder = FD.SelectedPath;
                }
            }
            else
            {
                FD.SelectedPath = Sett.Screen_Settings.ScreenFolder;
                if (FD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Sett.Screen_Settings.ScreenFolder = FD.SelectedPath;
                }
            }
        }
        private void PrintScreenResolution_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> PrintScreenResolution_data = new List<string>();
            PrintScreenResolution_data.Add("1600 x 1200");
            PrintScreenResolution_data.Add("1024 x 768");
            PrintScreenResolution_data.Add("800 x 600");

            var comboBox = sender as System.Windows.Controls.ComboBox;
            comboBox.ItemsSource = PrintScreenResolution_data;
            // дефаултное значение
            comboBox.SelectedIndex = PrintScreenResolution_data.IndexOf(Sett.Screen_Settings.ScreenResolution);
        }

        private void PrintScreenResolution_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as System.Windows.Controls.ComboBox;
            // узнаем выбранное значение
            Sett.Screen_Settings.ScreenResolution = comboBox.SelectedItem as string;
        }
        private void SaveScreenImageFormat_Loaded(object sender, RoutedEventArgs e)
        {
            List<ImageFormat> ScreenImageFormat_data = new List<ImageFormat>();
            ScreenImageFormat_data.Add(ImageFormat.Png);
            ScreenImageFormat_data.Add(ImageFormat.Jpeg);
            ScreenImageFormat_data.Add(ImageFormat.Bmp);
            ScreenImageFormat_data.Add(ImageFormat.Emf);

            var comboBox = sender as System.Windows.Controls.ComboBox;
            comboBox.ItemsSource = ScreenImageFormat_data;
            // дефаултное значение
            ImageFormat iff = help.FromString(Sett.Screen_Settings.SaveScreenImageFormat);
            comboBox.SelectedIndex = ScreenImageFormat_data.IndexOf(iff);
        }

        private void SaveScreenImageFormat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as System.Windows.Controls.ComboBox;
            // узнаем выбранное значение
            Sett.Screen_Settings.SaveScreenImageFormat = (comboBox.SelectedItem as ImageFormat).ToString();
        }
        private void ChangeIPSettings_Click(object sender, RoutedEventArgs e)
        {
            cip.GetIP();
            ((SplashWindow)App.Current.MainWindow).m_mainWindow.ShowGlobalUC(cip, true, false);
        }
        #endregion

        #region RsReceiver
        private void RsReceiverAux1_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> RsReceiverAux1_data = new List<string>();

            RsReceiverAux1_data.Add("None");
            RsReceiverAux1_data.Add("Antenna");
            RsReceiverAux1_data.Add("HA-240 GPS Mouse");
            RsReceiverAux1_data.Add("NMEA GPS Mouse");
            RsReceiverAux1_data.Add("NMEA Compass");
            RsReceiverAux1_data.Add("Triggerable Antenna");
            var comboBox = sender as System.Windows.Controls.ComboBox;
            comboBox.ItemsSource = RsReceiverAux1_data;
            // дефаултное значение
            //comboBox.SelectedValue = App.Sett.RsReceiver_Settings.Auxiliary1;
        }
        //private void RsReceiverAux1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    var comboBox = sender as System.Windows.Controls.ComboBox;
        //    // узнаем выбранное значение
        //    App.Sett.RsReceiver_Settings.Auxiliary1 = (string)comboBox.SelectedValue;
        //}
        private void RsReceiverAux2_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> RsReceiverAux2_data = new List<string>();

            RsReceiverAux2_data.Add("None");
            RsReceiverAux2_data.Add("HA-240 GPS Mouse");
            RsReceiverAux2_data.Add("NMEA GPS Mouse");
            RsReceiverAux2_data.Add("NMEA Compass");
            RsReceiverAux2_data.Add("HL300");
            var comboBox = sender as System.Windows.Controls.ComboBox;
            comboBox.ItemsSource = RsReceiverAux2_data;
            // дефаултное значение
            //comboBox.SelectedValue = App.Sett.RsReceiver_Settings.Auxiliary2;
        }
        //private void RsReceiverAux2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    var comboBox = sender as System.Windows.Controls.ComboBox;
        //    // узнаем выбранное значение
        //    App.Sett.RsReceiver_Settings.Auxiliary2 = (string)comboBox.SelectedValue;
        //}
        private void RsReceiverGPS_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> GPS_data = new List<string>();

            GPS_data.Add("None");
            GPS_data.Add("Aux1");
            GPS_data.Add("Aux2");
            GPS_data.Add("SCPI");
            var comboBox = sender as System.Windows.Controls.ComboBox;
            comboBox.ItemsSource = GPS_data;
            // дефаултное значение
            //comboBox.SelectedValue = App.Sett.RsReceiver_Settings.GPS;
        }
        //private void RsReceiverGPS_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    var comboBox = sender as System.Windows.Controls.ComboBox;
        //    // узнаем выбранное значение
        //    App.Sett.RsReceiver_Settings.GPS = (string)comboBox.SelectedValue;
        //}
        private void RsReceiverCOM_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> COM_data = new List<string>();

            COM_data.Add("None");
            COM_data.Add("Aux1");
            COM_data.Add("Aux2");
            COM_data.Add("SCPI");
            var comboBox = sender as System.Windows.Controls.ComboBox;
            comboBox.ItemsSource = COM_data;
            // дефаултное значение
            //comboBox.SelectedValue = App.Sett.RsReceiver_Settings.COM;
        }
        private void RsReceiverDFAntennaReference_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> DFAntennaReference_data = new List<string>();

            DFAntennaReference_data.Add("North");
            DFAntennaReference_data.Add("GPS");
            DFAntennaReference_data.Add("Compass");
            var comboBox = sender as System.Windows.Controls.ComboBox;
            comboBox.ItemsSource = DFAntennaReference_data;
            // дефаултное значение
            //comboBox.SelectedValue = App.Sett.RsReceiver_Settings.COM;
        }

        //private void RsReceiverCOM_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    var comboBox = sender as System.Windows.Controls.ComboBox;
        //    // узнаем выбранное значение
        //    App.Sett.RsReceiver_Settings.COM = (string)comboBox.SelectedValue;
        //}

        #endregion

        //private Equipment.GSMBands _Group;
        //public Equipment.GSMBands ContactGroup
        //{
        //    get { return _Group; }
        //    set { _Group = value; }
        //}
        #region TSMx        
        public IEnumerable<Equipment.GSMBands> GSMBandsList
        {
            get
            {
                return Enum.GetValues(typeof(Equipment.GSMBands)).Cast<Equipment.GSMBands>();
            }
        }
        public IEnumerable<RohdeSchwarz.ViCom.Net.GSM.Pdu.Type> GSMSITypeList
        {
            get
            {
                return Enum.GetValues(typeof(RohdeSchwarz.ViCom.Net.GSM.Pdu.Type)).Cast<RohdeSchwarz.ViCom.Net.GSM.Pdu.Type>();
            }
        }
        private void AddGSMBands_Click(object sender, RoutedEventArgs e)
        {
            //Equipment.GSM_Band t;
            //foreach (Equipment.GSM_Band tt in Equipment.GSM_BandFreqs)
            //Equipment.GSM_BandFreqs

            if (GSMBand_cb.SelectedIndex >= 0)
            {
                bool find = Sett.TSMxReceiver_Settings.GSM.Bands.Any(x => x.Band == (Equipment.GSMBands)GSMBand_cb.SelectedItem);
                if (find == false)
                {
                    Equipment.GSM_Band t = Equipment.TSMxReceiver.GSM_BandFreqs.Where(x => x.Band == (Equipment.GSMBands)GSMBand_cb.SelectedItem).First();
                    Sett.TSMxReceiver_Settings.GSM.Bands.Add(new Settings.GSMBand_Set()
                    {
                        Band = (Equipment.GSMBands)GSMBand_cb.SelectedItem,
                        Use = true,
                        FreqStart = t.FreqDnStart,
                        FreqStop = t.FreqDnStop,
                        ARFCNStart = t.ARFCNStart,
                        ARFCNStop = t.ARFCNStop
                    });
                }
                //GetUnifreqsBand();
                //var freq1 = freq.GroupBy(i => i, (e, g) => new { Value = e, Count = g.Count() });
                //freq = new ObservableCollection<decimal>(freq.OrderBy(x => x));

                //GSMUniFreq
            }
            //System.Windows.MessageBox.Show(GSMBand_cb.SelectedItem.ToString());
        }
        //private void GetUnifreqsBand()
        //{
        //    GSMUniFreq.Clear();
        //    List<decimal> freq = new List<decimal>() { };
        //    foreach (Settings.GSMBand_Set t in Sett.TSMxReceiver_Settings.GSM.Bands)
        //    {
        //        for (decimal i = t.FreqStart; i <= t.FreqStop; i += 0.2m)
        //        { freq.Add(i); }
        //    }
        //    freq.Sort();
        //    /*freq = */freq.Distinct();
        //    decimal st = freq[0], sp = 0;
        //    decimal tmp = freq[0];
        //    for (int i = 0; i < freq.Count; i++)
        //    {
        //        if (tmp + 0.3m < freq[i])
        //        {
        //            sp = tmp;
        //            GSMUniFreq.Add(new gsmunifreq() { Start = st, Stop = sp });
        //            st = freq[i];
        //            //sp = freq[i];
        //            tmp = freq[i];
        //        }
        //        else if (i == freq.Count - 1)
        //        {
        //            sp = freq[i];
        //            GSMUniFreq.Add(new gsmunifreq() { Start = st, Stop = sp });
        //            st = freq[i];
        //            //sp = freq[i];
        //            tmp = freq[i];
        //        }
        //        else { tmp = freq[i]; }
        //    }
        //    string str = "";
        //    foreach (gsmunifreq tp in GSMUniFreq)
        //    {
        //        str += "st " + tp.Start.ToString() + " sp " + tp.Stop.ToString() + "\r\n";
        //    }
        //    System.Windows.MessageBox.Show(str);
        //}
        private void DeleteGSMBands_Click(object sender, RoutedEventArgs e)
        {
            if (GSMFreqs_dg.SelectedIndex >= 0)
            {
                Sett.TSMxReceiver_Settings.GSM.Bands.RemoveAt(GSMFreqs_dg.SelectedIndex);// (Settings.TSMxGSMBand_Set)GSMFreqs.Items[GSMFreqs.SelectedIndex]);
            }
            //GetUnifreqsBand();
        }

        #region UMTS
        private void AddUMTSFreq_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Equipment.UMTS_Channel tt = MainWindow.help.GetUMTSCHFromFreqDN(UMTSFreqADD);
                Settings.UMTSFreqs_Set t = new Settings.UMTSFreqs_Set()
                {
                    FreqDn = tt.FreqDn,
                    FreqUp = tt.FreqUp,
                    UARFCN_DN = tt.UARFCN_DN,
                    UARFCN_UP = tt.UARFCN_UP,
                    StandartSubband = tt.StandartSubband,
                    Use = true
                };
                if (t.FreqDn != 0) Sett.TSMxReceiver_Settings.UMTS.Freqs.Add(t);
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "SettingsControl", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
        }
        private void UMTSFreqADD_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                UMTSFreqADD = help.KeyDownDecimal(UMTSFreqADD / 1000000, (System.Windows.Controls.TextBox)sender, e) * 1000000;
            }
        }

        private void DeleteUMTSFreq_Click(object sender, RoutedEventArgs e)
        {
            if (UMTSFreqs_dg.SelectedIndex >= 0)
            {
                Sett.TSMxReceiver_Settings.UMTS.Freqs.RemoveAt(UMTSFreqs_dg.SelectedIndex);// (Settings.TSMxGSMBand_Set)GSMFreqs.Items[GSMFreqs.SelectedIndex]);
            }
        }
        #endregion UMTS
        #region LTE
        private void AddLTEFreq_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Equipment.LTE_Channel tt = MainWindow.help.GetLTECHfromFreqDN(LTEFreqADD);
                Settings.LTEFreqs_Set t = new Settings.LTEFreqs_Set()
                {
                    FreqDn = tt.FreqDn,
                    FreqUp = tt.FreqUp,
                    EARFCN_DN = tt.EARFCN_DN,
                    EARFCN_UP = tt.EARFCN_UP,
                    StandartSubband = tt.StandartSubband,
                    Use = true
                };
                if (t.FreqDn != 0) Sett.TSMxReceiver_Settings.LTE.Freqs.Add(t);
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "SettingsControl", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
        }
        private void LTEFreqADD_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LTEFreqADD = help.KeyDownDecimal(LTEFreqADD / 1000000, (System.Windows.Controls.TextBox)sender, e) * 1000000;
            }
        }

        private void DeleteLTEFreq_Click(object sender, RoutedEventArgs e)
        {
            if (LTEFreqs_dg.SelectedIndex >= 0)
            {
                Sett.TSMxReceiver_Settings.LTE.Freqs.RemoveAt(LTEFreqs_dg.SelectedIndex);// (Settings.TSMxGSMBand_Set)GSMFreqs.Items[GSMFreqs.SelectedIndex]);
            }
        }
        #endregion LTE
        #region CDMA
        private void AddCDMAFreq_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //System.Windows.MessageBox.Show(CDMAFreqADD.ToString());
                Equipment.CDMA_Channel tt = MainWindow.help.GetCDMACHfromFreqDN(CDMAFreqADD);
                Settings.CDMAFreqs_Set t = new Settings.CDMAFreqs_Set()
                {
                    FreqDn = tt.FreqDn,
                    FreqUp = tt.FreqUp,
                    Channel = tt.ChannelN,
                    StandartSubband = tt.StandartSubband,
                    Use = true
                };
                if (t.FreqDn != 0) Sett.TSMxReceiver_Settings.CDMA.Freqs.Add(t);
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "SettingsControl", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
        }
        private void CDMAFreqADD_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CDMAFreqADD = help.KeyDownDecimal(CDMAFreqADD / 1000000, (System.Windows.Controls.TextBox)sender, e) * 1000000;
            }
        }
        private void DeleteCDMAFreq_Click(object sender, RoutedEventArgs e)
        {
            if (CDMAFreqs_dg.SelectedIndex >= 0)
            {
                Sett.TSMxReceiver_Settings.CDMA.Freqs.RemoveAt(CDMAFreqs_dg.SelectedIndex);// (Settings.TSMxGSMBand_Set)GSMFreqs.Items[GSMFreqs.SelectedIndex]);
            }
        }
        #endregion CDMA
        #endregion

        private void AddAntenna_Click(object sender, RoutedEventArgs e)
        {
            idc = new InputDialogControl(this.FindResource("Antenna_EnterAntennaName").ToString(), "Antenna");
            ((SplashWindow)App.Current.MainWindow).m_mainWindow.ShowGlobalUC(idc, true, false);
            idc.PropertyChanged += idc_PropertyChanged;
        }
        private void idc_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                if (idc.Value == true)
                {
                    Sett.Antennas_Settings.Antennas.Add(
                    new Settings.Antena_Set()
                    {
                        AntenaName = idc.ResponseText,
                        AntennaData = new ObservableCollection<Settings.AntennaLevel_Set>
                        { new Settings.AntennaLevel_Set() { NumPoint = 0, Freq = 1 } }
                    });
                }
                ((SplashWindow)App.Current.MainWindow).m_mainWindow.CloseGlobalUC(idc);
                idc = null;
            }
        }
        private void DeleteAntenna_Click(object sender, RoutedEventArgs e)
        {
            Sett.Antennas_Settings.Antennas.Remove((Settings.Antena_Set)Antenna_cb.SelectedItem);
        }
        private void AntennaData_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            e.NewItem = new Settings.AntennaLevel_Set()
            { NumPoint = Sett.SelectedAntena.AntennaData.Count, Freq = 1 };
        }
        private void InpInt_PreKeyDownInt(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            help.CheckIsInt((System.Windows.Controls.TextBox)sender, e);
        }
        private void InpDec_PreKeyDownDecimal(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            help.CheckIsDecimal((System.Windows.Controls.TextBox)sender, e);
        }


        #region Measurment
        private void MeasMon_SpecDeviece_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Sett.MeasMons_Settings.SpectrumMeasDeviece == 0)
            {

            }
            else if (Sett.MeasMons_Settings.SpectrumMeasDeviece == 1) //AN
            {
                #region GSM
                Sett.MeasMons_Settings.GSM.Data[0].MeasBW = 500000;
                Sett.MeasMons_Settings.GSM.Data[0].RBW = 1000;
                Sett.MeasMons_Settings.GSM.Data[0].VBW = 1000;
                Sett.MeasMons_Settings.GSM.Data[0].TracePoints = 1001;
                #endregion
                #region UMTS
                Sett.MeasMons_Settings.UMTS.Data[0].MeasBW = 5000000;
                Sett.MeasMons_Settings.UMTS.Data[0].RBW = 1000;
                Sett.MeasMons_Settings.UMTS.Data[0].VBW = 1000;
                Sett.MeasMons_Settings.UMTS.Data[0].TracePoints = 1001;
                #endregion
                #region CDMA
                Sett.MeasMons_Settings.CDMA.Data[0].MeasBW = 2000000;
                Sett.MeasMons_Settings.CDMA.Data[0].RBW = 1000;
                Sett.MeasMons_Settings.CDMA.Data[0].VBW = 1000;
                Sett.MeasMons_Settings.CDMA.Data[0].TracePoints = 1001;
                #endregion
                #region LTE
                Sett.MeasMons_Settings.LTE.Data[0].MeasBW = 1500000;
                Sett.MeasMons_Settings.LTE.Data[0].RBW = 1000;
                Sett.MeasMons_Settings.LTE.Data[0].VBW = 1000;
                Sett.MeasMons_Settings.LTE.Data[0].TracePoints = 1001;

                Sett.MeasMons_Settings.LTE.Data[1].MeasBW = 3000000;
                Sett.MeasMons_Settings.LTE.Data[1].RBW = 1000;
                Sett.MeasMons_Settings.LTE.Data[1].VBW = 1000;
                Sett.MeasMons_Settings.LTE.Data[1].TracePoints = 1001;

                Sett.MeasMons_Settings.LTE.Data[2].MeasBW = 5000000;
                Sett.MeasMons_Settings.LTE.Data[2].RBW = 1000;
                Sett.MeasMons_Settings.LTE.Data[2].VBW = 1000;
                Sett.MeasMons_Settings.LTE.Data[2].TracePoints = 1001;

                Sett.MeasMons_Settings.LTE.Data[3].MeasBW = 10000000;
                Sett.MeasMons_Settings.LTE.Data[3].RBW = 1000;
                Sett.MeasMons_Settings.LTE.Data[3].VBW = 1000;
                Sett.MeasMons_Settings.LTE.Data[3].TracePoints = 1001;

                Sett.MeasMons_Settings.LTE.Data[4].MeasBW = 15000000;
                Sett.MeasMons_Settings.LTE.Data[4].RBW = 1000;
                Sett.MeasMons_Settings.LTE.Data[4].VBW = 1000;
                Sett.MeasMons_Settings.LTE.Data[4].TracePoints = 1001;

                Sett.MeasMons_Settings.LTE.Data[5].MeasBW = 20000000;
                Sett.MeasMons_Settings.LTE.Data[5].RBW = 1000;
                Sett.MeasMons_Settings.LTE.Data[5].VBW = 1000;
                Sett.MeasMons_Settings.LTE.Data[5].TracePoints = 1001;
                #endregion
            }
            else if (Sett.MeasMons_Settings.SpectrumMeasDeviece == 2) //Em100/PR100/DDFxxx/EB5xx/ESMD
            {
                #region GSM
                Sett.MeasMons_Settings.GSM.Data[0].MeasBW = 500000;
                Sett.MeasMons_Settings.GSM.Data[0].RBW = 312.5m;
                Sett.MeasMons_Settings.GSM.Data[0].VBW = 312.5m;
                Sett.MeasMons_Settings.GSM.Data[0].TracePoints = 1601;
                #endregion
                #region UMTS
                Sett.MeasMons_Settings.UMTS.Data[0].MeasBW = 5000000;
                Sett.MeasMons_Settings.UMTS.Data[0].RBW = 3125;
                Sett.MeasMons_Settings.UMTS.Data[0].VBW = 3125;
                Sett.MeasMons_Settings.UMTS.Data[0].TracePoints = 1601;
                #endregion
                #region CDMA
                Sett.MeasMons_Settings.CDMA.Data[0].MeasBW = 2000000;
                Sett.MeasMons_Settings.CDMA.Data[0].RBW = 1250;
                Sett.MeasMons_Settings.CDMA.Data[0].VBW = 1250;
                Sett.MeasMons_Settings.CDMA.Data[0].TracePoints = 1601;
                #endregion
                #region LTE
                Sett.MeasMons_Settings.LTE.Data[0].MeasBW = 2000000;
                Sett.MeasMons_Settings.LTE.Data[0].RBW = 1250;
                Sett.MeasMons_Settings.LTE.Data[0].VBW = 1250;
                Sett.MeasMons_Settings.LTE.Data[0].TracePoints = 1601;

                Sett.MeasMons_Settings.LTE.Data[1].MeasBW = 5000000;
                Sett.MeasMons_Settings.LTE.Data[1].RBW = 3125;
                Sett.MeasMons_Settings.LTE.Data[1].VBW = 3125;
                Sett.MeasMons_Settings.LTE.Data[1].TracePoints = 1601;

                Sett.MeasMons_Settings.LTE.Data[2].MeasBW = 5000000;
                Sett.MeasMons_Settings.LTE.Data[2].RBW = 3125;
                Sett.MeasMons_Settings.LTE.Data[2].VBW = 3125;
                Sett.MeasMons_Settings.LTE.Data[2].TracePoints = 1601;

                Sett.MeasMons_Settings.LTE.Data[3].MeasBW = 10000000;
                Sett.MeasMons_Settings.LTE.Data[3].RBW = 6250;
                Sett.MeasMons_Settings.LTE.Data[3].VBW = 6250;
                Sett.MeasMons_Settings.LTE.Data[3].TracePoints = 1601;

                Sett.MeasMons_Settings.LTE.Data[4].MeasBW = 20000000;
                Sett.MeasMons_Settings.LTE.Data[4].RBW = 12500;
                Sett.MeasMons_Settings.LTE.Data[4].VBW = 12500;
                Sett.MeasMons_Settings.LTE.Data[4].TracePoints = 1601;

                Sett.MeasMons_Settings.LTE.Data[5].MeasBW = 20000000;
                Sett.MeasMons_Settings.LTE.Data[5].RBW = 12500;
                Sett.MeasMons_Settings.LTE.Data[5].VBW = 12500;
                Sett.MeasMons_Settings.LTE.Data[5].TracePoints = 1601;
                #endregion
            }
            else if (Sett.MeasMons_Settings.SpectrumMeasDeviece == 3) //TSMx
            {
                #region GSM
                Sett.MeasMons_Settings.GSM.Data[0].MeasBW = 500000;
                Sett.MeasMons_Settings.GSM.Data[0].RBW = 312.5m;
                Sett.MeasMons_Settings.GSM.Data[0].VBW = 312.5m;
                Sett.MeasMons_Settings.GSM.Data[0].TracePoints = 1601;
                #endregion
                #region UMTS
                Sett.MeasMons_Settings.UMTS.Data[0].MeasBW = 5000000;
                Sett.MeasMons_Settings.UMTS.Data[0].RBW = 3125;
                Sett.MeasMons_Settings.UMTS.Data[0].VBW = 3125;
                Sett.MeasMons_Settings.UMTS.Data[0].TracePoints = 1601;
                #endregion
                #region CDMA
                Sett.MeasMons_Settings.CDMA.Data[0].MeasBW = 2000000;
                Sett.MeasMons_Settings.CDMA.Data[0].RBW = 1250;
                Sett.MeasMons_Settings.CDMA.Data[0].VBW = 1250;
                Sett.MeasMons_Settings.CDMA.Data[0].TracePoints = 1601;
                #endregion
                #region LTE
                Sett.MeasMons_Settings.LTE.Data[0].MeasBW = 2000000;
                Sett.MeasMons_Settings.LTE.Data[0].RBW = 1250;
                Sett.MeasMons_Settings.LTE.Data[0].VBW = 1250;
                Sett.MeasMons_Settings.LTE.Data[0].TracePoints = 1601;

                Sett.MeasMons_Settings.LTE.Data[1].MeasBW = 3000000;
                Sett.MeasMons_Settings.LTE.Data[1].RBW = 1875;
                Sett.MeasMons_Settings.LTE.Data[1].VBW = 1875;
                Sett.MeasMons_Settings.LTE.Data[1].TracePoints = 1601;

                Sett.MeasMons_Settings.LTE.Data[2].MeasBW = 5000000;
                Sett.MeasMons_Settings.LTE.Data[2].RBW = 3125;
                Sett.MeasMons_Settings.LTE.Data[2].VBW = 3125;
                Sett.MeasMons_Settings.LTE.Data[2].TracePoints = 1601;

                Sett.MeasMons_Settings.LTE.Data[3].MeasBW = 10000000;
                Sett.MeasMons_Settings.LTE.Data[3].RBW = 6250;
                Sett.MeasMons_Settings.LTE.Data[3].VBW = 6250;
                Sett.MeasMons_Settings.LTE.Data[3].TracePoints = 1601;

                Sett.MeasMons_Settings.LTE.Data[4].MeasBW = 15000000;
                Sett.MeasMons_Settings.LTE.Data[4].RBW = 9375;
                Sett.MeasMons_Settings.LTE.Data[4].VBW = 9375;
                Sett.MeasMons_Settings.LTE.Data[4].TracePoints = 1601;

                Sett.MeasMons_Settings.LTE.Data[5].MeasBW = 20000000;
                Sett.MeasMons_Settings.LTE.Data[5].RBW = 12500;
                Sett.MeasMons_Settings.LTE.Data[5].VBW = 12500;
                Sett.MeasMons_Settings.LTE.Data[5].TracePoints = 1601;
                #endregion
            }
            else if (Sett.MeasMons_Settings.SpectrumMeasDeviece == 5) //SignalHound
            {
                #region GSM
                Sett.MeasMons_Settings.GSM.Data[0].MeasBW = 500000;
                Sett.MeasMons_Settings.GSM.Data[0].RBW = 1000;//1223,990208078335
                Sett.MeasMons_Settings.GSM.Data[0].VBW = 1000;//500000/1637=305,4367745876604 * 4 =1221,747098350641
                Sett.MeasMons_Settings.GSM.Data[0].TracePoints = 1638;
                //312,5
                #endregion
                #region UMTS
                Sett.MeasMons_Settings.UMTS.Data[0].MeasBW = 5000000;
                Sett.MeasMons_Settings.UMTS.Data[0].RBW = 1000;
                Sett.MeasMons_Settings.UMTS.Data[0].VBW = 1000;
                Sett.MeasMons_Settings.UMTS.Data[0].TracePoints = 1638;
                #endregion
                #region CDMA
                Sett.MeasMons_Settings.CDMA.Data[0].MeasBW = 2000000;
                Sett.MeasMons_Settings.CDMA.Data[0].RBW = 1000;
                Sett.MeasMons_Settings.CDMA.Data[0].VBW = 1000;
                Sett.MeasMons_Settings.CDMA.Data[0].TracePoints = 1638;
                #endregion
                #region LTE
                Sett.MeasMons_Settings.LTE.Data[0].MeasBW = 2000000;
                Sett.MeasMons_Settings.LTE.Data[0].RBW = 1000;
                Sett.MeasMons_Settings.LTE.Data[0].VBW = 1000;
                Sett.MeasMons_Settings.LTE.Data[0].TracePoints = 1638;

                Sett.MeasMons_Settings.LTE.Data[1].MeasBW = 3000000;
                Sett.MeasMons_Settings.LTE.Data[1].RBW = 1000;
                Sett.MeasMons_Settings.LTE.Data[1].VBW = 1000;
                Sett.MeasMons_Settings.LTE.Data[1].TracePoints = 1638;

                Sett.MeasMons_Settings.LTE.Data[2].MeasBW = 5000000;
                Sett.MeasMons_Settings.LTE.Data[2].RBW = 1000;
                Sett.MeasMons_Settings.LTE.Data[2].VBW = 1000;
                Sett.MeasMons_Settings.LTE.Data[2].TracePoints = 1638;

                Sett.MeasMons_Settings.LTE.Data[3].MeasBW = 10000000;
                Sett.MeasMons_Settings.LTE.Data[3].RBW = 1000;
                Sett.MeasMons_Settings.LTE.Data[3].VBW = 1000;
                Sett.MeasMons_Settings.LTE.Data[3].TracePoints = 1638;

                Sett.MeasMons_Settings.LTE.Data[4].MeasBW = 15000000;
                Sett.MeasMons_Settings.LTE.Data[4].RBW = 1000;
                Sett.MeasMons_Settings.LTE.Data[4].VBW = 1000;
                Sett.MeasMons_Settings.LTE.Data[4].TracePoints = 1638;

                Sett.MeasMons_Settings.LTE.Data[5].MeasBW = 20000000;
                Sett.MeasMons_Settings.LTE.Data[5].RBW = 1000;
                Sett.MeasMons_Settings.LTE.Data[5].VBW = 1000;
                Sett.MeasMons_Settings.LTE.Data[5].TracePoints = 1638;
                #endregion
            }
        }
        private void MeasMonBandADD_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string tag = (string)((TextBox)sender).Tag;
                if (tag == "Start")
                    MeasMonBandADD.Start = help.KeyDownDecimal(MeasMonBandADD.Start / 1000000, (System.Windows.Controls.TextBox)sender, e) * 1000000;
                if (tag == "Stop")
                    MeasMonBandADD.Stop = help.KeyDownDecimal(MeasMonBandADD.Stop / 1000000, (System.Windows.Controls.TextBox)sender, e) * 1000000;
                if (tag == "Step")
                    MeasMonBandADD.Step = help.KeyDownDecimal(MeasMonBandADD.Step / 1000000, (System.Windows.Controls.TextBox)sender, e) * 1000000;
            }
        }
        private void AddMeasMonBand_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MeasMonBandADD.Start != 0 && MeasMonBandADD.Stop != 0 && MeasMonBandADD.Step != 0) Sett.MeasMons_Settings.MeasMonBands.Add(MeasMonBandADD);
                MeasMonBandADD = new Settings.MeasMonBand();
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "SettingsControl", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
        }

        private void DeleteMeasMonBand_Click(object sender, RoutedEventArgs e)
        {
            if (MeasMonBands_DG.SelectedIndex >= 0)
            {
                Sett.MeasMons_Settings.MeasMonBands.RemoveAt(MeasMonBands_DG.SelectedIndex);// (Settings.TSMxGSMBand_Set)GSMFreqs.Items[GSMFreqs.SelectedIndex]);
            }
        }
        private void MeasMon_NdBLevel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (sender is TextBox)
                {
                    TextBox tb = (TextBox)sender;
                    double temp = 1;
                    temp = help.KeyDownDouble(temp, (TextBox)sender, e);

                    BindingExpression be = tb.GetBindingExpression(TextBox.TextProperty);
                    string bindingPath = be.ParentBinding.Path.Path;
                    object dataObject = tb.DataContext;

                    Type t = dataObject.GetType();
                    var propInfo = t.GetProperty(bindingPath);
                    propInfo.SetValue(dataObject, temp);
                }
            }
        }
        private void MeasMon_DeltaFreqLimit_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (sender is TextBox)
                {
                    TextBox tb = (TextBox)sender;
                    decimal temp = 1;
                    temp = help.KeyDownDecimal(temp, (TextBox)sender, e);

                    BindingExpression be = tb.GetBindingExpression(TextBox.TextProperty);
                    string bindingPath = be.ParentBinding.Path.Path;
                    object dataObject = tb.DataContext;

                    Type t = dataObject.GetType();
                    var propInfo = t.GetProperty(bindingPath);
                    propInfo.SetValue(dataObject, temp);
                }
            }
        }
        private void MeasMon_BWLimit_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (sender is TextBox)
                {
                    TextBox tb = (TextBox)sender;
                    decimal temp = 1000000;
                    temp = help.KeyDownDecimal(temp, (TextBox)sender, e) * 1000000; ;

                    BindingExpression be = tb.GetBindingExpression(TextBox.TextProperty);
                    string bindingPath = be.ParentBinding.Path.Path;
                    object dataObject = tb.DataContext;

                    Type t = dataObject.GetType();
                    var propInfo = t.GetProperty(bindingPath);
                    propInfo.SetValue(dataObject, temp);
                }
            }
        }
        #endregion


        #region Template


        private void SygmaBW_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                Sett.Template_Settings.DVB_T.SygmaBW = help.KeyDownDecimal(Sett.Template_Settings.DVB_T.SygmaBW / 1000000, (System.Windows.Controls.TextBox)sender, e) * 1000000;
            }
        }
        private void SygmaBW_PreKeyDownDecimal(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            help.CheckIsDecimal((System.Windows.Controls.TextBox)sender, e);
        }
        private void PowerBW_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                Sett.Template_Settings.DVB_T.PowerBW = help.KeyDownDecimal(Sett.Template_Settings.DVB_T.PowerBW / 1000000, (System.Windows.Controls.TextBox)sender, e) * 1000000;
            }
        }
        private void PowerBW_PreKeyDownDecimal(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            help.CheckIsDecimal((System.Windows.Controls.TextBox)sender, e);
        }
        private void DVBT_GetPermitionFile_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog OF = new System.Windows.Forms.OpenFileDialog();
            OF.Filter = "Файл экспорта из ICSM(*.Xml)|*.xml";
            if (OF.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Sett.Template_Settings.DVB_T.PermitionFileICSM = OF.FileName;
            }
        }
        private void DVBT_GetDotsFile_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog OF = new System.Windows.Forms.OpenFileDialog();
            OF.Filter = "Файл точек измерения(*.Xml)|*.xml";
            if (OF.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Sett.Template_Settings.DVB_T.DotsFile = OF.FileName;
            }
        }
        private void DVBT_Template_Folder_Click(object sender, RoutedEventArgs e)
        {
            string m = string.Empty;
            System.Windows.Forms.OpenFileDialog OF = new System.Windows.Forms.OpenFileDialog();

            OF.Filter = "Файл настроек Presets(*.docx)|*.docx";
            if (OF.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                m = OF.FileName;
            try { System.IO.File.Move(m, AppPath + @"\ReportTemplates\DVB_Template.docx"); } catch { }
        }
        #endregion

        #region GPS

        /// <summary>
        /// Номер порта
        /// </summary>
        private void GPSSerialPort_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> SerialPortGPS_data = new List<string>();
            foreach (string s in SerialPort.GetPortNames())
            {
                SerialPortGPS_data.Add(s);
            }
            var comboBox = sender as System.Windows.Controls.ComboBox;
            comboBox.ItemsSource = SerialPortGPS_data;
            // дефаултное значение
            comboBox.SelectedIndex = SerialPortGPS_data.IndexOf(Sett.GNSS_Settings.PortName);
        }
        private void GPSSerialPort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as System.Windows.Controls.ComboBox;
            // узнаем выбранное значение
            Sett.GNSS_Settings.PortName = comboBox.SelectedItem as string;
            //port.Text = value;
        }
        /// <summary>
        /// Скорость
        /// </summary>
        private void GPSPortBaudRate_Loaded(object sender, RoutedEventArgs e)
        {
            List<int> PortBaudRate_data = new List<int>();
            PortBaudRate_data.Add(2400);
            PortBaudRate_data.Add(4800);
            PortBaudRate_data.Add(9600);
            PortBaudRate_data.Add(14400);
            PortBaudRate_data.Add(19200);
            PortBaudRate_data.Add(38400);
            PortBaudRate_data.Add(57600);
            PortBaudRate_data.Add(115200);

            var comboBox = sender as System.Windows.Controls.ComboBox;
            comboBox.ItemsSource = PortBaudRate_data;
            // дефаултное значение
            //comboBox.SelectedIndex = PortBaudRate_data.IndexOf(FMeas.Settings.GPS.Default.GPSPortBaudRate);
            comboBox.SelectedIndex = PortBaudRate_data.IndexOf(Sett.GNSS_Settings.PortBaudRate);
        }
        private void GPSPortBaudRate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as System.Windows.Controls.ComboBox;
            // узнаем выбранное значение
            Sett.GNSS_Settings.PortBaudRate = int.Parse(comboBox.SelectedItem.ToString());//FMeas.Settings.GPS.Default.GPSPortBaudRate = comboBox.SelectedItem as string;
                                                                                          //port.Text = value;
        }
        /// <summary>
        /// Биты данных
        /// </summary>
        private void GPSPortDataBits_Loaded(object sender, RoutedEventArgs e)
        {
            List<int> PortDataBits_data = new List<int>();
            PortDataBits_data.Add(4);
            PortDataBits_data.Add(5);
            PortDataBits_data.Add(6);
            PortDataBits_data.Add(7);
            PortDataBits_data.Add(8);
            var comboBox = sender as System.Windows.Controls.ComboBox;
            comboBox.ItemsSource = PortDataBits_data;
            // дефаултное значение
            comboBox.SelectedIndex = PortDataBits_data.IndexOf(Sett.GNSS_Settings.PortDataBits);//comboBox.SelectedIndex = PortDataBits_data.IndexOf(FMeas.Settings.GPS.Default.GPSPortDataBits);
        }
        private void GPSPortDataBits_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as System.Windows.Controls.ComboBox;
            // узнаем выбранное значение
            Sett.GNSS_Settings.PortDataBits = int.Parse(comboBox.SelectedItem.ToString());
            //port.Text = value;
        }
        /// <summary>
        /// Четность
        /// </summary>
        private void GPSPortParity_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> PortParity_data = new List<string>();
            foreach (string s in Enum.GetNames(typeof(System.IO.Ports.Parity)))
            {
                PortParity_data.Add(s);
            }
            var comboBox = sender as System.Windows.Controls.ComboBox;
            comboBox.ItemsSource = PortParity_data;
            // дефаултное значение
            comboBox.SelectedIndex = PortParity_data.IndexOf(Sett.GNSS_Settings.PortParity.ToString());
        }
        private void GPSPortParity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as System.Windows.Controls.ComboBox;
            Sett.GNSS_Settings.PortParity = (System.IO.Ports.Parity)Enum.Parse(typeof(System.IO.Ports.Parity), comboBox.SelectedItem.ToString());
        }
        /// <summary>
        /// Стоповые биты
        /// </summary>
        private void GPSPortStopBits_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> PortStopBits_data = new List<string>();
            foreach (string s in Enum.GetNames(typeof(System.IO.Ports.StopBits)))
            {
                PortStopBits_data.Add(s);
            }

            var comboBox = sender as System.Windows.Controls.ComboBox;
            comboBox.ItemsSource = PortStopBits_data;
            // дефаултное значение
            comboBox.SelectedIndex = PortStopBits_data.IndexOf(Sett.GNSS_Settings.PortStopBits.ToString());
        }
        private void GPSPortStopBits_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as System.Windows.Controls.ComboBox;
            // узнаем выбранное значение
            Sett.GNSS_Settings.PortStopBits = (System.IO.Ports.StopBits)Enum.Parse(typeof(System.IO.Ports.StopBits), comboBox.SelectedItem.ToString());
            //port.Text = value;
        }
        /// <summary>
        /// Управление потоком
        /// </summary>
        private void GPSPortHandshake_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> PortHandshake_data = new List<string>();
            foreach (string s in Enum.GetNames(typeof(System.IO.Ports.Handshake)))
            {
                PortHandshake_data.Add(s);
            }

            var comboBox = sender as System.Windows.Controls.ComboBox;
            comboBox.ItemsSource = PortHandshake_data;
            // дефаултное значение
            comboBox.SelectedIndex = PortHandshake_data.IndexOf(Sett.GNSS_Settings.PortHandshake.ToString());
        }
        private void GPSPortHandshake_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as System.Windows.Controls.ComboBox;
            // узнаем выбранное значение
            Sett.GNSS_Settings.PortHandshake = (System.IO.Ports.Handshake)Enum.Parse(typeof(System.IO.Ports.Handshake), comboBox.SelectedItem.ToString()); //FMeas.Settings.GPS.Default.GPSPortHandshake = comboBox.SelectedItem as string;
                                                                                                                                                           //port.Text = value;
        }
        #endregion

        #region MAPS
        private void MapsFolder_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog OF = new System.Windows.Forms.FolderBrowserDialog();
            if (OF.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    if (System.IO.Directory.Exists(OF.SelectedPath))
                    {
                        Sett.Map_Settings.MapPath = OF.SelectedPath;
                    }
                }
                catch (Exception exp) { MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "SettingsControl", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name }; }
            }
            //OpenFileDialog OF = new OpenFileDialog();
            //OF.Filter = "Текстовые файлы(*.mpk)|*.mpk";
            //if (OF.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //    Sett.Map_Settings.MapsFile = OF.FileName;
        }

























        /*private void SaveSettingsMaps()
        {
            FMeas.Settings.Maps.Default.MapsFile = MapsFolder_But.Content.ToString();
        }
        private void InitializeSettingsMaps()
        {
            if (FMeas.Settings.Maps.Default.MapsFile != "")
            {
                MapsFolder_But.Content = FMeas.Settings.Maps.Default.MapsFile;
            }
        }*/

        #endregion

        private void AddIdentificationFormula_Click(object sender, RoutedEventArgs e)
        {
            if (OpsosIdentification_cb.SelectedIndex >= 0)
            {
                ComboBoxItem cbi = (ComboBoxItem)OpsosIdentification_cb.SelectedValue;
                Settings.OPSOSIdentification_Set oi = new Settings.OPSOSIdentification_Set()
                {
                    OpsosName = "",
                    Techonology = (string)cbi.Content,
                    MCC = 0,
                    MNC = 0,
                    Formula = ""
                };
                Sett.MeasMons_Settings.OPSOSIdentifications.Add(oi);

            }

        }

        private void DeleteIdentificationFormula_Click(object sender, RoutedEventArgs e)
        {
            if (OPSOSIdentifications_DG.SelectedIndex >= 0)
            {
                if (OPSOSIdentifications_DG.SelectedItem is Settings.OPSOSIdentification_Set)
                {
                    Settings.OPSOSIdentification_Set oi = (Settings.OPSOSIdentification_Set)OPSOSIdentifications_DG.SelectedItem;
                    Sett.MeasMons_Settings.OPSOSIdentifications.Remove(oi);
                }
            }
        }

        

        private void DeleteMeasMask_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Uni_PreKeyDownDecimal(object sender, TextCompositionEventArgs e)
        {
            help.CheckIsDecimal((TextBox)sender, e);
        }       

    }
}
