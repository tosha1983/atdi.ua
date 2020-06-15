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
        bool _enabled = true;
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
        public static DependencyProperty EnabledProperty = DependencyProperty.Register("Enabled", typeof(bool), typeof(OrmDatePicker),
            new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnPropertyChanged)));
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                SetValue(EnabledProperty, value);
                this._enabled = value;
                dtMain.IsEnabled = this._enabled;
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
            else if (e.Property == EnabledProperty)
                ctr.Enabled = (bool)e.NewValue;
        }
        private void RedrawControl()
        {
            lblCaption.Width = this._captionWith;
            dtMain.Margin = new Thickness() { Left = CaptionWith, Right = 2, Bottom = 2, Top = 2 };
            dtMain.Width = this.Width > this._captionWith + 4 ? this.Width - this._captionWith - 4 : 0;
            dtMain.Height = this.Height - 4;
        }
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.RedrawControl();
        }
        private void dtMain_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedDate = dtMain.SelectedDate;
        }
    }
}
