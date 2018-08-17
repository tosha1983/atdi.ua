using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns
{
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    [KnownType(typeof(MeasTaskIdentifier))]
    [KnownType(typeof(SensorIdentifier))]
    [KnownType(typeof(MeasFreqParam))]
    [KnownType(typeof(MeasSdrParam))]
    [KnownType(typeof(MeasSdrSOParam))]
    [KnownType(typeof(MeasLocParam))]
    [KnownType(typeof(StationDataForMeasurements))]
    [KnownType(typeof(MeasurementType))]
    [KnownType(typeof(SpectrumScanType))]
    public class MeasSdrTask
    {
        [DataMember]
        public int Id;
        [DataMember]
        public MeasTaskIdentifier MeasTaskId; // ссылка на задачу
        [DataMember]
        public MeasTaskIdentifier MeasSubTaskId; // ссылка на под задачу
        [DataMember]
        public int MeasSubTaskStationId; // ссылка на подзадачу для данной станции 
        [DataMember]
        public SensorIdentifier SensorId;
        [DataMember]
        public MeasurementType MeasDataType; // SO - spectrum occupation; LV - Level; FO - Offset; FR - Frequency; FM - Freq. Modulation; AM - Ampl. Modulation; BW	- Bandwidth Meas; BE - Bearing; SA - Sub Audio Tone; PR	- Program; PI - PI Code  (Hex Code identifying radio program); SI - Sound ID; LO	- Location;
        [DataMember]
        public SpectrumScanType TypeM; // Type of spectrums scan RT -  Real Time; SW - sweep
        [DataMember]
        public int SwNumber; // Number of scans at a time. 
        [DataMember]
        public DateTime Time_start; // Дата начала сканирования 
        [DataMember]
        public double PerInterval; // длительность измерений 
        [DataMember]
        public DateTime Time_stop; // Дата конца сканирования
        [DataMember]
        public int prio;
        [DataMember]
        public string status;
        [DataMember]
        public MeasFreqParam MeasFreqParam;
        [DataMember]
        public MeasSdrParam MeasSDRParam;
        [DataMember]
        public MeasSdrSOParam MeasSDRSOParam;
        [DataMember]
        public MeasLocParam[] MeasLocParam;
        [DataMember]
        public int NumberScanPerTask; // параметр который определяет какое количество сканирований надо еще произвести 
        [DataMember]
        public MeasurementType[] GroupeTypeMeasForMobEquipment;
        [DataMember]
        public StationDataForMeasurements[] StationsForMeasurements;// список станций для проведения измерения
        //[DataMember]
        //public BandwidthEstimation//
    }
}
