using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Atdi.Platform.Events;
using Atdi.Platform.Logging;

namespace Atdi.Icsm.Plugins.Core
{
	internal class LongProcessWorker : IDisposable
	{
		private readonly object _locker = new object();

		private ViewStarter _viewStarter;
		private readonly IEventBus _eventBus;
		private readonly ILogger _logger;
		private IEventHandlerToken<LongProcessUpdateEvent> _updateEvent;
		private IEventHandlerToken<LongProcessFinishEvent> _finishEvent;
		private IEventHandlerToken<LongProcessLogEvent> _logEvent;
		private LongProcessToken _processToken;
		private LongProcessForm _form;

		private CancellationTokenSource _abortSource;
		private CancellationTokenSource _stopSource;
		private CancellationTokenSource _resumeSource;

		private Action<LongProcessToken> _action;
		private Task _task;
		private bool _disposed;

		public LongProcessWorker(ViewStarter viewStarter, IEventBus eventBus, ILogger logger)
		{
			_viewStarter = viewStarter;
			_eventBus = eventBus;
			_logger = logger;
			_disposed = false;

			_updateEvent = _eventBus.Subscribe<LongProcessUpdateEvent>(this.OnUpdateEvent);
			_finishEvent = _eventBus.Subscribe<LongProcessFinishEvent>(this.OnFinishEvent);
			_logEvent = _eventBus.Subscribe<LongProcessLogEvent>(this.OnLogEvent);

		}

		private void OnLogEvent(LongProcessLogEvent data)
		{
			if (data.ProcessToken.Key != _processToken.Key)
			{
				return;
			}

			if (!_processToken.Options.UseLog)
			{
				return;
			}

			lock (_locker)
			{
				if (_disposed)
				{
					return;
				}

				PluginHelper.RunUiContext(() => _form.AddLogMessage(data.Message));
			}
		}

		private void OnFinishEvent(LongProcessFinishEvent data)
		{
			if (data.ProcessToken.Key != _processToken.Key)
			{
				return;
			}
			this.Dispose();
		}

		private void OnUpdateEvent(LongProcessUpdateEvent data)
		{
			if (data.ProcessToken.Key != _processToken.Key)
			{
				return;
			}
			lock (_locker)
			{
				if (_disposed)
				{
					return;
				}

				PluginHelper.RunUiContext(() => _form.UpdateState(data.Value, data.Title, data.Note));
			}
		}

		public void Dispose()
		{
			lock (_locker)
			{
				_disposed = true;

				if (_viewStarter != null)
				{
					_viewStarter.Close(this);
					_viewStarter = null;
				}

				if (_form != null)
				{
					_form.Close();
					_form.Dispose();
					_form = null;
				}

				if (_updateEvent != null)
				{
					_updateEvent.Dispose();
					_updateEvent = null;
				}

				if (_finishEvent != null)
				{
					_finishEvent.Dispose();
					_finishEvent = null;
				}

				if (_logEvent != null)
				{
					_logEvent?.Dispose();
					_logEvent = null;
				}

				_task = null;
			}
		}

		public void Start(LongProcessOptions options, Action<LongProcessToken> action)
		{
			var token = new LongProcessToken
			{
				Options = options,
				Key = Guid.NewGuid(),
			};

			if (options.CanAbort)
			{
				_abortSource= new CancellationTokenSource();
				token.AbortToken = _abortSource.Token;
			}
			if (options.CanStop)
			{
				_stopSource = new CancellationTokenSource();
				_resumeSource = new CancellationTokenSource();
				token.StopToken = _stopSource.Token;
				token.ResumeToken = _resumeSource.Token;
			}
			
			_action = action;
			_processToken = token;
			_form = new LongProcessForm(_processToken, _abortSource);
			_task = Task.Run((Action) this.Process);

			if (options.IsModal)
			{
				_form.ShowDialog();
			}
			else
			{
				_form.Show();
			}
		}

		private void Process()
		{
			try
			{
				_action(_processToken);

				_eventBus.Send(new LongProcessFinishEvent()
				{
					ProcessToken = _processToken
				});
			}
			catch (Exception e)
			{
				_logger.Exception((EventContext)"LongProcessWorker", (EventCategory)"Process", e);
				var finishEvent = new LongProcessFinishEvent()
				{
					ProcessToken = _processToken
				};
				_viewStarter.ShowException(_processToken.Options.Title, e);
				_eventBus.Send(finishEvent);
			}
		}
	}
}
