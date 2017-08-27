using Newtonsoft.Json;

namespace MyCrowdCharger.Mobile.Api.Dtos
{
    public class DeviceResult : JsonSerializable
    {
        [JsonProperty("result")]
        public Device Result { get; set; }
    }
}
