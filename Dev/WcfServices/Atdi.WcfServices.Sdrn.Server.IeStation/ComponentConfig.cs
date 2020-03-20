using Atdi.Platform.AppComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.WcfServices.Sdrn.Server.IeStation
{
    public class ComponentConfig
    {
        /// <summary>
        ///  время для группировки записей workTime по умолчанию 60
        /// </summary>
        public int? TimeBetweenWorkTimes_sec { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? CorrelationAdaptation { get; set; } // true означает что можно адаптировать коэфициент корреляции. Устанавливается когда первоначально коэфициент корреляции 0.99 и выше.
        /// <summary>
        /// 
        /// </summary>
        public int? MaxNumberEmitingOnFreq { get; set; } // брать из файла конфигурации.
        /// <summary>
        /// 
        /// </summary>
        [ComponentConfigProperty("MinCoeffCorrelation.double")]
        public double? MinCoeffCorrelation { get; set; } // брать из файла конфигурации.
        /// <summary>
        /// 
        /// </summary>
        public bool? UkraineNationalMonitoring { get; set; } // признак что делается все для Украины

        /// <summary>
        /// 
        /// </summary>
        public int? CountMaxEmission { get; set; }

    }
}
