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
using Atdi.Common;



namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements
{
    public class SOTaskResultHandler : IResultHandler<MesureTraceCommand, MesureTraceResult, SOTask, SpectrumOccupationProcess>
    {
        public void Handle(MesureTraceCommand command, MesureTraceResult result, DataModels.Sdrn.DeviceServer.ITaskContext<SOTask, SpectrumOccupationProcess> taskContext)
        {
            if (result != null)
            {
                //var result = CopyHelper.CreateDeepCopy(tempResult);

                SpectrumOcupationResult measResults = null;
                try
                {
                    measResults = CalcSpectrumOcupation.Calc(result, taskContext.Task.taskParameters, taskContext.Task.sensorParameters, taskContext.Task.lastResultParameters);
                    // Обновление последнего результата в буфере (кеше)
                    taskContext.Task.lastResultParameters = measResults;  //new  SpectrumOcupationResult() { fSemplesResult = measResults.fSemplesResult, NN = measResults.NN };
                    // Отправка результата в Task Handler
                    taskContext.SetEvent(measResults);
                }
                catch (Exception ex)
                {
                    taskContext.SetEvent((SpectrumOcupationResult)(null));
                    taskContext.SetEvent<ExceptionProcessSO>(new ExceptionProcessSO(CommandFailureReason.Exception, ex));
                }
            }
        }
    }
}
