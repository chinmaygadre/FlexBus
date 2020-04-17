using FlexBus.Core;
using NServiceBus;
using NServiceBus.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlexBus.Host
{
	class Program
    {
		private static readonly List<IEndpointInstance> _instances = new List<IEndpointInstance>();
		private static readonly List<string> _serviceNames = new List<string>();
        private static readonly ILog log = LogManager.GetLogger<Program>();

		static async Task Main()
		{
			var defaultLogger = LogManager.Use<DefaultFactory>();
			defaultLogger.Level(LogLevel.Info);

			Console.Title = "EndpointHub";

			log.Info("Finding service endpoints to host...");
			var services = Resolver.FindByAttribute<FlexEndpointAttribute>();
			log.Info($"Found {services.Count} endpoints...");

			foreach (var svc in services)
			{
				if (_serviceNames.Contains(svc.EndpointName))
					continue; 

				var endpointConfig = new EndpointConfiguration(svc.EndpointName);

				// NOTE: For purposes of first implementation, we shall use LearningTransport and LearningPersistence
				var transport = endpointConfig.UseTransport<LearningTransport>();
				// var persistence = endpointConfig.UsePersistence<LearningPersistence>();
				// var serialization = endpointConfig.UseSerialization<NewtonsoftSerializer>();
				
				// var routing = transport.Routing();

				var endpointInstance = await Endpoint.Start(endpointConfig)
											.ConfigureAwait(false);

				log.Info($"\t# endpoint '{svc.EndpointName}' started.");
				_instances.Add(endpointInstance);
				_serviceNames.Add(svc.EndpointName);
			}

			log.Info("Waiting for communications...");
			Console.ReadLine();

			foreach (var endpointInstance in _instances)
			{
				await endpointInstance.Stop()
						.ConfigureAwait(false);
			}
		}
	}
}
