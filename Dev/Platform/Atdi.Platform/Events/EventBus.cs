using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.Logging;

namespace Atdi.Platform.Events
{
	public sealed class EventBus : IEventBus
	{
		private class EventHandlerToken
		{

		}

		private class  EventHandlerToken<TEvent> : EventHandlerToken, IEventHandlerToken<TEvent>
		{
			private readonly EventBus _eventBus;
			private readonly WeakReference<Action<TEvent>> _refHandler;

			public EventHandlerToken(EventBus eventBus, Action<TEvent> handler)
			{
				_eventBus = eventBus;
				_refHandler = new WeakReference<Action<TEvent>>(handler);
			}

			public void Send(TEvent data)
			{
				if (_refHandler.TryGetTarget(out var handler))
				{
					handler.Invoke(data);
				}
				else
				{
					_eventBus.Unsubscribe(this);
				}
			}

			public void Dispose()
			{
				_eventBus.Unsubscribe(this);
			}
		}
		

		private readonly ILogger _logger;
		private readonly ConcurrentDictionary<Type, ConcurrentDictionary<EventHandlerToken, EventHandlerToken>> _tokens;

		public EventBus(ILogger logger)
		{
			_logger = logger;
			_tokens = new ConcurrentDictionary<Type, ConcurrentDictionary<EventHandlerToken, EventHandlerToken>>();
		}

		public void Dispose()
		{
			_tokens.Clear();
		}

		public void Send<TEvent>(TEvent data)
		{
			var type = typeof(TEvent);
			if (_tokens.TryGetValue(type, out var tokensByEvent))
			{
				var tokensArray = tokensByEvent.Values.ToArray();
				foreach (var token in tokensArray)
				{
					try
					{
						if (token is EventHandlerToken<TEvent> typedToken)
						{
							typedToken.Send(data);
						}
					}
					catch (Exception e)
					{
						_logger.Exception((EventContext)"Platform", (EventCategory)"Sending", e, this);
					}
				}
			}
		}

		public IEventHandlerToken<TEvent> Subscribe<TEvent>(Action<TEvent> eventHandler)
		{
			var type = typeof(TEvent);
			if (!_tokens.TryGetValue(type, out var tokensByEvent))
			{
				tokensByEvent= new ConcurrentDictionary<EventHandlerToken, EventHandlerToken>();
				if (!_tokens.TryAdd(type, tokensByEvent))
				{
					if (!_tokens.TryGetValue(type, out tokensByEvent))
					{
						throw new InvalidOperationException("Could not add the subscriber object to the root ConcurrentDictionary");
					}
				}
			}

			var token = new EventHandlerToken<TEvent>(this, eventHandler);
			if (!tokensByEvent.TryAdd(token, token))
			{
				throw new InvalidOperationException("Could not add object to the internal ConcurrentDictionary");
			}

			return token;
		}

		private void Unsubscribe<TEvent>(EventHandlerToken<TEvent> token)
		{
			var type = typeof(TEvent);
			if (_tokens.TryGetValue(type, out var tokensByEvent))
			{
				tokensByEvent.TryRemove(token, out var dummy);
			}
		}
	}
}
