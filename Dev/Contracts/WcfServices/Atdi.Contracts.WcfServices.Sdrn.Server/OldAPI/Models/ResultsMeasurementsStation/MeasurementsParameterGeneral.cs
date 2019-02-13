using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.Contracts.WcfServices.Sdrn.Server
{
    [DataContract(Namespace = Specification.Namespace)]
    [KnownType(typeof(MaskElements))]
    public class MeasurementsParameterGeneral
    {
        [DataMember]
        public double? CentralFrequency; //МГц частота согласно плану
        [DataMember]
        public double? CentralFrequencyMeas; //МГц измеренный результат
        [DataMember]
        public double? OffsetFrequency; // Относительно центральной частоты 10^-6
        [DataMember]
        public decimal? SpecrumStartFreq; // МГц  первая частота спетра
        [DataMember]
        public decimal? SpecrumSteps; // кГц  шаг у спектра
        [DataMember]
        public float[] LevelsSpecrum; //отсчеты спектра сигнала 
        [DataMember]
        public MaskElements[] MaskBW; // маска сигнала 
        [DataMember]
        public int? T1; // индекс Т1 надо отображать на спектрограмме
        [DataMember]
        public int? T2; // индекс Т2 надо отображать на спектрограмме 
        [DataMember]
        public int? MarkerIndex; // индекс для M1 надо отображать
        [DataMember]
        public double? DurationMeas; //сек, Длительность измерения частота согласно плану
        [DataMember]
        public DateTime? TimeStartMeas; // время начала измерения
        [DataMember]
        public DateTime? TimeFinishMeas; // время начала измерения
        

    }
}
