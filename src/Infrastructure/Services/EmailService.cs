using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using System.Text;
using CleanArchitecture.Application.Common.Interfaces;

namespace CleanArchitecture.Infrastructure.Services;

internal class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public EmailService(ILogger<EmailService> logger, IWebHostEnvironment webHostEnvironment)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _webHostEnvironment = webHostEnvironment;
    }

    private SmtpClient GetSmtpClient()
    {

        return new SmtpClient()
        {
            Host = "localhost",
            Port = 2525,
        };

    }

    public void SendEmailAsync(string email, string subject, string body)
    {
        try
        {
            using var smtpClient = GetSmtpClient();

            var from = new MailAddress("test@localhost.com");

            var to = new MailAddress(email);

            var message = new MailMessage(from, to);
            message.Subject = subject;
            message.SubjectEncoding = Encoding.UTF8;

            message.Body = PopulateBody(body);
            message.BodyEncoding = Encoding.UTF8;
            message.IsBodyHtml = true;

            smtpClient.Send(message);
        }
        catch (Exception ex)
        {
            _logger.LogError("SMTP error: {ExInnerException}", ex.InnerException);
            throw;
        }
    }

    private string PopulateBody(string message)
    {
        var webRoot = _webHostEnvironment.WebRootPath;

        var body = "";

        using (var reader = new StreamReader(webRoot + "/EmailTemplates/TestEmailTemplate.html"))
        {
            body = reader.ReadToEnd();
        }

        body = body?.Replace("{MESSAGE}", message);

        return body;
    }
}

