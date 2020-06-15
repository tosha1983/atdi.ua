using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Atdi.Icsm.Plugins.Core
{
	public enum LongProcessValueKind
	{
		Absolute = 0,
		Percent = 1,
		Infinity = 2
	}
	public class LongProcessOptions
	{
		public string Title;
		public string Note;
		public bool UseLog;
		public bool UseProgressBar;
		public LongProcessValueKind ValueKind;
		public int MinValue;
		public int MaxValue;
		public bool CanAbort;
		public bool CanStop;
		public bool IsModal;
	}

	public class LongProcessEvent
	{
		public LongProcessToken ProcessToken;
	}

	public class LongProcessLogEvent : LongProcessEvent
	{
		public string Message;
	}

	public class LongProcessUpdateEvent : LongProcessEvent
	{
		public int Value;
		public string Title;
		public string Note;
	}


	public class LongProcessFinishEvent : LongProcessEvent
	{
	}

	public class LongProcessConfirmAbortEvent : LongProcessEvent
	{
	}

	public class LongProcessConfirmStopEvent : LongProcessEvent
	{
	}

	public class LongProcessConfirmResumeEvent : LongProcessEvent
	{
	}

	public class LongProcessToken
	{
		public LongProcessOptions Options;
		public Guid Key;

		public CancellationToken AbortToken;
		public CancellationToken StopToken;
		public CancellationToken ResumeToken;
	}
}
