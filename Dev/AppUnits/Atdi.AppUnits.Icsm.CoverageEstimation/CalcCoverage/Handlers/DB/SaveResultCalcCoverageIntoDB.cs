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
using Atdi.DataModels.DataConstraint;
using Atdi.Platform.Logging;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.LegacyServices.Icsm;
using Atdi.AppUnits.Icsm.CoverageEstimation.Localization;

namespace Atdi.AppUnits.Icsm.CoverageEstimation.Handlers
{
    public class SaveResultCalcCoverageIntoDB
    {

        private string _imageFile { get; set; }
        private ILogger _logger { get; set; }
        private readonly IQueryExecutor _queryExecutor;
        private readonly IDataLayer<IcsmDataOrm> _dataLayer;
        private string _tableName { get; set; }

        public SaveResultCalcCoverageIntoDB(string tableName, IDataLayer<IcsmDataOrm> dataLayer, string imageFile, ILogger logger)
        {
            this._tableName = tableName;
            this._dataLayer = dataLayer;
            this._imageFile = imageFile;
            this._logger = logger;
            this._queryExecutor = this._dataLayer.Executor<IcsmDataContext>();
        }

        private byte[] ImageConversionToBytes(string imageName)
        {
            var fs = new FileStream(imageName, FileMode.Open, FileAccess.Read);
            var imgByteArr = new byte[fs.Length];
            fs.Read(imgByteArr, 0, Convert.ToInt32(fs.Length));
            fs.Close();
            return imgByteArr;
        }

        public bool SaveImageToFile(string provinceName)
        {
            if (File.Exists(this._imageFile))
            {
                var idCoverageEstimation = AllocID(this._tableName);

                var insertWebQuery = _dataLayer.Builder
               .Insert(this._tableName)
               .SetValue("ID", new IntegerValueOperand() { DataType = DataModels.DataType.Integer, Value = idCoverageEstimation })
               //.SetValue("RESULT_COVERAGE", new BytesValueOperand() { DataType = DataModels.DataType.Bytes, Value = ImageConversionToBytes(this._imageFile) })
               .SetValue("FILE_NAME", new StringValueOperand() { DataType = DataModels.DataType.String, Value = Path.GetFileName(this._imageFile) })
               .SetValue("DATE_CREATED", new DateTimeValueOperand() { DataType = DataModels.DataType.DateTime, Value = DateTime.Now })
               .SetValue("PROVINCE", new StringValueOperand() { DataType = DataModels.DataType.String, Value = provinceName });
               this._queryExecutor.Execute(insertWebQuery);


                var idDocFiles = AllocID("DOCFILES");
                var insertDocFiles = _dataLayer.Builder
               .Insert("DOCFILES")
               .SetValue("ID", new IntegerValueOperand() { DataType = DataModels.DataType.Integer, Value = idDocFiles })
               .SetValue("PATH", new StringValueOperand() { DataType = DataModels.DataType.String, Value = Path.GetFileName(this._imageFile) })
               .SetValue("DATE_CREATED", new DateTimeValueOperand() { DataType = DataModels.DataType.DateTime, Value = DateTime.Now })
               .SetValue("CREATED_BY", new StringValueOperand() { DataType = DataModels.DataType.String, Value = "Atdi.AppUnits.Icsm.CoverageEstimation" })
               .SetValue("MEMO", new StringValueOperand() { DataType = DataModels.DataType.String, Value = provinceName })
               .SetValue("DOC_TYPE", new StringValueOperand() { DataType = DataModels.DataType.String, Value = "CE" });
               this._queryExecutor.Execute(insertDocFiles);


               var insertDocLink = _dataLayer.Builder
               .Insert("DOCLINK")
               .SetValue("REC_TAB", new StringValueOperand() { DataType = DataModels.DataType.String, Value = this._tableName })
               .SetValue("REC_ID", new IntegerValueOperand() { DataType = DataModels.DataType.Integer, Value = idCoverageEstimation })
               .SetValue("DOC_ID", new IntegerValueOperand() { DataType = DataModels.DataType.Integer, Value = idDocFiles })
               .SetValue("PATH", new StringValueOperand() { DataType = DataModels.DataType.String, Value = Path.GetDirectoryName(this._imageFile) });

               this._queryExecutor.Execute(insertDocLink);


               this._logger.Info(Contexts.CalcCoverages, string.Format(CLocaliz.TxT(Events.OperationSaveImageFileCompleted.ToString()), this._imageFile));
               return true;
            }
            else
            {
                return false;
            }
        }

        private int AllocID(string table)
        {
            int? NextId = null;
            var QueryNext = _dataLayer.Builder
             .From("NEXT_ID")
             .Where("TABLE_NAME", table)
             .Select(
                 "TABLE_NAME",
                 "NEXT"
                );

            var isNotEmpty = this._queryExecutor
                .Fetch(QueryNext, reader =>
                {
                    var result = false;
                    while (reader.Read())
                    {
                        var val = reader.GetValue(DataModels.DataType.Integer,"NEXT");
                        if (val!=null)
                        {
                            NextId = Convert.ToInt32(val);
                        }
                        result = true;
                    }
                    return result;
                });
            if (isNotEmpty == false)
            {
                var QueryFromTable = _dataLayer.Builder
               .From(table)
               .Where(new ConditionExpression() { LeftOperand = new ColumnOperand() { Type = OperandType.Column, ColumnName = "ID" }, Operator = ConditionOperator.GreaterThan, RightOperand = new IntegerValueOperand() { Type = OperandType.Value, DataType = DataModels.DataType.Integer, Value = 0 } })
               .Select("ID");
                var isNotEmptyInTable = this._queryExecutor
               .Fetch(QueryFromTable, reader =>
               {
                   var result = false;
                   while (reader.Read())
                   {
                       NextId = reader.GetValueAsInt32(typeof(int), reader.GetOrdinal("ID"));
                       result = true;
                   }
                   return result;
               });

                if (NextId == null)
                {
                    NextId = 1;
                }
                else
                {
                    ++NextId;
                }
                var insertQuery = _dataLayer.Builder
               .Insert("NEXT_ID")
               .SetValue("NEXT", new IntegerValueOperand() { DataType = DataModels.DataType.Integer, Value = NextId })
               .SetValue("TABLE_NAME", new StringValueOperand() { DataType = DataModels.DataType.String, Value = table });
                this._queryExecutor.Execute(insertQuery);
            }
            else
            {
                var updateQuery = _dataLayer.Builder
                .Update("NEXT_ID")
                .SetValue("NEXT", new IntegerValueOperand() { DataType = DataModels.DataType.Integer, Value = ++NextId })
                .Where(new ConditionExpression() { LeftOperand = new ColumnOperand() { Type = OperandType.Column, ColumnName = "TABLE_NAME" }, Operator = ConditionOperator.Equal, RightOperand = new StringValueOperand() { Type = OperandType.Value, DataType = DataModels.DataType.String, Value = table } });
                this._queryExecutor.Execute(updateQuery);
            }
            return NextId.Value;
        }

    }
}
