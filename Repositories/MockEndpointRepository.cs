using ApiMockServer.Data;
using ApiMockServer.Interfaces;
using ApiMockServer.Models;
using MongoDB.Driver;

namespace ApiMockServer.Repositories
{
    public class MockEndpointRepository : IMockEndpointRepository
    {
        private readonly IMongoCollection<MockEndpoint> _endpoint;

        public MockEndpointRepository(MongoDbContext context)
        {
            _endpoint = context.Database
                .GetCollection<MockEndpoint>("MockEndpoints");
        }

        public async Task<List<MockEndpoint>> GetAllAsync()
        {
            return await _endpoint.Find(_ => true).ToListAsync();
        }

        public async Task<MockEndpoint?> GetByIdAsync(string id)
        {
            return await _endpoint
                .Find(x => x.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<MockEndpoint?> GetByPathAsync(string path)
        {
            return await _endpoint
                .Find(x => x.Path == path)
                .FirstOrDefaultAsync();
        }

        public async Task<MockEndpoint?> GetByMethodAndPathAsync(string method, string path)
        {
            return await _endpoint
                .Find(x =>
                    x.Method == method.ToUpper() &&
                    x.Path.ToLower() == path.ToLower())
                .FirstOrDefaultAsync();
        }
        
        public async Task CreateAsync(MockEndpoint endpoint)
        {
            await _endpoint.InsertOneAsync(endpoint);
        }

        public async Task UpdateAsync(string id, MockEndpoint endpoint)
        {
            endpoint.Id = id;

            await _endpoint.ReplaceOneAsync(
                x => x.Id == id,
                endpoint);
        }

        public async Task<bool> PatchAsync(string id, MockEndpoint endpoint)
        {
            endpoint.Id = id;
            var result = await _endpoint.ReplaceOneAsync(
                x => x.Id == endpoint.Id,
                endpoint);

            return result.ModifiedCount > 0;
        }

        public async Task DeleteAsync(string id)
        {
            await _endpoint.DeleteOneAsync(
                x => x.Id == id);
        }
    }
}