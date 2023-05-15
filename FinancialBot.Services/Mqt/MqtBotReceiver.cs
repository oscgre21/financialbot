using FinancialBot.BL.DTOs;
using FinancialBot.Core;
using FinancialBot.Core.Repository;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FinancialBot.Services.Mqt
{
    public class MqtBotReceiver:  BackgroundService
    {
        private readonly string _hostname;
        private readonly string _username;
        private readonly string _password;
        private readonly string _listenToQueueName;
        private readonly IMqtBot _bot;
        private readonly IMqtMessageSender _sender;
        private IConnection _connection;
        private IModel _channel;

        public MqtBotReceiver(IOptions<RMqtConfiguration> rabbitMqOptions, IMqtBot bot, IMqtMessageSender sender)
        {
            _hostname = rabbitMqOptions.Value.HostName;
            _password = rabbitMqOptions.Value.Password;
            _username = rabbitMqOptions.Value.UserName;
            _listenToQueueName = rabbitMqOptions.Value.ListenToQueueName;
            _bot = bot;
            _sender = sender;

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

            try
            {
                _connection = factory.CreateConnection();
                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
                _channel = _connection.CreateModel();
                _channel.QueueDeclare(queue: _listenToQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            }catch(Exception e)
            {
                _connection = null;
            }
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e) { }

        private void HandleMessage(UsersMessages message)
        {
            try
            {
                StockModel quote = _bot.GetStockQuote(message);

                if (quote != null)
                {
                    var msg = GetStockMessage(quote);
                    msg.group = message.group;
                    _sender.SendMessage(msg);
                }
            }
            catch (Exception)
            {
                _sender.SendMessage(new UsersMessages
                {
                    username = "#BOT",
                    sendedDateUtc = DateTime.Now,
                    Message = $"Could not get stock quote.",
                    group = message.group
                });
            }
        }

        private UsersMessages GetStockMessage(StockModel quote)
        {
            return new UsersMessages
            {
                username = "#BOT",
                sendedDateUtc = DateTime.Now,
                Message = $"{quote.Symbol} quote is ${quote.Close} per share"
            };
        }

        private void OnConsumerCancelled(object sender, ConsumerEventArgs e) { }

        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e) { }

        private void OnConsumerRegistered(object sender, ConsumerEventArgs e) { }

        private void OnConsumerShutdown(object sender, ShutdownEventArgs e) { }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_channel == null) { return Task.CompletedTask; }

            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                var clientMessage = JsonConvert.DeserializeObject<UsersMessages>(content);

                HandleMessage(clientMessage);

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
