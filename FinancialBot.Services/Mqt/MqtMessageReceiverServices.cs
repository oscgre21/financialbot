using FinancialBot.BL.DTOs;
using FinancialBot.Core;
using FinancialBot.Services.Hubs;
using Microsoft.AspNetCore.SignalR; 
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FinancialBot.Services.Mqt
{
    public class MqtMessageReceiverServices : BackgroundService
    {
        private readonly string _hostname;
        private readonly string _listenToQueueName;
        private readonly string _username;
        private readonly string _password;
        private readonly IHubContext<HubChatServices> _hubContext;
        private IConnection _connection;
        private IModel _channel;

        public MqtMessageReceiverServices(IOptions<RMqtConfiguration> rabbitMqOptions, IHubContext<HubChatServices> hubContext)
        {
            _hostname = rabbitMqOptions.Value.HostName;
            _listenToQueueName = rabbitMqOptions.Value.ListenToQueueName;
            _password = rabbitMqOptions.Value.Password;
            _username = rabbitMqOptions.Value.UserName;
            _hubContext = hubContext;

            InitializeRabbitMqListener();
        }

        private void InitializeRabbitMqListener()
        {
            var factory = new ConnectionFactory
            {
                HostName = _hostname,
                UserName = _username,
                Password = _password
            };

            _connection = factory.CreateConnection();
            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: _listenToQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e) { }

        private async Task HandleMessage(UsersMessages message)
        {
            try
            {
                await _hubContext.Clients.All.SendAsync("NewMessage", message);
            }
            catch (Exception e)
            {
                // TO DO
            }
        }

        private void OnConsumerCancelled(object sender, ConsumerEventArgs e) { }

        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e) { }

        private void OnConsumerRegistered(object sender, ConsumerEventArgs e) { }

        private void OnConsumerShutdown(object sender, ShutdownEventArgs e) { }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                var clientMessage = JsonConvert.DeserializeObject<UsersMessages>(content);

                await HandleMessage(clientMessage);

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            consumer.Shutdown += OnConsumerShutdown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerCancelled;

            _channel.BasicConsume(_listenToQueueName, false, consumer);

            return Task.CompletedTask;
        }
    }
}
