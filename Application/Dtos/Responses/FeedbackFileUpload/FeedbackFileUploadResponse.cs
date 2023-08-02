namespace Application.Dtos.Responses.FeedbackFileUpload
{
    public class FeedbackFileUploadResponse
    {
        public long Id { get; set; }
        public long FeedbackId { get; set; }
        public string NameFile { get; set; }
        public string TypeFile { get; set; }
    }
}
