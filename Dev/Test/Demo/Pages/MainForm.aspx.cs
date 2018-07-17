using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LitvaPortal;
using Utils;
using LitvaPortal.Utils;
using OnlinePortal.Utils;
using System.Text;
using System.Web.Services;
using System.Web.WebSockets;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using System.Web.ClientServices.Providers;
using System.Web.Caching;
using System.Web.ClientServices;
using System.Web.Globalization;
using System.Web.Handlers;
using System.ServiceModel;
using System.Net;
using System.IO;
using System.Windows;
using DAL;
using LitvaPortal.ServiceReference_WebQuery;

namespace OnlinePortal
{
    public partial class MainForm : System.Web.UI.Page
    {
        WebQueryClient client; 
        public ClassMenu intf_Lic { get; set; }
        public List<ClassIRPObject> Irp { get; set; }
        public List<SettingIRPClass> Lst_ { get; set; }

        public override void VerifyRenderingInServerForm(Control control)
        {
            
        }
        /// <summary>
        /// 
        /// </summary>
        protected void UpdateRangePage()
        {

            string NTypeContainer = "";
            string Type_Cont = Request.QueryString["TypeContainer"];
            string SectionT = "";
            if (Request.QueryString["SectionT"] != null)
            {
                SectionT = Request.QueryString["SectionT"].ToString();
            }

            if (Session["CurrentType"] == null) {
                Session["CurrentType"] = Type_Cont;
                Session["NavigationPages"] = Irp;
            }
            else if (Session["CurrentType"].ToString() != Type_Cont) {
                Session["NavigationPages"] = Irp;
                Session["CurrentType"] = Type_Cont;
            }
        }


        protected void Page_PreRender(object sender, EventArgs e)
        {
            TreeViewMenu.Attributes.Add("onclick", "alertVal()");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            

                client = new WebQueryClient("BasicHttpBinding_IWebQuery");
            
                string NTypeContainer = "";
                string SectionT = "";
                if (Request.QueryString["SectionT"] != null)
                {
                    SectionT = Request.QueryString["SectionT"].ToString();
                }
                if (Request.QueryString["TypeContainer"] != null) {
                    string Cont = Request.QueryString["TypeContainer"].ToString();
                    List<ClassIRPObject> LIrp = (List<ClassIRPObject>)Session["Irp"];
                    NTypeContainer = (string.IsNullOrEmpty(NTypeContainer) ? Cont : NTypeContainer);
                }


            if (Session["AuthUser"] != null)
            {
                string Type_Cont = Request.QueryString["TypeContainer"];
                if (!string.IsNullOrEmpty(NTypeContainer)) Type_Cont = NTypeContainer;
                if (Request.QueryString["PathNameFinder"] != null)
                {
                    PanelFinder.GroupingText = Request.QueryString["PathNameFinder"].ToString();
                }
                string TypeViewPage_ = Request.QueryString["TypeViewPage"];
                List<SettingIRPClass> Lst_Sett = new List<SettingIRPClass>();
                ResultOfQueryGroupskoy_Sv8m5 rs = client.GetQueryGroups(new UserToken
                {
                    Data = ((LitvaPortal.AuthenticationManager.UserToken)Session["AuthUser"]).Data
                });
                if (!Page.IsPostBack)
                {
                    if (string.IsNullOrEmpty(NTypeContainer))
                    {
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

                                        if (meta.Data.Name == NTypeContainer)
                                        {

                                            ClassIRPObject class_irp = new ClassIRPObject();
                                            string stat="";
                                            class_irp.Setting_param = new SettingIRPClass();
                                            class_irp.Setting_param.MAX_COLUMNS = meta.Data.Columns.Count();
                                            //class_irp.Setting_param.ExtendedControlRight = ExtendedControlRight.FullRight.ToString();
                                            class_irp.Setting_param.NAME = meta.Data.Name;
                                            //if (Enum.TryParse(SectionT, out stat))
                                                class_irp.Setting_param.STATUS_ = SectionT;
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
                                            //if (Enum.TryParse(SectionT, out stat))
                                                class_irp.StatusObject = SectionT;
                                            Irp.Add(class_irp);

                                            break;
                                        }


                                    }
                                }

                            }
                        }
                    }
                    else
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

