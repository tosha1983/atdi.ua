using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.Server
{
    public enum SdrnMessageHandlingStatus
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
        ///  Отказ от сообщения - просто удаляется
        /// </summary>
        Rejected,

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
