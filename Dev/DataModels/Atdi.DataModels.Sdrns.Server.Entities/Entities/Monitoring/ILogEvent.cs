using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities.Monitoring
{
    [EntityPrimaryKey]
    public interface ILogEvent_PK
    {
        Guid Id { get; set; }
    }

    [Entity]
    public interface ILogEvent : ILogEvent_PK
    {
        int LevelCode { get; set; }

        string LevelName { get; set; }

        string Text { get; set; }

        DateTime Time { get; set; }

        int Thread { get; set; }

        string Context { get; set; }

        string Category { get; set; }

        string Source { get; set; }

        TimeSpan? Duration { get; set; }

        IReadOnlyDictionary<string, string> Data { get; set; }

        object Exception { get; set; }
    }
}
