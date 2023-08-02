namespace Application.Dtos.Responses.ServiceImage
{
    public class ServiceImageResponse
    {
        public long Id { get; set; }
        public long ServiceId { get; set; }
        public string? NameFile { get; set; }
        public string? NameFileLink { get; set; }
    }
}