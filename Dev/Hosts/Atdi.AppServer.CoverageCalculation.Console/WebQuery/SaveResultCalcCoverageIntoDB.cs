using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using System.Text;
using System.IO;
using Atdi.DataModels.CoverageCalculation;
using Atdi.DataModels.DataConstraint;
using Atdi.DataModels.WebQuery;
using Atdi.DataModels.CommonOperation;
using Atdi.DataModels;
using System.ServiceModel;
using Atdi.Contracts.WcfServices.Identity;
using Atdi.Contracts.WcfServices.WebQuery;
using Atdi.DataModels.Identity;
using Atdi.AppServer.CoverageCalculation;
using Atdi.Platform.Logging;

namespace Atdi.WebQuery.CoverageCalculation
{
    public class SaveResultCalcCoverageIntoDB
    {
        private IWebQuery _webQuery  { get; set; }
        private UserToken _userToken  { get; set; }
        private QueryToken _queryToken { get; set; }
        private string _imageFile { get; set; }
        private ILogger _logger { get; set; }

        public SaveResultCalcCoverageIntoDB(IWebQuery webQuery, UserToken userToken, QueryToken queryToken, string imageFile, ILogger logger)
        {
            this._webQuery = webQuery;
            this._userToken = userToken;
            this._queryToken = queryToken;
            this._imageFile = imageFile;
            this._logger = logger;
        }

        public static byte[] ImageConversionToBytes(string imageName)
        {
            var fs = new FileStream(imageName, FileMode.Open, FileAccess.Read);
            var imgByteArr = new byte[fs.Length];
            fs.Read(imgByteArr, 0, Convert.ToInt32(fs.Length));
            fs.Close();
            return imgByteArr;
        }

        public bool SaveImageToBlob(string provinceName)
        {
            if (System.IO.File.Exists(this._imageFile))
            {
                var changeSet = new DataModels.Changeset()
                {
                    Actions = new DataModels.Action[]
                    {
                   new ObjectRowCreationAction
                   {
                     Id = Guid.NewGuid(),
                     Columns = new DataSetColumn[]
                     {
                        new DataSetColumn
                        {
                          Name = "ID", Type = DataModels.DataType.Long, Index = 0
                        },
                        new DataSetColumn
                        {
                          Name = "RESULT_COVERAGE", Type = DataModels.DataType.Bytes, Index = 1
                        },
                        new DataSetColumn
                        {
                          Name = "FILE_NAME", Type = DataModels.DataType.String, Index = 2
                        },
                        new DataSetColumn
                        {
                          Name = "DATE_CREATED", Type = DataModels.DataType.DateTime, Index = 3
                        },
                        new DataSetColumn
                        {
                          Name = "PROVINCE", Type = DataModels.DataType.String, Index = 4
                        }
                     },
                     Row = new ObjectDataRow
                     {
                        Cells = new object[]
                        {
                           1, ImageConversionToBytes(this._imageFile), System.IO.Path.GetFileName(this._imageFile), DateTime.Now, provinceName
                        }
                     }
                   }
                    }
                };
                var chsResult = this._webQuery.SaveChanges(this._userToken, this._queryToken, changeSet);
                this._logger.Info(Contexts.CalcCoverages, string.Format(Events.OperationSaveImageFileCompleted.ToString(), this._imageFile));
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
