using Confluent.Kafka;
using locket.Helpers;

namespace locket.Services
{
    public class KafkaProducerService
    {
        private readonly AppConfig _appConfig;
        private readonly IProducer<Null, string> _producer;

        public KafkaProducerService(AppConfig appConfig)
        {
            _appConfig = appConfig;
            var config = new ProducerConfig
            {
                BootstrapServers = _appConfig.Option.Kafka.BootstrapServers
            };
            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public async Task ProduceMessageAsync(string message)
        {
            try
            {
                var kafkaMessage = new Message<Null, string> { Value = message };
                var result = await _producer.ProduceAsync(_appConfig.Option.Kafka.Topic, kafkaMessage);
            }
            catch (ProduceException<Null, string> ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}