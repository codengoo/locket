using Confluent.Kafka;
using locket.Helpers;

namespace locket.Services
{
    public class KafkaConsumerService : BackgroundService
    {
        private readonly AppConfig _appConfig;
        private readonly IConsumer<Ignore, string> _consumer;
        public KafkaConsumerService(AppConfig appConfig)
        {
            _appConfig = appConfig;
            var config = new ConsumerConfig
            {
                BootstrapServers = _appConfig.Option.Kafka.BootstrapServers,
                GroupId = "0",
                AutoOffsetReset = AutoOffsetReset.Earliest,
            };
            _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.Subscribe(_appConfig.Option.Kafka.Topic);

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var consumeResult = _consumer.Consume(stoppingToken);
                    var message = consumeResult.Message.Value;

                    Console.WriteLine($"Received message: {message}");
                }
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine($"Error processing Kafka message: {ex.Message}");
            }
            finally
            {
                _consumer.Close();
            }
        }
    }
}
