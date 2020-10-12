using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// Логика взаимодействия для AddUser.xaml
    /// </summary>
    public partial class AddUser : UserControl, INotifyPropertyChanged
    {
        string AppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        private Settings.UserApps_Set _newuser = new Settings.UserApps_Set();
        public Settings.UserApps_Set newuser
        {
            get { return _newuser; }
            set { _newuser = value; OnPropertyChanged("newuser"); }
        }
        public AddUser()
        {
            InitializeComponent();
            addcntr.DataContext = newuser;

        }
        private void CreateUser_click(object sender, RoutedEventArgs e)
        {
            if (App.Sett.UsersApps_Settings.Users == null)
            { App.Sett.UsersApps_Settings.Users = new ObservableCollection<Settings.UserApps_Set>(); }

            int temp = 0;
            for (int i = 0; i < App.Sett.UsersApps_Settings.Users.Count; i++)
            {
                App.Sett.UsersApps_Settings.Users[i].ID = i;
                temp = i;
            }

            newuser.ID = temp + 1;
            newuser.SYS_ID = 1;
            App.Sett.UsersApps_Settings.Users.Add(newuser);
            ((SplashWindow)App.Current.MainWindow).m_mainWindow.CloseGlobalUC(this);
        }
        private void CloseUser_click(object sender, RoutedEventArgs e)
        {
            ((SplashWindow)App.Current.MainWindow).m_mainWindow.CloseGlobalUC(this);
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
