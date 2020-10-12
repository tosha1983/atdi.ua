using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using ICSM;
using XICSM.ICSControlClient.Environment;
using MD = XICSM.ICSControlClient.Metadata;
using DM = XICSM.ICSControlClient.Models.StartMeasurementsSO;
using WCF = XICSM.ICSControlClient.WcfServiceClients;
using SDR = Atdi.Contracts.WcfServices.Sdrn.Server;
using SVC = XICSM.ICSControlClient.WcfServiceClients;
using FM = XICSM.ICSControlClient.Forms;
using FRM = System.Windows.Forms;
using XICSM.ICSControlClient.Forms;

namespace XICSM.ICSControlClient.Handlers.InspectionCommnads
{
    public class CreateNewRecordCommand
    {
        public static bool Handle(IMQueryMenuNode.Context context)
        {
            ReportBandWidthForm reportBandWidthForm = new ReportBandWidthForm(-1);
            reportBandWidthForm.ShowDialog();
            IM.RefreshQueries(context.TableName);
            return true;
        }
      
    }

    public class UpdateRecordCommand
    {
        public static bool Handle(IMQueryMenuNode.Context context)
        {
            ReportBandWidthForm reportBandWidthForm = new ReportBandWidthForm(context.TableId);
            reportBandWidthForm.ShowDialog();
            return true;
        }

    }

    public class DeleteRecordCommand
    {
        public static bool Handle(IMQueryMenuNode.Context context)
        {
            if (System.Windows.Forms.MessageBox.Show("Delete selected record?", "Question", FRM.MessageBoxButtons.YesNo) == FRM.DialogResult.Yes)
            {
                IMRecordset recDel = new IMRecordset(context.TableName, IMRecordset.Mode.ReadWrite);
                recDel.Select("ID");
                recDel.SetWhere("ID", IMRecordset.Operation.Eq, context.TableId);
                recDel.Open();
                if (!recDel.IsEOF())
                {
                    recDel.Delete();
                }

                if (recDel.IsOpen())
                    recDel.Close();
                recDel.Destroy();
                IM.RefreshQueries(context.TableName);
            }
            return true;
        }

    }
}
