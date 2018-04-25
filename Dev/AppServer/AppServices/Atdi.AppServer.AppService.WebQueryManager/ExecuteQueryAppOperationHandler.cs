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
using Atdi.AppServer.Contracts;

namespace Atdi.AppServer.AppServices.WebQueryManager
{
    public class ExecuteQueryAppOperationHandler
        : AppOperationHandlerBase
        <
            WebQueryManagerAppService,
            WebQueryManagerAppService.ExecuteQueryAppOperation,
            ExecuteQueryAppOperationOptions,
            QueryResult
        >
    {
        public ExecuteQueryAppOperationHandler(IAppServerContext serverContext, ILogger logger) : base(serverContext, logger)
        {

        }

        public override QueryResult Handle(ExecuteQueryAppOperationOptions options, IAppOperationContext operationContext)
        {
            QueryResult QResult = new QueryResult();
            try { 
            string decimal_sep = System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
            Dictionary<string, object> QueryParams = options.OtherArgs.Values;
            ConnectDB conn = new ConnectDB();
            Class_ManageSettingInstance v_s = new Class_ManageSettingInstance();
            List<SettingIRPClass> Las_NameCat = Class_ManageSettingInstance.GetSettingWebQuery("XWEB_QUERY");
            string sql_params = v_s.GenerateQueryFromParams(options.OtherArgs.Values, v_s.GetQueryMetaData(Las_NameCat, options.Options.QueryRef.Id, options.OtherArgs.UserId));
            List<string> List_Id_Users = new List<string>();
            List_Id_Users = conn.GetRegistNum(((WebQueryManagerAppOperationOptionsBase)options).OtherArgs.UserId);
            Class_IRP_Object Irp = v_s.ExecuteSQL("([ID]>0) " + sql_params, List_Id_Users, "XWEB_QUERY", Las_NameCat, options.Options.QueryRef.Id, ((WebQueryManagerAppOperationOptionsBase)options).OtherArgs.UserId.ToString());
            QResult.TotalRowCount = (uint)Irp.Val_Arr.Count;
            QResult.ResultRowCount = (uint)Irp.Val_Arr.Count;
            QResult.QueryRef = new QueryReference();
            QResult.QueryRef.Id = options.Options.QueryRef.Id;
            QResult.QueryRef.Version = options.Options.QueryRef.Version;
            List<uint> indexMap = new List<uint>();
            List<Contracts.CommonDataType> CDataType = new List<Contracts.CommonDataType>();
            List<Type> rtv = ConnectDB.GetAllTypesFromFlds(Irp.TABLE_NAME, Irp.FLD);
            if (Irp.Setting_param.IS_SQL_REQUEST) rtv = Irp.FLD_TYPE;
            int int_idx = 0;
            int double_idx = 0;
            int string_idx = 0;
            int float_idx = 0;
            int bool_idx = 0;
            int DateTime_idx = 0;
            int byte_idx = 0;
            for (int i = 0; i < rtv.Count; i++) {
                if (rtv[i] == typeof(int)) { CDataType.Add(Contracts.CommonDataType.Integer); indexMap.Add((uint)int_idx); int_idx++; }
                else if (rtv[i] == typeof(double)) { CDataType.Add(Contracts.CommonDataType.Double); indexMap.Add((uint)double_idx); double_idx++; }
                else if (rtv[i] == typeof(string)) {  CDataType.Add(Contracts.CommonDataType.String); indexMap.Add((uint)string_idx); string_idx++; }
                else if (rtv[i] == typeof(float)) {  CDataType.Add(Contracts.CommonDataType.Double); indexMap.Add((uint)float_idx); float_idx++; }
                else if (rtv[i] == typeof(bool)) {  CDataType.Add(Contracts.CommonDataType.Boolean); indexMap.Add((uint)bool_idx); bool_idx++; }
                else if (rtv[i] == typeof(DateTime)) {  CDataType.Add(Contracts.CommonDataType.DateTime); indexMap.Add((uint)DateTime_idx); DateTime_idx++; }
                else if (rtv[i] == typeof(byte)) {  CDataType.Add(Contracts.CommonDataType.Bytes); indexMap.Add((uint)byte_idx); byte_idx++; }
            }
            QResult.ColumnIndexMap = indexMap.ToArray();
            QResult.ColumnTypeMap = CDataType.ToArray();
            bool?[][] G_BooleanValues = new bool?[Irp.Val_Arr.Count][];
            byte[][] G_BytesValues = new byte[Irp.Val_Arr.Count][];
            DateTime?[][] G_DateTimeValues = new DateTime?[Irp.Val_Arr.Count][];
            double?[][] G_DoubleValues = new double?[Irp.Val_Arr.Count][];
            int?[][] G_IntegerValues = new int?[Irp.Val_Arr.Count][];
            string[][] G_StringValues = new string[Irp.Val_Arr.Count][];
            if (indexMap.Count > 0) QResult.FirstRowIndex = indexMap.Min();
            if (Irp.Val_Arr.Count > 0) {
                if (Irp.Val_Arr[0].Count() == CDataType.Count) {
                    QResult.FirstRowIndex = 0; QResult.ResultRowCount = (uint)Irp.Val_Arr.Count(); QResult.TotalRowCount = (uint)Irp.Val_Arr.Count();
                    for (int i = 0; i < Irp.Val_Arr.Count(); i++) {
                        G_BooleanValues[i] = new bool?[CDataType.Count()];
                        G_BytesValues[i] = new byte[CDataType.Count()];
                        G_DateTimeValues[i] = new DateTime?[CDataType.Count()];
                        G_DoubleValues[i] = new double?[CDataType.Count()];
                        G_IntegerValues[i] = new int?[CDataType.Count()];
                        G_StringValues[i] = new string[CDataType.Count()];

                        List<bool?> BooleanValues = new List<bool?>();
                        List<byte> BytesValues = new List<byte>();
                        List<DateTime?> DateTimeValues = new List<DateTime?>();
                        List<double?> DoubleValues = new List<double?>();
                        List<int?> IntegerValues = new List<int?>();
                        List<string> StringValues = new List<string>();

                        for (int j = 0; j < CDataType.Count; j++){
                            if (Irp.Val_Arr[i][j] != null) {
                                if (CDataType[j] == Contracts.CommonDataType.Boolean) BooleanValues.Add(Convert.ToBoolean(Irp.Val_Arr[i][j].ToString()));
                                if (CDataType[j] == Contracts.CommonDataType.Bytes) BytesValues.Add(Convert.ToByte(Irp.Val_Arr[i][j]));
                                if (CDataType[j] == Contracts.CommonDataType.DateTime) if (Irp.Val_Arr[i][j].ToString() == "") DateTimeValues.Add(ConnectDB.NullT); else DateTimeValues.Add(Convert.ToDateTime(Irp.Val_Arr[i][j]));
                                if (CDataType[j] == Contracts.CommonDataType.Double) if (Irp.Val_Arr[i][j].ToString() == "") DoubleValues.Add(ConnectDB.NullD); else DoubleValues.Add(Convert.ToDouble(Irp.Val_Arr[i][j].ToString().Replace(".", decimal_sep).Replace(",", decimal_sep)));
                                if (CDataType[j] == Contracts.CommonDataType.Integer) if (Irp.Val_Arr[i][j].ToString() == "") IntegerValues.Add(ConnectDB.NullI); else IntegerValues.Add(Convert.ToInt32(Irp.Val_Arr[i][j]));
                                if (CDataType[j] == Contracts.CommonDataType.String) StringValues.Add(Convert.ToString(Irp.Val_Arr[i][j]));
                            }
                        }

                        G_BooleanValues[i] = BooleanValues.ToArray();
                        G_BytesValues[i] = BytesValues.ToArray();
                        G_DateTimeValues[i] = DateTimeValues.ToArray();
                        G_DoubleValues[i] = DoubleValues.ToArray();
                        G_IntegerValues[i] = IntegerValues.ToArray();
                        G_StringValues[i] = StringValues.ToArray();
                    }
                }
            }
            QResult.BooleanValues = G_BooleanValues;
            //QResult.BytesValues = G_BytesValues;
            QResult.DateTimeValues = G_DateTimeValues;
            QResult.DoubleValues = G_DoubleValues;
            QResult.IntegerValues = G_IntegerValues;
            QResult.StringValues = G_StringValues;
            Logger.Trace(this, options, operationContext);
            }
            catch (Exception ex) { Logger.Error(ex); }
            return QResult;
        }
    }
}
