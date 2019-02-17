using System;
using System.Collections.Generic;
using System.Linq;


namespace Atdi.Contracts.WcfServices.Sdrn.Server
{
    public class StatusDescription
    {
        public string NameStatus { get; set; }
        public int Weight { get; set; }
        public ModeStatus Type { get; set; }
        public StatusDescription(string NameStatus_, int Weight_, ModeStatus type_)
        {
            NameStatus = NameStatus_;
            Weight = Weight_;
            Type = type_;
        }
    }
}
