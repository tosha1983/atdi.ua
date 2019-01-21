using System;
using System.Collections.Generic;
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

namespace ControlU.Controls.Messages
{
    /// <summary>
    /// Логика взаимодействия для Message.xaml
    /// </summary>
    public partial class Message : UserControl, INotifyPropertyChanged
    {
        System.Timers.Timer Tmr;
        private string _MessageText = "";
        public string MessageText
        {
            get { return _MessageText; }
            set
            {
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    _MessageText = value;
                    OnPropertyChanged("MessageText");
                    Tmr = new System.Timers.Timer(10000);
                    Tmr.AutoReset = false;
                    Tmr.Elapsed += MesAutoClose;
                    Tmr.Enabled = true;
                });
            }
        }
        public Message(string text)
        {
            InitializeComponent();
            this.DataContext = this;
            MessageText = text;
        }
        public void MesAutoClose(object sender, System.Timers.ElapsedEventArgs e)
        {
            RemoveThisFromParent();
        }
        private void CloseMessageBorder_ButtonClick(object sender, RoutedEventArgs e)
        {
            RemoveThisFromParent();
        }
        private void MessageBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Tmr.Elapsed -= MesAutoClose; Tmr.Stop();
        }
        private void RemoveThisFromParent()
        {
            //неожиданно но хай так
            App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
            {
                if (this.Parent is Grid)
                {
                    Grid parent = (Grid)this.Parent;
                    parent.Children.Remove(this);
                }
                else if (this.Parent is StackPanel)
                {
                    StackPanel parent = (StackPanel)this.Parent;
                    parent.Children.Remove(this);
                }
            });
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
