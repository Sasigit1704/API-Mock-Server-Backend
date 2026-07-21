using ApiMockServer.DTOs;
using ApiMockServer.Models;

namespace ApiMockServer.Interfaces
{
    public interface ICollectionService
    {
        Task<List<Collection>> GetAllAsync();

        Task<Collection?> GetByIdAsync(string id);

        Task CreateAsync(CreateCollectionDTO dto);

        Task UpdateAsync(string id, UpdateCollectionDTO dto);

        Task<bool> PatchAsync(string id, PatchCollectionDTO dto);

        Task DeleteAsync(string id);
    }
}