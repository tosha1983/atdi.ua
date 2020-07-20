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
    /// Interaction logic for OrmCheckBox.xaml
    /// </summary>
    public partial class OrmCheckBox : UserControl
    {
        double _captionWith = 0;
        string _caption = "";
        bool _enabled = true;
        bool _isRequired = false;
        OrmCheckBoxData _value = null;
        OrmCheckBoxData[] _source;
        public OrmCheckBox()
        {
            InitializeComponent();
            UpdateSource();
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
        public bool? SelectedValue
        {
            get { return _value.Value; }
            set
            {
                if (this._isRequired && value == null)
                    value = false;
                SetValue(SelectedValueProperty, value);
                this._value = this._source.Where(v => v.Value == value).First();
                cmbMain.SelectedValue = this._value;
            }
        }
        public bool IsRequired
        {
            get { return this._isRequired; }
            set
            {
                SetValue(IsRequiredProperty, value);
                this._isRequired = value;
                UpdateSource();
            }
        }
        public static DependencyProperty CaptionWithProperty = DependencyProperty.Register("CaptionWith", typeof(double), typeof(OrmCheckBox), new FrameworkPropertyMetadata(default(double), new PropertyChangedCallback(OnPropertyChanged)));
        public static DependencyProperty CaptionProperty = DependencyProperty.Register("Caption", typeof(string), typeof(OrmCheckBox), new FrameworkPropertyMetadata("", new PropertyChangedCallback(OnPropertyChanged)));
        public static DependencyProperty EnabledProperty = DependencyProperty.Register("Enabled", typeof(bool), typeof(OrmCheckBox), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnPropertyChanged)));
        public static DependencyProperty SelectedValueProperty = DependencyProperty.Register("SelectedValue", typeof(bool?), typeof(OrmCheckBox), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnPropertyChanged)));
        public static DependencyProperty IsRequiredProperty = DependencyProperty.Register("IsRequired", typeof(bool), typeof(OrmCheckBox), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnPropertyChanged)));
        private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var ctr = sender as OrmCheckBox;

            if (e.Property == IsRequiredProperty)
                ctr.IsRequired = (bool)e.NewValue;
            else if (e.Property == EnabledProperty)
                ctr.Enabled = (bool)e.NewValue;
            else if (e.Property == SelectedValueProperty)
                ctr.SelectedValue = (bool?)e.NewValue;
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
            if ((sender as ComboBox).SelectedValue != null)
                SelectedValue = (e.AddedItems[0] as OrmCheckBoxData).Value;
        }
        private void UpdateSource()
        {
            var data = new List<OrmCheckBoxData>();
            if (!this._isRequired)
                data.Add(new OrmCheckBoxData() { Value = null, ViewName = "" });
            data.Add(new OrmCheckBoxData() { Value = true, ViewName = Properties.Resources.Yes });
            data.Add(new OrmCheckBoxData() { Value = false, ViewName = Properties.Resources.No });
            this._source = data.ToArray();
            cmbMain.ItemsSource = this._source;
            //SelectedValue = this._isRequired ? (bool?)false : null;
        }
    }
    public class OrmCheckBoxData
    {
        public bool? Value;

        public string ViewName;

        public override string ToString()
        {
            return ViewName;
        }
    }
}
