using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
using ICSM;
using FormsCs;
using OrmCs;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace XICSM.Atdi.Icsm.Plugins.WebQueryExtended
{
    public class Plugin : IPlugin
    {
        internal Control IcsmMainWindows { get; private set; }
        public static System.Windows.Forms.Timer time_check_status { get; set; }
        public static int CountCall = 0;
        //Версия плагина
        public string Ident { get { return "Atdi.Icsm.Plugins.WebQueryExtended"; } }
        public string Description { get { return null; } }
        public double SchemaVersion { get { return UpdateSchema.schemaVersion; } }

        public Plugin()
        {
            IcsmMainWindows = null;
        }

        //===================================================
        public bool OtherMessage(string message, Object inParam, ref Object outParam)
        {
            System.Diagnostics.Debug.Indent();
            System.Diagnostics.Debug.WriteLine("OtherMessage(" + message + ")");
            System.Diagnostics.Debug.Unindent();
            switch (message)
            {
                case "DBLCLK_RECORD":
                    if (inParam is RecordPtr)
                    {
                        RecordPtr recordPtrParam = (RecordPtr)inParam;
                        IM.AdminDisconnect();
                        OnDblClickRecord(recordPtrParam);
                    }
                    break;

            }

            return false;
        }


        private void OnDblClickRecord(RecordPtr recPtr)
        {

          

        }



        public bool UpgradeDatabase(IMSchema s, double dbCurVersion)
        {
            return UpdateSchema.UpgradeDatabase(s, dbCurVersion);
        }

        public void RegisterSchema(IMSchema s)
        {
            UpdateSchema.RegisterSchema(s);
            string appFolder = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            //string err;
            //if (!OrmCs.OrmSchema.ParseSchema(appFolder, "Atdi.Icsm.Plugins.WebQueryExtended", "XICSM_Atdi.Icsm.Plugins.WebQueryExtended.dll", out err)) MessageBox.Show("Could not load 'XICSM_Atdi.Icsm.Plugins.WebQueryExtended.Schema' :" + err);

        }

        public void RegisterBoard(IMBoard b)
        {
            b.RegisterQueryMenuBuilder("XV_WEB_BC", OnGetQueryMenu);
        }


        public List<IMQueryMenuNode> OnGetQueryMenu(String tableName, int nbSelMin)
        {
            List<IMQueryMenuNode> menuList = new List<IMQueryMenuNode>();
            //switch (tableName)
            //{
            //}
            return menuList;
        }
      


        public void GetMainMenu(IMMainMenu mainMenu)
        {
            

        }

        //===================================================
        /// <summary>
        /// Добавляем контекстное меню, если есть права
        /// </summary>
        /// <param name="menuList">список менб</param>
        /// <param name="tableName">имя таблицы</param>
        /// <param name="node">элемент меню</param>
        /// <param name="right">права</param>
        private void AddContextMenu(ref List<IMQueryMenuNode> menuList, string tableName, IMQueryMenuNode node, IMTableRight right)
        {
            if ((IM.TableRight(tableName) & right) == right)
                menuList.Add(node);
        }
        //===================================================



    }
     
}
