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
            StockItems PS = new StockItems();
            QueryMetaD QDM = new QueryMetaD();
            List<SettingIRPClass> Las_NameCat = (List<SettingIRPClass>)PS.GetAvailableStocksSettingIRP(false);
            List<QueryMetaD> LQD = (List<QueryMetaD>)PS.GetCacheKeyMetaData(false, options.OtherArgs.UserId);
            if (LQD != null) QDM = LQD.Find(t => t.settIRP.ID == options.QueryRef.Id);
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
