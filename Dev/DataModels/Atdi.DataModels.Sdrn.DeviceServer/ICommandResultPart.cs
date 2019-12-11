using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer
{
    public interface ICommandResultPart
    {
        ulong PartIndex { get; set; }

        CommandResultStatus Status { get; set; }

        Guid PoolId { get; set; }

        string PoolKey { get; set; }
    }

    [Serializable]
    public class CommandResultPartBase : ICommandResultPart
    {
        public CommandResultPartBase()
        {
        }

        public CommandResultPartBase(ulong partIndex, CommandResultStatus status)
        {
            this.PartIndex = partIndex;
            this.Status = status;
        }

        public ulong PartIndex { get; set; }

        public CommandResultStatus Status { get; set; }

        public Guid PoolId { get; set; }

        public string PoolKey { get; set; }
    }
}