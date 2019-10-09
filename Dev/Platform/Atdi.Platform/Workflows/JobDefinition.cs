using System;

namespace Atdi.Platform.Workflows
{
    public abstract class JobDefinition
    {
        public string Name { get; set; }

        public bool Recoverable { get; set; }

        public bool Repeatable { get; set; }

        public TimeSpan? StartDelay { get; set; }

        public TimeSpan? RepeatDelay { get; set; }

        public override string ToString()
        {
            return $"Job='{this.Name}'";
        }
    }

    public sealed class JobDefinition<TExecutor> : JobDefinition
        where TExecutor : IJobExecutor
    {
        public override string ToString()
        {
            return $"{base.ToString()}, Executor='{typeof(TExecutor).FullName}'";
        }
    }

    public sealed class JobDefinition<TExecutor, TState> : JobDefinition
        where TExecutor : IJobExecutor<TState>
    {
        public override string ToString()
        {
            return $"{base.ToString()}, Executor='{typeof(TExecutor).FullName}', State='{typeof(TState).FullName}'";
        }
    }
}