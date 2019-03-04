using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface ILogs
    {
        int Id { get; set; }
        string Event { get; set; }
        string TableName { get; set; }
        int? Lcount { get; set; }
        string Info { get; set; }
        string Who { get; set; }
        DateTime When { get; set; }
    }
}