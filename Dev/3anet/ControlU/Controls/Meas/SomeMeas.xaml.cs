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

namespace ControlU.Controls.Meas
{
    /// <summary>
    /// Логика взаимодействия для AnyMeas.xaml
    /// </summary>
    public partial class SomeMeas : UserControl, INotifyPropertyChanged
    {
        public ObservableCollection<Equipment.DataSomeMeas> DataSomeMeas
        {
            get { return MainWindow.amd.DataSomeMeas; }
            set { MainWindow.amd.DataSomeMeas = value; OnPropertyChanged("DataAnyMeas"); }
        }

        public Equipment.DataSomeMeas SelectedDataSomeMeas
        {
            get { return _SelectedDataSomeMeas; }
            set { _SelectedDataSomeMeas = value; OnPropertyChanged("SelectedDataSomeMeas"); }
        }
        private Equipment.DataSomeMeas _SelectedDataSomeMeas = new Equipment.DataSomeMeas();

        public bool DataSomeMeasState
        {
            get { return _DataSomeMeasState; }
            set { _DataSomeMeasState = value; OnPropertyChanged("DataSomeMeasState"); }
        }
        private bool _DataSomeMeasState = false;

        AddElementToSomeMeas Add;

        ///////////////////////////////////////////////////////////
        public SomeMeas()
        {
            InitializeComponent();
            Data.DataContext = this;
        }
        private void DataSomeMeasState_Click(object sender, RoutedEventArgs e)
        {
            DataSomeMeasState = !DataSomeMeasState;
            bool sh = false;
            for (int i = 0; i < DataSomeMeas.Count(); i++)
            {
                if (DataSomeMeas[i].DeviceType == 3)
                {
                    sh = true;
                }
            }
            if (sh == true) MainWindow.SHReceiver.IsSomeMeas = DataSomeMeasState;
        }
        private void SpectrumFromReceiver_Click(object sender, RoutedEventArgs e)
        {
            //DrawSpec_SomeMeas.SpectrumFromDevice = !DrawSpec_SomeMeas.SpectrumFromDevice;
        }
        #region data
        private void DataSomeMeas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedDataSomeMeas = (Equipment.DataSomeMeas)((ListBox)sender).SelectedItem;
            //DrawSpec_SomeMeas.DataSomeMeas = SelectedDataSomeMeas;
            //DrawSpec_SomeMeas.SpectrumFromDevice = false;
            //DrawSpec_SomeMeas.ShowSomeMeas = true;
        }

        private void AddData_Click(object sender, RoutedEventArgs e)
        {
            Equipment.DataSomeMeas newdata = new Equipment.DataSomeMeas();
            Add = new AddElementToSomeMeas();
            Add.Data = newdata;
            ((SplashWindow)App.Current.MainWindow).m_mainWindow.ShowGlobalUC(Add, true, false);
        }
        private void DeleteData_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedDataSomeMeas != null)
                DataSomeMeas.Remove(SelectedDataSomeMeas);
        }
        void DataSomeMeas_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            ListBoxItem lbi = (ListBoxItem)sender;
            SelectedDataSomeMeas = (Equipment.DataSomeMeas)lbi.Content;
            Add = new AddElementToSomeMeas();
            Add.Data = SelectedDataSomeMeas;
            Add.Сhange = true;
            ((SplashWindow)App.Current.MainWindow).m_mainWindow.ShowGlobalUC(Add, true, false);
        }

        public void AddDataOk(AddElementToSomeMeas control)
        {
            if (control.Сhange)
            {
                for (int i = 0; i < DataSomeMeas.Count(); i++)
                {
                    if (DataSomeMeas[i] == control.Data)
                    {
                        DataSomeMeas[i] = control.DataClone;
                    }
                }
            }
            else { DataSomeMeas.Add(control.DataClone); }
            ((SplashWindow)App.Current.MainWindow).m_mainWindow.CloseGlobalUC(control);
        }
        public void AddDataCancel(AddElementToSomeMeas control)
        {
            ((SplashWindow)App.Current.MainWindow).m_mainWindow.CloseGlobalUC(control);
        }
        public void AddDataApply(AddElementToSomeMeas control)
        {
            if (control.Сhange)
            {
                for (int i = 0; i < DataSomeMeas.Count(); i++)
                {
                    if (DataSomeMeas[i] == control.Data)
                    {
                        DataSomeMeas[i] = control.DataClone;
                    }
                }
            }
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
