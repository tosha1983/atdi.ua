using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IProperty : IPropertyBase
    {
        string TableName { get; set; }
        int TableRecId { get; }
        string PropName { get; set; }
        string Value { get; set; }
    }
}
