using ApiMockServer.Models;

namespace ApiMockServer.Interfaces
{
    public interface IMockEndpointRepository
    {
        Task<List<MockEndpoint>> GetAllAsync();

        Task<MockEndpoint?> GetByIdAsync(string id);

        Task<MockEndpoint?> GetByPathAsync(string path);

        Task<MockEndpoint?> GetByMethodAndPathAsync(string method, string path);

        Task CreateAsync(MockEndpoint endpoint);

        Task UpdateAsync(string id, MockEndpoint endpoint);

        Task<bool> PatchAsync(string id, MockEndpoint endpoint);

        Task DeleteAsync(string id);
    }
}