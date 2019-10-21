using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using System.Text;
using System.IO;
using Atdi.Platform.Logging;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.LegacyServices.Icsm;
using Atdi.AppUnits.Icsm.CoverageEstimation.Models;
using Atdi.AppUnits.Icsm.Hooks;


namespace Atdi.AppUnits.Icsm.CoverageEstimation.Handlers
{
    public static class CoverageConfig 
    {
        public static DataConfig Load(AppServerComponentConfig appServerComponentConfig)
        {
            var config = new Config();
            return config.Load(appServerComponentConfig.CoverageConfigFileName);
        }
       
    }
}
