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
                    //  с файла конфигурации
                    //  BandWidthEstimation.BandwidthEstimationType.beta, 1, 1 -вынести в сигнализацию
                    var measBandWidthResults = BandWidthEstimation.GetBandwidthPoint(result.Level, BandWidthEstimation.BandwidthEstimationType.beta, 1, 1);
                    if ((measBandWidthResults.СorrectnessEstimations!=null) && (measBandWidthResults.СorrectnessEstimations == true))
                    {
                        taskContext.Task.MeasBWResults = new BWResult();
                        if (measBandWidthResults.BandwidthkHz != null)
                        {
                            taskContext.Task.MeasBWResults.Bandwidth_kHz = measBandWidthResults.BandwidthkHz.Value;
                        }
                        if (measBandWidthResults.MarkerIndex != null)
                        {
                            taskContext.Task.MeasBWResults.MarkerIndex = measBandWidthResults.MarkerIndex.Value;
                        }
                        if (measBandWidthResults.T1 != null)
                        {
                            taskContext.Task.MeasBWResults.T1 = measBandWidthResults.T1.Value;
                        }
                        if (measBandWidthResults.T2 != null)
                        {
                            taskContext.Task.MeasBWResults.T2 = measBandWidthResults.T2.Value;
                        }

                        taskContext.Task.MeasBWResults.Levels_dBm = result.Level;
                        taskContext.Task.MeasBWResults.Freq_Hz = result.Freq_Hz;
                        taskContext.Task.MeasBWResults.СorrectnessEstimations = measBandWidthResults.СorrectnessEstimations.Value;
                    }
                    else
                    {

                    }

                    // Отправка результата в родительский процесс (если он есть)
                    var parentProcess = taskContext.Descriptor.Parent;
                    if (parentProcess != null)
                    {
                        ///если родительский контекст - сигнализация, то отправить результат 
                        if (parentProcess is DataModels.Sdrn.DeviceServer.ITaskContext<SignalizationTask, SignalizationProcess>)
                        {
                            parentProcess.SetEvent(taskContext.Task.MeasBWResults);
                        }
                        else // иначе отправка в воркер BandWidthTaskWorker 
                        {
                            taskContext.SetEvent(taskContext.Task.MeasBWResults);
                        }
                    }
                    else // иначе отправка в воркер BandWidthTaskWorker 
                    {
                        taskContext.SetEvent(taskContext.Task.MeasBWResults);
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
