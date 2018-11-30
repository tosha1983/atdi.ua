using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ICSM;
using NetPlugins2;
using DatalayerCs;
using Atdi.DataModels.DataConstraint;

namespace XICSM.Atdi.Icsm.Plugins.WebQuery
{
    public partial class FormEditConstraints : Form
    {
        private IcsDateTime icsDateTimeFrom { get; set; }
        private IcsDateTime icsDateTimeTo { get; set; }
        private IcsDouble icsDoubleFrom { get; set; }
        private IcsDouble icsDoubleTo { get; set; }
        private IcsInteger icsIntegerFrom { get; set; }
        private IcsInteger icsIntegerTo { get; set; }
        public int _id { get; set; }
        public int _web_id { get; set; }
        public bool IsNew { get; set; }

        public FormEditConstraints(int id, int web_id, bool isnew, List<string> L_Path)
        {
            InitializeComponent();
            icsDateTimeFrom = new IcsDateTime();
            icsDateTimeTo = new IcsDateTime();
            icsDoubleFrom = new IcsDouble();
            icsDoubleTo = new IcsDouble();
            icsIntegerFrom = new IcsInteger();
            icsIntegerTo = new IcsInteger();
            this.groupBox2.Controls.Add(icsDateTimeFrom);
            this.groupBox2.Controls.Add(icsDateTimeTo);
            this.groupBox2.Controls.Add(icsDoubleFrom);
            this.groupBox2.Controls.Add(icsDoubleTo);
            this.groupBox2.Controls.Add(icsIntegerFrom);
            this.groupBox2.Controls.Add(icsIntegerTo);
            icsDateTimeTo.Visible = false;
            icsDoubleTo.Visible = false;
            icsIntegerTo.Visible = false;
            textBoxToValue.Visible = false;
            icsDateTimeFrom.Visible = false;
            icsDoubleFrom.Visible = false;
            icsIntegerFrom.Visible = false;
            textBox_FromValue.Visible = false;
            _id = id;
            _web_id = web_id;
            IsNew = isnew;
            foreach (var e in Enum.GetValues(typeof(ConditionOperator)))
            {
                comboBox_OperationCondition.Items.Add(e.ToString());
            }
            comboBox_OperationCondition.SelectedIndex = 0;
            comboBox_MomentOfUse.SelectedIndex = 0;
            comboBox_TypeCondition.SelectedIndex = 0;




            if (id != IM.NullI)
            {
                if (!IsNew)
                {
                    var rsWebQueryNew = new IMRecordset(ICSMTbl.WebConstraint, IMRecordset.Mode.ReadWrite);
                    rsWebQueryNew.Select("ID,WEBQUERYID,PATH,MIN,MAX,STRVALUE,STRVALUETO,DATEVALUEMIN,INCLUDE,DATEVALUEMAX,DESCRCONDITION,TYPECONDITION,OPERCONDITION,MESSAGENOTVALID,DEFAULTVALUE,MOMENTOFUSE");
                    rsWebQueryNew.SetWhere("ID", IMRecordset.Operation.Eq, id);
                    rsWebQueryNew.Open();
                    if (!rsWebQueryNew.IsEOF())
                    {
                        OrmCs.OrmVarType ormVarType = OrmCs.OrmVarType.var_String;
                        textBox_NameField.Text = rsWebQueryNew.GetS("PATH");
                        comboBox_MomentOfUse.Text = rsWebQueryNew.GetS("MOMENTOFUSE");
                        comboBox_TypeCondition.Text = rsWebQueryNew.GetS("TYPECONDITION");
                        comboBox_OperationCondition.Text = rsWebQueryNew.GetS("OPERCONDITION");
                        if (_web_id == -1)
                        {
                            _web_id = rsWebQueryNew.GetI("WEBQUERYID");
                        }
                        if ((comboBox_OperationCondition.Items[comboBox_OperationCondition.SelectedIndex].ToString() == ConditionOperator.Between.ToString()) ||
                           (comboBox_OperationCondition.Items[comboBox_OperationCondition.SelectedIndex].ToString() == ConditionOperator.NotBetween.ToString()))
                        {
                            ormVarType = GetVarTypeFromColumn(textBox_NameField.Text);
                            label5.Visible = false;
                            label_From.Visible = true;
                            label_To.Visible = true;
                            ShowBlockTwoValue(ormVarType);
                        }
                        else
                        {
                            label5.Visible = true;
                            label_From.Visible = false;
                            label_To.Visible = false;
                            if (((comboBox_OperationCondition.Items[comboBox_OperationCondition.SelectedIndex].ToString() != ConditionOperator.In.ToString()) &&
                            (comboBox_OperationCondition.Items[comboBox_OperationCondition.SelectedIndex].ToString() != ConditionOperator.NotIn.ToString())))
                            {
                                ormVarType = GetVarTypeFromColumn(textBox_NameField.Text);
                                ShowBlockOneValue(ormVarType);
                            }
                        }

                        if (ormVarType == OrmCs.OrmVarType.var_String)
                        {
                            textBox_FromValue.Text = rsWebQueryNew.GetS("STRVALUE");
                            textBoxToValue.Text = rsWebQueryNew.GetS("STRVALUETO");
                        }
                        else if (ormVarType == OrmCs.OrmVarType.var_Int)
                        {
                            icsIntegerFrom.Value = rsWebQueryNew.GetI("MIN");
                            icsIntegerTo.Value = rsWebQueryNew.GetI("MAX");
                        }
                        else if ((ormVarType == OrmCs.OrmVarType.var_Dou) || (ormVarType == OrmCs.OrmVarType.var_Flo))
                        {
                            icsDoubleFrom.Value = rsWebQueryNew.GetD("MIN");
                            icsDoubleTo.Value = rsWebQueryNew.GetD("MAX");
                        }
                        else if (ormVarType == OrmCs.OrmVarType.var_Tim)
                        {
                            icsDateTimeFrom.Value = rsWebQueryNew.GetT("DATEVALUEMIN");
                            icsDateTimeTo.Value = rsWebQueryNew.GetT("DATEVALUEMAX");
                        }
                        textBox_DescriptionCondition.Text = rsWebQueryNew.GetS("DESCRCONDITION");
                        textBox_MessageNotValid.Text = rsWebQueryNew.GetS("MESSAGENOTVALID");
                        textBox_DefValues.Text = rsWebQueryNew.GetS("DEFAULTVALUE");
                    }
                    if (rsWebQueryNew.IsOpen())
                        rsWebQueryNew.Close();
                    rsWebQueryNew.Destroy();
                }
            }



            CLocaliz.TxT(this);
        }


