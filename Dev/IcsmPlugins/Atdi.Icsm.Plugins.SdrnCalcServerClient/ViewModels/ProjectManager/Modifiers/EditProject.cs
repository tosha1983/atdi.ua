using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels.ProjectManager.Modifiers
{
    public class EditProject
    {
        public long Id;

        public Guid OwnerId;

        public string Name;

        public string Note;

        public string Projection;

        public bool Success;
    }
}
