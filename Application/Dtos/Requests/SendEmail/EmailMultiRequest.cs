namespace Application.Dtos.Requests.SendEmail
{
    public class EmailMultiRequest
    {
        public List<string> To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string From { get; set; }
    }
}