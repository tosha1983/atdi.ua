using Atdi.Common;
using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Commands;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Results;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.Platform.Logging;
using System;
using DM = Atdi.DataModels.Sdrns.Device;
using System.Threading;
using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Platform.DependencyInjection;
using Atdi.DataModels.EntityOrm;
using Atdi.DataModels.Sdrns.Device;
using System.Linq;


namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements
{
    public class SysInfoTaskWorker : ITaskWorker<SysInfoTask, SysInfoProcess, SingletonTaskWorkerLifetime>
    {
        private readonly IController _controller;
        private readonly IBusGate _busGate;
        private readonly IProcessingDispatcher _processingDispatcher;
        private readonly ITimeService _timeService;
        private readonly ITaskStarter _taskStarter;
        private readonly ILogger _logger;
        private readonly IRepository<TaskParameters, long?> _repositoryTaskParametersByInt;


        public SysInfoTaskWorker(ITimeService timeService,
            IProcessingDispatcher processingDispatcher,
            ITaskStarter taskStarter,
            ILogger logger,
            IBusGate busGate,
            IRepository<TaskParameters, long?> repositoryTaskParametersByInt,
            IController controller)
        {
            this._processingDispatcher = processingDispatcher;
            this._timeService = timeService;
            this._taskStarter = taskStarter;
            this._logger = logger;
            this._busGate = busGate;
            this._controller = controller;
            this._repositoryTaskParametersByInt = repositoryTaskParametersByInt;
        }


        public void Run(ITaskContext<SysInfoTask, SysInfoProcess> context)
        {
            try
            {
                _logger.Verbouse(Contexts.SysInfoTaskWorker, Categories.Measurements, Events.StartSysInfoTaskWorker.With(context.Task.Id));
                if (context.Process.Parent != null)
                {
                    if (context.Process.Parent is DispatchProcess)
                    {
                        (context.Process.Parent as DispatchProcess).contextSysInfoTasks.Add(context);
                    }
                }



                var parentProc = context.Descriptor.Parent;
                //////////////////////////////////////////////
                // 
                // Отправка команды в контроллер 
                //
                //////////////////////////////////////////////
                var datenow = DateTime.Now;
                //context.Task.CountCallBW++;
                var deviceCommand = new MesureTraceCommand(context.Task.mesureTraceParameter);
                if (context.Task.durationForMeasBW_ms > 0)
                {
                    deviceCommand.Timeout = context.Task.durationForMeasBW_ms;
                }
                else
                {
                    deviceCommand.Timeout = 200;
                }
                deviceCommand.Delay = 0;
                deviceCommand.Options = CommandOption.StartImmediately;
                //_logger.Info(Contexts.BandWidthTaskWorker, Categories.Measurements, "Check time start" + Events.SendMeasureTraceCommandToController.With(deviceCommand.Id));

                if (parentProc != null)
                {
                    if ((parentProc is DataModels.Sdrn.DeviceServer.ITaskContext<SignalizationTask, SignalizationProcess>) == true)
                    {
                        this._controller.SendCommand<MesureTraceResult>(context, deviceCommand,
                        (
                        ITaskContext taskContext, ICommand command, CommandFailureReason failureReason, Exception ex
                        ) =>
                        {
                            parentProc.SetEvent<ExceptionProcessSysInfo>(new ExceptionProcessSysInfo(failureReason, ex));
                        });
                    }
                    else
                    {
                        this._controller.SendCommand<MesureTraceResult>(context, deviceCommand,
                        (
                            ITaskContext taskContext, ICommand command, CommandFailureReason failureReason, Exception ex
                        ) =>
                        {
                            taskContext.SetEvent<ExceptionProcessSysInfo>(new ExceptionProcessSysInfo(failureReason, ex));
                        });
                    }
                }
                else
                {
                    this._controller.SendCommand<MesureTraceResult>(context, deviceCommand,
                    (
                        ITaskContext taskContext, ICommand command, CommandFailureReason failureReason, Exception ex
                    ) =>
                    {
                        taskContext.SetEvent<ExceptionProcessSysInfo>(new ExceptionProcessSysInfo(failureReason, ex));
                    });
                }


                //////////////////////////////////////////////
                // 
                // Получение очередного  результат от Result Handler
                //
                //////////////////////////////////////////////
                ///
                if (parentProc == null)
                {
                    SysInfoResult outResultData = null;
                    bool isDown = context.WaitEvent<SysInfoResult>(out outResultData, (int)(context.Task.durationForMeasBW_ms));
                    if (isDown == false) // таймут - результатов нет
                    {
                        //context.Task.CountGetResultBWNegative++;
                        var error = new ExceptionProcessSysInfo();
                        if (context.WaitEvent<ExceptionProcessSysInfo>(out error, 1) == true)
                        {
                            if (error._ex != null)
                            {
                                /// реакция на ошибку выполнения команды
                                _logger.Error(Contexts.BandWidthTaskWorker, Categories.Measurements, Events.HandlingErrorSendCommandController.With(deviceCommand.Id), error._ex.StackTrace);
                                context.Cancel();
                            }
                        }
                    }
                    else
                    {
                        // есть результат
                        //context.Task.CountGetResultBWPositive++;
                    }

                    DateTime currTime = DateTime.Now;
                    var action = new Action(() =>
                    {
                    //реакция на принятые результаты измерения
                    if (outResultData != null)
                        {
                            
                            DM.MeasResults measResult = new DM.MeasResults();
                            context.Task.CountSendResults++;
                            
                            measResult.ResultId = string.Format("{0}|{1}", context.Task.taskParameters.SDRTaskId, context.Task.CountSendResults);
                            measResult.Status = "N";

                            

                            measResult.StartTime = context.Task.LastTimeSend.Value;
                            measResult.StopTime = currTime;
                            measResult.Location = new DataModels.Sdrns.GeoLocation();
                            measResult.Measured = currTime;
                        //////////////////////////////////////////////
                        // 
                        //  Здесь получаем данные с GPS приемника
                        //  
                        //////////////////////////////////////////////
                        var parentProcess = context.Process.Parent;
                            if (parentProcess != null)
                            {
                                if (parentProcess is DispatchProcess)
                                {
                                    DispatchProcess dispatchProcessParent = null;
                                    try
                                    {
                                        dispatchProcessParent = (parentProcess as DispatchProcess);
                                        if (dispatchProcessParent != null)
                                        {
                                            measResult.Location.ASL = dispatchProcessParent.Asl;
                                            measResult.Location.Lon = dispatchProcessParent.Lon;
                                            measResult.Location.Lat = dispatchProcessParent.Lat;
                                        }
                                        else
                                        {
                                            _logger.Error(Contexts.SysInfoTaskWorker, Categories.Measurements, Exceptions.ErrorConvertToDispatchProcess, Exceptions.AfterConvertParentProcessIsNull);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.Error(Contexts.SysInfoTaskWorker, Categories.Measurements, Exceptions.ErrorConvertToDispatchProcess, ex.Message);
                                    }
                                }
                                else
                                {
                                    _logger.Error(Contexts.SysInfoTaskWorker, Categories.Measurements, Exceptions.ErrorConvertToDispatchProcess, Exceptions.ParentProcessIsNotTypeDispatchProcess);
                                }
                            }
                            else
                            {
                                _logger.Error(Contexts.SysInfoTaskWorker, Categories.Measurements, Exceptions.ErrorConvertToDispatchProcess, Exceptions.ParentProcessIsNull);
                            }
                            measResult.TaskId = CommonConvertors.GetTaskId(measResult.ResultId);
                        //Отправка результатов в шину 
                        var publisher = this._busGate.CreatePublisher("main");
                            publisher.Send<DM.MeasResults>("SendMeasResults", measResult);
                            publisher.Dispose();
                            context.Task.sysInfoResult = null;
                        }
                    });


                    //////////////////////////////////////////////
                    // 
                    //  Принять решение о полноте результатов
                    //  
                    //////////////////////////////////////////////
                    if (outResultData != null)
                    {
                        action.Invoke();
                    }

                }
                //////////////////////////////////////////////
                // 
                // Принятие решение о завершении таска
                // 
                //
                //////////////////////////////////////////////
                context.Finish();
                _logger.Info(Contexts.SysInfoTaskWorker, Categories.Measurements, Events.FinishedSysInfoTaskWorker);

            }
            catch (Exception e)
            {
                _logger.Error(Contexts.SysInfoTaskWorker, Categories.Measurements, Exceptions.UnknownErrorSysInfoTaskWorker, e.Message);
                context.Abort(e);
            }
        }
    }
}
