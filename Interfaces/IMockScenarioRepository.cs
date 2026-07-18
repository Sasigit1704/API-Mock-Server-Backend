using ApiMockServer.Models;

namespace ApiMockServer.Interfaces
{
    public interface IMockScenarioRepository
    {
        Task<List<MockScenario>> GetAllAsync();

        Task<MockScenario?> GetByIdAsync(string id);

        Task CreateAsync(MockScenario scenario);

        Task UpdateAsync(string id, MockScenario scenario);

        Task DeleteAsync(string id);

        // Retrieves all mock scenarios associated with a specific mock endpoint ID.
        Task<List<MockScenario>> GetByMockEndpointIdAsync(string mockEndpointId);
    }
}