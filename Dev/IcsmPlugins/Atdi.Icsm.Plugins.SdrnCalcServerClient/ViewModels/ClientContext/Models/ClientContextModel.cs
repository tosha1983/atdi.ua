using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Entities;

namespace Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels.ClientContext
{
    public class ClientContextModel
    {
        public long Id { get; set; }
        public long ProjectId { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public string OwnerInstance { get; set; }
        public Guid OwnerContextId { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public ClientContextTypeCode TypeValue { get; set; }
        public byte TypeCode { get; set; }
        public string TypeName { get; set; }
        public byte StatusCode { get; set; }
        public string StatusName { get; set; }
        public string StatusNote { get; set; }
    }
}
