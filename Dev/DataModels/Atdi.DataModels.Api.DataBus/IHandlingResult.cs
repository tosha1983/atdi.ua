using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Api.DataBus
{
    public enum MessageHandlingStatus
    {
        /// <summary>
        /// начальное состояние поступившего сообщения но еще неразу никем не обработаным
        /// </summary>
        Unprocessed = 0,

        /// <summary>
        /// Подтвердить обработку сообщения
        /// </summary>
        Confirmed,

        /// <summary>
        /// Проигнорировать сообщение, оно будет передано другому обработчику в другом диспетчере, сообщение остается в очереди
        /// </summary>
        Ignored,

        /// <summary>
        ///  Отказ от сообщения - удаляется из очереди адресата, копируется в специальную локальную очередь для дальнейшего разбирательства
        /// </summary>
        Rejected
    }

    public interface IHandlingResult
    {
        MessageHandlingStatus Status { get; set; }

        string Reason { get; set; }

        string Detail { get; set; }
    }
}
