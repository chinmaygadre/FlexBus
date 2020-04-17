using System.Threading.Tasks;
using Flex.Plugins.Messages;
using FlexBus.Core;
using NServiceBus;
using NServiceBus.Logging;

namespace Flex.Plugins.Sales
{
    [FlexEndpoint(nameof(EndpointNames.Sales), FlexEndpointTypes.ReceiverOnly)]
    public class SalesHandlers
        : IHandleMessages<PlaceSalesOrder>
    {
        private static readonly ILog log = LogManager.GetLogger<SalesHandlers>();

        Task IHandleMessages<PlaceSalesOrder>.Handle(PlaceSalesOrder message, IMessageHandlerContext context)
        {
            log.Info($"Message 'PlaceSalesOrder' handled... (Order ID: {message.OrderId}; " +
                $"Received From: {context.MessageHeaders["NServiceBus.OriginatingMachine"]}\\" +
                $"{context.MessageHeaders["NServiceBus.OriginatingEndpoint"]}); ");
            return Task.CompletedTask;
        }
    }
}
