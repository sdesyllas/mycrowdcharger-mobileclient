using System.Collections.Generic;
using Newtonsoft.Json;

namespace MyCrowdCharger.Mobile.Api.Dtos
{
    public class DevicesResult : JsonSerializable
    {
        [JsonProperty("result")]
        public List<Device> Result { get; set; }
    }
}
