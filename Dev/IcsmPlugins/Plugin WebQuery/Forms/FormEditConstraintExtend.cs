using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ICSM;

namespace XICSM.WebQuery
{
    public partial class FormEditConstraintExtend : Form
    {
        public int ID { get; set; }
        public bool IsNew { get; set; }

        public FormEditConstraintExtend(int web_contraint, bool isnew, List<string> L_Path)
        {
            InitializeComponent();
            ID = web_contraint;
            IsNew = isnew;

            comboBox_path.Items.Clear();
            comboBox_path.AutoCompleteMode = AutoCompleteMode.None;
            comboBox_path.DrawMode = DrawMode.OwnerDrawFixed;

            if (L_Path == null)
            {
                if (ID != IM.NullI)
                {
                    if (!IsNew)
                    {
                        IMRecordset RsWebQueryNew = new IMRecordset(ICSMTbl.WebConstraint, IMRecordset.Mode.ReadWrite);
                        RsWebQueryNew.Select("ID,WebQueryId,Name,Path,Min,Max,StrValue,DateValueMin,DateValueMax,Include");
                        RsWebQueryNew.SetWhere("ID", IMRecordset.Operation.Eq, ID);
                        RsWebQueryNew.Open();
                        if (!RsWebQueryNew.IsEOF())
                        {

                            IMRecordset RsWebQuery = new IMRecordset(ICSMTbl.WebQuery, IMRecordset.Mode.ReadOnly);
                            RsWebQuery.Select("ID,Query,IsSqlRequest");
                            RsWebQuery.SetWhere("ID", IMRecordset.Operation.Eq, RsWebQueryNew.GetI("WebQueryId"));
                            RsWebQuery.Open();
                            if (!RsWebQuery.IsEOF())
                            {
                                string Q = RsWebQuery.GetS("Query");
                                List<string> List_Path = ClassORM.GetProperties(Q, true, RsWebQueryNew.GetI("IsSqlRequest") ==1 ? true : false);
                                comboBox_path.Items.Clear();
                                foreach (string item in List_Path)
                                    comboBox_path.Items.Add(item);
                            }
                            if (RsWebQuery.IsOpen())
                                RsWebQuery.Close();
                            RsWebQuery.Destroy();
                        }


                        if (RsWebQueryNew.IsOpen())
                            RsWebQueryNew.Close();
                        RsWebQueryNew.Destroy();
                    }
                }
            }
            else
            {
                comboBox_path.Items.Clear();
                foreach (string item in L_Path)
                    comboBox_path.Items.Add(item);
            }


            Init();
        }

