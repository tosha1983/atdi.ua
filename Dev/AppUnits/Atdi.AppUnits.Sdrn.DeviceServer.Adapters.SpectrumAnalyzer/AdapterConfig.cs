﻿using Atdi.Platform.AppComponent;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.SpectrumAnalyzer
{
    public class AdapterConfig
    {
        private readonly static string IPAddressName = "IPAddress";
        private readonly static string DisplayUpdateName = "DisplayUpdate";
        private readonly static string Prop3Name = "Prop3";
        private readonly static string Prop4Name = "Prop4";
        private readonly static string Prop5Name = "Prop5";

        /// <summary>
        /// Для тестирования нужен коснтруктор по умолчанию
        /// </summary>
        public AdapterConfig()
        {
        }

        public AdapterConfig(IComponentConfig config, ILogger logger)
        {
            try
            {
                this.IPAddress = config.GetParameterAsString(AdapterConfig.IPAddressName);
                this.DisplayUpdate = config.GetParameterAsBoolean(AdapterConfig.DisplayUpdateName);
                this.Prop3 = config.GetParameterAsInteger(AdapterConfig.Prop3Name);
                this.Prop4 = config.GetParameterAsInteger(AdapterConfig.Prop4Name);
                this.Prop5 = config.GetParameterAsInteger(AdapterConfig.Prop5Name);
            }
            catch (Exception e)
            {
                logger.Exception(Contexts.ThisComponent, Categories.ConfigLoading, e);
                throw new InvalidOperationException(Exceptions.ConfigWasNotLoaded, e);
            }
        }

        public string IPAddress { get; set;}

        public bool DisplayUpdate { get; set; }

        public int? Prop3 { get; set; }

        public int? Prop4 { get; set; }

        public int? Prop5 { get; set; }

    }
}
