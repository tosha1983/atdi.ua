using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ICSM;

namespace XICSM.ICSControlClient.Forms
{
    public partial class gridFilterNumeric : Form
    {
        double? _filterFromValue;
        double? _filterToValue;

        public bool IsPresOK = false;

        public gridFilterNumeric()
        {
            InitializeComponent();
        }
        public double? FilterFromValue
        {
            get => this._filterFromValue == IM.NullD ? null : this._filterFromValue;
            set
            {
                if (value == IM.NullD)
                    value = null;

                this._filterFromValue = value;
                if (value.HasValue)
                    txtFrom.Value = value.Value;
            }
        }
        public double? FilterToValue
        {
            get => this._filterToValue == IM.NullD ? null : this._filterToValue;
            set
            {
                if (value == IM.NullD)
                    value = null;

                this._filterToValue = value;
                if (value.HasValue)
                    txtTo.Value = value.Value;
            }
        }
        private void btnApply_Click(object sender, EventArgs e)
        {
            IsPresOK = true;
            this.Close();
        }
        private void txtTo_ValueChanged(object sender, EventArgs e)
        {
            FilterToValue = txtTo.Value;
        }
        private void txtFrom_ValueChanged(object sender, EventArgs e)
        {
            FilterFromValue = txtFrom.Value;
        }
    }
}