        /// <summary>
        /// 
        /// </summary>
        private void Init()
        {
            IMRecordset RsWebQueryNew = new IMRecordset(ICSMTbl.WebConstraint, IMRecordset.Mode.ReadWrite);
            RsWebQueryNew.Select("ID,WebQueryId,Name,Path,Min,Max,StrValue,DateValueMin,DateValueMax,Include");
            if (ID != IM.NullI)
            {
                if (!IsNew)
                {
                    RsWebQueryNew.SetWhere("ID", IMRecordset.Operation.Eq, ID);
                    RsWebQueryNew.Open();
                    if (!RsWebQueryNew.IsEOF())
                    {
                        textBox_name.Text = RsWebQueryNew.GetS("Name");
                        textBox_str_value.Text = RsWebQueryNew.GetS("StrValue");
                        icsDouble_from.Value = RsWebQueryNew.GetD("Min");
                        icsDouble_to.Value = RsWebQueryNew.GetD("Max");
                        icsDateTime_from.Value = RsWebQueryNew.GetT("DateValueMin");
                        icsDateTime_to.Value = RsWebQueryNew.GetT("DateValueMax");
                        if (RsWebQueryNew.GetI("Include") == 1) { checkBox_include.Checked = true; } else { checkBox_include.Checked = false; }
                        if (comboBox_path.Items.IndexOf(RsWebQueryNew.GetS("Path")) != -1) comboBox_path.SelectedIndex = comboBox_path.Items.IndexOf(RsWebQueryNew.GetS("Path"));

                        if (textBox_str_value.Text != "")
                        {
                            SetEnable(groupBox_String);
                            SetDisable(groupBox_DateTime);
                            SetDisable(groupBox_Numerical);
                        }
                        else if ((icsDouble_from.Value!=IM.NullD) || (icsDouble_to.Value!=IM.NullD))
                        {
                            SetEnable(groupBox_Numerical);
                            SetDisable(groupBox_DateTime);
                            SetDisable(groupBox_String);
                        }
                        else  if ((icsDateTime_from.Value != IM.NullT) || (icsDateTime_to.Value !=IM.NullT))
                        {
                            SetEnable(groupBox_DateTime);
                            SetDisable(groupBox_Numerical);
                            SetDisable(groupBox_String);
                        }

                    }
                }
            }
            if (RsWebQueryNew.IsOpen())
                RsWebQueryNew.Close();
            RsWebQueryNew.Destroy();
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

            if (comboBox_path.SelectedIndex == -1)
            {
                MessageBox.Show("Please select Path!");
                return;
            }

                       
            if (!isDuplicate)
            {
                 
                        IMRecordset RsWebQueryNew = new IMRecordset(ICSMTbl.WebConstraint, IMRecordset.Mode.ReadWrite);
                        RsWebQueryNew.Select("ID,WebQueryId,Name,Path,Min,Max,StrValue,DateValueMin,DateValueMax,Include");
                        if (ID != IM.NullI)
                        {
                            if (!IsNew)
                            {
                                RsWebQueryNew.SetWhere("ID", IMRecordset.Operation.Eq, ID);
                            }
                        }

                        try
                        {
                            RsWebQueryNew.Open();
                            if (RsWebQueryNew.IsEOF())
                            {
                                RsWebQueryNew.AddNew();
                                int ID_NEW = IM.AllocID(ICSMTbl.WebConstraint, 1, -1);
                                RsWebQueryNew.Put("ID", ID_NEW);
                                RsWebQueryNew.Put("WebQueryId", ID); 
                                RsWebQueryNew.Put("Name", textBox_name.Text);
                                RsWebQueryNew.Put("StrValue", textBox_str_value.Text);
                                RsWebQueryNew.Put("Min", icsDouble_from.Value!=IM.NullD ? icsDouble_from.Value : IM.NullD);
                                RsWebQueryNew.Put("Max", icsDouble_to.Value!=IM.NullD ? icsDouble_to.Value : IM.NullD);
                                RsWebQueryNew.Put("DateValueMin", icsDateTime_from.Value);
                                RsWebQueryNew.Put("DateValueMax", icsDateTime_to.Value); 
                                if (checkBox_include.Checked) { RsWebQueryNew.Put("Include", 1); }
                                if (!checkBox_include.Checked) { RsWebQueryNew.Put("Include", 0); }
                                if (comboBox_path.Items.Count > 0) { if (comboBox_path.SelectedIndex != -1) RsWebQueryNew.Put("Path", comboBox_path.Text); }
                                RsWebQueryNew.Update();
                            }
                            else
                            {
                                if (!IsNew)
                                {
                                    RsWebQueryNew.Edit();
                                    RsWebQueryNew.Put("Name", textBox_name.Text);
                                    RsWebQueryNew.Put("StrValue", textBox_str_value.Text);
                                    RsWebQueryNew.Put("Min", icsDouble_from.Value != IM.NullD ? icsDouble_from.Value : IM.NullD);
                                    RsWebQueryNew.Put("Max", icsDouble_to.Value != IM.NullD ? icsDouble_to.Value : IM.NullD);
                                    RsWebQueryNew.Put("DateValueMin", icsDateTime_from.Value);
                                    RsWebQueryNew.Put("DateValueMax", icsDateTime_to.Value);
                                    if (checkBox_include.Checked) { RsWebQueryNew.Put("Include", 1); }
                                    if (!checkBox_include.Checked) { RsWebQueryNew.Put("Include", 0); }
                                    if (comboBox_path.Items.Count > 0) { if (comboBox_path.SelectedIndex != -1) RsWebQueryNew.Put("Path", comboBox_path.Text); }
                                    RsWebQueryNew.Update();
                                }
                                else
                                {
                                    RsWebQueryNew.AddNew();
                                    int ID_NEW = IM.AllocID(ICSMTbl.WebConstraint, 1, -1);
                                    RsWebQueryNew.Put("ID", ID_NEW);
                                    RsWebQueryNew.Put("WebQueryId", ID);
                                    RsWebQueryNew.Put("Name", textBox_name.Text);
                                    RsWebQueryNew.Put("StrValue", textBox_str_value.Text);
                                    RsWebQueryNew.Put("Min", icsDouble_from.Value != IM.NullD ? icsDouble_from.Value : IM.NullD);
                                    RsWebQueryNew.Put("Max", icsDouble_to.Value != IM.NullD ? icsDouble_to.Value : IM.NullD);
                                    RsWebQueryNew.Put("DateValueMin", icsDateTime_from.Value);
                                    RsWebQueryNew.Put("DateValueMin", icsDateTime_to.Value);
                                    if (checkBox_include.Checked) { RsWebQueryNew.Put("Include", 1); }
                                    if (!checkBox_include.Checked) { RsWebQueryNew.Put("Include", 0); }
                                    if (comboBox_path.Items.Count > 0) { if (comboBox_path.SelectedIndex != -1) RsWebQueryNew.Put("Path", comboBox_path.Text); }
                                    RsWebQueryNew.Update();
                                }

                            }

                        }
                        finally
                        {
                            if (RsWebQueryNew.IsOpen())
                                RsWebQueryNew.Close();
                            RsWebQueryNew.Destroy();
                        }
                        MessageBox.Show("Record created successfull!", "Warning!"); Close();
                    
            }
            else { MessageBox.Show("Record is already exists in table 'XWEBCONSTRAINT'", "Warning!"); Close(); }

        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void icsDouble_from_ValueChanged(object sender, EventArgs e)
        {
            SetDisable(groupBox_String);
            SetDisable(groupBox_DateTime);
            if (icsDouble_from.Value > icsDouble_to.Value)
            {
                if ((icsDouble_from.Value != IM.NullD) && (icsDouble_to.Value != IM.NullD))
                {
                    MessageBox.Show("'From' value > 'To' Value");
                    icsDouble_from.Value = IM.NullD;
                }
            }
        }

        private void icsDouble_to_ValueChanged(object sender, EventArgs e)
        {
            SetDisable(groupBox_String);
            SetDisable(groupBox_DateTime);
            if (icsDouble_from.Value > icsDouble_to.Value)
            {
                if ((icsDouble_from.Value != IM.NullD) && (icsDouble_to.Value != IM.NullD))
                {
                    MessageBox.Show("'From' value > 'To' Value");
                    icsDouble_to.Value = IM.NullD;
                }
            }
        }

        private void icsDateTime_from_ValueChanged(object sender, EventArgs e)
        {
            SetDisable(groupBox_String);
            SetDisable(groupBox_Numerical);
            if (icsDateTime_from.Value > icsDateTime_to.Value)
            {
                if ((icsDateTime_from.Value != IM.NullT) &&  (icsDateTime_to.Value != IM.NullT))
                {
                    MessageBox.Show("'From' value > 'To' Value");
                    icsDateTime_from.Value = IM.NullT;
                }
            }
        }

        private void icsDateTime_to_ValueChanged(object sender, EventArgs e)
        {
            SetDisable(groupBox_String);
            SetDisable(groupBox_Numerical);
            if (icsDateTime_from.Value > icsDateTime_to.Value)
            {
                if ((icsDateTime_from.Value != IM.NullT) && (icsDateTime_to.Value != IM.NullT))
                {
                    MessageBox.Show("'From' value > 'To' Value");
                    icsDateTime_from.Value = IM.NullT;
                }
            }
        }

        private void comboBox_path_DropDown(object sender, EventArgs e)
        {

        }

        private void comboBox_path_DropDownClosed(object sender, EventArgs e)
        {
            toolTip1.Hide(this);
        }

        private void comboBox_path_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) { return; } 
            string text = comboBox_path.GetItemText(comboBox_path.Items[e.Index]);
            e.DrawBackground();
            using (SolidBrush br = new SolidBrush(e.ForeColor))
            { e.Graphics.DrawString(text, e.Font, br, e.Bounds); }
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            { toolTip1.Show(text, comboBox_path, e.Bounds.Right, e.Bounds.Bottom); }
            e.DrawFocusRectangle();
        }

        private void textBox_str_value_TextChanged(object sender, EventArgs e)
        {
            SetDisable(groupBox_DateTime);
            SetDisable(groupBox_Numerical);
        }
    }
}
