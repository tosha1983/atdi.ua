﻿
namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.GPS
{
    public sealed class NMEAStandartSentese : NMEASentense
    {
        public TalkerIdentifiers TalkerID { get; set; }
        public SentenceIdentifiers SentenseID { get; set; }        
    }
}