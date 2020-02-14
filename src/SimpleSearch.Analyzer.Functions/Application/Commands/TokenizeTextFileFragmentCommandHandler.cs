using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SimpleSearch.Analyzer.Functions.Application.Extensions;
using SimpleSearch.Messages;
using SimpleSearch.Storage.Blobs;

namespace SimpleSearch.Analyzer.Functions.Application.Commands
{
    public class TokenizeTextFileFragmentCommandHandler : 
        IRequestHandler<TokenizeTextFileFragmentCommand, FragmentTokenizedEvent>
    {
        private readonly IBlobStorage _blobStorage;

        public TokenizeTextFileFragmentCommandHandler(IBlobStorage blobStorage)
        {
            _blobStorage = blobStorage;
        }

        public async Task<FragmentTokenizedEvent> Handle(TokenizeTextFileFragmentCommand request, CancellationToken cancellationToken)
        {
            var ms = new MemoryStream();
            var readSuccess = await _blobStorage.ReadRangeToStreamAsync(request.UploadId, ms, request.Offset, request.Length,
                cancellationToken);

            if (!readSuccess)
            {
                return null;
            }

            ms.Position = 0;
            using var sr = new StreamReader(ms);

            var tokens = sr
                .ToCharStream()
                .FilterSpecialCharacters()
                .ToLowerCharacters()
                .SplitWords();

            var tokensFrequencyMap = CalculateFrequency(tokens);

            return new FragmentTokenizedEvent
            {
                Tokens = tokensFrequencyMap
                    .Select(pair => new Token {Tag = pair.Key, Frequency = pair.Value})
                    .ToList(),
                UploadId = request.UploadId, 
                FileName = request.FileName,
                Extension = request.Extension,
                Offset = request.Offset,
                Length = request.Length
            };
        }

        public IDictionary<string, int> CalculateFrequency(IEnumerable<string> words)
        {
            var dict = new Dictionary<string, int>();
            foreach (var word in words)
            {
                if (dict.ContainsKey(word))
                {
                    dict[word]++;
                }
                else
                {
                    dict.Add(word, 1);
                }
            }

            return dict;
        }
    }
}