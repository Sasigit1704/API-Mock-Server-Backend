using ApiMockServer.Models;

namespace ApiMockServer.Interfaces
{
    public interface IRequestHistoryRepository
    {
        Task<List<RequestHistory>> GetAllAsync();

        Task CreateAsync(RequestHistory log);
    }
}