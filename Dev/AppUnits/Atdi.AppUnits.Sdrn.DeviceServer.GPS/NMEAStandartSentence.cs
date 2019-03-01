
namespace Atdi.AppUnits.Sdrn.DeviceServer.GPS
{
    public sealed class NMEAStandartSentence : NMEASentence
    {
        public TalkerIdentifiers TalkerID { get; set; }
        public SentenceIdentifiers SentenceID { get; set; }        
    }
}
