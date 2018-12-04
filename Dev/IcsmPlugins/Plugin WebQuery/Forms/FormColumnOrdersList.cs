using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ICSM;
using System.Collections;

namespace XICSM.Atdi.Icsm.Plugins.WebQuery
{
    public partial class FormColumnOrdersList : Form
    {
        public int idweb { get; set; }
        public int idconstraint { get; set; }
        public List<string> pathlist { get; set; }
        public FormColumnOrdersList(int idwebquery,List<string> lPath)
        {
            InitializeComponent();
            idweb = idwebquery;
            pathlist = lPath;
            InitListView();
            ViewData();
            CLocaliz.TxT(this);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitListView()
        {

            listView_constraints_lst.View = View.Details;
            listView_constraints_lst.GridLines = true;
            listView_constraints_lst.FullRowSelect = true;
            listView_constraints_lst.Items.Clear();
            listView_constraints_lst.Columns.Clear();
            listView_constraints_lst.Columns.Add(new ColHeader("ID column order", 120, HorizontalAlignment.Left, true));
            listView_constraints_lst.Columns.Add(new ColHeader("Name Web Query", 150, HorizontalAlignment.Left, true));
            listView_constraints_lst.Columns.Add(new ColHeader("Name column (path)", 150, HorizontalAlignment.Left, true));
            listView_constraints_lst.Columns.Add(new ColHeader("Order type", 150, HorizontalAlignment.Left, true));
            this.listView_constraints_lst.ColumnClick += new ColumnClickEventHandler(listView_constraints_lst_click);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView_constraints_lst_click(object sender, ColumnClickEventArgs e)
        {
            var clickedCol = (ColHeader)this.listView_constraints_lst.Columns[e.Column];
            clickedCol.ascending = !clickedCol.ascending;
            int numItems = this.listView_constraints_lst.Items.Count;
            this.listView_constraints_lst.BeginUpdate();
            var SortArray = new ArrayList();
            for (int i = 0; i < numItems; i++)
            {
                SortArray.Add(new SortWrapper(this.listView_constraints_lst.Items[i], e.Column));
            }
            SortArray.Sort(0, SortArray.Count, new SortWrapper.SortComparer(clickedCol.ascending));
            this.listView_constraints_lst.Items.Clear();
            for (int i = 0; i < numItems; i++)
                this.listView_constraints_lst.Items.Add(((SortWrapper)SortArray[i]).sortItem);
            this.listView_constraints_lst.EndUpdate();
        }


        /// <summary>
        /// 
        /// </summary>
        private void ViewData()
        {
            listView_constraints_lst.Items.Clear();
            string[] arr = new string[6];
            int Num = 1; if (listView_constraints_lst.Items.Count > 0) Num = listView_constraints_lst.Items.Count + 1;
            int webqueryid = IM.NullI;
            var rsWebQueryNew = new IMRecordset(ICSMTbl.WebQueryOrders, IMRecordset.Mode.ReadWrite);
            rsWebQueryNew.Select("ID,WEBQUERYID,PATH,ORDER");
            rsWebQueryNew.SetWhere("WEBQUERYID", IMRecordset.Operation.Eq, idweb);
            for (rsWebQueryNew.Open(); !rsWebQueryNew.IsEOF(); rsWebQueryNew.MoveNext())
            {
                ListViewItem itm;
                arr[0] = rsWebQueryNew.GetI("ID").ToString();
                IMRecordset RsWebQuery = new IMRecordset(ICSMTbl.WebQuery, IMRecordset.Mode.ReadOnly);
                RsWebQuery.Select("ID,NAME");
                RsWebQuery.SetWhere("ID", IMRecordset.Operation.Eq, rsWebQueryNew.GetI("WEBQUERYID"));
                RsWebQuery.Open();
                if (!RsWebQuery.IsEOF())
                {
                    arr[1] = RsWebQuery.GetS("NAME");
                }
                if (RsWebQuery.IsOpen())
                    RsWebQuery.Close();
                RsWebQuery.Destroy();

                arr[2] = rsWebQueryNew.GetS("PATH");
                arr[3] = rsWebQueryNew.GetI("ORDER") == 1 ? "Asc" : "Desc";
                itm = new ListViewItem(arr); listView_constraints_lst.Items.Add(itm);
                listView_constraints_lst.FocusedItem = itm;
                Num++;
            }
            if (rsWebQueryNew.IsOpen())
                rsWebQueryNew.Close();
            rsWebQueryNew.Destroy();

        }


        private void button_add_new_Click(object sender, EventArgs e)
        {
            FormEditColumnOrder extend_attr = new FormEditColumnOrder(idweb,-1, true, pathlist);
            extend_attr.ShowDialog();
            ViewData();
        }

        private void button_edit_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listView_constraints_lst.SelectedItems.Count; i++)
            {
                int ID_;
                if (int.TryParse(listView_constraints_lst.SelectedItems[i].SubItems[0].Text, out ID_))
                {
                    FormEditColumnOrder extend_attr = new FormEditColumnOrder(idweb, ID_, false, pathlist);
                    extend_attr.ShowDialog();
                }
            }
            ViewData();
        }

        private void button_delete_Click(object sender, EventArgs e)
        {
            DialogResult DA = MessageBox.Show("Are you delete record?", "Question", MessageBoxButtons.YesNo);
            if (DA == DialogResult.Yes)
            {
                for (int i = 0; i < listView_constraints_lst.SelectedItems.Count; i++)
                {
                    int ID_;
                    if (int.TryParse(listView_constraints_lst.SelectedItems[i].SubItems[0].Text, out ID_))
                    {
                        IMRecordset RsWebQueryDel = new IMRecordset(ICSMTbl.WebQueryOrders, IMRecordset.Mode.ReadWrite);
                        RsWebQueryDel.Select("ID");
                        RsWebQueryDel.SetWhere("ID", IMRecordset.Operation.Eq, ID_);
                        RsWebQueryDel.Open();
                        if (!RsWebQueryDel.IsEOF())
                        {
                            RsWebQueryDel.Delete();
                        }
                    }
                }
            }
            ViewData();
        }
    }

   

}
