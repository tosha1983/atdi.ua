using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Identity;
using Atdi.DataModels.WebQuery;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.LegacyServices.Icsm;
using Atdi.Platform.Logging;

namespace Atdi.AppServices.WebQuery.Handlers
{
    public sealed class ExecuteQuery : LoggedObject
    {
        private readonly IDataLayer<IcsmDataOrm> _dataLayer;

        object[][] rows;
        string[][] rows2;

        public ExecuteQuery(ILogger logger, IDataLayer<IcsmDataOrm> dataLayer) : base(logger)
        {
            this._dataLayer = dataLayer;

            var r = new List<object[]>();
            var r2 = new List<string[]>();
            for (int i = 0; i < 10000; i++)
            {
                var row = new object[51];
                row[0] = "some string";
                row[1] = Convert.ToDecimal(1.1);
                row[2] = Convert.ToDouble(2.2);
                row[3] = (float)Convert.ToDouble(3.3);
                row[4] = DateTime.Now;
                row[5] = Convert.ToByte(1);
                row[6] = Convert.ToInt32(1);
                row[7] = true;
                row[8] = new byte[] { 1, 2, 3, 4, 5 };

                row[10] = "some string";
                row[11] = Convert.ToDecimal(1.1);
                row[12] = Convert.ToDouble(2.2);
                row[13] = (float)Convert.ToDouble(3.3);
                row[14] = DateTime.Now;
                row[15] = Convert.ToByte(1);
                row[16] = Convert.ToInt32(1);
                row[17] = true;
                row[18] = new byte[] { 1, 2, 3, 4, 5 };
                row[20] = "some string";
                row[21] = Convert.ToDecimal(1.1);
                row[22] = Convert.ToDouble(2.2);
                row[23] = (float)Convert.ToDouble(3.3);
                row[24] = DateTime.Now;
                row[25] = Convert.ToByte(1);
                row[26] = Convert.ToInt32(1);
                row[27] = true;
                row[28] = new byte[] { 1, 2, 3, 4, 5 };
                row[30] = "some string";
                row[31] = Convert.ToDecimal(1.1);
                row[32] = Convert.ToDouble(2.2);
                row[33] = (float)Convert.ToDouble(3.3);
                row[34] = DateTime.Now;
                row[35] = Convert.ToByte(1);
                row[36] = Convert.ToInt32(1);
                row[37] = true;
                row[38] = new byte[] { 1, 2, 3, 4, 5 };
                row[40] = "some string";
                row[41] = Convert.ToDecimal(1.1);
                row[42] = Convert.ToDouble(2.2);
                row[43] = (float)Convert.ToDouble(3.3);
                row[44] = DateTime.Now;
                row[45] = Convert.ToByte(1);
                row[46] = Convert.ToInt32(1);
                row[47] = true;
                row[48] = new byte[] { 1, 2, 3, 4, 5 };
                r.Add(row);
                var row2 = row.Select(s => Convert.ToString(s)).ToArray();
                r2.Add(row2);
            }

            rows = r.ToArray();


            rows2 = r2.ToArray(); 
        }

        public QueryResult Handle(UserToken userToken, QueryToken queryToken, FetchOptions fetchOptions)
        {
            if (userToken == null)
            {
                var result = new QueryResult() { Rows = rows };
                return result;
            }
            using (this.Logger.StartTrace(Contexts.WebQueryAppServices, Categories.Handling, TraceScopeNames.ExecuteQuery))
            {
                var result = new QueryResult() { RowsAsString = rows2 };
                return result;
            }
        }

        private void TestDataLayer()
        {
            var userQuery = _dataLayer.Builder
                .From("USERS").Where("ID", 124).Select("ID", "NAME").OrderByAsc("Name", "ID").OnTop(100);

            var result = _dataLayer.Executor<IcsmDataContext>().Fetch(userQuery, reader =>
            {
                var data = new List<object[]>();
                while(reader.Read())
                {
                    var row = new object[reader.FieldCount];
                    reader.GetValues(row);
                    data.Add(row);
                }
                return data;
            });

        }
    }
}
