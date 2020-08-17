﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations
{
	public class ResultRefSpectrumByDriveTests 
    {
        public long OrderId { get; set; }
        public string TableIcsmName { get; set; }
        public long IdIcsm { get; set; }
        public long IdSensor { get; set; }
        public string GlobalCID { get; set; }
        public double Freq_MHz { get; set; }
        public double Level_dBm { get; set; }
        public double Percent { get; set; }
        public DateTimeOffset DateMeas { get; set; }
    }
}