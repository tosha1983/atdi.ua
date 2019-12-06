﻿using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XICSM.ICSControlClient.Models;
using XICSM.ICSControlClient.Models.Views;
using XICSM.ICSControlClient.Environment.Wpf;
using CS = XICSM.ICSControlClient.WpfControls.Charts;
using XICSM.ICSControlClient.ViewModels;
using SDR = Atdi.Contracts.WcfServices.Sdrn.Server;

namespace XICSM.ICSControlClient.ViewModels
{
    public class GrapficViewModel : WpfViewModelBase
    {
        private SDR.MeasurementResults _measResult;
        private GeneralResultViewModel _generalResult;
        private SDR.MeasurementType _measType;
        private CS.ChartOption _currentChartOption;

        public GrapficViewModel(SDR.MeasurementType measType, SDR.MeasurementResults measResult, GeneralResultViewModel generalResult)
        {
            _measResult = measResult;
            _generalResult = generalResult;
            _measType = measType;
            this._currentChartOption = this.GetDefaultChartOption();
            UpdateCurrentChartOption();
        }
        public CS.ChartOption CurrentChartOption
        {
            get => this._currentChartOption;
            set => this.Set(ref this._currentChartOption, value);
        }
        private CS.ChartOption GetDefaultChartOption()
        {
            return new CS.ChartOption
            {
                ChartType = CS.ChartType.Line,
                XLabel = "Freq (Mhz)",
                XMin = 900,
                XMax = 960,
                XTick = 5,
                XInnerTickCount = 5,
                YLabel = "Level (dBm)",
                YMin = -120,
                YMax = -10,
                YTick = 10,
                YInnerTickCount = 5,
                Title = "Measurements",
                IsEnableSaveToFile = true
            };
        }
        private void UpdateCurrentChartOption()
        {
            CS.ChartOption chartOption;
            chartOption = this.GetChartOption();

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                this.CurrentChartOption = chartOption;
            }));
        }

        private CS.ChartOption GetChartOption()
        {
            if (_measType == SDR.MeasurementType.MonitoringStations)
            {
                return this.GetChartOptionByMonitoringStations();
            }
            else if (_measType == SDR.MeasurementType.SpectrumOccupation)
            {
                return this.GetChartOptionBySpectrumOccupation();
            }
            else if (_measType == SDR.MeasurementType.Level)
            {
                return this.GetChartOptionByLevel();
            }
            return this.GetDefaultChartOption();
        }

        private CS.ChartOption GetChartOptionByMonitoringStations()
        {
            var option = new CS.ChartOption
            {
                Title = "Monitoring Stations",
                YLabel = "Level (dBm)",
                XLabel = "Freq (Mhz)",
                ChartType = CS.ChartType.Line,
                XInnerTickCount = 5,
                YInnerTickCount = 5,
                YMin = -120,
                YMax = -10,
                XMin = 900,
                XMax = 960,
                YTick = 10,
                XTick = 10,
                IsEnableSaveToFile = true
            };

            var generalResult = this._generalResult;
            if (generalResult == null)
            {
                return option;
            }

            var spectrumLevels = generalResult.LevelsSpecrum;
            if (spectrumLevels == null || spectrumLevels.Length == 0)
            {
                return option;
            }

            var count = spectrumLevels.Length;
            var points = new Point[count];
            var maxX = default(double);
            var minX = default(double);

            var maxY = default(double);
            var minY = default(double);
            for (int i = 0; i < count; i++)
            {
                var valX = Convert.ToDouble(generalResult.SpecrumStartFreq + i * generalResult.SpecrumSteps / 1000);
                var valY = spectrumLevels[i];

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
            option.YTick = 10;
            option.YMax = preparedDataY.MaxValue;
            option.YMin = preparedDataY.MinValue;

            var preparedDataX = Environment.Utitlity.CalcFrequencyRange(minX, maxX, 8);
            if (preparedDataX.MinValue == 0 && preparedDataX.MaxValue == 0)
                option.XTick = 0;
            else
                option.XTick = preparedDataX.Step;
            option.XMin = preparedDataX.MinValue;
            option.XMax = preparedDataX.MaxValue;

            option.Points = points;

            return option;
        }

        private CS.ChartOption GetChartOptionBySpectrumOccupation()
        {
            var option = new CS.ChartOption
            {
                Title = "Spectrum Occupation",
                YLabel = "Occupation (%)",
                XLabel = "Freq (Mhz)",
                ChartType = CS.ChartType.Columns,
                XInnerTickCount = 5,
                YInnerTickCount = 5,
                YMin = 0,
                YMax = 100,
                XMin = 900,
                XMax = 960,
                YTick = 10,
                XTick = 10,
                IsEnableSaveToFile = true
            };

            if (_measResult.FrequenciesMeasurements == null)
                return option;

            var count = _measResult.FrequenciesMeasurements.Length;
            var points = new Point[count];
            var max = default(double);
            var min = default(double);
            for (int i = 0; i < count; i++)
            {
                var ms = _measResult.MeasurementsResults[i] as SDR.SpectrumOccupationMeasurementResult;
                var valX = _measResult.FrequenciesMeasurements[i].Freq;
                var valY = ms.Value ?? 0;
                var point = new Point
                {
                    X = valX,
                    Y = valY
                };
                if (i == 0)
                {
                    max = valX;
                    min = valX;
                }
                else
                {
                    if (max < valX)
                        max = valX;
                    if (min > valX)
                        min = valX;
                }
                points[i] = point;
            }

            var preparedDataX = Environment.Utitlity.CalcFrequencyRange(min, max, 6);
            if (preparedDataX.MinValue == 0 && preparedDataX.MaxValue == 0)
                option.XTick = 0;
            else
                option.XTick = preparedDataX.Step;
            option.XMin = preparedDataX.MinValue;
            option.XMax = preparedDataX.MaxValue;

            option.Points = points;
            return option;
        }

        private CS.ChartOption GetChartOptionByLevel()
        {
            var option = new CS.ChartOption
            {
                Title = "Level",
                YLabel = "Level (dBm)",
                XLabel = "Freq (Mhz)",
                ChartType = CS.ChartType.Line,
                XInnerTickCount = 5,
                YInnerTickCount = 5,
                YMin = -120,
                YMax = -10,
                XMin = 900,
                XMax = 960,
                YTick = 10,
                XTick = 10,
                IsEnableSaveToFile = true
            };

            var count = _measResult.FrequenciesMeasurements.Length;
            var points = new Point[count];

            var maxX = default(double);
            var minX = default(double);

            var maxY = default(double);
            var minY = default(double);

            for (int i = 0; i < count; i++)
            {
                var ms = _measResult.MeasurementsResults[i] as SDR.LevelMeasurementResult;
                var valX = _measResult.FrequenciesMeasurements[i].Freq;
                var valY = ms.Value ?? 0;
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
            option.YTick = 10;
            option.YMax = preparedDataY.MaxValue;
            option.YMin = preparedDataY.MinValue;

            var preparedDataX = Environment.Utitlity.CalcLevelRange(minX - 5, maxX + 5);
            option.XTick = 50;
            option.XMin = preparedDataX.MinValue;
            option.XMax = preparedDataX.MaxValue;

            option.Points = points;

            return option;
        }

    }
}