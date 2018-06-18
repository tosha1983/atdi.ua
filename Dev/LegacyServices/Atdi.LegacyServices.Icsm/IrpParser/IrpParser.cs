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
using Atdi.DataModels.WebQuery;


namespace Atdi.LegacyServices.Icsm
{

    internal sealed class IrpParser : LoggedObject, IIrpParser
    {
        private readonly Orm.SchemasMetadata _schemasMetadata;
        private readonly IcsmReport _report;

        public IrpParser(Orm.SchemasMetadata schemasMetadata, ILogger logger) : base(logger)
        {
            this._schemasMetadata = schemasMetadata;
            _report = new IcsmReport();
        }

        public Orm.Field GetOrmDataDesc(string fld_check, string tableName)
        {
            Orm.Field rc = null;
            var zeta = this._schemasMetadata.GetTableByName(tableName);
            if (zeta != null) {
                foreach (Orm.Field f1 in zeta.ClassFields) {
                    switch (f1.Nature){
                        case Orm.FieldNature.Column:
                            {
                                Orm.FieldF fjF = (Orm.FieldF)f1;
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
            var zeta = this._schemasMetadata.GetTableByName(tableName);
            if (zeta != null) {
                foreach (var f1 in zeta.ClassFields) {
                    switch (f1.Nature) {
                        case Orm.FieldNature.Join:
                            {
                                var fj = (Orm.FieldJ)f1;
                                var joi = fj.Join;
                                var tc = joi.JoinedTable;
                                //string joinedClass = OrmSourcer.TableNameToClassName(f1.Name);
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



        public  Orm.Field GetFieldFromOrm(string tableName, string fld)
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


        public IrpDescriptor ExecuteParseQuery(string value)
        {
            IrpDescriptor irpDescr = new IrpDescriptor();
            List<ColumnMetadata> L = new List<ColumnMetadata>();
            try
            {
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
                               
                                irpDescr.TableName = _report.m_dat.m_tab;
                                string t = _report.m_dat.m_list[0].m_query.lq[i].path;
                                t = t.Replace(_report.m_dat.m_tab + ".", "");
                                metaData.Description = t;
                                metaData.Name = t;
                                metaData.Format = _report.m_dat.m_list[0].m_query.lq[i].format;
                                if (_report.m_dat.m_list[0].m_query.lq[i].ord == Ordering.oNone) metaData.Order = OrderType.None;
                                else if (_report.m_dat.m_list[0].m_query.lq[i].ord == Ordering.oAsc) metaData.Order = OrderType.Ascending;
                                else if (_report.m_dat.m_list[0].m_query.lq[i].ord == Ordering.oDesc) metaData.Order = OrderType.Descending;
                                metaData.Position = 0;
                                metaData.Title = _report.m_dat.m_list[0].m_query.lq[i].title;
                                metaData.Width = _report.m_dat.m_list[0].m_query.lq[i].colWidth;
                                Orm.Field ty_p = GetOrmDataDesc(t, _report.m_dat.m_tab);
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
                                    case Orm.VarType.var_Bytes:
                                        metaData.Type = DataModels.DataType.Bytes;
                                        break;
                                    case Orm.VarType.var_Flo:
                                        metaData.Type = DataModels.DataType.Float;
                                        break;
                                    case Orm.VarType.var_Int:
                                        metaData.Type = DataModels.DataType.Integer;
                                        break;
                                    case Orm.VarType.var_Dou:
                                        metaData.Type = DataModels.DataType.Double;
                                        break;
                                    case Orm.VarType.var_String:
                                        metaData.Type = DataModels.DataType.String;
                                        break;
                                    case Orm.VarType.var_Tim:
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
                irpDescr.columnMetaData = L.ToArray();
            }
            catch (Exception e)
            {
                this.Logger.Exception(Contexts.LegacyServicesIcsm, Categories.ParseIRP, e, this);
                throw new InvalidOperationException(Exceptions.ParsingIRPFile, e);
            }
            return irpDescr;
        }

        public IrpDescriptor ExecuteParseQuery(byte[] value)
        {
            return ExecuteParseQuery(UTF8Encoding.UTF8.GetString(value));
        }
    }
}
