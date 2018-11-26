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
    public partial class FormWebConstrainsList : Form
    {
        public int idweb { get; set; }
        public int idconstraint { get; set; }
        public List<string> pathlist { get; set; }
        public FormWebConstrainsList(int idwebquery,List<string> lPath)
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
            listView_constraints_lst.Columns.Add(new ColHeader("ID constraint", 70, HorizontalAlignment.Left, true));
            listView_constraints_lst.Columns.Add(new ColHeader("Name Web Query", 150, HorizontalAlignment.Left, true));
            listView_constraints_lst.Columns.Add(new ColHeader("Name Constraint", 150, HorizontalAlignment.Left, true));
            listView_constraints_lst.Columns.Add(new ColHeader("Path", 150, HorizontalAlignment.Left, true));
            listView_constraints_lst.Columns.Add(new ColHeader("Numeric Min", 70, HorizontalAlignment.Left, true));
            listView_constraints_lst.Columns.Add(new ColHeader("Numeric Max", 70, HorizontalAlignment.Left, true));
            listView_constraints_lst.Columns.Add(new ColHeader("String value", 170, HorizontalAlignment.Left, true));
            listView_constraints_lst.Columns.Add(new ColHeader("Include", 20, HorizontalAlignment.Left, true));
            listView_constraints_lst.Columns.Add(new ColHeader("Date Min", 70, HorizontalAlignment.Left, true));
            listView_constraints_lst.Columns.Add(new ColHeader("Date Max", 70, HorizontalAlignment.Left, true));
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
            string[] arr = new string[10];
            int Num = 1; if (listView_constraints_lst.Items.Count > 0) Num = listView_constraints_lst.Items.Count + 1;
            int webqueryid=IM.NullI;
                var rsWebQueryNew = new IMRecordset(ICSMTbl.WebConstraint, IMRecordset.Mode.ReadWrite);
                rsWebQueryNew.Select("ID,WEBQUERYID,NAME,PATH,MIN,MAX,STRVALUE,DATEVALUEMIN,INCLUDE,DATEVALUEMAX");
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

                    arr[2] = rsWebQueryNew.GetS("NAME");
                    arr[3] = rsWebQueryNew.GetS("PATH");
                    arr[4] = ((rsWebQueryNew.GetD("MIN") != IM.NullD) && (rsWebQueryNew.GetD("MIN") != IM.NullI)) ? rsWebQueryNew.GetD("MIN").ToString() : "";
                    arr[5] = ((rsWebQueryNew.GetD("MAX") != IM.NullD) && (rsWebQueryNew.GetD("MAX") != IM.NullI)) ? rsWebQueryNew.GetD("MAX").ToString() : "";
                    arr[6] = rsWebQueryNew.GetS("STRVALUE");
                    arr[7] = rsWebQueryNew.GetI("INCLUDE") != IM.NullI ? rsWebQueryNew.GetI("INCLUDE").ToString() : "";
                    arr[8] = rsWebQueryNew.GetT("DATEVALUEMIN") != IM.NullT ? rsWebQueryNew.GetT("DATEVALUEMIN").ToString() : "";
                    arr[9] = rsWebQueryNew.GetT("DATEVALUEMAX") != IM.NullT ? rsWebQueryNew.GetT("DATEVALUEMAX").ToString() : "";
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
            FormEditConstraintExtend extend_constraint = new FormEditConstraintExtend(idweb, true, pathlist);
            extend_constraint.ShowDialog();
            ViewData();
        }

        private void button_edit_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listView_constraints_lst.SelectedItems.Count; i++)
            {
                int ID_;
                if (int.TryParse(listView_constraints_lst.SelectedItems[i].SubItems[0].Text, out ID_))
                {
                    FormEditConstraintExtend extend_constraint = new FormEditConstraintExtend(ID_, false, pathlist);
                    extend_constraint.ShowDialog();
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
                        IMRecordset RsWebQueryDel = new IMRecordset(ICSMTbl.WebConstraint, IMRecordset.Mode.ReadWrite);
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

    public class SortWrapper
    {
        internal ListViewItem sortItem;
        internal int sortColumn;


        public SortWrapper(ListViewItem Item, int iColumn)
        {
            sortItem = Item;
            sortColumn = iColumn;
        }

        
        public string Text
        {
            get
            {
                return sortItem.SubItems[sortColumn].Text;
            }
        }


        public class SortComparer : IComparer
        {
            bool ascending;


            public SortComparer(bool asc)
            {
                this.ascending = asc;
            }

            public int Compare(object x, object y)
            {
                var xItem = (SortWrapper)x;
                var yItem = (SortWrapper)y;
                string xText = xItem.sortItem.SubItems[xItem.sortColumn].Text;
                string yText = yItem.sortItem.SubItems[yItem.sortColumn].Text;
                return xText.CompareTo(yText) * (this.ascending ? 1 : -1);
            }
        }
    }

    public class ColHeader : ColumnHeader
    {
        public bool ascending;
        public ColHeader(string text, int width, HorizontalAlignment align, bool asc)
        {
            this.Text = text;
            this.Width = width;
            this.TextAlign = align;
            this.ascending = asc;
        }
    }


    public static class CallBackFunction
    {
        public delegate void callbackEvent(string what);
        public static callbackEvent callbackEventHandler;

        public delegate void callbackEventFunc(Action<string> what);
        public static callbackEventFunc callbackEventHandlerFunc;
    }

}
