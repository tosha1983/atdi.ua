using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using OnlinePortal;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using OnlinePortal.Utils;
using LitvaPortal.Utils;
using Utils;
using DAL;
using LitvaPortal.ServiceReference_WebQuery;

namespace LitvaPortal
{


    public class ClassMenu
    {
        public List<SettingIRPClass> Lst_ { get; set; }
        public List<ItemMenu> Menu_List { get; set; }


        public ClassMenu()
        {

        }

        public ClassMenu(dynamic Param)
        {
            if (Param is List<SettingIRPClass>)
            {
                Lst_ = new List<SettingIRPClass>();
                if (Param!=null) Lst_ = (List<SettingIRPClass>)Param;
            }

            Menu_List = new List<ItemMenu>();
        }

        public void GenerateMenu(ref TreeView view, QueryGroup[] IRP_obj, UserToken token)
        {
            UtilsDef gen_menu = new UtilsDef(ref view, view.Nodes, IRP_obj, token);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cls"></param>
        /// <returns></returns>
        public DataTable SearchValues(dynamic cls)
        {
            DataTable dt = new DataTable();
            return dt;
        }
      

    }
}