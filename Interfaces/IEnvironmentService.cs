using ApiMockServer.DTOs;
using ApiMockServer.Models;

namespace ApiMockServer.Interfaces
{
    public interface IEnvironmentService
    {
        Task<List<MockEnvironment>> GetAllAsync();

        Task<MockEnvironment?> GetByIdAsync(string id);

        Task CreateAsync(CreateEnvironmentDTO dto);

        Task UpdateAsync(string id, UpdateEnvironmentDTO dto);

        Task<bool> PatchAsync(string id, PatchEnvironmentDTO dto);

        Task DeleteAsync(string id);
    }
}