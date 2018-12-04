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
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;

namespace XICSM.Atdi.Icsm.Plugins.WebQuery
{
    public partial class AdminFormWebQuery : Form
    {
        public bool isNew { get; set; }
        public IMQueryMenuNode.Context _lst { get; set; }
        public int id { get; set; }
        public string query { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public AdminFormWebQuery(IMQueryMenuNode.Context Lst)
        {
            InitializeComponent();
            id = -1;
            Init(Lst);
            _lst = Lst;
            CLocaliz.TxT(this);

            if (id==-1)
            {
                button_Constraints.Enabled = false;
            }
            else
            {
                button_Constraints.Enabled = true;
            }
        }

        public List<string> GetAllTaskForceShortName()
        {
            var AllTaskForces = new List<string>();
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
            var rsWebQuery = new IMRecordset(Lst.TableName, IMRecordset.Mode.ReadOnly);
            rsWebQuery.Select("ID,NAME,QUERY,COMMENTS,IDENTUSER,CODE,TASKFORCEGROUP,VIEWCOLUMNS,ADDCOLUMNS,EDITCOLUMNS,TABLECOLUMNS");
            if (Lst.DataList == null) rsWebQuery.SetWhere("ID", IMRecordset.Operation.Eq, Lst.TableId);
            rsWebQuery.AddSelectionFrom(Lst.DataList, IMRecordset.WhereCopyOptions.SelectedLines);
            for (rsWebQuery.Open(); !rsWebQuery.IsEOF(); rsWebQuery.MoveNext())
            {
                textBoxName.Text = rsWebQuery.GetS("NAME");
                textBoxDescrQuery.Text = rsWebQuery.GetS("COMMENTS");
                ViewFormColumns.Text = rsWebQuery.GetS("VIEWCOLUMNS");
                AddFormColumns.Text = rsWebQuery.GetS("ADDCOLUMNS");
                EditFormColumns.Text = rsWebQuery.GetS("EDITCOLUMNS");
                TableColumns.Text = rsWebQuery.GetS("TABLECOLUMNS");

                byte[] zip = null;
                string sql; ANetDb d;
                OrmCs.OrmSchema.Linker.PrepareExecute("SELECT QUERY FROM %XWEBQUERY WHERE ID=" + rsWebQuery.GetI("ID"), out sql, out d);
                ANetRs Anet_rs = d.NewRecordset();
                Anet_rs.Open(sql);
                if (!Anet_rs.IsEOF())
                    zip = Anet_rs.GetBinary(0);
                Anet_rs.Destroy();
                if (zip != null)
                {
                    string cipherText = UTF8Encoding.UTF8.GetString(zip);
                    textBoxQuery.Text = cipherText;
                    query = cipherText;
                }
                txtUserIdent.Items.Clear();
                List<string> List_Path = ClassORM.GetProperties(query, true);
                txtUserIdent.Items.Clear();
                txtUserIdent.Items.Add("");
                foreach (string item in List_Path)
                    txtUserIdent.Items.Add(item);

                if (txtUserIdent.Items.IndexOf(rsWebQuery.GetS("IDENTUSER").ToString()) != -1) txtUserIdent.SelectedIndex = txtUserIdent.Items.IndexOf(rsWebQuery.GetS("IDENTUSER").ToString());
                textBox_code.Text = rsWebQuery.GetS("CODE");
                int idx = comboBox_group.FindString(rsWebQuery.GetS("TASKFORCEGROUP"));
                if (idx > -1) comboBox_group.SelectedIndex = idx;
                id = rsWebQuery.GetI("ID");
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
            Regex reGuid = new Regex(@"^[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}$",  RegexOptions.Compiled);
            string AllText = "";
            openFileDialog1.DefaultExt = "IRP";
            openFileDialog1.AddExtension = true;
            DialogResult DA = openFileDialog1.ShowDialog();
            if (DA == System.Windows.Forms.DialogResult.OK) {
                textBoxIRPFilePath.Text = openFileDialog1.FileName;
                using (StreamReader rdr = new StreamReader(textBoxIRPFilePath.Text, Encoding.Default))
                {
                    string cipherText = rdr.ReadToEnd();
                    string[] wordString = cipherText.Split(new char[] { '\n' });
                    for (int i = 0; i < wordString.Count(); i++)
                    {
                        if (wordString[i].Contains("PATH") && (wordString[i].Contains("CustomExpression")) && (!reGuid.IsMatch(wordString[i])))
                        {
                            wordString[i] = wordString[i].Replace("CustomExpression", "CustomExpression_" + Guid.NewGuid().ToString());
                        }
                        AllText += wordString[i] + "\n";
                    }
                    textBoxQuery.Text = AllText;
                }
            }
        }

        private void ButtonSaveChangeQuery_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxQuery.Text.ToString()))
            {
                MessageBox.Show("Відстуня інформація про запит!");
                return;
            }
            DialogResult DA = MessageBox.Show("Editable query manually may cause it to malfunction on the side of the Web portal to continue?", "Warning!", MessageBoxButtons.YesNo);
            if (DA == System.Windows.Forms.DialogResult.Yes){
                {
                    var rsWebQuery = new IMRecordset(ICSMTbl.WebQuery, IMRecordset.Mode.ReadWrite);
                    rsWebQuery.Select("ID,NAME,QUERY,COMMENTS,IDENTUSER,CODE,TASKFORCEGROUP,VIEWCOLUMNS,ADDCOLUMNS,EDITCOLUMNS,TABLECOLUMN");
                    if (_lst.DataList == null) rsWebQuery.SetWhere("ID", IMRecordset.Operation.Eq, _lst.TableId);
                    else rsWebQuery.AddSelectionFrom(_lst.DataList, IMRecordset.WhereCopyOptions.SelectedLines);
                    try {
                        rsWebQuery.Open();
                        if (!rsWebQuery.IsEOF()) {
                            try {
                                query = textBoxQuery.Text.ToString();
                                byte[] cipherText = UTF8Encoding.UTF8.GetBytes(textBoxQuery.Text.ToString());
                                string sql; ANetDb d; ANetNQ nq = null;
                                IMTransaction.Begin();
                                OrmCs.OrmSchema.Linker.PrepareExecute("UPDATE %XWEBQUERY SET QUERY=@p0 WHERE ID=@p1", out sql, out d);
                                nq = d.NewAdapter();
                                nq.Init(sql, "binary,int");
                                nq.SetParamValue(0, cipherText);
                                nq.SetParamValue(1, rsWebQuery.Get("ID"));
                                if (nq.Execute() != 1) throw new Exception("Could not update binary");
                                IMTransaction.Commit();
                                if (txtUserIdent.Items.Count == 0)
                                {
                                    txtUserIdent.Items.Clear();
                                    txtUserIdent.Items.Add("");
                                    List<string> List_Path = ClassORM.GetProperties(query, true);
                                    foreach (string item in List_Path)
                                        txtUserIdent.Items.Add(item);
                                }
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
                        if (rsWebQuery.IsOpen())
                            rsWebQuery.Close();
                        rsWebQuery.Destroy();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private bool CheckNameWebQuery(string Name, int ID_, string Code)
        {
            bool isContain = false;
            var rsWebQueryNew = new IMRecordset(ICSMTbl.WebQuery, IMRecordset.Mode.ReadOnly);
            rsWebQueryNew.Select("ID,NAME,CODE");
            rsWebQueryNew.SetWhere("NAME", IMRecordset.Operation.Eq, Name);
            rsWebQueryNew.SetWhere("CODE", IMRecordset.Operation.Eq, Code);
            if (id > -1) rsWebQueryNew.SetAdditional(string.Format("[ID] <> {0}", ID_));
            for (rsWebQueryNew.Open(); !rsWebQueryNew.IsEOF(); rsWebQueryNew.MoveNext()) {
                isContain = true;
            }
            if (rsWebQueryNew.IsOpen())
                rsWebQueryNew.Close();
            rsWebQueryNew.Destroy();
            return isContain;
        }



        private void ButtonSaveAllChange_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxQuery.Text.ToString()))
            {
                MessageBox.Show("Відстуня інформація про запит!");
                return;
            }

            if (string.IsNullOrEmpty(textBox_code.Text.ToString()))
            {
                MessageBox.Show("Відстуня інформація про код запиту!");
                return;
            }

            if (isNew){
                if (!string.IsNullOrEmpty(textBoxName.Text)) {
                    if (CheckNameWebQuery(textBoxName.Text.Replace("(", "[").Replace(")", "]"), id, textBox_code.Text)) {
                        MessageBox.Show("Dublicate Name. Please input another Name!");
                        return;
                    }
                        IMRecordset rsWebQueryNew = new IMRecordset(ICSMTbl.WebQuery, IMRecordset.Mode.ReadWrite);
                        rsWebQueryNew.Select("ID,NAME,CODE,TASKFORCEGROUP,IDENTUSER,COMMENTS,VIEWCOLUMNS,ADDCOLUMNS,EDITCOLUMNS,TABLECOLUMNS");
                    rsWebQueryNew.SetWhere("ID", IMRecordset.Operation.Eq, -1);
                    rsWebQueryNew.Open();
                    if (rsWebQueryNew.IsEOF())
                    {
                        rsWebQueryNew.AddNew();
                        int IDs = IM.AllocID(ICSMTbl.WebQuery, 1, -1);
                        rsWebQueryNew.Put("ID", IDs);
                        rsWebQueryNew.Put("NAME", textBoxName.Text.Replace("(", "[").Replace(")", "]"));
                        rsWebQueryNew.Put("CODE", textBox_code.Text);
                        rsWebQueryNew.Put("TASKFORCEGROUP", comboBox_group.Text);
                        rsWebQueryNew.Put("IDENTUSER", txtUserIdent.Text);
                        rsWebQueryNew.Put("COMMENTS", textBoxDescrQuery.Text);

                        rsWebQueryNew.Put("VIEWCOLUMNS", ViewFormColumns.Text);
                        rsWebQueryNew.Put("ADDCOLUMNS", AddFormColumns.Text);
                        rsWebQueryNew.Put("EDITCOLUMNS", EditFormColumns.Text);
                        rsWebQueryNew.Put("TABLECOLUMNS", TableColumns.Text);


                        rsWebQueryNew.Update();
                        rsWebQueryNew.Close();
                        rsWebQueryNew.Destroy();
                        query = textBoxQuery.Text.ToString();
                        byte[] cipherText = UTF8Encoding.UTF8.GetBytes(textBoxQuery.Text.ToString());
                        string sql; ANetDb d; ANetNQ nq = null;
                        IMTransaction.Begin();
                        OrmCs.OrmSchema.Linker.PrepareExecute("UPDATE %XWEBQUERY SET QUERY=@p0 WHERE ID=@p1", out sql, out d);
                        nq = d.NewAdapter();
                        nq.Init(sql, "binary,int");
                        nq.SetParamValue(0, cipherText);
                        nq.SetParamValue(1, IDs);
                        if (nq.Execute() != 1) throw new Exception("Could not update binary");
                        IMTransaction.Commit();
                        if (txtUserIdent.Items.Count == 0)
                        {
                            txtUserIdent.Items.Clear();
                            txtUserIdent.Items.Add("");
                            List<string> List_Path = ClassORM.GetProperties(query, true);
                            foreach (string item in List_Path)
                                txtUserIdent.Items.Add(item);
                        }
                        MessageBox.Show("Record created successfull!", "Warning!");
                        id = IDs;
                        button_Constraints.Enabled = true;
                        isNew = false;
                    }
                    
                }
                else MessageBox.Show("Please input data to field 'Name query'!", "Warning!");
            }
            else  {
                IMRecordset rsWebQueryNew = new IMRecordset(ICSMTbl.WebQuery, IMRecordset.Mode.ReadWrite);
                rsWebQueryNew.Select("ID,NAME,CODE,TASKFORCEGROUP,IDENTUSER,COMMENTS,VIEWCOLUMNS,ADDCOLUMNS,EDITCOLUMNS,TABLECOLUMNS");
                rsWebQueryNew.SetWhere("ID", IMRecordset.Operation.Eq, id);
                rsWebQueryNew.Open();
                if (!rsWebQueryNew.IsEOF())
                    {
                    rsWebQueryNew.Edit();
                    rsWebQueryNew.Put("NAME", textBoxName.Text.Replace("(", "[").Replace(")", "]"));
                    rsWebQueryNew.Put("CODE", textBox_code.Text);
                    rsWebQueryNew.Put("TASKFORCEGROUP", comboBox_group.Text);
                    rsWebQueryNew.Put("IDENTUSER", txtUserIdent.Text);
                    rsWebQueryNew.Put("COMMENTS", textBoxDescrQuery.Text);

                    rsWebQueryNew.Put("VIEWCOLUMNS", ViewFormColumns.Text);
                    rsWebQueryNew.Put("ADDCOLUMNS", AddFormColumns.Text);
                    rsWebQueryNew.Put("EDITCOLUMNS", EditFormColumns.Text);
                    rsWebQueryNew.Put("TABLECOLUMNS", TableColumns.Text);

                    rsWebQueryNew.Update();
                    rsWebQueryNew.Close();
                    rsWebQueryNew.Destroy();
                    query = textBoxQuery.Text.ToString();
                        byte[] cipherText = UTF8Encoding.UTF8.GetBytes(textBoxQuery.Text.ToString());
                        string sql; ANetDb d; ANetNQ nq = null;
                        IMTransaction.Begin();
                        OrmCs.OrmSchema.Linker.PrepareExecute("UPDATE %XWEBQUERY SET QUERY=@p0 WHERE ID=@p1", out sql, out d);
                        nq = d.NewAdapter();
                        nq.Init(sql, "binary,int");
                        nq.SetParamValue(0, cipherText);
                        nq.SetParamValue(1, id);
                        if (nq.Execute() != 1) throw new Exception("Could not update binary");
                        IMTransaction.Commit();
                        MessageBox.Show("Record saved successfull!", "Warning!");
                       button_Constraints.Enabled = true;
                }
            }
          
        }

    

        private void button_Constraints_Click(object sender, EventArgs e)
        {
            if (id != IM.NullI)
            {
                List<string> List_Path = ClassORM.GetProperties(textBoxQuery.Text.ToString(), false);
                FormWebConstrainsList constraintForm = new FormWebConstrainsList(id, List_Path);
                constraintForm.ShowDialog();
            }
        }

        private void ColumnAttr_Click(object sender, EventArgs e)
        {
            if (id != IM.NullI)
            {
                List<string> List_Path = ClassORM.GetProperties(textBoxQuery.Text.ToString(), false);
                FormColumnAttributesList constraintForm = new FormColumnAttributesList(id, List_Path);
                constraintForm.ShowDialog();
            }
        }

        private void BtnOrders_Click(object sender, EventArgs e)
        {
            if (id != IM.NullI)
            {
                List<string> List_Path = ClassORM.GetProperties(textBoxQuery.Text.ToString(), false);
                FormColumnOrdersList constraintForm = new FormColumnOrdersList(id, List_Path);
                constraintForm.ShowDialog();
            }
        }
    }
}
