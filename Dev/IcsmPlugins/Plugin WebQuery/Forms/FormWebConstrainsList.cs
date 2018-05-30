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

namespace XICSM.WebQuery
{
    public partial class FormWebConstrainsList : Form
    {
        public int ID_WEB { get; set; }
        public int ID_Constraint { get; set; }
        public List<string> Path_List { get; set; }
        public FormWebConstrainsList(int id_web_query,List<string> L_Path)
        {
            InitializeComponent();
            ID_WEB = id_web_query;
            Path_List = L_Path;
            InitListView();
            ViewData();
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
            ColHeader clickedCol = (ColHeader)this.listView_constraints_lst.Columns[e.Column];
            clickedCol.ascending = !clickedCol.ascending;
            int numItems = this.listView_constraints_lst.Items.Count;
            this.listView_constraints_lst.BeginUpdate();
            ArrayList SortArray = new ArrayList();
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
            int WEB_QUERY_ID=IM.NullI;

            
               
                IMRecordset RsWebQueryNew = new IMRecordset(ICSMTbl.WebConstraint, IMRecordset.Mode.ReadWrite);
                RsWebQueryNew.Select("ID,WebQueryId,Name,Path,Min,Max,StrValue,DateValueMin,DateValueMax,Include");
                RsWebQueryNew.SetWhere("WebQueryId", IMRecordset.Operation.Eq, ID_WEB);
                for (RsWebQueryNew.Open(); !RsWebQueryNew.IsEOF(); RsWebQueryNew.MoveNext())
                {
                    ListViewItem itm;
                    arr[0] = RsWebQueryNew.GetI("ID").ToString();
                    IMRecordset RsWebQuery = new IMRecordset(ICSMTbl.WebQuery, IMRecordset.Mode.ReadOnly);
                    RsWebQuery.Select("ID,Name");
                    RsWebQuery.SetWhere("ID", IMRecordset.Operation.Eq, RsWebQueryNew.GetI("ID"));
                    RsWebQuery.Open();
                    if (!RsWebQuery.IsEOF())
                    {
                        arr[1] = RsWebQuery.GetS("Name");
                    }
                    if (RsWebQuery.IsOpen())
                        RsWebQuery.Close();
                    RsWebQuery.Destroy();

                    arr[2] = RsWebQueryNew.GetS("Name");
                    arr[3] = RsWebQueryNew.GetS("Path");
                    arr[4] = ((RsWebQueryNew.GetD("Min") != IM.NullD) && (RsWebQueryNew.GetD("Min") != IM.NullI)) ? RsWebQueryNew.GetD("Min").ToString() : "";
                    arr[5] = ((RsWebQueryNew.GetD("Max") != IM.NullD) && (RsWebQueryNew.GetD("Max") != IM.NullI)) ? RsWebQueryNew.GetD("Max").ToString() : "";
                    arr[6] = RsWebQueryNew.GetS("StrValue");
                    arr[7] = RsWebQueryNew.GetI("Include") != IM.NullI ? RsWebQueryNew.GetI("Include").ToString() : "";
                    arr[8] = RsWebQueryNew.GetT("DateValueMin") != IM.NullT ? RsWebQueryNew.GetT("DateValueMin").ToString() : "";
                    arr[9] = RsWebQueryNew.GetT("DateValueMax") != IM.NullT ? RsWebQueryNew.GetT("DateValueMax").ToString() : "";
                    itm = new ListViewItem(arr); listView_constraints_lst.Items.Add(itm);
                    listView_constraints_lst.FocusedItem = itm;
                    Num++;
                }
                if (RsWebQueryNew.IsOpen())
                    RsWebQueryNew.Close();
                RsWebQueryNew.Destroy();
          
        }


        private void button_add_new_Click(object sender, EventArgs e)
        {
            FormEditConstraintExtend extend_constraint = new FormEditConstraintExtend(ID_WEB, true, Path_List);
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
                    FormEditConstraintExtend extend_constraint = new FormEditConstraintExtend(ID_, false, Path_List);
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
                SortWrapper xItem = (SortWrapper)x;
                SortWrapper yItem = (SortWrapper)y;
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
