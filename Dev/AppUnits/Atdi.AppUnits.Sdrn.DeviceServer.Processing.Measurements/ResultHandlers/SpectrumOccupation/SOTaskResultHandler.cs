using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Commands;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Results;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using DM = Atdi.DataModels.Sdrns.Device;
using System;
using System.Linq;
using System.Collections.Generic;
using Atdi.DataModels.Sdrn.DeviceServer;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;



namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements
{
    public class SOTaskResultHandler : IResultHandler<MesureTraceCommand, MesureTraceResult, SOTask, SpectrumOccupationProcess>
    {
        public void Handle(MesureTraceCommand command, MesureTraceResult result, DataModels.Sdrn.DeviceServer.ITaskContext<SOTask, SpectrumOccupationProcess> taskContext)
        {
            if (result != null)
            {
                SpectrumOcupationResult measResults = null;
                try
                {
                    /*
                    FileStream fs = new FileStream(Guid.NewGuid().ToString() + ".dat", FileMode.Create);

                    // Construct a BinaryFormatter and use it to serialize the data to the stream.
                    BinaryFormatter formatter = new BinaryFormatter();
                    try
                    {
                        formatter.Serialize(fs, result);
                    }
                    catch (SerializationException e)
                    {
                        Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                        throw;
                    }
                    finally
                    {
                        fs.Close();
                    }
                    */
                    measResults = CalcSpectrumOcupation.Calc(result, taskContext.Task.taskParameters, taskContext.Task.sensorParameters, taskContext.Task.lastResultParameters);
                    // Обновление последнего результата в буфере (кеше)
                    taskContext.Task.lastResultParameters = measResults;  //new  SpectrumOcupationResult() { fSemplesResult = measResults.fSemplesResult, NN = measResults.NN };
                    // Отправка результата в Task Handler
                    taskContext.SetEvent(measResults);
                }
                catch (Exception ex)
                {
                    taskContext.SetEvent<ExceptionProcessSO>(new ExceptionProcessSO(CommandFailureReason.Exception, ex));
                }
            }
        }
    }
}
