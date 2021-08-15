using System.Net;
using System.Net.Mail;


namespace WebApi.Utils
{
    public static class MailMessages
    {
        static SmtpClient smtpClient = new("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential(
                "rq.timer@gmail.com",
                "ybygsjdsdxrdkytk"
            ),
            EnableSsl = true,
        };

        public static void SendMail(string toEmail, string subject, string body)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress("rq.timer@gmail.com"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(toEmail);
            smtpClient.Send(mailMessage);
        }
    }
}
