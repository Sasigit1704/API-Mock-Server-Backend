using ApiMockServer.DTOs;
using ApiMockServer.Interfaces;
using ApiMockServer.Models;

namespace ApiMockServer.Services
{
    public class RequestHistoryService : IRequestHistoryService
    {
        private readonly IRequestHistoryRepository _repository;
        private readonly IMockEndpointRepository _endpointRepository;
        private readonly IMockScenarioRepository _scenarioRepository;

        public RequestHistoryService(
            IRequestHistoryRepository repository,
            IMockEndpointRepository endpointRepository,
            IMockScenarioRepository scenarioRepository)
        {
            _repository = repository;
            _endpointRepository = endpointRepository;
            _scenarioRepository = scenarioRepository;
        }

        public async Task<List<CreateRequestHistoryDTO>> GetAllAsync()
        {
            var logs = await _repository.GetAllAsync();

            var result = new List<CreateRequestHistoryDTO>();

            foreach (var log in logs)
            {
                var endpoint = log.MockEndpointId == null
                    ? null
                    : await _endpointRepository.GetByIdAsync(log.MockEndpointId);

                var scenario = log.MockScenarioId == null
                    ? null
                    : await _scenarioRepository.GetByIdAsync(log.MockScenarioId);

                result.Add(new CreateRequestHistoryDTO
                {
                    Id = log.Id,
                    Method = log.Method,
                    Path = log.Path,
                    StatusCode = log.StatusCode,
                    RequestTime = log.RequestTime,
                    ResponseTimeMs = log.ResponseTimeMs,
                    IPAddress = log.IPAddress,
                    UserAgent = log.UserAgent,
                    EndpointName = endpoint?.Name,
                    ScenarioName = scenario?.ScenarioName
                });
            }

            return result;
        }

        public async Task<CreateRequestHistoryDTO?> GetByIdAsync(string id)
        {
            var log = await _repository.GetByIdAsync(id);

            if (log == null)
                return null;

            var endpoint = log.MockEndpointId == null
                ? null
                : await _endpointRepository.GetByIdAsync(log.MockEndpointId);

            var scenario = log.MockScenarioId == null
                ? null
                : await _scenarioRepository.GetByIdAsync(log.MockScenarioId);

            return new CreateRequestHistoryDTO
            {
                Id = log.Id,
                Method = log.Method,
                Path = log.Path,
                StatusCode = log.StatusCode,
                RequestTime = log.RequestTime,
                ResponseTimeMs = log.ResponseTimeMs,
                IPAddress = log.IPAddress,
                UserAgent = log.UserAgent,
                EndpointName = endpoint?.Name,
                ScenarioName = scenario?.ScenarioName
            };
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