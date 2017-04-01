using Newtonsoft.Json;

namespace OauthSample.Model
{
    public class MailChimpApiMetadata
    {
        [JsonProperty("api_endpoint")]
        public string ApiEndpoint { get; set; }

        [JsonProperty("dc")]
        public string DataCenter { get; set; }
    }
}