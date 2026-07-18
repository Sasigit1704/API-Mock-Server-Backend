using ApiMockServer.Models;

namespace ApiMockServer.Interfaces
{
    public interface IEnvironmentRepository
    {
        Task<List<MockEnvironment>> GetAllAsync();

        Task<MockEnvironment?> GetByIdAsync(string id);

        Task CreateAsync(MockEnvironment environment);

        Task UpdateAsync(string id, MockEnvironment environment);

        Task DeleteAsync(string id);
    }
}