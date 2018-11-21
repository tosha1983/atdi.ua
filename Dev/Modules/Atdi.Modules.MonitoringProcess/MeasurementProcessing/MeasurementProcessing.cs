using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.SDR.Server.MeasurementProcessing.SingleHound;
using Atdi.SDR.Server.MeasurementProcessing.Measurement;

namespace Atdi.SDR.Server.MeasurementProcessing
{
    public class MeasurementProcessing
    {
        MeasError error;
        /// <summary>
        /// Start Meass 
        /// </summary>
        /// <param name="SDR">SDR - phisical measurements object BC60C</param>
        /// <param name="TaskSDR">Task for measurements</param>
        /// <param name="sensor">Sensore with parameters</param>
        /// <param name="lastSDRRes">Last result, can be null</param>
        /// <returns>Result of Measurements</returns>
        public object TaskProcessing(ISDR SDR, object taskSDR, object sensor, ref CirculatingData circulatingData, object lastSDRRes = null, ReferenceSignal[] referenceSignals = null)
        {
            error = MeasError.NoError; 
            TaskParameters taskParameters = new TaskParameters(taskSDR);
            LastResultParameters lastResultParameters = new LastResultParameters(lastSDRRes);
            SensorParameters sensorParameters = new SensorParameters(sensor);
            // инициализация и калибровка SDR если он выключен, настройка конфигураци измерения
            if (SDR.GetSDRState() == SDRState.NeedInitialization)
            {
                if (!SDR.Initiation()) { SDR.Close(); error = MeasError.Initialization; return null; }
                if (!SDR.Calibration()) { SDR.Close(); error = MeasError.Calibration; return null; }
            }
            SDRParameters sDRParameters = new SDRParameters(taskSDR);
            if (SDR.GetSDRState() == SDRState.ReadyForMeasurements)
            {
                if (SDR.GetLastTaskId() != taskParameters.TaskId)
                {
                    SDR.SetConfiguration(sDRParameters);
                    if (!SDR.SetConfiguration(sDRParameters)) { SDR.Close(); error = MeasError.ConfigurationSDR; return null; }
                }
            }
            else
            {
                return null;
            }
            DateTime Time_start = DateTime.Now; // замер времени измерения
            var MSDRRes = TaskProcessing(SDR, taskParameters, lastResultParameters, sensorParameters, ref circulatingData, referenceSignals);
            TimeSpan TimeMeasurements = DateTime.Now - Time_start;
            // формирование версии ответа 
            var Result = CreateResultCorrectVersion(MSDRRes, lastResultParameters); 
            // здесь будет обновление таска
            UpdateMeasTaskAfterMeasurements(taskSDR, TimeMeasurements);
            return Result;
        }
        private object TaskProcessing(ISDR SDR, TaskParameters taskParameters, LastResultParameters lastResultParameters, SensorParameters sensorParameters, ref CirculatingData circulatingData, ReferenceSignal[] referenceSignals)
        {
            MeasSdrResults_v2 MSDRRes = new MeasSdrResults_v2();
            try
            {
                // заполнение основных параметров таска

                MSDRRes.MeasSubTaskId = null;
                MSDRRes.MeasSubTaskStationId = 0;
                MSDRRes.MeasTaskId = new MeasTaskIdentifier();
                MSDRRes.MeasTaskId.Value = taskParameters.TaskId;
                MSDRRes.DataMeas = DateTime.Now;
                MSDRRes.MeasDataType = taskParameters.MeasurementType;
                MSDRRes.status = "N";
                // сканирование спектра 
                if (taskParameters.MeasurementType == MeasurementType.Level)
                {
                    // измерение уровня сигнала.
                    Trace trace = new Trace(SDR, taskParameters, sensorParameters, lastResultParameters);
                    if (trace.Frequencies_MHz != null) { MSDRRes.Freqs = trace.Frequencies_MHz; }
                    if (trace.Levels_dBm != null) { MSDRRes.Level = trace.Levels_dBm; }
                    if (trace.fSemples != null) { MSDRRes.FSemples = trace.fSemples; }
                }
                //занатие полос частот и каналов
                if (taskParameters.MeasurementType == MeasurementType.SpectrumOccupation)
                {
                    SpectrumOcupation spectrumOcupation = new SpectrumOcupation(SDR, taskParameters, sensorParameters, lastResultParameters);
                    if (spectrumOcupation.fSemplesResult != null) { MSDRRes.FSemples = spectrumOcupation.fSemplesResult;}
                    MSDRRes.NN = spectrumOcupation.NN;
                }
                if (taskParameters.MeasurementType == MeasurementType.BandwidthMeas)
                {
                    Bandwidth bandwidth = new Bandwidth(SDR, taskParameters, sensorParameters, lastResultParameters);
                    MSDRRes.FSemples = bandwidth.fSemples;
                    MSDRRes.ResultsBandwidth = bandwidth.measSdrBandwidthResults;
                }
                if (taskParameters.MeasurementType == MeasurementType.Level)
                {
                    //Bandwidth bandwidth = new Bandwidth(SDR, taskParameters, sensorParameters, lastResultParameters);
                    //MSDRRes.FSemples = bandwidth.fSemples;
                    //MSDRRes.ResultsBandwidth = bandwidth.measSdrBandwidthResults;
                }
            if (taskParameters.MeasurementType == MeasurementType.PICode)// пока с костылем работаем
                {
                    Signaling signaling = new Signaling(SDR, taskParameters, sensorParameters, lastResultParameters, ref circulatingData, referenceSignals);
                    MSDRRes.emittings = signaling.emittingsComplete;
                }
                if (taskParameters.MeasurementType == MeasurementType.SoundID)// пока с костылем работаем для тестов
                {
                IQStreem iQStreem = new IQStreem(SDR);
                }

             }
            catch { MSDRRes = null; error = MeasError.Measurements; }

