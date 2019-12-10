using System.Threading;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeviceServer;

namespace Atdi.Contracts.Sdrn.DeviceServer
{
    public static class TaskStarterExtensions
    {

        public static void Run(this ITaskStarter starter, ITask task, IProcess process)
        {
            starter.Run(task, process, null, CancellationToken.None);
        }

        public static void Run(this ITaskStarter starter, ITask task, CancellationToken cancellationToken)
        {
            starter.Run(task, null, null, cancellationToken);
        }

        public static void Run(this ITaskStarter starter, ITask task, ITaskContext parentTaskContext)
        {
            starter.Run(task, null, parentTaskContext, CancellationToken.None);
        }

        public static void Run(this ITaskStarter starter, ITask task, IProcess process, ITaskContext parentTaskContext)
        {
            starter.Run(task, process, parentTaskContext, CancellationToken.None);
        }

        public static void Run(this ITaskStarter starter, ITask task, ITaskContext parentTaskContext, CancellationToken cancellationToken)
        {
            starter.Run(task, null, parentTaskContext, cancellationToken);
        }



        public static Task RunParallel(this ITaskStarter starter, ITask task, IProcess process)
        {
            return starter.RunParallel(task, process, null, CancellationToken.None);
        }

        public static Task RunParallel(this ITaskStarter starter, ITask task, CancellationToken cancellationToken)
        {
            return starter.RunParallel(task, null, null, cancellationToken);
        }

        public static Task RunParallel(this ITaskStarter starter, ITask task, IProcess process, CancellationToken cancellationToken)
        {
            return starter.RunParallel(task, process, null, cancellationToken);
        }

        public static Task RunParallel(this ITaskStarter starter, ITask task, ITaskContext parentTaskContext)
        {
            return starter.RunParallel(task, null, parentTaskContext, CancellationToken.None);
        }

        public static Task RunParallel(this ITaskStarter starter, ITask task, IProcess process, ITaskContext parentTaskContext)
        {
            return starter.RunParallel(task, process, parentTaskContext, CancellationToken.None);
        }

        public static Task RunParallel(this ITaskStarter starter, ITask task, ITaskContext parentTaskContext, CancellationToken cancellationToken)
        {
            return starter.RunParallel(task, null, parentTaskContext, cancellationToken);
        }
    }
}