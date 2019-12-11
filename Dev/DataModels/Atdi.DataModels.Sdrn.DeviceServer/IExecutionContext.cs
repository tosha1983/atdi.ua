using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

        /// <summary>
        /// Получить объект для формирования результата из пула
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">ключи типа объекта результат из пула</param>
        /// <param name="index">индекс результата</param>
        /// <param name="status">состояние</param>
        /// <returns></returns>
        T TakeResult<T>(string key, ulong index, CommandResultStatus status) 
            where T : ICommandResultPart;

        void ReleaseResult<T>(T result)
            where T : ICommandResultPart;
    }

    public static class ExecutionContextExtension
    {
        public static T TakeResult<T>(this IExecutionContext context, ulong index, CommandResultStatus status)
            where T : ICommandResultPart
        {
            return context.TakeResult<T>(ResultPoolDescriptor<T>.DefaultKey, index, status);
        }
    }
}
