using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using DAL;

namespace OnlinePortal.Account
{
    public partial class Register : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            RegisterUser.ContinueDestinationPageUrl = Request.QueryString["ReturnUrl"];
        }


        protected void ButtonCreateUser_Click(object sender, EventArgs e)
        {
            
            
        }

        protected void RegisterUser_CreatedUser(object sender, EventArgs e)
        {
           
        }

    }
}
