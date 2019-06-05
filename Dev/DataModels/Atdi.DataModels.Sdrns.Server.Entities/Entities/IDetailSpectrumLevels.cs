using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IDetailSpectrumLevels
    {
        long Id { get; set; }
        double? level { get; set; }
        long? SpectrumId { get; set; }
        ISpectrum SPECTRUM { get; set; }
    }
}

