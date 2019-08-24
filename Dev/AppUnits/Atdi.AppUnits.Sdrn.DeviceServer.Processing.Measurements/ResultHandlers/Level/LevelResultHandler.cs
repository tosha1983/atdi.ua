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
    public class LevelResultHandler : IResultHandler<MesureTraceCommand, MesureTraceResult, LevelTask, LevelProcess>
    {
        public void Handle(MesureTraceCommand command, MesureTraceResult result, DataModels.Sdrn.DeviceServer.ITaskContext<LevelTask, LevelProcess> taskContext)
        {
            if (result != null)
            {
                try
                {
                    var levelResult = new LevelResult();
                    if (taskContext.Task.CountMeasurementDone == 0)
                    {
                        levelResult.Level = result.Level;
                        levelResult.Freq_Hz = result.Freq_Hz;
                        levelResult.DeviceParameterState = new DeviceParameterState();
                        levelResult.DeviceParameterState.ADCOverflow = false;
                        levelResult.DeviceParameterState.Att_dB = result.Att_dB;
                        levelResult.DeviceParameterState.PreAmp_dB = result.PreAmp_dB;
                        levelResult.DeviceParameterState.RBW_Hz = result.RBW_Hz;
                        levelResult.DeviceParameterState.RefLevel_dBm = result.RefLevel_dBm;
                        levelResult.DeviceParameterState.VBW_Hz = result.VBW_Hz;
                    }
                    else
                    {
                        levelResult.Level = result.Level;
                    }
                    taskContext.Task.LevelResult = levelResult;
                    if (taskContext.Task.LevelResult != null)
                    {
                        taskContext.SetEvent(taskContext.Task.LevelResult);
                    }
                    else
                    {
                        taskContext.SetEvent<ExceptionProcessLevel>(new ExceptionProcessLevel(CommandFailureReason.Exception, new Exception("Level is null")));
                    }

                    taskContext.Task.CountMeasurementDone++;
                }
                catch (Exception ex)
                {
                    taskContext.SetEvent<ExceptionProcessLevel>(new ExceptionProcessLevel(CommandFailureReason.Exception, ex));
                }
            }
        }
    }
}
