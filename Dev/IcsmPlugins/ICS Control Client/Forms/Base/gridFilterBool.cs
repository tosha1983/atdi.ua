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
    public partial class gridFilterBool : gridFilterBase
    {
        enum FilterValues { False = 0, True = 1, All = 2 }
        bool? _filterValue = null;

        public gridFilterBool()
        {
            InitializeComponent();

            cmbFilter.DataSource = Enum.GetValues(typeof(FilterValues)).Cast<FilterValues>().Select(p => new { Name = Enum.GetName(typeof(FilterValues), p), Value = (int)p }).ToList();
            cmbFilter.DisplayMember = "Name";
            cmbFilter.ValueMember = "Name";
            cmbFilter.SelectedValue = FilterValues.All.ToString();
        }
        public bool? FilterValue
        {
            get => this._filterValue;
            set
            {
                this._filterValue = value;
                if (value.HasValue)
                    cmbFilter.SelectedValue = value.Value ? FilterValues.True.ToString() : FilterValues.False.ToString();
                else
                    cmbFilter.SelectedValue = FilterValues.All.ToString();
            }
        }
        private void cmbFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((FilterValues)Enum.Parse(typeof(FilterValues), cmbFilter.SelectedValue.ToString()) == FilterValues.True)
                FilterValue = true;
            else if ((FilterValues)Enum.Parse(typeof(FilterValues), cmbFilter.SelectedValue.ToString()) == FilterValues.False)
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
