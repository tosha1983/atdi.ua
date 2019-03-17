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



namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements
{
    public class BandWidthTaskResultHandler : IResultHandler<MesureTraceCommand, MesureTraceResult, BandWidthTask, BandWidthProcess>
    {
        public void Handle(MesureTraceCommand command, MesureTraceResult result, DataModels.Sdrn.DeviceServer.ITaskContext<BandWidthTask, BandWidthProcess> taskContext)
        {
            if (result != null)
            {
                try
                {
                    var measBandWidthResults = BandWidthEstimation.GetBandwidthPoint(result.Level);

                    // Отправка результата в родительский процесс (если он есть)
                    if (taskContext.Descriptor.Parent != null)
                    {
                        ///если родительский контекст - сигнализация, то отправить результат 
                        if (taskContext.Descriptor.Parent is DataModels.Sdrn.DeviceServer.ITaskContext<SignalizationTask, SignalizationProcess>)
                        {
                            taskContext.Descriptor.Parent.SetEvent(measBandWidthResults);
                        }
                        else // иначе отправка в воркер BandWidthTaskWorker 
                        {
                            taskContext.SetEvent(measBandWidthResults);
                        }
                    }
                    else // иначе отправка в воркер BandWidthTaskWorker 
                    {
                        taskContext.SetEvent(measBandWidthResults);
                    }
                }
                catch (Exception ex)
                {
                    taskContext.SetEvent<ExceptionProcessBandWidth>(new ExceptionProcessBandWidth(CommandFailureReason.Exception, ex));
                }
            }
        }

    }
}
