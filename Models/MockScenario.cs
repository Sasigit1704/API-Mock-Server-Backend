using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ApiMockServer.Models
{
    public class MockScenario
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("mockEndpointId")]
        public string MockEndpointId { get; set; } = string.Empty;

        [BsonElement("scenarioName")]
        public string ScenarioName { get; set; } = string.Empty;

        [BsonElement("statusCode")]
        public int StatusCode { get; set; }

        [BsonElement("responseBody")]
        public string ResponseBody { get; set; } = string.Empty;

        [BsonElement("delay")]
        public int Delay { get; set; }

        [BsonElement("isActive")]
        public bool IsActive { get; set; } = true;
    }
}