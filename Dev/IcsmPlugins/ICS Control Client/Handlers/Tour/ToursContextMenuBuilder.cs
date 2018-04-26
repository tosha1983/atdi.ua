using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSM;
using MD = XICSM.ICSControlClient.Metadata;
using HD = XICSM.ICSControlClient.Handlers.TourCommnads;

namespace XICSM.ICSControlClient
{
    public class ToursContextMenuBuilder
    {
        public static bool IsBuildContextMenu(string tableName, int nbRecMin)
        {
            return MD.Tours.TableName.Equals(tableName, StringComparison.OrdinalIgnoreCase) && nbRecMin > 0;
        }

        public static List<IMQueryMenuNode> Build(string tableName, int nbRecMin)
        {
            var nodes = new List<IMQueryMenuNode>();

            if (IsBuildContextMenu(tableName, nbRecMin))
            {
                nodes.AddContextMenuToolForEachRecords(
                    PluginMetadata.ContextMenu.Tour.BuildInspections, 
                    HD.BuildInspectionsCommand.Handle
                );

                nodes.AddContextMenuToolForEachRecords(
                    PluginMetadata.ContextMenu.Tour.CreateMeasTask,
                    HD.CreateMeasTaskCommand.Handle
                );

                nodes.AddContextMenuToolForEachRecords(
                    PluginMetadata.ContextMenu.Tour.UpdateInspections,
                    HD.UpdateInspectionsCommand.Handle
                );

                nodes.AddContextMenuToolForEachRecords(
                    PluginMetadata.ContextMenu.Tour.SynchroInspections,
                    HD.SynchroInspectionsCommand.Handle
                );

            }

            return nodes;
        }
    }
}
