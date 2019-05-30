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
        int Id { get; set; }
        double? level { get; set; }
        int? SpectrumId { get; set; }
        ISpectrum SPECTRUM { get; set; }
    }
}

