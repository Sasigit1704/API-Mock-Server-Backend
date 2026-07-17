using MongoDB.Driver;

namespace ApiMockServer.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(MongoDbSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);

            _database = client.GetDatabase(settings.DatabaseName);
        }

        public IMongoDatabase Database => _database;
    }
}