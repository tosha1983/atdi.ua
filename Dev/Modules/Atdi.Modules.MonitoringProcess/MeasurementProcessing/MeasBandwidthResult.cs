using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Modules.MonitoringProcess
{
    public class MeasBandwidthResult
    {
        public int? T1; // индекс Т1 надо отображать на спектрограмме
        public int? T2; // индекс Т2 надо отображать на спектрограмме 
        public int? MarkerIndex; // индекс для M1 надо отображать
        public double? BandwidthkHz; //ширина спектра в килогерцах
        public bool? СorrectnessEstimations; //коректность проведеннного измерения
    }
}
