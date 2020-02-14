namespace SimpleSearch.Uploader.ClientRequests
{
    public class StartUploadSessionModel
    {
        public string FileName { get; set; }

        public long SizeInBytes { get; set; }

        public FileExtension Extension { get; set; }
    }
}
