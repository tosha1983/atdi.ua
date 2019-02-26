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
using Atdi.Api.Sdrn.Device.BusController;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements
{
    public class SOTaskWorker : ITaskWorker<SOTask, MeasProcess, SingletonTaskWorkerLifetime>
    {
        private readonly IController _controller;
        private readonly IBusGate _busGate;
        private readonly IProcessingDispatcher _processingDispatcher;
        private readonly ITimeService _timeService;
        private readonly ITaskStarter _taskStarter;
        private readonly ILogger _logger;



        public SOTaskWorker(ITimeService timeService, IProcessingDispatcher processingDispatcher, ITaskStarter taskStarter, ILogger logger, IBusGate busGate, IController controller)
        {
            this._processingDispatcher = processingDispatcher;
            this._timeService = timeService;
            this._taskStarter = taskStarter;
            this._logger = logger;
            this._busGate = busGate;
            this._controller = controller;
        }

    
        public void Run(ITaskContext<SOTask, MeasProcess> context)
        {
            try
            {
                while (true)
                {
                    // проверка - не отменили ли задачу
                    if (context.Token.IsCancellationRequested)
                    {
                        context.Cancel();
                        return;
                    }
                    //////////////////////////////////////////////
                    // 
                    //  Послать команду DeviceCotroler MeaseTrace
                    // 
                    //
                    //////////////////////////////////////////////
                    // Формирование команды (инициализация начальными параметрами) перед отправкой в контроллер
                    var deviceCommand = new MesureTraceCommand(context.Task.mesureTraceParameter)
                    {
                        Options = CommandOption.PutInQueue
                    };

                    //////////////////////////////////////////////
                    // 
                    // Отправка команды в контроллер (причем context уже содержит информацию о сообщение с шины RabbitMq)
                    //
                    //////////////////////////////////////////////
                    this._controller.SendCommand<MesureTraceResult>(context, deviceCommand,
                    (ITaskContext taskContext, ICommand command, CommandFailureReason failureReason, Exception ex
                    ) =>  {
                        taskContext.SetEvent<ExceptionProcessSO>(new ExceptionProcessSO(failureReason, ex));
                    });
                    //////////////////////////////////////////////
                    // 
                    // Получение очередного  результат от Result Handler
                    //
                    //
                    //////////////////////////////////////////////
                    SpectrumOcupationResult outSpectrumOcupation = null;
                    DM.MeasResults measResult = null;
                    bool isDown = false;
                    while (isDown == false)
                    {
                        isDown = context.WaitEvent<SpectrumOcupationResult>(out outSpectrumOcupation, 10);
                        if (isDown == false) // таймут - результатов нет
                        {
                            // проверка - не отменили ли задачу
                            if (context.Token.IsCancellationRequested)
                            {
                                // явно нужна логика отмены
                                context.Cancel();
                                return;
                            }

                            if (context.WaitEvent<ExceptionProcessSO>(10) == true)
                            {
                                /// реакция на ошибку выполнения команды
                                isDown = true;
                                break;
                            }
                        }
                        else
                        {
                            //реакция на принятые результаты измерения
                            measResult = new DM.MeasResults();
                            if (outSpectrumOcupation.fSemplesResult != null)
                            {
                                measResult.FrequencySamples = outSpectrumOcupation.fSemplesResult.Convert();
                                measResult.ScansNumber = outSpectrumOcupation.NN;
                            }
                            isDown = true;
                            break;
                        }
                    }
                    // проверка - не отменили ли задачу
                    if (context.Token.IsCancellationRequested)
                    {
                        context.Cancel();
                        return;
                    }

                    //////////////////////////////////////////////
                    // 
                    //  Принять решение о полноте результатов
                    //  
                    //////////////////////////////////////////////
                    bool isSendResultToBus = false;
                    if ((context.Task.LastTimeSend!=null) && (measResult!=null))
                    {
                        var sub = DateTime.Now.Subtract(context.Task.LastTimeSend.Value);
                        if (sub.TotalHours>1)
                        {
                            var hour = DateTime.Now.Hour;
                            context.Task.LastTimeSend = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hour, 0, 0);
                            // В этом случае отправка выполняется
                            if (outSpectrumOcupation != null)
                            {
                                if (outSpectrumOcupation.fSemplesResult != null)
                                {
                                    //Отправка результатов в шину 
                                    var publisher = this._busGate.CreatePublisher("main");
                                    publisher.Send<DM.MeasResults>("SendMeasResults", measResult);
                                    publisher.Dispose();
                                    isSendResultToBus = true;
                                }
                            }
                            context.Task.MeasResults = measResult;
                        }
                    }
                    else
                    {
                        //вычисляем как текущее время назад (с округлением до часа)
                        var hour = DateTime.Now.Hour;
                        context.Task.LastTimeSend = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hour, 0, 0); 
                        // В этом случае отправка не выполняется
                    }

                    //////////////////////////////////////////////
                    // 
                    // Принятие решение о завершении таска
                    // 
                    //
                    //////////////////////////////////////////////
                    if (DateTime.Now > context.Task.taskParameters.StopTime)
                    {
                        // Здесь отправка последнего таска 
                        //(С проверкой - чтобы не отправляллся дубликат)
                        if ((isSendResultToBus==false) && (measResult!=null))
                        {
                            var publisher = this._busGate.CreatePublisher("main");
                            publisher.Send<DM.MeasResults>("SendMeasResults", measResult);
                            publisher.Dispose();
                        }
                        context.Finish();
                        break;
                    }
                    

                    //////////////////////////////////////////////
                    // 
                    // Приостановка потока на рассчитаное время CalculateSleepParameter(context.Task.taskParameters)
                    //
                    //////////////////////////////////////////////
                    var sleepTime = CalculateTimeSleep(context.Task.taskParameters, context.Task.CountMeasurementDone.Value);
                    Thread.Sleep(sleepTime);
                }

                //this._controller.SendCommand<MesureTraceResult>(context, deviceCommand);
                /// теперь шлем задаче родителю евент
                //context.Descriptor.Parent.SetEvent(result);

               
                context.Task.CountMeasurementDone++;

            }
            catch (Exception e)
            {
                context.Abort(e);
            }
        }

        /// <summary>
        ///Вычисление задержки выполнения потока результатом является количество vмилисекунд на которое необходимо приостановить поток
        /// </summary>
        /// <param name="taskParameters">Параметры таска</param> 
        /// <param name="doneCount">Количество измерений которое было проведено</param>
        /// <returns></returns>
        private int CalculateTimeSleep(TaskParameters taskParameters, int DoneCount)
        {
            DateTime dateTimeNow = DateTime.Now;
            if (dateTimeNow > taskParameters.StopTime.Value) { return -1; }
            TimeSpan interval = taskParameters.StopTime.Value - dateTimeNow;
            int interval_ms = interval.Milliseconds;
            if (taskParameters.NCount.Value <= DoneCount) { return -1; }
            int duration = (int)(interval_ms / (taskParameters.NCount.Value - DoneCount));
            return duration;
        }
    }
}
