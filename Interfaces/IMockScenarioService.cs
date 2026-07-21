using ApiMockServer.DTOs;
using ApiMockServer.Models;

namespace ApiMockServer.Interfaces
{
    public interface IMockScenarioService
    {
        Task<List<MockScenario>> GetAllAsync();

        Task<MockScenario?> GetByIdAsync(string id);

        Task CreateAsync(CreateMockScenarioDTO dto);

        Task UpdateAsync(string id, UpdateMockScenarioDTO dto);

        Task<bool> PatchAsync(string id, PatchMockScenarioDTO dto);

        Task DeleteAsync(string id);

        Task<List<MockScenario>> GetByMockEndpointIdAsync(string mockEndpointId);
    }
}