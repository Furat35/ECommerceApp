namespace ECommerce.Mail.Abstract
{
    public interface IMailService
    {
        void SendMail(string to, string subject, string body);
    }
}
