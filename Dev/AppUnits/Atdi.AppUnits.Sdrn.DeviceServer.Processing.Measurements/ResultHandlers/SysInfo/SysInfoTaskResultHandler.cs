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
    public class SysInfoTaskResultHandler : IResultHandler<MesureSystemInfoCommand, MesureSystemInfoResult, SysInfoTask, SysInfoProcess>
    {
        public void Handle(MesureSystemInfoCommand command, MesureSystemInfoResult tempResult, DataModels.Sdrn.DeviceServer.ITaskContext<SysInfoTask, SysInfoProcess> taskContext)
        {
            if (tempResult != null)
            {
                var result = CopyHelper.CreateDeepCopy(tempResult);

                taskContext.Task.sysInfoResult = new SysInfoResult();
                try
                {
                    var parentProcess = taskContext.Descriptor.Parent;
                    if (parentProcess != null)
                    {
                        if ((parentProcess is DataModels.Sdrn.DeviceServer.ITaskContext<SignalizationTask, SignalizationProcess>) == true)
                        {
                            if ((result.SystemInfo != null) && (result.SystemInfo.Length > 0))
                            {
                                taskContext.Task.sysInfoResult = CommonConvertors.ConvertToSysInfoResult(result);
                            }
                        }
                    }
                 
                    // Отправка результата в родительский процесс (если он есть)
                    if (parentProcess != null)
                    {
                        ///если родительский контекст - сигнализация, то отправить результат 
                        if (parentProcess is DataModels.Sdrn.DeviceServer.ITaskContext<SignalizationTask, SignalizationProcess>)
                        {
                            if (taskContext.Task.sysInfoResult != null)
                            {
                                parentProcess.SetEvent(taskContext.Task.sysInfoResult);
                            }
                            else
                            {
                                parentProcess.SetEvent<ExceptionProcessSysInfo>(new ExceptionProcessSysInfo(CommandFailureReason.Exception, new Exception()));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    taskContext.SetEvent((SysInfoResult)(null));
                    taskContext.SetEvent<ExceptionProcessSysInfo>(new ExceptionProcessSysInfo(CommandFailureReason.Exception, ex));
                }
            }
        }

    }
}
