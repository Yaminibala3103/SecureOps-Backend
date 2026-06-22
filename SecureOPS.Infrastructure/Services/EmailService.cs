using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace SecureOPS.Infrastructure.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public void SendOtpEmail(string receiverEmail, string otp)
        {
            var senderEmail = "secureopsauthenticator@gmail.com";
            var appPassword = "njfhwhdkxauigbmk";

            // 'using' ensures the connection is disposed of properly after sending
            using (var client = new SmtpClient("smtp.gmail.com", 587))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(senderEmail, appPassword);
                client.Timeout = 20000; // Reduced to 20 seconds for snappier feedback

				var mailMessage = new MailMessage
				{
					From = new MailAddress(senderEmail, "Welcome to SecureOPS System , Your Security, Our Priority!!! "),
					Subject = "Your Security, Our Priority!!!" +
                    "" +
                    " Your Security OTP",
					Body = $"<h2>Welcome to SecureOPS</h2><p>Your verification code is: <b>{otp}</b></p>",
					IsBodyHtml = true
				};

                mailMessage.To.Add(receiverEmail);
                client.Send(mailMessage);
            }
        }
    }
}
