using ApiMockServer.Interfaces;
using ApiMockServer.Models;

namespace ApiMockServer.Services
{
    public class RequestHistoryService : IRequestHistoryService
    {
        private readonly IRequestHistoryRepository _repository;

        public RequestHistoryService(
            IRequestHistoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<RequestLog>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<RequestLog?> GetByIdAsync(string id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task CreateAsync(RequestLog log)
        {
            await _repository.CreateAsync(log);
        }

        public async Task DeleteAsync(string id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task DeleteAllAsync()
        {
            await _repository.DeleteAllAsync();
        }
    }
}