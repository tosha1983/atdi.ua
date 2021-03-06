﻿using Atdi.Modules.Licensing;
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

                txtTerms.Text = string.Empty;
                txtAssembly.Text = string.Empty;
                txtYear.Text = string.Empty;
                txtVersion.Text = string.Empty;
				txtExteranServices.Text = string.Empty;
				if (license is LicenseData2 license2)
                {
                    txtTerms.Text = license2.LimitationTerms.ToString();
                    txtAssembly.Text = license2.AssemblyFullName;
                    txtYear.Text = license2.Year.ToString();
                    txtVersion.Text = license2.Version;
                }
				if (license is LicenseData4 license4)
				{
					if (license4.ExternalServices != null && license4.ExternalServices.Length > 0)
					{
						var builder = new StringBuilder();
						var index = 0;
						foreach (var serviceDescriptor in license4.ExternalServices)
						{
							++index;
							builder.AppendLine($" {index:D3} SID='{serviceDescriptor.Id}'; Name='{serviceDescriptor.Name}'");
						}

						txtExteranServices.Text = builder.ToString();
					}
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
            else if (cmbConfigType.SelectedIndex == 7) // SDRN Calc Server Client ICSM Plugin
			{
	            sharedSecret = "9BE22B3F-2BA7-4486-9EE3-040A64A5CAD3";
            }
            else if (cmbConfigType.SelectedIndex == 8) // SDRN Station Calibration Calc ICSM Plugin
			{
	            sharedSecret = "A77839F8-5546-41C9-A6D9-3777894D3E41";
            }
            else if (cmbConfigType.SelectedIndex == 9) // SDRN GE06 Calc ICSM Plugin
            {
	            sharedSecret = "30A5488D-1AC7-41CB-B078-856733113E26";
            }

			txtEncryptedOwnerId.Text = Encryptor.EncryptStringAES(txtLicenseOwnerId.Text, sharedSecret);
			ToolTip tt = new ToolTip();
			tt.IsBalloon = true;
			tt.InitialDelay = 0;
			tt.ShowAlways = true;
			tt.SetToolTip(txtEncryptedOwnerId, Encryptor.DecryptStringAES(txtEncryptedOwnerId.Text, sharedSecret));

			txtEncryptedProductKey.Text = Encryptor.EncryptStringAES(txtLicenseProductKey.Text, sharedSecret);
			tt = new ToolTip();
			tt.IsBalloon = true;
			tt.InitialDelay = 0;
			tt.ShowAlways = true;
			tt.SetToolTip(txtEncryptedProductKey, Encryptor.DecryptStringAES(txtEncryptedProductKey.Text, sharedSecret));

			txtEncryptedPassword.Text = Encryptor.EncryptStringAES(txtPassword.Text, sharedSecret);
			tt = new ToolTip();
			tt.IsBalloon = true;
			tt.InitialDelay = 0;
			tt.ShowAlways = true;
			tt.SetToolTip(txtEncryptedPassword, Encryptor.DecryptStringAES(txtEncryptedPassword.Text, sharedSecret));

		}

		private void button4_Click(object sender, EventArgs e)
		{
			txtHostKey.Text = LicenseVerifier.GetHostKey();
			button5.Enabled = true;
			button6.Enabled = true;
		}

		private void button5_Click(object sender, EventArgs e)
		{
			Clipboard.SetText(txtHostKey.Text);
		}

		private void button6_Click(object sender, EventArgs e)
		{
			var saveDialog = new SaveFileDialog
			{
				Filter = @"Text|*.txt",
				Title = "Save an Host Key Info File",
				FileName = $"HostKey_{Environment.MachineName}_.txt"
			};

			var result = saveDialog.ShowDialog();

			if (result == DialogResult.OK && !string.IsNullOrEmpty(saveDialog.FileName))
			{
				File.WriteAllText(saveDialog.FileName, txtHostKey.Text);
			}
		}
	}
}
