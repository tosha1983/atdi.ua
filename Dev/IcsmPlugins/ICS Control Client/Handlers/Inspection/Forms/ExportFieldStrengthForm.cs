using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using ICSM;
using System.Windows.Forms;
using FM = XICSM.ICSControlClient.Forms;
using SDR = Atdi.Contracts.WcfServices.Sdrn.Server;

namespace XICSM.ICSControlClient.Forms
{
    public partial class ExportFieldStrengthForm : Form
    {
        public SDR.ShortMeasurementResults[] _shortMeasResults;
        public List<int> _measResultIds = new List<int>();

        public ExportFieldStrengthForm()
        {
            InitializeComponent();
        }

        private void ExportFieldStrengthForm_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < _shortMeasResults.Count(); i++)
            {
                var ms = _shortMeasResults[i] as SDR.ShortMeasurementResults;
                dataGrid.Rows.Add(ms.Id.MeasSdrResultsId, ms.TimeMeas, ms.DataRank, ms.Status, ms.TypeMeasurements, ms.SensorName);
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGrid.SelectedRows.Count; i++)
            {
                var cc = dataGrid.SelectedRows[i].Cells[0].Value;
                int measResultId;

                if (cc != null && int.TryParse(cc.ToString(), out measResultId))
                {
                    _measResultIds.Add(measResultId);
                }
            }
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            dataGrid.SelectAll();
        }
    }
}
