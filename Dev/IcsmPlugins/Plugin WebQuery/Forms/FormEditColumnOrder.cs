using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ICSM;
using DatalayerCs;

namespace XICSM.Atdi.Icsm.Plugins.WebQuery
{
    public partial class FormEditColumnOrder : Form
    {
        public int _id { get; set; }
        public bool IsNew { get; set; }
        public int _web_id { get; set; }

        public FormEditColumnOrder(int web_id, int id, bool isnew, List<string> L_Path)
        {
            InitializeComponent();
            _id = id;
            _web_id = web_id;
            IsNew = isnew;

            textBox_name.Text = "";
            comboBox1.SelectedIndex = 0;

            if (_id != IM.NullI)
            {
                if (!IsNew)
                {
                    var rsWebQueryNew = new IMRecordset(ICSMTbl.WebQueryOrders, IMRecordset.Mode.ReadWrite);
                    rsWebQueryNew.Select("ID,WEBQUERYID,PATH,ORDER");
                    rsWebQueryNew.SetWhere("ID", IMRecordset.Operation.Eq, _id);
                    rsWebQueryNew.Open();
                    if (!rsWebQueryNew.IsEOF())
                    {
                        textBox_name.Text = rsWebQueryNew.GetS("PATH");
                        web_id = rsWebQueryNew.GetI("WEBQUERYID");
                        if (rsWebQueryNew.GetI("ORDER") == 1) { comboBox1.SelectedIndex = 0; } else { comboBox1.SelectedIndex = 1; }
                    }
                    if (rsWebQueryNew.IsOpen())
                        rsWebQueryNew.Close();
                    rsWebQueryNew.Destroy();
                }
            }
            CLocaliz.TxT(this);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="contr"></param>
        public void SetDisable(Control contr)
        {
            foreach (Control control in contr.Controls)
            {
                control.Enabled = false; if (control.HasChildren) SetDisable(control); 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contr"></param>
        public void SetEnable(Control contr)
        {
            foreach (Control control in contr.Controls)
            {
                 control.Enabled = true; if (control.HasChildren) SetEnable(control); 
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_ok_Click(object sender, EventArgs e)
        {
            bool isDuplicate = false;
            try
            {
                if (textBox_name.Text == "")
                {
                    MessageBox.Show("Please input column name!");
                    return;
                }
                byte[] zip = null;
                string sql; ANetDb d;
                OrmCs.OrmSchema.Linker.PrepareExecute("SELECT QUERY FROM %XWEBQUERY WHERE ID=" + _web_id, out sql, out d);
                ANetRs Anet_rs = d.NewRecordset();
                Anet_rs.Open(sql);
                if (!Anet_rs.IsEOF())
                    zip = Anet_rs.GetBinary(0);
                Anet_rs.Destroy();
                if (zip != null)
                {
                    string cipherText = UTF8Encoding.UTF8.GetString(zip);
                    List<RecordPtrDB> Lod = ClassORM.GetLinkData(ClassORM.GetTableName(cipherText), textBox_name.Text);
                    if (Lod != null)
                    {
                        if (Lod.Count > 0)
                        {
                            if (Lod.Count == 1)
                            {
                                Lod[Lod.Count - 1].FieldJoinTo = Lod[Lod.Count - 1].LinkField;
                            }
                            if (ClassORM.CheckField(Lod[Lod.Count - 1].FieldJoinTo, Lod[Lod.Count - 1].NameTableTo))
                            {
                                string[] txt = textBox_name.Text.Split(new char[] { '.', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                if (txt != null)
                                {
                                    if (txt.Length > 0)
                                    {
                                        if (Lod[Lod.Count - 1].FieldJoinTo != txt[txt.Length - 1])
                                        {
                                            MessageBox.Show("Column name is wrong!");
                                            return;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Column name is wrong!");
                                return;
                            }
                        }
                    }
                }


                if (!isDuplicate)
                {

                    var rsWebQueryNew = new IMRecordset(ICSMTbl.WebQueryOrders, IMRecordset.Mode.ReadWrite);
                    rsWebQueryNew.Select("ID,WEBQUERYID,PATH,ORDER");
                    if (_id != IM.NullI)
                    {
                        if (!IsNew)
                        {
                            rsWebQueryNew.SetWhere("ID", IMRecordset.Operation.Eq, _id);
                        }
                    }

                    try
                    {
                        rsWebQueryNew.Open();
                        if (rsWebQueryNew.IsEOF())
                        {
                            rsWebQueryNew.AddNew();
                            int idnew = IM.AllocID(ICSMTbl.WebConstraint, 1, -1);
                            rsWebQueryNew.Put("ID", idnew);
                            rsWebQueryNew.Put("WEBQUERYID", _web_id);
                            rsWebQueryNew.Put("PATH", textBox_name.Text);
                            if (comboBox1.SelectedIndex == 0) { rsWebQueryNew.Put("ORDER", 1); } else { rsWebQueryNew.Put("ORDER", 2); }
                            rsWebQueryNew.Update();
                        }
                        else
                        {
                            if (!IsNew)
                            {
                                rsWebQueryNew.Edit();
                                rsWebQueryNew.Put("PATH", textBox_name.Text);
                                if (comboBox1.SelectedIndex == 0) { rsWebQueryNew.Put("ORDER", 1); } else { rsWebQueryNew.Put("ORDER", 2); }
                                rsWebQueryNew.Update();
                            }
                            else
                            {
                                rsWebQueryNew.AddNew();
                                int idnew = IM.AllocID(ICSMTbl.WebConstraint, 1, -1);
                                rsWebQueryNew.Put("ID", idnew);
                                rsWebQueryNew.Put("WEBQUERYID", _web_id);
                                rsWebQueryNew.Put("PATH", textBox_name.Text);
                                if (comboBox1.SelectedIndex == 0) { rsWebQueryNew.Put("ORDER", 1); } else { rsWebQueryNew.Put("ORDER", 2); }
                                rsWebQueryNew.Update();
                            }

                        }

                    }
                    finally
                    {
                        if (rsWebQueryNew.IsOpen())
                            rsWebQueryNew.Close();
                        rsWebQueryNew.Destroy();
                    }
                    MessageBox.Show("Record created successfull!", "Warning!"); Close();

                }
                else { MessageBox.Show("Record is already exists in table 'XWEBCONSTRAINT'", "Warning!"); Close(); }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}
