using Atdi.Modules.Licensing;
using Atdi.Platform.Cryptography;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
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

                if (license is LicenseData2 license2)
                {
                    txtTerms.Text = license2.LimitationTerms.ToString();
                    txtAssembly.Text = license2.AssemblyFullName;
                    txtYear.Text = license2.Year.ToString();
                    txtVersion.Text = license2.Version;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            

        }

        private void button3_Click(object sender, EventArgs e)
        {
            var sharedSecret = Assembly.GetAssembly(typeof(Encryptor)).FullName;

            if (cmbConfigType.SelectedIndex == 0)
            {
                sharedSecret = "Atdi.AppServer.AppService.SdrnsController";
            }
            else if (cmbConfigType.SelectedIndex == 1)
            {
                sharedSecret = "Atdi.WcfServices.Sdrn.Device";
            }
            else if (cmbConfigType.SelectedIndex == 2)
            {
                sharedSecret = "Atdi.AppServices.WebQuery";
            }
            else if (cmbConfigType.SelectedIndex == 3)
            {
                sharedSecret = "Atdi.WebPortal.WebQuery";
            }
            else if (cmbConfigType.SelectedIndex == 4)
            {
                sharedSecret = "Atdi.Tools.Sdrn.Client";
            }
            else if (cmbConfigType.SelectedIndex == 5)
            {
	            sharedSecret = "Atdi.AppServer.AppService.SdrnsController";
            }
            else if (cmbConfigType.SelectedIndex == 6) //infocentr
            {
	            sharedSecret = "Atdi.AppServer.AppService.SdrnsController";
            }
			txtEncryptedOwnerId.Text = Encryptor.EncryptStringAES(txtLicenseOwnerId.Text, sharedSecret);
            txtEncryptedProductKey.Text = Encryptor.EncryptStringAES(txtLicenseProductKey.Text, sharedSecret);
            txtEncryptedPassword.Text = Encryptor.EncryptStringAES(txtPassword.Text, sharedSecret);

        }
    }
}
