﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XICSM.ICSControlClient.Models.Views;
using XICSM.ICSControlClient.Environment.Wpf;
using XICSM.ICSControlClient.Models.WcfDataApadters;
using SVC = XICSM.ICSControlClient.WcfServiceClients;
using CS = XICSM.ICSControlClient.WpfControls.Charts;
using MP = XICSM.ICSControlClient.WpfControls.Maps;
using SDR = Atdi.AppServer.Contracts.Sdrns;
using System.Windows;
using FM = XICSM.ICSControlClient.Forms;
using ICSM;
using WCF = XICSM.ICSControlClient.WcfServiceClients;

namespace XICSM.ICSControlClient.ViewModels
{

    public class CreateMeasTaskViewModel : WpfViewModelBase
    {
        #region Current Objects
        private MeasTaskViewModel _currentMeasTask;
        private ShortSensorViewModel _currentShortSensor;
        private MP.MapDrawingData _currentMapData;
        #endregion

        public FM.MeasTaskForm _measTaskForm;
        private ShortSensorDataAdatper _shortSensors;

        #region Commands
        public WpfCommand CreateMeasTaskCommand { get; set; }
        #endregion

        public CreateMeasTaskViewModel()
        {
            this.CreateMeasTaskCommand = new WpfCommand(this.OnCreateMeasTaskCommand);
            this._shortSensors = new ShortSensorDataAdatper();
            this._currentMeasTask = new MeasTaskViewModel();
            this.SetDefaultVaues();
            this.ReloadShortSensors();
        }
        public MP.MapDrawingData CurrentMapData
        {
            get => this._currentMapData;
            set => this.Set(ref this._currentMapData, value);
        }
        public MeasTaskViewModel CurrentMeasTask
        {
            get => this._currentMeasTask;
            set => this.Set(ref this._currentMeasTask, value, () => { ReloadShortSensors(); RedrawMap(); });
        }
        public ShortSensorViewModel CurrentShortSensor
        {
            get => this._currentShortSensor;
            set => this.Set(ref this._currentShortSensor, value, RedrawMap);
        }
        #region Sources (Adapters)
        public ShortSensorDataAdatper ShortSensors => this._shortSensors;
        #endregion
        private void SetDefaultVaues()
        {
            this._currentMeasTask.MeasDtParamTypeMeasurements = SDR.MeasurementType.Level;
            this._currentMeasTask.MeasTimeParamListPerStart = DateTime.Today;
            this._currentMeasTask.MeasTimeParamListPerStop = DateTime.Today.AddDays(1);
            this._currentMeasTask.MeasTimeParamListTimeStart = DateTime.Today;
            this._currentMeasTask.MeasTimeParamListTimeStop = DateTime.Today.AddDays(1).AddMinutes(-1);
            this._currentMeasTask.MeasTimeParamListPerInterval = 600;
            this._currentMeasTask.MeasFreqParamRgL = 900;
            this._currentMeasTask.MeasFreqParamRgU = 1000;
            this._currentMeasTask.MeasFreqParamStep = 100;
            this._currentMeasTask.MeasFreqParamMode = SDR.FrequencyMode.FrequencyRange;
            this._currentMeasTask.MeasDtParamRBW = 3;
            this._currentMeasTask.MeasDtParamVBW = 3;
            this._currentMeasTask.MeasDtParamMeasTime = 0.001;
            this._currentMeasTask.MeasDtParamDetectType = SDR.DetectingType.Avarage;
            this._currentMeasTask.MeasDtParamRfAttenuation = 0;
            this._currentMeasTask.MeasDtParamPreamplification = 0;
            this._currentMeasTask.MeasOtherTypeSpectrumOccupation = SDR.SpectrumOccupationType.FreqChannelOccupation;
            this._currentMeasTask.MeasOtherLevelMinOccup = -75;
            this._currentMeasTask.MeasOtherSwNumber = 10;
            this._currentMeasTask.MeasOtherTypeSpectrumScan = SDR.SpectrumScanType.Sweep;
            this._currentMeasTask.ResultType = SDR.MeasTaskResultType.MeasurementResult;
            this._currentMeasTask.ExecutionMode = SDR.MeasTaskExecutionMode.Automatic;
            this._currentMeasTask.Task = SDR.MeasTaskType.Scan;
            this._currentMeasTask.DateCreated = DateTime.Now;
            this._currentMeasTask.CreatedBy = IM.ConnectedUser();
        }
        private void ReloadShortSensors()
        {
            var sdrSensors = SVC.SdrnsControllerWcfClient.GetShortSensors(new Atdi.AppServer.Contracts.DataConstraint());
            this._shortSensors.Source = sdrSensors;
        }
        private void OnCreateMeasTaskCommand(object parameter)
        {
            try
            {
                if (this._currentShortSensor == null)
                {
                    MessageBox.Show("Undefined sensor!");
                    return;
                }

                if (this._currentMeasTask.MeasTimeParamListPerStart > this._currentMeasTask.MeasTimeParamListPerStop)
                {
                    MessageBox.Show("Date Stop should be great of the Date Start!");
                    return;
                }
                if (this._currentMeasTask.MeasTimeParamListTimeStart > this._currentMeasTask.MeasTimeParamListTimeStop)
                {
                    MessageBox.Show("Time Stop should be great of the Time Start!");
                    return;
                }
                if (this._currentMeasTask.MeasTimeParamListPerInterval <= 0 || this._currentMeasTask.MeasTimeParamListPerInterval >= 3600)
                {
                    MessageBox.Show("Incorrect value Duration!");
                    return;
                }
                if (this._currentMeasTask.MeasFreqParamRgL > this._currentMeasTask.MeasFreqParamRgU)
                {
                    MessageBox.Show("Stop freq should be great of the Start freq!");
                    return;
                }
                if (this._currentMeasTask.MeasFreqParamRgL <= 1 || this._currentMeasTask.MeasFreqParamRgL >= 6000)
                {
                    MessageBox.Show("Incorrect value Start freq!");
                    return;
                }
                if (this._currentMeasTask.MeasFreqParamRgU <= 1 || this._currentMeasTask.MeasFreqParamRgU >= 6000)
                {
                    MessageBox.Show("Incorrect value Stop freq!");
                    return;
                }
                if (this._currentMeasTask.MeasFreqParamStep <= 0 || this._currentMeasTask.MeasFreqParamStep >= 20000)
                {
                    MessageBox.Show("Incorrect value Step whith!");
                    return;
                }
                if (this._currentMeasTask.MeasDtParamRBW <= 0.001 || this._currentMeasTask.MeasDtParamRBW >= 10000)
                {
                    MessageBox.Show("Incorrect value RBW!");
                    return;
                }
                if (this._currentMeasTask.MeasDtParamVBW <= 0.001 || this._currentMeasTask.MeasDtParamVBW >= 10000)
                {
                    MessageBox.Show("Incorrect value VBW!");
                    return;
                }
                if (this._currentMeasTask.MeasDtParamVBW > this._currentMeasTask.MeasDtParamRBW)
                {
                    MessageBox.Show("VBW should be great of the RBW!");
                    return;
                }
                if (this._currentMeasTask.MeasDtParamMeasTime < 0.001 || this._currentMeasTask.MeasDtParamMeasTime > 1)
                {
                    MessageBox.Show("Incorrect value Sweep time!");
                    return;
                }
                if (this._currentMeasTask.MeasDtParamRfAttenuation != 0 && this._currentMeasTask.MeasDtParamRfAttenuation != 10 && this._currentMeasTask.MeasDtParamRfAttenuation != 20 && this._currentMeasTask.MeasDtParamRfAttenuation != 30)
                {
                    MessageBox.Show("Incorrect value Attenuation!");
                    return;
                }
                if (this._currentMeasTask.MeasDtParamPreamplification != 0 && this._currentMeasTask.MeasDtParamPreamplification != 10 && this._currentMeasTask.MeasDtParamPreamplification != 20 && this._currentMeasTask.MeasDtParamPreamplification != 30)
                {
                    MessageBox.Show("Incorrect value Gain of preamplifier!");
                    return;
                }
                if (this._currentMeasTask.MeasOtherLevelMinOccup <= -130 || this._currentMeasTask.MeasOtherLevelMinOccup >= -30)
                {
                    MessageBox.Show("Incorrect value Level occupation!");
                    return;
                }
                if (this._currentMeasTask.MeasOtherSwNumber < 1 || this._currentMeasTask.MeasOtherSwNumber > 10000)
                {
                    MessageBox.Show("Incorrect value Number of scan!");
                    return;
                }

                var measTask = new SDR.MeasTask()
                {
                    Name = this._currentMeasTask.Name,
                    MaxTimeBs = this._currentMeasTask.MaxTimeBs,
                    MeasFreqParam = new SDR.MeasFreqParam()
                    {
                        Mode = this._currentMeasTask.MeasFreqParamMode,
                        RgL = this._currentMeasTask.MeasFreqParamRgL,
                        RgU = this._currentMeasTask.MeasFreqParamRgU,
                        Step = this._currentMeasTask.MeasFreqParamStep
                        //MeasFreqs = new SDR.MeasFreq()
                    },
                    //MeasLocParams = ,
                    MeasOther = new SDR.MeasOther()
                    {
                        TypeSpectrumOccupation = this._currentMeasTask.MeasOtherTypeSpectrumOccupation,
                        LevelMinOccup = this._currentMeasTask.MeasOtherLevelMinOccup,
                        SwNumber = this._currentMeasTask.MeasOtherSwNumber,
                        TypeSpectrumScan = this._currentMeasTask.MeasOtherTypeSpectrumScan,
                        NChenal = this._currentMeasTask.MeasOtherNChenal
                    },
                    //MeasSubTasks = ,
                    OrderId = this._currentMeasTask.OrderId,
                    Prio = this._currentMeasTask.Prio,
                    ResultType = this._currentMeasTask.ResultType,
                    Status = this._currentMeasTask.Status,
                    Type = this._currentMeasTask.Type,
                    MeasDtParam = new SDR.MeasDtParam()
                    {
                        TypeMeasurements = this._currentMeasTask.MeasDtParamTypeMeasurements,
                        RBW = this._currentMeasTask.MeasDtParamRBW,
                        VBW = this._currentMeasTask.MeasDtParamVBW,
                        MeasTime = this._currentMeasTask.MeasDtParamMeasTime,
                        DetectType = this._currentMeasTask.MeasDtParamDetectType,
                        RfAttenuation = this._currentMeasTask.MeasDtParamRfAttenuation,
                        Preamplification = this._currentMeasTask.MeasDtParamPreamplification,
                        Mode = this._currentMeasTask.MeasDtParamMode,
                        Demod = this._currentMeasTask.MeasDtParamDemod,
                        IfAttenuation = this._currentMeasTask.MeasDtParamIfAttenuation
                    },
                    MeasTimeParamList = new SDR.MeasTimeParamList()
                    {
                        PerStart = this._currentMeasTask.MeasTimeParamListPerStart,
                        PerStop = this._currentMeasTask.MeasTimeParamListPerStop,
                        TimeStart = this._currentMeasTask.MeasTimeParamListTimeStart,
                        TimeStop = this._currentMeasTask.MeasTimeParamListTimeStop,
                        Days = this._currentMeasTask.MeasTimeParamListDays,
                        PerInterval = this._currentMeasTask.MeasTimeParamListPerInterval
                    },

                    Stations = new SDR.MeasStation[] { new SDR.MeasStation()
                    {
                        StationId = new SDR.MeasStationIdentifier() { Value = SVC.SdrnsControllerWcfClient.GetSensorById(this._currentShortSensor.Id).Id.Value }
                    } } ,
                    //StationsForMeasurements = PreparedStationDataForMeasurements(tour, inspections),
                    Task = SDR.MeasTaskType.Scan,
                    ExecutionMode = SDR.MeasTaskExecutionMode.Automatic,
                    DateCreated = DateTime.Now,
                    CreatedBy = IM.ConnectedUser()
                };

                var measTaskId = WCF.SdrnsControllerWcfClient.CreateMeasTask(measTask);
                if (measTaskId == IM.NullI)
                {
                    throw new InvalidOperationException($"Could not create a meas task");
                }
                _measTaskForm.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private MP.MapDrawingDataPoint MakeDrawingPointForSensor(string status, double lon, double lat)
        {
            return new MP.MapDrawingDataPoint
            {
                Color = "A".Equals(status, StringComparison.OrdinalIgnoreCase) ? System.Windows.Media.Brushes.Blue : System.Windows.Media.Brushes.Silver,
                Fill = "A".Equals(status, StringComparison.OrdinalIgnoreCase) ? System.Windows.Media.Brushes.Blue : System.Windows.Media.Brushes.Silver,
                Location = new Models.Location(lon, lat)
            };
        }

        private void RedrawMap()
        {
            var data = new MP.MapDrawingData();
            var points = new List<MP.MapDrawingDataPoint>();

            if (this.CurrentShortSensor != null)
            {
                var svcSensor = SVC.SdrnsControllerWcfClient.GetSensorById(this.CurrentShortSensor.Id);
                if (svcSensor != null)
                {
                    var modelSensor = Mappers.Map(svcSensor);
                    if (modelSensor.Locations != null && modelSensor.Locations.Length > 0)
                    {
                        var sensorPoints = modelSensor.Locations
                            .Where(l => ("A".Equals(l.Status, StringComparison.OrdinalIgnoreCase)
                                    || "Z".Equals(l.Status, StringComparison.OrdinalIgnoreCase))
                                    && l.Lon.HasValue
                                    && l.Lat.HasValue)
                            .Select(l => this.MakeDrawingPointForSensor(l.Status, l.Lon.Value, l.Lat.Value))
                            .ToArray();

                        points.AddRange(sensorPoints);
                    }
                }
            }
            data.Points = points.ToArray();
            this.CurrentMapData = data;
        }
        
    }
}
