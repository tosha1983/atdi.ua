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
                

                if (context.Process.MeasTask.SomeMeasType == 1)
                {
                    var serverParams = new DeviceServerParametersData
                    {
                        SensorToken = Guid.NewGuid().ToByteArray(),
                        Frequencies = new float[3000]
                    };
                    context.Process.Parameters = serverParams;
                }
                else if (context.Process.MeasTask.SomeMeasType == 2)
                {
                    var serverParams = new DeviceServerParametersData
                    {
                        SensorToken = Guid.NewGuid().ToByteArray(),
                        Frequencies = new float[10000]
                    };
                    context.Process.Parameters = serverParams;
                }
                if (context.Process.MeasTask.SomeMeasType == 3)
                {
                    var serverParams = new DeviceServerParametersData
                    {
                        SensorToken = Guid.NewGuid().ToByteArray(),
                        Frequencies = new float[100]
                    };
                    context.Process.Parameters = serverParams;
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
