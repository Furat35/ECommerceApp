namespace ECommerce.Core.BackgroundServices.DTOs
{
    public class UserMessage
    {
        public string From { get; set; }
        public List<string> To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
    }
}
