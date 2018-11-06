using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSM;
using XICSM.ICSControlClient.Environment;
using MD = XICSM.ICSControlClient.Metadata;
using DM = XICSM.ICSControlClient.Models.StartMeasurementsSO;
using WCF = XICSM.ICSControlClient.WcfServiceClients;
using SDR = Atdi.AppServer.Contracts.Sdrns;
using AAC = Atdi.AppServer.Contracts;
using FM = XICSM.ICSControlClient.Forms;

namespace XICSM.ICSControlClient.Handlers.AllotmentCommnads
{
    public class CalcSOByMeasResultCommand
    {
        public static bool Handle(IMQueryMenuNode.Context context)
        {
            List<int> plans = new List<int>();
            List<string> points = new List<string>();
            List<int> allotments = new List<int>();

            IMRecordset rs = new IMRecordset(context.TableName, IMRecordset.Mode.ReadWrite);
            rs.Select("ID,PLAN_ID,Area.POINTS");
            rs.AddSelectionFrom(context.DataList, IMRecordset.WhereCopyOptions.SelectedLines);
            for (rs.Open(); !rs.IsEOF(); rs.MoveNext())
            {
                var planId = rs.GetI("PLAN_ID");
                if (planId != 0 && planId != IM.NullI)
                {
                    plans.Add(planId);
                    points.Add(rs.GetS("Area.POINTS"));
                    allotments.Add(rs.GetI("ID"));
                }
            }

            if (plans.Count > 0)
                CalcSOByMeasResult(plans.ToArray(), points.ToArray(), allotments.ToArray());

            return true;
        }
        private static bool CalcSOByMeasResult(int[] planIds, string[] points, int[] allotments)
        {
            try
            {
                var dlgForm = new FM.CalcSODlg1Form();
                dlgForm._planIds = planIds;
                dlgForm._points = points;
                dlgForm._allIds = allotments;
                dlgForm.ShowDialog();
                dlgForm.Dispose();
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.ToString());
            }

            return true;
        }
    }
}
