using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.Logging;
using Atdi.DataModels.Identity;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.LegacyServices.Icsm;
using System.Security.Cryptography;
using Atdi.DataModels.DataConstraint;
using Atdi.AppServices.WebQuery;
using OrmCs;
using DatalayerCs;
using Atdi.DataModels.WebQuery;


namespace Atdi.LegacyServices.Icsm
{

    internal sealed class QueryParser: IIrpParser
    {
      
        private readonly IcsmReport _report;
        public QueryParser() 
        {
            _report = new IcsmReport();
        }

        public static OrmField GetOrmDataDesc(string fld_check, string tableName)
        {
            OrmField rc = null;
            OrmTable zeta = OrmSchema.Table(tableName, false);
            if (zeta != null) {
                foreach (OrmField f1 in zeta.ClassFields) {
                    switch (f1.Nature){
                        case OrmFieldNature.Column:
                            {
                                OrmFieldF fjF = (OrmFieldF)f1;
                                if (fld_check == f1.Name) {
                                    if (fjF != null) {
                                        rc = fjF;
                                    }
                                }
                            }
                            break;

                    }
                }
            }
            return rc;
        }

        public PropertyTable GetTableFromORM(string fld_check, string tableName)
        {
            PropertyTable rc = new PropertyTable();
            OrmTable zeta = OrmSchema.Table(tableName, false);
            if (zeta != null) {
                foreach (OrmField f1 in zeta.ClassFields) {
                    switch (f1.Nature) {
                        case OrmFieldNature.Join:
                            {
                                OrmFieldJ fj = (OrmFieldJ)f1;
                                OrmJoin joi = fj.Join;
                                OrmTable tc = joi.JoinedTable;
                                string joinedClass = OrmSourcer.TableNameToClassName(f1.Name);
                                if (fld_check == f1.Name)  {
                                    rc.NameTableTo = tc.Name;
                                    rc.Name = f1.Name;
                                    if (joi.From.Count() > 0) rc.FieldJoinFrom = joi.From[0].Name;
                                    if (joi.To.Count() > 0) rc.FieldJoinTo = joi.To[0].Name;
                                    if (joi.To.Count() > 0) { if (joi.To[0].DDesc != null) rc.TypeValue = joi.To[0].DDesc.ClassType; rc.Precision = joi.To[0].DDesc.Precision; }
                                }
                            }
                            break;
                    }
                }
            }
            return rc;
        }



        public  OrmField GetFieldFromOrm(string tableName, string fld)
        {
            string TableName2 = "";
            List<int> Lst_Val_Index = new List<int>();
            TableName2 = tableName;
            string[] Spl = null;
            PropertyTable recDB = new PropertyTable();
            recDB.NameTableTo = tableName;
            if (fld.IndexOf(".") > 0) {
                Spl = fld.Split(new char[] { '.' });
            }
            if (Spl != null) {
                for (int r = 0; r < Spl.Count() - 1; r++) {
                    recDB = GetTableFromORM(Spl[r], r == 0 ? TableName2 : recDB.NameTableTo);
                    recDB.NameTableFrom = (r == 0 ? TableName2 : Spl[r - 1]);
                    Spl[r] = recDB.NameTableTo;
                }
                if (Spl.Count() > 0) recDB.NameFieldForSetValue = Spl[Spl.Count() - 1];
            }
            else {
                recDB.NameTableTo = tableName;
                recDB.NameFieldForSetValue = fld;
            }
            return GetOrmDataDesc(recDB.NameFieldForSetValue, recDB.NameTableTo);
        }


        public ColumnMetadata[] ExecuteParseQuery(string value)
        {
            List<ColumnMetadata> L = new List<ColumnMetadata>();
            Frame f = new Frame();
            string Query = value;
            int x1 = Query.IndexOf("\r\n");
            Query = Query.Remove(0, x1 + 2);
            int x2 = Query.IndexOf("\r\n");
            Query = Query.Remove(0, x2 + 2);
            InChannelString strx = new InChannelString(Query);
            f.Load(strx);
            _report.SetConfig(f);
            if (_report != null)
            {
                if (_report.m_dat.m_list.Count() > 0)
                {
                    for (int i = 0; i < _report.m_dat.m_list[0].m_query.lq.Count(); i++)
                    {
                        if (!_report.m_dat.m_list[0].m_query.lq[i].m_isCustExpr)
                        {
                            var metaData = new ColumnMetadata();
                            metaData.Description = _report.m_desc;
                            metaData.Title = "";
                            metaData.Name = _report.m_desc;
                            string t = _report.m_dat.m_list[0].m_query.lq[i].path;
                            t = t.Replace(_report.m_dat.m_tab + ".", "");
                            metaData.Description = t;
                            metaData.Format = _report.m_dat.m_list[0].m_query.lq[i].format;
                            if (_report.m_dat.m_list[0].m_query.lq[i].ord == Ordering.oNone) metaData.Order = OrderType.None;
                            else if (_report.m_dat.m_list[0].m_query.lq[i].ord == Ordering.oAsc) metaData.Order = OrderType.Ascending;
                            else if (_report.m_dat.m_list[0].m_query.lq[i].ord == Ordering.oDesc) metaData.Order = OrderType.Descending;
                            metaData.Position = 0;
                            metaData.Title = _report.m_dat.m_list[0].m_query.lq[i].title;
                            metaData.Width = _report.m_dat.m_list[0].m_query.lq[i].colWidth;
                            OrmField ty_p = GetOrmDataDesc(t, _report.m_dat.m_tab);
                            if (ty_p == null)
                            {
                                string FLD_STATE_FORMAT = ""; string FLD_STATE_value = "";
                                if (t.Contains("."))
                                {
                                    FLD_STATE_value = t;
                                    var count = t.Count(chr => chr == '.');
                                    FLD_STATE_FORMAT = t.Replace(".", "(") + "".PadRight(count, ')');
                                }
                                if (FLD_STATE_FORMAT != "")
                                {
                                    ty_p = GetFieldFromOrm(_report.m_dat.m_tab, FLD_STATE_value);
                                }
                            }

                            switch (ty_p.DDesc.ClassType)
                            {
                                case OrmVarType.var_Bytes:
                                    metaData.Type = DataModels.DataType.Bytes;
                                    break;
                                case OrmVarType.var_Flo:
                                    metaData.Type = DataModels.DataType.Float;
                                    break;
                                case OrmVarType.var_Int:
                                    metaData.Type = DataModels.DataType.Integer;
                                    break;
                                case OrmVarType.var_Dou:
                                    metaData.Type = DataModels.DataType.Double;
                                    break;
                                case OrmVarType.var_String:
                                    metaData.Type = DataModels.DataType.String;
                                    break;
                                case OrmVarType.var_Tim:
                                    metaData.Type = DataModels.DataType.DateTime;
                                    break;
                                default:
                                    metaData.Type = DataModels.DataType.String;
                                    break;
                            }
                            L.Add(metaData);
                        }
                    }
                }
            }
            return L.ToArray();
        }

        public ColumnMetadata[] ExecuteParseQuery(byte[] value)
        {
           return ExecuteParseQuery(UTF8Encoding.UTF8.GetString(value));
        }
    }
}
