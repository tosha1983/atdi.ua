using Atdi.Common;
using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Commands;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Results;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.Platform.Logging;
using System;
using DM = Atdi.DataModels.Sdrns.Device;
using Atdi.AppUnits.Sdrn.DeviceServer.Messaging.Convertor;
using System.Threading;
using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Platform.DependencyInjection;
using Atdi.DataModels.EntityOrm;
using Atdi.DataModels.Sdrns.Device;


namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements
{
    public class BandWidthTaskWorker : ITaskWorker<BandWidthTask, BandWidthProcess, SingletonTaskWorkerLifetime>
    {
        private readonly IController _controller;
        private readonly IBusGate _busGate;
        private readonly IProcessingDispatcher _processingDispatcher;
        private readonly ITimeService _timeService;
        private readonly ITaskStarter _taskStarter;
        private readonly ILogger _logger;
        private readonly IRepository<TaskParameters, int?> _repositoryTaskParametersByInt;


        public BandWidthTaskWorker(ITimeService timeService,
            IProcessingDispatcher processingDispatcher,
            ITaskStarter taskStarter,
            ILogger logger,
            IBusGate busGate,
            IRepository<TaskParameters, int?> repositoryTaskParametersByInt,
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


        public void Run(ITaskContext<BandWidthTask, BandWidthProcess> context)
        {
            try
            {
                _logger.Verbouse(Contexts.BandWidthTaskWorker, Categories.Measurements, Events.StartBandWidthTaskWorker.With(context.Task.Id));
                if (context.Process.Parent != null)
                {
                    if (context.Process.Parent is DispatchProcess)
                    {
                        (context.Process.Parent as DispatchProcess).contextBandWidthTasks.Add(context);
                    }
                }

                DateTime dateTimeNow = DateTime.Now;
                TimeSpan waitStartTask = context.Task.taskParameters.StartTime.Value - dateTimeNow;
                if (waitStartTask.TotalMilliseconds > 0)
                {
                    //Засыпаем до начала выполнения задачи
                    Thread.Sleep((int)waitStartTask.TotalMilliseconds);
                }

                var maximumDurationMeas = CalculateTimeSleep(context.Task.taskParameters, context.Task.CountMeasurementDone);

                ////////////////////////////////////////////////////////////////////////////////////////////////////
                // 
                //  Вычисление MesureTraceParameter
                // 
                ////////////////////////////////////////////////////////////////////////////////////////////////////







                //////////////////////////////////////////////
                // 
                // Отправка команды в контроллер 
                //
                //////////////////////////////////////////////
                var deviceCommand = new MesureTraceCommand(context.Task.mesureTraceParameter);
                DateTime currTime = DateTime.Now;
                _logger.Info(Contexts.BandWidthTaskWorker, Categories.Measurements, Events.SendMeasureTraceCommandToController.With(deviceCommand.Id));
                this._controller.SendCommand<MesureTraceResult>(context, deviceCommand,
                (
                    ITaskContext taskContext, ICommand command, CommandFailureReason failureReason, Exception ex
                ) =>
                {
                    taskContext.SetEvent<ExceptionProcessBandWidth>(new ExceptionProcessBandWidth(failureReason, ex));
                });
                //////////////////////////////////////////////
                // 
                // Получение очередного  результат от Result Handler
                //
                //////////////////////////////////////////////
                MeasResults outResultData = null;
                bool isDown = context.WaitEvent<MeasResults>(out outResultData, (int)context.Task.maximumTimeForWaitingResultBandWidth);
                if (isDown == false) // таймут - результатов нет
                {
                    var error = new ExceptionProcessBandWidth();
                    if (context.WaitEvent<ExceptionProcessBandWidth>(out error, 1) == true)
                    {
                        if (error._ex != null)
                        {
                            /// реакция на ошибку выполнения команды
                            _logger.Error(Contexts.BandWidthTaskWorker, Categories.Measurements, Events.HandlingErrorSendCommandController.With(deviceCommand.Id));
                            switch (error._failureReason)
                            {
                                case CommandFailureReason.DeviceIsBusy:
                                case CommandFailureReason.CanceledExecution:
                                case CommandFailureReason.TimeoutExpired:
                                case CommandFailureReason.CanceledBeforeExecution:
                                    _logger.Error(Contexts.BandWidthTaskWorker, Categories.Measurements, Events.SleepThread.With(deviceCommand.Id, (int)maximumDurationMeas));
                                    Thread.Sleep((int)maximumDurationMeas);
                                    return;
                                case CommandFailureReason.NotFoundConvertor:
                                case CommandFailureReason.NotFoundDevice:
                                    var durationToRepietMeas = (int)maximumDurationMeas * (int)context.Task.SOKoeffWaitingDevice;
                                    TimeSpan durationToFinishTask = context.Task.taskParameters.StopTime.Value - DateTime.Now;
                                    if (durationToRepietMeas < durationToFinishTask.TotalMilliseconds)
                                    {
                                        _logger.Error(Contexts.BandWidthTaskWorker, Categories.Measurements, Events.TaskIsCancled.With(context.Task.Id));
                                        context.Cancel();
                                        return;
                                    }
                                    else
                                    {
                                        _logger.Error(Contexts.BandWidthTaskWorker, Categories.Measurements, Events.SleepThread.With(deviceCommand.Id, durationToRepietMeas));
                                        Thread.Sleep(durationToRepietMeas);
                                    }
                                    break;
                                case CommandFailureReason.Exception:
                                    _logger.Error(Contexts.BandWidthTaskWorker, Categories.Measurements, Events.TaskIsCancled.With(context.Task.Id));
                                    context.Cancel();
                                    return;
                                default:
                                    throw new NotImplementedException($"Type {error._failureReason} not supported");
                            }
                        }
                    }
                }
                else
                {
                    // есть результат
                }


                var action = new Action(() =>
                {
                        //реакция на принятые результаты измерения
                    if (outResultData != null)
                    {
                        //////////////////////////////////////////////
                        // 
                        //  Здесь получаем данные с GPS приемника
                        //  
                        //////////////////////////////////////////////
                        if (context.Process.Parent != null)
                        {
                            if (context.Process.Parent is DispatchProcess)
                            {
                                outResultData.Location.ASL = (context.Process.Parent as DispatchProcess).Asl;
                                outResultData.Location.Lon = (context.Process.Parent as DispatchProcess).Lon;
                                outResultData.Location.Lat = (context.Process.Parent as DispatchProcess).Lat;
                            }
                        }
                        outResultData.TaskId = context.Task.taskParameters.SDRTaskId;
                            //Отправка результатов в шину 
                            var publisher = this._busGate.CreatePublisher("main");
                        publisher.Send<DM.MeasResults>("SendMeasResults", outResultData);
                        publisher.Dispose();
                        context.Task.MeasResults = null;
                        context.Task.LastTimeSend = currTime;
                    }
                });


                //////////////////////////////////////////////
                // 
                //  Принять решение о полноте результатов
                //  
                //////////////////////////////////////////////


                //////////////////////////////////////////////
                // 
                // Принятие решение о завершении таска
                // 
                //
                //////////////////////////////////////////////




            }
            catch (Exception e)
            {
                _logger.Error(Contexts.BandWidthTaskWorker, Categories.Measurements, Exceptions.UnknownErrorBandWidthTaskWorker, e.Message);
                context.Abort(e);
            }
        }



        /// <summary>
        ///Вычисление задержки выполнения потока результатом является количество vмилисекунд на которое необходимо приостановить поток
        /// </summary>
        /// <param name="taskParameters">Параметры таска</param> 
        /// <param name="doneCount">Количество измерений которое было проведено</param>
        /// <returns></returns>
        private long CalculateTimeSleep(TaskParameters taskParameters, int DoneCount)
        {
            DateTime dateTimeNow = DateTime.Now;
            if (dateTimeNow > taskParameters.StopTime.Value) { return -1; }
            TimeSpan interval = taskParameters.StopTime.Value - dateTimeNow;
            double interval_ms = interval.TotalMilliseconds;
            if (taskParameters.NCount <= DoneCount) { return -1; }
            long duration = (long)(interval_ms / (taskParameters.NCount - DoneCount));
            return duration;
        }
    }
}
