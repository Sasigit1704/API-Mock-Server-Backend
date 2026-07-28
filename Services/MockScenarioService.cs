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

        public async Task<MockScenario?> GetActiveScenarioAsync(string mockEndpointId)
        {
            return await _repository.GetActiveScenarioAsync(mockEndpointId);
        }

        public async Task<List<MockScenario>> GetByMockEndpointIdAsync(string mockEndpointId)
        {
            return await _repository.GetByMockEndpointIdAsync(mockEndpointId);
        }

        public async Task CreateAsync(CreateMockScenarioDTO dto)
        {
            var endpoint = await _endpointRepository.GetByIdAsync(dto.MockEndpointId);

            if (endpoint == null)
            {
                throw new ArgumentException("MockEndpoint does not exist.");
            }

            if (dto.FailureRate < 0 || dto.FailureRate > 100)
            {
                throw new ArgumentException("Failure Rate must be between 0 and 100.");
            }

            if (dto.TimeoutDelay < 0)
            {
                throw new ArgumentException("Timeout Delay cannot be negative.");
            }

            var scenario = new MockScenario
            {
                MockEndpointId = dto.MockEndpointId,
                ScenarioName = dto.ScenarioName,
                StatusCode = dto.StatusCode,
                ResponseBody = dto.ResponseBody,
                Delay = dto.Delay,
                IsActive = dto.IsActive,
                EnableRandomFailure = dto.EnableRandomFailure,
                FailureRate = dto.FailureRate,
                EnableTimeout = dto.EnableTimeout,
                TimeoutDelay = dto.TimeoutDelay
            };

            await _repository.CreateAsync(scenario);
        }

        public async Task UpdateAsync(string id, UpdateMockScenarioDTO dto)
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

            if (dto.FailureRate < 0 || dto.FailureRate > 100)
            {
                throw new ArgumentException("Failure Rate must be between 0 and 100.");
            }

            if (dto.TimeoutDelay < 0)
            {
                throw new ArgumentException("Timeout Delay cannot be negative.");
            }

            existingScenario.MockEndpointId = dto.MockEndpointId;
            existingScenario.ScenarioName = dto.ScenarioName;
            existingScenario.StatusCode = dto.StatusCode;
            existingScenario.ResponseBody = dto.ResponseBody;
            existingScenario.Delay = dto.Delay;
            existingScenario.IsActive = dto.IsActive;
            existingScenario.EnableTimeout = dto.EnableTimeout;
            existingScenario.TimeoutDelay = dto.TimeoutDelay;
            
            if (existingScenario.IsActive)
            {
                var scenarios = await _repository.GetByMockEndpointIdAsync(existingScenario.MockEndpointId);

                foreach (var item in scenarios)
                {
                    if (item.Id != existingScenario.Id)
                    {
                        item.IsActive = false;
                        await _repository.UpdateAsync(item.Id, item);
                    }
                }
            }

            await _repository.UpdateAsync(id, existingScenario);
        }

        public async Task<bool> PatchAsync(string id, PatchMockScenarioDTO dto)
        {
            var scenario = await _repository.GetByIdAsync(id);

            if (scenario == null)
            {
                throw new ArgumentException("MockScenario not found.");
            }

            if (dto.MockEndpointId != null)
            {
                var endpoint = await _endpointRepository.GetByIdAsync(dto.MockEndpointId);

                if (endpoint == null)
                {
                    throw new ArgumentException("MockEndpoint does not exist.");
                }

                scenario.MockEndpointId = dto.MockEndpointId;
            }

            if (dto.ScenarioName != null)
                scenario.ScenarioName = dto.ScenarioName;

            if (dto.StatusCode.HasValue)
                scenario.StatusCode = dto.StatusCode.Value;

            if (dto.ResponseBody != null)
                scenario.ResponseBody = dto.ResponseBody;

            if (dto.Delay.HasValue)
                scenario.Delay = dto.Delay.Value;

            if (dto.IsActive.HasValue)
                scenario.IsActive = dto.IsActive.Value;

            if(dto.EnableRandomFailure.HasValue)
                scenario.EnableRandomFailure = dto.EnableRandomFailure.Value;

            if(dto.FailureRate.HasValue)
            {
                if (dto.FailureRate < 0 || dto.FailureRate > 100)
                {
                    throw new ArgumentException("Failure Rate must be between 0 and 100.");
                }
                scenario.FailureRate = dto.FailureRate.Value;
            }

            if (dto.EnableTimeout.HasValue)
            {
                scenario.EnableTimeout = dto.EnableTimeout.Value;
            }

            if (dto.TimeoutDelay.HasValue)
            {
                if (dto.TimeoutDelay.Value < 0)
                {
                    throw new ArgumentException("Timeout Delay cannot be negative.");
                }

                scenario.TimeoutDelay = dto.TimeoutDelay.Value;
            }
            
            if (scenario.IsActive)
            {
                var scenarios = await _repository.GetByMockEndpointIdAsync(scenario.MockEndpointId);

                foreach (var item in scenarios)
                {
                    if (item.Id != scenario.Id)
                    {
                        item.IsActive = false;
                        await _repository.UpdateAsync(item.Id, item);
                    }
                }
            }

            return await _repository.PatchAsync(id, scenario);
        }

        public async Task DeleteAsync(string id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}