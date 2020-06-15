using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels.ClientContext.Modifiers
{
    public class CreateClientContext
    {
        public long ProjectId;

        public Guid OwnerId;

        public string Name;

        public string Note;

        public byte TypeCode;

        public bool Success;
    }
}
