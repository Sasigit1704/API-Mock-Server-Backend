using ApiMockServer.DTOs;
using ApiMockServer.Interfaces;
using ApiMockServer.Models;

namespace ApiMockServer.Services
{
    public class MockScenarioService : IMockScenarioService
    {
        private readonly IMockScenarioRepository _repository;
        private readonly IMockEndpointRepository _endpointRepository;

        public MockScenarioService(
            IMockScenarioRepository repository,
            IMockEndpointRepository endpointRepository)
        {
            _repository = repository;
            _endpointRepository = endpointRepository;
        }

        public async Task<List<MockScenario>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<MockScenario?> GetByIdAsync(string id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<List<MockScenario>> GetByMockEndpointIdAsync(string mockEndpointId)
        {
            return await _repository.GetByMockEndpointIdAsync(mockEndpointId);
        }

        public async Task CreateAsync(CreateMockScenarioDto dto)
        {
            var endpoint = await _endpointRepository.GetByIdAsync(dto.MockEndpointId);

            if (endpoint == null)
            {
                throw new ArgumentException("MockEndpoint does not exist.");
            }

            var scenario = new MockScenario
            {
                MockEndpointId = dto.MockEndpointId,
                ScenarioName = dto.ScenarioName,
                StatusCode = dto.StatusCode,
                ResponseBody = dto.ResponseBody,
                Delay = dto.Delay,
                IsActive = dto.IsActive
            };

            await _repository.CreateAsync(scenario);
        }

        public async Task UpdateAsync(string id, UpdateMockScenarioDto dto)
        {
            var existingScenario = await _repository.GetByIdAsync(id);

            if (existingScenario == null)
            {
                throw new ArgumentException("MockScenario not found.");
            }

            var endpoint = await _endpointRepository.GetByIdAsync(dto.MockEndpointId);

            if (endpoint == null)
            {
                throw new ArgumentException("MockEndpoint does not exist.");
            }

            existingScenario.MockEndpointId = dto.MockEndpointId;
            existingScenario.ScenarioName = dto.ScenarioName;
            existingScenario.StatusCode = dto.StatusCode;
            existingScenario.ResponseBody = dto.ResponseBody;
            existingScenario.Delay = dto.Delay;
            existingScenario.IsActive = dto.IsActive;

            await _repository.UpdateAsync(id, existingScenario);
        }

        public async Task DeleteAsync(string id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}