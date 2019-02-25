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
    class SOTaskWorker : ITaskWorker<SOTask, MeasProcess, PerThreadTaskWorkerLifetime>
    {
        private readonly IController _controller;
        private readonly ILogger _logger;
        private readonly IBusGateFactory _busGateFactory;
        private readonly IBusGateConfig _busGateConfig;
        private readonly IBusGate _busGate;
        private readonly IMessagePublisher _messagePublisher;

        public SOTaskWorker(ExampleConfig config, IController controller, ILogger logger)
        {
            this._controller = controller;
            this._logger = logger;
            this._busGateFactory = BusGateFactory.Create();
            this._busGateConfig = _busGateFactory.CreateConfig();
            SetConfig(config);
            this._busGate = _busGateFactory.CreateGate("ITaskWorker", this._busGateConfig);
            this._messagePublisher = this._busGate.CreatePublisher("TaskWorker");
        }

        public void SetConfig(ExampleConfig config)
        {
            this._busGateConfig["License.FileName"] = "";
            this._busGateConfig["License.OwnerId"] = "";
            this._busGateConfig["License.ProductKey"] = "";
            this._busGateConfig["RabbitMQ.Host"] = "";
            this._busGateConfig["RabbitMQ.User"] = "";
            this._busGateConfig["RabbitMQ.Password"] = "";
            this._busGateConfig["SDRN.ApiVersion"] = "";
            this._busGateConfig["SDRN.Server.Instance"] = "";
            this._busGateConfig["SDRN.Server.QueueNamePart"] = "";
            this._busGateConfig["SDRN.Device.SensorTechId"] = "";
            this._busGateConfig["SDRN.Device.Exchange"] = "";
            this._busGateConfig["SDRN.Device.QueueNamePart"] = "";
            this._busGateConfig["SDRN.Device.MessagesBindings"] = "";
            this._busGateConfig["SDRN.MessageConvertor.UseEncryption"] = "";
            this._busGateConfig["SDRN.MessageConvertor.UseCompression"] = "";
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
                        Options = CommandOption.StartDelayed,
                        Delay = 1000,
                        StartTimeStamp = TimeStamp.Milliseconds,
                        Timeout = (long)TimeSpan.FromSeconds(2).TotalMilliseconds
                    };

                    //////////////////////////////////////////////
                    // 
                    // Отправка команды в контроллер (причем context уже содержит информацию о сообщение с шины RabbitMq)
                    //
                    //////////////////////////////////////////////
                    this._controller.SendCommand<MesureTraceResult>(context, deviceCommand, 
                        (ITaskContext taskContext, ICommand command, CommandFailureReason failureReason, Exception ex
                        ) =>
                        {
                            taskContext.SetEvent<SamthingErrorEvent>(new SamthingErrorEvent(failureReason, ex));
                        });
                    //////////////////////////////////////////////
                    // 
                    // Получение очередного  результат от Result Handler
                    //
                    //
                    //////////////////////////////////////////////
                    SpectrumOcupation outSpectrumOcupation = null;

                    while (isDown == false)
                    {

                        isDown = context.WaitEvent<SpectrumOcupation>(out outSpectrumOcupation, 1);

                        if (isDown == false) // таймут - результатов нет
                        {
                            // проверка - не отменили ли задачу
                            if (context.Token.IsCancellationRequested)
                            {
                                // явно нужна логика отмены
                                context.Cancel();
                                return;
                            }

                            if (context.WaitEvent<SamthingErrorEvent>(out outSpectrumOcupation, 1) == true)
                            {
                                /// реакция на ошибку выполнения команды
                            }
                        }
                        else
                        {
                            //реакция на принятые результаты измерения
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
                    DM.MeasResults measResult = null;
                    if (context.Task.LastTimeSend!=null)
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
                                    measResult = new DM.MeasResults();
                                    measResult.FrequencySamples = outSpectrumOcupation.fSemplesResult.Convert();
                                    measResult.ScansNumber = outSpectrumOcupation.NN;
                                    //Отправка результатов в шину 
                                    this._messagePublisher.Send<DM.MeasResults>("SendMeasResults", measResult);
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
                            this._messagePublisher.Send<DM.MeasResults>("SendMeasResults", measResult);
                        }
                        context.Finish();
                        break;
                    }
                    

                    //////////////////////////////////////////////
                    // 
                    // Приостановка потока на рассчитаное время CalculateSleepParameter(context.Task.taskParameters)
                    //
                    //////////////////////////////////////////////
                    var sleepTime = CalculateTimeSleep(context.Task.taskParameters, context.Task.taskParameters.NCount.Value);
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
        ///Вычисление задержки выполнения потока
        /// </summary>
        /// <param name="taskParameters"></param>
        /// <returns></returns>
        private int CalculateTimeSleep(TaskParameters taskParameters, int NCount)
        {
            // заглушка
            return 100000;
        }
    }
}
