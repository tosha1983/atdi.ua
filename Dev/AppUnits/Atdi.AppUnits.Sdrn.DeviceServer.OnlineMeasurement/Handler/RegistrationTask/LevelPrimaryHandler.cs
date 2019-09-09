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



namespace Atdi.AppUnits.Sdrn.DeviceServer.OnlineMeasurement.Results
{
    public class LevelPrimaryHandler : IResultHandler<MesureTraceCommand, MesureTraceResult, ClientTaskRegistrationTask, OnlineMeasurementProcess>
    {
        private readonly AppServerComponentConfig _config;

        public LevelPrimaryHandler(AppServerComponentConfig config)
        {
            this._config = config;
        }

        public void Handle(MesureTraceCommand command, MesureTraceResult result, DataModels.Sdrn.DeviceServer.ITaskContext<ClientTaskRegistrationTask, OnlineMeasurementProcess> taskContext)
        {
            if (result != null)
            {
                try
                {
                    var parametersDataLevel = new DeviceServerParametersDataLevel();
                    parametersDataLevel.Freq_Hz = CutArray(result.Freq_Hz, this._config.MaxCountPoint.Value);
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
