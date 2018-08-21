using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    [DataContract(Namespace = Specification.Namespace)]
    public class GeneralMeasResult
    {
        /// <summary>
        /// МГц частота согласно плану
        /// </summary>
        [DataMember]
        public double? CentralFrequency { get; set; }

        /// <summary>
        /// МГц измеренный результат
        /// </summary>
        [DataMember]
        public double? CentralFrequencyMeas { get; set; }

        /// <summary>
        /// Относительно центральной частоты 10^-6
        /// </summary>
        [DataMember]
        public double? OffsetFrequency { get; set; }

        /// <summary>
        /// МГц  первая частота спетра
        /// </summary>
        [DataMember]
        public decimal? SpectrumStartFreq { get; set; }

        /// <summary>
        /// кГц  шаг у спектра
        /// </summary>
        [DataMember]
        public decimal? SpectrumSteps { get; set; }

        /// <summary>
        /// отсчеты спектра сигнала 
        /// </summary>
        [DataMember]
        public float[] LevelsSpectrum { get; set; }

        /// <summary>
        /// маска сигнала 
        /// </summary>
        [DataMember]
        public ElementsMask[] BWMask { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public BandwidthMeasResult BandwidthResult { get; set; }

        /// <summary>
        /// сек, Длительность измерения частота согласно плану
        /// </summary>
        [DataMember]
        public double? MeasDuration { get; set; }

        /// <summary>
        /// время начала измерения
        /// </summary>
        [DataMember]
        public DateTime? MeasStartTime { get; set; }

        /// <summary>
        /// время окончанияч измерения
        /// </summary>
        [DataMember]
        public DateTime? MeasFinishTime { get; set; }
    }
}
