using ApiMockServer.Data;
using ApiMockServer.Interfaces;
using ApiMockServer.Models;
using MongoDB.Driver;

namespace ApiMockServer.Repositories
{
    public class MockScenarioRepository : IMockScenarioRepository
    {
        private readonly IMongoCollection<MockScenario> _scenario;

        public MockScenarioRepository(MongoDbContext context)
        {
            _scenario = context.Database.GetCollection<MockScenario>("MockScenarios");
        }

        public async Task<List<MockScenario>> GetAllAsync()
        {
            return await _scenario.Find(_ => true).ToListAsync();
        }

        public async Task<MockScenario?> GetByIdAsync(string id)
        {
            return await _scenario.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(MockScenario scenario)
        {
            await _scenario.InsertOneAsync(scenario);
        }

        public async Task UpdateAsync(string id, MockScenario scenario)
        {
            await _scenario.ReplaceOneAsync(x => x.Id == id, scenario);
        }

        public async Task<bool> PatchAsync(string id, MockScenario scenario)
        {
            scenario.Id = id;
            var result = await _scenario.ReplaceOneAsync(
                x => x.Id == scenario.Id,
                scenario);

            return result.ModifiedCount > 0;
        }

        public async Task DeleteAsync(string id)
        {
            await _scenario.DeleteOneAsync(x => x.Id == id);
        }
        
        // Retrieves all mock scenarios associated with a specific mock endpoint ID.
        public async Task<List<MockScenario>> GetByMockEndpointIdAsync(string mockEndpointId)
        {
            return await _scenario
                .Find(x => x.MockEndpointId == mockEndpointId)
                .ToListAsync();
        }
    }
}