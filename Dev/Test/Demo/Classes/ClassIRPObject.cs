using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using DAL;
using System.Text;
using System.Web;
using System.Web.SessionState;
using OnlinePortal;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using OnlinePortal.Utils;
using LitvaPortal.Utils;
using Utils;
using LitvaPortal.ServiceReference_WebQuery;

namespace LitvaPortal
{


    public class ClassIRPObject
    {
        public List<RecordPtrDB> FLD_DETAIL { get; set; }
        public List<string> FLD { get; set; }
        public List<Type> FLD_TYPE { get; set; }
        public List<string> CAPTION_FLD { get; set; }
        public List<string> FORMAT_FLD { get; set; }
        public List<object[]> Val_Arr { get; set; }
        public string FILTER { get; set; }
        public string TABLE_NAME { get; set; }
        public SettingIRPClass Setting_param { get; set; }
        public string FormatConstraint { get; set; }
        public string StatusObject { get; set; }
        public Dictionary<string, string> NameItemMenu_CUR = new Dictionary<string, string>();
        public Dictionary<string, string> NameItemMenu_EXP = new Dictionary<string, string>();
        public SettingIRPClass[] PagesIndexRange { get; set; }

        public ClassIRPObject()
        {
            FLD = new List<string>();
            CAPTION_FLD = new List<string>();
            FORMAT_FLD = new List<string>();
            Val_Arr = new List<dynamic[]>();
            Setting_param = new SettingIRPClass();
            FILTER = "";
            TABLE_NAME = "";
            FLD_TYPE = new List<Type>();
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~ClassIRPObject()
        {
            Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            FLD = null;
            CAPTION_FLD = null;
            Val_Arr = null;
            Setting_param = null;
            FILTER = "";
            TABLE_NAME = "";
            GC.SuppressFinalize(this);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="cls"></param>
        /// <returns></returns>
        public DataTable GetRecords(int ID_STATION, ref GridView Grid, string Categ, List<BlockDataFind> Find_Params, int StartIndex, out List<ClassIRPObject> Irp_Res, object sess, string NameItemPress, string SectionT)
        {
            Irp_Res = new List<ClassIRPObject>();
            DataTable tbl = new DataTable();
            try
            {
                List<ClassIRPObject> Irp = new List<ClassIRPObject>();
                if (sess != null) Irp = (List<ClassIRPObject>)sess;
                ClassIRPObject fnd_ = Irp.Find(r => r.Setting_param.NAME == Categ && r.StatusObject.ToString() == SectionT && (r.NameItemMenu_CUR.Count == 0) && (r.NameItemMenu_EXP.Count == 0));
                if (fnd_ != null)
                {
                    tbl = DataAdapterClass.GetData(fnd_, Find_Params);
                    Grid.AutoGenerateColumns = false;
                    Grid.AutoGenerateColumns = true;
                    Grid.DataSourceID = null;
                    Grid.DataSource = tbl;
                    if (fnd_.FLD.Contains("ID")) { Grid.DataKeyNames = new string[] { "ID" }; Grid.DataBind(); } else { Grid.DataKeyNames = null; Grid.DataBind(); }
                    if (fnd_.Setting_param.index_group_field > 0) DataAdapterClass.GroupGridView(Grid.Rows, fnd_.Setting_param.index_group_field, fnd_.Setting_param.TOTAL_COUNT_GROUP);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return tbl;
        }

    }
}