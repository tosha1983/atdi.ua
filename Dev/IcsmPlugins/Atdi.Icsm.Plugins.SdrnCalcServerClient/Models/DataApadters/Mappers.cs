using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DM = Atdi.DataModels.Sdrn.CalcServer.Entities;

namespace Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels.ProjectManager
{
    public static class Mappers
    {
        public static ProjectModel Map(/* DM.IProject - пока не известно кто бутет сорсе */ object source)
        {
            if (source == null)
                return null;

            //return new VM.ProjectViewModel
            //{
            //    Id = source.Id
            //    .......
            //};
            return null;
        }
    }
}
