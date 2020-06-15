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
    public partial class gridFilterDate : gridFilterBase
    {
        DateTime? _filterFromValue;
        DateTime? _filterToValue;
        public gridFilterDate()
        {
            InitializeComponent();
            this._filterFromValue = DateTime.Now;
            this._filterToValue = DateTime.Now;
        }
        public DateTime? FilterFromValue
        {
            get => this._filterFromValue;
            set
            {
                this._filterFromValue = value;
                if (value.HasValue)
                    dtFrom.Value = value.Value;
            }
        }
        public DateTime? FilterToValue
        {
            get => this._filterToValue;
            set
            {
                this._filterToValue = value;
                if (value.HasValue)
                    dtTo.Value = value.Value;
            }
        }
        private void dtFrom_ValueChanged(object sender, EventArgs e)
        {
            FilterFromValue = dtFrom.Value;
        }
        private void dtTo_ValueChanged(object sender, EventArgs e)
        {
            FilterToValue = dtTo.Value;
        }
        private void btnApply_Click(object sender, EventArgs e)
        {
            IsPresOK = true;
            this.Close();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            this._filterFromValue = null;
            this._filterToValue = null;
            IsPresOK = true;
            this.Close();
        }
    }
}
