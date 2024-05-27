using MongoDB.Driver.Core.Connections;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;
using static MongoDB.Driver.WriteConcern;
using RMQConnection = RabbitMQ.Client.IConnection;

namespace AuctionService.Services 
{
public class RabbitMQSubscriber
{
    private readonly RMQConnection _connection;
        private readonly IModel _channel;
    private readonly string _queueName;

    public RabbitMQSubscriber(string queueName)
    {
        var factory = new ConnectionFactory() { HostName = "rabbitmq" }; // Change this to your RabbitMQ server
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _queueName = queueName;

        _channel.QueueDeclare(queue: _queueName,
                              durable: false,
                              exclusive: false,
                              autoDelete: false,
                              arguments: null);
    }

    public async Task StartListening(Func<string, Task> processMessage)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            await processMessage(message);
        };
        _channel.BasicConsume(queue: _queueName,
                              autoAck: true,
                              consumer: consumer);
        await Task.Delay(-1);
    }

    public void Close()
    {
        _channel.Close();
        _connection.Close();
    }
}
}