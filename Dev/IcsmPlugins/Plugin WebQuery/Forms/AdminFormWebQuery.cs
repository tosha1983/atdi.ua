using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ICSM;
using System.Web;
using System.Web.Security;
using System.Security.Cryptography;
using OrmCs;
using DatalayerCs;

namespace XICSM.WebQuery
{
    public partial class AdminFormWebQuery : Form
    {
        public bool isNew { get; set; }
        public IMQueryMenuNode.Context Lst_ { get; set; }
        public int ID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public AdminFormWebQuery(IMQueryMenuNode.Context Lst)
        {
            InitializeComponent();
            comboBoxLevelAccess.SelectedIndex = 0;
            ID = -1;
            Init(Lst);
            Lst_ = Lst;
        }

        public List<string> GetAllTaskForceShortName()
        {
            List<string> AllTaskForces = new List<string>();
            AllTaskForces.Add("");
            YTaskforce tskf = new YTaskforce();
            tskf.Format("SHORT_NAME");
            tskf.Distinct = true;
            for (tskf.OpenRs();!tskf.IsEOF();tskf.MoveNext()) {
                if (!AllTaskForces.Contains(tskf.m_short_name))
                    AllTaskForces.Add(tskf.m_short_name);
            }
            tskf.Close();
            tskf.Dispose();
            return AllTaskForces;
        }

     
        /// <summary>
        /// 
        /// </summary>
        public void Init(IMQueryMenuNode.Context Lst)
        {
            if (Lst.TableId == IM.NullI) isNew = true;
            comboBox_group.Items.AddRange(GetAllTaskForceShortName().ToArray());
            IMRecordset RsWebQuery = new IMRecordset(Lst.TableName, IMRecordset.Mode.ReadOnly);
            RsWebQuery.Select("ID,Name,Status,Query,Comments,IdentUser,IsSqlRequest,RightGroup");
            if (Lst.DataList == null) return;
            RsWebQuery.AddSelectionFrom(Lst.DataList, IMRecordset.WhereCopyOptions.SelectedLines);
            for (RsWebQuery.Open(); !RsWebQuery.IsEOF(); RsWebQuery.MoveNext()) {
                                textBoxName.Text = RsWebQuery.GetS("Name");
                                textBoxDescrQuery.Text = RsWebQuery.GetS("Comments");
                                byte[] zip = null;
                                string sql; ANetDb d;
                                OrmCs.OrmSchema.Linker.PrepareExecute("SELECT Query FROM %XWebQuery WHERE ID=" + RsWebQuery.GetI("ID"), out sql, out d);
                                ANetRs Anet_rs = d.NewRecordset();
                                Anet_rs.Open(sql);
                                if (!Anet_rs.IsEOF())
                                zip = Anet_rs.GetBinary(0);
                                Anet_rs.Destroy();
                                if (zip != null)  {
                                    string cipherText = CryptorEngine.Decrypt(zip, true);
                                    textBoxQuery.Text = cipherText;
                                }
                                txtUserIdent.Text = RsWebQuery.GetS("IdentUser").ToString();
                                int idx = comboBoxLevelAccess.FindString(RsWebQuery.GetS("Status"));
                                if (idx > -1) comboBoxLevelAccess.SelectedIndex = idx;
                                idx = comboBox_group.FindString(RsWebQuery.GetS("RightGroup"));
                                if (idx > -1) comboBox_group.SelectedIndex = idx;
                                if (RsWebQuery.GetI("IsSqlRequest") == 1) { checkBoxSQLmode.Checked = true; } else { checkBoxSQLmode.Checked = false; }
                                ID = RsWebQuery.GetI("ID");
                                isNew = false;

            }

        }

      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonOpenIRP_Click(object sender, EventArgs e)
        {
            openFileDialog1.DefaultExt = "IRP";
            openFileDialog1.AddExtension = true;
            DialogResult DA = openFileDialog1.ShowDialog();
            if (DA == System.Windows.Forms.DialogResult.OK) {
                textBoxIRPFilePath.Text = openFileDialog1.FileName;
                using (StreamReader rdr = new StreamReader(textBoxIRPFilePath.Text, Encoding.Default)) {
                   string cipherText = rdr.ReadToEnd();
                   textBoxQuery.Text = cipherText;
                }

            }
        }

