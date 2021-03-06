﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.IO;
using System.Windows.Markup;
using System.Windows;
using System.Windows.Media;
using XICSM.ICSControlClient.ViewModels;
using XICSM.ICSControlClient.Models;
using XICSM.ICSControlClient.WpfControls.Maps;
using UserControl = System.Windows.Controls.UserControl;

namespace XICSM.ICSControlClient.Forms
{

    public partial class MainForm : WpfFormBase
    {
        private readonly ControlClientViewModel _model;

        public MainForm() 
        {
            InitializeComponent();

            var appFolder = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);

            var fileName = Path.Combine(appFolder ?? throw new InvalidOperationException(), "XICSM_ICSControlClient\\Xaml\\ICSControlClient.xaml");
            using (var fileStream = new FileStream(fileName, FileMode.Open))
            {
                this._wpfElementHost.Child = (UIElement)XamlReader.Load(fileStream);
            }

            this._model = new ControlClientViewModel(DataStore.GetStore());
            ((UserControl)this._wpfElementHost.Child).DataContext = this._model;
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //((UserControl) this._wpfElementHost.Child).DataContext = null;
           
            this._model.Dispose();
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
            this.Controls.Remove(this._wpfElementHost);
            _wpfElementHost.Visible = false;
            _wpfElementHost.Child = null;
            _wpfElementHost.Dispose();
            _wpfElementHost.Parent = null;

        }
        //private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    DataGridRow row = sender as DataGridRow;
        //    if (row != null)
        //    {
        //        //var data = row.Item as ShortMeasTaskViewModel;
        //        //System.Windows.MessageBox.Show(data.Id.ToString());
        //    }
        //}
    }
}
