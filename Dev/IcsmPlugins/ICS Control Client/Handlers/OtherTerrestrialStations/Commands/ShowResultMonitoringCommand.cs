using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSM;
using XICSM.ICSControlClient.Environment;
using MD = XICSM.ICSControlClient.Metadata;
using DM = XICSM.ICSControlClient.Models.ShowResultMonitoring;
using WCF = XICSM.ICSControlClient.WcfServiceClients;
using FM = XICSM.ICSControlClient.Forms;

namespace XICSM.ICSControlClient.Handlers.OtherTerrestrialStationsCommands
{
    public class ShowResultMonitoringCommand
    {
        public static bool Handle(IMQueryMenuNode.Context context)
        {
            return
                context.ExecuteContextMenuAction(
                        PluginMetadata.Processes.ShowResultMonitoring,
                        ShowResultMonitoring
                    );

        }
        private static bool ShowResultMonitoring(IMDBList selectedItems)
        {
            var stations = Repository.GetEntitiesBySelected<DM.MobStations>(selectedItems);

            return true;
        }
    }
}
