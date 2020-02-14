using System.Threading;
using System.Threading.Tasks;
using SimpleSearch.Uploader.Application.Entities;

namespace SimpleSearch.Uploader.Application.Repositories
{
    public interface ISessionsRepository
    {
        Task<UploadSession> FindNotCompletedSessionAsync(string sessionId, CancellationToken cancellationToken);

        Task CompleteSessionAsync(string sessionId, CancellationToken cancellationToken);

        Task CreateSessionAsync(UploadSession session, CancellationToken cancellationToken);
    }
}