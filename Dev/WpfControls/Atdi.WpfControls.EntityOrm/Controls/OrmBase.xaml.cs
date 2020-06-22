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
    /// Interaction logic for OrmBase.xaml
    /// </summary>
    public partial class OrmBase : UserControl
    {
        double _captionWith = 100;
        string _caption = "";
        public OrmBase()
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
        public static DependencyProperty CaptionWithProperty = DependencyProperty.Register("CaptionWith", typeof(double), typeof(OrmBase), new FrameworkPropertyMetadata(default(double), new PropertyChangedCallback(OnPropertyChanged)));
        public static DependencyProperty CaptionProperty = DependencyProperty.Register("Caption", typeof(string), typeof(OrmBase), new FrameworkPropertyMetadata("", new PropertyChangedCallback(OnPropertyChanged)));
        private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var ctr = sender as OrmBase;

            if (e.Property == CaptionWithProperty)
                ctr.CaptionWith = (double)e.NewValue;
            else if (e.Property == CaptionProperty)
                ctr.Caption = (string)e.NewValue;
        }
        private void RedrawControl()
        {
            lblCaption.Width = this._captionWith;
        }
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.RedrawControl();
        }
    }
}
