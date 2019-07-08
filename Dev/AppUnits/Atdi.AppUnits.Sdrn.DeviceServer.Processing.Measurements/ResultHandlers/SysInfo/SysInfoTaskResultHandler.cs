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
    public class SysInfoTaskResultHandler : IResultHandler<MesureTraceCommand, MesureTraceResult, SysInfoTask, SysInfoProcess>
    {
        public void Handle(MesureTraceCommand command, MesureTraceResult result, DataModels.Sdrn.DeviceServer.ITaskContext<SysInfoTask, SysInfoProcess> taskContext)
        {
            if (result != null)
            {
                try
                {
                   
                    SysInfoResult measSysInfoResults = null;
                    var parentProcess = taskContext.Descriptor.Parent;
                    if (parentProcess != null)
                    {
                        if ((parentProcess is DataModels.Sdrn.DeviceServer.ITaskContext<SignalizationTask, SignalizationProcess>) == true)
                        {
                            // здесь преобразование
                            //measSysInfoResults = result
                        }
                       
                    }

                    taskContext.Task.sysInfoResult = new SysInfoResult();
                        

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
                    taskContext.SetEvent<ExceptionProcessSysInfo>(new ExceptionProcessSysInfo(CommandFailureReason.Exception, ex));
                }
            }
        }

    }
}