            return MSDRRes;
        }
        private object CreateResultCorrectVersion(object measSdrResults, LastResultParameters lastResultParameters) // внедрен костыль временный
        {
            switch (lastResultParameters.APIversion)
            {
                case 1:
                    return measSdrResults as MeasSdrResults;
                case 2:
                    return measSdrResults as MeasSdrResults_v2;
                default:
                    return measSdrResults;
            }
        }
        #region UpdateTask
        /// <summary>
        /// Данный метод производит изменение статуса и изменение времени измерения. Это нужно для регулирования времени работы SDR
        /// </summary>
        /// <param name="TaskSDR">Объект</param>
        /// <param name="TimeMeasurements">Время затраченное на измерение</param>
        private void UpdateMeasTaskAfterMeasurements(object TaskSDR, TimeSpan TimeMeasurements)
        {
            if (TaskSDR is MeasSdrTask)
            {
                UpdateMeasTaskAfterMeasurements_v1(TaskSDR as MeasSdrTask, TimeMeasurements);
            }
        }
        private void UpdateMeasTaskAfterMeasurements_v1(MeasSdrTask TaskSDR, TimeSpan TimeMeasurements)
        {
            if ((TaskSDR.status != "O") && (TaskSDR.status != "RT") && (TaskSDR.MeasDataType != MeasurementType.PICode))// для онлайн измерений не производим коректировку
            {
                if (TaskSDR.NumberScanPerTask == -999)
                { // Нужно задать количество измерений
                    TaskSDR.NumberScanPerTask = (int)(TaskSDR.PerInterval / TimeMeasurements.TotalSeconds) - 1; // задали 
                    if (TaskSDR.NumberScanPerTask > 0) // коректировка измерений
                    {
                        Double LostTimeMeas = TimeMeasurements.TotalMilliseconds * TaskSDR.NumberScanPerTask; // оставшиеся время измерения милисек
                        Double LostTime = (TaskSDR.Time_stop - DateTime.Now).TotalMilliseconds; // оставшееся время 
                        if (LostTime > LostTimeMeas)
                        {
                            Double Time = (LostTime - LostTimeMeas) / TaskSDR.NumberScanPerTask - TimeMeasurements.TotalMilliseconds; // время до следующего измерения
                            TaskSDR.Time_start = DateTime.Now - TimeSpan.FromMilliseconds(Time);
                        }
                    }
                }
                else
                {
                    if (TaskSDR.NumberScanPerTask <= 1)
                    {//Изменить статус надо все конец
                        TaskSDR.status = "C";  //это костыль должен быть запуск функции MeasTaskSDR.UpdateStatus() 
                    }
                    else
                    {// статус менять не надо но возможна коректировка скорости измерения
                        TaskSDR.NumberScanPerTask = TaskSDR.NumberScanPerTask - 1;
                        Double LostTimeMeas = TimeMeasurements.TotalMilliseconds * TaskSDR.NumberScanPerTask; // оставшиеся время измерения милисек
                        Double LostTime = (TaskSDR.Time_stop - DateTime.Now).TotalMilliseconds; // оставшееся время 
                        if (LostTime > LostTimeMeas)
                        {
                            Double Time = (LostTime - LostTimeMeas) / TaskSDR.NumberScanPerTask - TimeMeasurements.TotalMilliseconds; // время до следующего измерения
                            TaskSDR.Time_start = DateTime.Now + TimeSpan.FromMilliseconds(Time);
                        }
                    }
                }
            }
            else if (TaskSDR.MeasDataType != MeasurementType.PICode)
            {
                TaskSDR.Time_start = DateTime.Now + TimeSpan.FromMilliseconds(20);
            }
        }
        #endregion
    }
    public class MeasSdrResults_v2:MeasSdrResults
    {
        public Emitting[] emittings;
    }

}
