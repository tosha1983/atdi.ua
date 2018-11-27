﻿using System;
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
    public partial class FormEditColumnAttributes : Form
    {
        public int id { get; set; }
        public bool IsNew { get; set; }
        public int web_id { get; set; }

        public FormEditColumnAttributes(int web_contraint, bool isnew, List<string> L_Path)
        {
            InitializeComponent();
            id = web_contraint;
            IsNew = isnew;

            textBox_name.Text = "";
            ReadOnly.Checked = false;
            NotChangeableByAdd.Checked = false;
            NotChangeableByEdit.Checked = false;

            if (id != IM.NullI)
            {
                web_id = id;
                if (!IsNew)
                {
                    var rsWebQueryNew = new IMRecordset(ICSMTbl.WebQueryAtttributes, IMRecordset.Mode.ReadWrite);
                    rsWebQueryNew.Select("ID,WEBQUERYID,PATH,READONLY,NOTCHANGEADD,NOTCHANGEEDIT");
                    rsWebQueryNew.SetWhere("ID", IMRecordset.Operation.Eq, id);
                    rsWebQueryNew.Open();
                    if (!rsWebQueryNew.IsEOF())
                    {
                        textBox_name.Text = rsWebQueryNew.GetS("PATH");
                        web_id = rsWebQueryNew.GetI("WEBQUERYID");
                        if (rsWebQueryNew.GetI("READONLY") == 1) { ReadOnly.Checked = true; } else { ReadOnly.Checked = false; }
                        if (rsWebQueryNew.GetI("NOTCHANGEADD") == 1) { NotChangeableByAdd.Checked = true; } else { NotChangeableByAdd.Checked = false; }
                        if (rsWebQueryNew.GetI("NOTCHANGEEDIT") == 1) { NotChangeableByEdit.Checked = true; } else { NotChangeableByEdit.Checked = false; }
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
            if (textBox_name.Text == "")
            {
                MessageBox.Show("Please input column name!");
                return;
            }
            byte[] zip = null;
            string sql; ANetDb d;
            OrmCs.OrmSchema.Linker.PrepareExecute("SELECT QUERY FROM %XWEBQUERY WHERE ID=" + web_id, out sql, out d);
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
                        if (ClassORM.CheckField(Lod[Lod.Count-1].FieldJoinTo, Lod[Lod.Count - 1].NameTableTo))
                        {
                            string[] txt = textBox_name.Text.Split(new char[] { '.' });
                            if (txt != null)
                            {
                                if (txt.Length>0)
                                {
                                    if (Lod[Lod.Count - 1].FieldJoinTo != txt[txt.Length-1])
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

                var rsWebQueryNew = new IMRecordset(ICSMTbl.WebQueryAtttributes, IMRecordset.Mode.ReadWrite);
                rsWebQueryNew.Select("ID,WEBQUERYID,PATH,READONLY,NOTCHANGEADD,NOTCHANGEEDIT");
                if (id != IM.NullI)
                {
                    if (!IsNew)
                    {
                        rsWebQueryNew.SetWhere("ID", IMRecordset.Operation.Eq, id);
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
                        rsWebQueryNew.Put("WEBQUERYID", id);
                        rsWebQueryNew.Put("PATH", textBox_name.Text);
                        if (ReadOnly.Checked) { rsWebQueryNew.Put("READONLY", 1); } else { rsWebQueryNew.Put("READONLY", 0); }
                        if (NotChangeableByAdd.Checked) { rsWebQueryNew.Put("NOTCHANGEADD", 1); } else { rsWebQueryNew.Put("NOTCHANGEADD", 0); }
                        if (NotChangeableByEdit.Checked) { rsWebQueryNew.Put("NOTCHANGEEDIT", 1); } else { rsWebQueryNew.Put("NOTCHANGEEDIT", 0); }
                        rsWebQueryNew.Update();
                    }
                    else
                    {
                        if (!IsNew)
                        {
                            rsWebQueryNew.Edit();
                            rsWebQueryNew.Put("PATH", textBox_name.Text);
                            if (ReadOnly.Checked) { rsWebQueryNew.Put("READONLY", 1); } else { rsWebQueryNew.Put("READONLY", 0); }
                            if (NotChangeableByAdd.Checked) { rsWebQueryNew.Put("NOTCHANGEADD", 1); } else { rsWebQueryNew.Put("NOTCHANGEADD", 0); }
                            if (NotChangeableByEdit.Checked) { rsWebQueryNew.Put("NOTCHANGEEDIT", 1); } else { rsWebQueryNew.Put("NOTCHANGEEDIT", 0); }
                            rsWebQueryNew.Update();
                        }
                        else
                        {
                            rsWebQueryNew.AddNew();
                            int idnew = IM.AllocID(ICSMTbl.WebConstraint, 1, -1);
                            rsWebQueryNew.Put("ID", idnew);
                            rsWebQueryNew.Put("WEBQUERYID", id);
                            rsWebQueryNew.Put("PATH", textBox_name.Text);
                            if (ReadOnly.Checked) { rsWebQueryNew.Put("READONLY", 1); } else { rsWebQueryNew.Put("READONLY", 0); }
                            if (NotChangeableByAdd.Checked) { rsWebQueryNew.Put("NOTCHANGEADD", 1); } else { rsWebQueryNew.Put("NOTCHANGEADD", 0); }
                            if (NotChangeableByEdit.Checked) { rsWebQueryNew.Put("NOTCHANGEEDIT", 1); } else { rsWebQueryNew.Put("NOTCHANGEEDIT", 0); }
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

        private void button_cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}
