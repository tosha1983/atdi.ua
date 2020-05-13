using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.DataModels.Sdrns.Device
{
    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
    public class SignalingMeasTask
    {
        [DataMember]
        public bool? CompareTraceJustWithRefLevels { get; set; }
        [DataMember]
        public bool? FiltrationTrace { get; set; }
        [DataMember]
        public double? allowableExcess_dB { get; set; }// при формировании реферативного спектра допустимое превышение над шумом
        [DataMember]
        public int? SignalizationNCount { get; set; } // максимальное количество измерений сигнализации в день. 
        [DataMember]
        public int? SignalizationNChenal { get; set; } //Количество точек в канале при сканировании.
        [DataMember]
        public bool? CorrelationAnalize { get; set; }// проводить корреляционный анализ между излучениями по умолчанию лож
        [DataMember]
        public double? CorrelationFactor { get; set; }// коєфициент кореляции при котором обединяем излучения по умолчанию 0.7
        [DataMember]
        public bool? CheckFreqChannel { get; set; }// проверять совподение частоты сигнала с частотой канала по умолчанию лож
        [DataMember]
        public bool? AnalyzeByChannel { get; set; } // true значит надо анализировать согласно существующим частото каналам  по умолчанию false
        [DataMember]
        public bool? AnalyzeSysInfoEmission { get; set; } // true значит надо выполнять вложенное вокфлоу по SysInfo по умолчанию false 
        [DataMember]
        public bool? DetailedMeasurementsBWEmission { get; set; } // true значит надо выполнять вложенное вокфлоу по BW по умолчанию false 
        [DataMember]
        public string Standard { get; set; } // ожидаемій стандарт сигналов (GSM or LTE or UMTS) по умолчанию null
        [DataMember]
        public double? triggerLevel_dBm_Hz { get; set; } // уровень шума на входе приемника по умолчанию измерение автошума = -999 используется в  namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements строка 61 (в хендлере)
        [DataMember]
        public SignalingInterruptionParameters InterruptionParameters { get; set; }
        [DataMember]
        public SignalingGroupingParameters GroupingParameters { get; set; }
        [DataMember]
        public bool? CollectEmissionInstrumentalEstimation { get; set; } // true значит, что  после отправления результатов матрицы, которые содержат результаты измерений (EmittingSum, EmittingTemp) должны обнуляться,  (по умолчанию false)
        [DataMember]
        public bool? IsUseRefSpectrum { get; set; }   // Пользователь на форме выбирает данный флаг, и в зависимости от того выбран он или нет,  перед созданием таска показывают или не показывают доп форму для загрузки реф ситуации

    }
}
