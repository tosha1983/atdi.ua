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
using XICSM.ICSControlClient.WpfControls.Maps;
using SDR = Atdi.Contracts.WcfServices.Sdrn.Server;
using XICSM.ICSControlClient.Models.Views;

namespace XICSM.ICSControlClient.Forms
{
    public partial class MeasStationsSignalizationForm : WpfFormBase
    {
        private SDR.MeasurementResults _measResult;
        private MeasStationsSignalization[] _stationData;
        private bool _buttonAssociatedVisible;
        private EmittingViewModel _emitting;
        private int _startType;
        private SDR.Emitting[] _inputEmittings;
        private MeasStationsSignalizationFormViewModel _model;
        public MeasStationsSignalizationForm(MeasStationsSignalization[] stationData, SDR.MeasurementResults measResult, bool buttonAssociatedVisible, EmittingViewModel emitting, string captionAdd, int startType, SDR.Emitting[] inputEmittings)
        {
            this._stationData = stationData;
            this._measResult = measResult;
            this._buttonAssociatedVisible = buttonAssociatedVisible;
            this._emitting = emitting;
            this._startType = startType;
            this._inputEmittings = inputEmittings;
            InitializeComponent();
            this.Text = this.Text + captionAdd;

            var appFolder = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);

            var fileName = Path.Combine(appFolder, "XICSM_ICSControlClient\\Xaml\\MeasStationsSignalizationForm.xaml");
            using (var fileStream = new FileStream(fileName, FileMode.Open))
            {
                this._wpfElementHost.Child = (UIElement)XamlReader.Load(fileStream);
                this._model = new MeasStationsSignalizationFormViewModel(this._stationData, this._measResult, this._buttonAssociatedVisible, this._emitting, this._startType, this._inputEmittings) { _form = this };
                (this._wpfElementHost.Child as System.Windows.Controls.UserControl).DataContext = this._model;
            };
        }

        private void MeasStationsSignalizationForm_FormClosed(object sender, FormClosedEventArgs e)
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
            _model._form = null;
        }
    }
}
