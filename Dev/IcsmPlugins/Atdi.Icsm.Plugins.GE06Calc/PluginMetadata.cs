﻿using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Icsm.Plugins.GE06Calc
{
    internal static class Contexts
    {
        public static readonly EventContext DataAdapter = "DataAdapter";
    }

    internal static class Categories
    {
        public static readonly EventCategory Refresh = "Refresh";
    }

    public static class PluginMetadata
    {
        public static readonly string Title = "ICS GE06Calc";
        public static readonly string Ident = "GE06Calc";
        public static readonly double SchemaVersion = 20200610.1300;

        public class Processes
        {
            public static readonly string ShowResultMonitoring = "Show Result Monitoring";
        }

        public static class Menu
        {
            public static readonly string BeforeTool = "Tools";
            public static readonly string MainTool = "ICS GE06Calc";

            public static class Tools
            {
                public static readonly string RunSettingsCommand = "Settings";
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