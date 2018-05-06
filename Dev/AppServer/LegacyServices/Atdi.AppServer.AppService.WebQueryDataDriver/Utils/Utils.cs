using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Configuration;
using System.Runtime.Serialization;
using System.Data;
using System.Collections;
using System.Security.Cryptography;
using System.Globalization;
using System.Windows.Forms;
using Atdi.AppServer.AppService.WebQueryDataDriver;
using DatalayerCs;
using OrmCs;
using System.Runtime.Caching;
using Atdi.AppServer.Contracts.WebQuery;



namespace Atdi.AppServer.AppService.WebQueryDataDriver.ICSMUtilities
{
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

            StockItems PS = new StockItems();
            List<SettingIRPClass> Las_NameCat = (List<SettingIRPClass>)PS.GetAvailableStocksSettingIRP(true);
            PS.GetCacheKeyMetaData(true, ID_USER);

            Class_ManageSettingInstance v_s = new Class_ManageSettingInstance();
            string Role = v_s.GetRoleUser(ID_USER != null ? ID_USER : -1) != "" ? v_s.GetRoleUser(ID_USER != null ? ID_USER : -1) : RolesUsers.Guest.ToString();
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


            if (Las_NameCat != null)
            {
                foreach (SettingIRPClass item in Las_NameCat)
                {
                    if ((item.IS_TESTING_REQUEST) && (RolesUsers.Admin.ToString() != Role)) continue;
                    if ((RolesUsers.Admin.ToString() == Role) || (RolesUsers.Provider.ToString() == Role) || (RolesUsers.Provider_Government.ToString() == Role) || (RolesUsers.Provider_Healthcare.ToString() == Role))
                    {
                        if (TypeStatus.LT1 == item.STATUS_)
                        {
                            AddNode(ref view, item.STATUS_.ToString(), item.ID, item.NAME, item.NAME, item.DESCRIPTION, ref Tnode_CUST_STATION_LIST1.ChildNodes);
                        }
                    }
                    if ((RolesUsers.Admin.ToString() == Role) || (RolesUsers.Provider.ToString() == Role) || (RolesUsers.Provider_Government.ToString() == Role) || (RolesUsers.Provider_Healthcare.ToString() == Role))
                    {
                        if (TypeStatus.LT2 == item.STATUS_)
                        {
                            AddNode(ref view, item.STATUS_.ToString(), item.ID, item.NAME, item.NAME, item.DESCRIPTION, ref Tnode_CUST_STATION_LIST2.ChildNodes);
                        }
                    }
                    if ((RolesUsers.Admin.ToString() == Role) || (RolesUsers.Provider.ToString() == Role) || (RolesUsers.Government.ToString() == Role) || (RolesUsers.HealthCare.ToString() == Role) || (RolesUsers.Guest.ToString() == Role) || (RolesUsers.Government_Healthcare.ToString() == Role) || (RolesUsers.Provider_Government.ToString() == Role) || (RolesUsers.Provider_Healthcare.ToString() == Role))
                    {
                        if (TypeStatus.PUB == item.STATUS_)
                        {
                            AddNode(ref view, item.STATUS_.ToString(), item.ID, item.NAME, item.NAME, item.DESCRIPTION, ref Tnode_PUBLIC_VIEW_SPECTRUM_MANAGEMENT.ChildNodes);
                        }
                    }
                    if ((RolesUsers.Admin.ToString() == Role) || (RolesUsers.Provider.ToString() == Role) || (RolesUsers.Provider_Government.ToString() == Role) || (RolesUsers.Provider_Healthcare.ToString() == Role))
                    {
                        if (TypeStatus.CUS == item.STATUS_)
                        {
                            AddNode(ref view, item.STATUS_.ToString(), item.ID, item.NAME, item.NAME, item.DESCRIPTION, ref Tnode_USER_CUST_STATION_LIST.ChildNodes);
                        }
                    }
                    if ((RolesUsers.Admin.ToString() == Role) || (RolesUsers.Provider.ToString() == Role) || (RolesUsers.Provider_Government.ToString() == Role) || (RolesUsers.Provider_Healthcare.ToString() == Role))
                    {
                        if (TypeStatus.DSL == item.STATUS_)
                        {
                            AddNode(ref view, item.STATUS_.ToString(), item.ID, item.NAME, item.NAME, item.DESCRIPTION, ref Tnode_USER_DELEG_STATION_LIST.ChildNodes);
                        }
                    }
                    if ((RolesUsers.Admin.ToString() == Role) || (RolesUsers.Government.ToString() == Role) || (RolesUsers.Provider_Government.ToString() == Role) || (RolesUsers.Government_Healthcare.ToString() == Role))
                    {
                        if (TypeStatus.ISV == item.STATUS_)
                        {
                            AddNode(ref view, item.STATUS_.ToString(), item.ID, item.NAME, item.NAME, item.DESCRIPTION, ref Tnode_AUTH_USER_ISV.ChildNodes);
                        }
                    }
                    if ((RolesUsers.Admin.ToString() == Role) || (RolesUsers.HealthCare.ToString() == Role) || (RolesUsers.Provider_Healthcare.ToString() == Role) || (RolesUsers.Government_Healthcare.ToString() == Role))
                    {
                        if (TypeStatus.HCC == item.STATUS_)
                        {
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
            if (childNodes == null)
            {
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


    static public class ICSMUtils
    {
        /// <summary>
        /// Генерация хэш-функции по алгоритму SHA-256
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        static public string SHA256(string input)
        {
            byte[] inputbytes = UTF8Encoding.UTF8.GetBytes(input);
            SHA256 sha = new SHA256CryptoServiceProvider();
            byte[] hash = sha.ComputeHash(inputbytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++) sb.Append(hash[i].ToString("X2"));
            return sb.ToString();
        }

        static public void UpdateCacheObjects()
        {
            ObjectCache cache = MemoryCache.Default;
            List<SettingIRPClass> Ls_NameCat = cache["SettingIRPClass"] as List<SettingIRPClass>;
            List<QueryMetaD> Ls_QueryMetaD = cache["QueryMetaD"] as List<QueryMetaD>;
            CacheItemPolicy policy = new CacheItemPolicy();
        }
    }

    public class StockItems
    {
        private const string CacheKeySettingIRP = "SettingIRPClass";
        private const string CacheKeyMetaData = "GetQueryMetaData";
        public IEnumerable GetAvailableStocksSettingIRP(bool withAppend)
        {
            ObjectCache cache = MemoryCache.Default;
            if (cache.Contains(CacheKeySettingIRP)) {
                if (!withAppend)
                    return (IEnumerable)cache.Get(CacheKeySettingIRP);
                else {
                    IEnumerable availableStocks = Class_ManageSettingInstance.GetSettingWebQuery("XWEB_QUERY");
                    IEnumerable Res_cache = (IEnumerable)cache.Get(CacheKeySettingIRP);
                    if (Res_cache != null) {
                        foreach (SettingIRPClass R in availableStocks) {
                            if ((Res_cache as List<SettingIRPClass>).Find(t => t.ID == R.ID) == null)
                            {
                                CacheItemPolicy cacheItemPolicy = new CacheItemPolicy();
                                cache.Add(CacheKeySettingIRP, availableStocks, cacheItemPolicy);
                            }
                        }
                    }
                    else {
                        CacheItemPolicy cacheItemPolicy = new CacheItemPolicy();
                        cache.Add(CacheKeySettingIRP, availableStocks, cacheItemPolicy);
                    }
                    return availableStocks;
                }
            }
            else {
                if (!withAppend) {
                    IEnumerable availableStocks = Class_ManageSettingInstance.GetSettingWebQuery("XWEB_QUERY");
                    CacheItemPolicy cacheItemPolicy = new CacheItemPolicy();
                    cache.Add(CacheKeySettingIRP, availableStocks, cacheItemPolicy);
                    return availableStocks;
                }
                else {
                    IEnumerable availableStocks = Class_ManageSettingInstance.GetSettingWebQuery("XWEB_QUERY");
                    IEnumerable Res_cache = (IEnumerable)cache.Get(CacheKeySettingIRP);
                    if (Res_cache != null) {
                        foreach (SettingIRPClass R in availableStocks) {
                            if ((Res_cache as List<SettingIRPClass>).Find(t => t.ID == R.ID) == null){
                                CacheItemPolicy cacheItemPolicy = new CacheItemPolicy();
                                cache.Add(CacheKeySettingIRP, availableStocks, cacheItemPolicy);
                            }
                        }
                    }
                    else {
                        CacheItemPolicy cacheItemPolicy = new CacheItemPolicy();
                        cache.Add(CacheKeySettingIRP, availableStocks, cacheItemPolicy);
                    }
                    return availableStocks;
                }
            }
        }

        public IEnumerable GetCacheKeyMetaData(bool withAppend,  int UserId)
        {
            ObjectCache cache = MemoryCache.Default;
            if (cache.Contains(CacheKeyMetaData)){
                if (!withAppend)
                    return (IEnumerable)cache.Get(CacheKeyMetaData);
                else {
                    List<QueryMetaD> QDL = new List<QueryMetaD>();
                    Class_ManageSettingInstance v_s = new Class_ManageSettingInstance();
                    foreach (SettingIRPClass st in (List<SettingIRPClass>)GetAvailableStocksSettingIRP(false)) {
                        QueryMetaD availableStocks = v_s.GetQueryMetaData((List<SettingIRPClass>)GetAvailableStocksSettingIRP(false), st.ID, UserId);
                        QDL.Add(availableStocks);
                    }
                    IEnumerable Res_cache = (IEnumerable)cache.Get(CacheKeyMetaData);
                    if (Res_cache != null) {
                        foreach (QueryMetaD R in QDL) {
                            if ((Res_cache as List<QueryMetaD>).Find(t => t.settIRP.ID == R.settIRP.ID) == null) {
                                CacheItemPolicy cacheItemPolicy = new CacheItemPolicy();
                                cache.Add(CacheKeyMetaData, QDL, cacheItemPolicy);
                            }
                        }
                    }
                    else {
                        CacheItemPolicy cacheItemPolicy = new CacheItemPolicy();
                        cache.Add(CacheKeyMetaData, QDL, cacheItemPolicy);
                    }
                    return QDL;
                }
            }
            else {
                if (!withAppend) {
                    List<QueryMetaD> QDL = new List<QueryMetaD>();
                    Class_ManageSettingInstance v_s = new Class_ManageSettingInstance();
                    foreach (SettingIRPClass st in (List<SettingIRPClass>)GetAvailableStocksSettingIRP(false))
                    {
                        QueryMetaD availableStocks = v_s.GetQueryMetaData((List<SettingIRPClass>)GetAvailableStocksSettingIRP(false), st.ID, UserId);
                        QDL.Add(availableStocks);
                    }
                    return QDL;
                }
                else {
                    List<QueryMetaD> QDL = new List<QueryMetaD>();
                    Class_ManageSettingInstance v_s = new Class_ManageSettingInstance();
                    foreach (SettingIRPClass st in (List<SettingIRPClass>)GetAvailableStocksSettingIRP(false)) {
                        QueryMetaD availableStocks = v_s.GetQueryMetaData((List<SettingIRPClass>)GetAvailableStocksSettingIRP(false), st.ID, UserId);
                        QDL.Add(availableStocks);
                    }
                    IEnumerable Res_cache = (IEnumerable)cache.Get(CacheKeyMetaData);
                    if (Res_cache != null) {
                        foreach (QueryMetaD R in QDL) {
                            if ((Res_cache as List<QueryMetaD>).Find(t => t.settIRP.ID == R.settIRP.ID) == null) {
                                CacheItemPolicy cacheItemPolicy = new CacheItemPolicy();
                                cache.Add(CacheKeyMetaData, QDL, cacheItemPolicy);
                            }
                        }
                    }
                    else {
                        CacheItemPolicy cacheItemPolicy = new CacheItemPolicy();
                        cache.Add(CacheKeyMetaData, QDL, cacheItemPolicy);
                    }
                    return QDL;
                }
            }
        }
    }
}
