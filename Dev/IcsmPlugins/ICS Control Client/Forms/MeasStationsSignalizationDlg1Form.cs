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
    public partial class MeasStationsSignalizationDlg1Form : Form
    {
        double _distance;
        double _bw;
        public bool IsPresOK = false;
        public MeasStationsSignalizationDlg1Form(double defaultDistance, double defaultBw, bool isHideBw)
        {
            InitializeComponent();
            icsDistance.Value = defaultDistance;
            this._distance = defaultDistance;
            icsBw.Value = defaultBw;
            this._bw = defaultBw;
            if (isHideBw)
            {
                icsBw.Visible = false;
                lblBw.Visible = false;
                this.Height = 170;
            }
        }

        public double Distance
        {
            get => this._distance;
            set { this._distance = value; }

        }
        public double Bw
        {
            get => this._bw;
            set { this._bw = value; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IsPresOK = true;
            this.Close();
        }

        private void icsDistance_ValueChanged(object sender, EventArgs e)
        {
            Distance = icsDistance.Value;
        }

        private void icsBw_ValueChanged(object sender, EventArgs e)
        {
            Bw = icsBw.Value;
        }
    }
}
