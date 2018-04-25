using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

using Atdi.AppServer.Contracts;
using Atdi.AppServer.Contracts.WebQuery;
using Atdi.AppServer.Models.TechServices;
using Atdi.AppServer.Models.AppServices;
using Atdi.AppServer.Models.AppServices.WebQueryManager;

namespace Atdi.AppServer.Services.WebQuery
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class WebQueryManagerService : WcfServiceBase<WebQueryManagerAppService, IWebQueryManager>, IWebQueryManager
    {
        public WebQueryManagerService(IAppServiceInvokerFactory factory, ILogger logger) : base(factory, logger)
        {
        }

        CommonOperationDataResult<int> IWebQueryManager.AuthenticateUser(string userName, string password)
        {
            var result =
                Operation<WebQueryManagerAppService.AuthenticateUserAppOperation, int>()
                    .Invoke(
                        new AuthenticateUserAppOperationOptions
                        {
                            Password = password,
                            UserName = userName
                        },
                        this.OperationContext
                    );

            if (result > 0)
            {
                return new CommonOperationDataResult<int>()
                {
                    State = CommonOperationState.Success,
                    Data = result
                };
            }
            return new CommonOperationDataResult<int>()
            {
                State = CommonOperationState.Fault,
                Data = 0,
                FaultCause = "Incorrect user nam eor password"
            };
        }

        QueryResult IWebQueryManager.ExecuteQuery(QueryOptions options, CommonOperationArguments otherArgs)
        {
            var result =
                Operation<WebQueryManagerAppService.ExecuteQueryAppOperation, QueryResult>()
                    .Invoke(
                        new ExecuteQueryAppOperationOptions
                        {
                            Options = options,
                            OtherArgs = otherArgs
                        },
                        this.OperationContext
                    );

            return result;
        }

        PageMetadata IWebQueryManager.GetPageMetadata(CommonOperationArguments otherArgs)
        {
            var result =
                Operation<WebQueryManagerAppService.GetPageMetadataAppOperation , PageMetadata>()
                    .Invoke(
                        new GetPageMetadataAppOperationOptions
                        {
                            OtherArgs = otherArgs
                        },
                        this.OperationContext
                    );

            return result;
        }

        QueryMetadata IWebQueryManager.GetQueryMetadata(QueryReference queryRef, CommonOperationArguments otherArgs)
        {
            var result =
                Operation<WebQueryManagerAppService.GetQueryMetadataAppOperation, QueryMetadata>()
                    .Invoke(
                        new GetQueryMetadataAppOperationOptions
                        {
                            QueryRef = queryRef,
                            OtherArgs = otherArgs
                        },
                        this.OperationContext
                    );

            return result;
        }

        QueryTree IWebQueryManager.GetQueryTree(CommonOperationArguments otherArgs)
        {
            var result =
                Operation<WebQueryManagerAppService.GetQueryTreeAppOperation, QueryTree>()
                    .Invoke(
                        new GetQueryTreeAppOperationOptions
                        {
                            OtherArgs = otherArgs
                        },
                        this.OperationContext
                    );

            return result;
        }

        QueryChangesResult IWebQueryManager.SaveChanges(QueryChangeset changeset, CommonOperationArguments otherArgs)
        {
            var result =
                Operation<WebQueryManagerAppService.SaveChangesAppOperation, QueryChangesResult>()
                    .Invoke(
                        new SaveChangesAppOperationOptions
                            {
                                Changeset = changeset,
                                OtherArgs = otherArgs
                            }, 
                        this.OperationContext
                    );

            return result;
        }
    }
}