        private OrmCs.OrmVarType GetVarTypeFromColumn(string NameField)
        {
            OrmCs.OrmVarType ormVarType = OrmCs.OrmVarType.var_String;
            if (_web_id != IM.NullI)
            {
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
                    List<RecordPtrDB> Lod = ClassORM.GetLinkData(ClassORM.GetTableName(cipherText), NameField);
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
                                string[] txt = NameField.Split(new char[] { '.' });
                                if (txt != null)
                                {
                                    if (txt.Length > 0)
                                    {
                                        if (Lod[Lod.Count - 1].FieldJoinTo == txt[txt.Length - 1])
                                        {
                                            ormVarType = ClassORM.GetOrmTypeField(Lod[Lod.Count - 1].FieldJoinTo, Lod[Lod.Count - 1].NameTableTo);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return ormVarType;
        }


        private void ShowBlockTwoValue(OrmCs.OrmVarType ValueType)
        {
            switch (ValueType)
            {
                case OrmCs.OrmVarType.var_String:

                    textBox_FromValue.Location = new Point(142, 126);
                    textBox_FromValue.Size = new Size(121, 20);
                    textBox_FromValue.Visible = true;
                    textBoxToValue.Location = new Point(297, 126);
                    textBoxToValue.Size = new Size(121, 20);
                    textBoxToValue.Visible = true;
                    textBoxToValue.Text = "";
                    icsIntegerFrom.Visible = false;
                    icsIntegerTo.Visible = false;
                    icsDoubleFrom.Visible = false;
                    icsDoubleTo.Visible = false;
                    icsDateTimeFrom.Visible = false;
                    icsDateTimeTo.Visible = false;
                    icsIntegerFrom.Value = IM.NullI;
                    icsIntegerTo.Value = IM.NullI;
                    icsDateTimeFrom.Value = IM.NullT;
                    icsDateTimeTo.Value = IM.NullT;
                    icsDoubleFrom.Value = IM.NullD;
                    icsDoubleTo.Value = IM.NullD;
                    break;
                case OrmCs.OrmVarType.var_Int:
                    icsIntegerFrom.Location = new Point(142, 126);
                    icsIntegerFrom.Size = new Size(121, 20);
                    icsIntegerTo.Location = new Point(297, 126);
                    icsIntegerTo.Size = new Size(121, 20);
                    icsIntegerFrom.Visible = true;
                    icsIntegerTo.Visible = true;
                    icsDoubleFrom.Visible = false;
                    icsDoubleTo.Visible = false;
                    icsDateTimeFrom.Visible = false;
                    icsDateTimeTo.Visible = false;
                    textBox_FromValue.Visible = false;
                    textBoxToValue.Visible = false;


                    textBox_FromValue.Text = "";
                    textBoxToValue.Text = "";
                    icsDateTimeFrom.Value = IM.NullT;
                    icsDateTimeTo.Value = IM.NullT;
                    icsDoubleFrom.Value = IM.NullD;
                    icsDoubleTo.Value = IM.NullD;

                    break;
                case OrmCs.OrmVarType.var_Flo:
                case OrmCs.OrmVarType.var_Dou:
                    icsDoubleFrom.Location = new Point(142, 126);
                    icsDoubleFrom.Size = new Size(121, 20);
                    icsDoubleTo.Location = new Point(297, 126);
                    icsDoubleTo.Size = new Size(121, 20);
                    icsDoubleFrom.Visible = true;
                    icsDoubleTo.Visible = true;
                    icsIntegerFrom.Visible = false;
                    icsIntegerTo.Visible = false;
                    icsDateTimeFrom.Visible = false;
                    icsDateTimeTo.Visible = false;
                    textBox_FromValue.Visible = false;
                    textBoxToValue.Visible = false;

                    textBox_FromValue.Text = "";
                    textBoxToValue.Text = "";
                    icsIntegerFrom.Value = IM.NullI;
                    icsIntegerTo.Value = IM.NullI;
                    icsDateTimeFrom.Value = IM.NullT;
                    icsDateTimeTo.Value = IM.NullT;


                    break;

                case OrmCs.OrmVarType.var_Tim:
                    icsDateTimeFrom.Location = new Point(142, 126);
                    icsDateTimeFrom.Size = new Size(121, 20);
                    icsDateTimeTo.Location = new Point(297, 126);
                    icsDateTimeTo.Size = new Size(121, 20);
                    icsDateTimeFrom.Visible = true;
                    icsDateTimeTo.Visible = true;
                    icsIntegerFrom.Visible = false;
                    icsIntegerTo.Visible = false;
                    icsDoubleFrom.Visible = false;
                    icsDoubleTo.Visible = false;
                    textBox_FromValue.Visible = false;
                    textBoxToValue.Visible = false;

                    textBox_FromValue.Text = "";
                    textBoxToValue.Text = "";
                    icsIntegerFrom.Value = IM.NullI;
                    icsIntegerTo.Value = IM.NullI;
                    icsDoubleFrom.Value = IM.NullD;
                    icsDoubleTo.Value = IM.NullD;

                    break;

                default:
                    
                break;
            }
        }

        private void ShowBlockOneValue(OrmCs.OrmVarType ValueType)
        {
            switch (ValueType)
            {
                case OrmCs.OrmVarType.var_String:
                    textBox_FromValue.Location = new Point(142, 126);
                    textBox_FromValue.Size = new Size(277, 20);
                    textBox_FromValue.Visible = true;
                    icsIntegerFrom.Visible = false;
                    icsDoubleFrom.Visible = false;
                    icsDateTimeFrom.Visible = false;

                    icsDateTimeTo.Visible = false;
                    icsDoubleTo.Visible = false;
                    icsIntegerTo.Visible = false;
                    textBoxToValue.Visible = false;


                    icsIntegerFrom.Value = IM.NullI;
                    icsDateTimeFrom.Value = IM.NullT;
                    icsDoubleFrom.Value = IM.NullD;

                    break;

                case OrmCs.OrmVarType.var_Int:
                    icsIntegerFrom.Location = new Point(142, 126);
                    icsIntegerFrom.Size = new Size(277, 20);
                    icsIntegerFrom.Visible = true;
                    icsDoubleFrom.Visible = false;
                    icsDateTimeFrom.Visible = false;
                    textBox_FromValue.Visible = false;


                    icsDateTimeTo.Visible = false;
                    icsDoubleTo.Visible = false;
                    icsIntegerTo.Visible = false;
                    textBoxToValue.Visible = false;


                    textBox_FromValue.Text = "";
                    icsDateTimeFrom.Value = IM.NullT;
                    icsDoubleFrom.Value = IM.NullD;

                    break;

                case OrmCs.OrmVarType.var_Flo:
                case OrmCs.OrmVarType.var_Dou:
                    icsDoubleFrom.Location = new Point(142, 126);
                    icsDoubleFrom.Size = new Size(277, 20);
                    icsDoubleFrom.Visible = true;
                    icsIntegerFrom.Visible = false;
                    icsDateTimeFrom.Visible = false;
                    textBox_FromValue.Visible = false;


                    icsDateTimeTo.Visible = false;
                    icsDoubleTo.Visible = false;
                    icsIntegerTo.Visible = false;
                    textBoxToValue.Visible = false;


                    textBox_FromValue.Text = "";
                    icsIntegerFrom.Value = IM.NullI;
                    icsDateTimeFrom.Value = IM.NullT;

                    break;

                case OrmCs.OrmVarType.var_Tim:
                    icsDateTimeFrom.Location = new Point(142, 126);
                    icsDateTimeFrom.Size = new Size(277, 20);
                    icsDateTimeFrom.Visible = true;
                    icsIntegerFrom.Visible = false;
                    icsDoubleFrom.Visible = false;
                    textBox_FromValue.Visible = false;


                    icsDateTimeTo.Visible = false;
                    icsDoubleTo.Visible = false;
                    icsIntegerTo.Visible = false;
                    textBoxToValue.Visible = false;


                    textBox_FromValue.Text = "";
                    icsIntegerFrom.Value = IM.NullI;
                    icsDoubleFrom.Value = IM.NullD;
                    break;

                default:

                   break;
            }
        }

        private void comboBox_OperationCondition_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (
                (comboBox_OperationCondition.Items[comboBox_OperationCondition.SelectedIndex].ToString() == ConditionOperator.Between.ToString()) ||
                (comboBox_OperationCondition.Items[comboBox_OperationCondition.SelectedIndex].ToString() == ConditionOperator.NotBetween.ToString()))
            {
                label5.Visible = false;
                label_From.Visible = true;
                label_To.Visible = true;
                OrmCs.OrmVarType ormVarType = OrmCs.OrmVarType.var_String;
                ormVarType = GetVarTypeFromColumn(textBox_NameField.Text);
                ShowBlockTwoValue(ormVarType);
            }
            else
            {
                label5.Visible = true;
                label_From.Visible = false;
                label_To.Visible = false;
                OrmCs.OrmVarType ormVarType = OrmCs.OrmVarType.var_String;

                if (((comboBox_OperationCondition.Items[comboBox_OperationCondition.SelectedIndex].ToString() != ConditionOperator.In.ToString()) &&
                (comboBox_OperationCondition.Items[comboBox_OperationCondition.SelectedIndex].ToString() != ConditionOperator.NotIn.ToString())))
                {
                    ormVarType = GetVarTypeFromColumn(textBox_NameField.Text);
                }
                ShowBlockOneValue(ormVarType);
            }
        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            bool isDuplicate = false;
            if (string.IsNullOrEmpty(textBox_NameField.Text))
            {
                MessageBox.Show("Please input field name");
                return;
            }

            if (!isDuplicate)
            {
                var rsWebQueryNew = new IMRecordset(ICSMTbl.WebConstraint, IMRecordset.Mode.ReadWrite);
                rsWebQueryNew.Select("ID,WEBQUERYID,NAME,PATH,MIN,MAX,STRVALUE,STRVALUETO,DATEVALUEMIN,INCLUDE,DATEVALUEMAX,DESCRCONDITION,TYPECONDITION,OPERCONDITION,MESSAGENOTVALID,DEFAULTVALUE,MOMENTOFUSE");
                if (_id != IM.NullI)
                {
                    if (!IsNew)
                    {
                        rsWebQueryNew.SetWhere("ID", IMRecordset.Operation.Eq, _id);
                    }
                }

                OrmCs.OrmVarType ormVarType = OrmCs.OrmVarType.var_String;
                if (((comboBox_OperationCondition.Items[comboBox_OperationCondition.SelectedIndex].ToString() != ConditionOperator.In.ToString()) &&
                (comboBox_OperationCondition.Items[comboBox_OperationCondition.SelectedIndex].ToString() != ConditionOperator.NotIn.ToString())))
                {
                    ormVarType = GetVarTypeFromColumn(textBox_NameField.Text);
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
                        rsWebQueryNew.Put("PATH", textBox_NameField.Text);


                   
                        if (ormVarType == OrmCs.OrmVarType.var_String)
                        {
                            rsWebQueryNew.Put("STRVALUE", textBox_FromValue.Text);
                            rsWebQueryNew.Put("STRVALUETO", textBoxToValue.Text);
                        }
                        else if (ormVarType == OrmCs.OrmVarType.var_Int)
                        {
                            rsWebQueryNew.Put("MIN", icsIntegerFrom.Value != IM.NullI ? icsIntegerFrom.Value : IM.NullI);
                            rsWebQueryNew.Put("MAX", icsIntegerTo.Value != IM.NullI ? icsIntegerTo.Value : IM.NullI);
                        }
                        else if ((ormVarType == OrmCs.OrmVarType.var_Flo) || (ormVarType == OrmCs.OrmVarType.var_Dou))
                        {
                            rsWebQueryNew.Put("MIN", icsDoubleFrom.Value != IM.NullD ? icsDoubleFrom.Value : IM.NullD);
                            rsWebQueryNew.Put("MAX", icsDoubleTo.Value != IM.NullD ? icsDoubleTo.Value : IM.NullD);
                        }
                        else if (ormVarType == OrmCs.OrmVarType.var_Tim)
                        {
                            rsWebQueryNew.Put("DATEVALUEMIN", icsDateTimeFrom.Value);
                            rsWebQueryNew.Put("DATEVALUEMAX", icsDateTimeTo.Value);
                        }
                    
                        if (comboBox_MomentOfUse.Items.Count > 0) { if (comboBox_MomentOfUse.SelectedIndex != -1) rsWebQueryNew.Put("MOMENTOFUSE", comboBox_MomentOfUse.Text); }
                        if (comboBox_TypeCondition.Items.Count > 0) { if (comboBox_TypeCondition.SelectedIndex != -1) rsWebQueryNew.Put("TYPECONDITION", comboBox_TypeCondition.Text); }
                        if (comboBox_OperationCondition.Items.Count > 0) { if (comboBox_OperationCondition.SelectedIndex != -1) rsWebQueryNew.Put("OPERCONDITION", comboBox_OperationCondition.Text); }
                        rsWebQueryNew.Put("DESCRCONDITION", textBox_DescriptionCondition.Text);
                        rsWebQueryNew.Put("MESSAGENOTVALID", textBox_MessageNotValid.Text);
                        rsWebQueryNew.Put("DEFAULTVALUE", textBox_DefValues.Text);
                        rsWebQueryNew.Update();
                    }
                    else
                    {
                        if (!IsNew)
                        {
                            rsWebQueryNew.Edit();

                            rsWebQueryNew.Put("STRVALUE", "");
                            rsWebQueryNew.Put("STRVALUETO", "");
                            rsWebQueryNew.Put("MIN", IM.NullD);
                            rsWebQueryNew.Put("MAX", IM.NullD);
                            rsWebQueryNew.Put("DATEVALUEMIN", IM.NullT);
                            rsWebQueryNew.Put("DATEVALUEMAX", IM.NullT);

                            rsWebQueryNew.Put("WEBQUERYID", _web_id);
                            rsWebQueryNew.Put("PATH", textBox_NameField.Text);
                            if (ormVarType == OrmCs.OrmVarType.var_String)
                            {
                                rsWebQueryNew.Put("STRVALUE", textBox_FromValue.Text);
                                rsWebQueryNew.Put("STRVALUETO", textBoxToValue.Text);
                            }
                            else if (ormVarType == OrmCs.OrmVarType.var_Int)
                            {
                                rsWebQueryNew.Put("MIN", icsIntegerFrom.Value != IM.NullI ? icsIntegerFrom.Value : IM.NullI);
                                rsWebQueryNew.Put("MAX", icsIntegerTo.Value != IM.NullI ? icsIntegerTo.Value : IM.NullI);
                            }
                            else if ((ormVarType == OrmCs.OrmVarType.var_Flo) || (ormVarType == OrmCs.OrmVarType.var_Dou))
                            {
                                rsWebQueryNew.Put("MIN", icsDoubleFrom.Value != IM.NullD ? icsDoubleFrom.Value : IM.NullD);
                                rsWebQueryNew.Put("MAX", icsDoubleTo.Value != IM.NullD ? icsDoubleTo.Value : IM.NullD);
                            }
                            else if (ormVarType == OrmCs.OrmVarType.var_Tim)
                            {
                                rsWebQueryNew.Put("DATEVALUEMIN", icsDateTimeFrom.Value);
                                rsWebQueryNew.Put("DATEVALUEMAX", icsDateTimeTo.Value);
                            }
                          
                            if (comboBox_MomentOfUse.Items.Count > 0) { if (comboBox_MomentOfUse.SelectedIndex != -1) rsWebQueryNew.Put("MOMENTOFUSE", comboBox_MomentOfUse.Text); }
                            if (comboBox_TypeCondition.Items.Count > 0) { if (comboBox_TypeCondition.SelectedIndex != -1) rsWebQueryNew.Put("TYPECONDITION", comboBox_TypeCondition.Text); }
                            if (comboBox_OperationCondition.Items.Count > 0) { if (comboBox_OperationCondition.SelectedIndex != -1) rsWebQueryNew.Put("OPERCONDITION", comboBox_OperationCondition.Text); }
                            rsWebQueryNew.Put("DESCRCONDITION", textBox_DescriptionCondition.Text);
                            rsWebQueryNew.Put("MESSAGENOTVALID", textBox_MessageNotValid.Text);
                            rsWebQueryNew.Put("DEFAULTVALUE", textBox_DefValues.Text);
                            rsWebQueryNew.Update();
                        }
                        else
                        {
                            rsWebQueryNew.AddNew();
                            int idnew = IM.AllocID(ICSMTbl.WebConstraint, 1, -1);
                            rsWebQueryNew.Put("ID", idnew);
                            rsWebQueryNew.Put("WEBQUERYID", _web_id);
                            rsWebQueryNew.Put("PATH", textBox_NameField.Text);
                            if (ormVarType == OrmCs.OrmVarType.var_String)
                            {
                                rsWebQueryNew.Put("STRVALUE", textBox_FromValue.Text);
                                rsWebQueryNew.Put("STRVALUETO", textBoxToValue.Text);
                            }
                            else if (ormVarType == OrmCs.OrmVarType.var_Int)
                            {
                                rsWebQueryNew.Put("MIN", icsIntegerFrom.Value != IM.NullI ? icsIntegerFrom.Value : IM.NullI);
                                rsWebQueryNew.Put("MAX", icsIntegerTo.Value != IM.NullI ? icsIntegerTo.Value : IM.NullI);
                            }
                            else if ((ormVarType == OrmCs.OrmVarType.var_Flo) || (ormVarType == OrmCs.OrmVarType.var_Dou))
                            {
                                rsWebQueryNew.Put("MIN", icsDoubleFrom.Value != IM.NullD ? icsDoubleFrom.Value : IM.NullD);
                                rsWebQueryNew.Put("MAX", icsDoubleTo.Value != IM.NullD ? icsDoubleTo.Value : IM.NullD);
                            }
                            else if (ormVarType == OrmCs.OrmVarType.var_Tim)
                            {
                                rsWebQueryNew.Put("DATEVALUEMIN", icsDateTimeFrom.Value);
                                rsWebQueryNew.Put("DATEVALUEMAX", icsDateTimeTo.Value);
                            }
                         
                            if (comboBox_MomentOfUse.Items.Count > 0) { if (comboBox_MomentOfUse.SelectedIndex != -1) rsWebQueryNew.Put("MOMENTOFUSE", comboBox_MomentOfUse.Text); }
                            if (comboBox_TypeCondition.Items.Count > 0) { if (comboBox_TypeCondition.SelectedIndex != -1) rsWebQueryNew.Put("TYPECONDITION", comboBox_TypeCondition.Text); }
                            if (comboBox_OperationCondition.Items.Count > 0) { if (comboBox_OperationCondition.SelectedIndex != -1) rsWebQueryNew.Put("OPERCONDITION", comboBox_OperationCondition.Text); }
                            rsWebQueryNew.Put("DESCRCONDITION", textBox_DescriptionCondition.Text);
                            rsWebQueryNew.Put("MESSAGENOTVALID", textBox_MessageNotValid.Text);
                            rsWebQueryNew.Put("DEFAULTVALUE", textBox_DefValues.Text);
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

        private void textBox_NameField_Leave(object sender, EventArgs e)
        {
            comboBox_OperationCondition_SelectedIndexChanged(null, null);
        }
    }
}
