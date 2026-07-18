using ApiMockServer.DTOs;
using ApiMockServer.Interfaces;
using ApiMockServer.Models;

namespace ApiMockServer.Services
{
    public class EnvironmentService : IEnvironmentService
    {
        private readonly IEnvironmentRepository _repository;

        public EnvironmentService(IEnvironmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<MockEnvironment>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<MockEnvironment?> GetByIdAsync(string id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task CreateAsync(CreateEnvironmentDto dto)
        {
            var environment = new MockEnvironment
            {
                Name = dto.Name,
                BaseUrl = dto.BaseUrl,
                Description = dto.Description
            };

            await _repository.CreateAsync(environment);
        }

        public async Task UpdateAsync(string id, UpdateEnvironmentDto dto)
        {
            var environment = new MockEnvironment
            {
                Id = id,
                Name = dto.Name,
                BaseUrl = dto.BaseUrl,
                Description = dto.Description
            };

            await _repository.UpdateAsync(id, environment);
        }

        public async Task DeleteAsync(string id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}