using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XICSM.ICSControlClient
{
    /// <summary>
    /// The metadata of the plugin
    /// </summary>
    public static class PluginMetadata
    {
        public static readonly string Title = "ICS Control Client";
        public static readonly string Ident = "ICSControlClient";
        public static readonly double SchemaVersion = 20180216.1443;

        public class Processes
        {
            public static readonly string BuildInspections = "Build Inspections";
            public static readonly string CreateMeasTask = "Create Meas Task";
            public static readonly string UpdateInspections = "Update Inspections";
            public static readonly string SynchroInspections = "Synchro Inspections";
            public static readonly string InvokeWcfOperation = "Invoke WCF Operation";
            public static readonly string SaveTaskToLog = "Save task to log";
            public static readonly string StartMeasurementsSO = "Start Measurements Spectrum Occupation";
            public static readonly string SelectArea = "Select Area";
        }

        public static class Menu
        {
            public static readonly string BeforeTool = "Tools";
            public static readonly string MainTool = "ICS Control";

            public static class Tools
            {
                public static readonly string Run = "Run ...";
                public static readonly string About = "About";
            }
        }

        public static class ContextMenu
        {
            public static class Tour
            {
                public static readonly string BuildInspections = "Build Inspections";
                public static readonly string CreateMeasTask = "Create Meas Task";
                public static readonly string UpdateInspections = "Update Inspections";
                public static readonly string SynchroInspections = "Synchro Inspections";
                public static readonly string SelectArea = "Select Area";
            }

            public static class Allotment
            {
                public static readonly string StartMeasurementsSO = "Start measurements spectrum occupation";
            }
        }
        
    }
}
