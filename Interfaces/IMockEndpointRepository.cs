using ApiMockServer.Models;

namespace ApiMockServer.Interfaces
{
    public interface IMockEndpointRepository
    {
        Task<List<MockEndpoint>> GetAllAsync();

        Task<MockEndpoint?> GetByIdAsync(string id);

        Task CreateAsync(MockEndpoint endpoint);

        Task UpdateAsync(string id, MockEndpoint endpoint);

        Task DeleteAsync(string id);
    }
}