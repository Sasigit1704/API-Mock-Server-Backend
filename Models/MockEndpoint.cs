using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ApiMockServer.Models
{
    public class MockEndpoint
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("path")]
        public string Path { get; set; } = string.Empty;

        [BsonElement("method")]
        public string Method { get; set; } = string.Empty;

        [BsonElement("statusCode")]
        public int StatusCode { get; set; }

        [BsonElement("responseBody")]
        public string ResponseBody { get; set; } = string.Empty;

        [BsonElement("isEnabled")]
        public bool IsEnabled { get; set; } = true;
    }
}