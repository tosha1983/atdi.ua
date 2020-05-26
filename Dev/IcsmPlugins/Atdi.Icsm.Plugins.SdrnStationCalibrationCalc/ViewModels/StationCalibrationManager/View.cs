﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Icsm.Plugins.Core;
using Atdi.Platform.Logging;
using System.Collections.Specialized;
using System.Collections;
using Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.ProjectManager.Queries;
using Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager.Adapters;
using Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager.Modifiers;
using Atdi.Platform.Cqrs;
using Atdi.Platform.Events;
using System.Windows.Forms;
using System.Windows;
using MP = Atdi.WpfControls.EntityOrm.Maps;
using Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager;


namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager
{

	[ViewXaml("StationCalibrationManager.xaml")]
	[ViewCaption("Station Calibration calc client")]
	public class View : ViewBase
    {
        private DateTime? _dateStartLoadDriveTest;
        private DateTime? _dateStopLoadDriveTest;


        private MP.MapDrawingData _currentMapData;
        private readonly IObjectReader _objectReader;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly ViewStarter _starter;
        private readonly IEventBus _eventBus;
        private readonly ILogger _logger;

        private SelectedStationType _selectedStationType;
        private MethodParamsCalculationModel _methodParamsCalculationModel;


        private ParamsCalculationModel _currentParamsCalculationModel;
        private StationMonitoringModel _currentStationMonitoringModel;


        public StationMonitoringDataAdapter StationMonitoringDataAdapter { get; set; }
        public ParametersDataAdapter  ParametersDataAdapter { get; set; }


        public ViewCommand StartStationCalibrationCommand { get; set; }
        public ViewCommand LoadDriveTestsCommand { get; set; }





        public View(
            ParametersDataAdapter  parametersDataAdapter,
            StationMonitoringDataAdapter  stationMonitoringDataAdapter,
            IObjectReader objectReader,
            ICommandDispatcher commandDispatcher,
            ViewStarter starter,
            IEventBus eventBus,
            ILogger logger)
        {
            _objectReader = objectReader;
            _commandDispatcher = commandDispatcher;
            _starter = starter;
            _eventBus = eventBus;
            _logger = logger;


            this.StartStationCalibrationCommand = new ViewCommand(this.OnStartStationCalibrationCommand);
            this.LoadDriveTestsCommand = new ViewCommand(this.OnLoadDriveTestsCommand);

            this._currentParamsCalculationModel = new ParamsCalculationModel();

            this.ParametersDataAdapter = parametersDataAdapter;
            this.StationMonitoringDataAdapter = stationMonitoringDataAdapter;



            this.ReloadData();
            this.RedrawMap();
        }

        public MP.MapDrawingData CurrentMapData
        {
            get => this._currentMapData;
            set => this.Set(ref this._currentMapData, value);
        }

        public SelectedStationType SelectedStationTypeVal
        {
            get => this._selectedStationType;
            set => this.Set(ref this._selectedStationType, value);
        }

        public MethodParamsCalculationModel MethodParamsCalculationModelVal
        {
            get => this._methodParamsCalculationModel;
            set => this.Set(ref this._methodParamsCalculationModel, value);
        }

        public IList<SelectedStationType> SelectedStationTypeValues => Enum.GetValues(typeof(SelectedStationType)).Cast<SelectedStationType>().ToList();
        public IList<MethodParamsCalculationModel> MethodParamsCalculationModelValues => Enum.GetValues(typeof(MethodParamsCalculationModel)).Cast<MethodParamsCalculationModel>().ToList();

        private void RedrawMap()
        {
            var data = new MP.MapDrawingData();
            var points = new List<MP.MapDrawingDataPoint>();



            data.Points = points.ToArray();
            this.CurrentMapData = data;
        }

        public DateTime? DateStartLoadDriveTest
        {
            get => this._dateStartLoadDriveTest;
            set => this.Set(ref this._dateStartLoadDriveTest, value);
        }
        public DateTime? DateStopLoadDriveTest
        {
            get => this._dateStopLoadDriveTest;
            set => this.Set(ref this._dateStopLoadDriveTest, value);
        }

        public StationMonitoringModel CurrentStationMonitoringModel
        {
            get => this._currentStationMonitoringModel;
            set => this.Set(ref this._currentStationMonitoringModel, value, () => { this.OnChangedCurrentResMeas(value); });
        }

        public ParamsCalculationModel CurrentParamsCalculation
        {
            get => this._currentParamsCalculationModel;
            set => this.Set(ref this._currentParamsCalculationModel, value, () => { this.OnChangedParamsCalculation(value); });
        }

        private void OnChangedCurrentResMeas(StationMonitoringModel resMeas)
        {

        }

        
        private void OnChangedParamsCalculation(ParamsCalculationModel  paramsCalculationModel)
        {

        }

        public StationMonitoringModel ReadStationMonitoring(long id)
        {
            var resMeas = _objectReader
                .Read<StationMonitoringModel>()
                .By(new StationMonitoringModelById() 
                {
                     Id = id
                });
            return resMeas;
        }

        private void ReloadData()
        {
            this.DateStartLoadDriveTest = DateTime.Now.AddDays(-30);
            this.DateStopLoadDriveTest = DateTime.Now;
        }



		public override void Dispose()
		{

        }


        private void OnLoadDriveTestsCommand(object parameter)
        {
            this.StationMonitoringDataAdapter.StartDateTime = this.DateStartLoadDriveTest.Value;
            this.StationMonitoringDataAdapter.StopDateTime = this.DateStopLoadDriveTest.Value;
            this.StationMonitoringDataAdapter.Refresh();
        }



        private void OnStartStationCalibrationCommand(object parameter)
        {
            // тестовая проверка
            var parameterCalculationModifier = new CreateParamsCalculation()
            {
                AltitudeStation = CurrentParamsCalculation.AltitudeStation,
                AzimuthStation = CurrentParamsCalculation.AzimuthStation,
                TaskId = 1,
                CorrelationThresholdHard = CurrentParamsCalculation.CorrelationThresholdHard,
                CascadeTuning = CurrentParamsCalculation.CascadeTuning,
                CoordinatesStation = CurrentParamsCalculation.CoordinatesStation,
                CorrelationDistance_m = CurrentParamsCalculation.CorrelationDistance_m,
                CorrelationThresholdWeak = CurrentParamsCalculation.CorrelationThresholdWeak,
                Delta_dB = CurrentParamsCalculation.Delta_dB,
                Detail = CurrentParamsCalculation.Detail,
                DetailOfCascade = CurrentParamsCalculation.DetailOfCascade,
                DistanceAroundContour_km = CurrentParamsCalculation.DistanceAroundContour_km,
                InfocMeasResults = CurrentParamsCalculation.InfocMeasResults,
                MaxAntennasPatternLoss_dB = CurrentParamsCalculation.MaxAntennasPatternLoss_dB,
                MaxDeviationAltitudeStation_m = CurrentParamsCalculation.MaxDeviationAltitudeStation_m,
                MaxDeviationAzimuthStation_deg = CurrentParamsCalculation.MaxDeviationAzimuthStation_deg,
                MaxDeviationCoordinatesStation_m = CurrentParamsCalculation.MaxDeviationCoordinatesStation_m,
                MaxDeviationTiltStation_deg = CurrentParamsCalculation.MaxDeviationTiltStation_deg,
                MaxRangeMeasurements_dBmkV = CurrentParamsCalculation.MaxRangeMeasurements_dBmkV,
                Method = CurrentParamsCalculation.Method,
                MinNumberPointForCorrelation = CurrentParamsCalculation.MinNumberPointForCorrelation,
                MinRangeMeasurements_dBmkV = CurrentParamsCalculation.MinRangeMeasurements_dBmkV,
                NumberCascade = CurrentParamsCalculation.NumberCascade,
                PowerStation = CurrentParamsCalculation.PowerStation,
                ShiftAltitudeStationMax_m = CurrentParamsCalculation.ShiftAltitudeStationMax_m,
                ShiftAltitudeStationMin_m = CurrentParamsCalculation.ShiftAltitudeStationMin_m,
                ShiftAltitudeStationStep_m = CurrentParamsCalculation.ShiftAltitudeStationStep_m,
                ShiftAzimuthStationMax_deg = CurrentParamsCalculation.ShiftAzimuthStationMax_deg,
                ShiftAzimuthStationMin_deg = CurrentParamsCalculation.ShiftAzimuthStationMin_deg,
                ShiftAzimuthStationStep_deg = CurrentParamsCalculation.ShiftAzimuthStationStep_deg,
                ShiftCoordinatesStationStep_m = CurrentParamsCalculation.ShiftCoordinatesStationStep_m,
                ShiftCoordinatesStation_m = CurrentParamsCalculation.ShiftCoordinatesStation_m,
                ShiftPowerStationMax_dB = CurrentParamsCalculation.ShiftPowerStationMax_dB,
                ShiftPowerStationMin_dB = CurrentParamsCalculation.ShiftPowerStationMin_dB,
                ShiftPowerStationStep_dB = CurrentParamsCalculation.ShiftPowerStationStep_dB,
                ShiftTiltStationMax_deg = CurrentParamsCalculation.ShiftTiltStationMax_deg,
                ShiftTiltStationMin_deg = CurrentParamsCalculation.ShiftTiltStationMin_deg,
                ShiftTiltStationStep_deg = CurrentParamsCalculation.ShiftTiltStationStep_deg,
                StationIds = CurrentParamsCalculation.StationIds,
                TiltStation = CurrentParamsCalculation.TiltStation,
                TrustOldResults = CurrentParamsCalculation.TrustOldResults,
                UseMeasurementSameGSID = CurrentParamsCalculation.UseMeasurementSameGSID
            };
            _commandDispatcher.Send(parameterCalculationModifier);
        }

        //private void OnCreatedCreateParamsCalculationHandle(Events.OnCreatedParamsCalculation data)
        //{

        //}
    }
}
