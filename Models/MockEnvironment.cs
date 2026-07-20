using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ApiMockServer.Models
{
    public class MockEnvironment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("baseUrl")]
        public string BaseUrl { get; set; } = string.Empty;

        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("status")]
        public bool IsActive { get; set; }
    }
}