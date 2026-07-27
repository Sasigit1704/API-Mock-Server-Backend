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
            var endpoints = await _endpoint
                .Find(x =>
                    x.Method == method.ToUpper() &&
                    x.IsEnabled)
                .ToListAsync();
            foreach (var endpoint in endpoints)
            {
                if(IsPathMatch(endpoint.Path, path))
                {
                    return endpoint;
                }
            }
            return null;
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

        private bool IsPathMatch(string storedPath, string requestPath)
        {
            var storedSegments = storedPath.Trim('/').Split('/');
            var requestSegments = requestPath.Trim('/').Split('/');

            if (storedSegments.Length != requestSegments.Length)
                return false;
            for (int i = 0; i < storedSegments.Length; i++)
            {
                if (storedSegments[i].StartsWith("{") &&
                    storedSegments[i].EndsWith("}"))
                {
                    continue;
                }
                if (!storedSegments[i].Equals(
                        requestSegments[i],
                        StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }
            return true;
        }
    }
}