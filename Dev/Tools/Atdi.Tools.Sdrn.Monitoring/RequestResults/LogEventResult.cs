using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Tools.Sdrn.Monitoring
{
    public class LogEventResult
    {
        public Guid Id { get; set; }
        public DateTime Time { get; set; }
        public int Thread { get; set; }
        public int LevelCode { get; set; }
        public string LevelName { get; set; }
        public string Context { get; set; }
        public string Category { get; set; }
        public string Text { get; set; }
        public string Source { get; set; }
        public TimeSpan? Duration { get; set; }
        public IReadOnlyDictionary<string, string> Data { get; set; }
        public object Exception { get; set; }
    }
}
