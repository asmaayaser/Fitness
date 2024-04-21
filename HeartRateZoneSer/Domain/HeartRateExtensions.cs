using HeartRateZoneSer.Domain;

namespace HeartRateZoneSer.Domain
{
    public enum HeartRateZone
    {
        None,
        Zone1,
        Zone2,
        Zone3,
        Zone4,
        Zone5
    }

    public static class HeartRateExtensions
    {
        public static HeartRateZone GetHeartRateZone(this HeartRate hr, int maxHeartRate)
        {
            double percentage = (double)hr.Value / maxHeartRate * 100;

            if (percentage < 50)
                return HeartRateZone.None;
            else if (percentage >= 50 && percentage < 60)
                return HeartRateZone.Zone1;
            else if (percentage >= 60 && percentage < 70)
                return HeartRateZone.Zone2;
            else if (percentage >= 70 && percentage < 80)
                return HeartRateZone.Zone3;
            else if (percentage >= 80 && percentage < 90)
                return HeartRateZone.Zone4;
            else
                return HeartRateZone.Zone5;
        }
    }
}
