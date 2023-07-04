using System.Net.Mail;

namespace CleanArchitecture.Application.Common.Extensions;

public static class StringExtensions
{
    public static bool IsValidEmail(this string email)
    {
        try
        {
            var mailAddress = new MailAddress(email);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
