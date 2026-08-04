using ApiMockServer.Data;
using ApiMockServer.Interfaces;
using ApiMockServer.Models;
using MongoDB.Driver;

namespace ApiMockServer.Repositories
{
    public class RequestHistoryRepository : IRequestHistoryRepository
    {
        private readonly IMongoCollection<RequestLog> _logs;

        public RequestHistoryRepository(MongoDbContext context)
        {
            _logs = context.Database.GetCollection<RequestLog>("RequestLogs");
        }

        public async Task<List<RequestLog>> GetAllAsync()
        {
            return await _logs.Find(_ => true).SortByDescending(x => x.RequestTime).ToListAsync();
        }

        public async Task<RequestLog?> GetByIdAsync(string id)
        {
            return await _logs
                .Find(x => x.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task CreateAsync(RequestLog log)
        {
            await _logs.InsertOneAsync(log);
        }

        public async Task DeleteAsync(string id)
        {
            await _logs.DeleteOneAsync(x => x.Id == id);
        }

        public async Task DeleteAllAsync()
        {
            await _logs.DeleteManyAsync(_ => true);
        }
    }
}