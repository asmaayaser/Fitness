using Newtonsoft.Json;

namespace ClientGateway.Domain
{
    public class Biometrics
    {
        [JsonProperty("DeviceId")]
        public Guid DeviceId { get; }

        [JsonProperty("HeartRates")]
        public List<HeartRate> HeartRates { get; }

        [JsonProperty("StepCounts")]
        public List<StepCount> StepCounts { get; }

        [JsonProperty("MaxHeartRate")]
        public int MaxHeartRate { get; }

        // Constructor with parameter names matching JSON properties
        public Biometrics(Guid deviceId, List<HeartRate> heartRates, List<StepCount> stepCounts, int maxHeartRate)
        {
            DeviceId = deviceId;
            HeartRates = heartRates;
            StepCounts = stepCounts;
            MaxHeartRate = maxHeartRate;
        }
    }
    public class HeartRate
    {
        public int Value { get; }
        public DateTime DateTime { get; }
        public HeartRate(int value, DateTime dateTime)
        {
            Value = value;
            DateTime = dateTime;
        }
    }
    public class StepCount
    {
        public int Value { get; }
        public DateTime DateTime { get; }
        public StepCount(int value, DateTime dateTime)
        {
            Value = value;
            DateTime = dateTime;
        }

    }

}
