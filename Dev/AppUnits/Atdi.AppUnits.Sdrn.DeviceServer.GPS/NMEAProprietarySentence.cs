
namespace Atdi.AppUnits.Sdrn.DeviceServer.GPS
{
    public sealed class NMEAProprietarySentence : NMEASentence
    {
        public string SentenceIDString { get; set; }
        public ManufacturerCodes Manufacturer { get; set; }
    }
}
