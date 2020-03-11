using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface ILinkHeadRefSpectrum_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface ILinkHeadRefSpectrum : ILinkHeadRefSpectrum_PK
    {
        IHeadRefSpectrum HEAD_REF_SPECTRUM { get; set; }
        ISynchroProcess SYNCHRO_PROCESS { get; set; }
    }

}
