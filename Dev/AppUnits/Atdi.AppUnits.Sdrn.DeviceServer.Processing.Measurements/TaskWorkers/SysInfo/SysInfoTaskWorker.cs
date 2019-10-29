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


        public SysInfoTaskWorker(ITimeService timeService,
            IProcessingDispatcher processingDispatcher,
            ITaskStarter taskStarter,
            ILogger logger,
            IBusGate busGate,
            IController controller)
        {
            this._processingDispatcher = processingDispatcher;
            this._timeService = timeService;
            this._taskStarter = taskStarter;
            this._logger = logger;
            this._busGate = busGate;
            this._controller = controller;
        }


        public void Run(ITaskContext<SysInfoTask, SysInfoProcess> context)
        {
            try
            {
                _logger.Verbouse(Contexts.SysInfoTaskWorker, Categories.Measurements, Events.StartSysInfoTaskWorker.With(context.Task.taskParameters.SDRTaskId));
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

                for (int i = 0; i < context.Task.mesureSystemInfoParameters.Length; i++)
                {
                    var mesureSystemInfoParameter = context.Task.mesureSystemInfoParameters[i];
                    var deviceCommand = new MesureSystemInfoCommand(mesureSystemInfoParameter);
                    //deviceCommand.Delay = 10000;
                    deviceCommand.Options = CommandOption.StartImmediately;

                    if (parentProc != null)
                    {
                        if ((parentProc is DataModels.Sdrn.DeviceServer.ITaskContext<SignalizationTask, SignalizationProcess>) == true)
                        {
                            this._controller.SendCommand<MesureSystemInfoResult>(context, deviceCommand,
                            (
                            ITaskContext taskContext, ICommand command, CommandFailureReason failureReason, Exception ex
                            ) =>
                            {
                                parentProc.SetEvent<ExceptionProcessSysInfo>(new ExceptionProcessSysInfo(failureReason, ex));
                            });
                        }
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
                        //bool isDown = context.WaitEvent<SysInfoResult>(out outResultData, (int)(context.Task.durationForMeasSysInfo_ms));
                        bool isDown = context.WaitEvent<SysInfoResult>(out outResultData);
                        if (isDown == false) // таймут - результатов нет
                        {
                            var error = new ExceptionProcessSysInfo();
                            if (context.WaitEvent<ExceptionProcessSysInfo>(out error, 1) == true)
                            {
                                if (error._ex != null)
                                {
                                    /// реакция на ошибку выполнения команды
                                    _logger.Error(Contexts.SysInfoTaskWorker, Categories.Measurements, Events.HandlingErrorSendCommandController.With(deviceCommand.Id), error._ex.StackTrace);
                                    context.Cancel();
                                }
                            }
                        }
                        else
                        {
                            // есть результат
                            //context.Task.CountGetResultBWPositive++;
                        }

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
                _logger.Exception(Contexts.SysInfoTaskWorker, Categories.Measurements, Exceptions.UnknownErrorSysInfoTaskWorker, e);
                context.Abort(e);
            }
        }
    }
}
