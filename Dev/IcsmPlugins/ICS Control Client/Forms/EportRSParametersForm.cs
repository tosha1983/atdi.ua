using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XICSM.ICSControlClient.Forms
{
    public partial class EportRSParametersForm : Form
    {
        public bool IsPresOK = false;
        enum CorrectEstimationValue { False = 0, True = 1, All = 2 }
        bool? _filterValue = null;

        public EportRSParametersForm()
        {
            InitializeComponent();

            cmbCorrEstim.DataSource = Enum.GetValues(typeof(CorrectEstimationValue)).Cast<CorrectEstimationValue>().Select(p => new { Name = Enum.GetName(typeof(CorrectEstimationValue), p), Value = (int)p }).ToList();
            cmbCorrEstim.DisplayMember = "Name";
            cmbCorrEstim.ValueMember = "Name";
            cmbCorrEstim.SelectedValue = CorrectEstimationValue.All.ToString();
        }

        public bool? FilterValue
        {
            get => this._filterValue;
            set
            {
                this._filterValue = value;
                if (value.HasValue)
                    cmbCorrEstim.SelectedValue = value.Value ? CorrectEstimationValue.True.ToString() : CorrectEstimationValue.False.ToString();
                else
                    cmbCorrEstim.SelectedValue = CorrectEstimationValue.All.ToString();
            }
        }
        private void cmbFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((CorrectEstimationValue)Enum.Parse(typeof(CorrectEstimationValue), cmbCorrEstim.SelectedValue.ToString()) == CorrectEstimationValue.True)
                FilterValue = true;
            else if ((CorrectEstimationValue)Enum.Parse(typeof(CorrectEstimationValue), cmbCorrEstim.SelectedValue.ToString()) == CorrectEstimationValue.False)
                FilterValue = false;
            else
                FilterValue = null;
        }
        private void btnApply_Click(object sender, EventArgs e)
        {
            IsPresOK = true;
            this.Close();
        }
    }
}
