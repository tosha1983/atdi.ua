using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppServer.Models.AppServices;
using Atdi.AppServer.Models.AppServices.WebQueryManager;
using Atdi.AppServer.Contracts.WebQuery;

namespace Atdi.AppServer.AppServices.WebQueryManager
{
    public class GetPageMetadataAppOperationHandler
        : AppOperationHandlerBase
        <
            WebQueryManagerAppService,
            WebQueryManagerAppService.GetPageMetadataAppOperation,
            GetPageMetadataAppOperationOptions,
            PageMetadata
        >
    {
        public GetPageMetadataAppOperationHandler(IAppServerContext serverContext, ILogger logger) : base(serverContext, logger)
        {
        }

        public override PageMetadata Handle(GetPageMetadataAppOperationOptions options, IAppOperationContext operationContext)
        {
            PageMetadata pageMeta = new PageMetadata();
            pageMeta.PageStyle = new PageStyle();
            pageMeta.PageStyle.BackColor = "White";
            pageMeta.PageStyle.ForeColor = "Black";
            Logger.Trace(this, options, operationContext);
            return pageMeta;
        }
    }
}
