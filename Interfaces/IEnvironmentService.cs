using ApiMockServer.DTOs;
using ApiMockServer.Models;

namespace ApiMockServer.Interfaces
{
    public interface IEnvironmentService
    {
        Task<List<MockEnvironment>> GetAllAsync();

        Task<MockEnvironment?> GetByIdAsync(string id);

        Task CreateAsync(CreateEnvironmentDto dto);

        Task UpdateAsync(string id, UpdateEnvironmentDto dto);

        Task DeleteAsync(string id);
    }
}