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

        // Dictionary to track device IDs and their quantities
        private readonly static Dictionary<Guid, int> _deviceQuantities = new Dictionary<Guid, int>();

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
            // Check if the device ID already exists in the dictionary
            if (_deviceQuantities.ContainsKey(biometrics.DeviceId))
            {
                // If exists, increment the quantity by 1
                _deviceQuantities[biometrics.DeviceId]++;
            }
            else
            {
                // If not exists, add the device ID with quantity 1
                _deviceQuantities.Add(biometrics.DeviceId, 1);
            }
            // Log the contents of _deviceQuantities for debugging or monitoring purposes
            foreach (var pair in _deviceQuantities)
            {
                _logger.LogInformation($"Device ID: {pair.Key}, Quantity: {pair.Value}");
            }
            _logger.LogInformation("Message Received: " + biometrics.DeviceId);
            await Task.CompletedTask;
        }

        public Dictionary<Guid, int> GetConsumedDataCount()
        {
            // Log the contents of _deviceQuantities for debugging or monitoring purposes
            foreach (var pair in _deviceQuantities)
            {
                _logger.LogInformation($"Device ID: {pair.Key}, Quantity: {pair.Value}");
            }
            
            // Return the dictionary containing device IDs and their quantities
            return _deviceQuantities;
        }

        public Biometrics GetLatestMessage()
        {
            return _allMessages.LastOrDefault();
        }
        public Biometrics[] GetAllMessages()
        {
            return _allMessages.ToArray();
        }
        public Biometrics GetLatestdeviceIDquantity()
        {
            // Get the latest device ID and quantity from the dictionary
            var latestDeviceId = _deviceQuantities.Keys.LastOrDefault();
            if (latestDeviceId != Guid.Empty)
            {
                // Assuming there is a constructor for Biometrics that takes deviceId, heartRates, and maxHeartRate as parameters
                return new Biometrics(latestDeviceId, null, _deviceQuantities[latestDeviceId]);
            }
            else
            {
                return null;
            }
        }

        public Biometrics[] GetalldeviceIDsquantities()
        {
            // Return all device IDs and their quantities from the dictionary
            return _deviceQuantities.Select(pair => new Biometrics(pair.Key, null, pair.Value)).ToArray(); // Assuming constructor exists for Biometrics
        }


    }
}
