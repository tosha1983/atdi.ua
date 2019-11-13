using System.Threading;

namespace Atdi.Platform.Workflows
{
    public sealed class JobExecutionContext
    {
        internal JobExecutionContext(IJobToken token)
        {
            Token = token;
            this.CancellationToken = token.Breaker.Token;
        }

        public IJobToken Token { get; }

        public CancellationToken CancellationToken { get; }

        public bool IsRecovery { get; internal set; }

        public bool IsRepeat { get; internal set; }
        

    }
}