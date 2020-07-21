using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc
{
	internal static class Contexts
	{
		//public static readonly EventContext DataAdapter = "DataAdapter";
	}

	internal static class Categories
	{
		//public static readonly EventCategory Refresh = "Refresh";
	}

    internal static class Exceptions
    {
        public static readonly string StationCalibrationCalculation = "StationCalibrationCalculation";
    }

    public static class PluginMetadata
    {
        public static readonly string Title = "ICS Station Calibration Calculation";
        public static readonly string Ident = "SdrnStationCalibrationCalc";
        public static readonly double SchemaVersion = 20200721.1600;

       

        public static class Menu
        {
            public static readonly string MainTool = "ICS Station Calibration";

            public static class Tools
            {
                public static readonly string RunProjectManagerCommand = "Station calibration manager";
                public static readonly string About = "About";
            }
        }

        

		public static class ContextMenu
        {


        }

    }

}
