namespace Atdi.Platform.Workflows
{
    public interface IJobExecutor
    {
        JobExecutionResult Execute(JobExecutionContext context);
    }

    public interface IJobExecutor<in TState>
    {
        JobExecutionResult Execute(JobExecutionContext context, TState state);
    }
}