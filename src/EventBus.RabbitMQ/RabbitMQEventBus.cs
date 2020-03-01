using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using EventBus.Default;
using MessagePack;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EventBus.RabbitMQ
{
	public class RabbitMQEventBus : EventBus
	{
		private readonly IConnection _connection;
		private readonly ConcurrentDictionary<string, IModel> _modelDict;
		private readonly IRabbitMQOptions _options;

		public int PublishCounter;
		public int ConsumerCounter;

		public RabbitMQEventBus(IRabbitMQOptions options) : this(options, new DefaultEventHandlerTypeStore(),
			new ActivatorHandlerFactory())
		{
		}

		public RabbitMQEventBus(IRabbitMQOptions options, IEventHandlerTypeStore eventHandlerTypeStore,
			IHandlerFactory handlerFactory) : base(eventHandlerTypeStore, handlerFactory)

		{
			_options = options;
			_modelDict = new ConcurrentDictionary<string, IModel>();

			var connectionFactory = new ConnectionFactory
			{
				HostName = _options.HostName,

				DispatchConsumersAsync = true
			};
			if (_options.Port > 0)
			{
				connectionFactory.Port = _options.Port;
			}

			if (!string.IsNullOrWhiteSpace(_options.UserName))
			{
				connectionFactory.UserName = _options.UserName;
			}

			if (!string.IsNullOrWhiteSpace(_options.Password))
			{
				connectionFactory.Password = _options.Password;
			}

			_connection = connectionFactory.CreateConnection();
		}

		public override bool Register<TEvent, TEventHandler>()
		{
			var registered = base.Register<TEvent, TEventHandler>();
			if (registered)
			{
				var topic = GenerateTopic(typeof(TEvent));

				return RegisterRabbitMq(topic);
			}

			return false;
		}

		public override bool Register<TEvent>(Type handlerType)
		{
			var registered = base.Register<TEvent>(handlerType);
			if (registered)
			{
				var topic = GenerateTopic(typeof(TEvent));
				return RegisterRabbitMq(topic);
			}

			return false;
		}

		public override bool Unregister<TEvent>()
		{
			var unregistered = base.Unregister<TEvent>();
			if (unregistered)
			{
				var topic = GenerateTopic(typeof(TEvent));
				return ReleaseRabbitMq(topic);
			}

			return false;
		}

		public override bool Unregister<TEvent, TEventHandler>()
		{
			var unregistered = base.Unregister<TEvent, TEventHandler>();
			if (unregistered)
			{
				var topic = GenerateTopic(typeof(TEvent));
				return ReleaseRabbitMq(topic);
			}

			return false;
		}

		public override Task PublishAsync(object @event)
		{
			if (@event == null)
			{
				throw new ArgumentNullException(nameof(@event));
			}

			var eventType = @event.GetType();
			var topic = GenerateTopic(eventType);
			if (_modelDict.TryGetValue(topic, out var channel))
			{
				var bytes = MessagePackSerializer.Typeless.Serialize(@event);
				channel.BasicPublish(_options.Exchange, topic, null, bytes);
				Interlocked.Increment(ref PublishCounter);
				return Task.CompletedTask;
			}
			else
			{
				throw new ApplicationException("Get channel failed");
			}
		}

		private string GenerateTopic(Type type)
		{
			return type.FullName;
		}

		private bool RegisterRabbitMq(string topic)
		{
			lock (_modelDict)
			{
				if (!_modelDict.ContainsKey(topic))
				{
					var channel = _connection.CreateModel();
					channel.QueueDeclare(topic, durable: true, exclusive: false, autoDelete: false,
						arguments: null);
					channel.ExchangeDeclare(exchange: _options.Exchange, type: "direct", durable: true);

					var queue = channel.QueueDeclare().QueueName;
					channel.QueueBind(queue: queue, _options.Exchange, routingKey: topic);

					var consumer = new AsyncEventingBasicConsumer(channel);

					consumer.Received += async (model, ea) =>
					{
						var obj = MessagePackSerializer.Typeless.Deserialize(ea.Body);
						await base.PublishAsync(obj);
						Interlocked.Increment(ref ConsumerCounter);
						channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
					};

					//7. 启动消费者
					channel.BasicConsume(queue: queue, autoAck: false, consumer: consumer);

					return _modelDict.TryAdd(topic, channel);
				}

				return true;
			}
		}

		private bool ReleaseRabbitMq(string topic)
		{
			if (_modelDict.TryRemove(topic, out var channel))
			{
				channel.Close();
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}