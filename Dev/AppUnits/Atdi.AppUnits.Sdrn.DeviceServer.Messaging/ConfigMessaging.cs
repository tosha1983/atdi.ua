using System;
using Atdi.Platform.AppComponent;
using System.Collections.Generic;



namespace Atdi.AppUnits.Sdrn.DeviceServer.Messaging
{
    public class ConfigMessaging
    {
        /// <summary>
        /// 
        /// </summary>
        public bool CompareTraceJustWithRefLevels { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool AutoDivisionEmitting { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool FiltrationTrace { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [ComponentConfigProperty("DifferenceMaxMax.double")]
        public double DifferenceMaxMax { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [ComponentConfigProperty("allowableExcess_dB.double")]
        public double allowableExcess_dB { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int SignalizationNCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int SignalizationNChenal { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [ComponentConfigProperty("PercentForCalcNoise.double")]
        public double PercentForCalcNoise { get; set; }
    }
}
