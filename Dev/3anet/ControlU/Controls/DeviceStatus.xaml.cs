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

namespace ControlU.Controls
{
    /// <summary>
    /// Логика взаимодействия для DeviceStatus.xaml
    /// </summary>
    public partial class DeviceStatus : UserControl
    {

        public DeviceStatus()
        {
            InitializeComponent();

            //Receiver.DataContext = MainWindow.Rcvr;
            sp.DataContext = App.Sett;
            //SA.DataContext = MainWindow.An;
            //SA_bt.DataContext = MainWindow.An;

            SA.DataContext = MainWindow.An;
            SA_bat.DataContext = MainWindow.An;

            RCV.DataContext = MainWindow.Rcvr;
            //Romes.DataContext = MainWindow.RCR;
            TSMx_bt.DataContext = MainWindow.tsmx;

            SH_bt.DataContext = MainWindow.SHReceiver;
            GPS.DataContext = MainWindow.gps;
            RBS.DataContext = MainWindow.rbs;
        }

        private void AllState_Click(object sender, RoutedEventArgs e)
        {
            if (App.Sett.Equipments_Settings.GNSS.UseEquipment) { MainWindow.gps.ConnectToGNSS(); }
            if (App.Sett.Equipments_Settings.SpectrumAnalyzer.UseEquipment) { MainWindow.An.Run = !MainWindow.An.Run; }
            if (App.Sett.Equipments_Settings.RuSReceiver.UseEquipment) { MainWindow.Rcvr.Run = !MainWindow.Rcvr.Run; }
            if (App.Sett.Equipments_Settings.RuSRomesRC.UseEquipment) { MainWindow.RCR.Run = !MainWindow.RCR.Run; }
            if (App.Sett.Equipments_Settings.RuSTSMx.UseEquipment)
            {
                if (MainWindow.tsmx.Run == false && MainWindow.tsmx.IsRuning == false)
                    MainWindow.tsmx.Run = true;
                else if (MainWindow.tsmx.Run == true)
                    MainWindow.tsmx.Run = false;
            }

            if (App.Sett.Equipments_Settings.WR61.UseEquipment) { MainWindow.SHReceiver.Run = !MainWindow.SHReceiver.Run; }
        }
        private void SAState_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.An.Run = !MainWindow.An.Run;
        }
        private void RCVState_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Rcvr.Run = !MainWindow.Rcvr.Run;
        }
        private void RCRState_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.RCR.Run = !MainWindow.RCR.Run;
        }
        private void TSMxState_Click(object sender, RoutedEventArgs e)
        {
            //MainWindow.tsmx.Run = !MainWindow.tsmx.Run;

            if (MainWindow.tsmx.Run == false && MainWindow.tsmx.IsRuning == false)
                MainWindow.tsmx.Run = true;
            else if (MainWindow.tsmx.Run == true)
                MainWindow.tsmx.Run = false;
        }

        bool gps = false;
        private void GPSState_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.gps.Run = !MainWindow.gps.Run;
        }

        private void SHState_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.SHReceiver.Run = !MainWindow.SHReceiver.Run;
        }
        private void RBSState_Click(object sender, RoutedEventArgs e)
        {
            //MainWindow.rbs.Run = !MainWindow.rbs.Run;
        }
        //private void SAState_Click_v2(object sender, RoutedEventArgs e)
        //{
        //    MainWindow.An.Run = !MainWindow.An.Run;
        //}
    }
}
