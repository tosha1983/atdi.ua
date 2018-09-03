using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DAL;
using Utils;
using LitvaPortal;


namespace OnlinePortal
{
    public partial class _Default : System.Web.UI.Page
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["AuthUser"] != null)
            {
                PanelMenu.Visible = true;
                PanelWelcome.Visible = false;
                if (!Page.IsPostBack)
                {
                    
                }
            }
            else
            {
                PanelMenu.Visible = false;
                PanelWelcome.Visible = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void TreeViewMenu_SelectedNodeChanged(object sender, EventArgs e)
        {
            string Type_Cont = TreeViewMenu.SelectedNode.Value.ToString();
            Session["TYPE_CONTAINER_FINDER"] = new ClassMenu(Session["SettingIRPClass"]);
            Response.Redirect(string.Format("~/Pages/MainForm.aspx?TypeContainer={0}&User={1}", Type_Cont.ToString(), Session["AuthUser"].ToString()));
        }




    }

    /// <summary>
    /// 
    /// </summary>
    public class ItemMenu
    {
        public string ID { get; set; }
        public string NameItem { get; set;  }
        public string URL { get; set; }
    }
}
