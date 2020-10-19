using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ICSM;
using XICSM.ICSControlClient.Metadata;
using Atdi.Common;


namespace XICSM.ICSControlClient.Forms
{
    public partial class ReportBandWidthForm : Form
    {
        private int Id { get; set; }
        public ReportBandWidthForm(int id)
        {
            InitializeComponent();
            Id = id;
            ShowRecord();
        }
        public void ShowRecord()
        {
            if (Id > 0)
            {
                IMRecordset recSel = new IMRecordset(ProtocolBandWidth.TableName, IMRecordset.Mode.ReadOnly);
                recSel.Select("ID,BW,STANDARD_NAME");
                recSel.SetWhere("ID", IMRecordset.Operation.Eq, Id);
                recSel.Open();
                if (!recSel.IsEOF())
                {
                    tbRadiotechnology.Text = recSel.GetS("STANDARD_NAME");
                    tbBandWidth.Text = recSel.GetD("BW").ToString();
                }

                if (recSel.IsOpen())
                    recSel.Close();
                recSel.Destroy();
            }
        }


        public void CreateNewRecord(string standard, double BW)
        {
            IMRecordset recNew = new IMRecordset(ProtocolBandWidth.TableName, IMRecordset.Mode.ReadWrite);
            recNew.Select("ID,BW,STANDARD_NAME");
            recNew.Open();
            var id = IM.AllocID(ProtocolBandWidth.TableName, 1, -1);
            recNew.AddNew();
            recNew.Put("ID", id);
            recNew.Put("BW", BW);
            recNew.Put("STANDARD_NAME", standard);
            recNew.Update();

            if (recNew.IsOpen())
                recNew.Close();
            recNew.Destroy();

        }

        public void UpdateRecord(int id, string standard, double BW)
        {
            IMRecordset recEdit = new IMRecordset(ProtocolBandWidth.TableName, IMRecordset.Mode.ReadWrite);
            recEdit.Select("ID,BW,STANDARD_NAME");
            recEdit.SetWhere("ID", IMRecordset.Operation.Eq, id);
            recEdit.Open();
            if (!recEdit.IsEOF())
            {
                recEdit.Edit();
                recEdit.Put("BW", BW);
                recEdit.Put("STANDARD_NAME", standard);
                recEdit.Update();
            }

            if (recEdit.IsOpen())
                recEdit.Close();
            recEdit.Destroy();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (Id ==-1)
            {
                CreateNewRecord(tbRadiotechnology.Text, string.IsNullOrEmpty(tbBandWidth.Text) ? IM.NullD : tbBandWidth.Text.ConvertStringToDouble().Value);
            }
            else if (Id > 0)
            {
                UpdateRecord(Id, tbRadiotechnology.Text, string.IsNullOrEmpty(tbBandWidth.Text) ? IM.NullD : tbBandWidth.Text.ConvertStringToDouble().Value);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
