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
    public partial class ExportRSParametersForm : Form
    {
        public bool IsPresOK = false;
        enum CorrectEstimationType { False = 0, True = 1, All = 2 }
        bool? _correctEstimationValue = null;
        int _cutoff = 6;
        int _numberPoints = 6;

        public ExportRSParametersForm()
        {
            InitializeComponent();

            txtCutoff.Text = "6";
            txtNumberPoints.Text = "6";

            cmbCorrEstim.DataSource = Enum.GetValues(typeof(CorrectEstimationType)).Cast<CorrectEstimationType>().Select(p => new { Name = Enum.GetName(typeof(CorrectEstimationType), p), Value = (int)p }).ToList();
            cmbCorrEstim.DisplayMember = "Name";
            cmbCorrEstim.ValueMember = "Name";
            cmbCorrEstim.SelectedValue = CorrectEstimationType.All.ToString();
        }

        public bool? CorrectEstimationValue
        {
            get => this._correctEstimationValue;
            set
            {
                this._correctEstimationValue = value;
                if (value.HasValue)
                    cmbCorrEstim.SelectedValue = value.Value ? CorrectEstimationType.True.ToString() : CorrectEstimationType.False.ToString();
                else
                    cmbCorrEstim.SelectedValue = CorrectEstimationType.All.ToString();
            }
        }
        public int Cutoff
        {
            get => this._cutoff;
            set => this._cutoff = value;
        }
        public int NumberPoints
        {
            get => this._numberPoints;
            set => this._numberPoints = value;
        }
        private void cmbFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((CorrectEstimationType)Enum.Parse(typeof(CorrectEstimationType), cmbCorrEstim.SelectedValue.ToString()) == CorrectEstimationType.True)
                CorrectEstimationValue = true;
            else if ((CorrectEstimationType)Enum.Parse(typeof(CorrectEstimationType), cmbCorrEstim.SelectedValue.ToString()) == CorrectEstimationType.False)
                CorrectEstimationValue = false;
            else
                CorrectEstimationValue = null;
        }
        private void btnApply_Click(object sender, EventArgs e)
        {
            IsPresOK = true;
            this.Close();
        }

        private void txtCutoff_TextChanged(object sender, EventArgs e)
        {
            Int32.TryParse(txtCutoff.Text, out int result);
            this._cutoff = result;
        }

        private void txtNumberPoints_TextChanged(object sender, EventArgs e)
        {
            Int32.TryParse(txtNumberPoints.Text, out int result);
            this._numberPoints = result;
        }
    }
}
