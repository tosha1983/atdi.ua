using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для AddElementToAnyMeas.xaml
    /// </summary>
    public partial class AddElementToSomeMeas : UserControl, INotifyPropertyChanged
    {
        Helpers.Helper help;
        public Equipment.DataSomeMeas Data
        {
            get { return _Data; }
            set { _Data = value; CloneData(_Data); OnPropertyChanged("Data"); }
        }
        public Equipment.DataSomeMeas _Data = new Equipment.DataSomeMeas() { };

        public Equipment.DataSomeMeas DataClone
        {
            get { return _DataClone; }
            set { _DataClone = value; OnPropertyChanged("DataClone"); }
        }
        public Equipment.DataSomeMeas _DataClone = new Equipment.DataSomeMeas() { };

        public bool Сhange
        {
            get { return _Сhange; }
            set { _Сhange = value; OnPropertyChanged("Сhange"); }
        }
        public bool _Сhange = false;

        public AddElementToSomeMeas()
        {
            help = new Helpers.Helper();
            InitializeComponent();
            this.DataContext = this;
        }
        private void CloneData(Equipment.DataSomeMeas data)
        {
            DataClone = new Equipment.DataSomeMeas()
            {
                State = data.State,

                DeviceType = data.DeviceType,
                FreqType = data.FreqType,
                Channel = data.Channel,

                FreqCentr = data.FreqCentr,
                FreqSpan = data.FreqSpan,
                FreqStart = data.FreqStart,
                FreqStop = data.FreqStop,

                LevelUnit = data.LevelUnit,
                RBW = data.RBW,
                RBWIndex = data.RBWIndex,
                VBW = data.VBW,
                VBWIndex = data.VBWIndex,
                RefLevel = data.RefLevel,
                Range = data.Range,
                GainIndex = data.GainIndex,
                AttIndex = data.AttIndex,

                SweepTime = data.SweepTime,
                Trace = (Equipment.tracepoint[])data.Trace.Clone(),
                TraceTypeIndex = data.TraceTypeIndex,
                TraceAveragedList = data.TraceAveragedList,
                TraceTrackedList = data.TraceTrackedList,

                StayOnFrequency = data.StayOnFrequency,
                ThisStayOnFrequency = data.ThisStayOnFrequency,
                StayOnSignalState = data.StayOnSignalState,
                StayOnSignalType = data.StayOnSignalType,
                StayOnSignal = data.StayOnSignal,
                ThisStayOnSignal = data.ThisStayOnSignal,



            };
            OnPropertyChanged("DataClone");
        }
        //SelectionChanged="DeviceType_SelectionChanged" 
        private void DeviceType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DeviceControlParent.Children.Clear();
            int index = ((ComboBox)sender).SelectedIndex;
            if (index == 0)
            {
                AddElementToSomeMeasAN an = new AddElementToSomeMeasAN();
                an.Data = DataClone;
                DeviceControlParent.Children.Add(an);
            }
            else if (index == 3)
            {
                AddElementToSomeMeasSH sh = new AddElementToSomeMeasSH();
                sh.Data = DataClone;
                DeviceControlParent.Children.Add(sh);
            }
        }
        private void StayOnFrequencyMinus_Click(object sender, RoutedEventArgs e)
        {
            DataClone.StayOnFrequency -= help.PlMntime(DataClone.StayOnFrequency);
            if (DataClone.StayOnFrequency <= 0.001m) Data.SweepTime = 0.001m;
        }
        private void StayOnFrequencyPlus_Click(object sender, RoutedEventArgs e)
        {
            DataClone.StayOnFrequency += help.PlMntime(DataClone.StayOnFrequency);
            if (DataClone.StayOnFrequency >= 86400) Data.SweepTime = 86400;
        }
        private void StayOnFrequency_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DataClone.StayOnFrequency = help.KeyDownDecimal(DataClone.StayOnFrequency, (TextBox)sender, e);
                if (DataClone.StayOnFrequency <= 0.001m) Data.SweepTime = 0.001m;
                else if (DataClone.StayOnFrequency >= 86400) Data.SweepTime = 86400;
            }
        }

        private void StayOnFrequency_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            int i = help.MouseWheelOutStep(sender, e);
            if (i > 0)
            {
                DataClone.StayOnFrequency += help.PlMntime(DataClone.StayOnFrequency);
                if (DataClone.StayOnFrequency >= 86400) Data.SweepTime = 86400;
            }
            else if (i < 0)
            {
                DataClone.StayOnFrequency -= help.PlMntime(DataClone.StayOnFrequency);
                if (DataClone.StayOnFrequency <= 0.001m) Data.SweepTime = 0.001m;
            }
        }
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


        #region Down btn
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            //((SplashWindow)App.Current.MainWindow).m_mainWindow.SomeMeas.AddDataOk(this);
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            //((SplashWindow)App.Current.MainWindow).m_mainWindow.SomeMeas.AddDataCancel(this);
        }
        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            //((SplashWindow)App.Current.MainWindow).m_mainWindow.SomeMeas.AddDataApply(this);
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

        private void StayOnSignalMinus_Click(object sender, RoutedEventArgs e)
        {
            DataClone.StayOnSignal -= help.PlMntime(DataClone.StayOnSignal);
            if (DataClone.StayOnSignal <= 0.00001m) DataClone.StayOnSignal = 0.00001m;
        }

        private void StayOnSignalPlus_Click(object sender, RoutedEventArgs e)
        {
            DataClone.StayOnSignal += help.PlMntime(DataClone.StayOnSignal);
            if (DataClone.StayOnSignal >= 86400) DataClone.StayOnSignal = 86400;
        }

        private void StayOnSignal_KeyDown(object sender, KeyEventArgs e)
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
                DataClone.StayOnSignal = decimal.Parse(t.Replace(",", "."), CultureInfo.InvariantCulture);
                if (DataClone.StayOnSignal <= 0.00001m) DataClone.StayOnSignal = 0.00001m;
                else if (DataClone.StayOnSignal >= 1) DataClone.StayOnSignal = 86400;
            }
        }

        private void StayOnSignal_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            int step = help.MouseWheelOutStep(sender, e);
            if (step > 0) { DataClone.StayOnSignal += help.PlMntime(DataClone.StayOnSignal); if (DataClone.StayOnSignal >= 86400) DataClone.StayOnSignal = 86400; }
            if (step < 0) { DataClone.StayOnSignal -= help.PlMntime(DataClone.StayOnSignal); if (DataClone.StayOnSignal <= 0.00001m) DataClone.StayOnSignal = 0.00001m; }
        }


    }
}
