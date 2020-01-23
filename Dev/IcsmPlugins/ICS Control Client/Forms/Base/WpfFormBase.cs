﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Reflection;
using CTR = System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Input;
using System.Configuration;
using System.Windows.Controls.Primitives;
using FM = XICSM.ICSControlClient.Forms;
using System.Windows.Controls;
using ADP = XICSM.ICSControlClient.Models.WcfDataApadters;

namespace XICSM.ICSControlClient.Forms
{
    public class WpfFormBase : Form
    {
        public readonly ElementHost _wpfElementHost;

        private Dictionary<CTR.DataGrid, DataGridFilters> _dataGridFilters;

        public WpfFormBase()
        {
            var appSettings = ConfigurationManager.AppSettings;
            string currentUICulture = appSettings["UICulture"];
            if (!string.IsNullOrEmpty(currentUICulture))
                System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo(currentUICulture);

            if (null == System.Windows.Application.Current)
            {
                new System.Windows.Application();
            }

            _wpfElementHost = new ElementHost
            {
                Dock = DockStyle.Fill
            };
            this.Controls.Add(_wpfElementHost);

            this.FormClosing += ThisFormClosing;
            this.Load += ThisFormLoad;
        }
        void ThisFormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (var spl in FindVisualChildren<CTR.GridSplitter>(_wpfElementHost.Child))
            {
                var key = this.Name + "/" + GetSplitterKey(spl);

                var grd = VisualTreeHelper.GetParent(spl);
                Int32.TryParse(spl.GetValue(CTR.Grid.RowProperty).ToString(), out int splRow);
                Int32.TryParse(spl.GetValue(CTR.Grid.ColumnProperty).ToString(), out int splCol);

                if (grd is CTR.Grid)
                {
                    var grid = grd as CTR.Grid;

                    if (splRow > 0)
                    {
                        SaveSetting(key + "/R1/Type", grid.RowDefinitions[splRow - 1].Height.GridUnitType.ToString());
                        SaveSetting(key + "/R1/Value", grid.RowDefinitions[splRow - 1].Height.Value.ToString());
                    }
                    if (splRow > 0 && grid.RowDefinitions.Count > splRow)
                    {
                        SaveSetting(key + "/R2/Type", grid.RowDefinitions[splRow + 1].Height.GridUnitType.ToString());
                        SaveSetting(key + "/R2/Value", grid.RowDefinitions[splRow + 1].Height.Value.ToString());
                    }
                    if (splCol > 0)
                    {
                        SaveSetting(key + "/C1/Type", grid.ColumnDefinitions[splCol - 1].Width.GridUnitType.ToString());
                        SaveSetting(key + "/C1/Value", grid.ColumnDefinitions[splCol - 1].Width.Value.ToString());
                    }
                    if (splCol > 0 && grid.ColumnDefinitions.Count > splCol)
                    {
                        SaveSetting(key + "/C2/Type", grid.ColumnDefinitions[splCol + 1].Width.GridUnitType.ToString());
                        SaveSetting(key + "/C2/Value", grid.ColumnDefinitions[splCol + 1].Width.Value.ToString());
                    }
                }
            }
            Properties.Settings.Default.Save();
        }
        void ThisFormLoad(object sender, EventArgs e)
        {
            this.InitializeSplitters();
            this.InitializeDataGrids();
        }
        private void DataGrid_Column_MenuFilterClick(object sender, EventArgs e)
        {
            var column = ((sender as CTR.MenuItem).Parent as CTR.ContextMenu).PlacementTarget as DataGridColumnHeader;

            var dep = column as DependencyObject;
            while (dep != null && !(dep is CTR.DataGrid))
                dep = VisualTreeHelper.GetParent(dep);

            var grd = dep as CTR.DataGrid;
            var pointToScreen = System.Windows.Forms.Control.MousePosition;
            var columnName = (((column.Column as CTR.DataGridTextColumn).Binding as System.Windows.Data.Binding).Path as PropertyPath).Path;

            if (GetColumnType(grd, columnName) == typeof(double)
                || GetColumnType(grd, columnName) == typeof(double?)
                || GetColumnType(grd, columnName) == typeof(decimal)
                || GetColumnType(grd, columnName) == typeof(decimal?)
                || GetColumnType(grd, columnName) == typeof(int)
                || GetColumnType(grd, columnName) == typeof(int?)
                || GetColumnType(grd, columnName) == typeof(long)
                || GetColumnType(grd, columnName) == typeof(long?)
                || GetColumnType(grd, columnName) == typeof(float)
                || GetColumnType(grd, columnName) == typeof(float?))
            {
                var dlgForm = new FM.gridFilterNumeric();
                dlgForm.Left = pointToScreen.X;
                dlgForm.Top = pointToScreen.Y;
                dlgForm.Text = column.Content.ToString();
                if (_dataGridFilters[grd].FiltersNumeric.ContainsKey(columnName))
                {
                    dlgForm.FilterFromValue = _dataGridFilters[grd].FiltersNumeric[columnName].FromValue;
                    dlgForm.FilterToValue = _dataGridFilters[grd].FiltersNumeric[columnName].ToValue;
                }
                dlgForm.ShowDialog();
                dlgForm.Dispose();

                if (dlgForm.IsPresOK)
                {
                    if (dlgForm.FilterFromValue.HasValue || dlgForm.FilterToValue.HasValue)
                    {
                        if (!_dataGridFilters[grd].FiltersNumeric.ContainsKey(columnName))
                            _dataGridFilters[grd].FiltersNumeric.Add(columnName, new DataGridFilterNumeric());

                        _dataGridFilters[grd].FiltersNumeric[columnName].FromValue = dlgForm.FilterFromValue;
                        _dataGridFilters[grd].FiltersNumeric[columnName].ToValue = dlgForm.FilterToValue;

                        column.Foreground = new SolidColorBrush(Colors.Green);
                        if (column.Content.ToString().IndexOf(" (Filter:") > 0)
                            column.Content = column.Content.ToString().Remove(column.Content.ToString().IndexOf(" (Filter:"));
                        column.Content = column.Content + " (Filter:";
                        if (dlgForm.FilterFromValue.HasValue)
                            column.Content = column.Content + " >= " + dlgForm.FilterFromValue.ToString() + ";";
                        if (dlgForm.FilterToValue.HasValue)
                            column.Content = column.Content + " <= " + dlgForm.FilterToValue.ToString() + ";";
                        column.Content = column.Content + ")";
                    }
                    else
                    {
                        if (_dataGridFilters[grd].FiltersNumeric.ContainsKey(columnName))
                            _dataGridFilters[grd].FiltersNumeric.Remove(columnName);

                        column.Foreground = new SolidColorBrush(Colors.Black);
                        if (column.Content.ToString().IndexOf(" (Filter:") > 0)
                            column.Content = column.Content.ToString().Remove(column.Content.ToString().IndexOf(" (Filter:"));
                    }
                }
            }
            else if ((GetColumnType(grd, columnName) == typeof(bool)) || (GetColumnType(grd, columnName) == typeof(bool?)))
            {
                var dlgForm = new FM.gridFilterBool();
                dlgForm.Left = pointToScreen.X;
                dlgForm.Top = pointToScreen.Y;
                dlgForm.Text = column.Content.ToString();
                if (_dataGridFilters[grd].FiltersBool.ContainsKey(columnName))
                {
                    dlgForm.FilterValue = _dataGridFilters[grd].FiltersBool[columnName].Value;
                }
                dlgForm.ShowDialog();
                dlgForm.Dispose();

                if (dlgForm.IsPresOK)
                {
                    if (dlgForm.FilterValue.HasValue)
                    {
                        if (!_dataGridFilters[grd].FiltersBool.ContainsKey(columnName))
                            _dataGridFilters[grd].FiltersBool.Add(columnName, new DataGridFilterBool());

                        _dataGridFilters[grd].FiltersBool[columnName].Value = dlgForm.FilterValue;

                        column.Foreground = new SolidColorBrush(Colors.Green);
                        if (column.Content.ToString().IndexOf(" (Filter:") > 0)
                            column.Content = column.Content.ToString().Remove(column.Content.ToString().IndexOf(" (Filter:"));
                        column.Content = column.Content + " (Filter: =" + dlgForm.FilterValue.Value.ToString() + ";)";
                    }
                    else
                    {
                        if (_dataGridFilters[grd].FiltersBool.ContainsKey(columnName))
                            _dataGridFilters[grd].FiltersBool.Remove(columnName);

                        column.Foreground = new SolidColorBrush(Colors.Black);
                        if (column.Content.ToString().IndexOf(" (Filter:") > 0)
                            column.Content = column.Content.ToString().Remove(column.Content.ToString().IndexOf(" (Filter:"));
                    }
                }
            }
            DataGridApplyFilters(grd);
        }
        private void DataGrid_MenuClick(object sender, EventArgs e)
        {
            //var menuItem = sender as CTR.MenuItem;
            //var contextMenu = menuItem.Parent as CTR.ContextMenu;
            //var grid = contextMenu.PlacementTarget as CTR.DataGrid;
            //SaveFileDialog sfd = new SaveFileDialog()
            //{
            //    Filter = "CSV (*.csv)|*.csv",
            //    FileName = "DataGrid_.csv"
            //};
            //if (sfd.ShowDialog() == DialogResult.OK)
            //{
            //    int recCount = grid.Items.Count;
            //    string[] output = new string[recCount + 1];

            //    var csvRow = new List<string>(); 
            //    int colCount = grid.Columns.Count;
            //    foreach (var column in grid.Columns)
            //    {
            //        csvRow.Add(column.Header.ToString());
            //    } 
            //    output[0] += string.Join(";", csvRow);

            //    int i = 0;

            //    foreach (DataRowView row in grid.ItemsSource)
            //    {





            //        //csvRow = new List<string>();
            //        //foreach (var item in row.Row.ItemArray)
            //        //{
            //        //    csvRow.Add(item.ToString());
            //        //}
            //        //output[i++ + 1] += string.Join(";", csvRow);
            //    }


            //    //    if (leveldBmkVm > 0)
            //    //        output[i + 1] += ms.Lon.ToString() + ";" + ms.Lat.ToString() + ";" + leveldBmkVm.ToString() + ";";
            //    //}
            //    System.IO.File.WriteAllLines(sfd.FileName, output, System.Text.Encoding.UTF8);
            //    System.Windows.MessageBox.Show("Your file was generated and its ready for use.");
            //}




        }
        void DataGridApplyFilters(CTR.DataGrid grid)
        {
            if (grid.Name == "GridEmittings")
            {
                (grid.ItemsSource as ADP.EmittingDataAdapter).ClearFilter();
                (grid.ItemsSource as ADP.EmittingDataAdapter).ApplyFilter(c =>
                {
                    foreach (var filter in _dataGridFilters[grid].FiltersNumeric)
                    {
                        if (filter.Key == "StartFrequency_MHz")
                        {
                            if (filter.Value.FromValue.HasValue && c.StartFrequency_MHz < filter.Value.FromValue)
                                return false;
                            if (filter.Value.ToValue.HasValue && c.StartFrequency_MHz > filter.Value.ToValue)
                                return false;
                        }
                        if (filter.Key == "StopFrequency_MHz")
                        {
                            if (filter.Value.FromValue.HasValue && c.StopFrequency_MHz < filter.Value.FromValue)
                                return false;
                            if (filter.Value.ToValue.HasValue && c.StopFrequency_MHz > filter.Value.ToValue)
                                return false;
                        }
                        if (filter.Key == "CurentPower_dBm")
                        {
                            if (filter.Value.FromValue.HasValue && c.CurentPower_dBm < filter.Value.FromValue)
                                return false;
                            if (filter.Value.ToValue.HasValue && c.CurentPower_dBm > filter.Value.ToValue)
                                return false;
                        }
                        if (filter.Key == "ReferenceLevel_dBm")
                        {
                            if (filter.Value.FromValue.HasValue && c.ReferenceLevel_dBm < filter.Value.FromValue)
                                return false;
                            if (filter.Value.ToValue.HasValue && c.ReferenceLevel_dBm > filter.Value.ToValue)
                                return false;
                        }
                        if (filter.Key == "MeanDeviationFromReference")
                        {
                            if (filter.Value.FromValue.HasValue && c.MeanDeviationFromReference < filter.Value.FromValue)
                                return false;
                            if (filter.Value.ToValue.HasValue && c.MeanDeviationFromReference > filter.Value.ToValue)
                                return false;
                        }
                        if (filter.Key == "TriggerDeviationFromReference")
                        {
                            if (filter.Value.FromValue.HasValue && c.TriggerDeviationFromReference < filter.Value.FromValue)
                                return false;
                            if (filter.Value.ToValue.HasValue && c.TriggerDeviationFromReference > filter.Value.ToValue)
                                return false;
                        }
                        if (filter.Key == "EmissionFreqMHz")
                        {
                            if (filter.Value.FromValue.HasValue && c.EmissionFreqMHz < filter.Value.FromValue)
                                return false;
                            if (filter.Value.ToValue.HasValue && c.EmissionFreqMHz > filter.Value.ToValue)
                                return false;
                        }
                        if (filter.Key == "Bandwidth_kHz")
                        {
                            if (filter.Value.FromValue.HasValue && c.Bandwidth_kHz < filter.Value.FromValue)
                                return false;
                            if (filter.Value.ToValue.HasValue && c.Bandwidth_kHz > filter.Value.ToValue)
                                return false;
                        }
                        if (filter.Key == "TraceCount")
                        {
                            if (filter.Value.FromValue.HasValue && c.TraceCount < filter.Value.FromValue)
                                return false;
                            if (filter.Value.ToValue.HasValue && c.TraceCount > filter.Value.ToValue)
                                return false;
                        }
                        if (filter.Key == "SignalLevel_dBm")
                        {
                            if (filter.Value.FromValue.HasValue && c.SignalLevel_dBm < filter.Value.FromValue)
                                return false;
                            if (filter.Value.ToValue.HasValue && c.SignalLevel_dBm > filter.Value.ToValue)
                                return false;
                        }
                        if (filter.Key == "RollOffFactor")
                        {
                            if (filter.Value.FromValue.HasValue && c.RollOffFactor < filter.Value.FromValue)
                                return false;
                            if (filter.Value.ToValue.HasValue && c.RollOffFactor > filter.Value.ToValue)
                                return false;
                        }
                        if (filter.Key == "StandardBW")
                        {
                            if (filter.Value.FromValue.HasValue && c.StandardBW < filter.Value.FromValue)
                                return false;
                            if (filter.Value.ToValue.HasValue && c.StandardBW > filter.Value.ToValue)
                                return false;
                        }
                        if (filter.Key == "SumHitCount")
                        {
                            if (filter.Value.FromValue.HasValue && c.SumHitCount < filter.Value.FromValue)
                                return false;
                            if (filter.Value.ToValue.HasValue && c.SumHitCount > filter.Value.ToValue)
                                return false;
                        }
                        if (filter.Key == "IcsmID")
                        {
                            if (filter.Value.FromValue.HasValue && c.IcsmID < filter.Value.FromValue)
                                return false;
                            if (filter.Value.ToValue.HasValue && c.IcsmID > filter.Value.ToValue)
                                return false;
                        }
                        if (filter.Key == "MeasResultId")
                        {
                            if (filter.Value.FromValue.HasValue && c.MeasResultId < filter.Value.FromValue)
                                return false;
                            if (filter.Value.ToValue.HasValue && c.MeasResultId > filter.Value.ToValue)
                                return false;
                        }
                    }
                    foreach (var filter in _dataGridFilters[grid].FiltersBool)
                    {
                        if (filter.Key == "CorrectnessEstimations" && filter.Value.Value.HasValue && c.CorrectnessEstimations != filter.Value.Value.Value)
                            return false;
                        if (filter.Key == "Contravention" && filter.Value.Value.HasValue && c.Contravention != filter.Value.Value.Value)
                            return false;
                    }
                    return true;
                });
            }
            if (grid.Name == "GridWorkTimes")
            {
                (grid.ItemsSource as ADP.EmittingWorkTimeDataAdapter).ClearFilter();
                (grid.ItemsSource as ADP.EmittingWorkTimeDataAdapter).ApplyFilter(c =>
                {
                    foreach (var filter in _dataGridFilters[grid].FiltersNumeric)
                    {
                        if (filter.Key == "HitCount")
                        {
                            if (filter.Value.FromValue.HasValue && c.HitCount < filter.Value.FromValue)
                                return false;
                            if (filter.Value.ToValue.HasValue && c.HitCount > filter.Value.ToValue)
                                return false;
                        }
                        if (filter.Key == "PersentAvailability")
                        {
                            if (filter.Value.FromValue.HasValue && c.PersentAvailability < filter.Value.FromValue)
                                return false;
                            if (filter.Value.ToValue.HasValue && c.PersentAvailability > filter.Value.ToValue)
                                return false;
                        }
                    }
                    return true;
                });
            }
        }
        Type GetColumnType(CTR.DataGrid grid, string columnName)
        {
            if (grid.Name == "GridEmittings")
            {
                if (columnName == "StartFrequency_MHz") return typeof(double);
                if (columnName == "StopFrequency_MHz") return typeof(double);
                if (columnName == "CurentPower_dBm") return typeof(double);
                if (columnName == "ReferenceLevel_dBm") return typeof(double);
                if (columnName == "MeanDeviationFromReference") return typeof(double);
                if (columnName == "TriggerDeviationFromReference") return typeof(double);
                if (columnName == "EmissionFreqMHz") return typeof(double);
                if (columnName == "Bandwidth_kHz") return typeof(double);
                if (columnName == "CorrectnessEstimations") return typeof(bool);
                if (columnName == "Contravention") return typeof(bool);
                if (columnName == "TraceCount") return typeof(int);
                if (columnName == "SignalLevel_dBm") return typeof(float);
                if (columnName == "RollOffFactor") return typeof(double);
                if (columnName == "StandardBW") return typeof(double);
                if (columnName == "SensorName") return typeof(string);
                if (columnName == "SumHitCount") return typeof(int);
                if (columnName == "IcsmID") return typeof(long);
                if (columnName == "IcsmTable") return typeof(string);
                if (columnName == "MeasResultId") return typeof(long);
            }
            if (grid.Name == "GridWorkTimes")
            {
                if (columnName == "HitCount") return typeof(int);
                if (columnName == "PersentAvailability") return typeof(float);
            }
            //if (grid.Name == "GridSensor")
            //{
            //    if (columnName == "LowerFreq") return typeof(double);
            //    if (columnName == "UpperFreq") return typeof(double);
            //}


            ////Type[] typeArguments = grd.ItemsSource.GetType();

            //int i = 0;
            //foreach (dynamic row in grd.Items)
            //{
            //    //string text = row.Row.ItemArray[i++].ToString();
            //    foreach (var prop in row.GetType().GetProperties())
            //    {
            //        string propName = prop.Name;
            //        object value = prop.GetValue(row, null);
            //    }

            //}
            return typeof(object);
        }
        void InitializeDataGrids()
        {
            _dataGridFilters = new Dictionary<CTR.DataGrid, DataGridFilters>();

            foreach (var grd in FindVisualChildren<CTR.DataGrid>(_wpfElementHost.Child))
            {
                //var item = new CTR.MenuItem() { Header = "Save to CSV", Name = "SaveToCSV" };
                //item.Click += DataGrid_MenuClick;
                //grd.ContextMenu = new CTR.ContextMenu();
                //grd.ContextMenu.Items.Add(item);

                //foreach (var col in grd.Columns)
                //{
                //    var item = new CTR.MenuItem() { Header = "Filter...", Name = "Filter" };
                //    item.Click += DataGrid_Column_MenuClick;
                //    (col.Header as TextBlock).ContextMenu = new CTR.ContextMenu();
                //    //grd.ContextMenu.Items.Add(item);
                //}
                //if (grd.Name == "GridSensor" || grd.Name == "GridEmittings" || grd.Name == "GridWorkTimes")
                if (grd.Name == "GridEmittings" || grd.Name == "GridWorkTimes")
                    {
                    List<DataGridColumnHeader> columnHeaders = GetVisualChildCollection<DataGridColumnHeader>(grd);
                    foreach (DataGridColumnHeader columnHeader in columnHeaders)
                    {
                        //var columnName = (((columnHeader.Column as CTR.DataGridTextColumn).Binding as System.Windows.Data.Binding).Path as PropertyPath).Path;
                        //if (GetColumnType(grd, columnName) != typeof(object))
                        //{
                            var item = new CTR.MenuItem() { Header = "Filter...", Name = "Filter" };
                            item.Click += DataGrid_Column_MenuFilterClick;
                            columnHeader.ContextMenu = new CTR.ContextMenu();
                            columnHeader.ContextMenu.Items.Add(item);
                        //}
                    }
                    _dataGridFilters.Add(grd, new DataGridFilters() { FiltersNumeric = new Dictionary<string, DataGridFilterNumeric>(), FiltersBool = new Dictionary<string, DataGridFilterBool>() });
                }
            }
        }
        void InitializeSplitters()
        {
            foreach (var spl in FindVisualChildren<CTR.GridSplitter>(_wpfElementHost.Child))
            {
                var key = this.Name + "/" + GetSplitterKey(spl);
                var grd = VisualTreeHelper.GetParent(spl);
                Int32.TryParse(spl.GetValue(CTR.Grid.RowProperty).ToString(), out int splRow);
                Int32.TryParse(spl.GetValue(CTR.Grid.ColumnProperty).ToString(), out int splCol);

                if (grd is CTR.Grid)
                {
                    var grid = grd as CTR.Grid;
                    if (splRow > 0)
                    {
                        var stringType = LoadSetting(key + "/R1/Type");
                        var stringVal = LoadSetting(key + "/R1/Value");
                        if (!string.IsNullOrEmpty(stringType) && !string.IsNullOrEmpty(stringVal))
                        {
                            Enum.TryParse<GridUnitType>(stringType, out GridUnitType type);
                            double.TryParse(stringVal, out double val);
                            grid.RowDefinitions[splRow - 1].Height = new GridLength(val, type);
                        }
                    }
                    if (splRow > 0 && grid.RowDefinitions.Count > splRow)
                    {
                        var stringType = LoadSetting(key + "/R2/Type");
                        var stringVal = LoadSetting(key + "/R2/Value");
                        if (!string.IsNullOrEmpty(stringType) && !string.IsNullOrEmpty(stringVal))
                        {
                            Enum.TryParse<GridUnitType>(stringType, out GridUnitType type);
                            double.TryParse(stringVal, out double val);
                            grid.RowDefinitions[splRow + 1].Height = new GridLength(val, type);
                        }
                    }
                    if (splCol > 0)
                    {
                        var stringType = LoadSetting(key + "/C1/Type");
                        var stringVal = LoadSetting(key + "/C1/Value");
                        if (!string.IsNullOrEmpty(stringType) && !string.IsNullOrEmpty(stringVal))
                        {
                            Enum.TryParse<GridUnitType>(stringType, out GridUnitType type);
                            double.TryParse(stringVal, out double val);
                            grid.ColumnDefinitions[splCol - 1].Width = new GridLength(val, type);
                        }
                    }
                    if (splCol > 0 && grid.ColumnDefinitions.Count > splCol)
                    {
                        var stringType = LoadSetting(key + "/C2/Type");
                        var stringVal = LoadSetting(key + "/C2/Value");
                        if (!string.IsNullOrEmpty(stringType) && !string.IsNullOrEmpty(stringVal))
                        {
                            Enum.TryParse<GridUnitType>(stringType, out GridUnitType type);
                            double.TryParse(stringVal, out double val);
                            grid.ColumnDefinitions[splCol + 1].Width = new GridLength(val, type);
                        }
                    }
                }
            }
        }
        void SaveSetting(string key, string val)
        {
            var property = new SettingsProperty(Properties.Settings.Default.Properties["baseSetting"]);
            property.Name = key;
            try
            {
                Properties.Settings.Default.Properties.Add(property);
            }
            catch (Exception)
            {
            }
            Properties.Settings.Default[key] = val;
        }
        string LoadSetting(string key)
        {
            var property = new SettingsProperty(Properties.Settings.Default.Properties["baseSetting"]);
            property.Name = key;
            try
            {
                Properties.Settings.Default.Properties.Add(property);
            }
            catch (Exception)
            {
            }
            return Properties.Settings.Default[key].ToString();
        }
        string GetSplitterKey(CTR.GridSplitter splitter)
        {
            return GetTreeKey(splitter) + "/GridSplitter/" + splitter.HorizontalAlignment.ToString() + "/" + splitter.VerticalAlignment.ToString() + "/" + splitter.GetValue(CTR.Grid.RowProperty) + "/" + splitter.GetValue(CTR.Grid.ColumnProperty);
        }
        static string GetTreeKey(DependencyObject depObj)
        {
            string key = "";
            if (depObj != null)
            {
                var obj = VisualTreeHelper.GetParent(depObj);
                if (obj != null)
                {
                    key = GetTreeKey(obj) + "/" + obj.DependencyObjectType.Name;
                }
            }
            return key;
        }
        public List<T> GetVisualChildCollection<T>(object parent) where T : Visual
        {
            List<T> visualCollection = new List<T>();
            GetVisualChildCollection(parent as DependencyObject, visualCollection);
            return visualCollection;
        }

        private void GetVisualChildCollection<T>(DependencyObject parent, List<T> visualCollection) where T : Visual
        {
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is T)
                {
                    visualCollection.Add(child as T);
                }
                else if (child != null)
                {
                    GetVisualChildCollection(child, visualCollection);
                }
            }
        }
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    }
}