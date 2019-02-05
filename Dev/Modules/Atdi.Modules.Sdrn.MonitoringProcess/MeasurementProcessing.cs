// нет уже компоненты Atdi.AppServer.Contracts.Sdrns
// этот контракт нужно перенести в версию 2.0
/*
using System;
using System.Collections.Generic;
//using Atdi.AppServer.Contracts.Sdrns;
using Atdi.Modules.MonitoringProcess.Measurement;
using Atdi.Modules.MonitoringProcess;
using Atdi.Modules.MonitoringProcess.ProcessSignal;

namespace Atdi.Sdrn.Modules.MonitoringProcess
{
    public class MeasurementProcessing
    {
        public MeasError error;
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
            TaskParameters taskParameters = GetTaskParameters(taskSDR);
            LastResultParameters lastResultParameters = GetLastResultParameters(lastSDRRes);
            SensorParameters sensorParameters = GetSensorParameters(sensor);
            // инициализация и калибровка SDR если он выключен, настройка конфигураци измерения
            if (SDR.GetSDRState() == SDRState.NeedInitialization)
            {
                if (!SDR.Initiation()) { SDR.Close(); error = MeasError.Initialization; return null; }
                if (!SDR.Calibration()) { SDR.Close(); error = MeasError.Calibration; return null; }
            }
            SDRParameters sDRParameters = GetSDRParameters(taskSDR);
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
            //var MSDRRes = TaskProcessing(SDR, taskParameters, taskSDR as MeasSdrTask, lastResultParameters, sensorParameters, ref circulatingData, referenceSignals);
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
            //try
            //{
                // заполнение основных параметров таска

                MSDRRes.MeasSubTaskId = null;
                MSDRRes.MeasSubTaskStationId = 0;
                MSDRRes.MeasTaskId = new MeasTaskIdentifier { Value = taskParameters.TaskId };
                MSDRRes.DataMeas = DateTime.Now;
                MSDRRes.MeasDataType = GetMeasurementTypeFromMeasType(taskParameters.MeasurementType);
                MSDRRes.status = "N";
                // сканирование спектра 
                if (taskParameters.MeasurementType == MeasType.Level)
                {
                    // измерение уровня сигнала.
                    Trace trace = new Trace(SDR, taskParameters, sensorParameters, lastResultParameters);
                    if (trace.Frequencies_MHz != null) { MSDRRes.Freqs = trace.Frequencies_MHz; }
                    if (trace.Levels_dBm != null) { MSDRRes.Level = trace.Levels_dBm; }
                    if (trace.fSemples != null) { MSDRRes.FSemples = GetFemplesFromSemplesFreq(trace.fSemples); }
                }
                //занатие полос частот и каналов
                if (taskParameters.MeasurementType == MeasType.SpectrumOccupation)
                {
                    SpectrumOcupation spectrumOcupation = new SpectrumOcupation(SDR, taskParameters, sensorParameters, lastResultParameters);
                    if (spectrumOcupation.fSemplesResult != null) { MSDRRes.FSemples = GetFemplesFromSemplesFreq(spectrumOcupation.fSemplesResult); }
                    MSDRRes.NN = spectrumOcupation.NN;
                }
                if (taskParameters.MeasurementType == MeasType.BandwidthMeas)
                {
                    Bandwidth bandwidth = new Bandwidth(SDR, taskParameters, sensorParameters, lastResultParameters);
                    MSDRRes.FSemples = GetFemplesFromSemplesFreq(bandwidth.fSemples);
                    MSDRRes.ResultsBandwidth = GetMeasSdrBandwidthResultsFromMeasBandwidthResult( bandwidth.measSdrBandwidthResults);
                }
                if (taskParameters.MeasurementType == MeasType.Signaling)// пока не доделано и не дотестированно
                {
                    Signaling signaling = new Signaling(SDR, taskParameters, sensorParameters, lastResultParameters, ref circulatingData, referenceSignals);
                    MSDRRes.emittings = signaling.emittingsComplete;
                }
                if (taskParameters.MeasurementType == MeasType.IQReceive)// пока не доделано и не дотестированно
                {
                    IQStreem iQStreem = new IQStreem(SDR, taskParameters);
                }
                if (taskParameters.MeasurementType == MeasType.Timetimestamp)// пока не доделано и не дотестированно
                {
                    IQStreem iQStreem = new IQStreem(SDR, taskParameters);
                    GetTimeStamp getTimeStamp = new GetTimeStamp(null, 40000000, 200, GetTimeStamp.TypeTechnology.GSM);
                // функционал для тестирования 
                //    EstimationTimeDelayBetweenTwoTimestamp estimationTimeDelayBetweenTwoTimestamp = new EstimationTimeDelayBetweenTwoTimestamp(getTimeStamp.IQStreamTimeStampBloks, getTimeStamp.IQStreamTimeStampBloks);
                // 


                // функционал для тестирования 
                }


            //}
            //catch { MSDRRes = null; error = MeasError.Measurements; }

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

        #region ConvertationType
        private FSemples[] GetFemplesFromSemplesFreq(SemplFreq[] semplFreqs)
        {
            FSemples[] fSemples = new FSemples[semplFreqs.Length];
            for (int i = 0; i < semplFreqs.Length; i++)
            {
                fSemples[i] = new FSemples
                {
                    Freq = semplFreqs[i].Freq,
                    LeveldBm = semplFreqs[i].LeveldBm,
                    LeveldBmkVm = semplFreqs[i].LeveldBmkVm,
                    LevelMaxdBm = semplFreqs[i].LevelMaxdBm,
                    LevelMindBm = semplFreqs[i].LevelMindBm,
                    OcupationPt = semplFreqs[i].OcupationPt
                };
            }
            return fSemples;
        }
        private SemplFreq[] GetSemplesFreqFromFemples(FSemples[] fSemples)
        {
            if (fSemples is null) { return null;}
            SemplFreq[] semplFreqs = new SemplFreq[fSemples.Length];
            for (int i = 0; i < semplFreqs.Length; i++)
            {
                semplFreqs[i] = new SemplFreq
                {
                    Freq = fSemples[i].Freq,
                    LeveldBm = fSemples[i].LeveldBm,
                    LeveldBmkVm = fSemples[i].LeveldBmkVm,
                    LevelMaxdBm = fSemples[i].LevelMaxdBm,
                    LevelMindBm = fSemples[i].LevelMindBm,
                    OcupationPt = fSemples[i].OcupationPt
                };
            }
            return semplFreqs;
        }
        private DetectType GetDetectTypeFromDetectingType(DetectingType detectingType)
        {
            DetectType detectType;
            switch (detectingType)
            {
                case DetectingType.Avarage:
                    detectType = DetectType.Avarage;
                    break;
                case DetectingType.MaxPeak:
                    detectType = DetectType.MaxPeak;
                    break;
                case DetectingType.MinPeak:
                    detectType = DetectType.MinPeak;
                    break;
                case DetectingType.Peak:
                    detectType = DetectType.Peak;
                    break;
                default:
                    detectType = DetectType.Avarage;
                    break;
            }
            return (detectType);
        }
        private SOType GetSOTypeFromSpectrumOccupationType(SpectrumOccupationType spectrumOccupationType)
        {
            SOType sOType;
            switch (spectrumOccupationType)
            {
                case SpectrumOccupationType.FreqBandwidthOccupation:
                    sOType = SOType.FreqBandwidthOccupation;
                    break;
                case SpectrumOccupationType.FreqChannelOccupation:
                    sOType = SOType.FreqChannelOccupation;
                    break;
                 default:
                    sOType = SOType.FreqChannelOccupation;
                    break;
            }
            return (sOType);
        }
        private MeasType GetMeasTypeFromMeasurementType(MeasurementType measurementType)
        {
            MeasType measType;
            switch (measurementType)
            {
                case  MeasurementType.BandwidthMeas:
                    measType = MeasType.BandwidthMeas;
                    break;
                case MeasurementType.Level:
                    measType = MeasType.Level;
                    break;
                case MeasurementType.SpectrumOccupation:
                    measType = MeasType.SpectrumOccupation;
                    break;
                case MeasurementType.PICode:
                    measType = MeasType.Signaling;
                    break;
                case MeasurementType.SoundID:
                    measType = MeasType.IQReceive;
                    break;
                case MeasurementType.Program:
                    measType = MeasType.Timetimestamp;
                    break;
                default:
                    measType = MeasType.Level;
                    break;
            }
            return (measType);
        }
        private MeasurementType GetMeasurementTypeFromMeasType(MeasType measType)
        {
            MeasurementType measurementType;
            switch (measType)
            {
                case MeasType.BandwidthMeas:
                    measurementType = MeasurementType.BandwidthMeas;
                    break;
                case MeasType.Level:
                    measurementType = MeasurementType.Level;
                    break;
                case MeasType.SpectrumOccupation:
                    measurementType = MeasurementType.SpectrumOccupation;
                    break;
                case MeasType.Signaling:
                    measurementType = MeasurementType.PICode;
                    break;
                case MeasType.IQReceive :
                    measurementType = MeasurementType.SoundID;
                    break;
                default:
                    measurementType = MeasurementType.Level;
                    break;
            }
            return (measurementType);
        }
        private SpectrScanType GetSpectrScanTypeFromSpectrumScanType(SpectrumScanType spectrumScanType)
        {
            SpectrScanType spectrScanType;
            switch (spectrumScanType)
            {
                case SpectrumScanType.RealTime:
                    spectrScanType = SpectrScanType.RealTime;
                    break;
                case SpectrumScanType.Sweep:
                    spectrScanType = SpectrScanType.Sweep;
                    break;
                default:
                    spectrScanType = SpectrScanType.Sweep;
                    break;
            }
            return (spectrScanType);
        }
        private MeasSdrBandwidthResults GetMeasSdrBandwidthResultsFromMeasBandwidthResult(MeasBandwidthResult MeasBandwidthResult)
        {
            MeasSdrBandwidthResults measSdrBandwidthResults = new MeasSdrBandwidthResults
            {
                BandwidthkHz = MeasBandwidthResult.BandwidthkHz,
                MarkerIndex = MeasBandwidthResult.MarkerIndex,
                T1 = MeasBandwidthResult.T1,
                T2 = MeasBandwidthResult.T2,
                СorrectnessEstimations = MeasBandwidthResult.СorrectnessEstimations
            };
            return (measSdrBandwidthResults);
        }

        #endregion

        #region metodsTask
        private TaskParameters GetTaskParameters(object TaskSDR)
        {
            TaskParameters taskParameters = new TaskParameters();
            if (TaskSDR is MeasSdrTask)
            {
                taskParameters = FillingTaskParameters_v1(TaskSDR as MeasSdrTask);
            }
            return (taskParameters);

        }
        private TaskParameters FillingTaskParameters_v1(MeasSdrTask taskSDR)
        {
            TaskParameters taskParameters = new TaskParameters();
            if (taskSDR.MeasFreqParam.MeasFreqs != null)
            {
                taskParameters.List_freq_CH.Clear();
                for (int i = 0; i < taskSDR.MeasFreqParam.MeasFreqs.Length; i++)
                { double freq = taskSDR.MeasFreqParam.MeasFreqs[i].Freq; taskParameters.List_freq_CH.Add(freq); }
            }
            if ((taskSDR.MeasFreqParam.RgL == null) || (taskSDR.MeasFreqParam.RgU == null)) // если вдруг отсутвуют начало и конец
            {
                if (taskParameters.List_freq_CH != null)
                {
                    if (taskParameters.List_freq_CH.Count > 0)
                    {
                        taskParameters.List_freq_CH.Sort();
                        taskParameters.MinFreq_MHz = taskParameters.List_freq_CH[0];
                        taskParameters.MaxFreq_MHz = taskParameters.List_freq_CH[taskParameters.List_freq_CH.Count - 1];
                    }
                }
                else { taskParameters.MinFreq_MHz = 100; taskParameters.MaxFreq_MHz = 110; }
            }
            else
            {
                taskParameters.MaxFreq_MHz = taskSDR.MeasFreqParam.RgU.Value;
                taskParameters.MinFreq_MHz = taskSDR.MeasFreqParam.RgL.Value;
            }
            if (taskSDR.MeasSDRParam.RBW != 0) { taskParameters.RBW_Hz = taskSDR.MeasSDRParam.RBW * 1000; } else { taskParameters.RBW_Hz = 10000; }
            if (taskSDR.MeasSDRParam.VBW != 0) { taskParameters.VBW_Hz = taskSDR.MeasSDRParam.VBW * 1000; } else { taskParameters.VBW_Hz = 10000; }
            if (taskSDR.MeasFreqParam.Step != null) { taskParameters.StepSO_kHz = taskSDR.MeasFreqParam.Step.Value; } // обязательный параметер для SO (типа ширина канала или шаг сетки частот)
            SOType sOtype = new SOType();
            if (!(taskSDR.MeasSDRSOParam is null)) {sOtype = GetSOTypeFromSpectrumOccupationType(taskSDR.MeasSDRSOParam.TypeSO);}  // присвоение параметров для измерения SO
            if (taskSDR.MeasDataType == MeasurementType.SpectrumOccupation)
            {
                if ((taskSDR.MeasSDRSOParam.TypeSO == SpectrumOccupationType.FreqBandwidthOccupation) || (taskSDR.MeasSDRSOParam.TypeSO == SpectrumOccupationType.FreqChannelOccupation))
                {
                    if ((taskSDR.MeasSDRSOParam.NChenal > 0) && (taskSDR.MeasSDRSOParam.NChenal < 1000)) { taskParameters.NChenal = taskSDR.MeasSDRSOParam.NChenal; } else { taskParameters.NChenal = 10; }
                    if (taskSDR.MeasSDRSOParam.LevelMinOccup <= 0) { taskParameters.LevelMinOccup_dBm = taskSDR.MeasSDRSOParam.LevelMinOccup; } else { taskParameters.LevelMinOccup_dBm = -80; }
                    taskParameters.Type_of_SO = sOtype;
                    // формируем начало и конец для измерений 
                    taskParameters.List_freq_CH.Sort();
                    taskParameters.MinFreq_MHz = taskParameters.List_freq_CH[0] - taskParameters.StepSO_kHz / 2000;
                    taskParameters.MaxFreq_MHz = taskParameters.List_freq_CH[taskParameters.List_freq_CH.Count - 1] + taskParameters.StepSO_kHz / 2000;
                    // расчитываем желаемое RBW и VBW
                    taskParameters.VBW_Hz = taskParameters.StepSO_kHz * 1000 / taskParameters.NChenal;
                    taskParameters.RBW_Hz = taskParameters.StepSO_kHz * 1000 / taskParameters.NChenal;
                }
            }
            // коректировка режима измерения 
            taskParameters.TaskId = taskSDR.Id;
            taskParameters.MeasurementType = GetMeasTypeFromMeasurementType(taskSDR.MeasDataType);
            taskParameters.status = taskSDR.status;

            // до конца не определенные блоки
            taskParameters.ReceivedIQStreemDuration_sec = 1.0;


            return (taskParameters);
        }
        #endregion

        #region metodsSensore
        private SensorParameters GetSensorParameters(object sensor)
        {
            SensorParameters sensorParameters = new SensorParameters();
            if (sensor is Sensor)
            {
                sensorParameters = FillingSensorParameters_v1(sensor as Sensor);
            }
            if (sensor is null)
            {
                sensorParameters.RxLoss = 1;
                sensorParameters.Gain = 2.17;
            }
            return (sensorParameters);
        }
        class CompareGain : IComparer<SensorParameters.FreqGain>
        {
            public int Compare(SensorParameters.FreqGain x, SensorParameters.FreqGain y)
            {
                if (x.freq_MHz == y.freq_MHz) { return 0; }
                else
                {
                    if (x.freq_MHz > y.freq_MHz) { return -1; } else { return 1; }
                }
            }
        }
        private SensorParameters FillingSensorParameters_v1(Sensor sensor)
        {
            SensorParameters sensorParameters = new SensorParameters();
            if (sensor != null)
            {
                if (sensor.RxLoss != null) { sensorParameters.RxLoss = sensor.RxLoss.Value; } else { sensorParameters.RxLoss = 1; }
                if (sensor.Antenna != null)
                {
                    sensorParameters.Gain = sensor.Antenna.GainMax;
                    if (sensor.Antenna.AntennaPatterns != null)
                    { // НЕ ТЕСТИРОВАННО
                        if (sensor.Antenna.AntennaPatterns.Length > 0)
                        {
                            sensorParameters.freqGains = new SensorParameters.FreqGain[sensor.Antenna.AntennaPatterns.Length];
                            for (int i = 0; i < sensor.Antenna.AntennaPatterns.Length; i++)
                            {// сразу с сортировкой
                                sensorParameters.freqGains[i].freq_MHz = sensor.Antenna.AntennaPatterns[i].Freq;
                                sensorParameters.freqGains[i].GainLoss_dB = sensor.Antenna.AntennaPatterns[i].Gain;
                            }
                            // сортировка массива
                            IComparer<SensorParameters.FreqGain> compareGain = new CompareGain();
                            Array.Sort(sensorParameters.freqGains, compareGain);
                        }
                    }
                }
                else { sensorParameters.Gain = 2.17; }
            }
            return (sensorParameters);
        }
        #endregion

        #region metodsResult
        private LastResultParameters GetLastResultParameters(object LastResult)
        {
            LastResultParameters lastResultParameters = new LastResultParameters();
            if (LastResult != null)
            {
                if (LastResult is MeasSdrResults)
                {
                    lastResultParameters = FillingMeasParameters_v1(LastResult as MeasSdrResults);
                }
                if (LastResult is MeasSdrResults_v2)
                {
                    lastResultParameters = FillingMeasParameters_v2(LastResult as MeasSdrResults_v2);
                }
            }
            return (lastResultParameters);
        }
        private LastResultParameters FillingMeasParameters_v1(MeasSdrResults measSdrResults)
        {
            LastResultParameters lastResultParameters = new LastResultParameters
            {
                APIversion = 1,
                NN = measSdrResults.NN,
                FSemples = GetSemplesFreqFromFemples(measSdrResults.FSemples)
            };
            return (lastResultParameters);
        }
        private LastResultParameters FillingMeasParameters_v2(MeasSdrResults_v2 measSdrResults)
        {
            LastResultParameters lastResultParameters = new LastResultParameters
            {
                APIversion = 2,
                NN = measSdrResults.NN,
                FSemples = GetSemplesFreqFromFemples(measSdrResults.FSemples)
            };
            return (lastResultParameters);
        }

        #endregion

        #region metodsSDR
        private SDRParameters GetSDRParameters(object TaskSDR)
        {
            SDRParameters sDRParameters = new SDRParameters{filled = false};
            if (TaskSDR is MeasSdrTask)
            {
                sDRParameters = Filling_parameters_v1(TaskSDR as MeasSdrTask);
                SetTraceType(sDRParameters);
            }
            return (sDRParameters);
        }
        private SDRParameters Filling_parameters_v1(MeasSdrTask taskSDR)
        {
            SDRParameters sDRParameters = new SDRParameters {DetectTypeSDR = GetDetectTypeFromDetectingType(taskSDR.MeasSDRParam.DetectTypeSDR)};
            if (taskSDR.MeasFreqParam.MeasFreqs != null)
            {
                sDRParameters.List_freq_CH.Clear();
                for (int i = 0; i < taskSDR.MeasFreqParam.MeasFreqs.Length; i++)
                { double freq = taskSDR.MeasFreqParam.MeasFreqs[i].Freq; sDRParameters.List_freq_CH.Add(freq); }
            }
            if ((taskSDR.MeasFreqParam.RgL == null) || (taskSDR.MeasFreqParam.RgU == null)) // если вдруг отсутвуют начало и конец
            {
                if (sDRParameters.List_freq_CH != null)
                {
                    if (sDRParameters.List_freq_CH.Count > 0)
                    {
                        sDRParameters.List_freq_CH.Sort();
                        sDRParameters.MinFreq_MHz = sDRParameters.List_freq_CH[0];
                        sDRParameters.MaxFreq_MHz = sDRParameters.List_freq_CH[sDRParameters.List_freq_CH.Count - 1];
                    }
                }
                else { sDRParameters.MinFreq_MHz = 100; sDRParameters.MaxFreq_MHz = 110; }
            }
            else
            {
                sDRParameters.MaxFreq_MHz = taskSDR.MeasFreqParam.RgU.Value;
                sDRParameters.MinFreq_MHz = taskSDR.MeasFreqParam.RgL.Value;
            }
            SpectrScanType spectrScanType = GetSpectrScanTypeFromSpectrumScanType(taskSDR.TypeM);
            if (spectrScanType == SpectrScanType.RealTime) { sDRParameters.TypeSpectrumScan = spectrScanType; } else { sDRParameters.TypeSpectrumScan = SpectrScanType.Sweep; }
            if (taskSDR.MeasSDRParam.ref_level_dbm != 0) { sDRParameters.RefLevel_dBm = taskSDR.MeasSDRParam.ref_level_dbm; } else { sDRParameters.RefLevel_dBm = -30; }
            sDRParameters.RfAttenuationSDR = taskSDR.MeasSDRParam.RfAttenuationSDR;
            sDRParameters.PreamplificationSDR = taskSDR.MeasSDRParam.PreamplificationSDR;
            if (taskSDR.MeasSDRParam.RBW != 0) { sDRParameters.RBW_Hz = taskSDR.MeasSDRParam.RBW * 1000; } else { sDRParameters.RBW_Hz = 10000; }
            if (taskSDR.MeasSDRParam.VBW != 0) { sDRParameters.VBW_Hz = taskSDR.MeasSDRParam.VBW * 1000; } else { sDRParameters.VBW_Hz = 10000; }
            if (taskSDR.MeasSDRParam.MeasTime != 0) { sDRParameters.MeasTime_Sec = taskSDR.MeasSDRParam.MeasTime; } else { sDRParameters.MeasTime_Sec = 0.001; }
            if (taskSDR.SwNumber != 0) { sDRParameters.SwNumber = taskSDR.SwNumber; } else { sDRParameters.SwNumber = 10; }
            if (taskSDR.MeasFreqParam.Step != null) { sDRParameters.StepSO_kHz = taskSDR.MeasFreqParam.Step.Value; } // обязательный параметер для SO (типа ширина канала или шаг сетки частот)
            MeasType measType = GetMeasTypeFromMeasurementType(taskSDR.MeasDataType);                                                                                           
            if (measType == MeasType.SpectrumOccupation) // присвоение параметров для измерения SO
            {
                SOType sOType = new SOType();
                if (!(taskSDR.MeasSDRSOParam is null)) { sOType = GetSOTypeFromSpectrumOccupationType(taskSDR.MeasSDRSOParam.TypeSO); }
                if ((sOType == SOType.FreqBandwidthOccupation) || (sOType == SOType.FreqChannelOccupation))
                {
                    if ((taskSDR.MeasSDRSOParam.NChenal > 0) && (taskSDR.MeasSDRSOParam.NChenal < 1000)) { sDRParameters.NChenal = taskSDR.MeasSDRSOParam.NChenal; } else { sDRParameters.NChenal = 10; }
                    if (taskSDR.MeasSDRSOParam.LevelMinOccup <= 0) { sDRParameters.LevelMinOccup_dBm = taskSDR.MeasSDRSOParam.LevelMinOccup; } else { sDRParameters.LevelMinOccup_dBm = -80; }
                    sDRParameters.Type_of_SO = sOType;
                    // формируем начало и конец для измерений 
                    sDRParameters.List_freq_CH.Sort();
                    sDRParameters.MinFreq_MHz = sDRParameters.List_freq_CH[0] - sDRParameters.StepSO_kHz / 2000;
                    sDRParameters.MaxFreq_MHz = sDRParameters.List_freq_CH[sDRParameters.List_freq_CH.Count - 1] + sDRParameters.StepSO_kHz / 2000;
                    // расчитываем желаемое RBW и VBW
                    sDRParameters.VBW_Hz = sDRParameters.StepSO_kHz * 1000 / sDRParameters.NChenal;
                    sDRParameters.RBW_Hz = sDRParameters.StepSO_kHz * 1000 / sDRParameters.NChenal;
                }
            }
            // коректировка режима измерения 
            if ((spectrScanType == SpectrScanType.RealTime) && ((sDRParameters.MaxFreq_MHz - sDRParameters.MinFreq_MHz > 20) || ((sDRParameters.MaxFreq_MHz - sDRParameters.MinFreq_MHz) * 1000000 / sDRParameters.RBW_Hz > 8000))) { sDRParameters.TypeSpectrumScan = SpectrScanType.Sweep; }
            sDRParameters.TaskId = taskSDR.Id;
            sDRParameters.MeasurementType = measType;
            sDRParameters.filled = true;
            return (sDRParameters);
        }
        private void SetTraceType(SDRParameters sDRParameters)
        {
            sDRParameters.traceType = TraceType.Average;
            if (sDRParameters.MeasurementType == MeasType.SpectrumOccupation) { sDRParameters.traceType = TraceType.Average; }
            if (sDRParameters.MeasurementType == MeasType.BandwidthMeas) { sDRParameters.traceType = TraceType.MaxHold; }
        }
        #endregion
    }

    public class MeasSdrResults_v2:MeasSdrResults
    {
        public Emitting[] emittings;
    }
}
*/
