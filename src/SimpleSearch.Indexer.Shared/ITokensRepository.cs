using System.Threading;
using System.Threading.Tasks;
using SimpleSearch.Indexer.Shared.Entities;

namespace SimpleSearch.Indexer.Shared
{
    public interface ITokensRepository
    {
        Task AddDocumentToTokenAsync(DocumentEntity document, string tag, CancellationToken cancellationToken);

        Task<TokenEntity> FindByTagAsync(string tag, CancellationToken cancellationToken);
    }
}