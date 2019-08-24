using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XICSM.ICSControlClient.Models
{
    public class StationsEquipment
    {
        public double Freq_MHz;
        public string Code;
        public string Manufacturer;
        public string Name;
        public string DesigEmission;
        public double MaxPower;
        public double LowerFreq;
        public double UpperFreq;
        public string Status;
        public string IcsmTable;
        public long IcsmId;
        public float[] Loss;
        public double[] Freq;
    }
}
