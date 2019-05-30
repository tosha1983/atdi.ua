using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSM;
using MD = XICSM.ICSControlClient.Metadata;
using HD = XICSM.ICSControlClient.Handlers.OtherTerrestrialStationsCommands;

namespace XICSM.ICSControlClient
{
    public class OtherTerrestrialStationsContextMenuBuilder
    {
        public static bool IsBuildContextMenu(string tableName, int nbRecMin)
        {
            return MD.MobStations.TableName.Equals(tableName, StringComparison.OrdinalIgnoreCase) && nbRecMin > 0;
        }

        public static List<IMQueryMenuNode> Build(string tableName, int nbRecMin)
        {
            var nodes = new List<IMQueryMenuNode>();

            if (IsBuildContextMenu(tableName, nbRecMin))
            {
                nodes.AddContextMenuToolForSelectionOfRecords(PluginMetadata.ContextMenu.OtherTerrestrialStations.ShowResultMonitoring, HD.ShowResultMonitoringCommand.Handle);
            }

            return nodes;
        }
    }
}
