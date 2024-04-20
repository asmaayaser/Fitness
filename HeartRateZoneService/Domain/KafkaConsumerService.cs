using ClientGateway.Domain;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ClientGateway.Services
{
    public class KafkaConsumerService
    {
        private readonly ConsumerConfig _config;
        private readonly ILogger<KafkaConsumerService> _logger;

        public KafkaConsumerService(ConsumerConfig config, ILogger<KafkaConsumerService> logger)
        {
            _config = config;
            _logger = logger;
        }


    }
}
