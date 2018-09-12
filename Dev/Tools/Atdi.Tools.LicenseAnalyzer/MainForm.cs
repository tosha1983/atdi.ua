using Atdi.Modules.Licensing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Atdi.Tools.LicenseAnalyzer
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var result = openLicenseFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                txtLicenseFileName.Text = openLicenseFileDialog.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                var licenseBody = File.ReadAllBytes(txtLicenseFileName.Text);
                var license = LicenseVerifier.GetLicenseInfo(txtLicenseOwnerId.Text, txtLicenseProductKey.Text, licenseBody);

                txtNumber.Text = license.LicenseNumber;
                txtCompany.Text = license.Company;
                txtType.Text = license.LicenseType;
                txtOwner.Text = license.OwnerName;
                txtProduct.Text = license.ProductName;
                txtCreated.Text = license.Created.ToLongDateString();
                txtStartDate.Text = license.StartDate.ToLongDateString();
                txtStopDate.Text = license.StopDate.ToLongDateString();
                txtInstance.Text = license.Instance;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            

        }
    }
}
