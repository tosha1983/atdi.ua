using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.DependencyInjection;
using Atdi.Platform.Logging;

namespace Atdi.Platform.Cqrs
{
	public class CommandDispatcher : ICommandDispatcher
	{
		private readonly ILogger _logger;
		private readonly IServicesResolver _resolver;
		private readonly IServicesContainer _container;

		public CommandDispatcher(IServicesResolver resolver, IServicesContainer container, ILogger logger)
		{
			_resolver = resolver;
			_container = container;
			_logger = logger;
		}

		public void RegisterFrom(Assembly assembly)
		{
			//var commands = Types.FromAssemblyNamed("DreamTeam.DealerPortal.DAL")
			//	.BasedOn(typeof(ICommand<>))
			//	.WithService
			//	.AllInterfaces()
			//	.LifestyleTransient();

			_container.RegisterServicesBasedOn(
					assembly, 
					typeof(ICommandHandler<>), 
					BaseOnMode.AllInterface,
					ServiceLifetime.PerThread
				);
		}

		public void Send<TCommand>(TCommand command)
		{
			try
			{
				var handler = _resolver.Resolve<ICommandHandler<TCommand>>();
				handler.Handle(command);
			}
			catch (Exception e)
			{
				_logger.Exception((EventContext)"Platform", (EventCategory)"Dispatching", e, this);
				throw;
			}
			
		}
	}
}
