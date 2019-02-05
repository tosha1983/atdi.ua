namespace Atdi.DataModels.Sdrn.DeviceServer
{
    public interface ICommandResultPart
    {
        ulong PartIndex { get; }

        CommandResultStatus Status { get; }
    }

    public class CommandResultPartBase : ICommandResultPart
    {
        public CommandResultPartBase(ulong partIndex, CommandResultStatus status)
        {
            this.PartIndex = partIndex;
            this.Status = status;
        }

        public ulong PartIndex { get; private set; }

        public CommandResultStatus Status { get; private set; }
    }
}