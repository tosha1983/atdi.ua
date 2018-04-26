using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppServer.Contracts.Sdrns;

namespace XICSM.ICSControlClient.Models.Views
{
    public class LevelMeasurementsCarViewModel
    {
        public double? Lon { get; set; }

        public double? Lat { get; set; } //DEC координата в которой происходило измерение 

        public double? Altitude { get; set; } //м координата в которой происходило измерение 

        public double? LeveldBm { get; set; } // уровень измеренного сигнала в полосе канала 

        public double? LeveldBmkVm { get; set; } // уровень измеренного сигнала в полосе канала 

        public DateTime TimeOfMeasurements { get; set; } // время когда был получен результат 

        public double? DifferenceTimestamp { get; set; } // наносекунды 10^-9 Разсинхронизация с GPS

        public decimal? CentralFrequency { get; set; } // 

        public double? BW { get; set; } // кГц;

        public double? RBW { get; set; } // кГц;

        public double? VBW { get; set; } // кГц;
    }
}
