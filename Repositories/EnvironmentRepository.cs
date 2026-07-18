using ApiMockServer.Data;
using ApiMockServer.Interfaces;
using ApiMockServer.Models;
using MongoDB.Driver;

namespace ApiMockServer.Repositories
{
    public class EnvironmentRepository : IEnvironmentRepository
    {
        private readonly IMongoCollection<MockEnvironment> _collection;

        public EnvironmentRepository(MongoDbContext context)
        {
            _collection = context.Database.GetCollection<MockEnvironment>("Environments");
        }

        public async Task<List<MockEnvironment>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<MockEnvironment?> GetByIdAsync(string id)
        {
            return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(MockEnvironment environment)
        {
            await _collection.InsertOneAsync(environment);
        }

        public async Task UpdateAsync(string id, MockEnvironment environment)
        {
            environment.Id = id;
            await _collection.ReplaceOneAsync(x => x.Id == id, environment);
        }

        public async Task DeleteAsync(string id)
        {
            await _collection.DeleteOneAsync(x => x.Id == id);
        }
    }
}