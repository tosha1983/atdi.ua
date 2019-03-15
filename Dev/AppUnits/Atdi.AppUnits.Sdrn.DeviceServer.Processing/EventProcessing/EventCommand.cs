using Atdi.Common;
using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.Platform.Logging;
using System;
using System.Linq;
using Atdi.DataModels.EntityOrm;
using DM = Atdi.DataModels.Sdrns.Device;
using Atdi.Contracts.Api.Sdrn.MessageBus;
using System.Collections.Generic;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing
{
    /// <summary>
    /// 
    /// </summary>
    public class EventCommand<Task, Process>
        where Task: MeasurementTaskBase
        where Process: IProcess
    {
        private readonly ILogger _logger;
        private readonly ConfigProcessing _config;
        private readonly IRepository<TaskParameters, int?> _repositoryTaskParametersByInt;

        public EventCommand(ILogger logger,  IRepository<TaskParameters, int?> repositoryTaskParametersByInt,  ConfigProcessing config)
        {
            this._logger = logger;
            this._config = config;
            this._repositoryTaskParametersByInt = repositoryTaskParametersByInt;
        }

        public bool StartCommand(TaskParameters tskParam,  List<ITaskContext<Task, Process>> contextTasks,  Action action,  ref List<TaskParameters> listDeferredTasks, int cntActiveTaskParameters)
        {
            bool isSuccess = true;
            try
            {
                ITaskContext<Task, Process> findTask = null;
                if (tskParam.status == StatusTask.N.ToString())
                {
                    findTask = contextTasks.Find(z => z.Task.taskParameters.SDRTaskId == tskParam.SDRTaskId && z.Task.taskParameters.status == StatusTask.N.ToString());
                    if (findTask != null)
                    {
                        isSuccess = false;
                        return isSuccess;
                    }

                    if (tskParam.StartTime.Value > DateTime.Now)
                    {
                        TimeSpan timeSpan = tskParam.StartTime.Value - DateTime.Now;
                        //запускаем задачу в случае, если время 
                        if (timeSpan.TotalMinutes < this._config.MaxDurationBeforeStartTimeTask)
                        {
                            action.Invoke();
                        }
                        else
                        {
                            // здесь необходимо добавлять в список отложенных задач
                            if (!listDeferredTasks.Contains(tskParam))
                            {
                                listDeferredTasks.Add(tskParam);
                            }
                        }
                    }
                    else if ((tskParam.StartTime.Value <= DateTime.Now) && (tskParam.StopTime.Value >= DateTime.Now))
                    {
                        action.Invoke();
                    }
                    else
                    {
                        tskParam.status = StatusTask.C.ToString();
                        this._repositoryTaskParametersByInt.Update(tskParam);
                    }
                }
                else if (tskParam.status == StatusTask.A.ToString())
                {
                    findTask = contextTasks.Find(z => z.Task.taskParameters.SDRTaskId == tskParam.SDRTaskId && z.Task.taskParameters.status == StatusTask.A.ToString());
                    if (findTask != null)
                    {
                        isSuccess = false;
                        return isSuccess;
                    }

                    findTask = contextTasks.Find(z => z.Task.taskParameters.SDRTaskId == tskParam.SDRTaskId);
                    if (findTask != null)
                    {
                        findTask.Task.taskParameters.status = StatusTask.A.ToString();
                    }
                    else
                    {
                        if (tskParam.StartTime.Value > DateTime.Now)
                        {
                            TimeSpan timeSpan = tskParam.StartTime.Value - DateTime.Now;
                            //запускаем задачу в случае, если время 
                            if (timeSpan.TotalMinutes < this._config.MaxDurationBeforeStartTimeTask)
                            {
                                action.Invoke();
                            }
                            else
                            {
                                // здесь необходимо добавлять в список отложенных задач
                                if (!listDeferredTasks.Contains(tskParam))
                                {
                                    listDeferredTasks.Add(tskParam);
                                }
                            }
                        }
                        else if ((tskParam.StartTime.Value <= DateTime.Now) && (tskParam.StopTime.Value >= DateTime.Now))
                        {
                            action.Invoke();
                        }
                        else
                        {
                            tskParam.status = StatusTask.C.ToString();
                            this._repositoryTaskParametersByInt.Update(tskParam);
                        }
                    }
                }
                else if (tskParam.status == StatusTask.F.ToString())
                {
                    findTask = contextTasks.Find(z => z.Task.taskParameters.SDRTaskId == tskParam.SDRTaskId);
                    if (findTask != null)
                    {
                        findTask.Task.taskParameters.status = StatusTask.F.ToString();
                    }
                    else
                    {
                        if (cntActiveTaskParameters > 0)
                        {
                            if ((tskParam.StartTime.Value <= DateTime.Now) && (tskParam.StopTime.Value >= DateTime.Now))
                            {
                                tskParam.status = StatusTask.A.ToString();
                                this._repositoryTaskParametersByInt.Update(tskParam);
                                action.Invoke();
                                System.Threading.Thread.Sleep(this._config.SleepTimeForUpdateContextSOTask_ms);
                                findTask = contextTasks.Find(z => z.Task.taskParameters.SDRTaskId == tskParam.SDRTaskId);
                                if (findTask != null)
                                {
                                    findTask.Task.taskParameters.status = StatusTask.F.ToString();
                                }
                            }
                            else
                            {
                                tskParam.status = StatusTask.C.ToString();
                                this._repositoryTaskParametersByInt.Update(tskParam);
                            }
                        }
                    }
                }
                else if (tskParam.status == StatusTask.Z.ToString())
                {
                    findTask = contextTasks.Find(z => z.Task.taskParameters.SDRTaskId == tskParam.SDRTaskId);
                    if (findTask != null)
                    {
                        findTask.Task.taskParameters.status = StatusTask.Z.ToString();
                    }
                }
                contextTasks.RemoveAll(z => z.Task.taskParameters.status == StatusTask.Z.ToString() || z.Task.taskParameters.status == StatusTask.C.ToString());
            }
            catch (Exception e)
            {
                _logger.Error(Contexts.EventCommand, Categories.Processing, Exceptions.UnknownErrorEventCommand, e.Message);
            }
            return isSuccess;
        }
    }
}
