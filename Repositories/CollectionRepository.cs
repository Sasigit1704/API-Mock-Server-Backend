using ApiMockServer.Data;
using ApiMockServer.Interfaces;
using ApiMockServer.Models;
using MongoDB.Driver;

namespace ApiMockServer.Repositories
{
    public class CollectionRepository : ICollectionRepository
    {
        private readonly IMongoCollection<Collection> _collection;

        public CollectionRepository(MongoDbContext context)
        {
            _collection = context.Database.GetCollection<Collection>("Collections");
        }

        public async Task<List<Collection>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<Collection?> GetByIdAsync(string id)
        {
            return await _collection
                .Find(x => x.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<Collection?> GetByNameAsync(string name)
        {
            return await _collection
                .Find(x => x.Name == name)
                .FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Collection collection)
        {
            await _collection.InsertOneAsync(collection);
        }

        public async Task UpdateAsync(string id, Collection collection)
        {
            collection.Id = id;

            await _collection.ReplaceOneAsync(
                x => x.Id == id,
                collection);
        }

        public async Task DeleteAsync(string id)
        {
            await _collection.DeleteOneAsync(
                x => x.Id == id);
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _collection
                .Find(x => x.Id == id)
                .AnyAsync();
        }
    }
}