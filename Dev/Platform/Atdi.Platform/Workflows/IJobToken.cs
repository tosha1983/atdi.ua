using System;
using System.Threading;

namespace Atdi.Platform.Workflows
{
    public interface IJobToken
    {
        Guid Instance { get; }

        JobExecutionStatus Status{ get; }

        CancellationTokenSource Breaker { get; }

        long StartAttempts { get; }

        long RecoveryAttempts { get; }

        JobDefinition Definition { get; }

        void Abort();

    }
}