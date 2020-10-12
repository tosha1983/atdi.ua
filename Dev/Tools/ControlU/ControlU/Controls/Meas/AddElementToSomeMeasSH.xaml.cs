using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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

namespace ControlU.Controls.Meas
{
    /// <summary>
    /// Логика взаимодействия для AddElementToAnyMeasSH.xaml
    /// </summary>
    public partial class AddElementToSomeMeasSH : UserControl, INotifyPropertyChanged
    {
        Helpers.Helper help;
        Equipment.SignalHound sh = MainWindow.SHReceiver;
        public Equipment.DataSomeMeas Data
        {
            get { return _Data; }
            set { _Data = value; OnPropertyChanged("Data"); }
        }
        public Equipment.DataSomeMeas _Data = new Equipment.DataSomeMeas() { };

        public bool Сhange
        {
            get { return _Сhange; }
            set { _Сhange = value; OnPropertyChanged("Сhange"); }
        }
        public bool _Сhange = false;

        #region data from device
        #region RBW / VBW
        decimal[] RBWArr = new decimal[] { };

        public int RBWIndex
        {
            get { return _RBWIndex; }
            set
            {
                if (value > -1 && value < RBWArr.Length) { _RBWIndex = value; Data.RBW = RBWArr[_RBWIndex]; }
                else if (value < 0) { _RBWIndex = 0; Data.RBW = RBWArr[_RBWIndex]; }
                else if (value >= RBWArr.Length) { _RBWIndex = RBWArr.Length - 1; Data.RBW = RBWArr[_RBWIndex]; }
                if (_VBWIndex > _RBWIndex) VBWIndex = _RBWIndex;
            }
        }
        private int _RBWIndex = 7;

        decimal[] VBWArr = new decimal[] { };
        public int VBWIndex
        {
            get { return _VBWIndex; }
            set
            {
                if (value > -1 && value < VBWArr.Length) { _VBWIndex = value; Data.VBW = VBWArr[_VBWIndex]; }
                else if (value < 0) { _VBWIndex = 0; Data.VBW = VBWArr[_VBWIndex]; }
                else if (value >= VBWArr.Length) { _VBWIndex = VBWArr.Length - 1; Data.VBW = RBWArr[_VBWIndex]; }
                if (_VBWIndex > _RBWIndex) { VBWIndex = _RBWIndex; Data.VBW = RBWArr[_VBWIndex]; }
            }
        }
        private int _VBWIndex = 7;


        #endregion
        public ObservableCollection<Equipment.LevelUnit> LevelUnits
        {
            get { return _LevelUnits; }
            set { _LevelUnits = value; OnPropertyChanged("LevelUnits"); }
        }
        private ObservableCollection<Equipment.LevelUnit> _LevelUnits = new ObservableCollection<Equipment.LevelUnit>() { };

        public ObservableCollection<Equipment.ParamWithUI> TraceTypes
        {
            get { return _TraceTypes; }
            set { _TraceTypes = value; OnPropertyChanged("TraceTypes"); }
        }
        private ObservableCollection<Equipment.ParamWithUI> _TraceTypes = new ObservableCollection<Equipment.ParamWithUI>();
        #endregion

        /////////////////////////////////////////////////////////////////////////////////////
        public AddElementToSomeMeasSH()
        {
            RBWArr = sh.RBWArr;
            VBWArr = sh.VBWArr;
            LevelUnits = sh.LevelUnits;
            TraceTypes = sh.TraceTypes;
            help = new Helpers.Helper();
            InitializeComponent();
            this.DataContext = this;
        }




