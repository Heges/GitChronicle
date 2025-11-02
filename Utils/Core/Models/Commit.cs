using System.Text.Json.Serialization;

namespace Utils.Core.Models
{
    public class Commit
    {
        [JsonPropertyName("hash")] public string? Hash { get; set; }
        [JsonPropertyName("author")] public string? Author { get; set; }
        [JsonPropertyName("date")] public string? Date { get; set; }
        [JsonPropertyName("subject")] public string? Subject { get; set; }
        [JsonPropertyName("body")] public string? Body { get; set; }
    }
}