        private void ButtonSaveChangeQuery_Click(object sender, EventArgs e)
        {
            DialogResult DA = MessageBox.Show("Editable query manually may cause it to malfunction on the side of the Web portal to continue?", "Warning!", MessageBoxButtons.YesNo);
            if (DA == System.Windows.Forms.DialogResult.Yes){
                {
                    IMRecordset RsWebQuery = new IMRecordset(ICSMTbl.WebQuery, IMRecordset.Mode.ReadWrite);
                    RsWebQuery.Select("ID,Name,Status,Query,Comments,IdentUser,IsSqlRequest,RightGroup");
                    RsWebQuery.AddSelectionFrom(Lst_.DataList, IMRecordset.WhereCopyOptions.SelectedLines);
                    try {
                        RsWebQuery.Open();
                        if (!RsWebQuery.IsEOF()) {
                            try {
                                byte[] cipherText = CryptorEngine.Encrypt(textBoxQuery.Text.ToString(), true);
                                string sql; ANetDb d; ANetNQ nq = null;
                                IMTransaction.Begin();
                                OrmCs.OrmSchema.Linker.PrepareExecute("UPDATE %XWebQuery SET Query=@p0 WHERE ID=@p1", out sql, out d);
                                nq = d.NewAdapter();
                                nq.Init(sql, "binary,int");
                                nq.SetParamValue(0, cipherText);
                                nq.SetParamValue(1, RsWebQuery.Get("ID"));
                                if (nq.Execute() != 1) throw new Exception("Could not update binary");
                                IMTransaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                            MessageBox.Show("Query saved successfull!", "Warning!");
                        }

                    }
                    finally
                    {
                        if (RsWebQuery.IsOpen())
                            RsWebQuery.Close();
                        RsWebQuery.Destroy();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private bool CheckNameWebQuery(string Name, int ID_, string TypeS)
        {
            bool isContain = false;
            IMRecordset RsWebQueryNew = new IMRecordset(ICSMTbl.WebQuery, IMRecordset.Mode.ReadOnly);
            RsWebQueryNew.Select("ID,Name,Status");
            RsWebQueryNew.SetWhere("Name", IMRecordset.Operation.Eq, Name);
            RsWebQueryNew.SetWhere("Status", IMRecordset.Operation.Eq, TypeS);
            if (ID > -1) RsWebQueryNew.SetAdditional(string.Format("[ID] <> {0}", ID_));
            for (RsWebQueryNew.Open(); !RsWebQueryNew.IsEOF(); RsWebQueryNew.MoveNext()) {
                isContain = true;
            }
            if (RsWebQueryNew.IsOpen())
                RsWebQueryNew.Close();
            RsWebQueryNew.Destroy();
            return isContain;
        }



        private void ButtonSaveAllChange_Click(object sender, EventArgs e)
        {
            if (isNew){
                if (!string.IsNullOrEmpty(textBoxName.Text)) {
                    if (CheckNameWebQuery(textBoxName.Text.Replace("(", "[").Replace(")", "]"), ID, comboBoxLevelAccess.Text)) {
                        MessageBox.Show("Dublicate Name. Please input another Name!");
                        return;
                    }
                    YXwebquery RsWebQueryNew = new YXwebquery();
                    RsWebQueryNew.Format("*");
                        RsWebQueryNew.New();
                        int IDs = RsWebQueryNew.AllocID();
                        RsWebQueryNew.m_id = IDs;
                        RsWebQueryNew.m_name = textBoxName.Text.Replace("(", "[").Replace(")", "]");
                        RsWebQueryNew.m_status= comboBoxLevelAccess.Text;
                        RsWebQueryNew.m_rightgroup = comboBox_group.Text;
                        RsWebQueryNew.m_identuser = txtUserIdent.Text;
                        RsWebQueryNew.m_comments = textBoxDescrQuery.Text;
                        if (checkBoxSQLmode.Checked)
                            RsWebQueryNew.m_issqlrequest = 1;
                        else
                           RsWebQueryNew.m_issqlrequest = 0;
                        RsWebQueryNew.Save();
                        RsWebQueryNew.Close();
                        RsWebQueryNew.Dispose();


                        byte[] cipherText = CryptorEngine.Encrypt(textBoxQuery.Text.ToString(), true);
                        string sql; ANetDb d; ANetNQ nq = null;
                        IMTransaction.Begin();
                        OrmCs.OrmSchema.Linker.PrepareExecute("UPDATE %XWebQuery SET Query=@p0 WHERE ID=@p1", out sql, out d);
                        nq = d.NewAdapter();
                        nq.Init(sql, "binary,int");
                        nq.SetParamValue(0, cipherText);
                        nq.SetParamValue(1, IDs);
                        if (nq.Execute() != 1) throw new Exception("Could not update binary");
                        IMTransaction.Commit();
                        MessageBox.Show("Record created successfull!", "Warning!");
                }
                else MessageBox.Show("Please input data to field 'Name query'!", "Warning!");
            }
            else  {

                YXwebquery RsWebQuery = new YXwebquery();
                RsWebQuery.Format("*");
                    if (RsWebQuery.Fetch(ID)) {
                        RsWebQuery.m_name = textBoxName.Text.Replace("(", "[").Replace(")", "]");
                        RsWebQuery.m_status = comboBoxLevelAccess.Text;
                        RsWebQuery.m_rightgroup = comboBox_group.Text;
                        RsWebQuery.m_identuser = txtUserIdent.Text;
                        RsWebQuery.m_comments = textBoxDescrQuery.Text;
                        if (checkBoxSQLmode.Checked)
                            RsWebQuery.m_issqlrequest = 1;
                        else
                            RsWebQuery.m_issqlrequest = 0;
                        RsWebQuery.Save();
                        RsWebQuery.Close();
                        RsWebQuery.Dispose();

                        byte[] cipherText = CryptorEngine.Encrypt(textBoxQuery.Text.ToString(), true);
                        string sql; ANetDb d; ANetNQ nq = null;
                        IMTransaction.Begin();
                        OrmCs.OrmSchema.Linker.PrepareExecute("UPDATE %XWebQuery SET Query=@p0 WHERE Id=@p1", out sql, out d);
                        nq = d.NewAdapter();
                        nq.Init(sql, "binary,int");
                        nq.SetParamValue(0, cipherText);
                        nq.SetParamValue(1, ID);
                        if (nq.Execute() != 1) throw new Exception("Could not update binary");
                        IMTransaction.Commit();
                        MessageBox.Show("Record saved successfull!", "Warning!");
                    }
            }
            Close();
        }

    

        private void button_Constraints_Click(object sender, EventArgs e)
        {
            if (ID != IM.NullI)
            {
                List<string> List_Path = ClassORM.GetProperties(textBoxQuery.Text,false,checkBoxSQLmode.Checked);
                FormWebConstrainsList constraintForm = new FormWebConstrainsList(ID, List_Path);
                constraintForm.ShowDialog();
            }
        }

        private void checkBoxSQLmode_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxSQLmode.Checked)
                button_Constraints.Enabled = false;
            else
                button_Constraints.Enabled = true;
        }
    }
}
