using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSM;

namespace XICSM.ICSControlClient.Handlers.TourCommnads
{
    public class UpdateInspectionsCommand
    {
        public static bool Handle(IMQueryMenuNode.Context context)
        {
            return
                context.ExecuteContextMenuAction(
                        PluginMetadata.Processes.CreateMeasTask,
                        UpdateInspectionsByTour
                    );
        }

        private static bool UpdateInspectionsByTour(int tourId)
        {
            System.Windows.Forms.MessageBox.Show("Update Inspections for tour with ID - " + tourId + " : ", "BuildInspectionsCommand", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
            return true;
        }
    }
}
