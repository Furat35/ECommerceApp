using ECommerce.Mail.Abstract;
using System.Net;
using System.Net.Mail;

namespace ECommerce.Mail.Concrete
{
    public class MailService : IMailService
    {

        public void SendMail(string to, string subject, string body)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("firatdeneme@firatortac.com.tr");
            mailMessage.To.Add("furat__@hotmail.com");
            mailMessage.Subject = subject;
            mailMessage.Body = body;

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = "77.245.159.8";
            smtpClient.Port = 587;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential("firatdeneme@firatortac.com.tr", "Firat.3521");
            smtpClient.EnableSsl = false;

            try
            {
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}
