using Newtonsoft.Json;

public class EvaluationModel
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("question")]
    public string Question { get; set; }

    [JsonProperty("liked_percentage")]
    public double LikedPercentage { get; set; }

    [JsonProperty("disliked_percentage")]
    public double DislikedPercentage { get; set; }

    [JsonProperty("total_answers")]
    public int TotalAnswers { get; set; }

    [JsonProperty("created_at")]
    public string CreatedAt { get; set; }

    [JsonProperty("last_modified_at")]
    public string LastModifiedAt { get; set; }
}