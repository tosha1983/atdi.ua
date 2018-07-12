using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Utils;
using LitvaPortal;


namespace OnlinePortal
{
    public partial class ErrorForm : System.Web.UI.Page
    {
        public UtilsDef gen_menu { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["AuthUser"] != null)
            {

            }
        }

        protected void TreeViewMenu_SelectedNodeChanged(object sender, EventArgs e)
        {
           
        }

  
    }
}