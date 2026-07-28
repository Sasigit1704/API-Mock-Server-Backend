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

// POST
        public async Task CreateAsync(CreateMockScenarioDTO dto)
        {
            await ValidateEndpointExistsAsync(dto.MockEndpointId);

            ValidateFailureRate(dto.FailureRate);

            ValidateTimeoutDelay(dto.TimeoutDelay);

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

            if (scenario.IsActive)
            {
                await DeactivateOtherScenariosAsync(scenario.MockEndpointId, string.Empty);
            }

            await _repository.CreateAsync(scenario);
        }

// PUT
        public async Task UpdateAsync(string id, UpdateMockScenarioDTO dto)
        {
            var existingScenario = await _repository.GetByIdAsync(id);

            if (existingScenario == null)
            {
                throw new ArgumentException("MockScenario not found.");
            }

            await ValidateEndpointExistsAsync(dto.MockEndpointId);

            ValidateFailureRate(dto.FailureRate);

            ValidateTimeoutDelay(dto.TimeoutDelay);

            existingScenario.MockEndpointId = dto.MockEndpointId;
            existingScenario.ScenarioName = dto.ScenarioName;
            existingScenario.StatusCode = dto.StatusCode;
            existingScenario.ResponseBody = dto.ResponseBody;
            existingScenario.Delay = dto.Delay;
            existingScenario.IsActive = dto.IsActive;
            existingScenario.EnableRandomFailure = dto.EnableRandomFailure;
            existingScenario.FailureRate = dto.FailureRate;
            existingScenario.EnableTimeout = dto.EnableTimeout;
            existingScenario.TimeoutDelay = dto.TimeoutDelay;
            
            if (existingScenario.IsActive)
            {
                await DeactivateOtherScenariosAsync(existingScenario.MockEndpointId, existingScenario.Id);
            }

            await _repository.UpdateAsync(id, existingScenario);
        }

// PATCH
        public async Task<bool> PatchAsync(string id, PatchMockScenarioDTO dto)
        {
            var scenario = await _repository.GetByIdAsync(id);

            if (scenario == null)
            {
                throw new ArgumentException("MockScenario not found.");
            }

            if (dto.MockEndpointId != null)
            {
                await ValidateEndpointExistsAsync(dto.MockEndpointId);

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
                ValidateFailureRate(dto.FailureRate.Value);

                scenario.FailureRate = dto.FailureRate.Value;
            }

            if (dto.EnableTimeout.HasValue)
            {
                scenario.EnableTimeout = dto.EnableTimeout.Value;
            }

            if (dto.TimeoutDelay.HasValue)
            {
                ValidateTimeoutDelay(dto.TimeoutDelay.Value);

                scenario.TimeoutDelay = dto.TimeoutDelay.Value;
            }
            
            if (scenario.IsActive)
            {
                await DeactivateOtherScenariosAsync(scenario.MockEndpointId, scenario.Id);
            }

            return await _repository.PatchAsync(id, scenario);
        }

        public async Task DeleteAsync(string id)
        {
            await _repository.DeleteAsync(id);
        }

//HELPER METHODS

// Reusable code for Deactivating other Scenarios
        private async Task DeactivateOtherScenariosAsync(string mockEndpointId, string currentScenarioId)
        {
            var scenarios = await _repository.GetByMockEndpointIdAsync(mockEndpointId);

            foreach (var item in scenarios)
            {
                if (item.Id != currentScenarioId && item.IsActive)
                {
                    item.IsActive = false;
                    await _repository.UpdateAsync(item.Id, item);
                }
            }
        }

// Reusable Code for Validating Failure Rate
        private void ValidateFailureRate(int failureRate)
        {
            if (failureRate < 0 || failureRate > 100)
            {
                throw new ArgumentException("Failure Rate must be between 0 and 100.");
            }
        }

// Reusable Code for Validating Timeout Delay
        private void ValidateTimeoutDelay(int timeoutDelay)
        {
            if (timeoutDelay < 0)
            {
                throw new ArgumentException("Timeout Delay cannot be negative.");
            }
        }

// Reusable Code for Validating Endpoint Existance
        private async Task ValidateEndpointExistsAsync(string endpointId)
        {
            var endpoint = await _endpointRepository.GetByIdAsync(endpointId);

            if (endpoint == null)
            {
                throw new ArgumentException("MockEndpoint does not exist.");
            }
        }
    }
}