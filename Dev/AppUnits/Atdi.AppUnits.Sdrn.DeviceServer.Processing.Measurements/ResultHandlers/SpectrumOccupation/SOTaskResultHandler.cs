using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Commands;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Results;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using DM = Atdi.DataModels.Sdrns.Device;
using System;
using System.Linq;
using System.Collections.Generic;
using Atdi.DataModels.Sdrn.DeviceServer;




namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements
{
    public class SOTaskResultHandler : IResultHandler<MesureTraceCommand, MesureTraceResult, SOTask, MeasProcess>
    {
        public void Handle(MesureTraceCommand command, MesureTraceResult result, DataModels.Sdrn.DeviceServer.ITaskContext<SOTask, MeasProcess> taskContext)
        {
            if (result != null)
            {
                // Получение предыдущего результата вычисления SO по даному task
                var prevMEasurementResult = taskContext.Task.lastResultParameters;
                // Вычисление Spectrum Occupation
                SpectrumOcupationResult measResults = null;
                try
                {
                    CalcSpectrumOcupation.Calc(result, taskContext.Task.taskParameters, taskContext.Task.sensorParameters, prevMEasurementResult);
                }
                catch (Exception ex)
                {
                    taskContext.SetEvent<ExceptionProcessSO>(new ExceptionProcessSO(CommandFailureReason.Exception, ex));
                }
                // Обновление последнего результата в буфере (кеше)
                taskContext.Task.lastResultParameters = new LastResultParameters() { APIversion = 2, FSemples = measResults.fSemplesResult, NN = measResults.NN };

                // Отправка результата в Task Handler
                taskContext.SetEvent(measResults);
            }
        }
    }
}
