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
        double _captionWith = 0;
        string _caption = "";
        OrmEnumBoxData _value = null;
        OrmEnumBoxData[] _source = null;
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
                SetValue(CaptionWithProperty, value);
                this._captionWith = value;
                this.RedrawControl();
            }
        }
        public string Caption
        {
            get { return _caption; }
            set
            {
                SetValue(CaptionProperty, value);
                this._caption = value;
                lblCaption.Content = this._caption;
            }
        }
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
        public OrmEnumBoxData SelectedValue
        {
            get { return _value; }
            set
            {
                SetValue(SelectedValueProperty, value);
                this._value = value;
                cmbMain.SelectedValue = this._value;
            }
        }
        public int SelectedValueId
        {
            get { return _value.Id; }
            set
            {
                SetValue(SelectedValueIdProperty, value);
                this.SelectedValue = this._source.Where(v => v.Id == value).First();
                cmbMain.SelectedValue = this._value;
            }
        }
        public OrmEnumBoxData[] Source
        {
            get { return _source; }
            set
            {
                SetValue(SourceProperty, value);
                this._source = value;
                cmbMain.ItemsSource = this._source;
            }
        }
        public static DependencyProperty CaptionWithProperty = DependencyProperty.Register("CaptionWith", typeof(double), typeof(OrmEnumBox), new FrameworkPropertyMetadata(default(double), new PropertyChangedCallback(OnPropertyChanged)));
        public static DependencyProperty CaptionProperty = DependencyProperty.Register("Caption", typeof(string), typeof(OrmEnumBox), new FrameworkPropertyMetadata("", new PropertyChangedCallback(OnPropertyChanged)));
        public static DependencyProperty EnabledProperty = DependencyProperty.Register("Enabled", typeof(bool), typeof(OrmEnumBox), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnPropertyChanged)));
        public static DependencyProperty SelectedValueProperty = DependencyProperty.Register("SelectedValue", typeof(OrmEnumBoxData), typeof(OrmEnumBox), new FrameworkPropertyMetadata(default(OrmEnumBoxData), new PropertyChangedCallback(OnPropertyChanged)));
        public static DependencyProperty SelectedValueIdProperty = DependencyProperty.Register("SelectedValueId", typeof(int), typeof(OrmEnumBox), new FrameworkPropertyMetadata(int.MinValue, new PropertyChangedCallback(OnPropertyChanged)));
        public static DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(OrmEnumBoxData[]), typeof(OrmEnumBox), new FrameworkPropertyMetadata(default(OrmEnumBoxData[]), new PropertyChangedCallback(OnPropertyChanged)));
        private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var ctr = sender as OrmEnumBox;

            if (e.Property == SourceProperty)
                ctr.Source = (OrmEnumBoxData[])e.NewValue;
            else if (e.Property == EnabledProperty)
                ctr.Enabled = (bool)e.NewValue;
            else if (e.Property == SelectedValueProperty)
                ctr.SelectedValue = (OrmEnumBoxData)e.NewValue;
            else if (e.Property == SelectedValueIdProperty)
                ctr.SelectedValueId = (int)e.NewValue;
            else if (e.Property == CaptionWithProperty)
                ctr.CaptionWith = (double)e.NewValue;
            else if (e.Property == CaptionProperty)
                ctr.Caption = (string)e.NewValue;
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
        private void cmbMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbMain.SelectedValue != null)
                SelectedValueId = (cmbMain.SelectedValue as OrmEnumBoxData).Id;
        }
    }
    public class OrmEnumBoxData
    {
        public int Id;

        public string Name;

        public string ViewName;

        public override string ToString()
        {
            return ViewName;
        }
    }
}
