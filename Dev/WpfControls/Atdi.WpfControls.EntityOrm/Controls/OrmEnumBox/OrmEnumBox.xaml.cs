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
    /// Interaction logic for OrmEnumBox.xaml
    /// </summary>
    public partial class OrmEnumBox : UserControl
    {
        double _captionWith = 150;
        string _caption = "";
        OrmEnumBoxData[] _value = null;
        bool _enabled = true;
        public OrmEnumBox()
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
        public static DependencyProperty EnabledProperty = DependencyProperty.Register("Enabled", typeof(bool), typeof(OrmEnumBox),
            new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnPropertyChanged)));
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                SetValue(EnabledProperty, value);
                this._enabled = value;
                cmbMain.IsEnabled = this._enabled;
            }
        }

        public static DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(OrmEnumBoxData[]), typeof(OrmEnumBox),
            new FrameworkPropertyMetadata(default(OrmEnumBoxData[]), new PropertyChangedCallback(OnPropertyChanged)));
        public OrmEnumBoxData[] Source
        {
            get { return _value; }
            set
            {
                SetValue(SourceProperty, value);
                this._value = value;
                cmbMain.ItemsSource = this._value;//.Select(c => c.ViewName).ToArray();
                //cmbMain.DisplayMemberPath = "Name";
            }
        }
        private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var ctr = sender as OrmEnumBox;

            if (e.Property == SourceProperty)
                ctr.Source = (OrmEnumBoxData[])e.NewValue;
            else if (e.Property == EnabledProperty)
                ctr.Enabled = (bool)e.NewValue;
        }
        private void RedrawControl()
        {
            lblCaption.Width = this._captionWith;
            cmbMain.Margin = new Thickness() { Left = CaptionWith, Right = 2, Bottom = 2, Top = 2 };
            cmbMain.Width = this.Width > this._captionWith + 4 ? this.Width - this._captionWith - 4 : 0;
            cmbMain.Height = this.Height - 4;
        }
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RedrawControl();
        }
    }
}
