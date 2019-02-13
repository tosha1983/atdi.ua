using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Adapters
{
    public class InstrManufacrures
    {
        public InstrManufacrures() { }
        public ParamWithUI Unk = new ParamWithUI() { Parameter = "0", UI = "Unknown", UIShort = "Unk" };
        public ParamWithUI RuS = new ParamWithUI() { Parameter = "1", UI = "Rohde&Schwarz", UIShort = "R&S" };
        public ParamWithUI Keysight = new ParamWithUI() { Parameter = "2", UI = "Keysight Technologies", UIShort = "Keysight" };
        public ParamWithUI Anritsu = new ParamWithUI() { Parameter = "3", UI = "Anritsu", UIShort = "Anritsu" };
        public ParamWithUI SignalHound = new ParamWithUI() { Parameter = "4", UI = "SignalHound", UIShort = "SH" };
    }
}
