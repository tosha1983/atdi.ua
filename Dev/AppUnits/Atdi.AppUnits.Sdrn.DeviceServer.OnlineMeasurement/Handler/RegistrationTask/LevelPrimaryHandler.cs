using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Commands;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Results;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using DM = Atdi.DataModels.Sdrns.Device;
using System;
using System.Linq;
using System.Collections.Generic;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrns.Device;
using Atdi.DataModels.Sdrns.Device.OnlineMeasurement;
using Atdi.AppUnits.Sdrn.DeviceServer.OnlineMeasurement.Tasks;
using System.Collections;
using System.ComponentModel;
using Atdi.Common;
using Atdi.Platform;



namespace Atdi.AppUnits.Sdrn.DeviceServer.OnlineMeasurement.Results
{
    public class LevelPrimaryHandler : IResultHandler<MesureTraceCommand, MesureTraceResult, ClientTaskRegistrationTask, OnlineMeasurementProcess>
    {
        private readonly AppServerComponentConfig _config;

        public LevelPrimaryHandler(AppServerComponentConfig config)
        {
            this._config = config;
        }

        public void Handle(MesureTraceCommand command, MesureTraceResult tempResult, DataModels.Sdrn.DeviceServer.ITaskContext<ClientTaskRegistrationTask, OnlineMeasurementProcess> taskContext)
        {
            if (tempResult != null)
            {
                var result = CopyHelper.CreateDeepCopy(tempResult);

                try
                {
                    var parametersDataLevel = new DeviceServerParametersDataLevel();
                    parametersDataLevel.Freq_Hz = CutArray(result.Freq_Hz, this._config.MaxCountPoint.Value);
                    if (result.Freq_Hz.Length > 0)
                    {
                        var freq_Hz = (double)((result.Freq_Hz[0] + result.Freq_Hz[result.Freq_Hz.Length - 1]) / 2.0);
                        var freq_MHz = (double)(freq_Hz / 1000000.0);
                        parametersDataLevel.AntennaFactor = 20 * Math.Log10(freq_MHz) - SDRGainFromFrequency(taskContext.Process.MeasTraceDeviceProperties, freq_Hz) - 29.79;
                    }
                    parametersDataLevel.Att_dB = result.Att_dB;
                    parametersDataLevel.PreAmp_dB = result.PreAmp_dB;
                    parametersDataLevel.RBW_kHz = (double)(result.RBW_Hz /1000.0);
                    parametersDataLevel.RefLevel_dBm = result.RefLevel_dBm;
                    taskContext.SetEvent(parametersDataLevel);
                    taskContext.Process.CountMeasurementDone++;
                }
                catch (Exception ex)
                {
                    taskContext.SetEvent<ExceptionProcessLevel>(new ExceptionProcessLevel(CommandFailureReason.Exception, ex));
                }
            }
        }



        public static double SDRGainFromFrequency(MesureTraceDeviceProperties MesureTraceDeviceProperties, double Frequency_Hz)
        {
            // Константа с файла конфигурации
            double GainByDefault = 0;

            if ((Frequency_Hz < 9) || (Frequency_Hz > 400000000000) || (MesureTraceDeviceProperties == null) || (MesureTraceDeviceProperties.StandardDeviceProperties == null) ||
                (MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters == null)) { return GainByDefault; }
            if ((double)MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[0].Freq_Hz >= Frequency_Hz)
            {
                double GainLoss = MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[0].Gain - MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[0].FeederLoss_dB;
                return GainLoss;
            }
            if ((double)MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters.Length-1].Freq_Hz <= Frequency_Hz)
            {
                double GainLoss = MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters.Length-1].Gain- MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters.Length-1].FeederLoss_dB;
                return GainLoss;
            }
            if (MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters.Length == 1)
            {
                double GainLoss = MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[0].Gain - MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[0].FeederLoss_dB;
                return GainLoss;
            }
            for (int i = 0; i < MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters.Length - 1; i++)
            {
                if (((double)MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[i].Freq_Hz <= Frequency_Hz) && ((double)MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[i + 1].Freq_Hz >= Frequency_Hz))
                {
                    double G = MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[i].Gain - MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[i].FeederLoss_dB +
                        (Frequency_Hz - (double)MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[i].Freq_Hz) *
                        (MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[i + 1].Gain - MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[i].Gain -
                        MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[i + 1].FeederLoss_dB + MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[i].FeederLoss_dB) /
                        ((double)MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[i + 1].Freq_Hz - (double)MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[i].Freq_Hz);
                    return G;
                }
            }
            return GainByDefault;
        }
        


        public static double[] CutArray(double[] arr, int CountPoint)
        {
            if (arr.Length <= CountPoint)
            {
                double[] arrMhz = new double[arr.Length];
                for (int i=0; i< arr.Length;i++)
                {
                    arrMhz[i] = (double)(arr[i] / 1000000);
                }
                return arrMhz;
            }
            else
            {
                var k = (int)Math.Round((double)(arr.Length / CountPoint));
                int newpoint = (int)Math.Ceiling((double)(arr.Length / k));
                var reducedArray = new double[newpoint];
                int reducedIndex = 0;
                for (int i = 0; i < arr.Length; i += k)
                {
                    if (reducedIndex > newpoint - 1) break;
                    reducedArray[reducedIndex++] = (double)(arr[i] / 1000000);
                }
                return reducedArray;
            }
        }
    }
}
