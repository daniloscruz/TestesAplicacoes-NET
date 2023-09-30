using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace ServiceModels
{
    public class UserApiViewModel
    {
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public int CodigoCliente { get; set; }

        [JsonProperty("User")]
        public string? User { get; set; }
        [JsonProperty("Password")]
        public string? Password { get; set; }

    }
}
