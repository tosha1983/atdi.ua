using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using OnlinePortal.Utils;
using OnlinePortal;
using LitvaPortal.Utils;
using LitvaPortal;
using System.Text;
using DAL;
using System.IO;
using System.Drawing;
using System.Data.SqlClient;
using System.Configuration;
using DocumentFormat;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Office;
using DocumentFormat.OpenXml.Office2013;
using Utils;
using LitvaPortal.ServiceReference_WebQuery;

namespace OnlinePortal
{
    public partial class ContainerViewer : System.Web.UI.UserControl
    {
        WebQueryClient client;
        public int Curr_row { get; set; }
        public int TotalRecordsInQuery = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
           
            try {
                string FormatConstraint="";
                string PathNameFinder_ = Request.QueryString["PathNameFinder"];
                if (PathNameFinder_ == null) PathNameFinder_ = "";
                if (Session["AuthUser"] != null) {
                    ClassMenu Icls = new ClassMenu(Session["SettingIRPClass"]);
                    int ID = -1;
                    if (Request.QueryString["ID"] != null) {
                        int.TryParse(Request.QueryString["ID"].ToString(), out ID);
                    }
                    if (!Page.IsPostBack){
                        Session["Search_container"] = null;
                        if (Session["SettingIRPClass"] != null) {
                            Session["dt"] = null; Session["sort"] = null;
                            List<ClassIRPObject> Res_out = new List<ClassIRPObject>();
                            if ((Request.QueryString["TypeContainer"] != null) && (Session["Search_container"] == null)) {
                                string NTypeContainer = "";
                                string Cont = Request.QueryString["TypeContainer"].ToString();
                                string NameItemPress = Request.QueryString["NameItemPress"].ToString();
                                string SectionT = "";
                                if (Request.QueryString["SectionT"] != null)
                                {
                                    SectionT = Request.QueryString["SectionT"].ToString();
                                }
                                    List<ClassIRPObject> LIrp = (List<ClassIRPObject>)Session["Irp"];
                                    List<ClassIRPObject> Lst = (List<ClassIRPObject>)Session["NavigationPages"];
                                ClassIRPObject ityu_ = Lst.Find(r => r.Setting_param.NAME == (string.IsNullOrEmpty(NTypeContainer) ? Cont : NTypeContainer) && r.StatusObject.ToString() == SectionT);
                                    if (ityu_ != null) {
                                        if (ityu_.FormatConstraint!="")
                                            FormatConstraint = ityu_.FormatConstraint;
                                        if (ityu_.Setting_param.MAX_REC_PAGE > 0){
                                            GridView.PageSize = ityu_.Setting_param.MAX_REC_PAGE;
                                        }
                                    }
                                    if ((Session["sort"] == null)) { Session["dt"] = ityu_.GetRecords(ID, ref GridView, (string.IsNullOrEmpty(NTypeContainer) ? Cont : NTypeContainer), null, Session["StartIndex"] != null ? (int)Session["StartIndex"] : 0, out Res_out, Session["Irp"], NameItemPress, SectionT); Session["sort"] = "Asc"; }
                                if (GridView.DataSource != null) TotalRecordsInQuery = ((DataTable)GridView.DataSource).AsEnumerable().Count() + (TotalRecordsInQuery<0 ? TotalRecordsInQuery : 0);
                                
                            }
                        }
                    }
                    else  {

                        if (Session["Curr_Row"] != null)
                            InitPopupModal(false, (int)Session["Curr_Row"]);
                        if (Session["isCreateNewStation"] != null)
                            InitPopupModalCreate(false);
                        List<ClassIRPObject> Res_out = new List<ClassIRPObject>();
                        if ((Request.QueryString["TypeContainer"] != null) && (Session["Search_container"] != null)) {
                            string NTypeContainer = "";
                            string Cont = Request.QueryString["TypeContainer"].ToString();
                            string NameItemPress = Request.QueryString["NameItemPress"].ToString();
                            string SectionT = "";
                            if (Request.QueryString["SectionT"] != null)
                            {
                                SectionT = Request.QueryString["SectionT"].ToString();
                            }
                            

                            List<BlockDataFind> ir_ = new List<BlockDataFind>();
                            if (Session["Search_container"] != null) {
                                ir_ = (List<BlockDataFind>)(Session["Search_container"]);
                            }
                                List<ClassIRPObject> LIrp = (List<ClassIRPObject>)Session["Irp"];
                                List<ClassIRPObject> Lst = (List<ClassIRPObject>)Session["NavigationPages"];
                            ClassIRPObject ityu_ = Lst.Find(r => r.Setting_param.NAME == (string.IsNullOrEmpty(NTypeContainer) ? Cont : NTypeContainer) && r.StatusObject.ToString() == SectionT);
                                if (ityu_ != null)  {
                                    if (ityu_.FormatConstraint!="")
                                            FormatConstraint = ityu_.FormatConstraint;
                                    if (ityu_.Setting_param.MAX_REC_PAGE > 0) {
                                        GridView.PageSize = ityu_.Setting_param.MAX_REC_PAGE;
                                    }
                                }
                            //if ((Session["sort"] == null)) { Session["dt"] = Icls.GetRecords(ID, Session["AuthUser"].ToString(), ref GridView, Request.QueryString["TypeContainer"].ToString(), ir_, Session["StartIndex"] != null ? (int)Session["StartIndex"] : 0, out Res_out,  Session["Irp"]); Session["sort"] = "Asc"; }
                            if (ir_ != null) {
                                ////////////
                                List<ClassIRPObject> Irp = new List<ClassIRPObject>();
                                client = new WebQueryClient("BasicHttpBinding_IWebQueryManager");
                                ResultOfQueryGroupskoy_Sv8m5 rs = client.GetQueryGroups(new UserToken
                                {
                                    Data = ((LitvaPortal.AuthenticationManager.UserToken)Session["AuthUser"]).Data
                                });

                                if (rs.Data != null)
                                {
                                    NTypeContainer = (string.IsNullOrEmpty(NTypeContainer) ? Cont : NTypeContainer);

                                }
                                if (!string.IsNullOrEmpty(NTypeContainer))
                                {
                                    if (rs.Data != null)
                                    {
                                        Irp = new List<ClassIRPObject>();
                                        foreach (QueryGroup nd in rs.Data.Groups)
                                        {
                                            if (nd.QueryTokens != null)
                                            {
                                                foreach (QueryToken t in nd.QueryTokens)
                                                {


                                                    ClassIRPObject class_irp = new ClassIRPObject();
                                                    TypeStatus stat;

                                                    ResultOfQueryMetadatakoy_Sv8m5 meta = client.GetQueryMetadata(new UserToken
                                                    {
                                                        Data = ((LitvaPortal.AuthenticationManager.UserToken)Session["AuthUser"]).Data
                                                    }, t);

                                                    if (meta.Data.Name == NTypeContainer)
                                                    {

                                                       var res = client.ExecuteQuery(new UserToken
                                                        {
                                                            Data = ((LitvaPortal.AuthenticationManager.UserToken)Session["AuthUser"]).Data
                                                        },
                                                        t,
                                                        new FetchOptions
                                                        {
                                                            //Limit = new DataLimit { Type = LimitValueType.Records, Value = 10 },
                                                            Id = new Guid(),
                                                            ResultStructure = DataSetStructure.StringCells,
                                                             Orders = new OrderExpression[]
                                                             {
                                                                 new OrderExpression
                                                                 {
                                                                  ColumnName = "ID",
                                                                  OrderType = OrderType.Descending
                                                                 }
                                                             }
                                                            
                                                        });
                                                        if (res != null)
                                                        {
                                                            List<object[]> ResD = new List<object[]>();

                                                            string[][] row = (res.Data.Dataset as StringCellsDataSet).Cells;
                                                            foreach (string[] r in row)
                                                                ResD.Add(r.ToArray());

                                                            class_irp.Val_Arr = ResD;
                                                        }
                                                    }
                                                    class_irp.Setting_param = new SettingIRPClass();
                                                    class_irp.Setting_param.NAME = meta.Data.Name;
                                                    class_irp.Setting_param.MAX_COLUMNS = meta.Data.Columns.Count();
                                                    //class_irp.Setting_param.ExtendedControlRight = ExtendedControlRight.FullRight.ToString();
                                                    if (Enum.TryParse(meta.Data.Token.Version, out stat))
                                                        class_irp.Setting_param.STATUS_ = stat;
                                                    //class_irp.SettingConstraint = new List<WebConstraint>();
                                                    class_irp.PagesIndexRange = new SettingIRPClass[1];
                                                    foreach (ColumnMetadata cv in meta.Data.Columns)
                                                    {
                                                        class_irp.FLD.Add(cv.Description);
                                                        class_irp.CAPTION_FLD.Add(cv.Title);
                                                        if (cv.Type == DataType.Boolean)
                                                            class_irp.FLD_TYPE.Add(typeof(bool));
                                                        else if (cv.Type == DataType.DateTime)
                                                            class_irp.FLD_TYPE.Add(typeof(DateTime));
                                                        else if (cv.Type == DataType.Double)
                                                            class_irp.FLD_TYPE.Add(typeof(double));
                                                        else if (cv.Type == DataType.Integer)
                                                            class_irp.FLD_TYPE.Add(typeof(int));
                                                        else if (cv.Type == DataType.String)
                                                            class_irp.FLD_TYPE.Add(typeof(string));
                                                    }
                                                    if (Enum.TryParse(meta.Data.Token.Version, out stat))
                                                        class_irp.StatusObject = stat;
                                                    Irp.Add(class_irp);
                                                }
                                            }
                                        }
                                    }
                                }

                                Session["Irp"] = Irp;
                                Session["dt"] = ityu_.GetRecords(ID, ref GridView, (string.IsNullOrEmpty(NTypeContainer) ? Cont : NTypeContainer), ir_, Session["StartIndex"] != null ? (int)Session["StartIndex"] : 0, out Res_out, Session["Irp"], NameItemPress, SectionT); Session["sort"] = "Asc";
                            }
                            if ((Session["sort"] == null)) { Session["dt"] = ityu_.GetRecords(ID, ref GridView, (string.IsNullOrEmpty(NTypeContainer) ? Cont : NTypeContainer), ir_, Session["StartIndex"] != null ? (int)Session["StartIndex"] : 0, out Res_out, Session["Irp"], NameItemPress, SectionT); Session["sort"] = "Asc"; }
                            if (GridView.DataSource != null) TotalRecordsInQuery = ((DataTable)GridView.DataSource).AsEnumerable().Count() + (TotalRecordsInQuery < 0 ? TotalRecordsInQuery : 0);
                        }
                        else if ((Request.QueryString["TypeContainer"] != null) && (Session["Search_container"] == null)) {
                            string NTypeContainer = "";
                            string Cont = Request.QueryString["TypeContainer"].ToString();
                            string NameItemPress = Request.QueryString["NameItemPress"].ToString();
                            string SectionT = "";
                            if (Request.QueryString["SectionT"] != null)
                            {
                                SectionT = Request.QueryString["SectionT"].ToString();
                            }
                                List<ClassIRPObject> LIrp = (List<ClassIRPObject>)Session["Irp"];
                                List<ClassIRPObject> Lst = (List<ClassIRPObject>)Session["NavigationPages"];
                                ClassIRPObject ityu_ = Lst.Find(r => r.Setting_param.NAME == (string.IsNullOrEmpty(NTypeContainer) ? Cont : NTypeContainer) && r.StatusObject.ToString() == SectionT);
                                if (ityu_ != null) {
                                    if (ityu_.Setting_param.MAX_REC_PAGE > 0) {
                                        GridView.PageSize = ityu_.Setting_param.MAX_REC_PAGE;
                                    }
                                    if (ityu_.PagesIndexRange != null) {
                                        if (ityu_.FormatConstraint!="")
                                            FormatConstraint = ityu_.FormatConstraint;
                                    }
                                }
                                if ((Session["sort"] == null)) { Session["dt"] = ityu_.GetRecords(ID, ref GridView, (string.IsNullOrEmpty(NTypeContainer) ? Cont : NTypeContainer), null, Session["StartIndex"] != null ? (int)Session["StartIndex"] : 0, out Res_out, Session["Irp"], NameItemPress, SectionT); Session["sort"] = "Asc"; }
                                if (GridView.DataSource != null) TotalRecordsInQuery = ((DataTable)GridView.DataSource).AsEnumerable().Count() + (TotalRecordsInQuery < 0 ? TotalRecordsInQuery : 0);
                        }
                    }

                    PanelWebQuestionInput.Visible = false;
                    PanelWebQuestionOutput.Visible = false;
                    PanelAnswer.Visible = false;
                   
                }
            }
            catch (Exception ex)
            {
               MessageBox.Show(ex.Message);
            }
           
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (Session["AuthUser"] != null) {
                ClassMenu Icls = new ClassMenu(Session["SettingIRPClass"]);
                List<ClassIRPObject> Res_out = new List<ClassIRPObject>();
                ClassIRPObject irpObj = new ClassIRPObject();
                int ID = -1;
                if (Request.QueryString["ID"] != null)  {
                    int.TryParse(Request.QueryString["ID"].ToString(), out ID);
                }

                if (Page.IsPostBack)  {
                    if (Session["Curr_Row"] != null) {
                        Session["isCreateNewStation"] = null;
                        InitPopupModal(true, (int)Session["Curr_Row"]);
                    }
                    else if (Session["isCreateNewStation"] != null) {
                        Session["Curr_Row"] = null;
                        InitPopupModalCreate(true);
                    }
                    if ((Request.QueryString["TypeContainer"] != null) && (Session["Search_container"] == null)) {
                        string NTypeContainer = "";
                        string Cont = Request.QueryString["TypeContainer"].ToString();
                        string NameItemPress = Request.QueryString["NameItemPress"].ToString();
                        string SectionT = "";
                        if (Request.QueryString["SectionT"] != null)
                        {
                            SectionT = Request.QueryString["SectionT"].ToString();
                        }
                            List<ClassIRPObject> LIrp = (List<ClassIRPObject>)Session["Irp"];
                            if ((Session["sort"] == null)) { Session["dt"] = irpObj.GetRecords(ID,  ref GridView, (string.IsNullOrEmpty(NTypeContainer) ? Cont : NTypeContainer), null, Session["StartIndex"] != null ? (int)Session["StartIndex"] : 0, out Res_out, Session["Irp"], NameItemPress, SectionT); Session["sort"] = "Asc"; }
                            if (GridView.DataSource != null) TotalRecordsInQuery = ((DataTable)GridView.DataSource).AsEnumerable().Count() + (TotalRecordsInQuery < 0 ? TotalRecordsInQuery : 0);
                    }
                    else if ((Request.QueryString["TypeContainer"] != null) && (Session["Search_container"] != null)){
                        string NTypeContainer = "";
                        string Cont = Request.QueryString["TypeContainer"].ToString();
                        string NameItemPress = Request.QueryString["NameItemPress"].ToString();
                        string SectionT = "";
                        if (Request.QueryString["SectionT"] != null)
                        {
                            SectionT = Request.QueryString["SectionT"].ToString();
                        }
                            List<ClassIRPObject> LIrp = (List<ClassIRPObject>)Session["Irp"];
                        List<BlockDataFind> ir_ = new List<BlockDataFind>();
                        if (Session["Search_container"] != null) {
                            ir_ = (List<BlockDataFind>)(Session["Search_container"]);
                          
                            Session["dt"] = irpObj.GetRecords(ID, ref GridView, (string.IsNullOrEmpty(NTypeContainer) ? Cont : NTypeContainer), ir_, Session["StartIndex"] != null ? (int)Session["StartIndex"] : 0, out Res_out, Session["Irp"], NameItemPress, SectionT); Session["sort"] = "Asc";
                            if (GridView.DataSource != null) TotalRecordsInQuery = ((DataTable)GridView.DataSource).AsEnumerable().Count() + (TotalRecordsInQuery < 0 ? TotalRecordsInQuery : 0);
                        }
                        if ((Session["sort"] == null)) { Session["dt"] = irpObj.GetRecords(ID,  ref GridView, (string.IsNullOrEmpty(NTypeContainer) ? Cont : NTypeContainer), ir_, Session["StartIndex"] != null ? (int)Session["StartIndex"] : 0, out Res_out, Session["Irp"], NameItemPress, SectionT); Session["sort"] = "Asc"; }
                        if (GridView.DataSource != null) TotalRecordsInQuery = ((DataTable)GridView.DataSource).AsEnumerable().Count() + (TotalRecordsInQuery < 0 ? TotalRecordsInQuery : 0);
                    }
                }
                else
                {
                                       
                }
                btnUpdate.Visible = true;
                //TotalRecords.Text = TotalRecordsInQuery.ToString();
            }
        }



      

