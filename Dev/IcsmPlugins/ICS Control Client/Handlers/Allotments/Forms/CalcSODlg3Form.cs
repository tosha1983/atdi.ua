using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using SVC = XICSM.ICSControlClient.WcfServiceClients;
using FM = XICSM.ICSControlClient.Forms;
using SDR = Atdi.Contracts.WcfServices.Sdrn.Server;

namespace XICSM.ICSControlClient.Forms
{
    public partial class CalcSODlg3Form : Form
    {
        public SDR.SOFrequency[] _soFrequency;
        public CalcSODlg3Form()
        {
            InitializeComponent();
        }
        private void CalcSODlg3Form_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < _soFrequency.Count(); i++)
            {
                var ms = _soFrequency[i] as SDR.SOFrequency;
                var ocup = ms.OccupationByHuors.Split(';');
                var j = 0;

                var index = dataGrid.Rows.Add(ms.Frequency_MHz, ms.hit, ms.Occupation, ms.StantionIDs, ms.countStation, ms.OccupationByHuors);

                foreach (var oc in ocup)
                {
                    if (j > 23)
                        break;
                    dataGrid.Rows[index].Cells["Oc" + j.ToString()].Value = oc;
                    j++;
                }
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnGetCsv_Click(object sender, EventArgs e)
        {
            string filename = "";
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSV (*.csv)|*.csv";
            sfd.FileName = "Output.csv";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                //MessageBox.Show("Data will be exported and you will be notified when it is ready.");
                if (File.Exists(filename))
                {
                    try
                    {
                        File.Delete(filename);
                    }
                    catch (IOException ex)
                    {
                        MessageBox.Show("It wasn't possible to write the data to the disk." + ex.Message);
                    }
                }
                int columnCount = dataGrid.ColumnCount;
                string columnNames = "";
                string[] output = new string[dataGrid.RowCount + 1];
                for (int i = 0; i < columnCount; i++)
                {
                    columnNames += dataGrid.Columns[i].Name.ToString() + ";";
                }
                output[0] += columnNames;
                for (int i = 1; (i - 1) < dataGrid.RowCount; i++)
                {
                    for (int j = 0; j < columnCount; j++)
                    {
                        output[i] += dataGrid.Rows[i - 1].Cells[j].Value.ToString() + ";";
                    }
                }
                System.IO.File.WriteAllLines(sfd.FileName, output, System.Text.Encoding.UTF8);
                MessageBox.Show("Your file was generated and its ready for use.");
            }
        }

        private void btnOk_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
