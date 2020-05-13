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
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing
{
    /// <summary>
    /// 
    /// </summary>
    public class CommandHandler<Task, Process>
        where Task : MeasurementTaskBase
        where Process : IProcess
    {
        private readonly ILogger _logger;
        private readonly ConfigProcessing _config;
        private readonly IRepository<TaskParameters, string> _repositoryTaskParametersByString;

        public CommandHandler(ILogger logger, IRepository<TaskParameters, string> repositoryTaskParametersByString, ConfigProcessing config)
        {
            this._logger = logger;
            this._config = config;
            this._repositoryTaskParametersByString = repositoryTaskParametersByString;
        }

        private ITaskContext<Task, Process> FindTask(ConcurrentBag<ITaskContext<Task, Process>> contextTasks, TaskParameters tskParam, StatusTask statusTask, ref bool isSuccess)
        {
            isSuccess = true;
            ITaskContext<Task, Process> findTask = null;
            if (contextTasks != null)
            {
                for (int i = 0; i < contextTasks.Count; i++)
                {
                    var val = contextTasks.ElementAt(i);
                    if (val != null)
                    {
                        if (val.Task != null)
                        {
                            if (val.Task.taskParameters != null)
                            {
                                if ((val.Task.taskParameters.SDRTaskId == tskParam.SDRTaskId) && (val.Task.taskParameters.status == statusTask.ToString()))
                                {
                                    isSuccess = false;
                                    findTask = val;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return findTask;
        }

        private ITaskContext<Task, Process> FindTask(ConcurrentBag<ITaskContext<Task, Process>> contextTasks, TaskParameters tskParam, ref bool isSuccess)
        {
            isSuccess = true;
            ITaskContext<Task, Process> findTask = null;
            if (contextTasks != null)
            {
                for (int i = 0; i < contextTasks.Count; i++)
                {
                    var val = contextTasks.ElementAt(i);
                    if (val != null)
                    {
                        if (val.Task != null)
                        {
                            if (val.Task.taskParameters != null)
                            {
                                if (val.Task.taskParameters.SDRTaskId == tskParam.SDRTaskId)
                                {
                                    isSuccess = false;
                                    findTask = val;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return findTask;
        }

        private bool FindTask(ConcurrentBag<TaskParameters> listDeferredTasks, TaskParameters tskParam)
        {
            bool isFindTask = false;
            if (listDeferredTasks != null)
            {
                for (int i = 0; i < listDeferredTasks.Count; i++)
                {
                    var val = listDeferredTasks.ElementAt(i);
                    if (val != null)
                    {
                        if (val.SDRTaskId == tskParam.SDRTaskId)
                        {
                            isFindTask = true;
                            break;
                        }
                    }
                }
            }
            return isFindTask;
        }



        public bool StartCommand(TaskParameters tskParam, ref ConcurrentBag<ITaskContext<Task, Process>> contextTasks, Action action, ref ConcurrentBag<TaskParameters> listDeferredTasks, ref List<string> listRunTask, int cntActiveTaskParameters)
        {
            bool isSuccess = true;
            try
            {
                var concurrectBagCopy = contextTasks.ToList();
                ITaskContext<Task, Process> findTask = null;
                if (tskParam.status == StatusTask.A.ToString())
                {
                    findTask = FindTask(contextTasks, tskParam, StatusTask.A, ref isSuccess);
                    if (findTask != null)
                    {
                        isSuccess = false;
                        return isSuccess;
                    }

                    findTask = FindTask(contextTasks, tskParam, ref isSuccess);
                    if (findTask != null)
                    {
                        for (int m = 0; m < contextTasks.Count; m++)
                        {
                            var val = contextTasks.ElementAt(m);
                            if (val.Task != null)
                            {
                                if (val.Task.taskParameters != null)
                                {
                                    if (val.Task.taskParameters.SDRTaskId == tskParam.SDRTaskId)
                                    {
                                        val.Task.taskParameters.status = StatusTask.A.ToString();
                                        break;
                                    }
                                }
                            }
                        }
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
                                tskParam.status = StatusTask.A.ToString();
                                this._repositoryTaskParametersByString.Update(tskParam);
                            }
                            else
                            {
                                // здесь необходимо добавлять в список отложенных задач
                                var findTaskDeffered = FindTask(listDeferredTasks, tskParam);
                                if (findTaskDeffered == false)
                                {
                                    listDeferredTasks.Add(tskParam);
                                }
                            }
                        }
                        else if ((tskParam.StartTime.Value <= DateTime.Now) && (tskParam.StopTime.Value >= DateTime.Now))
                        {
                            action.Invoke();
                            tskParam.status = StatusTask.A.ToString();
                            this._repositoryTaskParametersByString.Update(tskParam);
                        }
                        else
                        {
                            if (tskParam.StopTime.Value < DateTime.Now)
                            {
                                tskParam.status = StatusTask.C.ToString();
                                this._repositoryTaskParametersByString.Update(tskParam);
                            }
                        }
                    }
                }
                else if (tskParam.status == StatusTask.F.ToString())
                {
                    findTask = FindTask(contextTasks, tskParam, ref isSuccess);
                    if (findTask != null)
                    {
                        for (int m = 0; m < contextTasks.Count; m++)
                        {
                            var val = contextTasks.ElementAt(m);
                            if (val.Task != null)
                            {
                                if (val.Task.taskParameters != null)
                                {
                                    if (val.Task.taskParameters.SDRTaskId == tskParam.SDRTaskId)
                                    {
                                        val.Task.taskParameters.status = StatusTask.F.ToString();
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (cntActiveTaskParameters > 0)
                        {
                            if ((tskParam.StartTime.Value <= DateTime.Now) && (tskParam.StopTime.Value >= DateTime.Now))
                            {
                                tskParam.status = StatusTask.F.ToString();
                                action.Invoke();
                                this._repositoryTaskParametersByString.Update(tskParam);
                            }
                            else
                            {
                                if (tskParam.StopTime.Value < DateTime.Now)
                                {
                                    tskParam.status = StatusTask.C.ToString();
                                    this._repositoryTaskParametersByString.Update(tskParam);
                                }
                                else
                                {
                                    if (tskParam.StartTime.Value > DateTime.Now)
                                    {
                                        var findTaskDeffered = FindTask(listDeferredTasks, tskParam);
                                        if (findTaskDeffered == false)
                                        {
                                            listDeferredTasks.Add(tskParam);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (tskParam.status == StatusTask.Z.ToString())
                {
                    findTask = FindTask(contextTasks, tskParam, ref isSuccess);
                    if (findTask != null)
                    {
                        findTask.Task.taskParameters.status = StatusTask.Z.ToString();
                        this._repositoryTaskParametersByString.Update(tskParam);
                        listRunTask.Remove(tskParam.SDRTaskId);
                        contextTasks = contextTasks.Remove(findTask);
                    }
                    for (int i = 0; i < listDeferredTasks.Count; i++)
                    {
                        var valDeferredTask = listDeferredTasks.ElementAt(i);
                        if (valDeferredTask != null)
                        {
                            if (valDeferredTask.SDRTaskId == tskParam.SDRTaskId)
                            {
                                listDeferredTasks = listDeferredTasks.Remove(valDeferredTask);
                                break;
                            }
                        }
                    }
                }
                for (int m = 0; m < contextTasks.Count; m++)
                {
                    var val = contextTasks.ElementAt(m);
                    if (val.Task != null)
                    {
                        if (val.Task.taskParameters != null)
                        {
                            if ((val.Task.taskParameters.status == StatusTask.Z.ToString()) || (val.Task.taskParameters.status == StatusTask.C.ToString()))
                            {
                                contextTasks = contextTasks.Remove(val);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.EventCommand, Categories.Processing, Exceptions.UnknownErrorEventCommand, e);
            }
            return isSuccess;
        }
    }
}
