using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using SimpleSearch.Storage.DocumentDb;
using SimpleSearch.Uploader.Application.Entities;

namespace SimpleSearch.Uploader.Application.Repositories
{
    public class MongoSessionsRepository : ISessionsRepository
    {
        private readonly MongoDbContext<UploadSession> _context;

        public MongoSessionsRepository(MongoDbContext<UploadSession> context)
        {
            _context = context;
        }

        public async Task<UploadSession> FindNotCompletedSessionAsync(string sessionId, CancellationToken cancellationToken)
        {
            var result = (await _context.Collection.FindAsync(
                Builders<UploadSession>.Filter.Where(s => s.Id == sessionId && !s.IsCompleted),
                cancellationToken: cancellationToken));

            return result.FirstOrDefault();
        }

        public Task CompleteSessionAsync(string sessionId, CancellationToken cancellationToken)
        {
            return _context.Collection.UpdateOneAsync(
                Builders<UploadSession>.Filter.Eq(s => s.Id, sessionId),
                Builders<UploadSession>.Update.Set(s => s.IsCompleted, true),
                cancellationToken: cancellationToken);
        }

        public Task CreateSessionAsync(UploadSession session, CancellationToken cancellationToken)
        {
            return _context.Collection.InsertOneAsync(session, new InsertOneOptions(), cancellationToken);
        }
    }
}