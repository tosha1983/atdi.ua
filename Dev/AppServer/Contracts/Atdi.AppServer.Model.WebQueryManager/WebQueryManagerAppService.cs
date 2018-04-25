using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppServer;

namespace Atdi.AppServer.Models.AppServices
{
    public sealed class WebQueryManagerAppService : AppServiceBase
    {
        public sealed class AuthenticateUserAppOperation : AppOperationBase<WebQueryManagerAppService>
        {
            public AuthenticateUserAppOperation() : base("AuthenticateUser")
            { }
        }
        public sealed class GetPageMetadataAppOperation : AppOperationBase<WebQueryManagerAppService>
        {
            public GetPageMetadataAppOperation() : base("GetPageMetadata")
            { }
        }

        public sealed class GetQueryTreeAppOperation : AppOperationBase<WebQueryManagerAppService>
        {
            public GetQueryTreeAppOperation() : base("GetQueryTree")
            { }
        }

        public sealed class ExecuteQueryAppOperation : AppOperationBase<WebQueryManagerAppService>
        {
            public ExecuteQueryAppOperation() : base("ExecuteQuery")
            { }
        }

        public sealed class GetQueryMetadataAppOperation : AppOperationBase<WebQueryManagerAppService>
        {
            public GetQueryMetadataAppOperation() : base("GetQueryMetadata")
            { }
        }

        public sealed class SaveChangesAppOperation : AppOperationBase<WebQueryManagerAppService>
        {
            public SaveChangesAppOperation() : base("SaveChanges")
            { }
        }

        public WebQueryManagerAppService() : base("WebQueryManager")
        {
            this._operations.AddRange(
                 new IAppOperation[] {
                     new AuthenticateUserAppOperation(),
                     new GetPageMetadataAppOperation(),
                     new GetQueryTreeAppOperation(),
                     new ExecuteQueryAppOperation(),
                     new GetQueryMetadataAppOperation(),
                     new SaveChangesAppOperation()
                    }
                );
        }
    }
}
