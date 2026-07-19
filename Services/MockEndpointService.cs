using ApiMockServer.DTOs;
using ApiMockServer.Interfaces;
using ApiMockServer.Models;

namespace ApiMockServer.Services
{
    public class MockEndpointService : IMockEndpointService
    {
        private readonly IMockEndpointRepository _repository;
        private readonly ICollectionRepository _collectionRepository;

        public MockEndpointService(IMockEndpointRepository repository, ICollectionRepository collectionRepository)
        {
            _repository = repository;
            _collectionRepository = collectionRepository;
        }

        public async Task<List<MockEndpoint>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<MockEndpoint?> GetByIdAsync(string id)
        {
            var endpoint = await _repository.GetByIdAsync(id);
            if (endpoint == null)
            {
                throw new ArgumentException("Mock endpoint not found.");
            }
            return endpoint;
        }

        public async Task CreateAsync(CreateMockEndpointDto dto)
        {
            var normalizedPath = dto.Path.StartsWith("/")
                ? dto.Path
                : "/" + dto.Path;

            var existingEndpoint = await _repository.GetByMethodAndPathAsync(
                dto.Method,
                normalizedPath);

            if (existingEndpoint != null)
            {
                throw new ArgumentException(
                    $"Endpoint {dto.Method.ToUpper()} {normalizedPath} already exists.");
            }
            
            if (!await _collectionRepository.ExistsAsync(dto.CollectionId))
            {
                throw new ArgumentException("Collection does not exist.");
            }
            
            var endpoint = new MockEndpoint
            {
                Name = dto.Name,
                Path = dto.Path.StartsWith("/") ? dto.Path : "/" + dto.Path,
                Method = dto.Method.ToUpper(),
                StatusCode = dto.StatusCode,
                ResponseBody = dto.ResponseBody,
                IsEnabled = dto.IsEnabled,
                CollectionId = dto.CollectionId
            };

            await _repository.CreateAsync(endpoint);
        }

        public async Task UpdateAsync(string id, UpdateMockEndpointDto dto)
        {
            var existingEndpoint = await _repository.GetByIdAsync(id);

            if (existingEndpoint == null)
            {
                throw new ArgumentException("Mock endpoint not found.");
            }

            var normalizedPath = dto.Path.StartsWith("/")
                ? dto.Path
                : "/" + dto.Path;

            var duplicate = await _repository.GetByMethodAndPathAsync(
                dto.Method,
                normalizedPath);

            if (duplicate != null && duplicate.Id != id)
            {
                throw new ArgumentException(
                    $"Endpoint {dto.Method.ToUpper()} {normalizedPath} already exists.");
            }

            if (!await _collectionRepository.ExistsAsync(dto.CollectionId))
            {
                throw new ArgumentException("Collection does not exist.");
            }

            var endpoint = new MockEndpoint
            {
                Id = id,
                Name = dto.Name,
                Path = normalizedPath,
                Method = dto.Method.ToUpper(),
                StatusCode = dto.StatusCode,
                ResponseBody = dto.ResponseBody,
                IsEnabled = dto.IsEnabled,
                CollectionId = dto.CollectionId
            };

            await _repository.UpdateAsync(id, endpoint);
        }

        public async Task DeleteAsync(string id)
        {
            var endpoint = await _repository.GetByIdAsync(id);
            if (endpoint == null)
            {
                throw new ArgumentException("Mock endpoint not found.");
            }
            await _repository.DeleteAsync(id);
        }
    }
}