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
using XICSM.ICSControlClient.Models.Views;

namespace XICSM.ICSControlClient.Forms
{
    public partial class SignalizationStationsEquipments : Form
    {
        private ElementHost _wpfElementHost;
        private SDR.MeasurementResults _measResult;
        private StationsEquipment[] _stationData;
        private EmittingViewModel[] _emittings;
        private double[] _selectedRangeX;
        private Stack<double[]> _zoomHistory = new Stack<double[]>();

        public SignalizationStationsEquipments(StationsEquipment[] stationData, EmittingViewModel[] emittings, SDR.MeasurementResults measResult, Stack<double[]> zoomHistory, double[] selectedRangeX)
        {
            this._stationData = stationData;
            this._emittings = emittings;
            this._measResult = measResult;
            this._zoomHistory = zoomHistory;
            this._selectedRangeX = selectedRangeX;
            InitializeComponent();
        }

        private void SignalizationStationsEquipments_Load(object sender, EventArgs e)
        {
            _wpfElementHost = new ElementHost();
            _wpfElementHost.Dock = DockStyle.Fill;
            this.Controls.Add(_wpfElementHost);


            var appFolder = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);

            var fileName = Path.Combine(appFolder, "XICSM_ICSControlClient\\Xaml\\SignalizationStationsEquipmentsForm.xaml");
            using (var fileStream = new FileStream(fileName, FileMode.Open))
            {
                this._wpfElementHost.Child = (UIElement)XamlReader.Load(fileStream);
                (this._wpfElementHost.Child as System.Windows.Controls.UserControl).DataContext = new SignalizationStationsEquipmentsViewModel(this._stationData, this._emittings, this._measResult, this._zoomHistory, this._selectedRangeX, this);
            }
        }
    }
}
