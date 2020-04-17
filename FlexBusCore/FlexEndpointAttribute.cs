using System;

namespace FlexBus.Core
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class FlexEndpointAttribute
		: Attribute, IFlexEndpoint
	{
		public FlexEndpointAttribute(string endpointName, FlexEndpointTypes endpointType)
		{
			EndpointName = endpointName ?? throw new ArgumentNullException(nameof(endpointName));
			EndpointType = endpointType;
		}

		public string EndpointName { get; private set; }

		public FlexEndpointTypes EndpointType { get; private set; }
	}
}
