using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using XICSM.ICSControlClient.Models;
using System.IO;
using System.Windows.Markup;
using System.Windows;
using XICSM.ICSControlClient.ViewModels;
using SDR = Atdi.Contracts.WcfServices.Sdrn.Server;

namespace XICSM.ICSControlClient.Forms
{
    public partial class SignalizationSysInfoForm : Form
    {
        private ElementHost _wpfElementHost;
        long _measResultId;
        double _freq_MHz;
        public SignalizationSysInfoForm(long measResultId, double freq_MHz, string captionAdd)
        {
            this._measResultId = measResultId;
            this._freq_MHz = freq_MHz;
            InitializeComponent();
            this.Text = this.Text + captionAdd;
        }
        private void SignalizationSysInfoForm_Load(object sender, EventArgs e)
        {
            _wpfElementHost = new ElementHost();
            _wpfElementHost.Dock = DockStyle.Fill;
            this.Controls.Add(_wpfElementHost);
            var appFolder = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);

            var fileName = Path.Combine(appFolder, "XICSM_ICSControlClient\\Xaml\\SignalizationSysInfoForm.xaml");
            using (var fileStream = new FileStream(fileName, FileMode.Open))
            {
                this._wpfElementHost.Child = (UIElement)XamlReader.Load(fileStream);
                (this._wpfElementHost.Child as System.Windows.Controls.UserControl).DataContext = new SignalizationSysInfoViewModel(this._measResultId, this._freq_MHz);
            }
        }
    }
}
