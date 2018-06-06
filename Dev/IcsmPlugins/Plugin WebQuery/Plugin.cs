﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using ICSM;
using FormsCs;
using OrmCs;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace XICSM.WebQuery
{
    public class Plugin : IPlugin
    {
        internal Control IcsmMainWindows { get; private set; }
        public static System.Windows.Forms.Timer time_check_status { get; set; }
        public static int CountCall = 0;
        //Версия плагина
        public string Ident { get { return "WebQuery"; } }
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
                        if (recordPtrParam.Table == ICSMTbl.WebQuery)
                        {
                            IM.AdminDisconnect();
                            OnDblClickRecord(recordPtrParam);
                        }
                    }
                    break;

            }

            return false;
        }


        private void OnDblClickRecord(RecordPtr recPtr)
        {

            if (recPtr.Table == ICSMTbl.WebQuery)
            {
                IMQueryMenuNode.Context context = new IMQueryMenuNode.Context();
                context.TableName = recPtr.Table;
                context.TableId = recPtr.Id;
                AdminFormWebQuery Web = new AdminFormWebQuery(context);
                Web.ShowDialog();
            }


        }

        private void AdminFormSettingWebQuery(string checkTable, string tableName, int nbSelMin, ref List<IMQueryMenuNode> menuList)
        {
            AddContextMenu(ref menuList, checkTable, new IMQueryMenuNode("Insert record...", null, OnNewRecWebQery, IMQueryMenuNode.ExecMode.Table), IMTableRight.Insert);
            AddContextMenu(ref menuList, checkTable, new IMQueryMenuNode("Edit record...", null, OnEditRecWebQery, IMQueryMenuNode.ExecMode.SelectionOfRecords), IMTableRight.Select);
            AddContextMenu(ref menuList, checkTable, new IMQueryMenuNode("Delete record...", null, OnDeleteRecSettingWebQuery, IMQueryMenuNode.ExecMode.SelectionOfRecords), IMTableRight.Delete);

        }

        private bool OnDeleteRecSettingWebQuery(IMQueryMenuNode.Context context)
        {
            DialogResult DA = MessageBox.Show("Are you delete record?", "Question", MessageBoxButtons.YesNo);
            if (DA == DialogResult.Yes)
            {
                IMRecordset RSettingWebQueryDel = new IMRecordset(context.TableName, IMRecordset.Mode.ReadWrite);
                RSettingWebQueryDel.Select("ID");
                RSettingWebQueryDel.AddSelectionFrom(context.DataList, IMRecordset.WhereCopyOptions.SelectedLines);
                RSettingWebQueryDel.Open();
                if (!RSettingWebQueryDel.IsEOF()){
                    RSettingWebQueryDel.Delete();
                }
                RSettingWebQueryDel.Close();
                RSettingWebQueryDel.Destroy();
            }
            return true;
        }


        private bool OnEditRecWebQery(IMQueryMenuNode.Context context)
        {
            AdminFormWebQuery Web = new AdminFormWebQuery(context);
            Web.ShowDialog();
            return true;
        }



        private bool OnNewRecWebQery(IMQueryMenuNode.Context context)
        {
            context.DataList = null;
            AdminFormWebQuery Web = new AdminFormWebQuery(context);
            Web.ShowDialog();
            return true;
        }


        public bool UpgradeDatabase(IMSchema s, double dbCurVersion)
        {
            return UpdateSchema.UpgradeDatabase(s, dbCurVersion);
        }

        public void RegisterSchema(IMSchema s)
        {
            UpdateSchema.RegisterSchema(s);
            string appFolder = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            string err;
            if (!OrmCs.OrmSchema.ParseSchema(appFolder, "WebQuery", "XICSM_WebQuery.dll", out err)) MessageBox.Show("Could not load 'WebQuery.Schema' :" + err);

        }

        public void RegisterBoard(IMBoard b)
        {
            // -- блицы ICSM --
            b.RegisterQueryMenuBuilder(ICSMTbl.WebQuery, OnGetQueryMenu);
           
        }
        //=================================================
        // Запрос отображения Popup Menu
        //=================================================
        public List<IMQueryMenuNode> OnGetQueryMenu(String tableName, int nbSelMin)
        {
            List<IMQueryMenuNode> menuList = new List<IMQueryMenuNode>();
            //=================================================
            //Таблицы манагера
            //=================================================
            switch (tableName)
            {
                case ICSMTbl.WebQuery:
                    AdminFormSettingWebQuery(tableName, tableName, nbSelMin, ref menuList);
                    break;
            }
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