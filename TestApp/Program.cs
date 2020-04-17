using Flex.Plugins.Messages;
using NServiceBus;
using NServiceBus.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FlexBus.TestApp
{
	class Program
	{
		static async Task Main(string[] args)
		{
			string epName = args.Length == 1 ? args[0] : "FlexBusTest";

			var done = false;
			Console.Clear();
			Console.Title = "FlexBus Test App";

			Console.WriteLine("Choose message to send:");
			Console.WriteLine("\t[O] send sales order");
			Console.WriteLine("\n\t[X] exit");

			var endpointConfig = new EndpointConfiguration(epName);
			endpointConfig.UseTransport<LearningTransport>();

			var endpointInstance = await Endpoint.Start(endpointConfig)
										.ConfigureAwait(false);

			while (!done)
			{
				var key = Console.ReadKey(true);

				switch (key.Key)
				{
					case ConsoleKey.O:
						var msg = new PlaceSalesOrder();
						await endpointInstance.Send(nameof(EndpointNames.Sales), msg)
								.ConfigureAwait(false);
						Console.WriteLine($"Message sent (ID: {msg.OrderId})");
						break;

					case ConsoleKey.X:
						done = true;
						break;
				}

			}

			await endpointInstance.Stop()
					.ConfigureAwait(false);
		}
	}
}
