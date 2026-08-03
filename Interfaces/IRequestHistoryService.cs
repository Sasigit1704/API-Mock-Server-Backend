using ApiMockServer.Models;

namespace ApiMockServer.Interfaces
{
    public interface IRequestHistoryService
    {
        Task<List<RequestLog>> GetAllAsync();

        Task<RequestLog?> GetByIdAsync(string id);

        Task CreateAsync(RequestLog log);

        Task DeleteAsync(string id);

        Task DeleteAllAsync();
    }
}