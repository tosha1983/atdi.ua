using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer
{
    public interface IExecutionContext
    {
        CancellationToken Token { get; }

        void PushResult(ICommandResultPart result);

        /// <summary>
        /// Подтвердить завершение выполнения комманды
        /// </summary>
        void Finish();

        /// <summary>
        /// Оборвать выполнение комманды
        /// </summary>
        void Abort(Exception e);

        /// <summary>
        /// Подтвердить отмену комманды
        /// </summary>
        void Cancel();

        /// <summary>
        /// Заблокировать определенный набор комманд на время выполнения комманды
        /// </summary>
        /// <param name="types"></param>
        void Lock(params CommandType[] types);
        void Lock(params Type[] commandTypes);
        void Lock();

        /// <summary>
        /// Снять блокировку от выполнения определенных комманд
        /// </summary>
        /// <param name="types"></param>
        void Unlock(params CommandType[] types);
        void Unlock(params Type[] commandTypes);
        void Unlock();
    }
}
