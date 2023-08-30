using System;
using FDex.Application.Contracts.Infrastructure;
using FDex.Application.Models.Infrastructure;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace FDex.Infrastructure.Mail
{
	public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;
        public EmailSender(IConfiguration config)
		{
            _config = config;
        }

        public async Task SendEmail(Email email)
        {
            MailMessage message = new MailMessage();
            SmtpClient smtp = new SmtpClient();
            message.From = new MailAddress(_config["Email:Account"]);
            message.To.Add(new MailAddress(email.To));
            message.Subject = email.Subject;
            message.Body = email.Body;
            smtp.Port = 587;
            smtp.Host = "smtp.gmail.com";
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential(_config["Email:Account"], _config["Email:Password"]);
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Send(message);
        }
    }
}

