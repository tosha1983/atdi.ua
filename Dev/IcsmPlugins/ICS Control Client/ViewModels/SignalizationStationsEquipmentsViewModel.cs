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
    public class CustomDataGridEquipments : DataGrid
    {
        public CustomDataGridEquipments()
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

        public static readonly DependencyProperty SelectedItemsListProperty = DependencyProperty.Register("SelectedItemsList", typeof(IList), typeof(CustomDataGridEquipments), new PropertyMetadata(null));

        #endregion
    }
    public class SignalizationStationsEquipmentsViewModel : WpfViewModelBase, IDisposable
    {
        private EmittingViewModel _currentEmitting;
        private StationsEquipmentViewModel _currentEquipment;
        private IList _currentEquipments;
        private CS.ChartOption _currentChartOption;
        private SDR.MeasurementResults _currentMeasResult;

        private double[] _selectedRangeX;
        private Stack<double[]> _zoomHistory = new Stack<double[]>();

        private StationsEquipment[] _stationData;
        private FM.SignalizationStationsEquipments _form;

        private EmittingViewModel[] _emittings;
        private StationsEquipmentDataAdapter _equipments;
        public EmittingViewModel[] Emittings => this._emittings;
        public StationsEquipmentDataAdapter Equipments => this._equipments;

        #region Commands
        public WpfCommand ZoomUndoCommand { get; set; }
        public WpfCommand ZoomDefaultCommand { get; set; }

        #endregion

        public SignalizationStationsEquipmentsViewModel(StationsEquipment[] stationData, EmittingViewModel[] emittingsData, SDR.MeasurementResults measResult, Stack<double[]> zoomHistory, double[] selectedRangeX, FM.SignalizationStationsEquipments form)
        {
            this._currentMeasResult = measResult;
            this._stationData = stationData;
            this._emittings = emittingsData;
            this._zoomHistory = zoomHistory;
            this._selectedRangeX = selectedRangeX;
            this._form = form;
            this._equipments = new StationsEquipmentDataAdapter();
            this.ZoomUndoCommand = new WpfCommand(this.OnZoomUndoCommand);
            this.ZoomDefaultCommand = new WpfCommand(this.OnZoomDefaultCommand);
            this.ReloadData();
        }
        public EmittingViewModel CurrentEmitting
        {
            get => this._currentEmitting;
            set => this.Set(ref this._currentEmitting, value, () => { this.UpdateChart(); });
        }
        public StationsEquipmentViewModel CurrentEquipment
        {
            get => this._currentEquipment;
            set => this.Set(ref this._currentEquipment, value);
        }
        public IList CurrentEquipments
        {
            get => this._currentEquipments;
            set
            {
                this._currentEquipments = value;
                this.UpdateChart();
            }
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
                    UpdateCurrentChartOption(this._selectedRangeX[0], this._selectedRangeX[1]);
                }
            }
        }

        private string _emittingCaption;
        public string EmittingCaption
        {
            get => this._emittingCaption;
            set => this.Set(ref this._emittingCaption, value);
        }

        private string _equipmentCaption;
        public string EquipmentCaption
        {
            get => this._equipmentCaption;
            set => this.Set(ref this._equipmentCaption, value);
        }

        private string _rbw = string.Empty;
        public string RBW
        {
            get => this._rbw;
            set => this.Set(ref this._rbw, value);
        }
        public CS.ChartOption CurrentChartOption
        {
            get => this._currentChartOption;
            set => this.Set(ref this._currentChartOption, value);
        }
        private void ReloadData()
        {
            this.RBW = this.GetCurrentRBWValue();
            this.UpdateChart();
            this._equipments.Source = this._stationData;
            this.EmittingCaption = this.GetCurrentEmittingCaption();
            this.EquipmentCaption = this.GetCurrentEquipmentCaption();

        }
        private void UpdateChart()
        {
            if (this._selectedRangeX != null && this._selectedRangeX.Count() == 2)
                this.UpdateCurrentChartOption(this._selectedRangeX[0], this._selectedRangeX[1]);
            else
                this.UpdateCurrentChartOption(null, null);
        }
        private string GetCurrentEmittingCaption()
        {
            return $"Emissions ({this._emittings.Count()})";
        }
        private string GetCurrentEquipmentCaption()
        {
            return $"Equipments ({this._equipments.Count()})";
        }
        private void OnZoomUndoCommand(object parameter)
        {
            if (_zoomHistory.Count > 0)
            {
                _zoomHistory.Pop();
                if (_zoomHistory.Count > 0)
                {
                    var lastZoom = _zoomHistory.Peek();
                    UpdateCurrentChartOption(lastZoom[0], lastZoom[1]);
                    this._selectedRangeX = lastZoom;
                }
                else
                {
                    UpdateCurrentChartOption(null, null);
                    this._selectedRangeX = null;
                }
            }
            else
            {
                UpdateCurrentChartOption(null, null);
                this._selectedRangeX = null;
            }
        }
        private void OnZoomDefaultCommand(object parameter)
        {
            this._selectedRangeX = null;
            _zoomHistory.Clear();
            UpdateCurrentChartOption(null, null);
        }
        private void UpdateCurrentChartOption(double? startFreq, double? stopFreq)
        {
            this.CurrentChartOption = this.GetChartOption(startFreq, stopFreq);
        }
        private CS.ChartOption GetChartOption(double? startFreq, double? stopFreq)
        {
            var option = new CS.ChartOption
            {
                Title = "Level",
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
                UseZoom = true,
                IsEnableSaveToFile = true
            };

            if (_currentMeasResult.RefLevels == null || _currentMeasResult.RefLevels.levels == null)
                return option;

            var maxX = default(double);
            var minX = default(double);

            var maxY = default(double);
            var minY = default(double);
            double level_max_dBm = double.MinValue;

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
                //pointsList.Add(new CS.ChartPoints() { Points = points.ToArray(), LineColor = System.Windows.Media.Brushes.DarkBlue });
            }

            if (this._currentEmitting != null)
            {
                if (this._currentEmitting.Spectrum != null)
                {
                    double constStep = 0;
                    if (Math.Abs(_currentMeasResult.RefLevels.StepFrequency_Hz - this._currentEmitting.Spectrum.SpectrumSteps_kHz) > 0.01 && _currentMeasResult.RefLevels.StepFrequency_Hz != 0)
                    {
                        constStep = -10 * Math.Log10(this._currentEmitting.Spectrum.SpectrumSteps_kHz * 1000 / _currentMeasResult.RefLevels.StepFrequency_Hz);
                    }

                    var count = this._currentEmitting.Spectrum.Levels_dBm.Count();
                    var points = new List<Point>();

                    for (int i = 0; i < count; i++)
                    {
                        var valX = (this._currentEmitting.Spectrum.SpectrumStartFreq_MHz * 1000000 + this._currentEmitting.Spectrum.SpectrumSteps_kHz * 1000 * i) / 1000000;
                        var valY = (double)this._currentEmitting.Spectrum.Levels_dBm[i] + constStep;

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
                        if (level_max_dBm < valY)
                            level_max_dBm = valY;

                        points.Add(point);
                    }

                    pointsList.Add(new CS.ChartPoints() { Points = points.ToArray(), LineColor = System.Windows.Media.Brushes.DarkRed });

                    if (this._currentEmitting.Spectrum.T1 != 0)
                    {
                        var val = (this._currentEmitting.Spectrum.SpectrumStartFreq_MHz * 1000000 + this._currentEmitting.Spectrum.SpectrumSteps_kHz * 1000 * this._currentEmitting.Spectrum.T1) / 1000000;
                        linesList.Add(new CS.ChartLine() { Point = new Point { X = val, Y = 0 }, LineColor = System.Windows.Media.Brushes.DarkRed, IsHorizontal = false, IsVertical = true, Name = Math.Round(val, 6).ToString(), LabelLeft = 5, LabelTop = -25 });
                    }
                    if (this._currentEmitting.Spectrum.T2 != 0)
                    {
                        var val = (this._currentEmitting.Spectrum.SpectrumStartFreq_MHz * 1000000 + this._currentEmitting.Spectrum.SpectrumSteps_kHz * 1000 * this._currentEmitting.Spectrum.T2) / 1000000;
                        linesList.Add(new CS.ChartLine() { Point = new Point { X = val, Y = 0 }, LineColor = System.Windows.Media.Brushes.DarkRed, IsHorizontal = false, IsVertical = true, Name = Math.Round(val, 6).ToString(), LabelLeft = 5, LabelTop = -45 });
                    }
                    if (this._currentEmitting.Spectrum.MarkerIndex != 0)
                    {
                        var val = (this._currentEmitting.Spectrum.SpectrumStartFreq_MHz * 1000000 + this._currentEmitting.Spectrum.SpectrumSteps_kHz * 1000 * this._currentEmitting.Spectrum.MarkerIndex) / 1000000;
                        linesList.Add(new CS.ChartLine() { Point = new Point { X = val, Y = 0 }, LineColor = System.Windows.Media.Brushes.DarkRed, IsHorizontal = false, IsVertical = true, Name = Math.Round(val, 6).ToString(), LabelLeft = 5, LabelTop = -35 });
                    }
                }
            }

            if (this._currentEquipments != null)
            {
                foreach (StationsEquipmentViewModel equipment in this._currentEquipments)
                {
                    if (equipment.Freq != null && equipment.Loss != null && equipment.Freq.Length > 0 && equipment.Loss.Length > 0 && equipment.Freq.Length == equipment.Loss.Length)
                    {
                        var count = equipment.Loss.Length;
                        var points = new List<Point>();

                        for (int i = 0; i < count; i++)
                        {
                            var valX = equipment.Freq_MHz - equipment.Freq[i];
                            var valY = level_max_dBm - equipment.Loss[i];

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
                            if (level_max_dBm < valY)
                                level_max_dBm = valY;

                            points.Add(point);
                        }

                        pointsList.Add(new CS.ChartPoints() { Points = points.ToArray(), LineColor = System.Windows.Media.Brushes.DarkBlue });
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

            option.PointsArray = pointsList.ToArray();
            option.LinesArray = linesList.ToArray();

            return option;
        }
        private string GetCurrentRBWValue()
        {
            if (_currentMeasResult.RefLevels == null)
            {
                return "RBW = (unknown) kHz";
            }

            string res = "";
            double rbw = _currentMeasResult.RefLevels.StepFrequency_Hz / 1000;

            if (rbw > 1000)
                res = Math.Round(rbw, 1).ToString();
            else if (1000 > rbw && rbw > 100)
                res = Math.Round(rbw, 2).ToString();
            else if (100 > rbw && rbw > 10)
                res = Math.Round(rbw, 3).ToString();
            else // if (10 > rbw && rbw > 1)
                res = Math.Round(rbw, 4).ToString();

            return "RBW = " + res + " kHz";
        }

        public void Dispose()
        {
            _form = null;
        }
    }
}
