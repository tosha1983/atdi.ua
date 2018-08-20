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
    [KnownType(typeof(LocationSensorMeasurement))]
    [KnownType(typeof(FSemples))]
    [KnownType(typeof(ResultsMeasurementsStation))]
    [KnownType(typeof(LevelMeasurementsCar))]
    [KnownType(typeof(MeasurementsParameterGeneral))]
    public class MeasSdrResults
    {
        [DataMember]
        public int Id; // счетчик результатов которые отправляются для данного MeasSDRTask Начинается с 1.
        [DataMember]
        public MeasTaskIdentifier MeasTaskId; // ссылка на идентификатор таска
        [DataMember]
        public MeasTaskIdentifier MeasSubTaskId; // ссылка на MEAS_SUB_TASK
        [DataMember]
        public int MeasSubTaskStationId; // ссылка на MEAS_SUB_TASK_ST
        [DataMember]
        public SensorIdentifier SensorId; // Идентификатор сенсора
        [DataMember]
        public DateTime DataMeas; // конкретное время окончания измерения  // для мобильных измерительных комплексов это время когда отправляется результат.
        [DataMember]
        public string status; // статус обекта
        [DataMember]
        public int NN; // Используется для SO и храниться для запоминания количества измерений
        [DataMember]
        public int SwNumber; // Number of scans at a time. 
        [DataMember]
        public LocationSensorMeasurement MeasSDRLoc;
        [DataMember]
        public MeasurementType MeasDataType; // тип измерения 
        [DataMember]
        public FSemples[] FSemples;
        [DataMember]
        public float[] Freqs;
        [DataMember]
        public float[] Level;
        [DataMember]
        public ResultsMeasurementsStation[] ResultsMeasStation;
        [DataMember]
        public MeasSdrBandwidthResults ResultsBandwidth;
    }
}
