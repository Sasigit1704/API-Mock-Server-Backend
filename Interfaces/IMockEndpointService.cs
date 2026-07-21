using ApiMockServer.DTOs;
using ApiMockServer.Models;

namespace ApiMockServer.Interfaces
{
    public interface IMockEndpointService
    {
        Task<List<MockEndpoint>> GetAllAsync();

        Task<MockEndpoint?> GetByIdAsync(string id);

        Task CreateAsync(CreateMockEndpointDTO dto);

        Task UpdateAsync(string id, UpdateMockEndpointDTO dto);

        Task<bool> PatchAsync(string id, PatchMockEndpointDTO dto);

        Task DeleteAsync(string id);
    }
}