using Newtonsoft.Json;

namespace MyCrowdCharger.Mobile.Api.Dtos
{
    public class JsonSerializable
    {
        public override string ToString()
        {
            var content = JsonConvert.SerializeObject(this);
            return content;
        }
    }
}
