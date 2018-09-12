using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Atdi.ConfigWizard.Sdrn.Device.WcfService
{
    [RunInstaller(true)]
    public partial class InstallerSetup : System.Configuration.Install.Installer
    {
        public InstallerSetup()
        {
            InitializeComponent();
        }

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
        }

        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);
            try
            {
                //ShowParameters();
                var form = new MainForm();
                form.ShowDialog();
            }
            catch(Exception)
            {

            }
        }

        public override void Rollback(IDictionary savedState)
        {
            base.Rollback(savedState);
        }

        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);
        }

        private void ShowParameters()
        {
            StringBuilder sb = new StringBuilder();
            StringDictionary myStringDictionary = this.Context.Parameters;
            if (this.Context.Parameters.Count > 0)
            {
                foreach (string myString in this.Context.Parameters.Keys)
                {
                    sb.AppendFormat("String={0} Value= {1}\n", myString,
                    this.Context.Parameters[myString]);
                }
            }
            MessageBox.Show(sb.ToString());
        }

    }
}
