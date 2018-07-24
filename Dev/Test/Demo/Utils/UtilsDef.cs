using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OnlinePortal;
using LitvaPortal;
using LitvaPortal.ServiceReference_WebQuery;



namespace Utils
{

    public class UtilsDef
    {
        
        /// <summary>
        /// Константные наименования итемов главного меню
        /// </summary>
        public static string MAIN_MENU_ROOT = "MainMenu";
        //public static string MAIN_MENU_URZP = "УРЗП";
        //public static string MAIN_MENU_URCP = "УРЧП";
        //public static string MAIN_MENU_FILIA = "Філія";
        //public static string MAIN_MENU_URCM= "УРЧМ";
        public Menu_ global_menu {get; set;}


        
        public UtilsDef(ref TreeView view, TreeNodeCollection nodeCollection, QueryGroup[] IRP_obj, UserToken token)
        {
            WebQueryClient client = new WebQueryClient("BasicHttpBinding_IWebQuery");
            TreeNode rootNode = AddNode(ref view, MAIN_MENU_ROOT, nodeCollection);
            global_menu = new Menu_();
            if (IRP_obj != null) {
                foreach (QueryGroup item in IRP_obj)
                {
                    global_menu.PUBLIC_VIEW = AddNode(ref view, item.Name, rootNode.ChildNodes);
                    if (item.QueryTokens != null) {
                        foreach (QueryToken it in item.QueryTokens)
                        {
                            ResultOfQueryMetadatakoy_Sv8m5 res = client.GetQueryMetadata(token, it);
                            AddNode(ref view, res.Data.Name, global_menu.PUBLIC_VIEW.ChildNodes);
                        }
                    }
                }
            }
        }

      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="view"></param>
        /// <param name="text"></param>
        /// <param name="nodeCollection"></param>
        /// <returns></returns>
        public TreeNode AddNode(ref TreeView view, string text,  TreeNodeCollection nodeCollection)
        {
            TreeNode tnode = new TreeNode();
            tnode.Text = text;
            tnode.SelectAction = TreeNodeSelectAction.Select;
            nodeCollection.Add(tnode);
            return tnode;
        }


    }

    /// <summary>
    /// Класс для хранения родительских узлов
    /// </summary>
    public sealed class Menu_
    {
        public TreeNode rootNode {get; set;}
        public List<TreeNode> Alloc { get; set; }
        public TreeNode PUBLIC_VIEW { get; set; }


        public Menu_()
        {
            rootNode = new TreeNode();
            Alloc = new List<TreeNode>();
        }
    }
    
}