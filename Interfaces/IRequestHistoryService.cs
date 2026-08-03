using ApiMockServer.DTOs;
using ApiMockServer.Models;

namespace ApiMockServer.Interfaces
{
    public interface IRequestHistoryService
    {
        Task<List<CreateRequestHistoryDTO>> GetAllAsync();

        Task<CreateRequestHistoryDTO?> GetByIdAsync(string id);

        Task CreateAsync(RequestLog log);

        Task DeleteAsync(string id);

        Task DeleteAllAsync();
    }
}