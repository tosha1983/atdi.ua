using Atdi.Platform.Logging;
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
    internal static class Exceptions
    {
        public static readonly string GE06Client = "GE06Client";
    }
    public static class PluginMetadata
    {
        public static readonly string Title = "ICS GE06Calc";
        public static readonly string Ident = "GE06Calc";
        public static readonly double SchemaVersion = 20200728.1630;

        public class Processes
        {
            public static readonly string StartGE06Task = "GE06";
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
            public static class FMTV_Assign
            {
                public static readonly string StartGE06Task = "GE06";
            }
            public static class GE06_allot_terra
            {
                public static readonly string StartGE06Task = "GE06";
            }
            public static class FMTV_terra
            {
                public static readonly string StartGE06Task = "GE06";
            }
        }
    }
}
