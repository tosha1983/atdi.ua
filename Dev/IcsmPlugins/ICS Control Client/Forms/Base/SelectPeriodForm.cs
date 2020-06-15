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
    public partial class SelectPeriodForm : Form
    {
        public bool IsPresOK = false;
        string[] months = new string[12] {
            "Січень",
            "Лютий",
            "Березень",
            "Квітень",
            "Травень",
            "Червень",
            "Липень",
            "Серпень",
            "Вересень",
            "Жовтень",
            "Листопад",
            "Грудень",
        };
        public string CurrentMonthName = string.Empty;
        public int CurrentMonth = 0;
        public int CurrentYear = 0;
        public SelectPeriodForm()
        {
            InitializeComponent();
            cmbMonth.Items.AddRange(months);
            cmbMonth.SelectedIndex = DateTime.Now.Month-1;
            CurrentYear = DateTime.Now.Year;
            textBoxYear.Text = CurrentYear.ToString();
            CurrentMonth = DateTime.Now.Month;
        }

        private void cmbMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentMonth = cmbMonth.SelectedIndex+1;
            CurrentMonthName = cmbMonth.Items[cmbMonth.SelectedIndex].ToString();
        }
        private void btnApply_Click(object sender, EventArgs e)
        {
            IsPresOK = true;
            try
            {
                CurrentYear = Convert.ToInt32(textBoxYear.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Incorrect fill field 'Year'!");
                return;
            }
            this.Close();
        }

    }
}
