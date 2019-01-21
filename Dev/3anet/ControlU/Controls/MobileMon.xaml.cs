using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ControlU.Controls
{
    /// <summary>
    /// Логика взаимодействия для MobileMon.xaml
    /// </summary>
    public partial class MobileMon : UserControl, INotifyPropertyChanged
    {
        DB.NpgsqlDB_v2 db = MainWindow.db_v2;
        Equipment.RsReceiver_v2 rcv = MainWindow.Rcvr;
        Equipment.IdentificationData Idfd = MainWindow.IdfData;
        Equipment.RCRomes rcr = MainWindow.RCR;


        public object BTSSelectedItem
        {
            get { return _BTSSelectedItem; }
            set { _BTSSelectedItem = value; OnPropertyChanged("BTSSelectedItem"); }
        }
        private object _BTSSelectedItem;

        public string InformationBlock_TextData
        {
            get { return _InformationBlock_TextData; }
            set { _InformationBlock_TextData = value; OnPropertyChanged("InformationBlock_TextData"); }
        }
        private string _InformationBlock_TextData = "";


        public MobileMon()
        {
            InitializeComponent();
            DrawSpec.DataContext = App.Sett;
            StateMeasMon_btn.DataContext = MainWindow.db_v2;

            tsmxxx.DataContext = MainWindow.tsmx;
            frst.DataContext = MainWindow.SHReceiver;
            frst2.DataContext = MainWindow.SHReceiver;

            SIBs_Expander.DataContext = this;
            TSMxPanel.DataContext = MainWindow.tsmx;
            MonDataGrid_uc.PropertyChanged += DG_DataSelectedItem_PropertyChanged;

            Identification_GSM.PropertyChanged += BTSSelectedItem_PropertyChanged;
            Identification_UMTS.PropertyChanged += BTSSelectedItem_PropertyChanged;
            Identification_LTE.PropertyChanged += BTSSelectedItem_PropertyChanged;
            Identification_CDMA.PropertyChanged += BTSSelectedItem_PropertyChanged;

            Identification_GSM.DataContext = App.Sett;
            Identification_UMTS.DataContext = App.Sett;
            Identification_LTE.DataContext = App.Sett;
            Identification_CDMA.DataContext = App.Sett;
            Identification_ACD.DataContext = App.Sett;
        }

        private void InformationBlock_Select_Click(object sender, RoutedEventArgs e)
        {
            Button bt = (Button)sender;
            InformationBlock_TextData = bt.Tag.ToString();
        }
        private void BTSSelectedItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "BTSSelectedItem")
            {
                if (sender is Controls.Identification.GSM) BTSSelectedItem = Identification_GSM.BTSSelectedItem;
                else if (sender is Controls.Identification.UMTS) BTSSelectedItem = Identification_UMTS.BTSSelectedItem;
                else if (sender is Controls.Identification.LTE) BTSSelectedItem = Identification_LTE.BTSSelectedItem;
                else if (sender is Controls.Identification.CDMA) BTSSelectedItem = Identification_CDMA.BTSSelectedItem;
                InformationBlock_TextData = "";
            }
        }
        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
                                                                  // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void StateMeasMon_Click(object sender, RoutedEventArgs e)
        {
            if (App.Lic)
            {
                if (SelectedUserCheck())
                {
                    if (SpectrumMeasCheck())
                    {
                        int count = db.MeasMonCheckSelectedTask();
                        if (count == 0 || count == 1)
                        {
                            db.MeasMonState = !db.MeasMonState;
                            if (db.MeasMonState)
                            {
                                db.MeasMon.GetUnifreqsOccupancy();
                            }
                            if (App.Sett.MeasMons_Settings.SpectrumMeasDeviece == App.Sett.Equipments_Settings.SpectrumAnalyzer.ID
                                && MainWindow.An.IsRuning == true)
                            {
                                MainWindow.An.IsMeasMon = db.MeasMonState;
                            }
                            else if (App.Sett.MeasMons_Settings.SpectrumMeasDeviece == App.Sett.Equipments_Settings.RuSReceiver.ID
                                && MainWindow.Rcvr.IsRuning == true)
                            {
                                MainWindow.Rcvr.IsMeasMon = db.MeasMonState;
                            }
                            else if (App.Sett.MeasMons_Settings.SpectrumMeasDeviece == App.Sett.Equipments_Settings.RuSTSMx.ID
                                && MainWindow.tsmx.Run == true)
                            {
                                MainWindow.tsmx.IsMeasMon = db.MeasMonState;
                            }
                            else if (App.Sett.MeasMons_Settings.SpectrumMeasDeviece == App.Sett.Equipments_Settings.SignalHound.ID
                                && MainWindow.SHReceiver.Run == true)
                            {
                                MainWindow.SHReceiver.IsMeasMon = db.MeasMonState;
                            }
                        }
                        if (count > 1) { ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = "Выбрано больше одного таска"; }
                    }
                }
            }
            else
            {
                ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = this.FindResource("LicenseFail").ToString();
            }
        }
        private bool SelectedUserCheck()
        {
            bool res = false;
            if (App.Sett.UsersApps_Settings.SelectedUser == null || App.Sett.UsersApps_Settings.Users == null || App.Sett.UsersApps_Settings.Users.Count == 0)
            {
                string msg = this.FindResource("SelectedUserFalse").ToString();
                ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = msg;
            }
            else { res = true; }
            return res;
        }
        private bool SpectrumMeasCheck()
        {
            bool res = false;
            if (App.Sett.MeasMons_Settings.SpectrumMeasDeviece == 0)
            {
                string msg = this.FindResource("SpectrumMeasFalse").ToString();
                ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = msg;
            }
            else { res = true; }
            return res;
        }

        private void RCRomesConnect_Click(object sender, RoutedEventArgs e)
        {
            rcr.DataCycle = true;
            rcr.Connect();
        }



        ////private void MonDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        ////{
        ////    if (MonDataGrid.SelectedIndex >= 0)
        ////    {
        ////        DB.MeasData smd = (DB.MeasData)MonDataGrid.Items[MonDataGrid.SelectedIndex];
        ////        MobileMonSpectrum_uc.SpectrumFromReceiver = false;
        ////        MobileMonSpectrum_uc.tomeas = smd;
        ////    }
        ////}

        private void SpectrumFromReceiver_Click(object sender, RoutedEventArgs e)
        {
            DrawSpec.SpectrumFromDevice = !DrawSpec.SpectrumFromDevice;
        }



        private void GSMIsEnabled_Click(object sender, RoutedEventArgs e)
        {
            Equipment.IdentificationData.GSM.TechIsEnabled = !Equipment.IdentificationData.GSM.TechIsEnabled;
        }
        private void UMTSIsEnabled_Click(object sender, RoutedEventArgs e)
        {
            Equipment.IdentificationData.UMTS.TechIsEnabled = !Equipment.IdentificationData.UMTS.TechIsEnabled;
        }
        private void LTEIsEnabled_Click(object sender, RoutedEventArgs e)
        {
            Equipment.IdentificationData.LTE.TechIsEnabled = !Equipment.IdentificationData.LTE.TechIsEnabled;
        }
        private void CDMAIsEnabled_Click(object sender, RoutedEventArgs e)
        {
            Equipment.IdentificationData.CDMA.TechIsEnabled = !Equipment.IdentificationData.CDMA.TechIsEnabled;
        }

        private void UHFIsEnabled_Click(object sender, RoutedEventArgs e)
        {
            Idfd.UHFTechIsEnabled = !Idfd.UHFTechIsEnabled;
        }
        //DataGridRow WRLSoldselected;


        private void SendToATDI_Click(object sender, RoutedEventArgs e)
        {
            //MainWindow.db_v2.SendMeasDataFromMonMeas();
            //MainWindow.atdi.SendResults(MainWindow.db_v2.ATDIRes);

            //MainWindow.atdi.SendResults(MainWindow.db_v2.SendMeasDataFromMonMeas());
        }
        private void DG_DataSelectedItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DG_DataSelectedItem")
            {
                DrawSpec.tomeas = MonDataGrid_uc.DG_DataSelectedItem;
                DrawSpec.SpectrumFromDevice = false;

            }
        }
        /// <summary>
        /// сохраняем измененные настройки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingsFreqUse_Changed(object sender, RoutedEventArgs e)
        {
            App.Sett.SaveTSMxReceiver();
        }
        /// <summary>
        /// сохраняем измененные настройки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingsACD_Changed(object sender, SelectionChangedEventArgs e)
        {
            App.Sett.SaveTSMxReceiver();
        }
    }

}
