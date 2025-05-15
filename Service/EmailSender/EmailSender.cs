using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Service.EmailSender
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var mail = "recipemobileapp@gmail.com";
            var password = "eoxi cyga uxlu oqis";
            var Client = new SmtpClient("smtp.gmail.com", 587)
            {
                Port = 587,
                Credentials = new NetworkCredential(mail, password),
                EnableSsl = true,
            };
            return Client.SendMailAsync(new MailMessage(from: mail, to: email, subject, message));
        }
    }
}
