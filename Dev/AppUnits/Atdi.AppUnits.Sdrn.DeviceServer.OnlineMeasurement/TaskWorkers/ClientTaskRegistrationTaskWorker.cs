using Atdi.AppUnits.Sdrn.DeviceServer.OnlineMeasurement.Tasks;
using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrns.Device.OnlineMeasurement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.OnlineMeasurement.TaskWorkers
{
    public class ClientTaskRegistrationTaskWorker : ITaskWorker<ClientTaskRegistrationTask, OnlineMeasurementProcess, PerThreadTaskWorkerLifetime>
    {
        public void Run(ITaskContext<ClientTaskRegistrationTask, OnlineMeasurementProcess> context)
        {
            try
            {
                if (context.Process.MeasTask.OnlineMeasType == OnlineMeasType.Level)
                {
                    var serverParams = new DeviceServerParametersDataLevel
                    {
                        SensorToken = Guid.NewGuid().ToByteArray(),
                        Att_dB = context.Process.MeasTask.Att_dB,
                        PreAmp_dB = context.Process.MeasTask.PreAmp_dB,
                        RefLevel_dBm = context.Process.MeasTask.RefLevel_dBm,
                        RBW_kHz = context.Process.MeasTask.RBW_kHz,
                        Freq_Hz = null,
                        isChanged_Att_dB = false,
                        isChanged_PreAmp_dB = false,
                        isChanged_RBW_kHz = false,
                        isChanged_RefLevel_dBm = false
                    };
                    context.Process.Parameters = serverParams;
                }
                else
                {
                    throw new NotImplementedException($"Type {context.Process.MeasTask.OnlineMeasType} is not supported");
                }
                context.Finish();
            }
            catch (Exception e)
            {
                context.Abort(e);
            }
            
        }
    }
}
