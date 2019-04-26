﻿using System;
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
    public partial class MeasStationsSignalizationForm : Form
    {
        private SDR.MeasurementResults _measResult;
        private MeasStationsSignalization[] _stationData;
        private ElementHost _wpfElementHost;
        public MeasStationsSignalizationForm(MeasStationsSignalization[] stationData, SDR.MeasurementResults measResult)
        {
            this._stationData = stationData;
            this._measResult = measResult;
            InitializeComponent();
        }

        private void MeasStationsSignalizationForm_Load(object sender, EventArgs e)
        {
            _wpfElementHost = new ElementHost();
            _wpfElementHost.Dock = DockStyle.Fill;
            this.Controls.Add(_wpfElementHost);


            var appFolder = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);

            var fileName = Path.Combine(appFolder, "XICSM_ICSControlClient\\Xaml\\MeasStationsSignalizationForm.xaml");
            using (var fileStream = new FileStream(fileName, FileMode.Open))
            {
                this._wpfElementHost.Child = (UIElement)XamlReader.Load(fileStream);
                (this._wpfElementHost.Child as System.Windows.Controls.UserControl).DataContext = new MeasStationsSignalizationFormViewModel(this._stationData, this._measResult);
            }

        }
    }
}
