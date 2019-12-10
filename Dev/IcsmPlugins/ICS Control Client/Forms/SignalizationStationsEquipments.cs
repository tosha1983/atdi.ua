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
using System.Windows.Media;
using XICSM.ICSControlClient.ViewModels;
using SDR = Atdi.Contracts.WcfServices.Sdrn.Server;
using XICSM.ICSControlClient.Models.Views;
using XICSM.ICSControlClient.WpfControls.Maps;
using UserControl = System.Windows.Controls.UserControl;

namespace XICSM.ICSControlClient.Forms
{
    public partial class SignalizationStationsEquipments : WpfFormBase
    {
        private SDR.MeasurementResults _measResult;
        private StationsEquipment[] _stationData;
        private EmittingViewModel[] _emittings;
        private double[] _selectedRangeX;
        private Stack<double[]> _zoomHistory = new Stack<double[]>();
        private SignalizationStationsEquipmentsViewModel _model;

        public SignalizationStationsEquipments(StationsEquipment[] stationData, EmittingViewModel[] emittings, SDR.MeasurementResults measResult, Stack<double[]> zoomHistory, double[] selectedRangeX)
        {
            this._stationData = stationData;
            this._emittings = emittings;
            this._measResult = measResult;
            this._zoomHistory = zoomHistory;
            this._selectedRangeX = selectedRangeX;
            InitializeComponent();

            var appFolder = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);

            var fileName = Path.Combine(appFolder, "XICSM_ICSControlClient\\Xaml\\SignalizationStationsEquipmentsForm.xaml");
            using (var fileStream = new FileStream(fileName, FileMode.Open))
            {
                this._wpfElementHost.Child = (UIElement)XamlReader.Load(fileStream);
                _model = new SignalizationStationsEquipmentsViewModel(this._stationData, this._emittings, this._measResult, this._zoomHistory, this._selectedRangeX, this);
                ((UserControl)this._wpfElementHost.Child).DataContext = _model;
            }
        }

        private void SignalizationStationsEquipments_FormClosed(object sender, FormClosedEventArgs e)
        {
            var maps = FindVisualChildren<Map>(_wpfElementHost.Child);
            foreach (var map in maps)
            {
                map.Dispose();
            }
            if (_wpfElementHost.Child is FrameworkElement fe)
            {
                // Memory leak workaround: elementHost.Child.SizeChanged -= elementHost.childFrameworkElement_SizeChanged;
                var handler = (SizeChangedEventHandler)Delegate.CreateDelegate(typeof(SizeChangedEventHandler), _wpfElementHost, "childFrameworkElement_SizeChanged");
                fe.SizeChanged -= handler;
            }
            _wpfElementHost.Visible = false;
            _wpfElementHost.Child = null;
            _wpfElementHost.Dispose();
            _wpfElementHost.Parent = null;

            _model.Dispose();
        }
    }
}
