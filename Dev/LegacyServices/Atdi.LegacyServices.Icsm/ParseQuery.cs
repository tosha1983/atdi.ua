using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.Logging;
using Atdi.DataModels.Identity;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.LegacyServices.Icsm;
using System.Security.Cryptography;
using Atdi.DataModels.DataConstraint;
using Atdi.AppServices.WebQuery;
using OrmCs;
using FormsCs;
using Atdi.DataModels.WebQuery;


namespace Atdi.LegacyServices.Icsm
{

    internal sealed class ParseQuery: IParseQuery
    {
        public IcsmReport _report;
        public ParseQuery(IcsmReport rep) 
        {
            _report = rep;
        }

        public ColumnMetadata[] ExecuteParseQuery(QueryToken token)
        {
           List<ColumnMetadata> L = new List<ColumnMetadata>();
          
           Frame f = new Frame();
           string Query = UTF8Encoding.UTF8.GetString(token.Stamp);
           int x1 = Query.IndexOf("\r\n");
           Query = Query.Remove(0, x1 + 2);
           int x2 = Query.IndexOf("\r\n");
           Query = Query.Remove(0, x2 + 2);
           InChannelString strx = new InChannelString(Query);
           f.Load(strx);
            _report.SetConfig(f);


           return L.ToArray();

        }



    }
}
