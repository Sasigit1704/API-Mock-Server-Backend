using ApiMockServer.DTOs;
using ApiMockServer.Interfaces;
using ApiMockServer.Models;

namespace ApiMockServer.Services
{
    public class CollectionService : ICollectionService
    {
        private readonly ICollectionRepository _repository;

        public CollectionService(ICollectionRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Collection>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Collection?> GetByIdAsync(string id)
        {
            var collection = await _repository.GetByIdAsync(id);
            if (collection == null)
            {
                throw new ArgumentException("Collection not found.");
            }
            return collection;
        }

        public async Task CreateAsync(CreateCollectionDto dto)
        {
            var existingCollection = await _repository.GetByNameAsync(dto.Name);
            if (existingCollection != null)
            {
                throw new ArgumentException("Collection with the same name already exists.");
            }
            var collection = new Collection
            {
                Name = dto.Name,
                Description = dto.Description
            };

            await _repository.CreateAsync(collection);
        }

        public async Task UpdateAsync(string id, UpdateCollectionDto dto)
        {
            var existingCollection = await _repository.GetByIdAsync(id);
            if (existingCollection == null)
            {
                throw new ArgumentException("Collection not found.");
            }
            var collection = new Collection
            {
                Id = id,
                Name = dto.Name,
                Description = dto.Description
            };

            await _repository.UpdateAsync(id, collection);
        }

        public async Task DeleteAsync(string id)
        {
            var collection = await _repository.GetByIdAsync(id);
            if (collection == null)
            {
                throw new ArgumentException("Collection not found.");
            }
            await _repository.DeleteAsync(id);
        }
    }
}