                                        ResultOfQueryMetadatakoy_Sv8m5 meta = client.GetQueryMetadata(new UserToken
                                        {
                                            Data = ((LitvaPortal.AuthenticationManager.UserToken)Session["AuthUser"]).Data
                                        }, t);



                                        if (meta.Data.Name == NTypeContainer)
                                        {

                                            ClassIRPObject class_irp = new ClassIRPObject();
                                            string stat="";



                                            var res = client.ExecuteQuery(new UserToken
                                            {
                                                Data = ((LitvaPortal.AuthenticationManager.UserToken)Session["AuthUser"]).Data
                                            },
                                            t,
                                            new FetchOptions
                                            {
                                                // Limit = new DataLimit { Type = LimitValueType.Records, Value = 10 },
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

                                            class_irp.Setting_param = new SettingIRPClass();
                                            class_irp.Setting_param.NAME = meta.Data.Name;
                                            class_irp.Setting_param.MAX_COLUMNS = meta.Data.Columns.Count();
                                            //class_irp.Setting_param.ExtendedControlRight = ExtendedControlRight.FullRight.ToString();
                                            //if (Enum.TryParse(SectionT, out stat))
                                                class_irp.Setting_param.STATUS_ = SectionT;
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
                                            //if (Enum.TryParse(SectionT, out stat))
                                                class_irp.StatusObject = SectionT;
                                            Irp.Add(class_irp);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (Irp != null)
                    {
                        foreach (ClassIRPObject ir_set in Irp)
                        {
                            Lst_Sett.Add(ir_set.Setting_param);
                        }
                        Session["TYPE_CONTAINER_FINDER"] = new ClassMenu(Lst_Sett); //(Lst_);
                        Session["SettingIRPClass"] = Lst_Sett;// Lst_;
                        Session["Irp"] = Irp;
                    }

                    intf_Lic = new ClassMenu();
                    if (rs.Data.Groups != null) intf_Lic.GenerateMenu(ref TreeViewMenu, rs.Data.Groups, new UserToken
                    {
                        Data = ((LitvaPortal.AuthenticationManager.UserToken)Session["AuthUser"]).Data
                    });
                    UpdateRangePage();

                    TreeViewMenu.ExpandAll();
                    if (PanelMain.Visible == false)
                        PanelMain.Visible = true;
                }
            }

        }

