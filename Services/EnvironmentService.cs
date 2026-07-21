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

        public async Task CreateAsync(CreateEnvironmentDTO dto)
        {
            if (dto.IsActive)
            {
                var environments = await _repository.GetAllAsync();
                foreach (var env in environments)
                {
                    env.IsActive = false;
                    await _repository.UpdateAsync(env.Id, env);
                }
            }            
            var environment = new MockEnvironment
            {
                Name = dto.Name,
                BaseUrl = dto.BaseUrl,
                Description = dto.Description,
                IsActive = dto.IsActive
            };

            await _repository.CreateAsync(environment);
        }

        public async Task UpdateAsync(string id, UpdateEnvironmentDTO dto)
        {
            if (dto.IsActive)
            {
                var environments = await _repository.GetAllAsync();
                foreach (var env in environments)
                {
                    if (env.Id != id && env.IsActive)
                    {
                        env.IsActive = false;
                        await _repository.UpdateAsync(env.Id, env);
                    }
                }
            }   
            var environment = new MockEnvironment
            {
                Id = id,
                Name = dto.Name,
                BaseUrl = dto.BaseUrl,
                Description = dto.Description,
                IsActive = dto.IsActive
            };

            await _repository.UpdateAsync(id, environment);
        }

        public async Task<bool> PatchAsync(string id, PatchEnvironmentDTO dto)
        {
            var environment = await _repository.GetByIdAsync(id);

            if (environment == null)
            {
                throw new ArgumentException("Environment not found.");
            }

            if (dto.IsActive.HasValue && dto.IsActive.Value)
            {
                var environments = await _repository.GetAllAsync();

                foreach (var env in environments)
                {
                    if (env.Id != id && env.IsActive)
                    {
                        env.IsActive = false;
                        await _repository.UpdateAsync(env.Id, env);
                    }
                }
            }

            if (dto.Name != null)
                environment.Name = dto.Name;

            if (dto.BaseUrl != null)
                environment.BaseUrl = dto.BaseUrl;

            if (dto.Description != null)
                environment.Description = dto.Description;

            if (dto.IsActive.HasValue)
                environment.IsActive = dto.IsActive.Value;

            return await _repository.PatchAsync(id, environment);
        }

        public async Task DeleteAsync(string id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}