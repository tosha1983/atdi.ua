using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSM;
using XICSM.ICSControlClient.Environment;
using FM = XICSM.ICSControlClient.Forms;

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
            try
            {
                var measTaskForm = new FM.SelectAreaForm();
                measTaskForm.tourId = tourId;
                measTaskForm.ShowDialog();
                measTaskForm.Dispose();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

            //System.Windows.Forms.MessageBox.Show("Select Area for tour with ID - " + tourId + " : ", "SelectAreaCommand", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
            return true;
        }
    }
}