        public bool GetStatusRequest()
        {
            bool isModify = true;
            return isModify;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool GetCommentsRequest()
        {
            bool isModify = false;
            string TypeContainer = "";
            string SectionT = "";
            if (Request.QueryString["SectionT"] != null) { SectionT = Request.QueryString["SectionT"].ToString();}
            if (Request.QueryString["TypeContainer"] != null) {
                TypeContainer = Request.QueryString["TypeContainer"].ToString();
                List<ClassIRPObject> Lst = (List<ClassIRPObject>)Session["Irp"];
                if (Lst != null) {
                    ClassIRPObject ir = Lst.Find(r => r.Setting_param.NAME == TypeContainer && r.StatusObject.ToString() == SectionT);
                    if (ir != null) {
                        if (!string.IsNullOrEmpty(ir.Setting_param.COMMENTS)) {
                            Comments_Request1.Visible = true;
                            Comments_Request2.Text = ir.Setting_param.COMMENTS;
                        }
                        else {
                            Comments_Request1.Visible = false;
                        }
                    }
                }
            }
            return isModify;
        }




        private TreeNode FindNode(TreeNode treenode, string name)
        {
            foreach (TreeNode tn in treenode.ChildNodes) {
                if (tn.Text == name) { return tn; }  
            }
            TreeNode node;
            foreach (TreeNode tn in treenode.ChildNodes) {
                node = FindNode(tn, name);
                if (node != null) { return node; } 
            }
            return null;
        }

        
        private TreeNode FindNode(TreeView tv, string name)
        {
            foreach (TreeNode tn in tv.Nodes) {
                if (tn.Text == name) { return tn; } 
            }
            TreeNode node;
            foreach (TreeNode tn in tv.Nodes) {
                node = FindNode(tn, name);
                if (node != null) { return node; } 
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Type_Cont"></param>
        /// <returns></returns>
        public string GetStructTreeView(string Type_Cont)
        {
            string Struct = "Область пошуку: ";
            List<string> LstItemTreeView = new List<string>();
            TreeNode node = TreeViewMenu.SelectedNode;
            while (node.Parent != null) {
                node = node.Parent;
                LstItemTreeView.Add(node.Text);
            }
            LstItemTreeView.Sort();
            foreach (string item in LstItemTreeView) {
                Struct += item + @"\";
            }
            Struct += Type_Cont;
            return Struct;
        }
        
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public KeyValuePair<string, string> GetParentItem(string Type_Cont)
        {
            KeyValuePair<string, string> ParentItem = new KeyValuePair<string,string>();
            string FindParentItem = "";
            List<string> LstItemTreeView = new List<string>();
            TreeNode node = TreeViewMenu.SelectedNode;
            string ParentName = "";
            while (node.Parent != null) {
                ParentName = node.Text;
                node = node.Parent;
                LstItemTreeView.Add(node.Text);
            }
            if (LstItemTreeView.Count() > 0) FindParentItem = LstItemTreeView[0];
            FindParentItem = Type_Cont;
            ParentItem = new KeyValuePair<string, string>(FindParentItem, ParentName);
            //if (LstItemTreeView.Contains(UtilsDef.MAIN_MENU_URCM))
            //ParentItem = new KeyValuePair<string, TypeStatus>(FindParentItem, TypeStatus.URCM);
            //else if (LstItemTreeView.Contains(UtilsDef.MAIN_MENU_URCP))
            //ParentItem = new KeyValuePair<string, TypeStatus>(FindParentItem, TypeStatus.URCP);
            //else if (LstItemTreeView.Contains(UtilsDef.MAIN_MENU_URZP))
            //ParentItem = new KeyValuePair<string, TypeStatus>(FindParentItem, TypeStatus.URZP);
            //else if (LstItemTreeView.Contains(UtilsDef.MAIN_MENU_FILIA))
            //ParentItem = new KeyValuePair<string, TypeStatus>(FindParentItem, TypeStatus.FILIA);
            return ParentItem;
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void TreeViewMenu_SelectedNodeChanged(object sender, EventArgs e)
        {
             Session["sort"] = null;
             Session["Search_container"] = null;
             string Type_Cont = TreeViewMenu.SelectedNode.Value.ToString();
             KeyValuePair<string, string> Type_C = new KeyValuePair<string, string>();
             string NameItemPress = Type_Cont;
             Session["TYPE_CONTAINER_FINDER"] = new ClassMenu(Session["SettingIRPClass"]);
             string StructTreeView = GetStructTreeView(Type_Cont);
             Type_C = GetParentItem(Type_Cont);
             Session["lastNode"] = TreeViewMenu.SelectedNode;
             Response.Redirect(string.Format("~/Pages/MainForm.aspx?TypeContainer={0}&PathNameFinder={1}&NameItemPress={2}&SectionT={3}", Type_C.Key.ToString(), StructTreeView, NameItemPress, Type_C.Value.ToString()));
           
        }


        protected void BtnCreateStation_Click(object sender, EventArgs e)
        {

            Session["Curr_Row"] = null;
            Session["sort"] = null;
            Session["Search_container"] = null;
            Session["isCreateNewStation"] = true;
            StringBuilder builder = new StringBuilder();
            builder.Append("<script language=JavaScript> ShowPopup(); </script>\n");
            Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowPopup", builder.ToString());

       }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            
        }


        protected void TestRequestService()
        {

        }

        protected void MainTable1_PreRender(object sender, EventArgs e)
        {
            
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            
        }


    }

    /// <summary>
    /// 
    /// </summary>
    public enum TypeViewPage
    {
        First,
        Prev,
        Next
    }


}