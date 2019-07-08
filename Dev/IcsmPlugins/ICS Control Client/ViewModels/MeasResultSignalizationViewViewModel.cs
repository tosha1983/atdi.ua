using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using XICSM.ICSControlClient.Models;
using XICSM.ICSControlClient.Models.Views;
using XICSM.ICSControlClient.Environment.Wpf;
using XICSM.ICSControlClient.Models.WcfDataApadters;
using SVC = XICSM.ICSControlClient.WcfServiceClients;
using CS = XICSM.ICSControlClient.WpfControls.Charts;
using MP = XICSM.ICSControlClient.WpfControls.Maps;
using SDR = Atdi.Contracts.WcfServices.Sdrn.Server;
using System.Windows;
using FRM = System.Windows.Forms;
using FM = XICSM.ICSControlClient.Forms;
using ICSM;
using System.Windows.Controls;
using INP = System.Windows.Input;
using System.Collections;
using System.Globalization;
using System.Timers;
using XICSM.ICSControlClient.Forms;

namespace XICSM.ICSControlClient.ViewModels
{
   
    public class MeasResultSignalizationViewViewModel : WpfViewModelBase
    {
        private long[] _stations;
        private readonly string _tableName;

        #region Current Objects
        private string _emittingCaption;

        private IList _currentEmittings;
        private EmittingViewModel _currentEmitting;
        private CS.ChartOption _currentChartOption;
        private CS.ChartOption _currentChartLevelsDistrbutionOption;
        private double[] _selectedRangeX;
        private Point _mouseClickPoint;
        private Stack<double[]> _zoomHistory = new Stack<double[]>();
        #endregion

        private EmittingDataAdapter _emittings;
        private EmittingWorkTimeDataAdapter _emittingWorkTimes;
        public EmittingDataAdapter Emittings => this._emittings;
        public EmittingWorkTimeDataAdapter EmittingWorkTimes => this._emittingWorkTimes;

        private readonly DataStore _dataStore;
        private bool _statusBarIsIndeterminate = false;
        private string _statusBarTitle = "";
        private string _statusBarDecription = "";
        private Timer _timer = null;
        private WaitForm _waitForm = null;

        #region Commands
        public WpfCommand ZoomUndoCommand { get; set; }
        public WpfCommand ZoomDefaultCommand { get; set; }
        //public WpfCommand AddAssociationStationCommand { get; set; }
        //public WpfCommand DeleteEmissionCommand { get; set; }
        #endregion

        public MeasResultSignalizationViewViewModel(long[] stations, string tableName, MeasResultSignalizationViewForm form)
        {
            this._dataStore = DataStore.GetStore();
            this._dataStore.OnBeginInvoke += _dataStore_OnBeginInvoke;
            this._dataStore.OnEndInvoke += _dataStore_OnEndInvoke;

            this._stations = stations;
            this._tableName = tableName;

            this._emittings = new EmittingDataAdapter();
            this._emittingWorkTimes = new EmittingWorkTimeDataAdapter();
            this.ZoomUndoCommand = new WpfCommand(this.OnZoomUndoCommand);
            this.ZoomDefaultCommand = new WpfCommand(this.OnZoomDefaultCommand);
            //this.AddAssociationStationCommand = new WpfCommand(this.OnAddAssociationStationCommand);
            //this.DeleteEmissionCommand = new WpfCommand(this.OnDeleteEmissionCommand);

            Task.Run(() => this.ReloadMeasResult(form));

            
        }

        private void _dataStore_OnEndInvoke(string description)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (_timer != null)
                {
                    _timer.Enabled = false;
                    _timer = null;
                }

                this.StatusBarIsIndeterminate = false;
                this.StatusBarTitle = $"Loaded data of {description}";


