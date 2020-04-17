namespace FlexBus.Core
{
	public interface IFlexEndpoint
	{
		string EndpointName { get; }
		FlexEndpointTypes EndpointType { get; }
	}
}
