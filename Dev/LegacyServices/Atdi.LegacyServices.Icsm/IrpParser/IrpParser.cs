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

        public IrpParser(Orm.SchemasMetadata schemasMetadata, ILogger logger) : base(logger)
        {
            this._schemasMetadata = schemasMetadata;
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
            var rc = new PropertyTable();
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
                                    if (joi.From.Length > 0) rc.FieldJoinFrom = joi.From[0].Name;
                                    if (joi.To.Length > 0) rc.FieldJoinTo = joi.To[0].Name;
                                    if (joi.To.Length > 0) { if (joi.To[0].DDesc != null) rc.TypeValue = joi.To[0].DDesc.ClassType; rc.Precision = joi.To[0].DDesc.Precision; }
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
            var TableName2 = "";
            var Lst_Val_Index = new List<int>();
            TableName2 = tableName;
            string[] Spl = null;
            var recDB = new PropertyTable();
            recDB.NameTableTo = tableName;
            if (fld.IndexOf(".") > 0) {
                Spl = fld.Split(new char[] { '.' });
            }
            if (Spl != null) {
                for (int r = 0; r < Spl.Length - 1; r++) {
                    recDB = GetTableFromORM(Spl[r], r == 0 ? TableName2 : recDB.NameTableTo);
                    recDB.NameTableFrom = (r == 0 ? TableName2 : Spl[r - 1]);
                    Spl[r] = recDB.NameTableTo;
                }
                if (Spl.Length > 0) recDB.NameFieldForSetValue = Spl[Spl.Length - 1];
            }
            else {
                recDB.NameTableTo = tableName;
                recDB.NameFieldForSetValue = fld;
            }
            return GetOrmDataDesc(recDB.NameFieldForSetValue, recDB.NameTableTo);
        }


        public IrpDescriptor ExecuteParseQuery(string value)
        {
            List<IrpColumn> listDescrColumns = new List<IrpColumn>();
            var irpDescr = new IrpDescriptor();
            try
            {
                

                var f = new Frame();
                string Query = value;
                int x1 = Query.IndexOf("\r\n");
                Query = Query.Remove(0, x1 + 2);
                int x2 = Query.IndexOf("\r\n");
                Query = Query.Remove(0, x2 + 2);
                var strx = new InChannelString(Query);
                f.Load(strx);
                var _report = new IcsmReport();
                _report.SetConfig(f);
                for (int i = 0; i < _report.m_dat.m_list[0].m_query.lq.Length; i++) {
                    {
                        var metaData = new IrpColumn();
                        metaData.columnMeta = new ColumnMetadata();

                        metaData.columnMeta.Description = _report.m_desc;
                        metaData.columnMeta.Title = "";
                        irpDescr.TableName = _report.m_dat.m_tab;
                        string t = _report.m_dat.m_list[0].m_query.lq[i].path;
                        t = t.Replace(_report.m_dat.m_tab + ".", "");
                        metaData.columnMeta.Description = t;
                        metaData.columnMeta.Name = t;
                        metaData.columnMeta.Format = _report.m_dat.m_list[0].m_query.lq[i].format;
                        if (_report.m_dat.m_list[0].m_query.lq[i].ord == Ordering.oNone) metaData.columnMeta.Order = OrderType.None;
                        else if (_report.m_dat.m_list[0].m_query.lq[i].ord == Ordering.oAsc) metaData.columnMeta.Order = OrderType.Ascending;
                        else if (_report.m_dat.m_list[0].m_query.lq[i].ord == Ordering.oDesc) metaData.columnMeta.Order = OrderType.Descending;
                        metaData.columnMeta.Position = 0;
                        metaData.columnMeta.Title = _report.m_dat.m_list[0].m_query.lq[i].title;
                        metaData.columnMeta.Width = _report.m_dat.m_list[0].m_query.lq[i].colWidth;
                        if (_report.m_dat.m_list[0].m_query.lq[i].m_isCustExpr)
                        {
                            metaData.Expr = "$" + _report.m_dat.m_list[0].m_query.lq[i].m_CustExpr + "#:" + _report.m_dat.m_list[0].m_query.lq[i].title;
                            metaData.TypeColumn = IrpColumnEnum.Expression;
                            metaData.columnMeta.Type = DataModels.DataType.String;
                        }
                        else
                        {
                            metaData.Expr = "";
                            metaData.TypeColumn = IrpColumnEnum.StandardColumn;
                        }
                        var ty_p = GetOrmDataDesc(t, _report.m_dat.m_tab);
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
                        if (ty_p != null)
                        {
                            switch (ty_p.DDesc.ClassType)
                            {
                                case Orm.VarType.var_Bytes:
                                    metaData.columnMeta.Type = DataModels.DataType.Bytes;
                                    break;
                                case Orm.VarType.var_Flo:
                                    metaData.columnMeta.Type = DataModels.DataType.Float;
                                    break;
                                case Orm.VarType.var_Int:
                                    metaData.columnMeta.Type = DataModels.DataType.Integer;
                                    break;
                                case Orm.VarType.var_Dou:
                                    metaData.columnMeta.Type = DataModels.DataType.Double;
                                    break;
                                case Orm.VarType.var_String:
                                    metaData.columnMeta.Type = DataModels.DataType.String;
                                    break;
                                case Orm.VarType.var_Tim:
                                    metaData.columnMeta.Type = DataModels.DataType.DateTime;
                                    break;
                                default:
                                    metaData.columnMeta.Type = DataModels.DataType.String;
                                    break;
                            }
                        }
                        listDescrColumns.Add(metaData);
                    }
                }
                _report.Clear(false);
                irpDescr.irpColumns = listDescrColumns;
                
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
