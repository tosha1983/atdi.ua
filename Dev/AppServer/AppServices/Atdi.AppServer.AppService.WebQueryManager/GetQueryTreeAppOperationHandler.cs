using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppServer.Models.AppServices;
using Atdi.AppServer.Models.AppServices.WebQueryManager;
using Atdi.AppServer.Contracts.WebQuery;
using Atdi.AppServer.AppService.WebQueryDataDriver.ICSMUtilities;
using Atdi.AppServer.AppService.WebQueryDataDriver;


namespace Atdi.AppServer.AppServices.WebQueryManager
{
    public class GetQueryTreeAppOperationHandler
        : AppOperationHandlerBase
        <
            WebQueryManagerAppService,
            WebQueryManagerAppService.GetQueryTreeAppOperation,
            GetQueryTreeAppOperationOptions,
            QueryTree
        >
    {
        public GetQueryTreeAppOperationHandler(IAppServerContext serverContext, ILogger logger) : base(serverContext, logger)
        {
        }

        public override QueryTree Handle(GetQueryTreeAppOperationOptions options, IAppOperationContext operationContext)
        {
            QueryTree QTree = new QueryTree();
            try {
                UtilsDef Menu = new UtilsDef(ref QTree, options.OtherArgs.UserId);
                Logger.Trace(this, options, operationContext);
            }
            catch (Exception ex) { Logger.Error(ex); }
            return QTree;
        }
    }


    public class UtilsDef
    {

