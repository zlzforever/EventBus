using Microsoft.Extensions.Configuration;

namespace EventBus.RabbitMQ.DependencyInjection
{
	public class RabbitMQOptions : IRabbitMQOptions
	{
		private readonly IConfiguration _configuration;

		public RabbitMQOptions(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public string Exchange => _configuration["EventBus:RabbitMq:Exchange"];

		public string HostName => _configuration["EventBus:RabbitMq:HostName"];

		public int Port => string.IsNullOrWhiteSpace(_configuration["EventBus:RabbitMq:Port"])
			? 0
			: int.Parse(_configuration["EventBus:RabbitMq:Port"]);

		public string UserName => _configuration["EventBus:RabbitMq:UserName"];

		public string Password => _configuration["EventBus:RabbitMq:Password"];
	}
}