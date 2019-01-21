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
using System.ComponentModel;

namespace ControlU.Controls
{
    /// <summary>
    /// Логика взаимодействия для DialogControl.xaml
    /// </summary>
    public partial class InputDialogControl : UserControl, INotifyPropertyChanged
    {
        public bool Value
        {
            get { return (bool)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); OnPropertyChanged("Value"); }
        }
        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(bool), typeof(InputDialogControl), new PropertyMetadata(null));

        public InputDialogControl(string TitleText, string caption)
        {
            InitializeComponent();
            myMessageBox = TitleText;
            ResponseText = caption;
        }
        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public string myMessageBox
        {
            get { return _MessageBox.Text; }
            set { _MessageBox.Text = value; }
        }
        public string ResponseText
        {
            get { return ResponseTextBox.Text; }
            set { ResponseTextBox.Text = value; }
        }
        private void OKButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Value = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Value = false;
        }
    }
}
