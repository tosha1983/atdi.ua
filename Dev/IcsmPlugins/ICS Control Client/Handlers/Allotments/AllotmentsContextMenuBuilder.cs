using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSM;
using MD = XICSM.ICSControlClient.Metadata;
using HD = XICSM.ICSControlClient.Handlers.AllotmentCommnads;

namespace XICSM.ICSControlClient
{
    public class AllotmentsContextMenuBuilder
    {
        public static bool IsBuildContextMenu(string tableName, int nbRecMin)
        {
            return MD.Allotments.TableName.Equals(tableName, StringComparison.OrdinalIgnoreCase) && nbRecMin > 0;
        }

        public static List<IMQueryMenuNode> Build(string tableName, int nbRecMin)
        {
            var nodes = new List<IMQueryMenuNode>();

            if (IsBuildContextMenu(tableName, nbRecMin))
            {
                nodes.AddContextMenuToolForEachRecords(
                    PluginMetadata.ContextMenu.Allotment.StartMeasurementsSO,
                    HD.StartMeasurementsSOCommand.Handle
                );

                nodes.Add(new IMQueryMenuNode(PluginMetadata.ContextMenu.Allotment.CalcSOByMeasResult, null, HD.CalcSOByMeasResultCommand.Handle, IMQueryMenuNode.ExecMode.SelectionOfRecords));
            }

            return nodes;
        }
    }
}
