using ApiMockServer.Data;
using ApiMockServer.Interfaces;
using ApiMockServer.Models;
using MongoDB.Driver;

namespace ApiMockServer.Repositories
{
    public class MockScenarioRepository : IMockScenarioRepository
    {
        private readonly IMongoCollection<MockScenario> _mockScenarios;

        public MockScenarioRepository(MongoDbContext context)
        {
            _mockScenarios = context.Database.GetCollection<MockScenario>("MockScenarios");
        }

        public async Task<List<MockScenario>> GetAllAsync()
        {
            return await _mockScenarios.Find(_ => true).ToListAsync();
        }

        public async Task<MockScenario?> GetByIdAsync(string id)
        {
            return await _mockScenarios.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(MockScenario scenario)
        {
            await _mockScenarios.InsertOneAsync(scenario);
        }

        public async Task UpdateAsync(string id, MockScenario scenario)
        {
            await _mockScenarios.ReplaceOneAsync(x => x.Id == id, scenario);
        }

        public async Task DeleteAsync(string id)
        {
            await _mockScenarios.DeleteOneAsync(x => x.Id == id);
        }
        
        // Retrieves all mock scenarios associated with a specific mock endpoint ID.
        public async Task<List<MockScenario>> GetByMockEndpointIdAsync(string mockEndpointId)
        {
            return await _mockScenarios
                .Find(x => x.MockEndpointId == mockEndpointId)
                .ToListAsync();
        }
    }
}