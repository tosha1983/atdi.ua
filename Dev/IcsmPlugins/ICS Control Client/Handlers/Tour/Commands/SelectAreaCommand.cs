using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSM;
using XICSM.ICSControlClient.Environment;
using DM = XICSM.ICSControlClient.Models.SynchroInspections;

namespace XICSM.ICSControlClient.Handlers.TourCommnads
{
    public class SelectAreaCommand
    {
        public static bool Handle(IMQueryMenuNode.Context context)
        {
            return
                context.ExecuteContextMenuAction(
                        PluginMetadata.Processes.SelectArea,
                        SelectAreaForTour
                    );
        }
        private static bool SelectAreaForTour(int tourId)
        {
            //var tour = Repository.ReadEntityById<DM.Tour>(tourId);

            //tour.
            //Repository.UpdateEntity(tour);


            System.Windows.Forms.MessageBox.Show("Select Area for tour with ID - " + tourId + " : ", "SelectAreaCommand", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
            return true;
        }
    }
}
