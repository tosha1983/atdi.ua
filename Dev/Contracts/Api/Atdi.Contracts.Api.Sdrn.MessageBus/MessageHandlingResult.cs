using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Api.Sdrn.MessageBus
{
    public enum MessageHandlingResult
    {
        /// <summary>
        /// начальное состояние поступившего сообщения но еще неразу никем не обработаным
        /// </summary>
        Received,
        /// <summary>
        /// Подтвердить обработку сообщения
        /// </summary>
        Confirmed, 

        /// <summary>
        /// Проигнорировать сообщение, оно будет передано другому обработчику в другом диспетчере
        /// </summary>
        Ignore,

        /// <summary>
        ///  Отказ от сообщения - просто удаляется
        /// </summary>
        Reject,

        /// <summary>
        ///  сообщение переместить вмусорную корзину - в специальную очередь для дальнейшего разбирательства
        /// </summary>
        Trash,

        /// <summary>
        ///  сообщение переместить в очередь с ошибками - в специальную очередь для дальнейшего разбирательства
        /// </summary>
        Error
    }
}
