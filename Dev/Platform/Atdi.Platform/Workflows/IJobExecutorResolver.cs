namespace Atdi.Platform.Workflows
{
    public interface IJobExecutorResolver
    {
        IJobExecutor Resolve<TExecutor>()
            where TExecutor : IJobExecutor;

        IJobExecutor<TState> Resolve<TExecutor, TState>()
            where TExecutor : IJobExecutor<TState>;
    }
}