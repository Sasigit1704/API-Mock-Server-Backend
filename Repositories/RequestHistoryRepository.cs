using ApiMockServer.Data;
using ApiMockServer.Interfaces;
using ApiMockServer.Models;
using MongoDB.Driver;

namespace ApiMockServer.Repositories
{
    public class RequestHistoryRepository : IRequestHistoryRepository
    {
        private readonly IMongoCollection<RequestHistory> _logs;

        public RequestHistoryRepository(MongoDbContext context)
        {
            _logs = context.Database.GetCollection<RequestHistory>("RequestHistory");
        }

        public async Task<List<RequestHistory>> GetAllAsync()
        {
            return await _logs.Find(_ => true).ToListAsync();
        }

        public async Task CreateAsync(RequestHistory log)
        {
            await _logs.InsertOneAsync(log);
        }
    }
}