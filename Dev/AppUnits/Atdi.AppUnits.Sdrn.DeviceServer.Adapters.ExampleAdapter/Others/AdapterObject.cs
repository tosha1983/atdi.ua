//using Atdi.Contracts.Sdrn.DeviceServer;
//using Atdi.DataModels.Sdrn.DeviceServer;
//using Atdi.DataModels.Sdrn.DeviceServer.Adapters;
//using Atdi.DataModels.Sdrn.DeviceServer.Commands;
//using Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters;
//using Atdi.Platform.Logging;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.ExampleAdapter
//{
//    /// <summary>
//    /// Пример реализации объект аадаптера
//    /// </summary>
//    public class AdapterObject : IAdapter
//    {
//        private readonly ILogger _logger;
//        private readonly AdapterConfig _adapterConfig;

//        /// <summary>
//        /// Все объекты адаптера создаются через DI-контейнер 
//        /// Запрашиваем через конструктор необходимые сервисы
//        /// </summary>
//        /// <param name="adapterConfig"></param>
//        /// <param name="logger"></param>
//        public AdapterObject(AdapterConfig adapterConfig, ILogger logger)
//        {
//            this._logger = logger;
//            this._adapterConfig = adapterConfig;
//        }

//        /// <summary>
//        /// Метод будет вызван при инициализации потока воркера адаптера
//        /// Адаптеру необходимо зарегестрировать свои обработчики комманд 
//        /// </summary>
//        /// <param name="host"></param>
//        public void Connect(IAdapterHost host)
//        {
//            /// включем устройство
//            /// иницируем его параметрами сконфигурации
//            /// проверяем к чем оно готово

//            /// сообщаем инфраструктуре что мы готовы обрабатывать комманду MesureGpsLocationExampleCommand
//            /// и при этом возвращать оезультат в типе MesureGpsLocationExampleAdapterResult
//            host.RegisterHandler<MesureGpsLocationExampleCommand, MesureGpsLocationExampleAdapterResult>(MesureGpsLocationExampleCommandHandler);
//        }

//        /// <summary>
//        /// Метод вызывается контрллером когда необходимо выгрузит адаптер с памяти
//        /// </summary>
//        public void Disconnect()
//        {
//            /// освобождаем ресурсы и отключаем устройство
//        }

//        /// <summary>
//        ///  типизированный обрабочик конкретной комманды MesureGpsLocation
//        /// </summary>
//        /// <param name="command"></param>
//        /// <param name="context"></param>
//        public void MesureGpsLocationExampleCommandHandler(MesureGpsLocationExampleCommand command, IExecutionContext context)
//        {
//            /// примерный сценарий обрработки комманды адаптером
//            /// этот сценарий работает в потоке адаптера
//            try
//            {
               
//                /// если нужно проводим блокировку комманд который мы не сможем обслужить пока что то меряем в течении выполнени яэтой комманды
//                /// и подсказывая этим инфраструктуре что этот устрйостов некоторое время не сможет обрабатываить такие комманды
//                context.Lock(CommandType.MesureGpsLocation, CommandType.MesureIQStream);

//                // если нужно заблокировать выполняему комманду то достатчоно вызвать метод без параметров и блокируется комманда которая выполняется
//                context.Lock();

//                // важно: если измерение идет в поток еадаптера то в принцпе в явных блокировках смысла нет - адаптер полностью занят и другие кломаныд обработаить не сможет
//                // функции блокировки имеют смысл если мы для измерений создает отдельные потоки а этот освобождаем для прослушивани яследующих комманд

//                // сценарйи в данном случаи за разарбочиком адаптера

//                // что то меряем

//                // пушаем результат
//                var result = new MesureGpsLocationExampleAdapterResult(0, CommandResultStatus.Final);
//                context.PushResult(result);

//                // иногда нужно проверять токен окончания работы комманды
//                if (context.Token.IsCancellationRequested)
//                {
//                    // все нужно остановиться

//                    // если есть порция данных возвращаем ее в обработчки только говрим что поток результатов не законченный и больше уже не будет поступать
//                    var result2 = new MesureGpsLocationExampleAdapterResult(0, CommandResultStatus.Ragged);
//                    context.PushResult(result2);

//                    // подтверждаем факт обработки отмены
//                    context.Cancel();
//                    // освобождаем поток 
//                    return;
//                }

//                // снимаем блокировку с текущей команды
//                context.Unlock();

//                // что то делаем еще 


//                // подтверждаем окончание выполнения комманды 
//                // важно: всн ранее устапнволеные в контексте обработки текущей команыд блокировки снимаются автоматически
//                context.Finish();
//                // дальше кода быть не должно, освобождаем поток
//            }
//            catch (Exception e)
//            {
//                // желательно записать влог
//                _logger.Exception(Contexts.ThisComponent, e);
//                // этот вызов обязательный в случаи обрыва
//                context.Abort(e);
//                // дальше кода быть не должно, освобождаем поток
//            }

//        }
//    }
//}
