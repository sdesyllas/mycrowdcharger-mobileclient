using Newtonsoft.Json;

namespace MyCrowdCharger.Mobile.Api.Dtos
{
    public class Device : JsonSerializable
    {
        [JsonProperty("battery_level")]
        public int BatteryLevel { get; set; }
        [JsonProperty("contributions")]
        public int Contributions { get; set; }
        [JsonProperty("loc")]
        public double[] Location { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("nickname")]
        public string Nickname { get; set; }

    }
}