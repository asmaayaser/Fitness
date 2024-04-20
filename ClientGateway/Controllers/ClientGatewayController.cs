using ClientGateway.Domain;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;
namespace ClientGateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClientGatewayController : ControllerBase
    {
        private string BiometricsImportedTopicName = "BiometricsImported";
        private IProducer<string, Biometrics> _producer;
        private readonly ILogger<ClientGatewayController> _logger;
        //private readonly KafkaConsumerService _consumerService; // Inject KafkaConsumerService


        public ClientGatewayController(ProducerConfig producerConfig, /*KafkaConsumerService consumerService,*/ IProducer<string, Biometrics> producer, ILogger<ClientGatewayController> logger)
        {
            //_consumerService = consumerService;
            _producer = producer;
            _logger = logger;
            logger.LogInformation("ClientGatewayController is Active.");
        }

        [HttpPost("Biometrics")]
        [ProducesResponseType(typeof(Biometrics), (int)HttpStatusCode.Accepted)]
        public async Task<ActionResult<Biometrics>> RecordMeasurements([FromBody] Biometrics metrics)
        {
            _logger.LogInformation("Accepted biometrics");

            // Extract DeviceId and convert it to a string for the message key
            string key = metrics.DeviceId.ToString();

            var message = new Message<string, Biometrics> { Key = key, Value = metrics };
            var result = await _producer.ProduceAsync(BiometricsImportedTopicName, message);
            _producer.Flush();

            return Accepted("", metrics);
        }

        [HttpGet("Hello")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        public string Hello()
        {
            _logger.LogInformation("Hello World");
            return "Hello World";
        }


    }
}
