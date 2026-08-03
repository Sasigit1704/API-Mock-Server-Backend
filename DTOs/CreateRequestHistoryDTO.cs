public class CreateRequestHistoryDTO
{
    public string Id { get; set; } = string.Empty;

    public string Method { get; set; } = string.Empty;

    public string Path { get; set; } = string.Empty;

    public int StatusCode { get; set; }

    public DateTime RequestTime { get; set; }

    public long ResponseTimeMs { get; set; }

    public string? IPAddress { get; set; }

    public string? UserAgent { get; set; }

    public string? EndpointName { get; set; }

    public string? ScenarioName { get; set; }
}