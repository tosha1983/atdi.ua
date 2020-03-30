using System;
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
using System.Windows.Data;

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

                    if (splRow > 0 && grid.RowDefinitions.Count > splRow && grid.RowDefinitions[splRow - 1].Height.Value > 0 && grid.RowDefinitions[splRow + 1].Height.Value > 0)
                    {
                        SaveSetting(key + "/R1/Type", grid.RowDefinitions[splRow - 1].Height.GridUnitType.ToString());
                        SaveSetting(key + "/R1/Value", grid.RowDefinitions[splRow - 1].Height.Value.ToString());
                        SaveSetting(key + "/R2/Type", grid.RowDefinitions[splRow + 1].Height.GridUnitType.ToString());
                        SaveSetting(key + "/R2/Value", grid.RowDefinitions[splRow + 1].Height.Value.ToString());
                    }
                    if (splCol > 0 && grid.ColumnDefinitions.Count > splCol && grid.ColumnDefinitions[splCol - 1].Width.Value > 0 && grid.ColumnDefinitions[splCol + 1].Width.Value > 0)
                    {
                        SaveSetting(key + "/C1/Type", grid.ColumnDefinitions[splCol - 1].Width.GridUnitType.ToString());
                        SaveSetting(key + "/C1/Value", grid.ColumnDefinitions[splCol - 1].Width.Value.ToString());
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
                dlgForm.Text = column.Column.Header.ToString();
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

                        //column.Foreground = new SolidColorBrush(Colors.Green);
                        if (column.Column.Header.ToString().IndexOf($" ({Properties.Resources.MenuFilter}:") > 0)
                            column.Column.Header = column.Column.Header.ToString().Remove(column.Column.Header.ToString().IndexOf($" ({Properties.Resources.MenuFilter}:"));
                        column.Column.Header = column.Column.Header + $" ({Properties.Resources.MenuFilter}:";
                        if (dlgForm.FilterFromValue.HasValue)
                            column.Column.Header = column.Column.Header + " >= " + dlgForm.FilterFromValue.ToString() + ";";
                        if (dlgForm.FilterToValue.HasValue)
                            column.Column.Header = column.Column.Header + " <= " + dlgForm.FilterToValue.ToString() + ";";
                        column.Column.Header = column.Column.Header + ")";
                    }
                    else
                    {
                        if (_dataGridFilters[grd].FiltersNumeric.ContainsKey(columnName))
                            _dataGridFilters[grd].FiltersNumeric.Remove(columnName);

                        column.Foreground = new SolidColorBrush(Colors.Black);
                        if (column.Column.Header.ToString().IndexOf($" ({Properties.Resources.MenuFilter}:") > 0)
                            column.Column.Header = column.Column.Header.ToString().Remove(column.Column.Header.ToString().IndexOf($" ({Properties.Resources.MenuFilter}:"));
                    }
                }
            }
            else if ((GetColumnType(grd, columnName) == typeof(bool)) || (GetColumnType(grd, columnName) == typeof(bool?)))
            {
                var dlgForm = new FM.gridFilterBool();
                dlgForm.Left = pointToScreen.X;
                dlgForm.Top = pointToScreen.Y;
                dlgForm.Text = column.Column.Header.ToString();
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

                        //column.Foreground = new SolidColorBrush(Colors.Green);
                        if (column.Column.Header.ToString().IndexOf($" ({Properties.Resources.MenuFilter}:") > 0)
                            column.Column.Header = column.Column.Header.ToString().Remove(column.Column.Header.ToString().IndexOf($" ({Properties.Resources.MenuFilter}:"));
                        column.Column.Header = column.Column.Header + $" ({Properties.Resources.MenuFilter}: =" + dlgForm.FilterValue.Value.ToString() + ";)";
                    }
                    else
                    {
                        if (_dataGridFilters[grd].FiltersBool.ContainsKey(columnName))
                            _dataGridFilters[grd].FiltersBool.Remove(columnName);

                        column.Foreground = new SolidColorBrush(Colors.Black);
                        if (column.Column.Header.ToString().IndexOf($" ({Properties.Resources.MenuFilter}:") > 0)
                            column.Column.Header = column.Column.Header.ToString().Remove(column.Column.Header.ToString().IndexOf($" ({Properties.Resources.MenuFilter}:"));
                    }
                }
            }

            else if ((GetColumnType(grd, columnName) == typeof(string)))
            {
                var dlgForm = new FM.gridFilterString();
                dlgForm.Left = pointToScreen.X;
                dlgForm.Top = pointToScreen.Y;
                dlgForm.Text = column.Column.Header.ToString();
                if (_dataGridFilters[grd].FiltersString.ContainsKey(columnName))
                {
                    dlgForm.FilterValue = _dataGridFilters[grd].FiltersString[columnName].Value;
                }
                dlgForm.ShowDialog();
                dlgForm.Dispose();

                if (dlgForm.IsPresOK)
                {
                    if (!string.IsNullOrEmpty(dlgForm.FilterValue))
                    {
                        if (!_dataGridFilters[grd].FiltersString.ContainsKey(columnName))
                            _dataGridFilters[grd].FiltersString.Add(columnName, new DataGridFilterString());

                        _dataGridFilters[grd].FiltersString[columnName].Value = dlgForm.FilterValue;

                        //column.Foreground = new SolidColorBrush(Colors.Green);
                        if (column.Column.Header.ToString().IndexOf($" ({Properties.Resources.MenuFilter}:") > 0)
                            column.Column.Header = column.Column.Header.ToString().Remove(column.Column.Header.ToString().IndexOf($" ({Properties.Resources.MenuFilter}:"));
                        column.Column.Header = column.Column.Header + $" ({Properties.Resources.MenuFilter}: =" + dlgForm.FilterValue + ";)";
                    }
                    else
                    {
                        if (_dataGridFilters[grd].FiltersString.ContainsKey(columnName))
                            _dataGridFilters[grd].FiltersString.Remove(columnName);

                        column.Foreground = new SolidColorBrush(Colors.Black);
                        if (column.Column.Header.ToString().IndexOf($" ({Properties.Resources.MenuFilter}:") > 0)
                            column.Column.Header = column.Column.Header.ToString().Remove(column.Column.Header.ToString().IndexOf($" ({Properties.Resources.MenuFilter}:"));
                    }
                }
            }
            DataGridApplyFilters(grd);
        }
        private void DataGrid_MenuClick_ClearAllFilters(object sender, EventArgs e)
        {
            var menuItem = sender as CTR.MenuItem;
            var contextMenu = menuItem.Parent as CTR.ContextMenu;
            var grd = contextMenu.PlacementTarget as CTR.DataGrid;

            _dataGridFilters[grd].FiltersNumeric.Clear();
            _dataGridFilters[grd].FiltersBool.Clear();
            foreach (var column in grd.Columns)
            {
                if (column.Header.ToString().IndexOf($" ({Properties.Resources.MenuFilter}:") > 0)
                {
                    column.Header = column.Header.ToString().Remove(column.Header.ToString().IndexOf($" ({Properties.Resources.MenuFilter}:"));

                    //Style style = new Style(typeof(DataGridColumnHeader));
                    //style.Setters.Add(new Setter { Property = CTR.Control.ForegroundProperty, Value = new SolidColorBrush(Colors.Black)});
                    //style.TargetType = typeof(DataGridColumnHeader);
                    //column.HeaderStyle = style;
                }
            }

            DataGridApplyFilters(grd);
        }
        private void DataGrid_MenuClick_SaveToCSV(object sender, EventArgs e)
        {
            var menuItem = sender as CTR.MenuItem;
            var contextMenu = menuItem.Parent as CTR.ContextMenu;
            var grid = contextMenu.PlacementTarget as CTR.DataGrid;
            SaveFileDialog sfd = new SaveFileDialog()
            {
                Filter = "CSV (*.csv)|*.csv",
                FileName = $"DataGrid_{DateTime.Now.Year}_{DateTime.Now.Month}_{DateTime.Now.Day}_{DateTime.Now.Hour}_{DateTime.Now.Minute}_{DateTime.Now.Second}.csv"
            };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                int recCount = grid.Items.Count;
                string[] output = new string[recCount + 1];

                    
                var csvRow = new List<string>();
                var columnsBindName = new List<string>();
                    
                foreach (var column in grid.Columns)
                {
                    var columnName = (((column as CTR.DataGridTextColumn).Binding as System.Windows.Data.Binding).Path as PropertyPath).Path;
                    columnsBindName.Add(columnName);
                    csvRow.Add(column.Header.ToString());
                }
                output[0] += string.Join(";", csvRow);

                int i = 1;

                if (grid.SelectedItems.Count > 1)
                    foreach (dynamic row in grid.SelectedItems)
                    {
                        csvRow = new List<string>();

                        foreach (var columnName in columnsBindName)
                        {
                            var cellValue = row.GetType().GetProperty(columnName).GetValue(row, null);
                            csvRow.Add(cellValue == null ? "" : $"\"{cellValue.ToString()}\"");
                        }
                        output[i++] += string.Join(";", csvRow);
                    }
                else
                    foreach (dynamic row in grid.Items)
                    {
                        csvRow = new List<string>();

                        foreach (var columnName in columnsBindName)
                        {
                            var cellValue = row.GetType().GetProperty(columnName).GetValue(row, null);
                            csvRow.Add(cellValue == null ? ""  : $"\"{cellValue.ToString()}\"");
                        }
                        output[i++] += string.Join(";", csvRow);
                    }

                System.IO.File.WriteAllLines(sfd.FileName, output, System.Text.Encoding.UTF8);
                System.Windows.MessageBox.Show("Your file was generated and its ready for use.");
            }
            
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
                    foreach (var filter in _dataGridFilters[grid].FiltersString)
                    {
                        if (filter.Key == "SensorName" && !c.SensorName.Contains(filter.Value.Value))
                            return false;
                        if (filter.Key == "IcsmTable" && c.IcsmTable.Contains(filter.Value.Value))
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
            if (grid.Name == "GroupeEmissionProtocolDetail")
            {
                (grid.ItemsSource as ADP.DataSynchronizationProcessProtocolDataAdapter).ClearFilter();
                (grid.ItemsSource as ADP.DataSynchronizationProcessProtocolDataAdapter).ApplyFilter(c =>
                { 
                    foreach (var filter in _dataGridFilters[grid].FiltersNumeric)
                    {
                        if (filter.Key == "RadioControlMeasFreq_MHz")
                        {
                            if (filter.Value.FromValue.HasValue && c.RadioControlMeasFreq_MHz.HasValue && c.RadioControlMeasFreq_MHz < filter.Value.FromValue)
                                return false;
                            if (filter.Value.ToValue.HasValue && c.RadioControlMeasFreq_MHz.HasValue && c.RadioControlMeasFreq_MHz > filter.Value.ToValue)
                                return false;
                        }
                        if (filter.Key == "RadioControlBandWidth_KHz")
                        {
                            if (filter.Value.FromValue.HasValue && c.RadioControlBandWidth_KHz.HasValue && c.RadioControlBandWidth_KHz < filter.Value.FromValue)
                                return false;
                            if (filter.Value.ToValue.HasValue && c.RadioControlBandWidth_KHz.HasValue && c.RadioControlBandWidth_KHz > filter.Value.ToValue)
                                return false;
                        }
                        if (filter.Key == "FieldStrength")
                        {
                            if (filter.Value.FromValue.HasValue && c.FieldStrength.HasValue && c.FieldStrength < filter.Value.FromValue)
                                return false;
                            if (filter.Value.ToValue.HasValue && c.FieldStrength.HasValue && c.FieldStrength > filter.Value.ToValue)
                                return false;
                        }
                        if (filter.Key == "Level_dBm")
                        {
                            if (filter.Value.FromValue.HasValue && c.Level_dBm.HasValue && c.Level_dBm < filter.Value.FromValue)
                                return false;
                            if (filter.Value.ToValue.HasValue && c.Level_dBm.HasValue && c.Level_dBm > filter.Value.ToValue)
                                return false;
                        }
                        if (filter.Key == "RadioControlDeviationFreq_MHz")
                        {
                            if (filter.Value.FromValue.HasValue && c.RadioControlDeviationFreq_MHz.HasValue && c.RadioControlDeviationFreq_MHz < filter.Value.FromValue)
                                return false;
                            if (filter.Value.ToValue.HasValue && c.RadioControlDeviationFreq_MHz.HasValue && c.RadioControlDeviationFreq_MHz > filter.Value.ToValue)
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

            if (grid.Name == "GroupeEmissionProtocolDetail")
            {
                if (columnName == "RadioControlMeasFreq_MHz") return typeof(double?);
                if (columnName == "RadioControlBandWidth_KHz") return typeof(double?);
                if (columnName == "FieldStrength") return typeof(double?);
                if (columnName == "Level_dBm") return typeof(double?);
                if (columnName == "RadioControlDeviationFreq_MHz") return typeof(double);
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
                if (grd.ContextMenu == null)
                    grd.ContextMenu = new CTR.ContextMenu();
                if (grd.SelectionMode == DataGridSelectionMode.Extended)
                    grd.SelectedItems.Clear();

                var itemCSV = new CTR.MenuItem() { Header = Properties.Resources.SaveToCSV, Name = "SaveToCSV" };
                itemCSV.Click += DataGrid_MenuClick_SaveToCSV;
                grd.ContextMenu.Items.Add(itemCSV);

                //var itemClearFilter = new CTR.MenuItem() { Header = "Clear all filters", Name = "ClearAllFilters" };
                //itemClearFilter.Click += DataGrid_MenuClick_ClearAllFilters;
                //grd.ContextMenu.Items.Add(itemClearFilter);


                //if (grd.Name == "GridSensor" || grd.Name == "GridEmittings" || grd.Name == "GridWorkTimes")
                //if (grd.Name == "GridEmittings" || grd.Name == "GridWorkTimes" || grd.Name == "GroupeEmissionProtocolDetail")
                if (grd.Name == "GridEmittings" || grd.Name == "GridWorkTimes")
                    {
                        List<DataGridColumnHeader> columnHeaders = GetVisualChildCollection<DataGridColumnHeader>(grd);
                    foreach (DataGridColumnHeader columnHeader in columnHeaders)
                    {
                        //var columnName = (((columnHeader.Column as CTR.DataGridTextColumn).Binding as System.Windows.Data.Binding).Path as PropertyPath).Path;
                        //if (GetColumnType(grd, columnName) != typeof(object))
                        //{
                            var item = new CTR.MenuItem() { Header = $"{Properties.Resources.MenuFilter}...", Name = "Filter" };
                            item.Click += DataGrid_Column_MenuFilterClick;
                            columnHeader.ContextMenu = new CTR.ContextMenu();
                            columnHeader.ContextMenu.Items.Add(item);
                        //}
                    }
                    _dataGridFilters.Add(grd, new DataGridFilters()
                    {
                        FiltersNumeric = new Dictionary<string, DataGridFilterNumeric>(),
                        FiltersBool = new Dictionary<string, DataGridFilterBool>(),
                        FiltersString = new Dictionary<string, DataGridFilterString>()
                    });
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
                            if (val > 0)
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
                            if (val > 0)
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
                            if (val > 0)
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
                            if (val > 0)
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
