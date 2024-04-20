using Newtonsoft.Json;

namespace HeartRateZoneService.Domain
{
    public class HeartRate
    {
        [JsonProperty("DeviceId")]
        public Guid DeviceId { get; }

        [JsonProperty("HeartRates")]
        public List<HeartRateData> HeartRates { get; }

        [JsonProperty("MaxHeartRate")]
        public int MaxHeartRate { get; }
        public int Value { get; set; } // Assuming the heart rate value is of type int


        // Constructor with parameter names matching JSON properties
        public HeartRate(Guid deviceId, List<HeartRateData> heartRates, int maxHeartRate, int value)
        {
            DeviceId = deviceId;
            HeartRates = heartRates;
            MaxHeartRate = maxHeartRate;
            Value = value;
        }
    }
    public class HeartRateData
    {
        public int Value { get; }
        public DateTime DateTime { get; }
        public HeartRateData(int value, DateTime dateTime)
        {
            Value = value;
            DateTime = dateTime;
        }
    }
}
