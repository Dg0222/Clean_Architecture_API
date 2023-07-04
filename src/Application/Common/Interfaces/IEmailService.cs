namespace CleanArchitecture.Application.Common.Interfaces;

public interface IEmailService
{
    void SendEmailAsync(string email, string subject, string body);
}

