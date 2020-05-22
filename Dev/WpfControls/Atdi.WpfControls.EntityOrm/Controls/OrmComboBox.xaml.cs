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
    /// Interaction logic for OrmComboBox.xaml
    /// </summary>
    public partial class OrmComboBox : UserControl
    {
        double _captionWith = 150;
        string _caption = "";
        string[] _value = null;
        public OrmComboBox()
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
        public string[] Source
        {
            get { return _value; }
            set
            {
                this._value = value;
                cmbMain.ItemsSource = this._value;
            }
        }
        //public string
        private void RedrawControl()
        {
            lblCaption.Width = this._captionWith;
            cmbMain.Margin = new Thickness() { Left = CaptionWith, Right = 5, Bottom = 5, Top = 5 };
            cmbMain.Width = this.Width > this._captionWith + 10 ? this.Width - this._captionWith - 10 : 0;
            cmbMain.Height = this.Height - 10;
        }
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RedrawControl();
        }
    }
}
