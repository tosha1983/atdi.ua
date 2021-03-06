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
using System.Windows.Media;
using XICSM.ICSControlClient.ViewModels;
using SDR = Atdi.Contracts.WcfServices.Sdrn.Server;
using XICSM.ICSControlClient.Models.Views;
using XICSM.ICSControlClient.WpfControls.Maps;
using UserControl = System.Windows.Controls.UserControl;

namespace XICSM.ICSControlClient.Forms
{
    public partial class SignalizationSensorsForm : WpfFormBase
    {
        private int _startType;
        private DateTime? _timeMeas;
        private EmittingViewModel[] _emittings;
        public SignalizationSensorsForm(int startType, EmittingViewModel[] emittings, DateTime? timeMeas)
        {
            this._startType = startType;
            this._emittings = emittings;
            this._timeMeas = timeMeas;
            InitializeComponent();

            var appFolder = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);

            var fileName = Path.Combine(appFolder, "XICSM_ICSControlClient\\Xaml\\SignalizationSensorsForm.xaml");
            using (var fileStream = new FileStream(fileName, FileMode.Open))
            {
                this._wpfElementHost.Child = (UIElement)XamlReader.Load(fileStream);
                ((UserControl)this._wpfElementHost.Child).DataContext = new SignalizationSensorsViewModel(_startType, this, _emittings, _timeMeas);
            }
        }

        private void SignalizationSensorsForm_FormClosed(object sender, FormClosedEventArgs e)
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
        }
    }
}
