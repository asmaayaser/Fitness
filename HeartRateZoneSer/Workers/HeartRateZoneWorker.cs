using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using HeartRateZoneSer.Domain;
using System.Threading.Tasks;

namespace HeartRateZoneSer.Workers
{
    public class HeartRateZoneWorker : BackgroundService
    {
        private readonly ILogger<HeartRateZoneWorker> _logger;
        private const string BiometricsImportedTopicName = "BiometricsImported";
        private readonly IConsumer<string, Biometrics> _consumer;

        private readonly static List<Biometrics> _allMessages = new List<Biometrics>();

        public HeartRateZoneWorker(IConsumer<string, Biometrics> consumer, ILogger<HeartRateZoneWorker> logger)
        {
            _consumer = consumer ?? throw new ArgumentNullException(nameof(consumer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _logger.LogInformation("HeartRateZoneWorker is Active.");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
            _consumer.Subscribe(BiometricsImportedTopicName);

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var result = _consumer.Consume(stoppingToken);
                    await HandleMessage(result.Message.Value, stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                // Ignore if cancellation is requested
            }
            finally
            {
                _consumer.Close();
            }
        }

        protected virtual async Task HandleMessage(Biometrics biometrics, CancellationToken cancellationToken)
        {
            _allMessages.Add(biometrics);

            _logger.LogInformation(_allMessages.Count.ToString());
            _logger.LogInformation("Message Received: " + biometrics.DeviceId);

            await Task.CompletedTask;
        }

        public int GetConsumedDataCount()
        {
            _logger.LogInformation(_allMessages.Count.ToString());

            return _allMessages.Count;

        }

        public Biometrics GetLatestMessage()
        {
            return _allMessages.LastOrDefault();
        }

        public Biometrics[] GetAllMessages()
        {
            return _allMessages.ToArray();
        }
    }
}
