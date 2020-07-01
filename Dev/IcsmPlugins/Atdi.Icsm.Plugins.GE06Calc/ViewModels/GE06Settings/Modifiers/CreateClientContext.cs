using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Icsm.Plugins.GE06Calc.ViewModels.GE06Settings.Modifiers
{
    public class CreateClientContext
    {
        public long BaseContextId;

        public long ProjectId;

        public Guid OwnerId;

        public string Name;

        public string Note;

        public bool ActiveContext;

        public bool Success;
    }
}
