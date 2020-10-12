using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
    /// Логика взаимодействия для SaveTrackData.xaml
    /// </summary>
    public partial class SaveTrackData : UserControl, INotifyPropertyChanged
    {
        Helpers.Helper help;
        Task TaskSave = null;
        public DB.Track Track
        {
            get { return _Track; }
            set { _Track = value; OnPropertyChanged("Track"); }
        }
        private DB.Track _Track = null;

        public int SaveProgress
        {
            get { return _SaveProgress; }
            set { _SaveProgress = value; OnPropertyChanged("SaveProgress"); }
        }
        private int _SaveProgress = 0;

        public double AntennaGain
        {
            get { return _AntennaGain; }
            set { _AntennaGain = value; OnPropertyChanged("AntennaGain"); }
        }
        private double _AntennaGain = 0;

        public SaveTrackData(DB.Track track)
        {
            help = new Helpers.Helper();
            InitializeComponent();
            this.DataContext = this;
            Track = track;
        }


        private void AntennaGainMinus_Click(object sender, RoutedEventArgs e)
        {
            AntennaGain -= 0.1;
        }

        private void AntennaGainPlus_Click(object sender, RoutedEventArgs e)
        {
            AntennaGain += 0.1;
        }

        private void AntennaGain_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            AntennaGain += ((double)help.MouseWheelOutStep(sender, e)) / 10;
        }

        private void AntennaGain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AntennaGain = help.KeyDownDouble(AntennaGain, (TextBox)sender, e);
            }
        }

        private void Uni_PreKeyDownDecimal(object sender, TextCompositionEventArgs e)
        {
            help.CheckIsDoubleWithMinus((TextBox)sender, e);
        }

        private void Uni_GotMouseCapture(object sender, MouseEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.SelectAll();
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TaskSave == null)
                {
                    TaskSave = new Task(() =>
                    {
                        string strPath = App.Sett.Screen_Settings.ScreenFolder + "\\" + Track.table_name + " an_" + AntennaGain.ToString().Replace(",", ".") + ".csv";
                        if (!File.Exists(strPath))
                        {
                            using (System.IO.StreamWriter sw = System.IO.File.AppendText(strPath))
                            {
                                StringBuilder sb = new StringBuilder();
                                sb.Append("LAT;LON;LEVEL_DBMKVM;TIME;LEVEL_DBM;STANDART;FFREQ_MHZ;MEAS_GCID;");
                                sw.WriteLine(sb);
                                sb.Clear();
                                for (int i = 0; i < Track.Data.Count(); i++)
                                {

                                    for (int j = 0; j < Track.Data[i].level_results.Count(); j++)
                                    {
                                        sb.Append(Math.Round(Track.Data[i].level_results[j].location.latitude, 6).ToString() + ";");
                                        sb.Append(Math.Round(Track.Data[i].level_results[j].location.longitude, 6).ToString() + ";");
                                        sb.Append(Math.Round(help.CalcStrength(Track.Data[i].level_results[j].level_dbm, AntennaGain, (double)Track.Data[i].freq), 2).ToString() + ";");
                                        sb.Append(Track.Data[i].level_results[j].measurement_time.ToString("yyyy.MM.dd HH:mm:ss;"));
                                        sb.Append(Math.Round(Track.Data[i].level_results[j].level_dbm, 2).ToString() + ";");
                                        sb.Append(Track.Data[i].tech + ";");
                                        sb.Append(Math.Round(Track.Data[i].freq / 1000000, 6) + ";");
                                        sb.Append(Track.Data[i].gcid + ";");
                                        sw.WriteLine(sb);
                                        sb.Clear();
                                    }
                                    App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                                    {
                                        SaveProgress = i + 1;
                                    });
                                }
                                sb = null;
                                sw.Dispose();
                            }
                            App.Current.Dispatcher.BeginInvoke((Action)(() =>
                            {
                                ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = this.FindResource("SaveTrackData_DataSaved").ToString().Replace("track.csv", strPath);
                                ((SplashWindow)App.Current.MainWindow).m_mainWindow.CloseGlobalUC(this);
                            }));
                        }
                        else
                        {
                            App.Current.Dispatcher.BeginInvoke((Action)(() =>
                            {
                                ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = this.FindResource("FileIsExists").ToString().Replace("FileName", strPath);
                            }));
                        }
                        TaskSave = null;
                    });
                    TaskSave.Start();
                }
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "SaveTrackData", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (TaskSave == null)
            {
                ((SplashWindow)App.Current.MainWindow).m_mainWindow.CloseGlobalUC(this);
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


    }
}
