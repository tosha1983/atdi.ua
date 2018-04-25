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
    public class GetQueryMetadataAppOperationHandler
        : AppOperationHandlerBase
            <
                WebQueryManagerAppService,
                WebQueryManagerAppService.GetQueryMetadataAppOperation,
                GetQueryMetadataAppOperationOptions,
                QueryMetadata
            >
    {
        public GetQueryMetadataAppOperationHandler(IAppServerContext serverContext, ILogger logger) : base(serverContext, logger)
        {
        }

        public override QueryMetadata Handle(GetQueryMetadataAppOperationOptions options, IAppOperationContext operationContext)
        {
            QueryMetadata metaData = new QueryMetadata();
            try { 
            ConnectDB conn = new ConnectDB();
            Class_ManageSettingInstance v_s = new Class_ManageSettingInstance();
            List<SettingIRPClass> Las_NameCat = Class_ManageSettingInstance.GetSettingWebQuery("XWEB_QUERY");
            QueryMetaD QDM = v_s.GetQueryMetaData(Las_NameCat, options.QueryRef.Id, options.OtherArgs.UserId);
            List<ColumnMetadata> colMeta = new List<ColumnMetadata>();
            if (QDM != null) {
                if (QDM.Columns != null)
                {
                    foreach (ColumnMetaD mdf in QDM.Columns)
                    {
                        ColumnMetadata metaCol = new ColumnMetadata();
                        metaCol.Description = mdf.Description;
                        metaCol.Format = mdf.Format;
                        metaCol.JsonOptions = "";
                        metaCol.Order = mdf.Order;
                        metaCol.Position = 0;
                        metaCol.Rank = mdf.Rank;
                        metaCol.Show = mdf.Show;
                        metaCol.Style = new ColumnStyle();
                        metaCol.Title = mdf.Title;
                        if (mdf.Type == typeof(int)) metaCol.Type = Contracts.CommonDataType.Integer;
                        else if (mdf.Type == typeof(double)) metaCol.Type = Contracts.CommonDataType.Double;
                        else if (mdf.Type == typeof(string)) metaCol.Type = Contracts.CommonDataType.String;
                        else if (mdf.Type == typeof(float)) metaCol.Type = Contracts.CommonDataType.Double;
                        else if (mdf.Type == typeof(bool)) metaCol.Type = Contracts.CommonDataType.Boolean;
                        else if (mdf.Type == typeof(DateTime)) metaCol.Type = Contracts.CommonDataType.DateTime;
                        else if (mdf.Type == typeof(byte)) metaCol.Type = Contracts.CommonDataType.Bytes;
                        metaCol.Width = mdf.Width;
                        colMeta.Add(metaCol);
                    }
                }



                //Описание параметров запроса (Имя поля - значение)  (НА ТЕКУЩИЙ МОМЕНТ НИЧЕГО НЕ ПЕРЕДАЮ)
                //List<QueryParameter> Qparams = new List<QueryParameter>();
                //QueryParameter Qpar = new QueryParameter();
                //Qpar.Name = "";
                //Qpar.Value = "";
                //Qparams.Add(Qpar);



                //Таблица стилей (НА ТЕКУЩИЙ МОМЕНТ НИЧЕГО НЕ ПЕРЕДАЮ)
                /*
                QueryTableStyle QStyle = new QueryTableStyle();
                QStyle.BackColor = "";
                QStyle.FontName = "";
                QStyle.FontSize = 12;
                QStyle.FontStyle = "";
                QStyle.ForeColor = "";
                */


                metaData.Columns = colMeta.ToArray();
                metaData.Description = QDM.Description;
                metaData.JsonOptions = "";
                metaData.Name = QDM.Name;
                metaData.Parameters = null;
                metaData.QueryRef = options.QueryRef;
                metaData.TableStyle = new QueryTableStyle();
                metaData.Techno = QDM.Techno;
                metaData.Title = QDM.Title;
            }
            Logger.Trace(this, options, operationContext);
            }
            catch (Exception ex) { Logger.Error(ex); }
            return metaData;
        }
    }
}
