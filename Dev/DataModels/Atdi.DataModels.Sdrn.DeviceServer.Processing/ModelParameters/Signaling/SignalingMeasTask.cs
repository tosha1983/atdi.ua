using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.DataModels.Sdrn.DeviceServer.Processing
{
    [Serializable]
    public class SignalingMeasTask
    {
        public bool? CompareTraceJustWithRefLevels { get; set; }
        public bool? FiltrationTrace { get; set; }
        public double? allowableExcess_dB { get; set; }// при формировании реферативного спектра допустимое превышение над шумом
        public int? SignalizationNCount { get; set; } // максимальное количество измерений сигнализации в день. 
        public int? SignalizationNChenal { get; set; } //Количество точек в канале при сканировании.
        public bool? CorrelationAnalize { get; set; }// проводить корреляционный анализ между излучениями по умолчанию лож
        public double? CorrelationFactor { get; set; }// коєфициент кореляции при котором обединяем излучения по умолчанию 0.7
        public bool? CheckFreqChannel { get; set; }// проверять совподение частоты сигнала с частотой канала по умолчанию лож
        public bool? AnalyzeByChannel { get; set; } // true значит надо анализировать согласно существующим частото каналам  по умолчанию false
        public bool? AnalyzeSysInfoEmission { get; set; } // true значит надо выполнять вложенное вокфлоу по SysInfo по умолчанию false 
        public bool? DetailedMeasurementsBWEmission { get; set; } // true значит надо выполнять вложенное вокфлоу по BW по умолчанию false 
        public string Standard { get; set; } // ожидаемій стандарт сигналов (GSM or LTE or UMTS) по умолчанию null
        public double? triggerLevel_dBm_Hz { get; set; } // уровень шума на входе приемника по умолчанию измерение автошума = -999 используется в  namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements строка 61 (в хендлере)
        public SignalingInterruptionParameters InterruptionParameters { get; set; }
        public SignalingGroupingParameters GroupingParameters { get; set; }
        public bool? CorrelationAdaptation { get; set; } // true означает что можно адаптировать коэфициент корреляции. Устанавливается когда первоначально коэфициент корреляции 0.99 и выше.
        public int? MaxNumberEmitingOnFreq { get; set; } // брать из файла конфигурации.
        public double? MinCoeffCorrelation { get; set; } // брать из файла конфигурации.
        public bool? UkraineNationalMonitoring { get; set; } // признак что делается все для Украины
    }
}
