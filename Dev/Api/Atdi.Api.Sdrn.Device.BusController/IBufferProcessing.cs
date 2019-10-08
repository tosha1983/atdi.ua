using Atdi.Modules.Sdrn.DeviceBus;

namespace Atdi.Api.Sdrn.Device.BusController
{
    internal interface IBufferProcessing
    {
        void Start();

        void Stop();

        void Save(BusMessage message);
    }
}
