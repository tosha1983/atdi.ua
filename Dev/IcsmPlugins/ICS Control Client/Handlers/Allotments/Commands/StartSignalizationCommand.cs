using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSM;
using XICSM.ICSControlClient.Environment;
using MD = XICSM.ICSControlClient.Metadata;
using DM = XICSM.ICSControlClient.Models.StartMeasurementsSO;
using WCF = XICSM.ICSControlClient.WcfServiceClients;
using SDR = Atdi.Contracts.WcfServices.Sdrn.Server;
using AAC = Atdi.DataModels.DataConstraint;
using FM = XICSM.ICSControlClient.Forms;

namespace XICSM.ICSControlClient.Handlers.AllotmentCommnads
{
    public class StartSignalizationCommand
    {
        public static bool Handle(IMQueryMenuNode.Context context)
        {
            return context.ExecuteContextMenuAction(PluginMetadata.Processes.StartMeasurementsSO, CreateMeasTask);
        }
        private static bool CreateMeasTask(int allotmentId)
        {
            try
            {
                var allotment = Repository.ReadEntityById<DM.Allotment>(allotmentId);

                if (!allotment.IsNewState)
                {
                    Logger.WriteWarning(PluginMetadata.Processes.CreateMeasTask, $"Incorrect the status of the allotment #{allotment.Id}", true);
                    return false;
                }

                var measTaskForm = new FM.MeasTaskForm(allotmentId, SDR.MeasurementType.Signaling);
                measTaskForm.ShowDialog();
                measTaskForm.Dispose();
                
                allotment.Status = MD.Allotments.Statuses.Dur;
                allotment.MeasTaskId = measTaskForm.TaskId;
                if (allotment.MeasTaskId > 0)
                    Repository.UpdateEntity(allotment);

                return true;
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.ToString());
                return false;
            }
        }
    }
}
