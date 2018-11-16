using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SDR = Atdi.AppServer.Contracts.Sdrns;
using SVC = XICSM.ICSControlClient.WcfServiceClients;
using FM = XICSM.ICSControlClient.Forms;

namespace XICSM.ICSControlClient.Forms
{
    public partial class CalcSODlg1Form : Form
    {
        public int[] _planIds;
        public int[] _allIds;
        public string[] _points;
        public CalcSODlg1Form()
        {
            InitializeComponent();
        }
        private void CalcSODlg1Form_Load(object sender, EventArgs e)
        {
            dtStopMeas.Value = DateTime.Today;
            dtStartMeas.Value = dtStopMeas.Value.AddDays(-14);
            txtTrigger.Text = "-100";
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOk_Click(object sender, EventArgs ev)
        {
            try
            {
                var dlgForm = new FM.CalcSODlg2Form();
                int trigger;
                if (!int.TryParse(txtTrigger.Text, out trigger))
                {
                    System.Windows.MessageBox.Show("Invalid value - Trigger level for calculation SO, dBm");
                    return;
                }
                if (dtStopMeas.Value < dtStartMeas.Value)
                {
                    System.Windows.MessageBox.Show("Date stop mast be great of the Date start");
                    return;
                }

                var shortMeasResults = SVC.SdrnsControllerWcfClient.GetShortMeasResultsByDates(dtStartMeas.Value, dtStopMeas.Value);
                dlgForm._shortMeasResults = shortMeasResults;
                dlgForm._planIds = _planIds;
                dlgForm._points = _points;
                dlgForm._trigger = trigger;
                dlgForm._allIds = _allIds;
                dlgForm.ShowDialog();
                dlgForm.Dispose();
                this.Close();
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.ToString());
            }
        }
    }
}
