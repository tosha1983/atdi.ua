using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Atdi.Contracts.AppServices.WebQuery;
using Atdi.DataModels.Identity;
using Atdi.DataModels.WebQuery;
using Atdi.Platform.Logging;
using FileInfo = Atdi.DataModels.WebQuery.FileInfo;


namespace Atdi.WebApiServices.WebQuery.Controllers
{
    public class FileStorageController : WebApiController
    {
        public class FileStorageOptions
        {
            public UserToken UserToken { get; set; }
        }

        public class GetFileInfoOptions : FileStorageOptions
        {
            public int Id { get; set; }
        }

        public class GetFilesInfoOptions : FileStorageOptions
        {
            public int[] Ids { get; set; }
        }

        public class GetFileOptions : FileStorageOptions
        {
            public int Id { get; set; }
        }

        public class GetFilePartOptions : FileStorageOptions
        {
            public int Id { get; set; }

            public int From { get; set; }

            public int To { get; set; }
        }

        private readonly IFileStorage _fileStorageAppServices;

        public FileStorageController(IFileStorage fileStorageAppServices, ILogger logger) : base(logger)
        {
            _fileStorageAppServices = fileStorageAppServices;
        }

        [HttpPost]
        public FileInfo GetFileInfo(GetFileInfoOptions options)
        {
            ValidateOptions(options);

            using (this.Logger.StartTrace(Contexts.FileStorage, Categories.OperationCall, TraceScopeNames.GetFileInfo))
            {
                var result = this._fileStorageAppServices.GetFileInfo(options.UserToken, options.Id);
                return result;
            }
        }

        [HttpPost]
        public FileInfo[] GetFilesInfo(GetFilesInfoOptions options)
        {
            ValidateOptions(options);

            using (this.Logger.StartTrace(Contexts.FileStorage, Categories.OperationCall, TraceScopeNames.GetFilesInfo))
            {
                var result = this._fileStorageAppServices.GetFilesInfo(options.UserToken, options.Ids);
                return result;
            }
        }

        [HttpPost]
        public FileContent GetFile(GetFileOptions options)
        {
            ValidateOptions(options);

            using (this.Logger.StartTrace(Contexts.FileStorage, Categories.OperationCall, TraceScopeNames.GetFile))
            {
                var result = this._fileStorageAppServices.GetFile(options.UserToken, options.Id);
                return result;
            }
        }

        [HttpPost]
        public byte[] GetFilePart(GetFilePartOptions options)
        {
            ValidateOptions(options);

            using (this.Logger.StartTrace(Contexts.FileStorage, Categories.OperationCall, TraceScopeNames.GeFilePart))
            {
                var result = this._fileStorageAppServices.GeFilePart(options.UserToken, options.Id, options.From, options.To);
                return result;
            }
        }

        [HttpGet]
        public HttpResponseMessage File(string userToken, int id)
        {
            if (userToken == null) throw new ArgumentNullException(nameof(userToken));
            if (userToken.Length == 0)
                throw new ArgumentException("Value cannot be an empty collection.", nameof(userToken));


            using (this.Logger.StartTrace(Contexts.FileStorage, Categories.OperationCall, TraceScopeNames.File))
            {
                var content = this._fileStorageAppServices.GetFile(new UserToken{ Data = Convert.FromBase64String(userToken) },id);

                var httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);
                httpResponseMessage.Content = new StreamContent(new MemoryStream(content.Body));
                httpResponseMessage.Content.Headers.ContentDisposition =
                    new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = Path.ChangeExtension(content.FileName, content.FileExtension)
                    };
                httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                return httpResponseMessage;
            }

            
        }

        private static void ValidateOptions(FileStorageOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (options.UserToken == null)
            {
                throw new ArgumentNullException(nameof(options.UserToken));
            }
        }
    }
}