        public static string MAIN_MENU_ROOT = "MainMenu";
        public static string MAIN_MENU_PUBLIC_VIEW_SPECTRUM_MANAGEMENT = "Spectrum management";
        public static string MAIN_MENU_AUTH_USER_CUST_STATION_LIST = "User stations List";
        public static string MAIN_MENU_AUTH_USER_DELEG_STATION_LIST = "Delegated stations list";
        public static string MAIN_MENU_AUTH_USER_LT1 = "Simplified registration of stations";
        public static string MAIN_MENU_AUTH_USER_LT2 = "Hardware registration";
        public static string MAIN_MENU_AUTH_USER_ISV = "Public authorities";
        public static string MAIN_MENU_AUTH_HEALTH_CARE = "Calculations according to hygiene norms";
        public static string LINK_URL_ITEM = "";
        public UtilsDef(ref QueryTree view, int ID_USER)
        {
            Class_ManageSettingInstance v_s = new Class_ManageSettingInstance();
            List<SettingIRPClass> Las_NameCat = Class_ManageSettingInstance.GetSettingWebQuery("XWEB_QUERY");
            string Role = v_s.GetRoleUser(ID_USER != ConnectDB.NullI ? ID_USER : -1) != "" ? v_s.GetRoleUser(ID_USER != ConnectDB.NullI ? ID_USER : -1) : RolesUsers.Guest.ToString();
            view.Root = new QueryTreeNode();
            view.Style = new QueryTreeStyle();
            view.Root.Name = MAIN_MENU_ROOT;
            view.Root.Description = "";
            view.Root.Title = MAIN_MENU_ROOT;


            List<QueryTreeNode> L_Categ = new List<QueryTreeNode>();
            QueryTreeNode Tnode_PUBLIC_VIEW_SPECTRUM_MANAGEMENT = AddCategoryNode(ref view, MAIN_MENU_PUBLIC_VIEW_SPECTRUM_MANAGEMENT, MAIN_MENU_PUBLIC_VIEW_SPECTRUM_MANAGEMENT, ""); L_Categ.Add(Tnode_PUBLIC_VIEW_SPECTRUM_MANAGEMENT);
            QueryTreeNode Tnode_USER_CUST_STATION_LIST = AddCategoryNode(ref view, MAIN_MENU_AUTH_USER_CUST_STATION_LIST, MAIN_MENU_AUTH_USER_CUST_STATION_LIST, ""); L_Categ.Add(Tnode_USER_CUST_STATION_LIST);
            QueryTreeNode Tnode_USER_DELEG_STATION_LIST = AddCategoryNode(ref view, MAIN_MENU_AUTH_USER_DELEG_STATION_LIST, MAIN_MENU_AUTH_USER_DELEG_STATION_LIST, ""); L_Categ.Add(Tnode_USER_DELEG_STATION_LIST);
            QueryTreeNode Tnode_CUST_STATION_LIST1 = AddCategoryNode(ref view, MAIN_MENU_AUTH_USER_LT1, MAIN_MENU_AUTH_USER_LT1, ""); L_Categ.Add(Tnode_CUST_STATION_LIST1);
            QueryTreeNode Tnode_CUST_STATION_LIST2 = AddCategoryNode(ref view, MAIN_MENU_AUTH_USER_LT2, MAIN_MENU_AUTH_USER_LT2, ""); L_Categ.Add(Tnode_CUST_STATION_LIST2);
            QueryTreeNode Tnode_AUTH_USER_ISV = AddCategoryNode(ref view, MAIN_MENU_AUTH_USER_ISV, MAIN_MENU_AUTH_USER_ISV, ""); L_Categ.Add(Tnode_AUTH_USER_ISV);
            QueryTreeNode Tnode_AUTH_HEALTH_CARE = AddCategoryNode(ref view, MAIN_MENU_AUTH_HEALTH_CARE, MAIN_MENU_AUTH_HEALTH_CARE, ""); L_Categ.Add(Tnode_AUTH_HEALTH_CARE);


            if (Las_NameCat != null) {
                foreach (SettingIRPClass item in Las_NameCat) {
                    if ((item.IS_TESTING_REQUEST) && (RolesUsers.Admin.ToString() != Role)) continue;
                    if ((RolesUsers.Admin.ToString() == Role) || (RolesUsers.Provider.ToString() == Role) || (RolesUsers.Provider_Government.ToString() == Role) || (RolesUsers.Provider_Healthcare.ToString() == Role)) {
                        if (TypeStatus.LT1 == item.STATUS_) {
                            AddNode(ref view, item.STATUS_.ToString(), item.ID, item.NAME, item.NAME, item.DESCRIPTION, ref Tnode_CUST_STATION_LIST1.ChildNodes);
                        }
                    }
                    if ((RolesUsers.Admin.ToString() == Role) || (RolesUsers.Provider.ToString() == Role) || (RolesUsers.Provider_Government.ToString() == Role) || (RolesUsers.Provider_Healthcare.ToString() == Role)) {
                        if (TypeStatus.LT2 == item.STATUS_) {
                            AddNode(ref view, item.STATUS_.ToString(), item.ID, item.NAME, item.NAME, item.DESCRIPTION, ref Tnode_CUST_STATION_LIST2.ChildNodes);
                        }
                    }

                    if ((RolesUsers.Admin.ToString() == Role) || (RolesUsers.Provider.ToString() == Role) || (RolesUsers.Government.ToString() == Role) || (RolesUsers.HealthCare.ToString() == Role) || (RolesUsers.Guest.ToString() == Role) || (RolesUsers.Government_Healthcare.ToString() == Role) || (RolesUsers.Provider_Government.ToString() == Role) || (RolesUsers.Provider_Healthcare.ToString() == Role)) {
                        if (TypeStatus.PUB == item.STATUS_) {
                            AddNode(ref view, item.STATUS_.ToString(), item.ID, item.NAME, item.NAME, item.DESCRIPTION, ref Tnode_PUBLIC_VIEW_SPECTRUM_MANAGEMENT.ChildNodes);
                        }
                    }

                    if ((RolesUsers.Admin.ToString() == Role) || (RolesUsers.Provider.ToString() == Role) || (RolesUsers.Provider_Government.ToString() == Role) || (RolesUsers.Provider_Healthcare.ToString() == Role)) {
                        if (TypeStatus.CUS == item.STATUS_) {
                            AddNode(ref view, item.STATUS_.ToString(), item.ID, item.NAME, item.NAME, item.DESCRIPTION, ref Tnode_USER_CUST_STATION_LIST.ChildNodes);
                        }
                    }

                    if ((RolesUsers.Admin.ToString() == Role) || (RolesUsers.Provider.ToString() == Role) || (RolesUsers.Provider_Government.ToString() == Role) || (RolesUsers.Provider_Healthcare.ToString() == Role)) {
                        if (TypeStatus.DSL == item.STATUS_) {
                            AddNode(ref view, item.STATUS_.ToString(), item.ID, item.NAME, item.NAME, item.DESCRIPTION, ref Tnode_USER_DELEG_STATION_LIST.ChildNodes);
                        }
                    }

                    if ((RolesUsers.Admin.ToString() == Role) || (RolesUsers.Government.ToString() == Role) || (RolesUsers.Provider_Government.ToString() == Role) || (RolesUsers.Government_Healthcare.ToString() == Role)) {
                        if (TypeStatus.ISV == item.STATUS_) {
                            AddNode(ref view, item.STATUS_.ToString(), item.ID, item.NAME, item.NAME, item.DESCRIPTION, ref Tnode_AUTH_USER_ISV.ChildNodes);
                        }
                    }

                    if ((RolesUsers.Admin.ToString() == Role) || (RolesUsers.HealthCare.ToString() == Role) || (RolesUsers.Provider_Healthcare.ToString() == Role) || (RolesUsers.Government_Healthcare.ToString() == Role)) {
                        if (TypeStatus.HCC == item.STATUS_) {
                            AddNode(ref view, item.STATUS_.ToString(), item.ID, item.NAME, item.NAME, item.DESCRIPTION, ref Tnode_AUTH_HEALTH_CARE.ChildNodes);
                        }
                    }
                }
            }
            view.Root.ChildNodes = L_Categ.ToArray();
        }



        public QueryTreeNode AddCategoryNode(ref QueryTree view, string Name, string Title, string Description)
        {
            QueryTreeNode tnode = new QueryTreeNode();
            tnode.QueryRef = new QueryReference();
            tnode.Description = Description;
            tnode.Name = Name;
            tnode.Title = Title;
            tnode.ChildNodes = null;
            return tnode;
        }


        public QueryTreeNode AddNode(ref QueryTree view, string QReference_Version, int QReference_ID, string Name, string Title, string Description, ref QueryTreeNode[] childNodes)
        {
            QueryTreeNode tnode = new QueryTreeNode();
            tnode.QueryRef = new QueryReference();
            tnode.QueryRef.Id = QReference_ID;
            tnode.QueryRef.Version = QReference_Version;
            tnode.Description = Description;
            tnode.Name = Name;
            tnode.Title = Title;
            
            if (childNodes == null) {
                List<QueryTreeNode> L_ = new List<QueryTreeNode>();
                L_.Add(tnode);
                childNodes = L_.ToArray();
            }
            else
            {
                List<QueryTreeNode> L_ = childNodes.ToList();
                L_.Add(tnode);
                childNodes = L_.ToArray();
            }
            return tnode;
        }

    }


}
