using ApiMockServer.Data;
using ApiMockServer.Interfaces;
using ApiMockServer.Models;
using MongoDB.Driver;

namespace ApiMockServer.Repositories
{
    public class MockEndpointRepository : IMockEndpointRepository
    {
        private readonly IMongoCollection<MockEndpoint> _collection;

        public MockEndpointRepository(MongoDbContext context)
        {
            _collection = context.Database
                .GetCollection<MockEndpoint>("MockEndpoints");
        }

        public async Task<List<MockEndpoint>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<MockEndpoint?> GetByIdAsync(string id)
        {
            return await _collection
                .Find(x => x.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<MockEndpoint?> GetByPathAsync(string path)
        {
            return await _collection
                .Find(x => x.Path == path)
                .FirstOrDefaultAsync();
        }

        public async Task<MockEndpoint?> GetByMethodAndPathAsync(string method, string path)
        {
            return await _collection
                .Find(x =>
                    x.Method == method.ToUpper() &&
                    x.Path.ToLower() == path.ToLower())
                .FirstOrDefaultAsync();
        }
        
        public async Task CreateAsync(MockEndpoint endpoint)
        {
            await _collection.InsertOneAsync(endpoint);
        }

        public async Task UpdateAsync(string id, MockEndpoint endpoint)
        {
            endpoint.Id = id;

            await _collection.ReplaceOneAsync(
                x => x.Id == id,
                endpoint);
        }

        public async Task DeleteAsync(string id)
        {
            await _collection.DeleteOneAsync(
                x => x.Id == id);
        }
    }
}