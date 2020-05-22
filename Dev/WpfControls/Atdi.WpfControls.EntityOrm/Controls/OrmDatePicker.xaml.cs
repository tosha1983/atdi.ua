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

namespace Atdi.WpfControls.EntityOrm.Controls
{
    /// <summary>
    /// Interaction logic for OrmDatePicker.xaml
    /// </summary>
    public partial class OrmDatePicker : UserControl
    {
        double _captionWith = 150;
        string _caption = "";
        DateTime? _value = null;
        public OrmDatePicker()
        {
            InitializeComponent();
        }
        public double CaptionWith
        {
            get { return _captionWith; }
            set
            {
                this._captionWith = value;
                this.RedrawControl();
            }
        }
        public string Caption
        {
            get { return _caption; }
            set
            {
                this._caption = value;
                lblCaption.Content = this._caption;
            }
        }
        public static DependencyProperty SelectedDateProperty = DependencyProperty.Register("SelectedDate", typeof(DateTime?), typeof(OrmDatePicker),
            new FrameworkPropertyMetadata(default(DateTime?), new PropertyChangedCallback(OnPropertyChanged)));
        public DateTime? SelectedDate
        {
            get { return _value; }
            set
            {
                SetValue(SelectedDateProperty, value);
                this._value = value;
                dtMain.SelectedDate = this._value;
            }
        }
        private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var ctr = sender as OrmDatePicker;

            if (e.Property == SelectedDateProperty)
                ctr.SelectedDate = (DateTime?)e.NewValue;
        }
        private void RedrawControl()
        {
            lblCaption.Width = this._captionWith;
            dtMain.Margin = new Thickness() { Left = CaptionWith, Right = 5, Bottom = 5, Top = 5 };
            dtMain.Width = this.Width > this._captionWith + 10 ? this.Width - this._captionWith - 10 : 0;
            dtMain.Height = this.Height - 10;
        }
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RedrawControl();
        }
    }
}
