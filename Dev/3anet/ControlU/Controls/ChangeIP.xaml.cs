using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
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
    /// Логика взаимодействия для ChangeIP.xaml
    /// </summary>
    public partial class ChangeIP : UserControl, INotifyPropertyChanged
    {
        string AppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        Equipment.TelnetConnection tc = new Equipment.TelnetConnection();
        private string _IP = "";
        public string IP
        {
            get { return _IP; }
            set { _IP = value; OnPropertyChanged("IP"); }
        }
        private string _Mask = "";
        public string Mask
        {
            get { return _Mask; }
            set { _Mask = value; OnPropertyChanged("Mask"); }
        }
        private string _Gateway = "";
        public string Gateway
        {
            get { return _Gateway; }
            set { _Gateway = value; OnPropertyChanged("Gateway"); }
        }

        public ChangeIP()
        {
            InitializeComponent();
            addcntr.DataContext = this;

        }
        public void GetIP()
        {
            tc.Open(App.Sett.RsReceiver_Settings.IPAdress, App.Sett.RsReceiver_Settings.TCPPort);
            IP = tc.Query("SYSTem:COMMunicate:SOCKET:ADDRess?").Replace("\"", "").TrimEnd();
            Mask = tc.Query("SYSTem:COMMunicate:LAN:SUBMask?").Replace("\"", "").TrimEnd();
            Gateway = tc.Query("SYSTem:COMMunicate:LAN:GATeway?").Replace("\"", "").TrimEnd();
        }

        private void Apply_click(object sender, RoutedEventArgs e)
        {
            string s = "SYSTem:COMMunicate:SOCKET:ADDRess \"" + IP + "\";SYSTem:COMMunicate:LAN:SUBMask \"" + Mask + "\";SYSTem:COMMunicate:LAN:GATeway \"" + Gateway + "\";";
            System.Windows.MessageBox.Show(s);
            //Блабла бла
            tc.WriteLine(s);
            App.Sett.RsReceiver_Settings.IPAdress = IP;
            App.Sett.SaveRsReceiver();
            tc.Close();
            MainWindow mainWindow = (MainWindow)App.Current.MainWindow;
            mainWindow.CloseGlobalUC(mainWindow.SetCtrl.cip);
        }
        private void Close_click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)App.Current.MainWindow;
            mainWindow.CloseGlobalUC(mainWindow.SetCtrl.cip);
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
