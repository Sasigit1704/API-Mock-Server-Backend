using ApiMockServer.DTOs;
using ApiMockServer.Models;

namespace ApiMockServer.Interfaces
{
    public interface IMockScenarioService
    {
        Task<List<MockScenario>> GetAllAsync();

        Task<MockScenario?> GetByIdAsync(string id);

        Task CreateAsync(CreateMockScenarioDto dto);

        Task UpdateAsync(string id, UpdateMockScenarioDto dto);

        Task DeleteAsync(string id);

        Task<List<MockScenario>> GetByMockEndpointIdAsync(string mockEndpointId);
    }
}