using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.Sdrns.Server;


namespace Atdi.DataModels.Sdrns.Server
{
    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
    public class SignalingMeasTaskParameters
    {
        [DataMember]
        public double? allowableExcess_dB;// при формировании реферативного спектра допустимое превышение над шумом
        [DataMember]
        public bool? CompareTraceJustWithRefLevels;
        [DataMember]
        public bool? FiltrationTrace;

        [DataMember]
        public int? SignalizationNCount; // максимальное количество измерений сигнализации в день. 
        [DataMember]
        public int? SignalizationNChenal; //Количество точек в канале при сканировании.
        [DataMember]
        public bool? CorrelationAnalize;// проводить корреляционный анализ между излучениями по умолчанию лож
        [DataMember]
        public double? CorrelationFactor;// коєфициент кореляции при котором обединяем излучения по умолчанию 0.7
        [DataMember]
        public bool? CheckFreqChannel;// проверять совподение частоты сигнала с частотой канала по умолчанию лож
        [DataMember]
        public bool? AnalyzeByChannel; // true значит надо анализировать согласно существующим частото каналам  по умолчанию false
        [DataMember]
        public bool? AnalyzeSysInfoEmission; // true значит надо выполнять вложенное вокфлоу по SysInfo по умолчанию false 
        [DataMember]
        public bool? DetailedMeasurementsBWEmission; // true значит надо выполнять вложенное вокфлоу по BW по умолчанию false 
        [DataMember]
        public string Standard; // ожидаемій стандарт сигналов (GSM or LTE or UMTS) по умолчанию null
        [DataMember]
        public double? triggerLevel_dBm_Hz; // уровень шума на входе приемника по умолчанию измерение автошума = -999 используется в  namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements строка 61 (в хендлере)
        [DataMember]
        public SignalingInterruptionParameters InterruptionParameters;
        [DataMember]
        public SignalingGroupingParameters GroupingParameters;
        [DataMember]
        public bool? CollectEmissionInstrumentalEstimation { get; set; } // true значит, что  после отправления результатов матрицы, которые содержат результаты измерений (EmittingSum, EmittingTemp) должны обнуляться,  (по умолчанию false)
    }
}
