namespace EventBus.RabbitMQ
{
	public class RabbitMQOptions : IRabbitMQOptions
	{
		public string Exchange { get; set; }

		public string HostName { get; set; }

		public int Port { get; set; }

		public string UserName { get; set; }

		public string Password { get; set; }
	}
}