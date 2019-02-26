using Atdi.DataModels.Sdrn.DeviceServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.DeviceServer
{
    public interface ITaskStarter
    {
        void Run(ITask task, IProcess process, ITaskContext parentTaskContext, CancellationToken cancellationToken);

        Task RunParallel(ITask task, IProcess process, ITaskContext parentTaskContext, CancellationToken cancellationToken);

        Task RunAsync(ITask task, IProcess process, ITaskContext parentTaskContext, CancellationToken cancellationToken);
    }

    public static class TaskExecuterExtensions
    {

        public static void Run(this ITaskStarter executer, ITask task, IProcess process)
        {
            executer.Run(task, process, null, CancellationToken.None);
        }

        public static void Run(this ITaskStarter executer, ITask task, CancellationToken cancellationToken)
        {
            executer.Run(task, null, null, cancellationToken);
        }

        public static void Run(this ITaskStarter executer, ITask task, ITaskContext parentTaskContext)
        {
            executer.Run(task, null, parentTaskContext, CancellationToken.None);
        }

        public static void Run(this ITaskStarter executer, ITask task, IProcess process, ITaskContext parentTaskContext)
        {
            executer.Run(task, process, parentTaskContext, CancellationToken.None);
        }

        public static void Run(this ITaskStarter executer, ITask task, ITaskContext parentTaskContext, CancellationToken cancellationToken)
        {
            executer.Run(task, null, parentTaskContext, cancellationToken);
        }



        public static Task RunParallel(this ITaskStarter executer, ITask task, IProcess process)
        {
            return executer.RunParallel(task, process, null, CancellationToken.None);
        }

        public static Task RunParallel(this ITaskStarter executer, ITask task, CancellationToken cancellationToken)
        {
            return executer.RunParallel(task, null, null, cancellationToken);
        }

        public static Task RunParallel(this ITaskStarter executer, ITask task, IProcess process, CancellationToken cancellationToken)
        {
            return executer.RunParallel(task, process, null, cancellationToken);
        }

        public static Task RunParallel(this ITaskStarter executer, ITask task, ITaskContext parentTaskContext)
        {
            return executer.RunParallel(task, null, parentTaskContext, CancellationToken.None);
        }

        public static Task RunParallel(this ITaskStarter executer, ITask task, IProcess process, ITaskContext parentTaskContext)
        {
            return executer.RunParallel(task, process, parentTaskContext, CancellationToken.None);
        }

        public static Task RunParallelRun(this ITaskStarter executer, ITask task, ITaskContext parentTaskContext, CancellationToken cancellationToken)
        {
            return executer.RunParallel(task, null, parentTaskContext, cancellationToken);
        }
    }
}
