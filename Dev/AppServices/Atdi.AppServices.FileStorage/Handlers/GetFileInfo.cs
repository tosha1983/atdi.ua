using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.Identity;
using Atdi.Contracts.LegacyServices.Icsm;
using Atdi.DataModels.Identity;
using Atdi.DataModels.WebQuery;
using Atdi.Platform.Logging;
using Atdi.DataModels;

namespace Atdi.AppServices.FileStorage.Handlers
{
    public sealed class GetFileInfo : LoggedObject
    {
        private readonly IUserTokenProvider _tokenProvider;
        private readonly IQueryExecutor _queryExecutor;
        private ILogger _logger { get; set; }
        private readonly IDataLayer<IcsmDataOrm> _dataLayer;
        private readonly string _tableNameDocFiles;
        private readonly string _tableNameDocLink;
        private ConfigFileStorage _configFileStorage { get; set; }

        public GetFileInfo(IUserTokenProvider tokenProvider, IDataLayer<IcsmDataOrm> dataLayer, ConfigFileStorage configFileStorage, ILogger logger) : base(logger)
        {
            this._logger = logger;
            this._tokenProvider = tokenProvider;
            this._dataLayer = dataLayer;
            this._queryExecutor = this._dataLayer.Executor<IcsmDataContext>();
            this._tableNameDocFiles = "DOCFILES";
            this._tableNameDocLink = "DOCLINK";
            this._configFileStorage = configFileStorage;
        }

        private IQuerySelectStatement GetQuerySelectStatementFromDocFiles(int id)
        {
            return this._dataLayer.Builder
              .From(this._tableNameDocFiles)
              .Select(new string[] { "ID", "NAME", "MEMO", "PATH", "DOC_DATE", "DATE_CREATED", "CREATED_BY", "DOC_TYPE" })
              .OrderByAsc(new string[] { "ID" })
              .Where("ID", id);
        }

        private IQuerySelectStatement GetQuerySelectStatementFromDocLink(int id)
        {
            return this._dataLayer.Builder
              .From(this._tableNameDocLink)
              .Select(new string[] { "REC_TAB", "REC_ID", "DOC_ID", "PATH" })
              .OrderByAsc(new string[] { "DOC_ID" })
              .Where("DOC_ID", id);
        }

        public FileInfo Handle(UserToken userToken, int id)
        {
            using (this.Logger.StartTrace(Contexts.WebQueryAppServices, Categories.Handling, TraceScopeNames.GetFileInfo))
            {

                if (userToken == null) throw new ArgumentNullException(nameof(userToken));

                var tokenData = this._tokenProvider.UnpackUserToken(userToken);

                // Here put code of read file information from ICSM DB by Id

                var fileInfo = new FileInfo();

                var statementDocFiles = GetQuerySelectStatementFromDocFiles(id);
                string nameDoc = ""; string memoDoc = ""; string pathDoc = ""; string createdByDoc = ""; string docType = "";
                DateTime? dateCreatedDoc = null;
                var dataSetDocFiles = this._queryExecutor.Fetch(statementDocFiles, handler =>
                {
                    while (handler.Read())
                    {
                        var nameDocObject = handler.GetValue(DataType.String, "NAME");
                        if (nameDocObject != null)
                        {
                            nameDoc = nameDocObject.ToString();
                        }
                        var memoDocObject = handler.GetValue(DataType.String, "MEMO");
                        if (memoDocObject != null)
                        {
                            memoDoc = memoDocObject.ToString();
                        }
                        var pathDocObject = handler.GetValue(DataType.String, "PATH");
                        if (pathDocObject != null)
                        {
                            pathDoc = pathDocObject.ToString();
                        }
                        var createdByDocObject = handler.GetValue(DataType.String, "CREATED_BY");
                        if (createdByDocObject != null)
                        {
                            createdByDoc = createdByDocObject.ToString();
                        }
                        var dateCreatedDocObject = handler.GetValue(DataType.DateTime, "DATE_CREATED");
                        if (dateCreatedDocObject != null)
                        {
                            dateCreatedDoc = Convert.ToDateTime(dateCreatedDocObject);
                        }
                        var docTypeObject = handler.GetValue(DataType.String, "DOC_TYPE");
                        if (docTypeObject != null)
                        {
                            docType = docTypeObject.ToString();
                        }
                    }
                    return true;
                });


                var statementDocLink = GetQuerySelectStatementFromDocLink(id);
                var dataSetDocLink = this._queryExecutor.Fetch(statementDocLink, handler =>
                {
                    while (handler.Read())
                    {
                        var dirDocObject = handler.GetValue(DataType.String, "PATH");
                        if (dirDocObject != null)
                        {
                            pathDoc = System.IO.Path.GetDirectoryName(dirDocObject.ToString())+"\\"+ System.IO.Path.GetFileName(pathDoc);
                            break;
                        }
                    }
                    return true;
                });


                if (!string.IsNullOrEmpty(this._configFileStorage.WorkFolder))
                {
                    pathDoc = System.IO.Path.GetDirectoryName(this._configFileStorage.WorkFolder) + "\\" + System.IO.Path.GetFileName(pathDoc);
                }


                fileInfo.FileExtension = System.IO.Path.GetExtension(pathDoc);
                fileInfo.Id = id;
                if (!string.IsNullOrEmpty(pathDoc))
                {
                    fileInfo.FileName = System.IO.Path.GetFileNameWithoutExtension(pathDoc);
                    fileInfo.PathDocument = System.IO.Path.GetDirectoryName(pathDoc);
                    fileInfo.Size = (int)(new System.IO.FileInfo(pathDoc)).Length;
                }
                if (dateCreatedDoc != null)
                {
                    fileInfo.DateCreated = dateCreatedDoc.Value;
                }
                if (!string.IsNullOrEmpty(docType))
                {
                    fileInfo.TypeDocument = docType;
                }
                if (!string.IsNullOrEmpty(createdByDoc))
                {
                    fileInfo.CreatedBy = createdByDoc;
                }
                if (!string.IsNullOrEmpty(memoDoc))
                {
                    fileInfo.DocMemo = memoDoc;
                }
                if (!string.IsNullOrEmpty(nameDoc))
                {
                    fileInfo.DocName = nameDoc;
                }
                return fileInfo;
            }
        }
    }
}


