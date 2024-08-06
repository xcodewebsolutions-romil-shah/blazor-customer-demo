using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using SendGrid.Helpers.Mail;
using Customers.Data.Models;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MimeKit.Cryptography;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Customers.Data.Domains;
using Customers.Data;
using Microsoft.EntityFrameworkCore;

namespace Customers.Infrastructure.Helper
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string body, List<string> ccEmails = null);
    }

    public class EmailHelper : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailSettings> _logger;
        private readonly CustomersDBContext _dbContext;

        public EmailHelper(IOptions<EmailSettings> options, ILogger<EmailSettings> logger, CustomersDBContext customersDBContext)
        {
            _emailSettings = options.Value;
            _logger = logger;
            _dbContext = customersDBContext;

        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body, List<string> ccEmails = null)
            {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
                email.To.Add(new MailboxAddress("", toEmail));
                if (ccEmails != null)
                {
                    foreach (var ccEmail in ccEmails)
                    {
                        email.Cc.Add(new MailboxAddress("", ccEmail));
                    }
                }
                email.Subject = subject;
                email.Body = new TextPart("html") { Text = body };

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.Port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
               
                _logger.LogInformation(" Mail Sent Successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending email: {ex.Message}");
                return false;
            }
        }
    }
}

