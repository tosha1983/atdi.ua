using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Содержит результаты измерений передатчика.
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class GeneralMeasResult
    {
        /// <summary>
        /// кГц;
        /// </summary>
        [DataMember]
        public double? RBW_kHz { get; set; }

        /// <summary>
        /// кГц;
        /// </summary>
        [DataMember]
        public double? VBW_kHz { get; set; }

        /// <summary>
        /// МГц частота согласно плану
        /// </summary>
        [DataMember]
        public double? CentralFrequency_MHz { get; set; }

        /// <summary>
        /// МГц измеренный результат
        /// </summary>
        [DataMember]
        public double? CentralFrequencyMeas_MHz { get; set; }

        /// <summary>
        /// Относительно центральной частоты 10^-6
        /// </summary>
        [DataMember]
        public double? OffsetFrequency_mk { get; set; }

        /// <summary>
        /// МГц  первая частота спетра
        /// </summary>
        [DataMember]
        public decimal? SpectrumStartFreq_MHz { get; set; }

        /// <summary>
        /// кГц  шаг у спектра
        /// </summary>
        [DataMember]
        public decimal? SpectrumSteps_kHz { get; set; }

        /// <summary>
        /// отсчеты спектра сигнала 
        /// </summary>
        [DataMember]
        public float[] LevelsSpectrum_dBm { get; set; }

        /// <summary>
        /// маска сигнала 
        /// </summary>
        [DataMember]
        public ElementsMask[] BWMask { get; set; }

        /// <summary>
        /// Результаты измерения полосы частот сигнала 
        /// </summary>
        [DataMember]
        public BandwidthMeasResult BandwidthResult { get; set; }

        /// <summary>
        /// сек, Длительность измерения частота согласно плану
        /// </summary>
        [DataMember]
        public double? MeasDuration_sec { get; set; }

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

        /// <summary>
        /// Системная информация станции
        /// </summary>
        [DataMember]
        public StationSysInfo StationSysInfo { get; set; }


    }
}
