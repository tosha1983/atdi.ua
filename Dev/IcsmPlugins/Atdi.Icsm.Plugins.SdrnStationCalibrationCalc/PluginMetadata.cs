﻿using Atdi.Platform.Logging;
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

	public static class PluginMetadata
    {
        public static readonly string Title = "ICS Station Calibration Calculation";
        public static readonly string Ident = "SdrnStationCalibrationCalc";
        public static readonly double SchemaVersion = 20200518.1638;

       

        public static class Menu
        {
            public static readonly string MainTool = "ICS Station Calibration";

            public static class Tools
            {
                public static readonly string RunProjectManagerCommand = "Project manager";
                public static readonly string About = "About";
            }
        }

        

		public static class ContextMenu
        {
            //public static class Tour
            //{
            //    public static readonly string BuildInspections = "Build Inspections";
            //    public static readonly string CreateMeasTask = "Create Meas Task";
            //    public static readonly string UpdateInspections = "Update Inspections";
            //    public static readonly string SynchroInspections = "Synchro Inspections";
            //    public static readonly string SelectArea = "Select Area";
            //}

            //public static class Allotment
            //{
            //    public static readonly string StartSignalization = "Start signalization";
            //    public static readonly string StartMeasurementsSO = "Start measurements spectrum occupation";
            //    public static readonly string CalcSOByMeasResult = "Calc spectrum occupation by MeasResults";
            //}
            //public static class Inspection
            //{
            //    public static readonly string ExportFieldStrength = "Export field strength";
            //}
            //public static class OtherTerrestrialStations
            //{
            //    public static readonly string ShowResultMonitoring = "Show Result Monitoring (Fix Sensor)";
            //}
            //public static class YetOtherTerrestrialStations
            //{
            //    public static readonly string ShowResultMonitoring = "Show Result Monitoring (Fix Sensor)";
            //}

        }

    }

}