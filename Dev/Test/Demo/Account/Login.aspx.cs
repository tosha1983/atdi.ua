using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DAL;
using OnlinePortal;
using LitvaPortal.ServiceReference_WebQuery;
using LitvaPortal.AuthenticationManager;

namespace OnlinePortal.Account
{
    public partial class Login : System.Web.UI.Page 
    {
        AuthenticationManagerClient AuthClient;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void LoginUser_Authenticate(object sender, AuthenticateEventArgs e)
        {
            WebQueryClient client = new WebQueryClient("BasicHttpBinding_IWebQuery");
            AuthClient = new AuthenticationManagerClient("BasicHttpBinding_IAuthenticationManager");
            UserCredential credential = new UserCredential();
            credential.UserName = LoginUser.UserName;
            credential.Password = this.LoginUser.Password;
            ResultOfUserIdentityPRoijPX3 comm = AuthClient.AuthenticateUser(credential);
            int ID = comm.Data.Id;
            if (ID > 0) {
                Session["AuthUser"] = comm.Data.UserToken;
                Response.Redirect("~/Pages/MainForm.aspx");
            }
            else {
                MessageBox.Show("Error authorization. Please input correct parameters authorization!");
            }
        }


    }
}
