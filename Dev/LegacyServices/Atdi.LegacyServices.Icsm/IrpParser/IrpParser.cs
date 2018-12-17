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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fld_check"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public ColumnProperties[] GetMandatoryFieldsFromTables(List<string> tableNames)
        {
            List<ColumnProperties> mandatoryList = new List<ColumnProperties>();
            for (int l = 0; l < tableNames.Count; l++)
            {
                var zeta = this._schemasMetadata.GetTableByName(tableNames[l]);
                if (zeta != null)
                {
                    foreach (Orm.Field f1 in zeta.ClassFields)
                    {
                        switch (f1.Nature)
                        {
                            case Orm.FieldNature.Column:
                                {
                                    Orm.FieldF fjF = (Orm.FieldF)f1;
                                    if (fjF != null)
                                    {
                                        if (fjF.Options == (Orm.FieldFOption.fld_NOTNULL))
                                        {
                                            ColumnProperties propertyTable = new ColumnProperties();
                                            propertyTable.FieldJoinTo = f1.Name;
                                            propertyTable.NameTableTo = tableNames[l];
                                            if (f1.DDesc != null)
                                            {
                                                propertyTable.Precision = f1.DDesc.Precision;
                                                propertyTable.DefaultValue = fjF.DefVal;

                                                if (f1.DDesc.ClassType == Orm.VarType.var_Int)
                                                {
                                                    propertyTable.TypeColumn = typeof(int);
                                                }
                                                else if (f1.DDesc.ClassType == Orm.VarType.var_String)
                                                {
                                                    propertyTable.TypeColumn = typeof(string);
                                                }
                                                else if (f1.DDesc.ClassType == Orm.VarType.var_Bytes)
                                                {
                                                    propertyTable.TypeColumn = typeof(byte[]);
                                                }
                                                else if (f1.DDesc.ClassType == Orm.VarType.var_Dou)
                                                {
                                                    propertyTable.TypeColumn = typeof(double);
                                                }
                                                else if (f1.DDesc.ClassType == Orm.VarType.var_Flo)
                                                {
                                                    propertyTable.TypeColumn = typeof(float);
                                                }
                                                else if (f1.DDesc.ClassType == Orm.VarType.var_Guid)
                                                {
                                                    propertyTable.TypeColumn = typeof(Guid);
                                                }
                                                else if (f1.DDesc.ClassType == Orm.VarType.var_Tim)
                                                {
                                                    propertyTable.TypeColumn = typeof(DateTime);
                                                }
                                                else if (f1.DDesc.ClassType == Orm.VarType.var_Null)
                                                {
                                                    propertyTable.TypeColumn = typeof(int);
                                                }
                                                else
                                                {
                                                    throw new NotImplementedException("Not supported type for linked column");
                                                }

                                            }
                                            mandatoryList.Add(propertyTable);
                                        }
                                    }

                                }
                                break;

                        }
                    }
                }
            }
            return mandatoryList.ToArray();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fld_check"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public ColumnProperties[] GetPrimaryFieldsFromTables(List<string> tableNames)
        {
            List<ColumnProperties> mandatoryList = new List<ColumnProperties>();
            for (int l = 0; l < tableNames.Count; l++)
            {
                var zeta = this._schemasMetadata.GetTableByName(tableNames[l]);
                if (zeta != null)
                {
                    foreach (Orm.Field f1 in zeta.ClassFields)
                    {
                        switch (f1.Nature)
                        {
                            case Orm.FieldNature.Column:
                                {
                                    Orm.FieldF fjF = (Orm.FieldF)f1;
                                    if (fjF != null)
                                    {
                                        if ((fjF.Options == (Orm.FieldFOption.fld_NOTNULL | Orm.FieldFOption.fld_PRIMARY | Orm.FieldFOption.fld_FKEY)) || ((fjF.Options == (Orm.FieldFOption.fld_NOTNULL | Orm.FieldFOption.fld_PRIMARY)) && (fjF.Index == 0)))
                                        {
                                            ColumnProperties propertyTable = new ColumnProperties();
                                            propertyTable.FieldJoinTo = f1.Name;
                                            propertyTable.DefaultValue = fjF.DefVal;
                                            propertyTable.NameTableTo = tableNames[l];
                                            if (f1.DDesc != null)
                                            {
                                                propertyTable.Precision = f1.DDesc.Precision;

                                                if (f1.DDesc.ClassType == Orm.VarType.var_Int)
                                                {
                                                    propertyTable.TypeColumn = typeof(int);
                                                }
                                                else if (f1.DDesc.ClassType == Orm.VarType.var_String)
                                                {
                                                    propertyTable.TypeColumn = typeof(string);
                                                }
                                                else if (f1.DDesc.ClassType == Orm.VarType.var_Bytes)
                                                {
                                                    propertyTable.TypeColumn = typeof(byte[]);
                                                }
                                                else if (f1.DDesc.ClassType == Orm.VarType.var_Dou)
                                                {
                                                    propertyTable.TypeColumn = typeof(double);
                                                }
                                                else if (f1.DDesc.ClassType == Orm.VarType.var_Flo)
                                                {
                                                    propertyTable.TypeColumn = typeof(float);
                                                }
                                                else if (f1.DDesc.ClassType == Orm.VarType.var_Guid)
                                                {
                                                    propertyTable.TypeColumn = typeof(Guid);
                                                }
                                                else if (f1.DDesc.ClassType == Orm.VarType.var_Tim)
                                                {
                                                    propertyTable.TypeColumn = typeof(DateTime);
                                                }
                                                else if (f1.DDesc.ClassType == Orm.VarType.var_Null)
                                                {
                                                    propertyTable.TypeColumn = typeof(int);
                                                }
                                                else
                                                {
                                                    throw new NotImplementedException("Not supported type for linked column");
                                                }

                                            }
                                            mandatoryList.Add(propertyTable);
                                        }
                                    }

                                }
                                break;

                        }
                    }
                }
            }
            return mandatoryList.ToArray();
        }



        public string[] GetPrimaryKeys(string tableName)
        {
            List<string> allPrimaryKeys = new List<string>();
            var zeta = this._schemasMetadata.GetTableByName(tableName);
            if (zeta != null)
            {
                foreach (Orm.Field f1 in zeta.ClassFields)
                {
                    switch (f1.Nature)
                    {
                        case Orm.FieldNature.Column:
                            {
                                Orm.FieldF fjF = (Orm.FieldF)f1;
                                if (fjF != null)
                                {
                                    if ((fjF.Options == (Orm.FieldFOption.fld_NOTNULL | Orm.FieldFOption.fld_PRIMARY | Orm.FieldFOption.fld_FKEY)) || ((fjF.Options == (Orm.FieldFOption.fld_NOTNULL | Orm.FieldFOption.fld_PRIMARY)) && (fjF.Index == 0)))
                                    {
                                        allPrimaryKeys.Add(f1.Name);
                                    }
                                }
                            }
                            break;

                    }
                }
            }
            return allPrimaryKeys.ToArray();
        }


        public Orm.FieldF GetOrmDataDesc(string fld_check, string tableName)
        {
            Orm.FieldF rc = null;
            var zeta = this._schemasMetadata.GetTableByName(tableName);
            if (zeta != null) {
                foreach (Orm.Field f1 in zeta.Fields) {
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
                                if (fld_check == f1.Name)  {
                                    rc.NameTableTo = tc.Name;
                                    rc.Name = f1.Name;
                                    if (joi.From.Length > 0)
                                    {
                                        rc.FieldJoinFrom = joi.From[0].Name;
                                        var fromField = GetOrmDataDesc(joi.From[0].Name, joi.From[0].Table.Name);
                                        if (fromField != null)
                                        {
                                            rc.DefaultValueFrom = fromField.DefVal;
                                            if (fromField.DDesc != null)
                                            {
                                                rc.PrecisionFieldJoinFrom = fromField.DDesc.Precision;
                                            }
                                        }
                                    }
                                    if (joi.To.Length > 0)
                                    {
                                        rc.FieldJoinTo = joi.To[0].Name;
                                        var toField = GetOrmDataDesc(joi.To[0].Name, joi.To[0].Table.Name);
                                        if (toField != null)
                                        {
                                            rc.DefaultValueTo = toField.DefVal;
                                            if (toField.DDesc != null)
                                            {
                                                rc.PrecisionFieldJoinTo = toField.DDesc.Precision;
                                            }
                                        }
                                    }
                                    if (joi.To.Length > 0) { if (joi.To[0].DDesc != null) rc.TypeValue = joi.To[0].DDesc.ClassType; rc.Precision = joi.To[0].DDesc.Precision; }
                                }
                            }
                            break;
                    }
                }
            }
            return rc;
        }

        public List<string> GetAllTables(List<ColumnProperties> columnProperties)
        {
            var allListTables = new List<string>();
            for (int i=0; i< columnProperties.Count; i++)
            {
                if (!allListTables.Contains(columnProperties[i].NameTableFrom))
                {
                    allListTables.Add(columnProperties[i].NameTableFrom);
                }
                if (!allListTables.Contains(columnProperties[i].NameTableTo))
                {
                    allListTables.Add(columnProperties[i].NameTableTo);
                }
            }
            return allListTables;
        }


        public ColumnProperties[] GetPropertyFieldFromOrm(string tableName, string fld)
        {
            List<ColumnProperties> propertyField = new List<ColumnProperties>();
            var TableName2 = "";
            TableName2 = tableName;
            string[] Spl = null;
            var recDB = new PropertyTable();
            recDB.NameTableTo = tableName;
            if (fld.IndexOf(".") > 0)
            {
                Spl = fld.Split(new char[] { '.' });
            }
            if (Spl != null)
            {
                for (int r = 0; r < Spl.Length - 1; r++)
                {
                    var columnProperties = new ColumnProperties();
                    recDB = GetTableFromORM(Spl[r], r == 0 ? TableName2 : recDB.NameTableTo);
                    recDB.NameTableFrom = (r == 0 ? TableName2 : Spl[r - 1]);
                    Spl[r] = recDB.NameTableTo;
                    columnProperties.FieldJoinFrom = recDB.FieldJoinFrom;
                    columnProperties.FieldJoinTo = recDB.FieldJoinTo;
                    columnProperties.Name = recDB.Name;
                    columnProperties.NameField = recDB.NameFieldForSetValue;
                    columnProperties.NameTableFrom = recDB.NameTableFrom;
                    columnProperties.NameTableTo = recDB.NameTableTo;
                    columnProperties.DefaultValueFrom = recDB.DefaultValueFrom;
                    columnProperties.DefaultValueTo = recDB.DefaultValueTo;
                    columnProperties.PrecisionFieldJoinFrom = recDB.PrecisionFieldJoinFrom;
                    columnProperties.PrecisionFieldJoinTo = recDB.PrecisionFieldJoinTo;

                    if (recDB.TypeValue == Orm.VarType.var_Int)
                    {
                        columnProperties.TypeColumn = typeof(int);
                    }
                    else if (recDB.TypeValue == Orm.VarType.var_String)
                    {
                        columnProperties.TypeColumn = typeof(string);
                    }
                    else if (recDB.TypeValue == Orm.VarType.var_Bytes)
                    {
                        columnProperties.TypeColumn = typeof(byte[]);
                    }
                    else if (recDB.TypeValue == Orm.VarType.var_Dou)
                    {
                        columnProperties.TypeColumn = typeof(double);
                    }
                    else if (recDB.TypeValue == Orm.VarType.var_Flo)
                    {
                        columnProperties.TypeColumn = typeof(float);
                    }
                    else if (recDB.TypeValue == Orm.VarType.var_Guid)
                    {
                        columnProperties.TypeColumn = typeof(Guid);
                    }
                    else if (recDB.TypeValue == Orm.VarType.var_Tim)
                    {
                        columnProperties.TypeColumn = typeof(DateTime);
                    }
                    else if (recDB.TypeValue == Orm.VarType.var_Null)
                    {
                        columnProperties.TypeColumn = typeof(int);
                    }
                    else
                    {
                        throw new NotImplementedException("Not supported type for linked column");
                    }
                    propertyField.Add(columnProperties);
                }
                if (Spl.Length > 0)
                {
                    recDB.NameFieldForSetValue = Spl[Spl.Length - 1];
                    var columnProperties = new ColumnProperties();
                    columnProperties.FieldJoinFrom = recDB.FieldJoinFrom;
                    columnProperties.FieldJoinTo = recDB.FieldJoinTo;
                    columnProperties.Name = recDB.Name;
                    columnProperties.NameField = recDB.NameFieldForSetValue;
                    columnProperties.NameTableFrom = recDB.NameTableFrom;
                    columnProperties.NameTableTo = recDB.NameTableTo;
                    columnProperties.DefaultValueFrom = recDB.DefaultValueFrom;
                    columnProperties.DefaultValueTo = recDB.DefaultValueTo;
                    columnProperties.PrecisionFieldJoinFrom = recDB.PrecisionFieldJoinFrom;
                    columnProperties.PrecisionFieldJoinTo = recDB.PrecisionFieldJoinTo;
                    if (recDB.TypeValue == Orm.VarType.var_Int)
                    {
                        columnProperties.TypeColumn = typeof(int);
                    }
                    else if (recDB.TypeValue == Orm.VarType.var_String)
                    {
                        columnProperties.TypeColumn = typeof(string);
                    }
                    else if (recDB.TypeValue == Orm.VarType.var_Bytes)
                    {
                        columnProperties.TypeColumn = typeof(byte[]);
                    }
                    else if (recDB.TypeValue == Orm.VarType.var_Dou)
                    {
                        columnProperties.TypeColumn = typeof(double);
                    }
                    else if (recDB.TypeValue == Orm.VarType.var_Flo)
                    {
                        columnProperties.TypeColumn = typeof(float);
                    }
                    else if (recDB.TypeValue == Orm.VarType.var_Guid)
                    {
                        columnProperties.TypeColumn = typeof(Guid);
                    }
                    else if (recDB.TypeValue == Orm.VarType.var_Tim)
                    {
                        columnProperties.TypeColumn = typeof(DateTime);
                    }
                    else if (recDB.TypeValue == Orm.VarType.var_Null)
                    {
                        columnProperties.TypeColumn = typeof(int);
                    }
                    else
                    {
                        throw new NotImplementedException("Not supported type for linked column");
                    }
                    propertyField.Add(columnProperties);
                }
            }
            else
            {
                recDB.NameTableTo = tableName;
                recDB.NameFieldForSetValue = fld;
                var columnProperties = new ColumnProperties();
                columnProperties.FieldJoinFrom = recDB.FieldJoinFrom;
                columnProperties.FieldJoinTo = recDB.FieldJoinTo;
                columnProperties.Name = recDB.Name;
                columnProperties.NameField = recDB.NameFieldForSetValue;
                columnProperties.NameTableFrom = recDB.NameTableFrom;
                columnProperties.NameTableTo = recDB.NameTableTo;
                columnProperties.DefaultValueFrom = recDB.DefaultValueFrom;
                columnProperties.DefaultValueTo = recDB.DefaultValueTo;
                columnProperties.PrecisionFieldJoinFrom = recDB.PrecisionFieldJoinFrom;
                columnProperties.PrecisionFieldJoinTo = recDB.PrecisionFieldJoinTo;
                if (recDB.TypeValue == Orm.VarType.var_Int)
                {
                    columnProperties.TypeColumn = typeof(int);
                }
                else if (recDB.TypeValue == Orm.VarType.var_String)
                {
                    columnProperties.TypeColumn = typeof(string);
                }
                else if (recDB.TypeValue == Orm.VarType.var_Bytes)
                {
                    columnProperties.TypeColumn = typeof(byte[]);
                }
                else if (recDB.TypeValue == Orm.VarType.var_Dou)
                {
                    columnProperties.TypeColumn = typeof(double);
                }
                else if (recDB.TypeValue == Orm.VarType.var_Flo)
                {
                    columnProperties.TypeColumn = typeof(float);
                }
                else if (recDB.TypeValue == Orm.VarType.var_Guid)
                {
                    columnProperties.TypeColumn = typeof(Guid);
                }
                else if (recDB.TypeValue == Orm.VarType.var_Tim)
                {
                    columnProperties.TypeColumn = typeof(DateTime);
                }
                else if (recDB.TypeValue == Orm.VarType.var_Null)
                {
                    columnProperties.TypeColumn = typeof(int);
                }
                else
                {
                    throw new NotImplementedException("Not supported type for linked column");
                }
                propertyField.Add(columnProperties);
            }
            return propertyField.ToArray();
        }


        public  Orm.FieldF GetFieldFromOrm(string tableName, string fld)
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
                if (x1 > 0)
                {
                    Query = Query.Remove(0, x1 + 2);
                    int x2 = Query.IndexOf("\r\n");
                    Query = Query.Remove(0, x2 + 2);
                }
                else
                {
                    x1 = Query.IndexOf("\n");
                    if (x1 > 0)
                    {
                        Query = Query.Remove(0, x1 + 1);
                        int x2 = Query.IndexOf("\n");
                        Query = Query.Remove(0, x2 + 1);
                    }
                }
                var strx = new InChannelString(Query);
                f.Load(strx);
                var _report = new IcsmReport();
                _report.SetConfig(f);
                List<ColumnProperties> listColumnProperties = new List<ColumnProperties>();
                List<KeyValuePair<string, string>> listColumnsFromSchema = new List<KeyValuePair<string, string>>();
                var zeta = this._schemasMetadata.GetTableByName(_report.m_dat.m_tab);
                if (zeta != null)
                {
                    if (!string.IsNullOrEmpty(zeta.ShortDesc))
                    {
                        string[] blocks = zeta.ShortDesc.Split(new char[] { '|' });
                        for (int i = 0; i < blocks.Length; i++)
                        {
                            if (!string.IsNullOrEmpty(blocks[i]))
                            {
                                string[] wrds = blocks[i].Split(new char[] { ';' });
                                if (wrds.Length > 1)
                                {
                                    listColumnsFromSchema.Add(new KeyValuePair<string, string>(wrds[0], wrds[1]));
                                }
                            }
                        }
                    }
                }
                for (int i = 0; i < _report.m_dat.m_list[0].m_query.lq.Length; i++)
                {

                    var metaData = new IrpColumn();
                    metaData.columnMeta = new ColumnMetadata();
                    metaData.columnMeta.Description = _report.m_desc;
                    metaData.columnMeta.Title = "";
                    irpDescr.PrimaryKey = GetPrimaryKeys(_report.m_dat.m_tab);
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
                        metaData.columnMeta.Name = t;
                        metaData.columnMeta.Description = t;
                        metaData.Expr = "$" + _report.m_dat.m_list[0].m_query.lq[i].m_CustExpr + "#:" + t;
                        metaData.TypeColumn = IrpColumnEnum.Expression;
                        metaData.columnMeta.Type = DataModels.DataType.String;
                    }
                    else
                    {
                        metaData.Expr = "";
                        metaData.TypeColumn = IrpColumnEnum.StandardColumn;
                    }
                    metaData.columnProperties = GetPropertyFieldFromOrm(_report.m_dat.m_tab, t);
                    listColumnProperties.AddRange(metaData.columnProperties);
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
                    bool isDefinedRealType = false;
                    {
                        if (listColumnsFromSchema.Count > 0)
                        {
                            KeyValuePair<string, string> fndColumn = listColumnsFromSchema.Find(z => z.Key == t);
                            if (!string.IsNullOrEmpty(fndColumn.Value))
                            {
                                isDefinedRealType = true;
                                string typeRecognize = fndColumn.Value.Trim();
                                switch (typeRecognize)
                                {
                                    case "System.Int32":
                                        metaData.columnMeta.Type = DataModels.DataType.Integer;
                                        break;
                                    case "System.Boolean":
                                        metaData.columnMeta.Type = DataModels.DataType.Boolean;
                                        break;
                                    case "System.Byte":
                                        metaData.columnMeta.Type = DataModels.DataType.Byte;
                                        break;
                                    case "System.Decimal":
                                        metaData.columnMeta.Type = DataModels.DataType.Decimal;
                                        break;
                                    case "System.Double":
                                        metaData.columnMeta.Type = DataModels.DataType.Double;
                                        break;
                                    case "System.String":
                                        metaData.columnMeta.Type = DataModels.DataType.String;
                                        break;
                                    case "System.DateTime":
                                        metaData.columnMeta.Type = DataModels.DataType.DateTime;
                                        break;
                                    case "System.Byte[]":
                                        metaData.columnMeta.Type = DataModels.DataType.Bytes;
                                        break;
                                    case "System.Guid":
                                        metaData.columnMeta.Type = DataModels.DataType.Guid;
                                        break;
                                    case "System.Single":
                                        metaData.columnMeta.Type = DataModels.DataType.Float;
                                        break;
                                    default:
                                        metaData.columnMeta.Type = DataModels.DataType.Undefined;
                                        break;
                                }
                            }
                        }
                    }
                    if ((ty_p != null) && (isDefinedRealType == false))
                    {
                        switch (ty_p.DDesc.ClassType)
                        {
                            case Orm.VarType.var_Guid:
                                metaData.columnMeta.Type = DataModels.DataType.Guid;
                                break;
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
                                metaData.columnMeta.Type = DataModels.DataType.Undefined;
                                break;
                        }
                    }
                    listDescrColumns.Add(metaData);

                }
                var allTables = GetAllTables(listColumnProperties);
                irpDescr.MandatoryColumns = GetMandatoryFieldsFromTables(allTables);
                irpDescr.PrimaryColumns = GetPrimaryFieldsFromTables(allTables);
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
