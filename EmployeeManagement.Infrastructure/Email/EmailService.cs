using EmployeeManagement.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace EmployeeManagement.Infrastructure.Email;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendOtpAsync(string email, string firstName, string otp)
    {
        var host = _configuration["SmtpSettings:Host"];
        var port = _configuration.GetValue<int>("SmtpSettings:Port");

        var senderEmail = _configuration["SmtpSettings:SenderEmail"];

        var senderName = _configuration["SmtpSettings:SenderName"];

        var appPassword = _configuration["SmtpSettings:AppPassword"];

        using var client = new SmtpClient(host, port)
        {
            EnableSsl = true,

            Credentials = new NetworkCredential(
                senderEmail,
                appPassword)
        };

        using var message = new MailMessage
        {
            From = new MailAddress(
                senderEmail!,
                senderName),

            Subject = "Verify your email",

            Body = $"""
                    Hello {firstName},

                    Your verification OTP is:

                    {otp}

                    This OTP will expire shortly.

                    If you did not request this verification,
                    please ignore this email.

                    Employee Management System
                    """,

            IsBodyHtml = false
        };

        message.To.Add(email);

        await client.SendMailAsync(message);
    }
}