using ApiMockServer.Models;

namespace ApiMockServer.Interfaces
{
    public interface ICollectionRepository
    {
        Task<List<Collection>> GetAllAsync();

        Task<Collection?> GetByIdAsync(string id);

        Task<Collection?> GetByNameAsync(string name);

        Task CreateAsync(Collection collection);

        Task UpdateAsync(string id, Collection collection);

        Task<bool> PatchAsync(string id, Collection collection);

        Task DeleteAsync(string id);

        Task<bool> ExistsAsync(string id);
    }
}