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
        private SDR.MeasurementResults _currentMeasResult;
        private IList _currentEmittings;
        private EmittingViewModel _currentEmitting;
        private CS.ChartOption _currentChartOption;
        #endregion

        private EmittingDataAdapter _emittings;
        private EmittingWorkTimeDataAdapter _emittingWorkTimes;

        public EmittingDataAdapter Emittings => this._emittings;
        public EmittingWorkTimeDataAdapter EmittingWorkTimes => this._emittingWorkTimes;
        public MeasResultSignalizationViewModel(int resultId)
        {
            this._resultId = resultId;
            this._emittings = new EmittingDataAdapter();
            this._emittingWorkTimes = new EmittingWorkTimeDataAdapter();
            this.ReloadMeasResult();
            this.UpdateCurrentChartOption();
        }
        public CS.ChartOption CurrentChartOption
        {
            get => this._currentChartOption;
            set => this.Set(ref this._currentChartOption, value);
        }
        public IList CurrentEmittings
        {
            get => this._currentEmittings;
            set
            {
                this._currentEmittings = value;
                UpdateCurrentChartOption();
            }
        }
        public EmittingViewModel CurrentEmitting
        {
            get => this._currentEmitting;
            set => this.Set(ref this._currentEmitting, value, () => { ReloadEmittingWorkTime(); });
        }
        private void ReloadMeasResult()
        {
            _currentMeasResult = SVC.SdrnsControllerWcfClient.GetMeasurementResultByResId(_resultId);
            this._emittings.Source = _currentMeasResult.Emittings;
        }
        private void UpdateCurrentChartOption()
        {
            this.CurrentChartOption = this.GetChartOption();
        }
        private void ReloadEmittingWorkTime()
        {
            //var emitting = this._currentEmittings[0] as EmittingViewModel;
            this._emittingWorkTimes.Source = _currentEmitting.WorkTimes;
        }
        //private CS.ChartOption GetChartOption()
        //{
        //    var option = new CS.ChartOption
        //    {
        //        Title = "Ref level",
        //        YLabel = "Level (dBm)",
        //        XLabel = "Freq (Xz)",
        //        ChartType = CS.ChartType.Line,
        //        XInnerTickCount = 5,
        //        YInnerTickCount = 5,
        //        YMin = -120,
        //        YMax = -10,
        //        XMin = 900,
        //        XMax = 960,
        //        YTick = 10,
        //        XTick = 10
        //    };

        //    if (_currentMeasResult.Emittings == null)
        //        return option;

        //    var count = _currentMeasResult.Emittings.Length;
        //    var points = new Point[count];

        //    var maxX = default(double);
        //    var minX = default(double);

        //    var maxY = default(double);
        //    var minY = default(double);

        //    for (int i = 0; i < count; i++)
        //    {
        //        if (_currentMeasResult.Emittings[i].Spectrum != null)
        //        {
        //            var valX = _currentMeasResult.Emittings[i].Spectrum.SpectrumStartFreq_MHz + _currentMeasResult.Emittings[i].Spectrum.SpectrumSteps_kHz * i;
        //            var valY = _currentMeasResult.Emittings[i].Spectrum.Levels_dBm;
        //            var point = new Point
        //            {
        //                X = valX,
        //                Y = valY
        //            };
        //            if (i == 0)
        //            {
        //                maxX = valX;
        //                minX = valX;
        //                maxY = valY;
        //                minY = valY;
        //            }
        //            else
        //            {
        //                if (maxX < valX)
        //                    maxX = valX;
        //                if (minX > valX)
        //                    minX = valX;

        //                if (maxY < valY)
        //                    maxY = valY;
        //                if (minY > valY)
        //                    minY = valY;
        //            }
        //            points[i] = point;
        //        }
        //    }

        //    var preparedDataY = Environment.Utitlity.CalcLevelRange(minY, maxY);
        //    option.YTick = 10;
        //    option.YMin = preparedDataY.MinValue;
        //    option.YMax = preparedDataY.MaxValue;

        //    //var preparedDataX = Environment.Utitlity.CalcFrequencyRange(minX, maxX, 8);
        //    //option.XTick = preparedDataX.Step;
        //    //option.XMin = preparedDataX.MinValue;
        //    //option.XMax = preparedDataX.MaxValue;

        //    var preparedDataX = Environment.Utitlity.CalcLevelRange(minX - 5, maxX + 5);
        //    option.XTick = 50;
        //    option.XMin = preparedDataX.MinValue;
        //    option.XMax = preparedDataX.MaxValue;

        //    option.Points = points;

        //    return option;
        //}



        private CS.ChartOption GetChartOption()
        {
            var option = new CS.ChartOption
            {
                Title = "Ref level",
                YLabel = "Level (dBm)",
                XLabel = "Freq (Xz)",
                ChartType = CS.ChartType.Line,
                XInnerTickCount = 5,
                YInnerTickCount = 5,
                YMin = -120,
                YMax = -10,
                XMin = 900,
                XMax = 960,
                YTick = 10,
                XTick = 10
            };

            if (_currentMeasResult.RefLevels == null || _currentMeasResult.RefLevels.levels == null)
                return option;

            var pointsList = new List<CS.ChartPoints>();

            {
                var count = _currentMeasResult.RefLevels.levels.Length;
                var points = new Point[count];

                var maxX = default(double);
                var minX = default(double);

                var maxY = default(double);
                var minY = default(double);

                for (int i = 0; i < count; i++)
                {
                    var valX = _currentMeasResult.RefLevels.StartFrequency_Hz + _currentMeasResult.RefLevels.StepFrequency_Hz * i;
                    var valY = _currentMeasResult.RefLevels.levels[i];
                    var point = new Point
                    {
                        X = valX,
                        Y = valY
                    };
                    if (i == 0)
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
                    points[i] = point;
                }

                var preparedDataY = Environment.Utitlity.CalcLevelRange(minY, maxY);
                option.YTick = 20;
                option.YMin = preparedDataY.MinValue;
                option.YMax = preparedDataY.MaxValue;

                var preparedDataX = Environment.Utitlity.CalcFrequencyRange(minX, maxX, 50);
                option.XTick = preparedDataX.Step;
                option.XMin = preparedDataX.MinValue;
                option.XMax = preparedDataX.MaxValue;
                //option.Points = points;
                var chPoints = new CS.ChartPoints() { Points = points, LineColor = System.Windows.Media.Brushes.DarkGreen };
                pointsList.Add(chPoints);
            }

            if (this._currentEmittings != null)
            {
                foreach (EmittingViewModel emitting in this._currentEmittings)
                {
                    var count = _currentMeasResult.Emittings.Length;
                    var points = new Point[count];

                    var maxX = default(double);
                    var minX = default(double);

                    var maxY = default(double);
                    var minY = default(double);

                    for (int i = 0; i < count; i++)
                    {
                        if (emitting.Spectrum != null)
                        {
                            var valX = emitting.Spectrum.SpectrumStartFreq_MHz * 1000000 + emitting.Spectrum.SpectrumSteps_kHz * 1000 * i;
                            var valY = (double)emitting.Spectrum.Levels_dBm[i];
                            var point = new Point
                            {
                                X = valX,
                                Y = valY
                            };
                            if (i == 0)
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
                            points[i] = point;
                        }
                    }

                    //var preparedDataY = Environment.Utitlity.CalcLevelRange(minY, maxY);
                    //option.YTick = 10;
                    //option.YMin = preparedDataY.MinValue;
                    //option.YMax = preparedDataY.MaxValue;

                    //var preparedDataX = Environment.Utitlity.CalcFrequencyRange(minX, maxX, 8);
                    //option.XTick = preparedDataX.Step;
                    //option.XMin = preparedDataX.MinValue;
                    //option.XMax = preparedDataX.MaxValue;

                    var chPoints = new CS.ChartPoints() { Points = points, LineColor = System.Windows.Media.Brushes.DarkRed };
                    pointsList.Add(chPoints);
                }
            }





            option.PointsArray = pointsList.ToArray();

            return option;
        }
    }
}
