using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Utils;
using LitvaPortal;
using LitvaPortal.Utils;
using OnlinePortal.Utils;

namespace OnlinePortal
{
    public partial class AcceptForm : System.Web.UI.Page
    {
        

        protected void Page_Load(object sender, EventArgs e)
        {
           
        }

       
        protected void Accept_Click(object sender, EventArgs e)
        {
            string confirmValue = Request.Form["confirm_value_agree"];
             if (confirmValue.Contains("Yes"))
             {
                 if (Request.QueryString["RVISID"] != null)
                 {
                     Session["RVIS_Ident"] = Request.QueryString["RVISID"];
                    //int ID_ = DataAdapterClass.GetData(string.Format("{0}?id={1}", DataAdapterClass.AuthService, Request.QueryString["RVISID"].ToString()), true);
                    int ID_ = -1;
                    if (ID_ > -1)
                     {
                         Session["AuthUser"] = ID_;
                         Response.Redirect("~/Pages/MainForm.aspx");
                     }
                     else
                     {
                         MessageBox.Show("Error authorization! Please, try to log in as Guest.");
                     }
                 }
                 else MessageBox.Show("Error authorization! Please, try to log in as Guest.");

             }

        }

        protected void Cancel_Click(object sender, EventArgs e)
        {

        }

  
    }
}