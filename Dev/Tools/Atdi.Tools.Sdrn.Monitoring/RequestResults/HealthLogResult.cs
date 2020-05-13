using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Tools.Sdrn.Monitoring
{
    public class HealthLogResult
	{
        public long Id { get; set; }

        public long? SenderLogId { get; set; }

        public byte? SenderTypeCode { get; set; }

        public string SenderTypeName { get; set; }

        public string SenderInstance { get; set; }

        public string SenderHost { get; set; }


        public byte SourceTypeCode { get; set; }

        public string SourceTypeName { get; set; }

        public string SourceInstance { get; set; }

        public string SourceTechId { get; set; }

        public string SourceHost { get; set; }

        public byte EventCode { get; set; }

        public string EventName { get; set; }

        public string EventNote { get; set; }

        public DateTimeOffset DispatchTime { get; set; }

        public DateTimeOffset ReceivedTime { get; set; }

		public DateTimeOffset? ForwardedTime { get; set; }
	}

    public class HealthLogDetailResult
    {
	    public long Id { get; set; }

	    public string Message { get; set; }

	    public string Note { get; set; }

	    public DateTimeOffset CreatedDate { get; set; }

	    public int ThreadId { get; set; }

	    public string Source { get; set; }

	    public byte SiteTypeCode { get; set; }

	    public string SiteTypeName { get; set; }

	    public string SiteInstance { get; set; }

	    public string SiteHost { get; set; }
	}
}
