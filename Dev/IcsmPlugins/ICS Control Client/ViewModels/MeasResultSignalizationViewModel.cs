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

namespace XICSM.ICSControlClient.ViewModels
{
    public class CustomDataGridEmitting : DataGrid
    {
        public CustomDataGridEmitting()
        {
            this.SelectionChanged += CustomDataGrid_SelectionChanged;
        }

        void CustomDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.SelectedItemsList = this.SelectedItems;
        }
        #region SelectedItemsList

        public IList SelectedItemsList
        {
            get { return (IList)GetValue(SelectedItemsListProperty); }
            set { SetValue(SelectedItemsListProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemsListProperty = DependencyProperty.Register("SelectedItemsList", typeof(IList), typeof(CustomDataGridEmitting), new PropertyMetadata(null));

        #endregion
    }
    public class MeasResultSignalizationViewModel : WpfViewModelBase
    {
        private int _resultId;

        #region Current Objects
        private string _emittingCaption;
        private SDR.MeasurementResults _currentMeasResult;
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

        #region Commands
        public WpfCommand ZoomUndoCommand { get; set; }
        public WpfCommand ZoomDefaultCommand { get; set; }
        public WpfCommand DetailForRefLevelCommand { get; set; }
        public WpfCommand ViewStationCommand { get; set; }
        #endregion

        public MeasResultSignalizationViewModel(int resultId)
        {
            this._resultId = resultId;
            this._emittings = new EmittingDataAdapter();
            this._emittingWorkTimes = new EmittingWorkTimeDataAdapter();
            this.ZoomUndoCommand = new WpfCommand(this.OnZoomUndoCommand);
            this.ZoomDefaultCommand = new WpfCommand(this.OnZoomDefaultCommand);
            this.ReloadMeasResult();
            this.UpdateCurrentChartOption(null, null);
            this.UpdateCurrentChartLevelsDistrbutionOption();
        }
        public string EmittingCaption
        {
            get => this._emittingCaption;
            set => this.Set(ref this._emittingCaption, value);
        }
        public string RBW
        {
            get => this.GetCurrentRBWValue();
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
                //System.Windows.MessageBox.Show("Hi from " + value.Name + " x = " + _mouseClickPoint.X.ToString() + "; y = " + _mouseClickPoint.Y.ToString());
                if (value.Name == "DetailForRefLevel")
                    this.OnDetailForRefLevelCommand();
                if (value.Name == "ViewStation")
                    this.OnViewStationCommand();
            }
        }

        private void ReloadMeasResult()
        {
            _currentMeasResult = SVC.SdrnsControllerWcfClient.GetMeasurementResultByResId(_resultId, null, null);
            this._emittings.Source = this._currentMeasResult.Emittings;
            this.EmittingCaption = this.GetCurrentEmittingCaption();
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
        private void OnDetailForRefLevelCommand()
        {
            try
            {
                var measTask = SVC.SdrnsControllerWcfClient.GetMeasTaskById(_currentMeasResult.Id.MeasTaskId.Value);
                var stationData = new List<MeasStationsSignalization>();

                var freq = _mouseClickPoint.X;
                double lonSensor = 0;
                double latSensor = 0;

                if (this._currentMeasResult != null && this._currentMeasResult.LocationSensorMeasurement != null && this._currentMeasResult.LocationSensorMeasurement.Count() > 0)
                {
                    var _currentSensorLocation = this._currentMeasResult.LocationSensorMeasurement[this._currentMeasResult.LocationSensorMeasurement.Count() - 1];

                    if (_currentSensorLocation.Lon.HasValue && _currentSensorLocation.Lat.HasValue)
                    {
                        lonSensor = _currentSensorLocation.Lon.Value;
                        latSensor = _currentSensorLocation.Lat.Value;
                    }
                }

                if (measTask.RefSituation != null)
                {
                    foreach (var refSituation in measTask.RefSituation)
                    {
                        foreach (var refSignal in refSituation.ReferenceSignal)
                        {
                            if (refSignal.Frequency_MHz - refSignal.Bandwidth_kHz / 2000 <= freq && freq <= refSignal.Frequency_MHz + refSignal.Bandwidth_kHz / 2000)
                            {
                                if (!string.IsNullOrEmpty(refSignal.IcsmTable) && refSignal.IcsmId > 0)
                                {
                                    IMRecordset rs = new IMRecordset(refSignal.IcsmTable, IMRecordset.Mode.ReadOnly);
                                    rs.SetWhere("ID", IMRecordset.Operation.Eq, refSignal.IcsmId);
                                    rs.Select("NAME,STANDARD,STATUS,Position.LONGITUDE,Position.LATITUDE,AGL,POWER,BW,Owner.NAME");
                                    for (rs.Open(); !rs.IsEOF(); rs.MoveNext())
                                    {
                                        var measStationSignalization = new MeasStationsSignalization()
                                        {
                                            IcsmId = refSignal.IcsmId,
                                            IcsmTable = refSignal.IcsmTable,
                                            StationName = rs.GetS("NAME"),
                                            Standart = rs.GetS("STANDARD"),
                                            Status = rs.GetS("STATUS"),
                                            Lon = rs.GetD("Position.LONGITUDE"),
                                            Lat = rs.GetD("Position.LATITUDE"),
                                            Agl = rs.GetD("AGL"),
                                            Eirp = rs.GetD("POWER"),
                                            Bw = rs.GetD("BW"),
                                            Freq = refSignal.Frequency_MHz,
                                            Owner = rs.GetS("Owner.NAME"),
                                            RelivedLevel = refSignal.LevelSignal_dBm
                                        };

                                        if (measStationSignalization.Lon != IM.NullD && measStationSignalization.Lat != IM.NullD)
                                        {
                                            measStationSignalization.Distance = 111.315 * Math.Pow((Math.Pow((lonSensor - measStationSignalization.Lon) * Math.Cos(measStationSignalization.Lat * Math.PI / 180), 2) + Math.Pow((latSensor - measStationSignalization.Lat), 2)), 0.5);
                                        }
                                        stationData.Add(measStationSignalization);
                                    }
                                    if (rs.IsOpen())
                                        rs.Close();
                                    rs.Destroy();
                                }
                            }
                        }
                    }
                }

                //stationData.Add(new MeasStationsSignalization() { Agl = 55, Bw = 66, Distance = 100, Eirp = 5, Freq = 150, Lat = 50.50, Lon = 30.64, Owner = "Test", Standart = "qqq", Status = "aa", RelivedLevel = 2, StationName = "Station1" });
                //stationData.Add(new MeasStationsSignalization() { Agl = 55, Bw = 66, Distance = 100, Eirp = 5, Freq = 150, Lat = 50.60, Lon = 30.60, Owner = "Test", Standart = "qqq", Status = "aa", RelivedLevel = 2, StationName = "Station2" });
                //stationData.Add(new MeasStationsSignalization() { Agl = 55, Bw = 66, Distance = 100, Eirp = 5, Freq = 150, Lat = 50.70, Lon = 30.40, Owner = "Test", Standart = "qqq", Status = "aa", RelivedLevel = 2, StationName = "Station3" });
                //stationData.Add(new MeasStationsSignalization() { Agl = 55, Bw = 66, Distance = 100, Eirp = 5, Freq = 150, Lat = 50.30, Lon = 30.45, Owner = "Test", Standart = "qqq", Status = "aa", RelivedLevel = 2, StationName = "Station4" });
                //stationData.Add(new MeasStationsSignalization() { Agl = 55, Bw = 66, Distance = 100, Eirp = 5, Freq = 150, Lat = 50.20, Lon = 30.55, Owner = "Test", Standart = "qqq", Status = "aa", RelivedLevel = 2, StationName = "Station5" });
                if (stationData.Count == 0)
                {
                    System.Windows.MessageBox.Show("No Stations");
                    return;
                }


                var measTaskForm = new FM.MeasStationsSignalizationForm(stationData.ToArray(), this._currentMeasResult);
                measTaskForm.ShowDialog();
                measTaskForm.Dispose();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void OnViewStationCommand()
        {
            try
            {
                var stationData = new List<MeasStationsSignalization>();

                System.Windows.MessageBox.Show("Under construction!!!");
                return;

                var measTaskForm = new FM.MeasStationsSignalizationForm(stationData.ToArray(), this._currentMeasResult);
                measTaskForm.ShowDialog();
                measTaskForm.Dispose();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private string GetCurrentRBWValue()
        {
            if (_currentMeasResult.RefLevels == null)
                return "";
            else
            {
                string res = "";
                double rbw = _currentMeasResult.RefLevels.StepFrequency_Hz;

                if (rbw > 1000)
                    res = Math.Round(rbw, 1).ToString();
                else if (1000 > rbw && rbw < 100)
                    res = Math.Round(rbw, 2).ToString();
                else if (100 > rbw && rbw < 10)
                    res = Math.Round(rbw, 3).ToString();
                else if (10 > rbw && rbw < 1)
                    res = Math.Round(rbw, 4).ToString();

                return "RBW = " + res + " kHz";
            }
        }
        private string GetCurrentEmittingCaption()
        {
            return "Illegal emission (" + this._emittings.Count().ToString() + ")";
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

            if (_currentMeasResult.RefLevels == null || _currentMeasResult.RefLevels.levels == null)
                return option;

            var maxX = default(double);
            var minX = default(double);

            var maxY = default(double);
            var minY = default(double);

            var linesList = new List<CS.ChartLine>();
            var pointsList = new List<CS.ChartPoints>();
            {
                var count = _currentMeasResult.RefLevels.levels.Length;
                var points = new List<Point>();

                int j = 0;
                for (int i = 0; i < count; i++)
                {
                    var valX = (_currentMeasResult.RefLevels.StartFrequency_Hz + _currentMeasResult.RefLevels.StepFrequency_Hz * i) / 1000000;
                    var valY = _currentMeasResult.RefLevels.levels[i];

                    if (startFreq.HasValue && valX < startFreq.Value || stopFreq.HasValue && valX > stopFreq.Value)
                        continue;

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
                pointsList.Add(new CS.ChartPoints() { Points = points.ToArray(), LineColor = System.Windows.Media.Brushes.DarkBlue });
            }

            if (this._currentEmittings != null)
            {
                foreach (EmittingViewModel emitting in this._currentEmittings)
                {
                    if (emitting.Spectrum != null)
                    {
                        double constStep = 0;
                        if (Math.Abs(_currentMeasResult.RefLevels.StepFrequency_Hz - emitting.Spectrum.SpectrumSteps_kHz) > 0.01 && _currentMeasResult.RefLevels.StepFrequency_Hz != 0)
                        {
                            constStep = -10 * Math.Log10(emitting.Spectrum.SpectrumSteps_kHz * 1000 / _currentMeasResult.RefLevels.StepFrequency_Hz);
                        }

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

                        if (emitting.Spectrum.T1 != 0)
                            linesList.Add(new CS.ChartLine() { Point = new Point { X = (emitting.Spectrum.SpectrumStartFreq_MHz * 1000000 + emitting.Spectrum.SpectrumSteps_kHz * 1000 * emitting.Spectrum.T1) / 1000000, Y = 0 }, LineColor = System.Windows.Media.Brushes.DarkRed, IsHorizontal = false, IsVertical = true });
                        if (emitting.Spectrum.T2 != 0)
                            linesList.Add(new CS.ChartLine() { Point = new Point { X = (emitting.Spectrum.SpectrumStartFreq_MHz * 1000000 + emitting.Spectrum.SpectrumSteps_kHz * 1000 * emitting.Spectrum.T2) / 1000000, Y = 0 }, LineColor = System.Windows.Media.Brushes.DarkRed, IsHorizontal = false, IsVertical = true });
                        if (emitting.Spectrum.MarkerIndex != 0)
                            linesList.Add(new CS.ChartLine() { Point = new Point { X = (emitting.Spectrum.SpectrumStartFreq_MHz * 1000000 + emitting.Spectrum.SpectrumSteps_kHz * 1000 * emitting.Spectrum.MarkerIndex) / 1000000, Y = 0 }, LineColor = System.Windows.Media.Brushes.DarkRed, IsHorizontal = false, IsVertical = true });
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
                var count = this._currentEmitting.LevelsDistribution.Levels.Count();
                var points = new List<Point>();
                var maxX = default(double);
                var minX = default(double);
                var maxY = default(double);
                var minY = default(double);

                int sumCount = this._currentEmitting.LevelsDistribution.Count.Sum();

                int j = 0;
                //bool startPos = false;

                for (int i = 0; i < count; i++)
                {
                    var valX = this._currentEmitting.LevelsDistribution.Levels[i];
                    double valY = sumCount != 0 ? (double)this._currentEmitting.LevelsDistribution.Count[i] / sumCount : 0 ;

                    //if (valY == 0)
                    //    continue;

                    //if (valY == 0 && !startPos)
                    //    continue;
                    //else
                    //    startPos = true;

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
                option.YMax = 1;
                option.YTick = 0.2; // Math.Round(maxY / 5, 3) != 0 ? Math.Round(maxY / 5, 3) : 1;
                option.Points = points.ToArray();
            }

            return option;
        }
    }
}
