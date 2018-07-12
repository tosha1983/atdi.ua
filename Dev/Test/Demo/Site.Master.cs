using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LitvaPortal;
using LitvaPortal.Utils;
using OnlinePortal.Utils;

namespace OnlinePortal
{ 
    
    public partial class SiteMaster : System.Web.UI.MasterPage
    {

        
        protected override void OnInit(EventArgs e)
        {
           
        }
        

        public bool CheckForSessionTimeout()
        {
            if (Context.Session != null && Context.Session.IsNewSession) {
                string cookieHeader = Page.Request.Headers["Cookie"];
                if ((null != cookieHeader) && (cookieHeader.IndexOf("ASP.NET_SessionId") >= 0))
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            if (Page.IsPostBack) {
                Session["Abandon"] = true;
            }
            else {
                if (Request.QueryString["RVISID"] != null) {
                    Session["RVIS_Ident"] = Request.QueryString["RVISID"];
                    //int ID_ = DataAdapterClass.GetData(string.Format("{0}?id={1}", DataAdapterClass.AuthService, Request.QueryString["RVISID"].ToString()), true);
                    int ID_ = -1;
                    if (ID_ > -1) {
                        Session["AuthUser"] = ID_;
                        Response.Redirect("~/Pages/MainForm.aspx");
                    }
                    else {
                        Session["AuthUser"] = null;
                        Session["AuthUser"] = ID_;
                        Response.Redirect("~/Pages/MainForm.aspx");
                    }
                }
                else {
                    if (Session["Abandon"] == null) {
                        if (Session["AuthUser"] == null) {
                            Session["AuthUser"] = null;
                        }
                    }
                }


                if (Session["AuthUser"] == null) {
                    if (Session["Abandon"] == null) {
                        Session["AuthUser"] = -1;
                        //Response.Redirect("~/Pages/MainForm.aspx");
                        string TypeContainer = "";
                        string NameItemPress = "";
                        if (Request.QueryString["NameItemPress"] != null) {
                            NameItemPress = Request.QueryString["NameItemPress"].ToString();
                        }
                        string SectionT = "";
                        if (Request.QueryString["SectionT"] != null) {
                            SectionT = Request.QueryString["SectionT"].ToString();
                        }
                        if (Request.QueryString["TypeContainer"] != null) {
                            TypeContainer = Request.QueryString["TypeContainer"].ToString();
                            if (!string.IsNullOrEmpty(TypeContainer))
                                Response.Redirect(string.Format("~/Pages/MainForm.aspx?TypeContainer={0}&NameItemPress={1}&SectionT={2}", TypeContainer.ToString(), NameItemPress, SectionT));
                        }
                        else if ((NameItemPress=="") && (TypeContainer=="") && (SectionT=="")) {
                            //Response.Redirect("~/Pages/MainForm.aspx");
                            Response.Redirect("~/Account/Login.aspx");
                        }
                    }
                    else {
                        Session["AuthUser"] = null;
                        Session.Abandon();
                    }
               }
            }

            if (CheckForSessionTimeout()) {
                if (Page.Request.IsAuthenticated) {
                    Session["AuthUser"] = null; Session["RVIS_Ident"] = null;  Session["NavigationPages"] = null;
                    Session["AuthUser"] = null; Session["StartIndex"] = null;  Session["EndIndex"] = null;
                    Session["Curr_Row"] = null; Session["sort"] = null; Session["Search_container"] = null;
                    Session["isCreateNewStation"] = null;  Session["TYPE_CONTAINER_FINDER"] = null;   Session["SettingIRPClass"] = null;
                    Session["Irp"] = null;
                    Response.Redirect("~/Account/Login.aspx");
                }
            }

        }

        protected void HyperLink1_Load(object sender, EventArgs e)
        {
            
        }

        protected void HyperLink1_Init(object sender, EventArgs e)
        {
            
        }



        protected void ButtonLogin_Click(object sender, EventArgs e)
        {
            if ((Session["AuthUser"] == null)) {
                Session["RVIS_Ident"] = null;  Session["NavigationPages"] = null;  Session["AuthUser"] = null;
                Session["StartIndex"] = null;  Session["EndIndex"] = null;         Session["Curr_Row"] = null;
                Session["sort"] = null;        Session["Search_container"] = null; Session["isCreateNewStation"] = null;
                Session["TYPE_CONTAINER_FINDER"] = null;  Session["SettingIRPClass"] = null;  Session["Irp"] = null;
                Response.Redirect("~/Account/Login.aspx");

            }
            else  {
                Session["RVIS_Ident"] = null;  Session["NavigationPages"] = null;  Session["AuthUser"] = null;
                Session["StartIndex"] = null;  Session["EndIndex"] = null;         Session["Curr_Row"] = null;
                Session["sort"] = null;        Session["Search_container"] = null; Session["isCreateNewStation"] = null;
                Session["TYPE_CONTAINER_FINDER"] =  null; Session["SettingIRPClass"] = null;   Session["Irp"] = null;
                Session.Clear(); Session["Abandon"] = true;
                Response.Redirect("~//");
                
                
            }
        
        }

        public void UpdatePanel1_Load(object sender, EventArgs e)
        {
            if (Session["AuthUser"] != null) {
                ButtonLogin.Text = "Išeiti";
            }
            else {
                ButtonLogin.Text = "Prisijungti";
            }
            
        }

        protected void ButtonHyperLinkGuest_Click(object sender, EventArgs e)
        {

        }

        protected void HyperLinkGuest_CheckedChanged(object sender, EventArgs e)
        {
          
        }
    }
}
