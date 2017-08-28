using Newtonsoft.Json;

namespace MyCrowdCharger.Mobile.Api.Dtos
{
    public class BatterySend : JsonSerializable
    {
        [JsonProperty("sender")]
        public Sender SenderUser { get; set; }
        [JsonProperty("recipient")]
        public Recipient RecipientUser { get; set; }

        public class Participator
        {
            [JsonProperty("name")]
            public string Name { get; set; }
        }

        public class Sender : Participator
        {
            [JsonProperty("battery")]
            public int Battery { get; set; }
        }

        public class Recipient : Participator
        {

        }
    }
}