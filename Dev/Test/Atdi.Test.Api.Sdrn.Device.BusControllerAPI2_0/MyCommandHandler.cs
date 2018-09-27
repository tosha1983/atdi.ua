using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Api.Sdrn.MessageBus;
using DM = Atdi.DataModels.Sdrns.Device;

namespace Atdi.Test.Api.Sdrn.Device.BusControllerAPI2_0
{
    class MyTaskHandler : MessageHandlerBase<DM.MeasTask>
    {
        public MyTaskHandler() 
            : base("SendMeasTask")
        {
        }

        public override void OnHandle(IReceivedMessage<DM.MeasTask> message)
        {
            // тут код обработки сообщения

            try
            {
                //
                if (message.Data.Stations.Length == 0)
                {
                    throw new ArgumentException("Undefined stations");
                }

                // и т.п. код

                // сохранение таска
            }
            catch(Exception e)
            {
                // допустим невалидные таскаи мы не принимаем и таки есообщение отправляем в очередь ошибочных
                message.Result = MessageHandlingResult.Error;
                message.ReasonFailure = e.Message;
            }
        }
    }
}
