using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Icsm.CoverageEstimation.Models
{
    public class Station
    {
        public long Id { get; set; }
        public string CallSign { get; set; }
        public string Address { get; set; }
        public string TypeCoord { get; set; }
        public string Category { get; set; }
        public double Altitude { get; set; }
        public double NominalPower { get; set; }
        public double Frequency { get; set; }
        public double Bandwidth { get; set; }
        public double BandwidthRx { get; set; }
        public double HAntenna { get; set; }
        public double Azimuth { get; set; }
        public double Tilt { get; set; }
        public double Gain { get; set; }
        public double GainRx { get; set; }
        public double Losses { get; set; }
        public double LossesRx { get; set; }
        public double CoordX { get; set; }
        public double CoordY { get; set; }
        public string Info1 { get; set; }
        public int NetId { get; set; }
        public string Polar { get; set; }
        public string PolarRx { get; set; }
        public string DiagH { get; set; }
        public string DiagV { get; set; }
        public double FKTB { get; set; }
        public double D_cx1 { get; set; }
        public double U_cx1 { get; set; }

    }
}
