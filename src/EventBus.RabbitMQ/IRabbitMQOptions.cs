namespace EventBus.RabbitMQ
{
	public interface IRabbitMQOptions
	{
		string Exchange { get; }

		string HostName { get; }

		int Port { get; }

		string UserName { get; }

		string Password { get; }
	}
}