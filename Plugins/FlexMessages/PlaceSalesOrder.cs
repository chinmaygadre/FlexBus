using NServiceBus;
using System;

namespace Flex.Plugins.Messages
{
	public class PlaceSalesOrder
		: ICommand
	{
		public Guid OrderId { get; private set; }

		public PlaceSalesOrder()
		{
			OrderId = Guid.NewGuid();
		}
	}
}
