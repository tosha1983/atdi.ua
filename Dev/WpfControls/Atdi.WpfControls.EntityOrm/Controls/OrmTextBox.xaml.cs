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
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class OrmTextBox : UserControl
    {
        double _captionWith = 150;
        string _caption = "";
        string _text = "";
        bool _enabled = true;
        public OrmTextBox()
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
        public static DependencyProperty EnabledProperty = DependencyProperty.Register("Enabled", typeof(bool), typeof(OrmTextBox),
            new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnPropertyChanged)));
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                SetValue(EnabledProperty, value);
                this._enabled = value;
                txtMain.IsEnabled = this._enabled;
            }
        }

        public static DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(OrmTextBox),
            new FrameworkPropertyMetadata(default(string), new PropertyChangedCallback(OnPropertyChanged)));
        public string Text
        {
            get { return _text; }
            set
            {
                SetValue(TextProperty, value);
                this._text = value;
                txtMain.Text = this._text;
            }
        }
        private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var ctr = sender as OrmTextBox;

            if (e.Property == TextProperty)
                ctr.Text = (string)e.NewValue;
            if (e.Property == EnabledProperty)
                ctr.IsEnabled = (bool)e.NewValue;
        }
        private void RedrawControl()
        {
            lblCaption.Width = this._captionWith;
            txtMain.Margin = new Thickness() { Left = CaptionWith, Right = 2, Bottom = 2, Top = 2 };
            txtMain.Width = this.Width > this._captionWith + 4 ? this.Width - this._captionWith - 4 : 0;
            txtMain.Height = this.Height - 4;
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.RedrawControl();
        }

        private void TxtMain_TextChanged(object sender, TextChangedEventArgs e)
        {
            Text = txtMain.Text;
        }
    }
}