        /// <summary>
        /// ПОлучить имя корневой таблицы текущего запроса
        /// </summary>
        /// <returns></returns>
        public string GetTableName()
        {
            string TableName = "";
            string TypeContainer = "";
            string SectionT = "";
            if (Request.QueryString["SectionT"] != null)
            {
                SectionT = Request.QueryString["SectionT"].ToString();
            }
            if (Request.QueryString["TypeContainer"] != null) {
                TypeContainer = Request.QueryString["TypeContainer"].ToString();
                List<ClassIRPObject> Lst = (List<ClassIRPObject>)Session["Irp"];
                if (Lst != null) {
                    string NTypeContainer = "";
                    ClassIRPObject ir = Lst.Find(r => r.Setting_param.NAME == (string.IsNullOrEmpty(NTypeContainer) ? TypeContainer : NTypeContainer) && r.StatusObject.ToString() == SectionT);
                    if (ir != null) {
                        TableName = ir.TABLE_NAME;
                    }
                }
            }
            return TableName;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pn"></param>
        /// <returns></returns>
        public bool CheckContaintTextBoxControl(Panel pn)
        {
            bool is_ = false;
            foreach (Control it in pn.Controls) {
                if (it is TextBox) {
                    is_=true;
                    break;
                }
            }
            return is_;
        }

        /// <summary>
        /// Метод, выполняющий генерацию перечня полей ввода
        /// для всплывающей формы редактирования выделенной записи
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="TypeContainer"></param>
        /// <param name="Prefix"></param>
        public void GenerateStructEditorField(bool IsPostback, object obj, string TypeContainer, string Prefix, int Curr_row, bool isCreate, string SectionT, bool isRecreated)
        {
            if (isRecreated)
            {
                PanelEditor1.Controls.Clear();

                ManageBlocks tr = new ManageBlocks(TypeContainer);
                tr.FillArrBlocks((List<ClassIRPObject>)Session["Irp"], Prefix, true, this, SectionT);
                Table findet_tabl = new Table();
                TableRow tr_x_gl = new TableRow();
                List<TableCell> cell_arr = new List<TableCell>();
                List<TableRow> row_arr = new List<TableRow>();
                int idx_t_block = 1;

                //////
                string IdentUser = "";
                List<ClassIRPObject> f_info = (List<ClassIRPObject>)Session["Irp"];
                int start_hide_idx = 0;
                int countColumnVisible = 0;
                List<string> Fld_except = new List<string>();
                if (!string.IsNullOrEmpty(TypeContainer))
                {
                    ClassIRPObject Ir_ = f_info.Find(r => r.Setting_param.NAME == TypeContainer && r.StatusObject.ToString() == SectionT);
                    if (Ir_ != null)
                    {
                        countColumnVisible = Ir_.Setting_param.HIDDEN_COLUMNS;
                        start_hide_idx = Ir_.FLD.Count - countColumnVisible;
                        for (int y = 0; y < Ir_.FLD.Count; y++) {
                            if (Ir_.FLD[y] == Ir_.Setting_param.Ident_User) {
                                IdentUser = Ir_.CAPTION_FLD[y];
                                break;
                            }
                        }
                        for (int y = start_hide_idx; y < Ir_.FLD.Count; y++) {
                                Fld_except.Add(Ir_.FLD[y]);
                        }
                    }
                }
                /////
                bool isContainExcept = false;
                int StartIdxContainExcludeField = tr.Arr_Block.Count-1;
                for (int i = 0; i < tr.Arr_Block.Count(); i++) {
                    if (Fld_except.Contains(tr.Arr_Block[i].NameField)) { 
                        StartIdxContainExcludeField = i;
                        isContainExcept = true;
                        break; 
                    }
                }
                /////

                for (int i = 0; i < tr.Arr_Block.Count(); i++)
                {

                    if (tr.Arr_Block[i].Lbl.Text.Trim() == "ID")
                        continue;

                    if ((tr.Arr_Block[i].Lbl.Text.Trim() == IdentUser) && (IdentUser != ""))
                    continue;

                    if ((i >= StartIdxContainExcludeField) && (isContainExcept)) continue;
                    

                    Panel pn = new Panel();
                    pn.ID = tr.Arr_Block[i].Lbl.Text + "Pn" + i.ToString();
                    pn.BorderWidth = 1;
                    pn.HorizontalAlign = HorizontalAlign.Center;
                    pn.BorderStyle = BorderStyle.Dotted;
                    pn.Attributes["style"] = "padding: 5px; margin 5px;";

                    Table tabl = new Table();
                    TableRow tr_x = new TableRow();
                    TableCell cell1 = new TableCell();
                    cell1.Controls.Add(tr.Arr_Block[i].Lbl);
                    TableCell cell2 = new TableCell(); cell2.Width = new Unit("100%");
                    //cell2.Controls.Add(tr.Arr_Block[i].Txt);
                    if (tr.Arr_Block[i].LnkControl != null) { cell2.Controls.Add(tr.Arr_Block[i].LnkControl); }
                    else
                    {
                        if (tr.Arr_Block[i].Txt.ID != null)
                        {
                            { if (tr.Arr_Block[i].Txt.ID.Length > 0) cell2.Controls.Add(tr.Arr_Block[i].Txt); }
                        }
                        else if (tr.Arr_Block[i].Combo.ID != null)
                        {
                            { if (tr.Arr_Block[i].Combo.ID.Length > 0) cell2.Controls.Add(tr.Arr_Block[i].Combo); }
                        }
                    }
                    if (tr.Arr_Block[i].Extender != null) cell2.Controls.Add(tr.Arr_Block[i].Extender);



                    tr_x.Controls.Add(cell1); tr_x.Controls.Add(cell2); //tr_x.Controls.Add(cell3);
                    tabl.Controls.Add(tr_x);
                    pn.Controls.Add(tabl);

                    if (idx_t_block == 1)
                    {
                        cell_arr = new List<TableCell>();
                        tr_x_gl = new TableRow();
                        tr_x_gl.Width = new Unit("100%");
                    }


                    if (((idx_t_block % 3) == 0) || (i == tr.Arr_Block.Count() - 1) || ((i == (StartIdxContainExcludeField - 1)) && (isContainExcept)))
                    {
                        TableCell cx_ = new TableCell();
                        cx_.Controls.Add(pn);
                        cell_arr.Add(cx_);
                        foreach (TableCell item in cell_arr)
                        {
                            tr_x_gl.Controls.Add(item);
                        }
                        row_arr.Add(tr_x_gl);
                        idx_t_block = 1;
                    }
                    else
                    {
                        TableCell cx_ = new TableCell();
                        cx_.Controls.Add(pn);
                        cell_arr.Add(cx_);
                        idx_t_block++;
                    }

                }

                findet_tabl.Width = new Unit("100%");
                findet_tabl.BorderWidth = 1;
                findet_tabl.BorderStyle = BorderStyle.Dotted;

                findet_tabl.Style.Add("-webkit-border-radius", "10px");
                findet_tabl.Style.Add("-moz-border-radius", "10px");
                findet_tabl.Style.Add("border-radius", "10px");

                for (int itx = 0; itx < row_arr.Count(); itx++)
                {
                    row_arr[itx].BorderStyle = BorderStyle.Dotted;
                    row_arr[itx].BorderWidth = 1;
                    row_arr[itx].Width = new Unit("100%");
                    findet_tabl.Controls.Add(row_arr[itx]);
                }

                PanelEditor1.Width = new Unit("100%");
                PanelEditor1.HorizontalAlign = HorizontalAlign.Center;
                PanelEditor1.Controls.Add(findet_tabl);

            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void InitPopupModal(bool isPostBack, int Curr_Row)
        {
               bool isCreateNewStation = false;

                string TypeContainer = "";
                string NTypeContainer = "";
                string SectionT = "";
                if (Request.QueryString["SectionT"] != null)
                {
                    SectionT = Request.QueryString["SectionT"].ToString();
                }
                if (Request.QueryString["TypeContainer"] != null) {
                    TypeContainer = Request.QueryString["TypeContainer"].ToString();
                }

                int ID = -1;
                if (Request.QueryString["ID"] != null) {
                    int.TryParse(Request.QueryString["ID"].ToString(), out ID);
                }
                
                List<ClassIRPObject> Res_out = new List<ClassIRPObject>();
                List<ClassIRPObject> LIrp = (List<ClassIRPObject>)Session["Irp"];
                if (Session["Irp"] != null) {
                    if (!isPostBack)
                        GenerateStructEditorField(isPostBack, Session["Irp"], string.IsNullOrEmpty(NTypeContainer) ? TypeContainer : NTypeContainer, "Editor", Curr_Row, false, SectionT, true);
                }
                if (Session["isCreateNewStation"] != null) isCreateNewStation = (bool)Session["isCreateNewStation"];
                ContainerFinder cnf = new ContainerFinder();
                List<BlockDataFind> obj = cnf.GetFieldFromFormFinder(this, Session["Irp"], string.IsNullOrEmpty(NTypeContainer) ? TypeContainer : NTypeContainer, "Editor", true, 1, Curr_Row, false, SectionT);
               
        }

        /// <summary>
        /// 
        /// </summary>
        public void InitPopupModalCreate(bool isPostBack)
        {

            string SectionT = "";
            if (Request.QueryString["SectionT"] != null)
            {
                SectionT = Request.QueryString["SectionT"].ToString();
            }
            bool isCreateNewStation = false;
            string TypeContainer = "";
            if (Request.QueryString["TypeContainer"] != null){
                TypeContainer = Request.QueryString["TypeContainer"].ToString();
            }
            if (Session["Irp"] != null){
                GenerateStructEditorField(isPostBack, Session["Irp"], TypeContainer, "Editor", -1, true, SectionT, true);
            }
            if (Session["isCreateNewStation"] != null) isCreateNewStation = (bool)Session["isCreateNewStation"];
            ContainerFinder cnf = new ContainerFinder(); List<BlockDataFind> LstBlockFndVal = new List<BlockDataFind>();
            List<object> obj = cnf.GetFieldFromFormFinderCreate(this, Session["Irp"], TypeContainer, "Editor", true, 0, out LstBlockFndVal, SectionT);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GridViewMain_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (Session["dt"] != null) {
                string TypeContainer = "";
                string SectionT = "";
                if (Request.QueryString["SectionT"] != null)
                {
                    SectionT = Request.QueryString["SectionT"].ToString();
                }
                if (Request.QueryString["TypeContainer"] != null) {
                    TypeContainer = Request.QueryString["TypeContainer"].ToString();
                }
                DataTable dt1 = (DataTable)Session["dt"];
                if (dt1.Rows.Count > 0){
                    if (Convert.ToString(Session["sort"]) == "Asc") {
                        dt1.DefaultView.Sort = e.SortExpression + " Desc";
                        Session["sort"] = "Desc";
                    }
                    else {
                        dt1.DefaultView.Sort = e.SortExpression + " Asc";
                        Session["sort"] = "Asc";
                    }
                    GridView.DataSource = dt1;
                    GridView.DataBind();
                    if (Session["Irp"] != null) {
                        List<ClassIRPObject> Res_out = new List<ClassIRPObject>();
                        List<ClassIRPObject> LIrp = (List<ClassIRPObject>)Session["Irp"];
                        {
                            ClassIRPObject fnd_ = LIrp.Find(r => r.Setting_param.NAME == TypeContainer && r.StatusObject.ToString() == SectionT);
                            if (fnd_ != null)
                            {
                                if (fnd_.Setting_param.index_group_field > 0) DataAdapterClass.GroupGridView(GridView.Rows, fnd_.Setting_param.index_group_field, fnd_.Setting_param.TOTAL_COUNT_GROUP);
                            }
                        }
                    }
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GridViewMain_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            GridView.DataBind();
        }


        protected int GetColumnIndexByName_(GridViewRow row, string columnName, out bool isCheck, out int Delta)
        {
            isCheck = false;
            int columnIndex = 0;
            Delta = 0;
            foreach (DataControlFieldCell cell in row.Cells) {
                if ((cell.ContainingField).HeaderText=="") Delta++;
                if ((cell.ContainingField).HeaderText.Equals(columnName)) {
                    isCheck = true;
                    break;
                }
                columnIndex++;
            }
            return columnIndex;
        }

     

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GridViewMain_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try {
            int ID_USER = -1;
             List<string> List_Id_Users = new List<string>();
             if (Session["AuthUser"] != null) {
                 if (int.TryParse(Session["AuthUser"].ToString(), out ID_USER))
                     List_Id_Users.Add(ID_USER.ToString());
             }

            int Delta = 0;
            bool isRedactFull = false;
            bool isCheckID = false;
            bool isCheckOwnerID = false;
            bool isVisibleDelete = false;
            bool isVisibleEdit = false;
            string NTypeContainer = "";
            string Cont = Request.QueryString["TypeContainer"].ToString();
            string SectionT = "";
            if (Request.QueryString["SectionT"] != null)
            {
                SectionT = Request.QueryString["SectionT"].ToString();
            }
            List<ClassIRPObject> LIrp = (List<ClassIRPObject>)Session["Irp"];
                ClassIRPObject fnd_ = new ClassIRPObject();
            string Caption_Fld = "";
            string Ident_User = "";


            if (((Request.QueryString["TypeContainer"] != null)))
            {
                fnd_ = LIrp.Find(r => r.Setting_param.NAME == (string.IsNullOrEmpty(NTypeContainer) ? Cont : NTypeContainer) && r.StatusObject.ToString() == SectionT);
                for (int k = 0; k < fnd_.FLD.Count(); k++)
                {
                    if (fnd_.FLD[k] == fnd_.Setting_param.Ident_User)
                    {
                        Caption_Fld = fnd_.CAPTION_FLD[k];
                        break;
                    }
                }
            }
            if (fnd_ != null)
            {
                if (fnd_.Setting_param.Ident_User != "")
                {
                    
                    for (int l = 0; l < fnd_.FLD.Count; l++)
                    {
                        if (fnd_.FLD[l] == fnd_.Setting_param.Ident_User)
                        {
                            if (fnd_.CAPTION_FLD[l] != "")
                            {
                                Ident_User = fnd_.CAPTION_FLD[l];
                                break;
                            }
                        }
                    }
                }
            }

            int index_owner = ConnectDB.NullI;

            if (e.Row.RowType == DataControlRowType.Footer){
                int index = GetColumnIndexByName_(e.Row, "ID", out isCheckID, out Delta);
                if (Ident_User != "")
                    index_owner = GetColumnIndexByName_(e.Row, Ident_User, out isCheckOwnerID, out Delta);
                if (isCheckID) e.Row.Cells[index].Visible = false;
                if ((index_owner > 0) || (index_owner != ConnectDB.NullI))
                {
                    e.Row.Cells[index_owner].Visible = false;
                }
            }

            if (e.Row.RowType == DataControlRowType.Header) {
                int index = GetColumnIndexByName_(e.Row, "ID", out isCheckID, out Delta);
                if (Ident_User!="")
                    index_owner = GetColumnIndexByName_(e.Row, Ident_User, out isCheckOwnerID, out Delta);
                if (isCheckID) e.Row.Cells[index].Visible = false;
                if ((index_owner>0) || (index_owner!=ConnectDB.NullI))
                {
                    e.Row.Cells[index_owner].Visible = false;
                }


            }
            if (e.Row.RowType == DataControlRowType.DataRow) {
                int index_ID = GetColumnIndexByName_(e.Row, "ID", out isCheckID, out Delta);
                if (Ident_User != "")
                    index_owner = GetColumnIndexByName_(e.Row, Ident_User, out isCheckOwnerID, out Delta);
                if (isCheckID) e.Row.Cells[index_ID].Visible = false;
                if (isCheckOwnerID) e.Row.Cells[index_owner].Visible = false;
                for (int i = 0; i < e.Row.Cells.Count; i++)  {
                    if (e.Row.Cells[i].Text == "ID")  {
                        e.Row.Cells[i].Visible = false;
                    }
                }


              

               if (fnd_ != null)
               {



                int x_b = e.Row.RowIndex;
                if (((GridView)sender).PageIndex > 1) {
                    x_b = x_b + (((GridView)sender).PageIndex) * (((GridView)sender).PageSize);
                }
                else if (((GridView)sender).PageIndex == 1) {
                    x_b = x_b + (((GridView)sender).PageSize);
                }


                int index_ident = 0;
                for (int x = 0; x < fnd_.CAPTION_FLD.Count(); x++) {
                    if (fnd_.CAPTION_FLD[x] == Caption_Fld) {
                        index_ident = x;
                        break;
                    }
                }


                object rec_id = Convert.ChangeType(GridView.DataKeys[e.Row.RowIndex].Value, typeof(System.Int32));
                if (rec_id != null) {
                    if (index_ID>=0) {
                        int Num_ID=0;
                            for (int x=0; x< fnd_.CAPTION_FLD.Count(); x++)  {
                                if (fnd_.CAPTION_FLD[x]=="ID") {
                                    Num_ID=x;
                                    break;
                                }
                            }
                        for (int x=0; x < fnd_.Val_Arr.Count();x++) {
                            double P=0; double.TryParse(fnd_.Val_Arr[x][Num_ID].ToString(), out P);
                            int var1 = (int)P;
                            int var2 = (int)rec_id;
                            if (var1 == var2) {
                                Num_ID = x;
                                break;
                            }
                        }

                        if (Num_ID >= 0)  {
                            //int index_ident = GetColumnIndexByName_(e.Row, Caption_Fld, out isCheckID, out Delta);
                            bool is_check_ident_id = false;
                            int index_ident_ID = GetColumnIndexByName_(e.Row, "ID", out is_check_ident_id, out Delta);

                                    Button myButton_del_ext = default(Button);
                                    myButton_del_ext = (Button)e.Row.Cells[1].FindControl("ButtonDelete");
                                    if (myButton_del_ext != null)
                                    {
                                        if (!isVisibleDelete)
                                        {
                                            myButton_del_ext.Visible = true;
                                            myButton_del_ext.Enabled = true;
                                        }
                                    }
                           }
                    }
                }
            }

                   Button myButton = default(Button);
                   myButton = (Button)e.Row.Cells[0].FindControl("ButtonLink");
                   if (myButton != null) {
                            myButton.Visible = true;
                       }
                   }
           
          }   
          catch (Exception ex)
          {
                MessageBox.Show(ex.Message);
          }
        }

      
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
          
            if (Session["AuthUser"] != null)  {
                client = new WebQueryClient("BasicHttpBinding_IWebQuery");
                int max_Val = ConnectDB.NullI;
                Session["sort"] = null;
                string TypeContainer = "";
                string SectionT = "";
                if (Request.QueryString["SectionT"] != null) {
                    SectionT = Request.QueryString["SectionT"].ToString();
                }
                if (Request.QueryString["TypeContainer"] != null) {
                    TypeContainer = Request.QueryString["TypeContainer"].ToString();
                }

                if (Session["isCreateNewStation"] == null) {


                        List<ClassIRPObject> Lst = (List<ClassIRPObject>)Session["Irp"];
                    ClassIRPObject ityu_ = Lst.Find(r => r.Setting_param.NAME == TypeContainer && r.StatusObject.ToString() == SectionT);
                        if (ityu_ != null) {
                            ContainerFinder cnf = new ContainerFinder();
                            List<BlockDataFind> obj_base = cnf.GetFieldFromFormFinder(this, Session["Irp"], TypeContainer, "Editor", true, 0, Session["Curr_Row"], false, SectionT);

                        ResultOfQueryGroupskoy_Sv8m5 rs = client.GetQueryGroups(new UserToken
                        {
                            Data = ((LitvaPortal.AuthenticationManager.UserToken)Session["AuthUser"]).Data
                        });
                        if (rs.Data != null)
                        {

                            foreach (QueryGroup nd in rs.Data.Groups)
                            {
                                if (nd.QueryTokens != null)
                                {
                                    foreach (QueryToken t in nd.QueryTokens)
                                    {

                                        ResultOfQueryMetadatakoy_Sv8m5 meta = client.GetQueryMetadata(new UserToken
                                        {
                                            Data = ((LitvaPortal.AuthenticationManager.UserToken)Session["AuthUser"]).Data
                                        }, t);



                                        if (meta.Data.Name == TypeContainer)
                                        {

                                            if (Session["Curr_Row"] != null)
                                            {
                                                int cntInt = 0;
                                                int cntBool = 0; int cntByte = 0; int cntDatetime = 0; int cntDecimal = 0; int cntDouble = 0; int cntFloat = 0; int cntGuid = 0; int cntString = 0;
                                                TypedDataRow row = new TypedDataRow();
                                                DataSetColumn[] dataSetCol = new DataSetColumn[ityu_.CAPTION_FLD.Count() - 1];
                                                int idx = 0;
                                                for (int k = 0; k < ityu_.CAPTION_FLD.Count(); k++)
                                                {
                                                    if (ityu_.CAPTION_FLD[k].Trim() == "ID")
                                                        continue;
                                                    dataSetCol[idx] = new DataSetColumn();
                                                    
                                                    dataSetCol[idx].Name = ityu_.CAPTION_FLD[k].Trim();
                                                    if (ityu_.FLD_TYPE[k] == typeof(int))
                                                    {
                                                        dataSetCol[idx].Type = DataType.Integer;
                                                        dataSetCol[idx].Index = cntInt;
                                                        cntInt++;
                                                    }
                                                    else if (ityu_.FLD_TYPE[k] == typeof(bool))
                                                    {
                                                        dataSetCol[idx].Type = DataType.Boolean;
                                                        dataSetCol[idx].Index = cntBool;
                                                        cntBool++;
                                                    }
                                                    else if(ityu_.FLD_TYPE[k] == typeof(byte))
                                                    {
                                                        dataSetCol[idx].Type = DataType.Byte;
                                                        dataSetCol[idx].Index = cntByte;
                                                        cntByte++;
                                                    }
                                                    else if(ityu_.FLD_TYPE[k] == typeof(DateTime))
                                                    {
                                                        dataSetCol[idx].Type = DataType.DateTime;
                                                        dataSetCol[idx].Index = cntDatetime;
                                                        cntDatetime++;
                                                    }
                                                    else if (ityu_.FLD_TYPE[k] == typeof(decimal))
                                                    {
                                                        dataSetCol[idx].Type = DataType.Decimal;
                                                        dataSetCol[idx].Index = cntDecimal;
                                                        cntDecimal++;
                                                    }
                                                    else if (ityu_.FLD_TYPE[k] == typeof(double))
                                                    {
                                                        dataSetCol[idx].Type = DataType.Double;
                                                        dataSetCol[idx].Index = cntDouble;
                                                        cntDouble++;
                                                    }
                                                    else if (ityu_.FLD_TYPE[k] == typeof(float))
                                                    {
                                                        dataSetCol[idx].Type = DataType.Float;
                                                        dataSetCol[idx].Index = cntFloat;
                                                        cntFloat++;
                                                    }
                                                    else if (ityu_.FLD_TYPE[k] == typeof(Guid))
                                                    {
                                                        dataSetCol[idx].Type = DataType.Guid;
                                                        dataSetCol[idx].Index = cntGuid;
                                                        cntGuid++;
                                                    }
                                                    else if (ityu_.FLD_TYPE[k] == typeof(string))
                                                    {
                                                        dataSetCol[idx].Type = DataType.String;
                                                        dataSetCol[idx].Index = cntString;
                                                        cntString++;
                                                    }
                                                    else
                                                    {
                                                        dataSetCol[idx].Type = DataType.String;
                                                        dataSetCol[idx].Index = cntString;
                                                        cntString++;
                                                    }

                                                    idx++;
                                                }
                                                row.IntegerCells = new int?[cntInt];
                                                row.BooleanCells = new bool?[cntBool];
                                                row.ByteCells = new byte?[cntByte];
                                                row.DateTimeCells = new DateTime?[cntDatetime];
                                                row.DecimalCells = new decimal?[cntDecimal];
                                                row.DoubleCells = new double?[cntDouble];
                                                row.FloatCells = new float?[cntFloat];
                                                row.GuidCells = new Guid?[cntGuid];
                                                row.StringCells = new string[cntString];


                                                List<int?> IntegerCells = new List<int?>();
                                                List<bool?> BooleanCells = new List<bool?>();
                                                List<byte?> ByteCells = new List<byte?>();
                                                List<DateTime?> DateTimeCells = new List<DateTime?>();
                                                List<decimal?> DecimalCells = new List<decimal?>();
                                                List<double?> DoubleCells = new List<double?>();
                                                List<float?> FloatCells = new List<float?>();
                                                List<Guid?> GuidCells = new List<Guid?>();
                                                List<string> StringCells = new List<string>();

                                                idx = 0;
                                                for (int k = 0; k < ityu_.CAPTION_FLD.Count(); k++)
                                                {
                                                    if (ityu_.CAPTION_FLD[k].Trim() == "ID")
                                                        continue;
                                                    BlockDataFind block_f = obj_base.Find(r => r.CaptionField == ityu_.CAPTION_FLD[k]);

                                                    if (block_f.Value == "") block_f.Value = null;
                                                    if (ityu_.FLD_TYPE[k] == typeof(int))
                                                    {
                                                        
                                                        IntegerCells.Add((int?)block_f.Value);
                                                    }
                                                    else if (ityu_.FLD_TYPE[k] == typeof(bool))
                                                    {
                                                        BooleanCells.Add((bool?)block_f.Value);
                                                    }
                                                    else if (ityu_.FLD_TYPE[k] == typeof(byte))
                                                    {
                                                        ByteCells.Add((byte?)block_f.Value);
                                                    }
                                                    else if (ityu_.FLD_TYPE[k] == typeof(DateTime))
                                                    {
                                                        DateTimeCells.Add((DateTime?)block_f.Value);
                                                    }
                                                    else if (ityu_.FLD_TYPE[k] == typeof(decimal))
                                                    {
                                                        DecimalCells.Add((decimal?)block_f.Value);
                                                    }
                                                    else if (ityu_.FLD_TYPE[k] == typeof(double))
                                                    {
                                                        DoubleCells.Add((double?)block_f.Value);
                                                    }
                                                    else if (ityu_.FLD_TYPE[k] == typeof(float))
                                                    {
                                                        FloatCells.Add((float?)block_f.Value);
                                                    }
                                                    else if (ityu_.FLD_TYPE[k] == typeof(Guid))
                                                    {
                                                        GuidCells.Add((Guid?)block_f.Value);
                                                    }
                                                    else if (ityu_.FLD_TYPE[k] == typeof(string))
                                                    {
                                                        StringCells.Add((string)block_f.Value);
                                                    }
                                                    else 
                                                    {
                                                        StringCells.Add((string)block_f.Value);
                                                    }
                                                }

                                                row.IntegerCells = IntegerCells.ToArray();
                                                row.BooleanCells = BooleanCells.ToArray();
                                                row.ByteCells = ByteCells.ToArray();
                                                row.DateTimeCells = DateTimeCells.ToArray();
                                                row.DecimalCells = DecimalCells.ToArray();
                                                row.DoubleCells = DoubleCells.ToArray();
                                                row.FloatCells = FloatCells.ToArray();
                                                row.GuidCells = GuidCells.ToArray();
                                                row.StringCells = StringCells.ToArray();

                                                ResultOfChangesResultPRoijPX3 chRes = client.SaveChanges(new UserToken
                                                {
                                                    Data = ((LitvaPortal.AuthenticationManager.UserToken)Session["AuthUser"]).Data
                                                }, t,

                                                      new Changeset
                                                      {
                                                          Id = new Guid(),
                                                          
                                                          Actions = new LitvaPortal.ServiceReference_WebQuery.Action[]
                                                        {
                                                           new TypedRowUpdationAction
                                                                {
                                                                    Condition = new ConditionExpression
                                                                    {
                                                                        Type = ConditionType.Expression,
                                                                        LeftOperand = new ColumnOperand{ ColumnName = "ID", Type = OperandType.Column },
                                                                        Operator = ConditionOperator.Equal,
                                                                        RightOperand = new IntegerValueOperand{ Value =  (int?)Session["Curr_Row"], Type = OperandType.Value, DataType= DataType.Integer },
                                                                    },
                                                                    Id = Guid.NewGuid(),
                                                                    Type = ActionType.Update,
                                                                    Columns = dataSetCol,
                                                                    Row =  row
                                                                  
                                                                }
                                                                  
                                                          },
                                                      });
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (Session["isCreateNewStation"] != null) {
                    ContainerFinder cnf = new ContainerFinder(); List<BlockDataFind> LstBlockFndVal = new List<BlockDataFind>();
                    List<BlockDataFind> obj_base = cnf.GetFieldFromFormFinder(this, Session["Irp"], TypeContainer, "Editor", true, 0, Session["Curr_Row"], false, SectionT);
                    List<ClassIRPObject> Lst = (List<ClassIRPObject>)Session["Irp"];
                    ClassIRPObject ityu_ = Lst.Find(r => r.Setting_param.NAME == TypeContainer && r.StatusObject.ToString() == SectionT);
                    if (ityu_ != null)
                    {
                      

                        cnf = new ContainerFinder();
                        ResultOfQueryGroupskoy_Sv8m5 rs = client.GetQueryGroups(new UserToken
                        {
                            Data = ((LitvaPortal.AuthenticationManager.UserToken)Session["AuthUser"]).Data
                        });
                        if (rs.Data != null)
                        {

                            foreach (QueryGroup nd in rs.Data.Groups)
                            {
                                if (nd.QueryTokens != null)
                                {
                                    foreach (QueryToken t in nd.QueryTokens)
                                    {

                                        ResultOfQueryMetadatakoy_Sv8m5 meta = client.GetQueryMetadata(new UserToken
                                        {
                                            Data = ((LitvaPortal.AuthenticationManager.UserToken)Session["AuthUser"]).Data
                                        }, t);



                                        if (meta.Data.Name == TypeContainer)
                                        {


                                           
                                            {
                                                int cntInt = 0;
                                                int cntBool = 0; int cntByte = 0; int cntDatetime = 0; int cntDecimal = 0; int cntDouble = 0; int cntFloat = 0; int cntGuid = 0; int cntString = 0;
                                                TypedDataRow row = new TypedDataRow();
                                                DataSetColumn[] dataSetCol = new DataSetColumn[ityu_.CAPTION_FLD.Count()];
                                                for (int k = 0; k < ityu_.CAPTION_FLD.Count(); k++)
                                                {
                                                    dataSetCol[k] = new DataSetColumn();

                                                    dataSetCol[k].Name = ityu_.CAPTION_FLD[k].Trim();
                                                    if (ityu_.FLD_TYPE[k] == typeof(int))
                                                    {
                                                        dataSetCol[k].Type = DataType.Integer;
                                                        dataSetCol[k].Index = cntInt;
                                                        cntInt++;
                                                    }
                                                    else if (ityu_.FLD_TYPE[k] == typeof(bool))
                                                    {
                                                        dataSetCol[k].Type = DataType.Boolean;
                                                        dataSetCol[k].Index = cntBool;
                                                        cntBool++;
                                                    }
                                                    else if (ityu_.FLD_TYPE[k] == typeof(byte))
                                                    {
                                                        dataSetCol[k].Type = DataType.Byte;
                                                        dataSetCol[k].Index = cntByte;
                                                        cntByte++;
                                                    }
                                                    else if (ityu_.FLD_TYPE[k] == typeof(DateTime))
                                                    {
                                                        dataSetCol[k].Type = DataType.DateTime;
                                                        dataSetCol[k].Index = cntDatetime;
                                                        cntDatetime++;
                                                    }
                                                    else if (ityu_.FLD_TYPE[k] == typeof(decimal))
                                                    {
                                                        dataSetCol[k].Type = DataType.Decimal;
                                                        dataSetCol[k].Index = cntDecimal;
                                                        cntDecimal++;
                                                    }
                                                    else if (ityu_.FLD_TYPE[k] == typeof(double))
                                                    {
                                                        dataSetCol[k].Type = DataType.Double;
                                                        dataSetCol[k].Index = cntDouble;
                                                        cntDouble++;
                                                    }
                                                    else if (ityu_.FLD_TYPE[k] == typeof(float))
                                                    {
                                                        dataSetCol[k].Type = DataType.Float;
                                                        dataSetCol[k].Index = cntFloat;
                                                        cntFloat++;
                                                    }
                                                    else if (ityu_.FLD_TYPE[k] == typeof(Guid))
                                                    {
                                                        dataSetCol[k].Type = DataType.Guid;
                                                        dataSetCol[k].Index = cntGuid;
                                                        cntGuid++;
                                                    }
                                                    else if (ityu_.FLD_TYPE[k] == typeof(string))
                                                    {
                                                        dataSetCol[k].Type = DataType.String;
                                                        dataSetCol[k].Index = cntString;
                                                        cntString++;
                                                    }
                                                    else
                                                    {
                                                        dataSetCol[k].Type = DataType.String;
                                                        dataSetCol[k].Index = cntString;
                                                        cntString++;
                                                    }

                                                }
                                                row.IntegerCells = new int?[cntInt];
                                                row.BooleanCells = new bool?[cntBool];
                                                row.ByteCells = new byte?[cntByte];
                                                row.DateTimeCells = new DateTime?[cntDatetime];
                                                row.DecimalCells = new decimal?[cntDecimal];
                                                row.DoubleCells = new double?[cntDouble];
                                                row.FloatCells = new float?[cntFloat];
                                                row.GuidCells = new Guid?[cntGuid];
                                                row.StringCells = new string[cntString];


                                                List<int?> IntegerCells = new List<int?>();
                                                List<bool?> BooleanCells = new List<bool?>();
                                                List<byte?> ByteCells = new List<byte?>();
                                                List<DateTime?> DateTimeCells = new List<DateTime?>();
                                                List<decimal?> DecimalCells = new List<decimal?>();
                                                List<double?> DoubleCells = new List<double?>();
                                                List<float?> FloatCells = new List<float?>();
                                                List<Guid?> GuidCells = new List<Guid?>();
                                                List<string> StringCells = new List<string>();


                                                for (int k = 0; k < ityu_.CAPTION_FLD.Count(); k++)
                                                {
                                                    if (ityu_.CAPTION_FLD[k].Trim() == "ID")
                                                    {
                                                        IntegerCells.Add(null);
                                                    }
                                                        
                                                    BlockDataFind block_f = obj_base.Find(r => r.CaptionField == ityu_.CAPTION_FLD[k]);

                                                    if (block_f == null) continue;
                                                    if (block_f.Value == "") block_f.Value = null;
                                                    if (ityu_.FLD_TYPE[k] == typeof(int))
                                                    {

                                                        IntegerCells.Add((int?)block_f.Value);
                                                    }
                                                    else if (ityu_.FLD_TYPE[k] == typeof(bool))
                                                    {
                                                        BooleanCells.Add((bool?)block_f.Value);
                                                    }
                                                    else if (ityu_.FLD_TYPE[k] == typeof(byte))
                                                    {
                                                        ByteCells.Add((byte?)block_f.Value);
                                                    }
                                                    else if (ityu_.FLD_TYPE[k] == typeof(DateTime))
                                                    {
                                                        DateTimeCells.Add((DateTime)block_f.Value);
                                                    }
                                                    else if (ityu_.FLD_TYPE[k] == typeof(decimal))
                                                    {
                                                        DecimalCells.Add((decimal?)block_f.Value);
                                                    }
                                                    else if (ityu_.FLD_TYPE[k] == typeof(double))
                                                    {
                                                        DoubleCells.Add((double?)block_f.Value);
                                                    }
                                                    else if (ityu_.FLD_TYPE[k] == typeof(float))
                                                    {
                                                        FloatCells.Add((float?)block_f.Value);
                                                    }
                                                    else if (ityu_.FLD_TYPE[k] == typeof(Guid))
                                                    {
                                                        GuidCells.Add((Guid?)block_f.Value);
                                                    }
                                                    else if (ityu_.FLD_TYPE[k] == typeof(string))
                                                    {
                                                        StringCells.Add((string)block_f.Value);
                                                    }
                                                    else
                                                    {
                                                        StringCells.Add((string)block_f.Value);
                                                    }
                                                }

                                                row.IntegerCells = IntegerCells.ToArray();
                                                row.BooleanCells = BooleanCells.ToArray();
                                                row.ByteCells = ByteCells.ToArray();
                                                row.DateTimeCells = DateTimeCells.ToArray();
                                                row.DecimalCells = DecimalCells.ToArray();
                                                row.DoubleCells = DoubleCells.ToArray();
                                                row.FloatCells = FloatCells.ToArray();
                                                row.GuidCells = GuidCells.ToArray();
                                                row.StringCells = StringCells.ToArray();

                                                ResultOfChangesResultPRoijPX3 chRes = client.SaveChanges(new UserToken
                                                {
                                                    Data = ((LitvaPortal.AuthenticationManager.UserToken)Session["AuthUser"]).Data
                                                }, t,

                                                      new Changeset
                                                       {
                                                         Id = new Guid(),
                                                         Actions = new LitvaPortal.ServiceReference_WebQuery.Action[]
                                                        {
                                                                new TypedRowCreationAction
                                                                {

                                                                    Id = Guid.NewGuid(),
                                                                    Type = ActionType.Create,
                                                                    Columns = dataSetCol,
                                                                    Row = row
                                                                }
                                                          },
                                                     });
                                            }
                                        }
                                    }
                                }
                            }
                        }


                      
                    }
                    int D_I = ConnectDB.NullI;

                    ClassIRPObject ir__ = (((List<ClassIRPObject>)Session["Irp"])).Find(r => r.Setting_param.NAME == TypeContainer && r.StatusObject.ToString() == SectionT);
                    if (ir__!=null) {
                    }
                    if (ir__.Val_Arr.Count == 0) {
                        string PathNameFinder_ = Request.QueryString["PathNameFinder"];
                    }
                }
                string NameItemPress = Request.QueryString["NameItemPress"].ToString();
                Response.Redirect(string.Format("~/Pages/MainForm.aspx?TypeContainer={0}&NameItemPress={1}&SectionT={2}", TypeContainer.ToString(), NameItemPress, SectionT));
            }
            
        }


        //To show message after performing operations
        public void Popup(bool isDisplay)
        {
            StringBuilder builder = new StringBuilder();
            if (isDisplay) {
                builder.Append("<script language=JavaScript> ShowPopup(); </script>\n");
                Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowPopup", builder.ToString());
            }
            else {
                builder.Append("<script language=JavaScript> HidePopup(); </script>\n");
                Page.ClientScript.RegisterStartupScript(this.GetType(), "HidePopup", builder.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void dgProviders_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            
            try{
                if (e.CommandName == "Approval") {
                    Session["isCreateNewStation"] = null;
                    int row = -1;
                    Curr_row = -1;
                    int.TryParse(e.CommandArgument as string, out row);
                    if (row != -1) {
                        if (((GridView)sender).PageIndex > 1){
                            row = row - (((GridView)sender).PageIndex) * (((GridView)sender).PageSize);
                        }
                        else if (((GridView)sender).PageIndex == 1) {
                            row = row - (((GridView)sender).PageSize);
                        }
                        GridViewRow gdrow = ((GridView)sender).Rows[row];
                        Session["Curr_Row"] = Convert.ChangeType(GridView.DataKeys[gdrow.RowIndex].Value, typeof(System.Int32));
                        InitPopupModal(false, (int)Session["Curr_Row"]);
                        Popup(true);
                    }
                }
                if (e.CommandName == "DeleteApproval") {
                    int row = -1;
                    Curr_row = -1;
                    int.TryParse(e.CommandArgument as string, out row);
                    if (row != -1) {
                        if (((GridView)sender).PageIndex > 1) {
                            row = row - (((GridView)sender).PageIndex) * (((GridView)sender).PageSize);
                        }
                        else if (((GridView)sender).PageIndex == 1) {
                            row = row - (((GridView)sender).PageSize);
                        }

                        
                        GridViewRow gdrow = ((GridView)sender).Rows[row];
                        object ID_S = Convert.ChangeType(GridView.DataKeys[gdrow.RowIndex].Value, typeof(System.Int32));
                        if (ID_S != null) {
                            Session["Curr_Row"] = ID_S;
                            string confirmValue = Request.Form["confirm_value_delete"];
                            if (confirmValue.Contains("Yes"))
                            {
                                string NTypeContainer = "";
                                string Cont = Request.QueryString["TypeContainer"].ToString();
                                List<ClassIRPObject> LIrp = (List<ClassIRPObject>)Session["Irp"];
                                List<ClassIRPObject> Lst = (List<ClassIRPObject>)Session["NavigationPages"];
                                string SectionT = "";
                                if (Request.QueryString["SectionT"] != null)
                                {
                                    SectionT = Request.QueryString["SectionT"].ToString();
                                }
                                ClassIRPObject ityu_ = Lst.Find(r => r.Setting_param.NAME == (string.IsNullOrEmpty(NTypeContainer) ? Cont : NTypeContainer) && r.StatusObject.ToString() == SectionT);
                                client = new WebQueryClient("BasicHttpBinding_IWebQuery");
                                ResultOfQueryGroupskoy_Sv8m5 rs = client.GetQueryGroups(new UserToken
                                {
                                    Data = ((LitvaPortal.AuthenticationManager.UserToken)Session["AuthUser"]).Data
                                });
                                if (rs.Data != null)
                                {

                                    foreach (QueryGroup nd in rs.Data.Groups)
                                    {
                                        if (nd.QueryTokens != null)
                                        {
                                            foreach (QueryToken t in nd.QueryTokens)
                                            {

                                                ResultOfQueryMetadatakoy_Sv8m5 meta = client.GetQueryMetadata(new UserToken
                                                {
                                                    Data = ((LitvaPortal.AuthenticationManager.UserToken)Session["AuthUser"]).Data
                                                }, t);



                                                if (meta.Data.Name == (string.IsNullOrEmpty(NTypeContainer) ? Cont : NTypeContainer))
                                                {


                                                    if (Session["Curr_Row"] != null)
                                                    {
                                                        ResultOfChangesResultPRoijPX3 chRes = client.SaveChanges(new UserToken
                                                        {
                                                            Data = ((LitvaPortal.AuthenticationManager.UserToken)Session["AuthUser"]).Data
                                                        }, t,

                                                        new Changeset
                                                        {
                                                            Id = new Guid(),
                                                            Actions = new LitvaPortal.ServiceReference_WebQuery.Action[]
                                                            {
                                                                new DeletionAction
                                                                {
                                                                    
                                                                    Id = Guid.NewGuid(),
                                                                    Type = ActionType.Delete,
                                                                    Condition = new ConditionExpression
                                                                    {
                                                                        Type = ConditionType.Expression,
                                                                        LeftOperand = new ColumnOperand{ ColumnName = "ID", Type = OperandType.Column },
                                                                        Operator = ConditionOperator.Equal,
                                                                        RightOperand = new IntegerValueOperand{ Value =  (int?)Session["Curr_Row"], Type = OperandType.Value, DataType= DataType.Integer },
                                                                    }

                                                                }
                                                            },

                                                        });
                                                    }

                                                    ///

                                                    string TypeContainer = "";
                                                    string NameItemPress = Request.QueryString["NameItemPress"].ToString();
                                                    SectionT = "";
                                                    if (Request.QueryString["SectionT"] != null)
                                                    {
                                                        SectionT = Request.QueryString["SectionT"].ToString();
                                                    }
                                                    if (Request.QueryString["TypeContainer"] != null)
                                                    {
                                                        TypeContainer = Request.QueryString["TypeContainer"].ToString();
                                                        if (!string.IsNullOrEmpty(TypeContainer))
                                                            Response.Redirect(string.Format("~/Pages/MainForm.aspx?TypeContainer={0}&NameItemPress={1}&SectionT={2}", TypeContainer.ToString(), NameItemPress, SectionT));
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
            
        }


        protected void ExportExcel_Click(object sender, EventArgs e)
        {
            ExportToExcel(1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ExportToCSV_Click(object sender, EventArgs e)
        {
            ExportToExcel(0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private string Win1251ToUTF8(string source)
        {
            Encoding utf8 = Encoding.UTF8;
            Encoding win1251 = Encoding.GetEncoding("windows-1251");
            byte[] utf8Bytes = win1251.GetBytes(source);
            byte[] win1251Bytes = Encoding.Convert(win1251, utf8, utf8Bytes);
            source = win1251.GetString(win1251Bytes);
            return source;
        }

        protected void SaveWebQuestion_Click(object sender, EventArgs e)
        {

        }


        protected void GridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            ClassIRPObject ityu_ = null;
            GridView.PageIndex = e.NewPageIndex;
            ClassMenu Icls = new ClassMenu(Session["SettingIRPClass"]);
            List<ClassIRPObject> Res_out = new List<ClassIRPObject>();
            int ID = -1;
            if (Request.QueryString["ID"] != null)  {
                int.TryParse(Request.QueryString["ID"].ToString(), out ID);
            }
            Session["dt"] = null; Session["sort"] = null;
            if (!((Request.QueryString["TypeContainer"] != null) && (Session["Search_container"] == null)))  {
                string NTypeContainer = "";
                string Cont = Request.QueryString["TypeContainer"].ToString();
                string NameItemPress = Request.QueryString["NameItemPress"].ToString();
                string SectionT = "";
                if (Request.QueryString["SectionT"] != null) {
                    SectionT = Request.QueryString["SectionT"].ToString();
                }
                List<ClassIRPObject> LIrp = (List<ClassIRPObject>)Session["Irp"];
                List<BlockDataFind> ir_ = new List<BlockDataFind>();
                if (Session["Search_container"] != null) {
                    ir_ = (List<BlockDataFind>)(Session["Search_container"]);
                }
                List<ClassIRPObject> Lst = (List<ClassIRPObject>)Session["NavigationPages"];
                ityu_ = Lst.Find(r => r.Setting_param.NAME == (string.IsNullOrEmpty(NTypeContainer) ? Cont : NTypeContainer) && r.StatusObject.ToString() == SectionT);
                //if ((Session["sort"] == null)) { Session["dt"] = Icls.GetRecords(ID, Session["AuthUser"].ToString(), ref GridView, Request.QueryString["TypeContainer"].ToString(), ir_, Session["StartIndex"] != null ? (int)Session["StartIndex"] : 0, out Res_out, Session["Irp"]); Session["sort"] = "Asc"; }
                if ((Session["sort"] == null)) { Session["dt"] = ityu_.GetRecords(ID, ref GridView, (string.IsNullOrEmpty(NTypeContainer) ? Cont : NTypeContainer), ir_, Session["StartIndex"] != null ? (int)Session["StartIndex"] : 0, out Res_out, Session["Irp"], NameItemPress, SectionT); Session["sort"] = "Asc"; }
                if (GridView.DataSource != null) TotalRecordsInQuery = ((DataTable)GridView.DataSource).AsEnumerable().Count() + (TotalRecordsInQuery < 0 ? TotalRecordsInQuery : 0);
                
            }
            else {
                string NTypeContainer = "";
                string Cont = Request.QueryString["TypeContainer"].ToString();
                string NameItemPress = Request.QueryString["NameItemPress"].ToString();
                string SectionT = "";
                if (Request.QueryString["SectionT"] != null) {
                    SectionT = Request.QueryString["SectionT"].ToString();
                }
                List<ClassIRPObject> LIrp = (List<ClassIRPObject>)Session["Irp"];
                List<BlockDataFind> ir_ = new List<BlockDataFind>();
                if (Session["Search_container"] != null) {
                    ir_ = (List<BlockDataFind>)(Session["Search_container"]);
                }
                List<ClassIRPObject> Lst = (List<ClassIRPObject>)Session["NavigationPages"];
                ityu_ = Lst.Find(r => r.Setting_param.NAME == (string.IsNullOrEmpty(NTypeContainer) ? Cont : NTypeContainer) && r.StatusObject.ToString() == SectionT);
                Session["dt"] = ityu_.GetRecords(ID, ref GridView, string.IsNullOrEmpty(NTypeContainer) ? Cont : NTypeContainer, null, Session["StartIndex"] != null ? (int)Session["StartIndex"] : 0, out Res_out, Session["Irp"], "", SectionT); Session["sort"] = "Asc";
                if (GridView.DataSource != null) TotalRecordsInQuery = ((DataTable)GridView.DataSource).AsEnumerable().Count() + (TotalRecordsInQuery < 0 ? TotalRecordsInQuery : 0);
            }
            if (Res_out != null)
            GridView.DataBind();
            if (ityu_ != null) { if (ityu_.Setting_param.index_group_field > 0) DataAdapterClass.GroupGridView(GridView.Rows, ityu_.Setting_param.index_group_field, ityu_.Setting_param.TOTAL_COUNT_GROUP); }
        }

        protected void ButtonDelete_Click(object sender, EventArgs e)
        {
              
        }
        /// <summary>
        /// isXLSs - (1-XLSX), (0-CSV)
        /// </summary>
        protected void ExportToExcel(int isXLSs)
        {
           
        }

        /// <summary>
        /// Export all records to XLS
        /// </summary>
        /// <param name="isXLSs"></param>
        protected void ExportAllToExcel(int isXLSs)
        {
           
        }

        protected void ButtonAllExcel_Click(object sender, EventArgs e)
        {
           
        }

        protected void ButtonAllCSV_Click(object sender, EventArgs e)
        {
           
        }


    }
}