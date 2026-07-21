using ApiMockServer.Data;
using ApiMockServer.Interfaces;
using ApiMockServer.Models;
using MongoDB.Driver;

namespace ApiMockServer.Repositories
{
    public class EnvironmentRepository : IEnvironmentRepository
    {
        private readonly IMongoCollection<MockEnvironment> _environment;

        public EnvironmentRepository(MongoDbContext context)
        {
            _environment = context.Database.GetCollection<MockEnvironment>("Environments");
        }

        public async Task<List<MockEnvironment>> GetAllAsync()
        {
            return await _environment.Find(_ => true).ToListAsync();
        }

        public async Task<MockEnvironment?> GetByIdAsync(string id)
        {
            return await _environment.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(MockEnvironment environment)
        {
            await _environment.InsertOneAsync(environment);
        }

        public async Task UpdateAsync(string id, MockEnvironment environment)
        {
            environment.Id = id;
            await _environment.ReplaceOneAsync(x => x.Id == id, environment);
        }

        public async Task<bool> PatchAsync(string id, MockEnvironment environment)
        {
            environment.Id = id;
            var result = await _environment.ReplaceOneAsync(
                x => x.Id == environment.Id,
                environment);

            return result.ModifiedCount > 0;
        }

        public async Task DeleteAsync(string id)
        {
            await _environment.DeleteOneAsync(x => x.Id == id);
        }
    }
}