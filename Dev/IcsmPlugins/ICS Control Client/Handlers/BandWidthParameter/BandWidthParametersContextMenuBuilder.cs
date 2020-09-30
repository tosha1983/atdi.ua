using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSM;
using MD = XICSM.ICSControlClient.Metadata;
using HD = XICSM.ICSControlClient.Handlers.InspectionCommnads;

namespace XICSM.ICSControlClient
{
    public class BandWidthParametersContextMenuBuilder
    {
        public static bool IsBuildContextMenu(string tableName, int nbRecMin)
        {
            return MD.ProtocolBandWidth.TableName.Equals(tableName, StringComparison.OrdinalIgnoreCase) && nbRecMin >= 0;
        }

        public static List<IMQueryMenuNode> Build(string tableName, int nbRecMin)
        {
            var nodes = new List<IMQueryMenuNode>();

            if (IsBuildContextMenu(tableName, nbRecMin))
            {
                nodes.Add(new IMQueryMenuNode(PluginMetadata.ContextMenu.BandWidthParameter.CreateNewRecord, null, HD.CreateNewRecordCommand.Handle, IMQueryMenuNode.ExecMode.Table));
                nodes.Add(new IMQueryMenuNode(PluginMetadata.ContextMenu.BandWidthParameter.UpdateRecord, null, HD.UpdateRecordCommand.Handle, IMQueryMenuNode.ExecMode.FirstRecord));
                nodes.Add(new IMQueryMenuNode(PluginMetadata.ContextMenu.BandWidthParameter.DeleteRecord, null, HD.DeleteRecordCommand.Handle, IMQueryMenuNode.ExecMode.FirstRecord));
            }
            return nodes;
        }
    }
}
