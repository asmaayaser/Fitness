using Moq;
using Confluent.Kafka;
using ClientGateway.Controllers;
using ClientGateway.Domain;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClientGatewayTests.Controllers
{
    public class ClientGatewayControllerTests
    {
        private ILogger<ClientGatewayController> _logger;
        private Mock<IProducer<string, Biometrics>> _mockProducer;
        private ClientGatewayController _controller;

        [SetUp]
        public void Setup()
        {
            // Create mock objects
            _logger = new Mock<ILogger<ClientGatewayController>>().Object;
            _mockProducer = new Mock<IProducer<string, Biometrics>>();

            // Create an instance of ClientGatewayController with mock dependencies
            _controller = new ClientGatewayController(new ProducerConfig(), _mockProducer.Object, _logger);
        }

        [Test]
        public async Task RecordMeasurements_ShouldProduceTheExpectedMessage()
        {
            var expectedTopic = "BiometricsImported";
            var expectedMetrics = new Biometrics(Guid.NewGuid(), new List<HeartRate>(), new List<StepCount>(), 0);

            _mockProducer.Setup(producer => producer.ProduceAsync(It.IsAny<string>(), It.IsAny<Message<string, Biometrics>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new DeliveryResult<string, Biometrics>
                {
                    Message = new Message<string, Biometrics> { Value = expectedMetrics }
                }));

            var result = await _controller.RecordMeasurements(expectedMetrics);

            _mockProducer.Verify(producer => producer.ProduceAsync(
                expectedTopic,
                It.Is<Message<string, Biometrics>>(msg => msg.Value == expectedMetrics),
                It.IsAny<CancellationToken>()));

            _mockProducer.Verify(producer => producer.Flush(It.IsAny<CancellationToken>()), Times.Once());

            Assert.That(result.Value, Is.EqualTo(expectedMetrics));
        }

        [Test]
        public void RecordMeasurements_ShouldReturnAFailure_IfTheMessageProducerFails()
        {
            var expectedTopic = "BiometricsImported";
            var expectedMetrics = new Biometrics(Guid.NewGuid(), new List<HeartRate>(), new List<StepCount>(), 0);

            _mockProducer.Setup(producer => producer.ProduceAsync(It.IsAny<string>(), It.IsAny<Message<string, Biometrics>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromException<DeliveryResult<string, Biometrics>>(new Exception("Boom")));

            Assert.ThrowsAsync<Exception>(() => _controller.RecordMeasurements(expectedMetrics));

            _mockProducer.Verify(producer => producer.ProduceAsync(
                expectedTopic,
                It.Is<Message<string, Biometrics>>(msg => msg.Value == expectedMetrics),
                It.IsAny<CancellationToken>()));
        }
    }
}
