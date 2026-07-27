using ApiMockServer.Models;

namespace ApiMockServer.Interfaces
{
    public interface IRequestHistoryService
    {
        Task<List<RequestHistory>> GetAllAsync();

        Task CreateAsync(RequestHistory log);
    }
}