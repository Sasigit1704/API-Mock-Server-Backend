using ApiMockServer.Interfaces;
using ApiMockServer.Models;

namespace ApiMockServer.Services
{
    public class RequestHistoryService : IRequestHistoryService
    {
        private readonly IRequestHistoryRepository _repository;

        public RequestHistoryService(IRequestHistoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<RequestHistory>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task CreateAsync(RequestHistory log)
        {
            await _repository.CreateAsync(log);
        }
    }
}