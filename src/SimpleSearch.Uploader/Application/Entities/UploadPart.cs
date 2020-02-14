namespace SimpleSearch.Uploader.Application.Entities
{
    public class UploadPart
    {
        public string Id { get; set; }

        public long Offset { get; set; }

        public long SizeInBytes { get; set; }
    }
}
