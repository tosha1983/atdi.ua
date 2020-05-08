using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Icsm.Plugins.SdrnCalcServerClient.Models
{
    public class ClientContextViewModel
    {
        long Id { get; set; }
        string Name { get; set; }
        string Note { get; set; }
        string OwnerInstance { get; set; }
        Guid OwnerContextId { get; set; }
        DateTimeOffset CreatedDate { get; set; }
        byte TypeCode { get; set; }
        string TypeName { get; set; }
        byte StatusCode { get; set; }
        string StatusName { get; set; }
        string StatusNote { get; set; }
    }
}
