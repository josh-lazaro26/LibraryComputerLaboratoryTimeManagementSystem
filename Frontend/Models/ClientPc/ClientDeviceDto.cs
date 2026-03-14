using Newtonsoft.Json;

public class ClientDeviceDto
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("name")]
    public string DeviceName { get; set; }

    [JsonProperty("connected_at")]
    public string ConnectedAt { get; set; }
}