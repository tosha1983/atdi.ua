using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrns.Device;
using Atdi.DataModels.Sdrn.DeviceServer;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Messaging.Convertor
{
    public static class ConvertMeasTaskToTaskParameters
    {
        public static TaskParameters Convert(this MeasTask taskSDR)
        {
            const int SO_Ncount = 10000;
            const int SignalizationNCount = 1000000;
            const int OtherNCount = 1000;


            const int SO_NChenal = 10;
            const int SignalizationNChenal = 100;

            var taskParameters = new TaskParameters();

            //
            //taskParameters.NCount = необходимо вичислить

            taskParameters.StartTime = taskSDR.StartTime;
            taskParameters.StopTime = taskSDR.StopTime;
            if (taskSDR.Frequencies != null)
            {
                if (taskSDR.Frequencies.Values_MHz != null)
                {
                    taskParameters.ListFreqCH = new List<double>();
                    taskParameters.ListFreqCH.Clear();
                    for (int i = 0; i < taskSDR.Frequencies.Values_MHz.Length; i++)
                    {
                        var freq = taskSDR.Frequencies.Values_MHz[i];
                        taskParameters.ListFreqCH.Add(freq);
                    }
                }
                if ((taskSDR.Frequencies.RgL_MHz == null) || (taskSDR.Frequencies.RgU_MHz == null)) // если вдруг отсутвуют начало и конец
                {
                    if (taskParameters.ListFreqCH != null)
                    {
                        if (taskParameters.ListFreqCH.Count > 0)
                        {
                            taskParameters.ListFreqCH.Sort();
                            taskParameters.MinFreq_MHz = taskParameters.ListFreqCH[0];
                            taskParameters.MaxFreq_MHz = taskParameters.ListFreqCH[taskParameters.ListFreqCH.Count - 1];
                        }
                    }
                    else { taskParameters.MinFreq_MHz = 100; taskParameters.MaxFreq_MHz = 110; }
                }
                else
                {
                    taskParameters.MaxFreq_MHz = taskSDR.Frequencies.RgU_MHz.Value;
                    taskParameters.MinFreq_MHz = taskSDR.Frequencies.RgL_MHz.Value;
                }
                if (taskSDR.Frequencies.Step_kHz != null) { taskParameters.StepSO_kHz = taskSDR.Frequencies.Step_kHz.Value; } // обязательный параметер для SO (типа ширина канала или шаг сетки частот)
            }
            if (taskSDR.DeviceParam != null)
            {
                if (taskSDR.DeviceParam.RBW_kHz != 0) { taskParameters.RBW_Hz = taskSDR.DeviceParam.RBW_kHz * 1000; } else { taskParameters.RBW_Hz = 10000; }
                if (taskSDR.DeviceParam.VBW_kHz != 0) { taskParameters.VBW_Hz = taskSDR.DeviceParam.VBW_kHz.Value * 1000; } else { taskParameters.VBW_Hz = 10000; }
            }
            if (taskSDR.DeviceParam.MeasTime_sec != null) { taskParameters.SweepTime_s = taskSDR.DeviceParam.MeasTime_sec.Value; } else { taskParameters.SweepTime_s = 0.0001; }

            if (taskSDR.SOParam != null)
            {
                if ((taskSDR.DeviceParam.MeasTime_sec == null) || (taskSDR.Interval_sec == 0) || ((taskSDR.DeviceParam.MeasTime_sec != null) && (taskSDR.DeviceParam.MeasTime_sec == 0)))
                {
                    switch (taskSDR.Measurement)
                    {
                        case DataModels.Sdrns.MeasurementType.SpectrumOccupation:
                            taskParameters.NCount = SO_Ncount;
                            break;
                        case DataModels.Sdrns.MeasurementType.Signaling:
                            taskParameters.NCount = SignalizationNCount;
                            break;
                        default:
                            taskParameters.NCount = OtherNCount;
                            break;
                    }
                }
                else
                {
                    taskParameters.NCount = (int)((double)taskSDR.Interval_sec / (double)taskSDR.DeviceParam.MeasTime_sec);
                }
                var sOtype = GetSOTypeFromSpectrumOccupationType(taskSDR.SOParam.Type);
                if (taskSDR.Measurement == DataModels.Sdrns.MeasurementType.SpectrumOccupation)
                {
                    if ((taskSDR.SOParam.Type == DataModels.Sdrns.SpectrumOccupationType.FreqBandOccupancy) || (taskSDR.SOParam.Type == DataModels.Sdrns.SpectrumOccupationType.FreqChannelOccupancy))
                    {
                        if ((taskSDR.SOParam.MeasurmentNumber > 0) && (taskSDR.SOParam.MeasurmentNumber < 1000)) { taskParameters.NChenal = taskSDR.SOParam.MeasurmentNumber; } else { taskParameters.NChenal = SO_NChenal; }
                        if (taskSDR.SOParam.LevelMinOccup_dBm <= 0) { taskParameters.LevelMinOccup_dBm = taskSDR.SOParam.LevelMinOccup_dBm; } else { taskParameters.LevelMinOccup_dBm = -80; }
                        taskParameters.TypeOfSO = sOtype;
                        if ((taskParameters.ListFreqCH != null) && (taskParameters.ListFreqCH.Count > 0))
                        {
                            // формируем начало и конец для измерений 
                            taskParameters.ListFreqCH.Sort();
                            taskParameters.MinFreq_MHz = taskParameters.ListFreqCH[0] - taskParameters.StepSO_kHz / 2000;
                            taskParameters.MaxFreq_MHz = taskParameters.ListFreqCH[taskParameters.ListFreqCH.Count - 1] + taskParameters.StepSO_kHz / 2000;
                        }
                        // расчитываем желаемое RBW и VBW
                        taskParameters.VBW_Hz = taskParameters.StepSO_kHz * 1000 / taskParameters.NChenal;
                        taskParameters.RBW_Hz = taskParameters.StepSO_kHz * 1000 / taskParameters.NChenal;
                    }
                }
                else if (taskSDR.Measurement == DataModels.Sdrns.MeasurementType.Signaling)
                {
                    if ((taskSDR.SOParam.MeasurmentNumber > 0) && (taskSDR.SOParam.MeasurmentNumber < 1000)) { taskParameters.NChenal = taskSDR.SOParam.MeasurmentNumber; } else { taskParameters.NChenal = SignalizationNChenal; }
                    if ((taskParameters.ListFreqCH != null) && (taskParameters.ListFreqCH.Count > 0))
                    {
                        // формируем начало и конец для измерений 
                        taskParameters.ListFreqCH.Sort();
                        taskParameters.MinFreq_MHz = taskParameters.ListFreqCH[0] - taskParameters.StepSO_kHz / 2000;
                        taskParameters.MaxFreq_MHz = taskParameters.ListFreqCH[taskParameters.ListFreqCH.Count - 1] + taskParameters.StepSO_kHz / 2000;
                    }
                }
            }
            // коректировка режима измерения 
            taskParameters.SDRTaskId = taskSDR.TaskId;
            taskParameters.MeasurementType = GetMeasTypeFromMeasurementType(taskSDR.Measurement);
            taskParameters.status = taskSDR.Status;

            if (taskSDR.Measurement == DataModels.Sdrns.MeasurementType.Signaling)
            {
                if (taskSDR.RefSituation != null)
                {
                    var listReferenceSituation = new List<ReferenceSituation>();
                    {
                        var refSituation = new ReferenceSituation();
                        var refSituationTemp = taskSDR.RefSituation;
                        refSituation.SensorId = refSituationTemp.SensorId;
                        var referenceSignal = refSituationTemp.ReferenceSignal;
                        if (referenceSignal.Length > 0)
                        {
                            refSituation.ReferenceSignal = new ReferenceSignal[referenceSignal.Length];
                            for (int l = 0; l < referenceSignal.Length; l++)
                            {
                                var refSituationReferenceSignal = refSituation.ReferenceSignal[l];
                                refSituationReferenceSignal = new ReferenceSignal();
                                refSituationReferenceSignal.Bandwidth_kHz = referenceSignal[l].Bandwidth_kHz;
                                refSituationReferenceSignal.Frequency_MHz = referenceSignal[l].Frequency_MHz;
                                refSituationReferenceSignal.LevelSignal_dBm = referenceSignal[l].LevelSignal_dBm;
                                refSituationReferenceSignal.SignalMask = new SignalMask();
                                if (referenceSignal[l].SignalMask != null)
                                {
                                    refSituationReferenceSignal.SignalMask.Freq_kHz = referenceSignal[l].SignalMask.Freq_kHz;
                                    refSituationReferenceSignal.SignalMask.Loss_dB = referenceSignal[l].SignalMask.Loss_dB;
                                }
                                refSituation.ReferenceSignal[l] = refSituationReferenceSignal;
                            }
                        }
                        listReferenceSituation.Add(refSituation);
                    }
                    if (listReferenceSituation.Count > 0)
                    {
                        taskParameters.RefSituation = listReferenceSituation[0];
                    }
                }
            }
        

            // до конца не определенные блоки
            taskParameters.ReceivedIQStreemDuration_sec = 1.0;
            return taskParameters;
        }

        private static SOType GetSOTypeFromSpectrumOccupationType(DataModels.Sdrns.SpectrumOccupationType spectrumOccupationType)
        {
            SOType sOType;
            switch (spectrumOccupationType)
            {
                case DataModels.Sdrns.SpectrumOccupationType.FreqBandOccupancy:
                    sOType = SOType.FreqBandwidthOccupation;
                    break;
                case DataModels.Sdrns.SpectrumOccupationType.FreqChannelOccupancy:
                    sOType = SOType.FreqChannelOccupation;
                    break;
                default:
                    sOType = SOType.FreqChannelOccupation;
                    break;
            }
            return (sOType);
        }

        private static MeasType GetMeasTypeFromMeasurementType(DataModels.Sdrns.MeasurementType measurementType)
        {
            MeasType measType;
            switch (measurementType)
            {
                case DataModels.Sdrns.MeasurementType.BandwidthMeas:
                    measType = MeasType.BandwidthMeas;
                    break;
                case DataModels.Sdrns.MeasurementType.Level:
                    measType = MeasType.Level;
                    break;
                case DataModels.Sdrns.MeasurementType.SpectrumOccupation:
                    measType = MeasType.SpectrumOccupation;
                    break;
                case DataModels.Sdrns.MeasurementType.MonitoringStations:
                    measType = MeasType.MonitoringStations;
                    break;
                case DataModels.Sdrns.MeasurementType.Signaling:
                    measType = MeasType.Signaling;
                    break;
                default:
                    measType = MeasType.Level;
                    break;
            }
            return (measType);
        }

    }
}
