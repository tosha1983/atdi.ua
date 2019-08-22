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
    public class BandWidthTaskWorker : ITaskWorker<BandWidthTask, BandWidthProcess, SingletonTaskWorkerLifetime>
    {
        private readonly IController _controller;
        private readonly IBusGate _busGate;
        private readonly IProcessingDispatcher _processingDispatcher;
        private readonly ITimeService _timeService;
        private readonly ITaskStarter _taskStarter;
        private readonly ILogger _logger;
        private readonly IRepository<MeasResults, string> _measResultsByStringRepository;
        private readonly IRepository<DM.DeviceCommandResult, string> _repositoryDeviceCommandResult;


        public BandWidthTaskWorker(ITimeService timeService,
            IProcessingDispatcher processingDispatcher,
            ITaskStarter taskStarter,
            ILogger logger,
            IBusGate busGate,
            IRepository<MeasResults, string> measResultsByStringRepository,
            IRepository<DM.DeviceCommandResult, string> repositoryDeviceCommandResult,
            IController controller)
        {
            this._processingDispatcher = processingDispatcher;
            this._timeService = timeService;
            this._taskStarter = taskStarter;
            this._logger = logger;
            this._busGate = busGate;
            this._measResultsByStringRepository = measResultsByStringRepository;
            this._controller = controller;
            this._repositoryDeviceCommandResult = repositoryDeviceCommandResult;
        }


        public void Run(ITaskContext<BandWidthTask, BandWidthProcess> context)
        {
            try
            {
                _logger.Verbouse(Contexts.BandWidthTaskWorker, Categories.Measurements, Events.StartBandWidthTaskWorker.With(context.Task.taskParameters.SDRTaskId));
                if (context.Process.Parent != null)
                {
                    if (context.Process.Parent is DispatchProcess)
                    {
                        (context.Process.Parent as DispatchProcess).contextBandWidthTasks.Add(context);
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
                            parentProc.SetEvent<ExceptionProcessBandWidth>(new ExceptionProcessBandWidth(failureReason, ex));
                        });
                    }
                    else
                    {
                        this._controller.SendCommand<MesureTraceResult>(context, deviceCommand,
                        (
                            ITaskContext taskContext, ICommand command, CommandFailureReason failureReason, Exception ex
                        ) =>
                        {
                            taskContext.SetEvent<ExceptionProcessBandWidth>(new ExceptionProcessBandWidth(failureReason, ex));
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
                        taskContext.SetEvent<ExceptionProcessBandWidth>(new ExceptionProcessBandWidth(failureReason, ex));
                    });
                }


                //////////////////////////////////////////////
                // 
                // Получение очередного  результат от Result Handler
                //
                //////////////////////////////////////////////
                ///
                if ((parentProc == null) || ((parentProc!=null) && ((parentProc is DataModels.Sdrn.DeviceServer.ITaskContext<SignalizationTask, SignalizationProcess>) == false)))
                {
                    
                    BWResult outResultData = null;
                    bool isDown = context.WaitEvent<BWResult>(out outResultData, (int)(context.Task.durationForMeasBW_ms));
                    if (isDown == false) // таймут - результатов нет
                    {
                        //context.Task.CountGetResultBWNegative++;
                        var error = new ExceptionProcessBandWidth();
                        if (context.WaitEvent<ExceptionProcessBandWidth>(out error, 1) == true)
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
                            //measResult.ResultId = string.Format("{0}|{1}", context.Task.taskParameters.SDRTaskId, context.Task.CountSendResults);
                            measResult.TaskId = context.Task.taskParameters.SDRTaskId;
                            measResult.ResultId = Guid.NewGuid().ToString();
                            measResult.ScansNumber = context.Task.CountSendResults;
                            measResult.Status = "N";
                            measResult.Measurement = DataModels.Sdrns.MeasurementType.BandwidthMeas;
                            measResult.Levels_dBm = outResultData.Levels_dBm;
                            if ((outResultData.Freq_Hz != null) && (outResultData.Freq_Hz.Length > 0))
                            {
                                var floatArray = outResultData.Freq_Hz.Select(x => (float)x).ToList();
                                measResult.Frequencies = floatArray.ToArray();
                            }
                            measResult.BandwidthResult = new BandwidthMeasResult();
                            measResult.BandwidthResult.MarkerIndex = outResultData.MarkerIndex;
                            measResult.BandwidthResult.Bandwidth_kHz = outResultData.Bandwidth_kHz;
                            measResult.BandwidthResult.T1 = outResultData.T1;
                            measResult.BandwidthResult.T2 = outResultData.T2;
                            measResult.BandwidthResult.СorrectnessEstimations = outResultData.СorrectnessEstimations;
                            measResult.BandwidthResult.TraceCount = outResultData.TraceCount;
                            measResult.StartTime = context.Task.LastTimeSend.Value;
                            measResult.StopTime = currTime;
                            measResult.Location = new DataModels.Sdrns.GeoLocation();
                            measResult.Measured = currTime;
                            //////////////////////////////////////////////
                            // 
                            //  Здесь получаем данные с GPS приемника
                            //  
                            //////////////////////////////////////////////
                            bool isParentProcess = false;
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
                                            isParentProcess = true;
                                        }
                                        else
                                        {
                                            _logger.Error(Contexts.BandWidthTaskWorker, Categories.Measurements, Exceptions.ErrorConvertToDispatchProcess, Exceptions.AfterConvertParentProcessIsNull);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.Error(Contexts.BandWidthTaskWorker, Categories.Measurements, Exceptions.ErrorConvertToDispatchProcess, ex.Message);
                                    }
                                }
                                else
                                {
                                    _logger.Error(Contexts.BandWidthTaskWorker, Categories.Measurements, Exceptions.ErrorConvertToDispatchProcess, Exceptions.ParentProcessIsNotTypeDispatchProcess);
                                }
                            }
                            else
                            {
                                _logger.Error(Contexts.BandWidthTaskWorker, Categories.Measurements, Exceptions.ErrorConvertToDispatchProcess, Exceptions.ParentProcessIsNull);
                            }
                            
                            //Отправка результатов в шину 
                            if (isParentProcess == false)
                            {
                                this._measResultsByStringRepository.Create(measResult);

                                DM.DeviceCommandResult deviceCommandResult = new DM.DeviceCommandResult();
                                deviceCommandResult.CommandId = "UpdateStatusMeasTask";
                                deviceCommandResult.CustDate1 = DateTime.Now;
                                deviceCommandResult.Status = StatusTask.C.ToString();
                                deviceCommandResult.CustTxt1 = context.Task.taskParameters.SDRTaskId;
                                this._repositoryDeviceCommandResult.Create(deviceCommandResult);

                            }
                            context.Task.MeasBWResults = null;
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
                _logger.Info(Contexts.BandWidthTaskWorker, Categories.Measurements, Events.FinishedBandWidthTaskWorker);

            }
            catch (Exception e)
            {
                _logger.Error(Contexts.BandWidthTaskWorker, Categories.Measurements, Exceptions.UnknownErrorBandWidthTaskWorker, e.Message);
                context.Abort(e);
            }
        }
    }
}
