using System;
using System.Collections.Generic;

namespace Atdi.WcfServices.Sdrn.Server
{

    public class SOFrequencyTemp
    {
        public double Frequency_MHz;
        public int hit = 0;
        public List<string> StantionIDs = new List<string>();
        public int[] HitByHuors = new int[24];
        public List<string> measglobalsid = new List<string>();
    }

}
