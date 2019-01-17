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
    /// Логика взаимодействия для Confirm.xaml
    /// </summary>
    public partial class Confirm : UserControl, INotifyPropertyChanged
    {
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
                });
            }
        }
        private string _Title = "";
        public string Title
        {
            get { return _Title; }
            set
            {
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    _Title = value;
                    OnPropertyChanged("Title");
                });
            }
        }

        public Confirm(string title, string message)
        {
            InitializeComponent();
            this.DataContext = this;
            MessageText = message;
            Title = title;
        }
        
        private void Cancel_btn(object sender, RoutedEventArgs e)
        {
            OnPropertyChanged("Cancel");
            RemoveThisFromParent();
        }

        private void Confirm_btn(object sender, RoutedEventArgs e)
        {
            OnPropertyChanged("Confirm");
            RemoveThisFromParent();
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
