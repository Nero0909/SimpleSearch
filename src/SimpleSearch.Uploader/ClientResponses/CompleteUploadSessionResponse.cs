using System;
using System.Collections.Generic;

namespace SimpleSearch.Uploader.ClientResponses
{
    public class CompleteUploadSessionResponse
    {
        public CompleteUploadSessionResponse()
        {
            CorruptedParts = Array.Empty<CorruptedPart>();
        }

        public CompleteUploadSessionResponse(IEnumerable<CorruptedPart> corruptedParts)
        {
            CorruptedParts = corruptedParts;
        }

        public IEnumerable<CorruptedPart> CorruptedParts { get; }
    }
}