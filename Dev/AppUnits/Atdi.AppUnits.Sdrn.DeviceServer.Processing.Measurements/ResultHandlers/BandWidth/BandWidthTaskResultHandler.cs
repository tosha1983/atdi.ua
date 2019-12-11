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
using Atdi.Common;


namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements
{
    public class BandWidthTaskResultHandler : IResultHandler<MesureTraceCommand, MesureTraceResult, BandWidthTask, BandWidthProcess>
    {
        //  constant
        private static BandWidthEstimation.BandwidthEstimationType bandwidthEstimationTypeDefault = BandWidthEstimation.BandwidthEstimationType.xFromCentr;
        private static double X_beta = 25;
        // end constant

        public void Handle(MesureTraceCommand command, MesureTraceResult tempResult, DataModels.Sdrn.DeviceServer.ITaskContext<BandWidthTask, BandWidthProcess> taskContext)
        {
            if (tempResult != null)
            {
                var result = CopyHelper.CreateDeepCopy(tempResult);

                try
                {
                    var Levels = result.Level;
                    if (taskContext.Task.Smooth) { Levels = SmoothTrace.blackman(Levels); }
                    int MaximumIgnorPoint =(int)Math.Round(result.Level.Length / 300.0);
                    MeasBandwidthResult measBandWidthResults = null;
                    var parentProcess = taskContext.Descriptor.Parent;
                    if (parentProcess != null)
                    {
                        if ((parentProcess is DataModels.Sdrn.DeviceServer.ITaskContext<SignalizationTask, SignalizationProcess>) == true)
                        {
                            BandWidthEstimation.BandwidthEstimationType bandwidthEstimationType;
                            if (Enum.TryParse(taskContext.Task.BandwidthEstimationType, out bandwidthEstimationType))
                            {
                                measBandWidthResults = BandWidthEstimation.GetBandwidthPoint(result.Level, bandwidthEstimationType, taskContext.Task.X_Beta, taskContext.Task.MaximumIgnorPoint);
                            }
                            else
                            {
                                measBandWidthResults = BandWidthEstimation.GetBandwidthPoint(result.Level, bandwidthEstimationTypeDefault, X_beta, MaximumIgnorPoint);
                            }
                        }
                        else
                        {
                            measBandWidthResults = BandWidthEstimation.GetBandwidthPoint(result.Level, bandwidthEstimationTypeDefault, X_beta, MaximumIgnorPoint);
                        }
                    }
                    else
                    {
                        measBandWidthResults = BandWidthEstimation.GetBandwidthPoint(result.Level, bandwidthEstimationTypeDefault, X_beta, MaximumIgnorPoint);
                    }



                    if ((measBandWidthResults.СorrectnessEstimations != null) && (measBandWidthResults.СorrectnessEstimations == true))
                    {
                        taskContext.Task.MeasBWResults = new BWResult();
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

                        taskContext.Task.MeasBWResults.Bandwidth_kHz = ((taskContext.Task.MeasBWResults.T2 - taskContext.Task.MeasBWResults.T1) *
                        (result.FrequencyStep_Hz)) / 1000;
                        //((result.Freq_Hz[result.Freq_Hz.Length - 1] - result.Freq_Hz[0]) / (result.Freq_Hz.Length - 1))) / 1000;

                        taskContext.Task.MeasBWResults.Levels_dBm = result.Level;
                        taskContext.Task.MeasBWResults.Freq_Hz = new double[result.LevelMaxIndex + 1];
                        for (int v=0; v<= result.LevelMaxIndex; v++)
                        {
                            taskContext.Task.MeasBWResults.Freq_Hz[v] = (result.FrequencyStart_Hz + v * result.FrequencyStep_Hz);
                        }
                       
                        taskContext.Task.MeasBWResults.СorrectnessEstimations = measBandWidthResults.СorrectnessEstimations.Value;
                        taskContext.Task.MeasBWResults.TimeMeas = DateTime.Now;
                    }

                    // Отправка результата в родительский процесс (если он есть)

                    if (parentProcess != null)
                    {
                        ///если родительский контекст - сигнализация, то отправить результат 
                        if (parentProcess is DataModels.Sdrn.DeviceServer.ITaskContext<SignalizationTask, SignalizationProcess>)
                        {
                            if (taskContext.Task.MeasBWResults != null)
                            {
                                parentProcess.SetEvent(taskContext.Task.MeasBWResults);
                            }
                            else
                            {
                                parentProcess.SetEvent<ExceptionProcessBandWidth>(new ExceptionProcessBandWidth(CommandFailureReason.Exception, new Exception()));
                            }
                        }
                        else // иначе отправка в воркер BandWidthTaskWorker 
                        {
                            if (taskContext.Task.MeasBWResults != null)
                            {
                                taskContext.SetEvent(taskContext.Task.MeasBWResults);
                            }
                            else
                            {
                                taskContext.SetEvent<ExceptionProcessBandWidth>(new ExceptionProcessBandWidth(CommandFailureReason.Exception, new Exception()));
                            }
                        }
                    }
                    else // иначе отправка в воркер BandWidthTaskWorker 
                    {
                        if (taskContext.Task.MeasBWResults != null)
                        {
                            taskContext.SetEvent(taskContext.Task.MeasBWResults);
                        }
                        else
                        {
                            taskContext.SetEvent<ExceptionProcessBandWidth>(new ExceptionProcessBandWidth(CommandFailureReason.Exception, new Exception()));
                        }
                    }
                }
                catch (Exception ex)
                {
                    taskContext.SetEvent((BWResult)null);
                    taskContext.SetEvent<ExceptionProcessBandWidth>(new ExceptionProcessBandWidth(CommandFailureReason.Exception, ex));
                }
            }
        }

    }
}
