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
                //  constant
                bool Smooth = true; // параметер прокинуть туда откуда береться и taskContext.Task.bandwidthEstimationType там его и присвоить
                BandWidthEstimation.BandwidthEstimationType bandwidthEstimationTypeDefault = BandWidthEstimation.BandwidthEstimationType.xFromCentr;
                double X_beta = 25;
                // end constant

                try
                {
                    float[] Levels = result.Level;
                    if (Smooth) { Levels = SmoothTrace.blackman(Levels); }
                    int MaximumIgnorPoint =(int)Math.Round(result.Level.Length / 300.0);
                    MeasBandwidthResult measBandWidthResults = null;
                    var parentProcess = taskContext.Descriptor.Parent;
                    if (parentProcess != null)
                    {
                        if ((parentProcess is DataModels.Sdrn.DeviceServer.ITaskContext<SignalizationTask, SignalizationProcess>) == true)
                        {
                            BandWidthEstimation.BandwidthEstimationType bandwidthEstimationType;
                            if (Enum.TryParse(taskContext.Task.bandwidthEstimationType, out bandwidthEstimationType))
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
                            ((result.Freq_Hz[result.Freq_Hz.Length - 1] - result.Freq_Hz[0]) / (result.Freq_Hz.Length - 1))) / 1000;

                        taskContext.Task.MeasBWResults.Levels_dBm = result.Level;
                        taskContext.Task.MeasBWResults.Freq_Hz = result.Freq_Hz;
                        taskContext.Task.MeasBWResults.СorrectnessEstimations = measBandWidthResults.СorrectnessEstimations.Value;

                        /*
                        string val = "";
                        string newVal = "";
                        if ((taskContext.Task.MeasBWResults.Levels_dBm != null) && (taskContext.Task.MeasBWResults.Levels_dBm.Length > 50))
                        {
                            val = string.Join(",", taskContext.Task.MeasBWResults.Levels_dBm);
                            newVal = val;
                        }
                        */

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
                    taskContext.SetEvent<ExceptionProcessBandWidth>(new ExceptionProcessBandWidth(CommandFailureReason.Exception, ex));
                }
            }
        }

    }
}
