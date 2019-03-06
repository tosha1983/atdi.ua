
namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.GPS
{
    public sealed class NMEAProprietarySentese : NMEASentense
    {
        public string SenteseIDString { get; set; }
        public ManufacturerCodes Manufacturer { get; set; }
    }
}
