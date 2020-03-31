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
    public partial class gridFilterString : Form
    {
        string _filterValue = "";

        public bool IsPresOK = false;

        public gridFilterString()
        {
            InitializeComponent();
        }
        public string FilterValue
        {
            get => this._filterValue;
            set
            {
                this._filterValue = value;
                txtFilter.Text = value;
            }
        }
        private void btnApply_Click(object sender, EventArgs e)
        {
            IsPresOK = true;
            this.Close();
        }
        private void ValueChanged(object sender, EventArgs e)
        {
            FilterValue = txtFilter.Text;
        }
    }
}