                if (_waitForm != null)
                {
                    _waitForm.Close();
                    _waitForm.Dispose();
                    _waitForm = null;
                }
            }));
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                _timer.Enabled = false;
                _waitForm = new FM.WaitForm();
                _waitForm.SetMessage($"Please wait. {this.StatusBarTitle}");
                _waitForm.TopMost = true;
                _waitForm.ShowDialog();
                //_waitForm.FormBorderStyle = FRM.FormBorderStyle.FixedSingle;
                //_waitForm.Refresh();
            }));
        }

        private void _dataStore_OnBeginInvoke(string description)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                this.StatusBarIsIndeterminate = true;
                this.StatusBarTitle = $"Loading data of {description} ...";

                this._timer = new Timer(300);
                this._timer.Elapsed += _timer_Elapsed;
                this._timer.Enabled = true;
            }));
        }

        public string StatusBarTitle
        {
            get => this._statusBarTitle;
            set => this.Set(ref this._statusBarTitle, value);
        }

        public string StatusBarDescription
        {
            get => this._statusBarDecription;
            set => this.Set(ref this._statusBarDecription, value);
        }

        public bool StatusBarIsIndeterminate
        {
            get => this._statusBarIsIndeterminate;
            set => this.Set(ref this._statusBarIsIndeterminate, value);
        }

        public string EmittingCaption
        {
            get => this._emittingCaption;
            set => this.Set(ref this._emittingCaption, value);
        }
        private string _rbw = string.Empty;

        public string RBW
        {
            get => this._rbw; // this.GetCurrentRBWValue();
            set => this.Set(ref this._rbw, value);
        }

        public CS.ChartOption CurrentChartOption
        {
            get => this._currentChartOption;
            set => this.Set(ref this._currentChartOption, value);
        }
        public CS.ChartOption CurrentChartLevelsDistrbutionOption
        {
            get => this._currentChartLevelsDistrbutionOption;
            set => this.Set(ref this._currentChartLevelsDistrbutionOption, value);
        }
        
        public IList CurrentEmittings
        {
            get => this._currentEmittings;
            set
            {
                this._currentEmittings = value;
                if (this._selectedRangeX != null && this._selectedRangeX.Count() == 2)
                    UpdateCurrentChartOption(this._selectedRangeX[0], this._selectedRangeX[1]);
                else
                    this.UpdateCurrentChartOption(null, null);
            }
        }
        public EmittingViewModel CurrentEmitting
        {
            get => this._currentEmitting;
            set => this.Set(ref this._currentEmitting, value, () => { ReloadEmittingWorkTime(); UpdateCurrentChartLevelsDistrbutionOption(); });
        }
        public double[] SelectedRangeX
        {
            get => this._selectedRangeX;
            set
            {
                this._selectedRangeX = value;

                if (this._selectedRangeX != null && this._selectedRangeX.Count() == 2)
                {
                    _zoomHistory.Push(this._selectedRangeX);
                    this.FilterEmittings(this._selectedRangeX[0], this._selectedRangeX[1]);
                    UpdateCurrentChartOption(this._selectedRangeX[0], this._selectedRangeX[1]);
                }
            }
        }

        public Point MouseClickPoint
        {
            get => this._mouseClickPoint;
            set => this.Set(ref this._mouseClickPoint, value);

        }
        public MenuItem MenuClick
        {
            set
            {
                //if (value.Name == "DetailForRefLevel")
                //    this.OnDetailForRefLevelCommand();
                //if (value.Name == "ViewStation")
                //    this.OnViewStationCommand();
            }
        }

        private void ReloadMeasResult(MeasResultSignalizationViewForm form)
        {

            var emittings = this._dataStore.GetEmittingsBy(_stations, _tableName); //SVC.SdrnsControllerWcfClient.GetMeasurementResultByResId(_resultId, null, null);

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                this._emittings.Source = emittings;
                this.RBW = this.GetCurrentRBWValue();
                this.EmittingCaption = this.GetCurrentEmittingCaption();

                this.UpdateCurrentChartOption(null, null);
                this.UpdateCurrentChartLevelsDistrbutionOption();

                if (emittings == null || emittings.Length == 0)
                {
                    MessageBox.Show("No Emissions", "Result Monitoring (Fix Sensor) Station", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    if (form != null)
                    {
                        form.Close();
                    }
                }

            }));
        }
        private void UpdateCurrentChartOption(double? startFreq, double? stopFreq)
        {
            this.CurrentChartOption = this.GetChartOption(startFreq, stopFreq);
        }
        private void UpdateCurrentChartLevelsDistrbutionOption()
        {
            this.CurrentChartLevelsDistrbutionOption = this.GetChartLevelsDistrbutionOption();
        }
        private void FilterEmittings(double? startFreq, double? stopFreq)
        {
            if (startFreq.HasValue && stopFreq.HasValue)
            {
                this._emittings.ApplyFilter(c => (c.StartFrequency_MHz > startFreq.Value && c.StartFrequency_MHz < stopFreq.Value) || (c.StopFrequency_MHz > startFreq.Value && c.StopFrequency_MHz < stopFreq.Value));
            }
            else
                this._emittings.ClearFilter();

            this.EmittingCaption = this.GetCurrentEmittingCaption();
        }
        private void ReloadEmittingWorkTime()
        {
            this._emittingWorkTimes.Source = _currentEmitting.WorkTimes;
        }
        private void OnZoomUndoCommand(object parameter)
        {
            if (_zoomHistory.Count > 0)
            {
                _zoomHistory.Pop();
                if (_zoomHistory.Count > 0)
                {
                    var lastZoom = _zoomHistory.Peek();
                    this._emittings.ClearFilter();
                    this.FilterEmittings(lastZoom[0], lastZoom[1]);
                    UpdateCurrentChartOption(lastZoom[0], lastZoom[1]);
                    this._selectedRangeX = lastZoom;
                }
                else
                {
                    this.FilterEmittings(null, null);
                    UpdateCurrentChartOption(null, null);
                    this._selectedRangeX = null;
                }
            }
            else
            {
                this.FilterEmittings(null, null);
                UpdateCurrentChartOption(null, null);
                this._selectedRangeX = null;
            }
        }
        private void OnZoomDefaultCommand(object parameter)
        {
            this._selectedRangeX = null;
            _zoomHistory.Clear();
            this.FilterEmittings(null, null);
            UpdateCurrentChartOption(null, null);
        }
        //private void OnDeleteEmissionCommand(object parameter)
        //{
        //    try
        //    {
        //        if (this._currentEmittings != null)
        //        {
        //            List<int> emitings = new List<int>();
        //            HashSet<int?> ids = new HashSet<int?>();
        //            foreach (EmittingViewModel emitting in this._currentEmittings)
        //            {
        //                if (emitting.Id.HasValue)
        //                {
        //                    emitings.Add(emitting.Id.Value);
        //                    ids.Add(emitting.Id.Value);
        //                }
                            
        //            }

        //            this.StatusBarTitle = $"Deleting emissions ({emitings.Count}) ...";
        //            this.StatusBarIsIndeterminate = true;

        //            _waitForm = new FM.WaitForm();
        //            _waitForm.SetMessage($"Please wait. {this.StatusBarTitle}");
        //            _waitForm.TopMost = true;

        //            Task.Run(() =>
        //            {

        //                try
        //                {

        //                    SVC.SdrnsControllerWcfClient.DeleteEmittingById(emitings.ToArray());

        //                    this._currentMeasResult.Emittings = this._currentMeasResult.Emittings.Where(e => !ids.Contains(e.Id)).ToArray();

        //                    Application.Current.Dispatcher.Invoke(new Action(() =>
        //                    {

        //                        this._emittings.Source = this._currentMeasResult.Emittings;
        //                        this.EmittingCaption = this.GetCurrentEmittingCaption();

        //                        if (this._selectedRangeX != null && this._selectedRangeX.Length == 2)
        //                        {
        //                            this.FilterEmittings(this._selectedRangeX[0], this._selectedRangeX[1]);
        //                        }
        //                        this.StatusBarTitle = $"Deleted emissions ({emitings.Count})";
        //                    }));
        //                }
        //                catch (Exception e)
        //                {
        //                    this.StatusBarTitle = $"An error occurred while the emissions deleting: {e.Message}";
        //                }
        //            }).ContinueWith(task =>
        //            {
        //                Application.Current.Dispatcher.Invoke(new Action(() =>
        //                {
        //                    this.StatusBarIsIndeterminate = false;
        //                    if (_waitForm != null)
        //                    {
        //                        _waitForm.Close();
        //                        _waitForm = null;
        //                    }
        //                }));
        //            });

        //            _waitForm.ShowDialog();

        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        MessageBox.Show(e.ToString());
        //    }
        //}
        //private void OnAddAssociationStationCommand(object parameter)
        //{
        //    try
        //    {
        //        if (this._currentEmitting == null)
        //            return;

        //        var stationData = new List<MeasStationsSignalization>();

        //        string sep = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        //        double lonSensor = 0;
        //        double latSensor = 0;

        //        if (this._currentMeasResult != null && this._currentMeasResult.LocationSensorMeasurement != null && this._currentMeasResult.LocationSensorMeasurement.Count() > 0)
        //        {
        //            var _currentSensorLocation = this._currentMeasResult.LocationSensorMeasurement[this._currentMeasResult.LocationSensorMeasurement.Count() - 1];

        //            if (_currentSensorLocation.Lon.HasValue && _currentSensorLocation.Lat.HasValue)
        //            {
        //                lonSensor = _currentSensorLocation.Lon.Value;
        //                latSensor = _currentSensorLocation.Lat.Value;
        //            }
        //        }

        //        double freq = this._currentEmitting.EmissionFreqMHz == IM.NullD ? (this._currentEmitting.StartFrequency_MHz + this._currentEmitting.StopFrequency_MHz) / 2 : this._currentEmitting.EmissionFreqMHz;
        //        double bw = this._currentEmitting.Bandwidth_kHz == IM.NullD ? (this._currentEmitting.StopFrequency_MHz - this._currentEmitting.StartFrequency_MHz) * 1000 : this._currentEmitting.Bandwidth_kHz;

        //        var dlgForm = new FM.MeasStationsSignalizationDlg1Form(50, bw, true);
        //        dlgForm.ShowDialog();
        //        dlgForm.Dispose();

        //        double distance = dlgForm.Distance;

        //        string sqlQuery = "[TX_FREQ] - " + (bw / 2000).ToString().Replace(sep, ".") + " - [Station.BW] / 2000 <= " + freq.ToString().Replace(sep, ".");
        //        sqlQuery = sqlQuery + " and " + "[TX_FREQ] + " + (bw / 2000).ToString().Replace(sep, ".") + " + [Station.BW] / 2000 >= " + freq.ToString().Replace(sep, ".");
        //        sqlQuery = sqlQuery + " and " + "111.315 * POWER((POWER(((" + lonSensor.ToString().Replace(sep, ".") + " - [Station.Position.LONGITUDE]) * COS([Station.Position.LATITUDE] * 3.14159265359 / 180)), 2) + POWER((" + latSensor.ToString().Replace(sep, ".") + " - [Station.Position.LATITUDE]), 2)), 0.5)  < " + distance.ToString().Replace(sep, ".");
        //        {
        //            IMRecordset rs = new IMRecordset("MOBSTA_FREQS", IMRecordset.Mode.ReadOnly);
        //            rs.SetAdditional(sqlQuery);
        //            rs.Select("Station.NAME,Station.STANDARD,Station.STATUS,Station.Position.LONGITUDE,Station.Position.LATITUDE,Station.AGL,Station.POWER,Station.BW,Station.Owner.NAME,TX_FREQ,Station.ID");
        //            for (rs.Open(); !rs.IsEOF(); rs.MoveNext())
        //            {
        //                var measStationSignalization = new MeasStationsSignalization()
        //                {
        //                    IcsmId = rs.GetI("Station.ID"),
        //                    IcsmTable = "MOB_STATION",
        //                    StationName = rs.GetS("Station.NAME"),
        //                    Standart = rs.GetS("Station.STANDARD"),
        //                    Status = rs.GetS("Station.STATUS"),
        //                    Lon = rs.GetD("Station.Position.LONGITUDE"),
        //                    Lat = rs.GetD("Station.Position.LATITUDE"),
        //                    Agl = rs.GetD("Station.AGL"),
        //                    Eirp = rs.GetD("Station.POWER"),
        //                    Bw = rs.GetD("Station.BW"),
        //                    Freq = rs.GetD("TX_FREQ"),
        //                    Owner = rs.GetS("Station.Owner.NAME"),
        //                    RelivedLevel = 0
        //                };

        //                if (measStationSignalization.Lon != IM.NullD && measStationSignalization.Lat != IM.NullD)
        //                {
        //                    measStationSignalization.Distance = 111.315 * Math.Pow((Math.Pow((lonSensor - measStationSignalization.Lon) * Math.Cos(measStationSignalization.Lat * Math.PI / 180), 2) + Math.Pow((latSensor - measStationSignalization.Lat), 2)), 0.5);
        //                }
        //                stationData.Add(measStationSignalization);
        //            }
        //            if (rs.IsOpen())
        //                rs.Close();
        //            rs.Destroy();
        //        }
        //        {
        //            IMRecordset rs = new IMRecordset("MOBSTA_FREQS2", IMRecordset.Mode.ReadOnly);
        //            rs.SetAdditional(sqlQuery);
        //            rs.Select("Station.NAME,Station.STANDARD,Station.STATUS,Station.Position.LONGITUDE,Station.Position.LATITUDE,Station.AGL,Station.POWER,Station.BW,Station.Owner.NAME,TX_FREQ,Station.ID");
        //            for (rs.Open(); !rs.IsEOF(); rs.MoveNext())
        //            {
        //                var measStationSignalization = new MeasStationsSignalization()
        //                {
        //                    IcsmId = rs.GetI("Station.ID"),
        //                    IcsmTable = "MOB_STATION2",
        //                    StationName = rs.GetS("Station.NAME"),
        //                    Standart = rs.GetS("Station.STANDARD"),
        //                    Status = rs.GetS("Station.STATUS"),
        //                    Lon = rs.GetD("Station.Position.LONGITUDE"),
        //                    Lat = rs.GetD("Station.Position.LATITUDE"),
        //                    Agl = rs.GetD("Station.AGL"),
        //                    Eirp = rs.GetD("Station.POWER"),
        //                    Bw = rs.GetD("Station.BW"),
        //                    Freq = rs.GetD("TX_FREQ"),
        //                    Owner = rs.GetS("Station.Owner.NAME"),
        //                    RelivedLevel = 0
        //                };

        //                if (measStationSignalization.Lon != IM.NullD && measStationSignalization.Lat != IM.NullD)
        //                {
        //                    measStationSignalization.Distance = 111.315 * Math.Pow((Math.Pow((lonSensor - measStationSignalization.Lon) * Math.Cos(measStationSignalization.Lat * Math.PI / 180), 2) + Math.Pow((latSensor - measStationSignalization.Lat), 2)), 0.5);
        //                }
        //                stationData.Add(measStationSignalization);
        //            }
        //            if (rs.IsOpen())
        //                rs.Close();
        //            rs.Destroy();
        //        }
                
        //        if (stationData.Count == 0)
        //        {
        //            System.Windows.MessageBox.Show("No Stations");
        //            return;
        //        }
        //        string caption = ", Frequency - " + Math.Round(freq, 6).ToString() + ", MHz, Pow of Emission - " + Math.Round(this._currentEmitting.CurentPower_dBm, 1).ToString() + ", dBm";
        //        var measTaskForm = new FM.MeasStationsSignalizationForm(stationData.OrderBy(c => c.Distance).ToArray(), this._currentMeasResult, true, this._currentEmitting.Id, caption);
        //        measTaskForm.ShowDialog();
        //        measTaskForm.Dispose();
        //    }
        //    catch (Exception e)
        //    {
        //        MessageBox.Show(e.ToString());
        //    }
        //}
        //private void OnDetailForRefLevelCommand()
        //{
        //    try
        //    {
        //        var measTask = SVC.SdrnsControllerWcfClient.GetMeasTaskById(_currentMeasResult.Id.MeasTaskId.Value);
        //        var stationData = new List<MeasStationsSignalization>();

        //        var freq = _mouseClickPoint.X;
        //        double lonSensor = 0;
        //        double latSensor = 0;

        //        if (this._currentMeasResult != null && this._currentMeasResult.LocationSensorMeasurement != null && this._currentMeasResult.LocationSensorMeasurement.Count() > 0)
        //        {
        //            var _currentSensorLocation = this._currentMeasResult.LocationSensorMeasurement[this._currentMeasResult.LocationSensorMeasurement.Count() - 1];

        //            if (_currentSensorLocation.Lon.HasValue && _currentSensorLocation.Lat.HasValue)
        //            {
        //                lonSensor = _currentSensorLocation.Lon.Value;
        //                latSensor = _currentSensorLocation.Lat.Value;
        //            }
        //        }

        //        if (measTask.RefSituation != null)
        //        {
        //            foreach (var refSituation in measTask.RefSituation)
        //            {
        //                foreach (var refSignal in refSituation.ReferenceSignal)
        //                {
        //                    if (refSignal.Frequency_MHz - refSignal.Bandwidth_kHz / 2000 <= freq && freq <= refSignal.Frequency_MHz + refSignal.Bandwidth_kHz / 2000)
        //                    {
        //                        if (!string.IsNullOrEmpty(refSignal.IcsmTable) && refSignal.IcsmId > 0)
        //                        {
        //                            IMRecordset rs = new IMRecordset(refSignal.IcsmTable, IMRecordset.Mode.ReadOnly);
        //                            rs.SetWhere("ID", IMRecordset.Operation.Eq, refSignal.IcsmId);
        //                            rs.Select("NAME,STANDARD,STATUS,Position.LONGITUDE,Position.LATITUDE,AGL,POWER,BW,Owner.NAME");
        //                            for (rs.Open(); !rs.IsEOF(); rs.MoveNext())
        //                            {
        //                                var measStationSignalization = new MeasStationsSignalization()
        //                                {
        //                                    IcsmId = refSignal.IcsmId,
        //                                    IcsmTable = refSignal.IcsmTable,
        //                                    StationName = rs.GetS("NAME"),
        //                                    Standart = rs.GetS("STANDARD"),
        //                                    Status = rs.GetS("STATUS"),
        //                                    Lon = rs.GetD("Position.LONGITUDE"),
        //                                    Lat = rs.GetD("Position.LATITUDE"),
        //                                    Agl = rs.GetD("AGL"),
        //                                    Eirp = rs.GetD("POWER"),
        //                                    Bw = rs.GetD("BW"),
        //                                    Freq = refSignal.Frequency_MHz,
        //                                    Owner = rs.GetS("Owner.NAME"),
        //                                    RelivedLevel = refSignal.LevelSignal_dBm
        //                                };

        //                                if (measStationSignalization.Lon != IM.NullD && measStationSignalization.Lat != IM.NullD)
        //                                {
        //                                    measStationSignalization.Distance = 111.315 * Math.Pow((Math.Pow((lonSensor - measStationSignalization.Lon) * Math.Cos(measStationSignalization.Lat * Math.PI / 180), 2) + Math.Pow((latSensor - measStationSignalization.Lat), 2)), 0.5);
        //                                }
        //                                stationData.Add(measStationSignalization);
        //                            }
        //                            if (rs.IsOpen())
        //                                rs.Close();
        //                            rs.Destroy();
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        if (stationData.Count == 0)
        //        {
        //            System.Windows.MessageBox.Show("No Stations");
        //            return;
        //        }
        //        string caption = ", Frequency - " + Math.Round(freq, 6).ToString() + ", MHz";
        //        var measTaskForm = new FM.MeasStationsSignalizationForm(stationData.ToArray(), this._currentMeasResult, false, null, caption);
        //        measTaskForm.ShowDialog();
        //        measTaskForm.Dispose();
        //    }
        //    catch (Exception e)
        //    {
        //        MessageBox.Show(e.ToString());
        //    }
        //}
        //private void OnViewStationCommand()
        //{
        //    try
        //    {
        //        var stationData = new List<MeasStationsSignalization>();

        //        string sep = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        //        var freq = _mouseClickPoint.X;
        //        double lonSensor = 0;
        //        double latSensor = 0;

        //        if (this._currentMeasResult != null && this._currentMeasResult.LocationSensorMeasurement != null && this._currentMeasResult.LocationSensorMeasurement.Count() > 0)
        //        {
        //            var _currentSensorLocation = this._currentMeasResult.LocationSensorMeasurement[this._currentMeasResult.LocationSensorMeasurement.Count() - 1];

        //            if (_currentSensorLocation.Lon.HasValue && _currentSensorLocation.Lat.HasValue)
        //            {
        //                lonSensor = _currentSensorLocation.Lon.Value;
        //                latSensor = _currentSensorLocation.Lat.Value;
        //            }
        //        }

        //        var dlgForm = new FM.MeasStationsSignalizationDlg1Form(10, 200, false);
        //        dlgForm.ShowDialog();
        //        dlgForm.Dispose();

        //        double distance = dlgForm.Distance;
        //        double bw = dlgForm.Bw;

        //        string sqlQuery = "[TX_FREQ] - " + (bw / 2000).ToString().Replace(sep, ".") + " - [Station.BW] / 2000 <= " + freq.ToString().Replace(sep, ".");
        //        sqlQuery = sqlQuery + " and " + "[TX_FREQ] + " + (bw / 2000).ToString().Replace(sep, ".") + " + [Station.BW] / 2000 >= " + freq.ToString().Replace(sep, ".");
        //        sqlQuery = sqlQuery + " and " + "111.315 * POWER((POWER(((" + lonSensor.ToString().Replace(sep, ".") + " - [Station.Position.LONGITUDE]) * COS([Station.Position.LATITUDE] * 3.14159265359 / 180)), 2) + POWER((" + latSensor.ToString().Replace(sep, ".") + " - [Station.Position.LATITUDE]), 2)), 0.5)  < " + distance.ToString().Replace(sep, ".");
        //        {
        //            IMRecordset rs = new IMRecordset("MOBSTA_FREQS", IMRecordset.Mode.ReadOnly);
        //            rs.SetAdditional(sqlQuery);
        //            rs.Select("Station.NAME,Station.STANDARD,Station.STATUS,Station.Position.LONGITUDE,Station.Position.LATITUDE,Station.AGL,Station.POWER,Station.BW,Station.Owner.NAME,TX_FREQ,Station.ID");
        //            for (rs.Open(); !rs.IsEOF(); rs.MoveNext())
        //            {
        //                var measStationSignalization = new MeasStationsSignalization()
        //                {
        //                    IcsmId = rs.GetI("Station.ID"),
        //                    IcsmTable = "MOB_STATION",
        //                    StationName = rs.GetS("Station.NAME"),
        //                    Standart = rs.GetS("Station.STANDARD"),
        //                    Status = rs.GetS("Station.STATUS"),
        //                    Lon = rs.GetD("Station.Position.LONGITUDE"),
        //                    Lat = rs.GetD("Station.Position.LATITUDE"),
        //                    Agl = rs.GetD("Station.AGL"),
        //                    Eirp = rs.GetD("Station.POWER"),
        //                    Bw = rs.GetD("Station.BW"),
        //                    Freq = rs.GetD("TX_FREQ"),
        //                    Owner = rs.GetS("Station.Owner.NAME"),
        //                    RelivedLevel = 0
        //                };

        //                if (measStationSignalization.Lon != IM.NullD && measStationSignalization.Lat != IM.NullD)
        //                {
        //                    measStationSignalization.Distance = 111.315 * Math.Pow((Math.Pow((lonSensor - measStationSignalization.Lon) * Math.Cos(measStationSignalization.Lat * Math.PI / 180), 2) + Math.Pow((latSensor - measStationSignalization.Lat), 2)), 0.5);
        //                }
        //                stationData.Add(measStationSignalization);
        //            }
        //            if (rs.IsOpen())
        //                rs.Close();
        //            rs.Destroy();
        //        }
        //        {
        //            IMRecordset rs = new IMRecordset("MOBSTA_FREQS2", IMRecordset.Mode.ReadOnly);
        //            rs.SetAdditional(sqlQuery);
        //            rs.Select("Station.NAME,Station.STANDARD,Station.STATUS,Station.Position.LONGITUDE,Station.Position.LATITUDE,Station.AGL,Station.POWER,Station.BW,Station.Owner.NAME,TX_FREQ,Station.ID");
        //            for (rs.Open(); !rs.IsEOF(); rs.MoveNext())
        //            {
        //                var measStationSignalization = new MeasStationsSignalization()
        //                {
        //                    IcsmId = rs.GetI("Station.ID"),
        //                    IcsmTable = "MOB_STATION2",
        //                    StationName = rs.GetS("Station.NAME"),
        //                    Standart = rs.GetS("Station.STANDARD"),
        //                    Status = rs.GetS("Station.STATUS"),
        //                    Lon = rs.GetD("Station.Position.LONGITUDE"),
        //                    Lat = rs.GetD("Station.Position.LATITUDE"),
        //                    Agl = rs.GetD("Station.AGL"),
        //                    Eirp = rs.GetD("Station.POWER"),
        //                    Bw = rs.GetD("Station.BW"),
        //                    Freq = rs.GetD("TX_FREQ"),
        //                    Owner = rs.GetS("Station.Owner.NAME"),
        //                    RelivedLevel = 0
        //                };

        //                if (measStationSignalization.Lon != IM.NullD && measStationSignalization.Lat != IM.NullD)
        //                {
        //                    measStationSignalization.Distance = 111.315 * Math.Pow((Math.Pow((lonSensor - measStationSignalization.Lon) * Math.Cos(measStationSignalization.Lat * Math.PI / 180), 2) + Math.Pow((latSensor - measStationSignalization.Lat), 2)), 0.5);
        //                }
        //                stationData.Add(measStationSignalization);
        //            }
        //            if (rs.IsOpen())
        //                rs.Close();
        //            rs.Destroy();
        //        }

        //        if (stationData.Count == 0)
        //        {
        //            System.Windows.MessageBox.Show("No Stations");
        //            return;
        //        }
        //        string caption = ", Frequency - " + Math.Round(freq, 6).ToString() + ", MHz";
        //        var measTaskForm = new FM.MeasStationsSignalizationForm(stationData.ToArray(), this._currentMeasResult, false, null, caption);
        //        measTaskForm.ShowDialog();
        //        measTaskForm.Dispose();
        //    }
        //    catch (Exception e)
        //    {
        //        MessageBox.Show(e.ToString());
        //    }
        //}
        private string GetCurrentRBWValue()
        {
            if (this._emittings.Source == null || this._emittings.Source.Length == 0)
            {
                return "RBW = (unknown) kHz";
            }


            var firstEmession = ((SDR.Emitting)this._emittings.Source[0]);
            var rbw =  firstEmession.Spectrum.SpectrumSteps_kHz;

            var res = string.Empty;

            if (rbw > 1000)
                res = Math.Round(rbw, 1).ToString();
            else if (1000 > rbw && rbw > 100)
                res = Math.Round(rbw, 2).ToString();
            else if (100 > rbw && rbw > 10)
                res = Math.Round(rbw, 3).ToString();
            else 
                res = Math.Round(rbw, 4).ToString();

            return "RBW = " + res + " kHz";
            
        }
        private string GetCurrentEmittingCaption()
        {
            return $"Emissions ({this._emittings.Count()})";
        }

        private CS.ChartOption GetChartOption(double? startFreq, double? stopFreq)
        {
            var option = new CS.ChartOption
            {
                Title = "Ref level",
                YLabel = "Level (dBm)",
                XLabel = "Freq (MHz)",
                ChartType = CS.ChartType.Line,
                XInnerTickCount = 5,
                YInnerTickCount = 5,
                YMin = -120,
                YMax = -10,
                XMin = 900,
                XMax = 960,
                YTick = 10,
                XTick = 10,
                UseZoom = true
            };

            //if (_currentMeasResult.RefLevels == null || _currentMeasResult.RefLevels.levels == null)
            //    return option;

            var maxX = option.XMax; // default(double);
            var minX = option.XMin; // default(double);

            var maxY = option.YMax; // default(double);
            var minY = option.YMin; // default(double);

            var linesList = new List<CS.ChartLine>();
            var pointsList = new List<CS.ChartPoints>();
            //{
            //    var count = _currentMeasResult.RefLevels.levels.Length;
            //    var points = new List<Point>();

            //    int j = 0;
            //    for (int i = 0; i < count; i++)
            //    {
            //        var valX = (_currentMeasResult.RefLevels.StartFrequency_Hz + _currentMeasResult.RefLevels.StepFrequency_Hz * i) / 1000000;
            //        var valY = _currentMeasResult.RefLevels.levels[i];

            //        if (startFreq.HasValue && valX < startFreq.Value || stopFreq.HasValue && valX > stopFreq.Value)
            //            continue;

            //        var point = new Point
            //        {
            //            X = valX,
            //            Y = valY
            //        };
            //        if (j == 0)
            //        {
            //            maxX = valX;
            //            minX = valX;
            //            maxY = valY;
            //            minY = valY;
            //        }
            //        else
            //        {
            //            if (maxX < valX)
            //                maxX = valX;
            //            if (minX > valX)
            //                minX = valX;
            //            if (maxY < valY)
            //                maxY = valY;
            //            if (minY > valY)
            //                minY = valY;
            //        }
            //        points.Add(point);
            //        j++;
            //    }
            //    pointsList.Add(new CS.ChartPoints() { Points = points.ToArray(), LineColor = System.Windows.Media.Brushes.DarkBlue });
            //}

            if (this._currentEmittings != null && this._currentEmittings.Count > 0)
            {
                minX = int.MaxValue;
                maxX = int.MinValue;

                var firstEmession = ((EmittingViewModel)this._currentEmittings[0]);
                foreach (EmittingViewModel emitting in this._currentEmittings)
                {
                    if (emitting.Spectrum != null)
                    {
                        double constStep = 0;

                        constStep = -10 * Math.Log10(emitting.Spectrum.SpectrumSteps_kHz  / firstEmession.Spectrum.SpectrumSteps_kHz);

                        var count = emitting.Spectrum.Levels_dBm.Count();
                        var points = new List<Point>();

                        for (int i = 0; i < count; i++)
                        {
                            var valX = (emitting.Spectrum.SpectrumStartFreq_MHz * 1000000 + emitting.Spectrum.SpectrumSteps_kHz * 1000 * i) / 1000000;
                            var valY = (double)emitting.Spectrum.Levels_dBm[i] + constStep;

                            if (startFreq.HasValue && valX < startFreq.Value || stopFreq.HasValue && valX > stopFreq.Value)
                                continue;

                            var point = new Point
                            {
                                X = valX,
                                Y = valY
                            };

                            if (maxX < valX)
                                maxX = valX;
                            if (minX > valX)
                                minX = valX;
                            if (maxY < valY)
                                maxY = valY;
                            if (minY > valY)
                                minY = valY;

                            points.Add(point);
                        }

                        pointsList.Add(new CS.ChartPoints() { Points = points.ToArray(), LineColor = System.Windows.Media.Brushes.DarkRed });

                        if (this._currentEmittings.Count == 1)
                        {
                            if (emitting.Spectrum.T1 != 0)
                            {
                                var val = (emitting.Spectrum.SpectrumStartFreq_MHz * 1000000 + emitting.Spectrum.SpectrumSteps_kHz * 1000 * emitting.Spectrum.T1) / 1000000;
                                linesList.Add(new CS.ChartLine() { Point = new Point { X = val, Y = 0 }, LineColor = System.Windows.Media.Brushes.DarkRed, IsHorizontal = false, IsVertical = true, Name = Math.Round(val,6).ToString() });
                            }
                            if (emitting.Spectrum.T2 != 0)
                            {
                                var val = (emitting.Spectrum.SpectrumStartFreq_MHz * 1000000 + emitting.Spectrum.SpectrumSteps_kHz * 1000 * emitting.Spectrum.T2) / 1000000;
                                linesList.Add(new CS.ChartLine() { Point = new Point { X = val, Y = 0 }, LineColor = System.Windows.Media.Brushes.DarkRed, IsHorizontal = false, IsVertical = true, Name = Math.Round(val, 6).ToString() });
                            }
                            if (emitting.Spectrum.MarkerIndex != 0)
                            {
                                var val = (emitting.Spectrum.SpectrumStartFreq_MHz * 1000000 + emitting.Spectrum.SpectrumSteps_kHz * 1000 * emitting.Spectrum.MarkerIndex) / 1000000;
                                linesList.Add(new CS.ChartLine() { Point = new Point { X = val, Y = 0 }, LineColor = System.Windows.Media.Brushes.DarkRed, IsHorizontal = false, IsVertical = true, Name = Math.Round(val, 6).ToString() });
                            }
                        }
                        else
                        {
                            if (emitting.Spectrum.T1 != 0)
                                linesList.Add(new CS.ChartLine() { Point = new Point { X = (emitting.Spectrum.SpectrumStartFreq_MHz * 1000000 + emitting.Spectrum.SpectrumSteps_kHz * 1000 * emitting.Spectrum.T1) / 1000000, Y = 0 }, LineColor = System.Windows.Media.Brushes.DarkRed, IsHorizontal = false, IsVertical = true });
                            if (emitting.Spectrum.T2 != 0)
                                linesList.Add(new CS.ChartLine() { Point = new Point { X = (emitting.Spectrum.SpectrumStartFreq_MHz * 1000000 + emitting.Spectrum.SpectrumSteps_kHz * 1000 * emitting.Spectrum.T2) / 1000000, Y = 0 }, LineColor = System.Windows.Media.Brushes.DarkRed, IsHorizontal = false, IsVertical = true });
                            if (emitting.Spectrum.MarkerIndex != 0)
                                linesList.Add(new CS.ChartLine() { Point = new Point { X = (emitting.Spectrum.SpectrumStartFreq_MHz * 1000000 + emitting.Spectrum.SpectrumSteps_kHz * 1000 * emitting.Spectrum.MarkerIndex) / 1000000, Y = 0 }, LineColor = System.Windows.Media.Brushes.DarkRed, IsHorizontal = false, IsVertical = true });
                        }
                    }
                }
            }

            var preparedDataY = Environment.Utitlity.CalcLevelRange(minY, maxY);
            option.YTick = 20;
            option.YMin = preparedDataY.MinValue;
            option.YMax = preparedDataY.MaxValue;

            var preparedDataX = Environment.Utitlity.CalcFrequencyRange(minX, maxX, 20);
            option.XTick = preparedDataX.Step;
            //option.XMin = minX;
            //option.XMax = maxX;
            option.XMin = preparedDataX.MinValue;// + preparedDataX.Step;
            option.XMax = preparedDataX.MaxValue;// - preparedDataX.Step;

            var menuItems = new List<CS.ChartMenuItem>();
            menuItems.Add(new CS.ChartMenuItem() { Header = "Detailed for RefLevel on that Frequency", Name = "DetailForRefLevel" });
            menuItems.Add(new CS.ChartMenuItem() { Header = "View Station in ICSM", Name = "ViewStation" });

            option.PointsArray = pointsList.ToArray();
            option.LinesArray = linesList.ToArray();
            option.MenuItems = menuItems.ToArray();

            return option;
        }
        private CS.ChartOption GetChartLevelsDistrbutionOption()
        {
            var option = new CS.ChartOption
            {
                Title = "Levels Distribution",
                YLabel = "P",
                XLabel = "Levels",
                ChartType = CS.ChartType.Columns,
                XInnerTickCount = 10,
                YInnerTickCount = 10,
                YMin = 0,
                YMax = 1,
                XMin = -100,
                XMax = 0,
                YTick = 0.2,
                XTick = 10
            };


            if (this._currentEmitting != null)
            {
                var count = this._currentEmitting.LevelsDistribution.Levels.Length;
                var points = new List<Point>();
                var linesList = new List<CS.ChartLine>();
                var maxX = default(double);
                var minX = default(double);
                var maxY = default(double);
                var minY = default(double);

                int sumCount = this._currentEmitting.LevelsDistribution.Count.Sum();

                int j = 0;
                int startPos = -1;
                int stopPos = -1;

                for (int i = 0; i < count; i++)
                {
                    double valY = sumCount != 0 ? (double)this._currentEmitting.LevelsDistribution.Count[i] / sumCount : 0;

                    if (valY > 0 && startPos == -1)
                        startPos = i;

                    if (valY > 0)
                        stopPos = i;
                }
                //sumCount = 0;
                //for (int i = startPos; i <= stopPos; i++)
                //{
                //    sumCount += this._currentEmitting.LevelsDistribution.Count[i];
                //}
                for (int i = 0; i < count; i++)
                //for (int i = startPos; i <= stopPos; i++)
                {
                    var valX = this._currentEmitting.LevelsDistribution.Levels[i];
                    double valY = sumCount != 0 ? (double)this._currentEmitting.LevelsDistribution.Count[i] / sumCount : 0;

                    //if (i < startPos || i > stopPos)
                    //    continue;

                    var point = new Point
                    {
                        X = valX,
                        Y = valY
                    };
                    if (j == 0)
                    {
                        maxX = valX;
                        minX = valX;
                        maxY = valY;
                        minY = valY;

                    }
                    else
                    {
                        if (maxX < valX)
                            maxX = valX;
                        if (minX > valX)
                            minX = valX;
                        if (maxY < valY)
                            maxY = valY;
                        if (minY > valY)
                            minY = valY;
                    }
                    points.Add(point);
                    j++;
                }

                if (this._currentEmitting.ReferenceLevel_dBm != IM.NullD)
                {
                    linesList.Add(new CS.ChartLine() { Point = new Point { X = this._currentEmitting.ReferenceLevel_dBm, Y = 0 }, LineColor = System.Windows.Media.Brushes.DarkRed, IsHorizontal = false, IsVertical = true });

                    if (this._currentEmitting.ReferenceLevel_dBm < minX)
                        minX = this._currentEmitting.ReferenceLevel_dBm;

                    if (this._currentEmitting.ReferenceLevel_dBm > maxX)
                        maxX = this._currentEmitting.ReferenceLevel_dBm;
                }

                if (minX == maxX)
                {
                    minX = minX - 1;
                    maxX = maxX + 1;
                }

                var preparedDataX = Environment.Utitlity.CalcFrequencyRange(minX, maxX, 6);
                option.XTick = preparedDataX.Step;
                option.XMin = preparedDataX.MinValue;
                option.XMax = preparedDataX.MaxValue;
                option.YMin = 0;
                option.YMax = maxY;
                option.YTick = 0.1; // Math.Round(maxY / 5, 3) != 0 ? Math.Round(maxY / 5, 3) : 1;
                option.Points = points.ToArray();
                option.LinesArray = linesList.ToArray();
            }

            return option;
        }
    }
}
