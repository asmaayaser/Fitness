using Newtonsoft.Json;

namespace HeartRateZoneSer.Domain
{
    public class Biometrics
    {
        [JsonProperty("DeviceId")]
        public Guid DeviceId { get; }

        [JsonProperty("HeartRates")]
        public List<HeartRatedata> HeartRates { get; }

        [JsonProperty("MaxHeartRate")]
        public int MaxHeartRate { get; }

        // Constructor with parameter names matching JSON properties
        public Biometrics(Guid deviceId, List<HeartRatedata> heartRates, int maxHeartRate)
        {
            DeviceId = deviceId;
            HeartRates = heartRates;
            MaxHeartRate = maxHeartRate;
        }
    }
    public class HeartRatedata
    {
        public int Value { get; }
        public DateTime DateTime { get; }
        public HeartRatedata(int value, DateTime dateTime)
        {
            Value = value;
            DateTime = dateTime;
        }
    }
 

}
