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
using XICSM.ICSControlClient.WpfControls;

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
        void DataGrid_Column_MenuFilterClick(object sender, EventArgs e)
        {
            var column = ((sender as CTR.MenuItem).Parent as CTR.ContextMenu).PlacementTarget as DataGridColumnHeader;
            var columnName = (((column.Column as CTR.DataGridTextColumn).Binding as System.Windows.Data.Binding).Path as PropertyPath).Path;

            var dep = column as DependencyObject;
            while (dep != null && !(dep is CTR.DataGrid))
                dep = VisualTreeHelper.GetParent(dep);

            var grd = dep as CTR.DataGrid;
            var columnType = GetColumnType(grd, columnName);

            if (columnType == typeof(double) || columnType == typeof(double?)
                || columnType == typeof(decimal) || columnType == typeof(decimal?)
                || columnType == typeof(int) || columnType == typeof(int?)
                || columnType == typeof(long) || columnType == typeof(long?)
                || columnType == typeof(float) || columnType == typeof(float?))
            {
                PrepareFilterForm(new FM.gridFilterNumeric(), grd, column);
            }
            else if ((columnType == typeof(bool)) || (columnType == typeof(bool?)))
            {
                PrepareFilterForm(new FM.gridFilterBool(), grd, column);
            }
            else if ((columnType == typeof(string)))
            {
                PrepareFilterForm(new FM.gridFilterString(), grd, column);
            }
            else if (columnType == typeof(DateTime) || columnType == typeof(DateTime?))
            {
                PrepareFilterForm(new FM.gridFilterDate(), grd, column);
            }
            DataGridApplyFilters(grd);
        }
        void DataGrid_MenuClick_ClearAllFilters(object sender, EventArgs e)
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
        void DataGrid_MenuClick_SaveToCSV(object sender, EventArgs e)
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
        public void ApplyAllDataGridsFilters()
        {
            foreach (var item in _dataGridFilters.Keys)
                DataGridApplyFilters(item);
        }
        public void ApplyDataGridsFiltersByGridName(string name)
        {
            foreach (var item in _dataGridFilters.Keys)
                if (item.Name == name)
                    DataGridApplyFilters(item);
        }
        void DataGridApplyFilters(CTR.DataGrid grid)
        {
            DataGridApplyFiltersTemp(grid);
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

                InitializeDataGridsTemp(grd);
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
        Type GetColumnType(CTR.DataGrid grid, string columnName)
        {
            return GetColumnTypeTemp(grid, columnName);
        }

        void PrepareFilterForm<T>(T form, CTR.DataGrid grd, DataGridColumnHeader column) where T : gridFilterBase
        {
            var columnName = (((column.Column as CTR.DataGridTextColumn).Binding as System.Windows.Data.Binding).Path as PropertyPath).Path;
            var columnHeader = column.Column.Header.ToString();
            var pointToScreen = System.Windows.Forms.Control.MousePosition;

            form.Left = pointToScreen.X;
            form.Top = pointToScreen.Y;
            form.Text = columnHeader;

            if (form is FM.gridFilterNumeric)
            {
                var frm = form as FM.gridFilterNumeric;
                if (_dataGridFilters[grd].FiltersNumeric.ContainsKey(columnName))
                {
                    frm.FilterFromValue = _dataGridFilters[grd].FiltersNumeric[columnName].FromValue;
                    frm.FilterToValue = _dataGridFilters[grd].FiltersNumeric[columnName].ToValue;
                }
            }
            else if (form is FM.gridFilterBool)
            {
                var frm = form as FM.gridFilterBool;
                if (_dataGridFilters[grd].FiltersBool.ContainsKey(columnName))
                {
                    frm.FilterValue = _dataGridFilters[grd].FiltersBool[columnName].Value;
                }
            }
            else if (form is FM.gridFilterString)
            {
                var frm = form as FM.gridFilterString;
                if (_dataGridFilters[grd].FiltersString.ContainsKey(columnName))
                {
                    frm.FilterValue = _dataGridFilters[grd].FiltersString[columnName].Value;
                }
            }
            if (form is FM.gridFilterDate)
            {
                var frm = form as FM.gridFilterDate;
                if (_dataGridFilters[grd].FiltersDate.ContainsKey(columnName))
                {
                    frm.FilterFromValue = _dataGridFilters[grd].FiltersDate[columnName].FromValue;
                    frm.FilterToValue = _dataGridFilters[grd].FiltersDate[columnName].ToValue;
                }
            }

            form.ShowDialog();
            form.Dispose();

            if (form.IsPresOK)
            {
                if (columnHeader.IndexOf($" ({Properties.Resources.MenuFilter}:") > 0)
                {
                    column.Column.Header = columnHeader.Remove(columnHeader.IndexOf($" ({Properties.Resources.MenuFilter}:"));
                    column.Foreground = new SolidColorBrush(Colors.Black);
                }

                string filterText = "";
                if (form is FM.gridFilterNumeric)
                {
                    var frm = form as FM.gridFilterNumeric;
                    if (frm.FilterFromValue.HasValue || frm.FilterToValue.HasValue)
                    {
                        if (!_dataGridFilters[grd].FiltersNumeric.ContainsKey(columnName))
                            _dataGridFilters[grd].FiltersNumeric.Add(columnName, new DataGridFilterNumeric() { FromValue = frm.FilterFromValue, ToValue = frm.FilterToValue });
                        else
                        {
                            _dataGridFilters[grd].FiltersNumeric[columnName].FromValue = frm.FilterFromValue;
                            _dataGridFilters[grd].FiltersNumeric[columnName].ToValue = frm.FilterToValue;
                        }

                        if (frm.FilterFromValue.HasValue)
                            filterText = filterText + " >= " + frm.FilterFromValue.ToString() + ";";
                        if (frm.FilterToValue.HasValue)
                            filterText = filterText + " <= " + frm.FilterToValue.ToString() + ";";
                    }
                    else if (_dataGridFilters[grd].FiltersNumeric.ContainsKey(columnName))
                        _dataGridFilters[grd].FiltersNumeric.Remove(columnName);
                }
                else if (form is FM.gridFilterBool)
                {
                    var frm = form as FM.gridFilterBool;
                    if (frm.FilterValue.HasValue)
                    {
                        if (!_dataGridFilters[grd].FiltersBool.ContainsKey(columnName))
                            _dataGridFilters[grd].FiltersBool.Add(columnName, new DataGridFilterBool() { Value = frm.FilterValue });
                        else
                            _dataGridFilters[grd].FiltersBool[columnName].Value = frm.FilterValue;

                        filterText = frm.FilterValue.Value.ToString();
                    }
                    else if (_dataGridFilters[grd].FiltersBool.ContainsKey(columnName))
                        _dataGridFilters[grd].FiltersBool.Remove(columnName);
                }
                else if (form is FM.gridFilterString)
                {
                    var frm = form as FM.gridFilterString;
                    if (!string.IsNullOrEmpty(frm.FilterValue))
                    {
                        if (!_dataGridFilters[grd].FiltersString.ContainsKey(columnName))
                            _dataGridFilters[grd].FiltersString.Add(columnName, new DataGridFilterString() { Value = frm.FilterValue });
                        else
                            _dataGridFilters[grd].FiltersString[columnName].Value = frm.FilterValue;

                        filterText = frm.FilterValue;
                    }
                    else if (_dataGridFilters[grd].FiltersString.ContainsKey(columnName))
                        _dataGridFilters[grd].FiltersString.Remove(columnName);
                }
                else if (form is FM.gridFilterDate)
                {
                    var frm = form as FM.gridFilterDate;
                    if (frm.FilterFromValue.HasValue || frm.FilterToValue.HasValue)
                    {
                        if (!_dataGridFilters[grd].FiltersDate.ContainsKey(columnName))
                            _dataGridFilters[grd].FiltersDate.Add(columnName, new DataGridFilterDate() { FromValue = frm.FilterFromValue, ToValue = frm.FilterToValue });
                        else
                        {
                            _dataGridFilters[grd].FiltersDate[columnName].FromValue = frm.FilterFromValue;
                            _dataGridFilters[grd].FiltersDate[columnName].ToValue = frm.FilterToValue;
                        }

                        if (frm.FilterFromValue.HasValue)
                            filterText = filterText + " >= " + frm.FilterFromValue.Value.ToString("dd.MM.yyyy") + ";";
                        if (frm.FilterToValue.HasValue)
                            filterText = filterText + " <= " + frm.FilterToValue.Value.ToString("dd.MM.yyyy") + ";";
                    }
                    else if (_dataGridFilters[grd].FiltersDate.ContainsKey(columnName))
                        _dataGridFilters[grd].FiltersDate.Remove(columnName);
                }
                if (!string.IsNullOrEmpty(filterText))
                {
                    column.Foreground = new SolidColorBrush(Colors.Green);
                    column.Column.Header = $"{column.Column.Header} ({Properties.Resources.MenuFilter}: {filterText})";
                }
            }
        }

        #region TempCode
        void InitializeDataGridsTemp(CTR.DataGrid grd)
        {
            //var itemClearFilter = new CTR.MenuItem() { Header = "Clear all filters", Name = "ClearAllFilters" };
            //itemClearFilter.Click += DataGrid_MenuClick_ClearAllFilters;
            //grd.ContextMenu.Items.Add(itemClearFilter);


            if (grd.Name == "GridEmittings" || grd.Name == "GridWorkTimes" || grd.Name == "GroupeEmissionProtocol" || grd.Name == "GroupeEmissionProtocolDetail")
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
                    FiltersString = new Dictionary<string, DataGridFilterString>(),
                    FiltersDate = new Dictionary<string, DataGridFilterDate>()
                });
            }
        }
        Type GetColumnTypeTemp(CTR.DataGrid grid, string columnName)
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
                if (columnName == "SensorTitle") return typeof(string);
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

            if (grid.Name == "GroupeEmissionProtocol")
            {
                if (columnName == "GSID") return typeof(string);
                if (columnName == "DateMeas") return typeof(DateTime);
                if (columnName == "StatusMeasStationFull") return typeof(string);
                if (columnName == "Owner") return typeof(string);
                if (columnName == "StationAddress") return typeof(string);
                if (columnName == "CoordinatesLon") return typeof(string);
                if (columnName == "CoordinatesLat") return typeof(string);
                if (columnName == "NumberPermission") return typeof(string);
                if (columnName == "PermissionStart") return typeof(DateTime);
                if (columnName == "PermissionPeriod") return typeof(DateTime);
                if (columnName == "SensorName") return typeof(string);
            }
            if (grid.Name == "GroupeEmissionProtocolDetail")
            {
                if (columnName == "GlobalSID") return typeof(string);
                if (columnName == "PermissionGlobalSID") return typeof(string);
                if (columnName == "StationTxFreq") return typeof(string);
                if (columnName == "StationChannel") return typeof(string);
                if (columnName == "StatusMeasFull") return typeof(string);
                if (columnName == "RadioControlMeasFreq_MHz") return typeof(double?);
                if (columnName == "RadioControlDeviationFreq_MHz") return typeof(double?);
                if (columnName == "RadioControlBandWidth_KHz") return typeof(double?);
                if (columnName == "FieldStrength") return typeof(double?);
                if (columnName == "Level_dBm") return typeof(double?);
                if (columnName == "DateMeas_OnlyDate") return typeof(DateTime);
                if (columnName == "SensorCoordinatesLon") return typeof(string);
                if (columnName == "SensorCoordinatesLat") return typeof(string);
                if (columnName == "CoordinatesLon") return typeof(string);
                if (columnName == "CoordinatesLat") return typeof(string);
                if (columnName == "OwnerName") return typeof(string);
                if (columnName == "Standard") return typeof(string);
                if (columnName == "Address") return typeof(string);
                if (columnName == "PermissionNumber") return typeof(string);
                if (columnName == "PermissionStart") return typeof(DateTime);
                if (columnName == "PermissionStop") return typeof(DateTime);
                if (columnName == "CurentStatusStation") return typeof(string);
                if (columnName == "TitleSensor") return typeof(string);
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

        void DataGridApplyFiltersTemp(CTR.DataGrid grid)
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
                        if (filter.Key == "SensorTitle" && !c.SensorTitle.Contains(filter.Value.Value))
                            return false;
                        if (filter.Key == "IcsmTable" && !c.IcsmTable.Contains(filter.Value.Value))
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
            if (grid.Name == "GroupeEmissionProtocol")
            {
                (grid.ItemsSource as ADP.DataSynchronizationProcessDataAdapter).ClearFilter();
                (grid.ItemsSource as ADP.DataSynchronizationProcessDataAdapter).ApplyFilter(c =>
                {
                    foreach (var filter in _dataGridFilters[grid].FiltersString)
                    {
                        if (filter.Key == "GSID" && !c.GSID.Contains(filter.Value.Value))
                            return false;
                        if (filter.Key == "StatusMeasStationFull" && !c.StatusMeasStationFull.Contains(filter.Value.Value))
                            return false;
                        if (filter.Key == "Owner" && !c.Owner.Contains(filter.Value.Value))
                            return false;
                        if (filter.Key == "StationAddress" && !c.StationAddress.Contains(filter.Value.Value))
                            return false;
                        if (filter.Key == "CoordinatesLon" && !c.CoordinatesLon.Contains(filter.Value.Value))
                            return false;
                        if (filter.Key == "CoordinatesLat" && !c.CoordinatesLat.Contains(filter.Value.Value))
                            return false;
                        if (filter.Key == "NumberPermission" && !c.NumberPermission.Contains(filter.Value.Value))
                            return false;
                        if (filter.Key == "SensorName" && !c.SensorName.Contains(filter.Value.Value))
                            return false;
                    }
                    foreach (var filter in _dataGridFilters[grid].FiltersDate)
                    {
                        if (filter.Key == "DateMeas")
                        {
                            if (filter.Value.FromValue.HasValue && (!c.DateMeas.HasValue || c.DateMeas < filter.Value.FromValue.Value.Date))
                                return false;
                            if (filter.Value.ToValue.HasValue && (!c.DateMeas.HasValue || c.DateMeas > filter.Value.ToValue.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59)))
                                return false;
                        }
                        if (filter.Key == "PermissionStart")
                        {
                            if (filter.Value.FromValue.HasValue && (!c.PermissionStart.HasValue || c.PermissionStart < filter.Value.FromValue.Value.Date))
                                return false;
                            if (filter.Value.ToValue.HasValue && (!c.PermissionStart.HasValue || c.PermissionStart > filter.Value.ToValue.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59)))
                                return false;
                        }
                        if (filter.Key == "PermissionPeriod")
                        {
                            if (filter.Value.FromValue.HasValue && (!c.PermissionPeriod.HasValue || c.PermissionPeriod < filter.Value.FromValue.Value.Date))
                                return false;
                            if (filter.Value.ToValue.HasValue && (!c.PermissionPeriod.HasValue || c.PermissionPeriod > filter.Value.ToValue.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59)))
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
                            if (filter.Value.FromValue.HasValue && (!c.RadioControlMeasFreq_MHz.HasValue || c.RadioControlMeasFreq_MHz < filter.Value.FromValue))
                                return false;
                            if (filter.Value.ToValue.HasValue && (!c.RadioControlMeasFreq_MHz.HasValue || c.RadioControlMeasFreq_MHz > filter.Value.ToValue))
                                return false;
                        }
                        if (filter.Key == "RadioControlDeviationFreq_MHz")
                        {
                            if (filter.Value.FromValue.HasValue && (!c.RadioControlDeviationFreq_MHz.HasValue || c.RadioControlDeviationFreq_MHz < filter.Value.FromValue))
                                return false;
                            if (filter.Value.ToValue.HasValue && (!c.RadioControlDeviationFreq_MHz.HasValue || c.RadioControlDeviationFreq_MHz > filter.Value.ToValue))
                                return false;
                        }
                        if (filter.Key == "RadioControlBandWidth_KHz")
                        {
                            if (filter.Value.FromValue.HasValue && (!c.RadioControlBandWidth_KHz.HasValue || c.RadioControlBandWidth_KHz < filter.Value.FromValue))
                                return false;
                            if (filter.Value.ToValue.HasValue && (!c.RadioControlBandWidth_KHz.HasValue || c.RadioControlBandWidth_KHz > filter.Value.ToValue))
                                return false;
                        }
                        if (filter.Key == "FieldStrength")
                        {
                            if (filter.Value.FromValue.HasValue && (!c.FieldStrength.HasValue || c.FieldStrength < filter.Value.FromValue))
                                return false;
                            if (filter.Value.ToValue.HasValue && (!c.FieldStrength.HasValue || c.FieldStrength > filter.Value.ToValue))
                                return false;
                        }
                        if (filter.Key == "Level_dBm")
                        {
                            if (filter.Value.FromValue.HasValue && (!c.Level_dBm.HasValue || c.Level_dBm < filter.Value.FromValue))
                                return false;
                            if (filter.Value.ToValue.HasValue && (!c.Level_dBm.HasValue || c.Level_dBm > filter.Value.ToValue))
                                return false;
                        }
                    }
                    foreach (var filter in _dataGridFilters[grid].FiltersString)
                    {
                        if (filter.Key == "GlobalSID" && !c.GlobalSID.Contains(filter.Value.Value))
                            return false;
                        if (filter.Key == "PermissionGlobalSID" && !c.PermissionGlobalSID.Contains(filter.Value.Value))
                            return false;
                        if (filter.Key == "StationTxFreq" && !c.StationTxFreq.Contains(filter.Value.Value))
                            return false;
                        if (filter.Key == "StationTxChannel" && !c.StationTxChannel.Contains(filter.Value.Value))
                            return false;
                        if (filter.Key == "StationRxChannel" && !c.StationRxChannel.Contains(filter.Value.Value))
                            return false;
                        if (filter.Key == "StatusMeasFull" && !c.StatusMeasFull.Contains(filter.Value.Value))
                            return false;
                        if (filter.Key == "SensorCoordinatesLon" && !c.SensorCoordinatesLon.Contains(filter.Value.Value))
                            return false;
                        if (filter.Key == "SensorCoordinatesLat" && !c.SensorCoordinatesLat.Contains(filter.Value.Value))
                            return false;
                        if (filter.Key == "CoordinatesLon" && !c.CoordinatesLon.Contains(filter.Value.Value))
                            return false;
                        if (filter.Key == "CoordinatesLat" && !c.CoordinatesLat.Contains(filter.Value.Value))
                            return false;
                        if (filter.Key == "OwnerName" && !c.OwnerName.Contains(filter.Value.Value))
                            return false;
                        if (filter.Key == "Standard" && !c.Standard.Contains(filter.Value.Value))
                            return false;
                        if (filter.Key == "Address" && !c.Address.Contains(filter.Value.Value))
                            return false;
                        if (filter.Key == "PermissionNumber" && !c.PermissionNumber.Contains(filter.Value.Value))
                            return false;
                        if (filter.Key == "CurentStatusStation" && !c.CurentStatusStation.Contains(filter.Value.Value))
                            return false;
                        if (filter.Key == "TitleSensor" && !c.TitleSensor.Contains(filter.Value.Value))
                            return false;
                    }
                    foreach (var filter in _dataGridFilters[grid].FiltersDate)
                    {
                        if (filter.Key == "DateMeas_OnlyDate")
                        {
                            if (filter.Value.FromValue.HasValue && (!c.DateMeas_OnlyDate.HasValue || c.DateMeas_OnlyDate < filter.Value.FromValue.Value.Date))
                                return false;
                            if (filter.Value.ToValue.HasValue && (!c.DateMeas_OnlyDate.HasValue || c.DateMeas_OnlyDate > filter.Value.ToValue.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59)))
                                return false;
                        }
                        if (filter.Key == "PermissionStart")
                        {
                            if (filter.Value.FromValue.HasValue && (!c.PermissionStart.HasValue || c.PermissionStart < filter.Value.FromValue.Value.Date))
                                return false;
                            if (filter.Value.ToValue.HasValue && (!c.PermissionStart.HasValue || c.PermissionStart > filter.Value.ToValue.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59)))
                                return false;
                        }
                        if (filter.Key == "PermissionStop")
                        {
                            if (filter.Value.FromValue.HasValue && (!c.PermissionStop.HasValue || c.PermissionStop < filter.Value.FromValue.Value.Date))
                                return false;
                            if (filter.Value.ToValue.HasValue && (!c.PermissionStop.HasValue || c.PermissionStop > filter.Value.ToValue.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59)))
                                return false;
                        }
                    }
                    return true;
                });
            }

        }
        #endregion

        #region VisualTree
        static string GetTreeKey(DependencyObject depObj)
        {
            string key = "";
            if (depObj != null)
            {
                var obj = VisualTreeHelper.GetParent(depObj);
                if (obj != null)
                    key = GetTreeKey(obj) + "/" + obj.DependencyObjectType.Name;
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
                    visualCollection.Add(child as T);
                else if (child != null)
                    GetVisualChildCollection(child, visualCollection);
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
                        yield return (T)child;

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                        yield return childOfChild;
                }
            }
        }
        #endregion

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // WpfFormBase
            // 
            this.ClientSize = new System.Drawing.Size(278, 244);
            this.Name = "WpfFormBase";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.ResumeLayout(false);

        }
    }
}
