using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSM;
using MD = XICSM.ICSControlClient.Metadata;
using HD = XICSM.ICSControlClient.Handlers.InspectionCommnads;

namespace XICSM.ICSControlClient
{
    public class InspectionsContextMenuBuilder
    {
        public static bool IsBuildContextMenu(string tableName, int nbRecMin)
        {
            return MD.Inspection.TableName.Equals(tableName, StringComparison.OrdinalIgnoreCase) && nbRecMin > 0;
        }

        public static List<IMQueryMenuNode> Build(string tableName, int nbRecMin)
        {
            var nodes = new List<IMQueryMenuNode>();

            if (IsBuildContextMenu(tableName, nbRecMin))
            {
                nodes.Add(new IMQueryMenuNode(PluginMetadata.ContextMenu.Inspection.ExportFieldStrength, null, HD.ExportFieldStrengthCommand.Handle, IMQueryMenuNode.ExecMode.SelectionOfRecords));
            }

            return nodes;
        }
    }
}
