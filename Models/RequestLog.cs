using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ApiMockServer.Models;

public class RequestLog
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("method")]
    public string Method { get; set; } = string.Empty;

    [BsonElement("path")]
    public string Path { get; set; } = string.Empty;

    [BsonElement("statusCode")]
    public int StatusCode { get; set; }

    [BsonElement("requestTime")]
    public DateTime RequestTime { get; set; }

    [BsonElement("responseTimeMs")]
    public long ResponseTimeMs { get; set; }

    [BsonElement("ipAddress")]
    public string? IPAddress { get; set; }

    [BsonElement("userAgent")]
    public string? UserAgent { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("mockEndpointId")]
    public string? MockEndpointId { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("mockScenarioId")]
    public string? MockScenarioId { get; set; }
}