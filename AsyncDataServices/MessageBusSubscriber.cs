using System.Text;
using CommandsService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandsService.AsyncDataServices
{
  public class MessageBusSubscriber : BackgroundService
  {
    public IConfiguration _configuration;
    public IEventProcessor _eventProcessor;
    public IConnection _connection;
    public IChannel _channel;

    public MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor)
    {
      _configuration = configuration;
      _eventProcessor = eventProcessor;

      InitializeRabbitMQ();
    }

    private async void InitializeRabbitMQ()
    {
      var rabbitMQHost = _configuration["RabbitMQHost"];
      var rabbitMQPort = _configuration["RabbitMQPort"];

      ConnectionFactory factory = new ConnectionFactory();
      factory.HostName = rabbitMQHost;
      factory.Port = int.Parse(rabbitMQPort);

      try
      {
        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        await _channel.ExchangeDeclareAsync("trigger", ExchangeType.Fanout);
        await _channel.QueueDeclareAsync("queue", false, false, false);
        await _channel.QueueBindAsync("queue", "trigger", "");

        _connection.ConnectionShutdownAsync += RabbitMQ_ConnectionShutdown;

        Console.WriteLine("S --> Listening on The Message Bus");
        };

        await _channel.BasicConsumeAsync("queue", false, consumer);
      }
      catch (Exception exception)
      {
        Console.WriteLine($"X --> Failed to Connect to Message Bus: {exception}");
      }
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
      stoppingToken.ThrowIfCancellationRequested();

      // var consumer = new AsyncEventingBasicConsumer(_channel);
      //
      // consumer.ReceivedAsync += async (ModuleHandle, ea) =>
      // {
      //   Console.WriteLine("S --> Event Received");
      //
      //   var body = ea.Body.ToArray();
      //   var notificationMessage = Encoding.UTF8.GetString(body);
      //
      //   Console.WriteLine($"Received Message: {notificationMessage}");
      //
      //   await _channel.BasicAckAsync(ea.DeliveryTag, false);
      //
      //   _eventProcessor.ProcessEvent(notificationMessage);
      // };
      //
      // await _channel.BasicConsumeAsync("queue", false, consumer);

      Console.WriteLine("Executed Async");
    }

    private async Task RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs @event)
    {
      Console.WriteLine("X --> RabbitMQ Connection Shutdown");
    }
  }
}