        #region Freq
        private void FreqCentr_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Data.FreqCentr += ((decimal)help.MouseWheelOutStep(sender, e)) * Data.FreqSpan / 100;
        }
        private void FreqCentr_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Data.FreqCentr = help.KeyDownDecimal(Data.FreqCentr / 1000000, (TextBox)sender, e) * 1000000;
            }
        }

        private void FreqSpan_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Data.FreqSpan += ((decimal)help.MouseWheelOutStep(sender, e)) * Data.FreqSpan / 100;
        }
        private void FreqSpan_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Data.FreqSpan = help.KeyDownDecimal(Data.FreqSpan / 1000000, (TextBox)sender, e) * 1000000;
            }
        }

        private void FreqStart_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Data.FreqStart += ((decimal)help.MouseWheelOutStep(sender, e)) * Data.FreqSpan / 100;
        }
        private void FreqStart_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Data.FreqStart = help.KeyDownDecimal(Data.FreqStart / 1000000, (TextBox)sender, e) * 1000000;
            }
        }

        private void FreqStop_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Data.FreqStop += ((decimal)help.MouseWheelOutStep(sender, e)) * Data.FreqSpan / 100;
        }
        private void FreqStop_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Data.FreqStop = help.KeyDownDecimal(Data.FreqStop / 1000000, (TextBox)sender, e) * 1000000;
            }
        }
        #endregion

        #region Channel
        private void Channel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Data.Channel = help.KeyDownUInt(Data.Channel, (TextBox)sender, e);
                if (Data.FreqType == 2 || Data.FreqType == 3)//gsm
                {
                    if (Data.Channel < 0) Data.Channel = 0;
                    else if (Data.Channel > 124 && Data.Channel <= 318) Data.Channel = 124;
                    else if (Data.Channel > 318 && Data.Channel < 512) Data.Channel = 512;
                    else if (Data.Channel > 885 && Data.Channel < 930) Data.Channel = 885;
                    else if (Data.Channel > 930 && Data.Channel < 975) Data.Channel = 975;
                    else if (Data.Channel > 1023) Data.Channel = 1023;
                    try
                    {
                        if (Data.FreqType == 2)
                            Data.FreqCentr = help.GetGSMCHFromChannel(Data.Channel).FreqDn;
                        else if (Data.FreqType == 3) Data.FreqCentr = help.GetGSMCHFromChannel(Data.Channel).FreqUp;
                    }
                    catch (Exception exp) { ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = exp.Message; }
                }
                if (Data.FreqType == 4)//umts
                {
                    if (Data.Channel < 412) Data.Channel = 412;
                    else if (Data.Channel > 687 && Data.Channel < 1162) Data.Channel = 1162;
                    else if (Data.Channel > 1513 && Data.Channel < 1537) Data.Channel = 1537;
                    else if (Data.Channel > 1738 && Data.Channel < 1887) Data.Channel = 1887;
                    else if (Data.Channel > 2087 && Data.Channel < 9662) Data.Channel = 9662;
                    else if (Data.Channel > 9938 && Data.Channel < 10562) Data.Channel = 10562;
                    else if (Data.Channel > 10838) Data.Channel = 10838;
                    try
                    {
                        if (Data.FreqType == 4)
                            Data.FreqCentr = help.GetUMTSCHFromChannelDn(Data.Channel).FreqDn;
                        else if (Data.FreqType == 5) Data.FreqCentr = help.GetUMTSCHFromChannelUp(Data.Channel).FreqUp;
                    }
                    catch (Exception exp) { ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = exp.Message; }
                }
            }
        }
        private void Channel_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Data.FreqType == 2 || Data.FreqType == 3) //gsm
            {
                #region
                int i = help.MouseWheelOutStep(sender, e);
                if (i < 0 && Data.Channel == 0) Data.Channel = 1023;
                else if (i > 0 && Data.Channel == 0) Data.Channel += i;
                else if (Data.Channel > 0 && Data.Channel < 124) Data.Channel += i;
                else if (i > 0 && Data.Channel == 124) Data.Channel = 512;
                else if (i < 0 && Data.Channel == 124) Data.Channel += i;


                else if (i < 0 && Data.Channel == 512) Data.Channel = 124;
                else if (i > 0 && Data.Channel == 512) Data.Channel += i;
                else if (Data.Channel > 512 && Data.Channel < 885) Data.Channel += i;
                else if (i > 0 && Data.Channel == 885) Data.Channel = 975;
                else if (i < 0 && Data.Channel == 885) Data.Channel += i;

                else if (i < 0 && Data.Channel == 975) Data.Channel = 885;
                else if (i > 0 && Data.Channel == 975) Data.Channel += i;
                else if (Data.Channel > 975 && Data.Channel < 1023) Data.Channel += i;
                else if (i > 0 && Data.Channel == 1023) Data.Channel = 0;
                else if (i < 0 && Data.Channel == 1023) Data.Channel += i;
                try
                {
                    if (Data.FreqType == 2)
                        Data.FreqCentr = help.GetGSMCHFromChannel(Data.Channel).FreqDn;
                    else if (Data.FreqType == 3) Data.FreqCentr = help.GetGSMCHFromChannel(Data.Channel).FreqUp;
                }
                catch (Exception exp) { ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = exp.Message; }
                #endregion
            }
            else if (Data.FreqType == 4) //umts
            {
                #region
                int i = help.MouseWheelOutStep(sender, e);
                if (i < 0 && Data.Channel == 412) Data.Channel = 10838;
                else if (i > 0 && Data.Channel == 412) Data.Channel += i * 25;
                else if (Data.Channel > 412 && Data.Channel < 687) Data.Channel += i * 25;
                else if (i > 0 && Data.Channel == 687) Data.Channel = 1162;
                else if (i < 0 && Data.Channel == 687) Data.Channel += i * 25;

                else if (i < 0 && Data.Channel == 1162) Data.Channel = 687;
                else if (i > 0 && Data.Channel == 1162) Data.Channel += i;
                else if (Data.Channel > 1162 && Data.Channel < 1513) Data.Channel += i;
                else if (i > 0 && Data.Channel == 1513) Data.Channel = 1537;
                else if (i < 0 && Data.Channel == 1513) Data.Channel += i;


                else if (i < 0 && Data.Channel == 1537) Data.Channel = 1513;
                else if (i > 0 && Data.Channel == 1537) Data.Channel += i;
                else if (Data.Channel > 1537 && Data.Channel < 1738) Data.Channel += i;
                else if (i > 0 && Data.Channel == 1738) Data.Channel = 1887;
                else if (i < 0 && Data.Channel == 1738) Data.Channel += i;

                else if (i < 0 && Data.Channel == 1887) Data.Channel = 1738;
                else if (i > 0 && Data.Channel == 1887) Data.Channel += i * 25;
                else if (Data.Channel > 1887 && Data.Channel < 2087) Data.Channel += i * 25;
                else if (i > 0 && Data.Channel == 2087) Data.Channel = 9662;
                else if (i < 0 && Data.Channel == 2087) Data.Channel += i * 25;

                else if (i < 0 && Data.Channel == 9662) Data.Channel = 2087;
                else if (i > 0 && Data.Channel == 9662) Data.Channel += i;
                else if (Data.Channel > 9662 && Data.Channel < 9938) Data.Channel += i;
                else if (i > 0 && Data.Channel == 9938) Data.Channel = 10562;
                else if (i < 0 && Data.Channel == 9938) Data.Channel += i;

                else if (i < 0 && Data.Channel == 10562) Data.Channel = 9938;
                else if (i > 0 && Data.Channel == 10562) Data.Channel += i;
                else if (Data.Channel > 10562 && Data.Channel < 10838) Data.Channel += i;
                else if (i > 0 && Data.Channel == 10838) Data.Channel = 412;
                else if (i < 0 && Data.Channel == 10838) Data.Channel += i;

                try
                {
                    if (Data.FreqType == 4)
                        Data.FreqCentr = help.GetUMTSCHFromChannelDn(Data.Channel).FreqDn;
                    // else if (Data.FreqType == 3) Data.FreqCentr = help.GetUMTSCHFromChannelDn(Data.Channel).FreqUp;
                }
                catch (Exception exp) { ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = exp.Message; }
                #endregion
            }
            else if (Data.FreqType == 5) //umts
            {
                #region
                int i = help.MouseWheelOutStep(sender, e);
                if (i < 0 && Data.Channel == 12) Data.Channel = 9888;
                else if (i > 0 && Data.Channel == 12) Data.Channel += i * 25;
                else if (Data.Channel > 12 && Data.Channel < 287) Data.Channel += i * 25;
                else if (i > 0 && Data.Channel == 287) Data.Channel = 937;
                else if (i < 0 && Data.Channel == 287) Data.Channel += i * 25;

                else if (i < 0 && Data.Channel == 937) Data.Channel = 287;
                else if (i > 0 && Data.Channel == 937) Data.Channel += i;
                else if (Data.Channel > 937 && Data.Channel < 1288) Data.Channel += i;
                else if (i > 0 && Data.Channel == 1288) Data.Channel = 1312;
                else if (i < 0 && Data.Channel == 1288) Data.Channel += i;


                else if (i < 0 && Data.Channel == 1312) Data.Channel = 1288;
                else if (i > 0 && Data.Channel == 1312) Data.Channel += i;
                else if (Data.Channel > 1312 && Data.Channel < 1513) Data.Channel += i;
                else if (i > 0 && Data.Channel == 1513) Data.Channel = 1662;
                else if (i < 0 && Data.Channel == 1513) Data.Channel += i;

                else if (i < 0 && Data.Channel == 1662) Data.Channel = 1513;
                else if (i > 0 && Data.Channel == 1662) Data.Channel += i * 25;
                else if (Data.Channel > 1662 && Data.Channel < 1862) Data.Channel += i * 25;
                else if (i > 0 && Data.Channel == 1862) Data.Channel = 9262;
                else if (i < 0 && Data.Channel == 1862) Data.Channel += i * 25;

                else if (i < 0 && Data.Channel == 9262) Data.Channel = 1862;
                else if (i > 0 && Data.Channel == 9262) Data.Channel += i;
                else if (Data.Channel > 9262 && Data.Channel < 9538) Data.Channel += i;
                else if (i > 0 && Data.Channel == 9538) Data.Channel = 9612;
                else if (i < 0 && Data.Channel == 9538) Data.Channel += i;

                else if (i < 0 && Data.Channel == 9612) Data.Channel = 9538;
                else if (i > 0 && Data.Channel == 9612) Data.Channel += i;
                else if (Data.Channel > 9612 && Data.Channel < 9888) Data.Channel += i;
                else if (i > 0 && Data.Channel == 9888) Data.Channel = 12;
                else if (i < 0 && Data.Channel == 9888) Data.Channel += i;

                try
                {
                    if (Data.FreqType == 5)
                        Data.FreqCentr = help.GetUMTSCHFromChannelUp(Data.Channel).FreqUp;
                    // else if (Data.FreqType == 3) Data.FreqCentr = help.GetUMTSCHFromChannelDn(Data.Channel).FreqUp;
                }
                catch (Exception exp) { ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = exp.Message; }
                #endregion
            }
        }
        #endregion Channel

        #region RBW / VBW
        private void RBWMinus_Click(object sender, RoutedEventArgs e)
        {
            RBWIndex--;
            Data.RBWIndex = RBWIndex;
        }
        private void RBWPlus_Click(object sender, RoutedEventArgs e)
        {
            RBWIndex++;
            Data.RBWIndex = RBWIndex;
        }
        ///////////////////////////////////////
        private void VBWMinus_Click(object sender, RoutedEventArgs e)
        {
            VBWIndex--;
            Data.VBWIndex = VBWIndex;
        }
        private void VBWPlus_Click(object sender, RoutedEventArgs e)
        {
            VBWIndex++;
            Data.VBWIndex = VBWIndex;
        }
        #endregion

        #region Sweep
        private void SweepTimeMinus_Click(object sender, RoutedEventArgs e)
        {
            Data.SweepTime -= help.PlMntime(Data.SweepTime);
            if (Data.SweepTime <= 0.00001m) Data.SweepTime = 0.00001m;

        }
        private void SweepTimePlus_Click(object sender, RoutedEventArgs e)
        {
            Data.SweepTime += help.PlMntime(Data.SweepTime);
            if (Data.SweepTime >= 1) Data.SweepTime = 1;
        }
        private void SweepTime_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string temp = (sender as System.Windows.Controls.TextBox).Text;
                string t = string.Empty;
                foreach (char c in temp)
                {
                    if (Char.IsDigit(c) || c == '.' || c == ',')
                    {
                        t += c;
                    }
                }
                Data.SweepTime = decimal.Parse(t.Replace(",", "."), CultureInfo.InvariantCulture);
                if (Data.SweepTime <= 0.00001m) Data.SweepTime = 0.00001m;
                else if (Data.SweepTime >= 1) Data.SweepTime = 1;
            }
        }
        private void SweepTime_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            int step = help.MouseWheelOutStep(sender, e);
            if (step > 0) { Data.SweepTime += help.PlMntime(Data.SweepTime); if (Data.SweepTime >= 1) Data.SweepTime = 1; }
            if (step < 0) { Data.SweepTime -= help.PlMntime(Data.SweepTime); if (Data.SweepTime <= 0.00001m) Data.SweepTime = 0.00001m; }
        }
        #endregion

        #region Amplitude
        private void RefLevelMinus_Click(object sender, RoutedEventArgs e)
        {
            Data.RefLevel--;
        }
        private void RefLevelPlus_Click(object sender, RoutedEventArgs e)
        {
            Data.RefLevel++;
        }
        private void RefLevel_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Data.RefLevel += (decimal)help.MouseWheelOutStep(sender, e);
        }

        private void RangeMinus_Click(object sender, RoutedEventArgs e)
        {
            Data.Range -= 10;
            if (Data.Range < 10) Data.Range = 10;
        }
        private void RangePlus_Click(object sender, RoutedEventArgs e)
        {
            Data.Range += 10;
            if (Data.Range > 200) Data.Range = 200;
        }
        private void Range_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Data.Range += (decimal)help.MouseWheelOutStep(sender, e) * 10;
            if (Data.Range < 10) Data.Range = 10;
            else if (Data.Range > 200) Data.Range = 200;
        }


        private void GainMinus_Click(object sender, RoutedEventArgs e)
        {
            Data.GainIndex--;
            if (Data.GainIndex < -1) Data.GainIndex = -1;
        }
        private void GainPlus_Click(object sender, RoutedEventArgs e)
        {
            Data.GainIndex++;
            if (Data.GainIndex > 3) Data.GainIndex = 3;
        }
        private void Gain_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Data.GainIndex += help.MouseWheelOutStep(sender, e);
            if (Data.AttIndex < -1) Data.AttIndex = -1;
            if (Data.GainIndex > 3) Data.GainIndex = 3;
        }
        private void AttMinus_Click(object sender, RoutedEventArgs e)
        {
            Data.AttIndex--;
            if (Data.AttIndex < -1) Data.AttIndex = -1;
        }
        private void AttPlus_Click(object sender, RoutedEventArgs e)
        {
            Data.AttIndex++;
            if (Data.AttIndex > 3) Data.AttIndex = 3;
        }
        private void Att_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Data.AttIndex += help.MouseWheelOutStep(sender, e);
            if (Data.AttIndex < -1) Data.AttIndex = -1;
            if (Data.AttIndex > 3) Data.AttIndex = 3;
        }
        #endregion Amplitude

        #region Trace
        private void TraceReset_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TraceAverageMinus_Click(object sender, RoutedEventArgs e)
        {
            Data.TraceAveragedList.AveragingCount--;
            if (Data.TraceAveragedList.AveragingCount < 2) Data.TraceAveragedList.AveragingCount = 2;
        }
        private void TraceAveragePlus_Click(object sender, RoutedEventArgs e)
        {
            Data.TraceAveragedList.AveragingCount++;
            if (Data.TraceAveragedList.AveragingCount > 500) Data.TraceAveragedList.AveragingCount = 500;
        }
        private void TraceAverage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Data.TraceAveragedList.AveragingCount = help.KeyDownUInt(Data.TraceAveragedList.AveragingCount, (TextBox)sender, e);
                if (Data.TraceAveragedList.AveragingCount < 2) Data.TraceAveragedList.AveragingCount = 2;
                else if (Data.TraceAveragedList.AveragingCount > 500) Data.TraceAveragedList.AveragingCount = 500;
            }
        }

        private void TraceTrackingMinus_Click(object sender, RoutedEventArgs e)
        {
            Data.TraceTrackedList.TrackingCount--;
            if (Data.TraceTrackedList.TrackingCount < 2) Data.TraceTrackedList.TrackingCount = 2;
        }

        private void TraceTrackingPlus_Click(object sender, RoutedEventArgs e)
        {
            Data.TraceTrackedList.TrackingCount++;
            if (Data.TraceTrackedList.TrackingCount > 500) Data.TraceTrackedList.TrackingCount = 500;
        }

        private void TraceTracking_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Data.TraceTrackedList.TrackingCount = help.KeyDownUInt(Data.TraceTrackedList.TrackingCount, (TextBox)sender, e);
                if (Data.TraceTrackedList.TrackingCount < 2) Data.TraceTrackedList.TrackingCount = 2;
                else if (Data.TraceTrackedList.TrackingCount > 500) Data.TraceTrackedList.TrackingCount = 500;
            }
        }


        #endregion Trace

        #region Uni
        private void Uni_PreKeyDownDecimal(object sender, TextCompositionEventArgs e)
        {
            help.CheckIsDecimal((TextBox)sender, e);
        }
        private void Uni_GotMouseCapture(object sender, MouseEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.SelectAll();
        }
        private void Uni_PreKeyDownInteger(object sender, TextCompositionEventArgs e)
        {
            help.CheckIsInt((TextBox)sender, e);
        }
        #endregion

        // Событие, которое нужно вызывать при изменении
        public event PropertyChangedEventHandler PropertyChanged;
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            GUIThreadDispatcher.Instance.Invoke(() =>
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            });
        }


    }
}
