using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer
{
    public interface ITaskContext
    {
        ITaskDescriptor Descriptor { get; }

        CancellationToken Token { get; }

        /// <summary>
        /// Подтвердить завершение выполнения таска
        /// </summary>
        void Finish();

        /// <summary>
        /// Оборвать выполнение таска
        /// </summary>
        void Abort(Exception e);

        /// <summary>
        /// Подтвердить отмену таска
        /// </summary>
        void Cancel();


        TEvent WaitEvent<TEvent>();

        void SetEvent<TEvent>(TEvent @event);
    }

    public interface ITaskContext<TTask, TProcess> : ITaskContext
        where TTask : ITask
        where TProcess: IProcess
    {
        TTask Task { get; }

        TProcess Process { get; }
    }
}
